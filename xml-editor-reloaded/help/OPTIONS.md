# Game options that change field meaning

Some TableData fields are read only when an optional game system is enabled. This page
records those switches and points to the fields they affect. The game option file and the
runtime code remain the authority; the table documents describe the effect on a field.

## Contents

- [Chance to Hit: NCTH and OCTH](#chance-to-hit-ncth-and-octh)
- [Inventory: OIV and NIV](#inventory-oiv-and-niv)
- [Other option-gated fields](#other-option-gated-fields)
- [What is not a global option](#what-is-not-a-global-option)

## Chance to Hit: NCTH and OCTH

The game has two aiming models:

- **OCTH** - old Chance to Hit, the original JA2 calculation.
- **NCTH** - new Chance to Hit, selected by the `NCTH` option in the game option file.
  `NCTH = TRUE` enables it; `FALSE` keeps OCTH.

NCTH-only fields in `Items`:

- `ScopeMagFactor`
- `ProjectionFactor`
- `RecoilModifierX`
- `RecoilModifierY`
- `PercentRecoilModifier`
- `PercentAccuracyModifier`
- the three stance groups `STAND_MODIFIERS`, `CROUCH_MODIFIERS` and `PRONE_MODIFIERS`

The weapon table has its own NCTH fields (accuracy, recoil and aiming data); they are
documented in the future `Weapons` document. Do not apply an NCTH-only field to OCTH balance
without checking the option state.

## Inventory: OIV and NIV

The game has two inventory models:

- **OIV** - old inventory, the original JA2 slot/pocket model.
- **NIV** - new inventory, the 1.13 LBE/pocket model.

The supplied 1.13 data set is NIV. The `OLD_INVENTORY_ITEM_NUMBER` option maps items that do
not appear in NIV; it is a compatibility aid, not a replacement for the item table. Legacy
OIV data remains readable.

Fields affected in `Items`:

- `ubPerPocket` - OIV pocket capacity. NIV derives pocket capacity from the `Pockets` and
  `LoadBearingEquipment` tables, so treat `ubPerPocket` as an OIV compatibility field.
  The OIV-only reading is inferred from the new-inventory tables and the legacy item-editor
  grouping; the field entry in `Items.md` is marked `inferred` for the same reason.
- `NewInv` - marks an item as NIV-only; OIV does not expose it.
- `ItemSize` - the item's size class. It is part of the item definition in both models, but
  the pocket checks that consume it differ between OIV and NIV.

Other OIV/NIV details live in the `Pockets`, `PocketPopups` and `LoadBearingEquipment`
documents (B3).

## Other option-gated fields

| Option | Field | Effect |
|---|---|---|
| Sci-Fi mode | `Items.SciFi` | Marks the item as part of the Sci-Fi item set. |
| Tons of Guns | `Items.BigGunList` | Includes the item in the expanded gun list. |
| Disease system | `Items.DiseaseSystemExclusive` | Marks an item that belongs to the disease system. |

These are item flags, not global switches stored in the table; the option decides whether the
flagged item is available or active.

## What is not a global option

`AttachmentSystem`, `AttachmentClass`, `nasAttachmentClass` and `nasLayoutClass` are
per-item attachment-system data. They select OAS/NAS behaviour for one item; they are not
global game options. The OAS/NAS distinction is documented with the attachment fields.

## Sources

- Game option definitions for `NCTH`, `OLD_INVENTORY_ITEM_NUMBER` and the feature switches.
- Item table data and the legacy item-editor option grouping.
- The inventory and attachment tables that receive the values.

No code file or line reference is used here; the option names and field effects are the
stable anchors.
