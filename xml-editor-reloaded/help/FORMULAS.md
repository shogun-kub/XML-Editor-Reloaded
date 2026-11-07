# Related formulas

This page holds the multi-stage calculations that do not belong in a single field entry.
It is written for modders and for the future context help; the C++ code remains the
authority. All arithmetic below is integer arithmetic unless a value is marked `FLOAT`.

## Contents

- [Reading the formulas](#reading-the-formulas)
- [Armour protection](#armour-protection)
- [Ammo and weapon damage dealt](#ammo-and-weapon-damage-dealt)
- [Implementation map](#implementation-map)

## Reading the formulas

- `trunc` means C++ integer division; fractions are discarded at that step.
- `max(1, divisor)` means the code guards against a zero divisor. The XML data should use
  non-zero divisors, but the formula is written as the code executes it.
- `PreRandom(n)` returns an integer from `0` to `n-1`; `Random(n)` behaves the same way in
  these functions.
- The formulas describe the default code path. Options, difficulty settings, traits and
  target type can add branches; those branches are named where they matter.
- The order of operations matters. The same numbers applied in a different order can produce
  a different integer result.

## Armour protection

The game resolves a hit against each armour item separately and then subtracts the total
from the bullet impact. The relevant functions are `ArmourProtection` and
`TotalArmourProtection`.

### Inputs for one armour item

| Symbol | Meaning | Source |
|---|---|---|
| `P0` | raw protection | `Items.Armours.ubProtection` |
| `C` | coverage, percent | `Items.Armours.ubCoverage` |
| `S` | current condition, 0..100 | item `objectStatus` |
| `D` | wear per hit, percent | `Items.Armours.ubDegradePercent` |
| `I` | incoming impact | bullet impact after the ammo life factor and the random/accuracy step |
| `K` | armour class | `Items.Armours.ubArmourClass` |

### Coverage and condition

The code first checks whether the armour covers the hit at all:

```
r1 = PreRandom(100) + 1
if r1 > C:
    if K == VEST and not AmmoTypes[type].ignoreArmour:
        return I / 2
    return 0
```

So `C = 100` always covers. A vest still halves the impact on a coverage miss unless the
ammo ignores armour; other classes return zero protection on a miss.

If the armour is hit, the code checks for a weak spot using the current condition:

```
r2 = PreRandom(100) + 1
if r2 > S:
    P0 = P0 - (r2 - S)
    if P0 < 0:
        if K == PLATE: plateHit = true
        return 0
```

A damaged item therefore loses protection in two ways: its condition lowers the weak-spot
threshold, and the failed roll directly reduces `P0`.

`plateHit` starts as `false` and is set when a plate is hit or when a plate's protection
fails. It forces `C = 100` for the next plate check, so a plate that is already in the hit
path does not roll coverage again.

### Ammo adjustment and applied protection

```
P = trunc(P0 * AmmoTypes[type].armourImpactReductionMultiplier
                / max(1, AmmoTypes[type].armourImpactReductionDivisor))
```

`AmmoTypes` is `Items.AmmoTypes`; the two divisor fields are integers, not percentages.
The result `P` is the protection actually offered by the armour after the ammo's penetration
modifier.

The code then chooses the amount used for wear:

```
if P > I:
    A = I
else:
    A = P0
```

When the round is stopped (`P > I`), only the impact is applied. When the round is not
stopped, the code uses the raw protection `P0` for the wear calculation, not the
ammo-adjusted `P`.

### Wear and special ammo

```
if AmmoTypes[type].acidic:
    S = S - 3 * A * D / 100
    P = P / 2
else if AmmoTypes[type].ignoreArmour and (K == VEST or K == LEGGINGS):
    P = 0

S = S - A * D / 100
if K == PLATE: plateHit = true
return P
```

Acidic ammo does four times the normal wear (an extra `3x`) and halves the protection after
the wear step. `ignoreArmour` zeroes vest and leggings protection after the wear calculation,
so the armour still loses condition even when it does not stop the hit.

### Which armour is checked

`TotalArmourProtection` selects the slot from the hit location:

| Hit location | Slot |
|---|---|
| head | helmet |
| legs | leggings |
| torso / default | vest |
| gland | none; returns 0 |

For a torso hit it can also use armour attached to the vest pack (for example a plate
carrier). The code takes the first `IC_ARMOUR` attachment with condition above zero, adds its
protection (multiplied by 3 for a flak jacket when fragments are considered), and then checks
the main armour and its first armour attachment if the impact is still greater than the
protection accumulated so far. The main armour itself is added last. This is why a vest plus
a plate can stop more than either item alone.

The final result for the hit is:

```
totalProtection = TotalArmourProtection(...)
impactAfterArmour = impactBeforeArmour - totalProtection
```

The minimum-damage rule in [Ammo and weapon damage dealt](#ammo-and-weapon-damage-dealt)
then applies. A plate can also break when its condition drops below `USABLE`.

### Where it is implemented

- `Tactical/Weapons.cpp:7860-7951` - `ArmourProtection` (coverage, condition, ammo, wear).
- `Tactical/Weapons.cpp:7954-8089` - `TotalArmourProtection` (slot, vest pack, plates, flak).
- `Tactical/Weapons.cpp:8238-8258` - subtraction and minimum damage.
- `Tactical/Weapons.h:221-245` - ammo multipliers and flags.
- `Items.Armours` fields: `ubProtection`, `ubCoverage`, `ubDegradePercent`,
  `ubArmourClass`; see [Items.Armours](tables/Items/Armours.md).

## Ammo and weapon damage dealt

This is a pipeline, not one closed formula. The output of each stage is the input of the
next; the names below follow the code.

### 1. Base weapon damage

For a gun, `GetDamage` starts from the weapon's `ubImpact` and applies the gun-type damage
modifier, the item damage bonus and the global gun-damage modifier:

```
B0 = Weapon[item].ubImpact
B1 = trunc(B0 * fDamageModifierGun[Weapon[item].ubWeaponType])
B2 = B1 + GetDamageBonus(weapon)
B  = min(255, GetModifiedGunDamage(B2))
```

`ubImpact` is the `Items.Weapons.ubImpact` field; `fDamageModifierGun` is an item-setting
multiplier by weapon type. The exact composition of `GetDamageBonus` is attachment- and
trait-dependent; it is added before `GetModifiedGunDamage`, not after.

### 2. Multiple projectiles

If the ammo fires more than one projectile, the base damage is divided between them before
the hit:

```
if AmmoTypes[type].numberOfBullets > 1:
    B = trunc(B * AmmoTypes[type].multipleBulletDamageMultiplier
                / max(1, AmmoTypes[type].multipleBulletDamageDivisor))
```

### 3. Ammo and target factor

```
F = AmmoTypes[type].dDamageModifierLife
if target is zombie:
    F = F * dDamageModifierZombie
else if target is vehicle or robot:
    if jeep / robot: F = F * dDamageModifierArmouredVehicle
    else if tank:    F = F * dDamageModifierTank
    else:            F = F * dDamageModifierCivilianVehicle
if target is a robot with a chassis damage-reduction modifier in (0, 1):
    F = F * fRobotDamageReductionModifier
```

The robot chassis branch also degrades the chassis plate by `max(1, originalImpact / 12)`
before the damage factor is used.

### 4. Random fluke and accuracy bonus

```
I0 = trunc(B * F)
fluke = PreRandom(51) - 25          // -25 .. +25
bonus = sHitBy / 2                  // up to +50
I1 = I0 * (100 + fluke + bonus) / 100
if I1 < 1: I1 = 1
```

`sHitBy` is the hit margin carried by the bullet; it is not the final chance to hit. The
fluke represents organ hits, a lucky lighter and similar random events.

### 5. High-explosive ammo

```
if AmmoTypes[type].highExplosive:
    I1 = trunc(I1 * beforeArmourDamageMultiplier
                  / max(1, beforeArmourDamageDivisor))
    if target is tank: I1 = I1 / 2
```

### 6. Armour subtraction

```
armour = TotalArmourProtection(target, hitLocation, I1, ammoType, fragments)
I2 = I1 - armour
```

If the hit is a blinded-by-spit special, the armour step is skipped and `I2 = I1`.

### 7. Minimum damage

```
if AmmoTypes[type].zeroMinimumDamage:
    I2 = max(0, I2)
else:
    I2 = max(I2, (I1 + 5) / 10)
    if numberOfBullets > 1 and pelletsHitBy > 0:
        I2 = I2 + (pelletsHitBy - 1) / 2
```

The default floor is 10% of the pre-armour impact, rounded up by integer division. Ammo
with `zeroMinimumDamage` can do zero damage, but it still passes through the later stages.

### 8. After-armour ammo factor

```
I3 = trunc(I2 * afterArmourDamageMultiplier
              / max(1, afterArmourDamageDivisor))
```

### 9. Trait bonuses

The new trait system can add damage after the ammo factor:

- throwing knives with the throwing trait: a percentage of `I3` based on the trait value and
  aim time;
- sniper trait: a percentage of `I3` per aim click above the configured threshold.

These bonuses are added to `I3`, not multiplied into the base weapon damage.

### 10. Damage resistance

```
I4 = max(0 or 1, trunc(I3 * (100 - target.GetDamageResistance(...)) / 100))
```

The floor is `0` for `zeroMinimumDamage` ammo and `1` otherwise. `GetDamageResistance`
combines the target's armour/robot/creature resistances and the current difficulty/option
state; it is not only the armour value from `Items.Armours`.

### 11. Hit location and criticals

`AdjustImpactByHitLocation` runs after resistance. It can add critical damage for head,
torso and leg hits and produces a separate critical value used by the wound/stat-loss code.
Drug effects, cryo, knockdown and other special effects run after this core pipeline.

### Where it is implemented

- `Tactical/Weapons.cpp:10804-10868` - `GetDamage` (base weapon/melee damage).
- `Tactical/LOS.cpp:4887-4902` - base impact and multiple-projectile divisor.
- `Tactical/Weapons.cpp:8156-8264` - ammo factor, fluke, HE, armour subtraction, minimum.
- `Tactical/Weapons.cpp:8299-8334` - after-armour factor, traits, resistance, hit location.
- `Tactical/Weapons.h:214-262` - `AmmoTypes` fields.
- [Items.Armours](tables/Items/Armours.md), [Items.Magazines](tables/Items/Magazines.md),
  `Items.AmmoTypes`, `Items.Weapons` - the data fields used above.

## Implementation map

| Formula | Entry point | Main data |
|---|---|---|
| Armour protection | `Tactical/Weapons.cpp:7860` | `Items.Armours`, `Items.AmmoTypes` |
| Total armour | `Tactical/Weapons.cpp:7954` | `Items.Armours`, `Items.Items` |
| Weapon base damage | `Tactical/Weapons.cpp:10804` | `Items.Weapons`, `Items.Items` |
| Bullet damage | `Tactical/Weapons.cpp:8092` | `Items.AmmoTypes`, `Items.Weapons`, `Items.Armours` |

## Open questions

- The formulas above describe the code paths used by ordinary guns. Melee, explosions,
  structures and vehicle-specific branches are separate and are not documented here.
- `GetDamageBonus` and `GetModifiedGunDamage` are named as stages, not expanded into every
  attachment and trait contribution; those contributions belong to `Items.Items` and
  `Items.Weapons` once those tables are documented.
- `GetDamageResistance` is a target-side aggregate; it will be documented with the soldier
  profile/armour tables if the user asks for it.
