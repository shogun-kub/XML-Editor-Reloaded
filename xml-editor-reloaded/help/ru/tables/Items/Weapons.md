---
schema: 1
lang: ru
family: items-weapons
title: "Weapons — характеристики оружия"
tables:
  - Weapons
table_ids:
  - Items.Weapons
files:
  - Items/Weapons.xml
root: WEAPONLIST
record: WEAPON
key: uiIndex
sources:
  - Weapon table data
  - Weapon table schema comments
  - Game code (weapon loader and `WEAPONTYPE`)
  - Item metadata and lookup tables
status: reviewed
updated: 2026-11-12
---

# Weapons — характеристики оружия

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Game options](#game-options)
- [Fields](#fields)
  - [`WEAPONLIST/WEAPON/uiIndex`](#weaponlistweaponuiindex)
  - [`WEAPONLIST/WEAPON/szWeaponName`](#weaponlistweaponszweaponname)
  - [`WEAPONLIST/WEAPON/ubWeaponClass`](#weaponlistweaponubweaponclass)
  - [`WEAPONLIST/WEAPON/ubWeaponType`](#weaponlistweaponubweapontype)
  - [`WEAPONLIST/WEAPON/ubCalibre`](#weaponlistweaponubcalibre)
  - [`WEAPONLIST/WEAPON/ubReadyTime`](#weaponlistweaponubreadytime)
  - [`WEAPONLIST/WEAPON/ubShotsPer4Turns`](#weaponlistweaponubshotsper4turns)
  - [`WEAPONLIST/WEAPON/ubShotsPerBurst`](#weaponlistweaponubshotsperburst)
  - [`WEAPONLIST/WEAPON/ubBurstPenalty`](#weaponlistweaponubburstpenalty)
  - [`WEAPONLIST/WEAPON/ubBulletSpeed`](#weaponlistweaponubbulletspeed)
  - [`WEAPONLIST/WEAPON/ubImpact`](#weaponlistweaponubimpact)
  - [`WEAPONLIST/WEAPON/ubDeadliness`](#weaponlistweaponubdeadliness)
  - [`WEAPONLIST/WEAPON/bAccuracy`](#weaponlistweaponbaccuracy)
  - [`WEAPONLIST/WEAPON/ubMagSize`](#weaponlistweaponubmagsize)
  - [`WEAPONLIST/WEAPON/usRange`](#weaponlistweaponusrange)
  - [`WEAPONLIST/WEAPON/usReloadDelay`](#weaponlistweaponusreloaddelay)
  - [`WEAPONLIST/WEAPON/BurstAniDelay`](#weaponlistweaponburstanidelay)
  - [`WEAPONLIST/WEAPON/ubAttackVolume`](#weaponlistweaponubattackvolume)
  - [`WEAPONLIST/WEAPON/ubHitVolume`](#weaponlistweaponubhitvolume)
  - [`WEAPONLIST/WEAPON/sSound`](#weaponlistweaponssound)
  - [`WEAPONLIST/WEAPON/sBurstSound`](#weaponlistweaponsburstsound)
  - [`WEAPONLIST/WEAPON/sSilencedBurstSound`](#weaponlistweaponssilencedburstsound)
  - [`WEAPONLIST/WEAPON/sReloadSound`](#weaponlistweaponsreloadsound)
  - [`WEAPONLIST/WEAPON/sLocknLoadSound`](#weaponlistweaponslocknloadsound)
  - [`WEAPONLIST/WEAPON/SilencedSound`](#weaponlistweaponsilencedsound)
  - [`WEAPONLIST/WEAPON/bBurstAP`](#weaponlistweaponbburstap)
  - [`WEAPONLIST/WEAPON/bAutofireShotsPerFiveAP`](#weaponlistweaponbautofireshotsperfiveap)
  - [`WEAPONLIST/WEAPON/APsToReload`](#weaponlistweaponapstoreload)
  - [`WEAPONLIST/WEAPON/SwapClips`](#weaponlistweaponswapclips)
  - [`WEAPONLIST/WEAPON/MaxDistForMessyDeath`](#weaponlistweaponmaxdistformessydeath)
  - [`WEAPONLIST/WEAPON/AutoPenalty`](#weaponlistweaponautopenalty)
  - [`WEAPONLIST/WEAPON/NoSemiAuto`](#weaponlistweaponnosemiauto)
  - [`WEAPONLIST/WEAPON/EasyUnjam`](#weaponlistweaponeasyunjam)
  - [`WEAPONLIST/WEAPON/APsToReloadManually`](#weaponlistweaponapstoreloadmanually)
  - [`WEAPONLIST/WEAPON/ManualReloadSound`](#weaponlistweaponmanualreloadsound)
  - [`WEAPONLIST/WEAPON/nAccuracy`](#weaponlistweaponnaccuracy)
  - [`WEAPONLIST/WEAPON/bRecoilX`](#weaponlistweaponbrecoilx)
  - [`WEAPONLIST/WEAPON/bRecoilY`](#weaponlistweaponbrecoily)
  - [`WEAPONLIST/WEAPON/ubAimLevels`](#weaponlistweaponubaimlevels)
  - [`WEAPONLIST/WEAPON/ubRecoilDelay`](#weaponlistweaponubrecoildelay)
  - [`WEAPONLIST/WEAPON/Handling`](#weaponlistweaponhandling)
  - [`WEAPONLIST/WEAPON/usOverheatingJamThreshold`](#weaponlistweaponusoverheatingjamthreshold)
  - [`WEAPONLIST/WEAPON/usOverheatingDamageThreshold`](#weaponlistweaponusoverheatingdamagethreshold)
  - [`WEAPONLIST/WEAPON/usOverheatingSingleShotTemperature`](#weaponlistweaponusoverheatingsingleshottemperature)
  - [`WEAPONLIST/WEAPON/HeavyGun`](#weaponlistweaponheavygun)
  - [`WEAPONLIST/WEAPON/fBurstOnlyByFanTheHammer`](#weaponlistweaponfburstonlybyfanthehammer)
  - [`WEAPONLIST/WEAPON/BarrelConfiguration`](#weaponlistweaponbarrelconfiguration)
  - [`WEAPONLIST/WEAPON/szNWSSSound`](#weaponlistweaponsznwsssound)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Weapons.xml` содержит боевые и презентационные характеристики каждого оружия. `Items` указывает на строку оружия через `ubClassIndex`; эта строка задаёт стоимость в ОД, точность, дальность, выбор звука, поведение очереди и панель описания предмета.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Weapons` | `Items/Weapons.xml` | В v4 и v5 одинаковые 48 полей записи. В эталонных данных строки идентичны, кроме одной дополнительной записи в v4; `BarrelConfiguration` — повторяющийся элемент. |

## Structure

```
WEAPONLIST
└── WEAPON
    ├── uiIndex
    ├── szWeaponName
    ├── ubWeaponClass
    ├── ubWeaponType
    ├── ubCalibre
    ├── ubReadyTime
    ├── ubShotsPer4Turns
    ├── ubShotsPerBurst
    ├── ubBurstPenalty
    ├── ubBulletSpeed
    ├── ubImpact
    ├── ubDeadliness
    ├── bAccuracy
    ├── ubMagSize
    ├── usRange
    ├── usReloadDelay
    ├── BurstAniDelay
    ├── ubAttackVolume
    ├── ubHitVolume
    ├── sSound
    ├── sBurstSound
    ├── sSilencedBurstSound
    ├── sReloadSound
    ├── sLocknLoadSound
    ├── SilencedSound
    ├── bBurstAP
    ├── bAutofireShotsPerFiveAP
    ├── APsToReload
    ├── SwapClips
    ├── MaxDistForMessyDeath
    ├── AutoPenalty
    ├── NoSemiAuto
    ├── EasyUnjam
    ├── APsToReloadManually
    ├── ManualReloadSound
    ├── nAccuracy
    ├── bRecoilX
    ├── bRecoilY
    ├── ubAimLevels
    ├── ubRecoilDelay
    ├── Handling
    ├── usOverheatingJamThreshold
    ├── usOverheatingDamageThreshold
    ├── usOverheatingSingleShotTemperature
    ├── HeavyGun
    ├── fBurstOnlyByFanTheHammer
    ├── BarrelConfiguration
    └── szNWSSSound
```

## Game options

`bAccuracy`, `ubBurstPenalty` и `AutoPenalty` относятся к старому шансу попадания (OCTH). `nAccuracy`, `bRecoilX`, `bRecoilY`, `ubAimLevels`, `ubRecoilDelay` и `Handling` относятся к новому шансу попадания (NCTH). В сетевой игре NCTH принудительно отключён, даже если параметр включён. См. [OPTIONS](../../OPTIONS.md#chance-to-hit-ncth-and-octh).

## Fields

### `WEAPONLIST/WEAPON/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс строки оружия в таблице оружия времени выполнения и идентификатор XML-записи. Сюда указывает `Items.ubClassIndex` предмета-оружия.
- **Values:** `0..16000`; загрузчик пишет в `Weapon[MAXITEMS]`, где `MAXITEMS` = `16001`, а строки с большим `uiIndex` молча игнорируются.
- **Default:** отсутствие или пустое значение означает `0`; парсер обнуляет запись, поэтому такая строка сохраняется с индексом `0`.
- **Source:** game code (weapon loader and runtime `Weapon` array); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/szWeaponName`

- **Type:** `String` (`CHAR8`)
- **Description:** Название, хранящееся в `WEAPONTYPE.szWeaponName`. Ни игровой код, ни код интерфейса его не читают; отображаемые названия оружия берутся из главной таблицы предметов.
- **Values:** Свободный текст, усекается до `MAX_WEAPON_NAME_LENGTH` (`20`) символов.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (weapon loader); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubWeaponClass`

- **Type:** `Integer` (`UINT8`)
- **Description:** Широкий класс оружия, используемый фильтрами дилеров и интерфейса, а также сравнениями классов: пистолет, пистолет-пулемёт, дробовик, нож, монстр. Он грубее, чем `ubWeaponType`.
- **Values:** ID из `Lookup.WeaponClass`: `0` None, `1` Handgun, `2` Submachinegun, `3` Rifle, `4` Machinegun, `5` Shotgun, `6` Knife, `7` Monster.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader and `WeaponClass` checks); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubWeaponType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Конкретный тип оружия внутри класса; используется для модификаторов предметов по типу, предпочтений оружия у ИИ и группировки в интерфейсе.
- **Values:** ID из `Lookup.WeaponType`: `0` None, `1` Pistol, `2` Machine Pistol, `3` Submachinegun, `4` Rifle, `5` Sniper Rifle, `6` Assault Rifle, `7` Light Machinegun, `8` Shotgun.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader and `WeaponType` enum); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubCalibre`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс калибра. Сравнивается с `Magazines.ubCalibre` для совместимости боеприпасов и индексирует строки `AmmoCaliber` и `NWSSCaliber` в `AmmoStrings`.
- **Values:** Индекс строки в [AmmoStrings](AmmoStrings.md); `0` — пустой калибр.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; ammunition matching and `AmmoCaliber` lookup); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubReadyTime`

- **Type:** `Integer` (`UINT8`)
- **Description:** Стоимость в ОД на приведение оружия в готовность перед выстрелом. Она отделена от стоимости в ОД самого выстрела.
- **Numeric semantics:** Стоимость в очках действия, а не время. Загрузчик пересчитывает её от базовых 100 ОД к настроенному `AP_MAXIMUM` с помощью `DynamicAdjustAPConstants`; большие значения означают более дорогую подготовку.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; AP constants); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubShotsPer4Turns`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Механическая скорострельность, выраженная в выстрелах за четыре хода. Игра выводит стоимость в ОД из этого значения и настроек ОД оружия; это не прямой счётчик «выстрелов за ход».
- **Numeric semantics:** Большие значения означают более высокую механическую скорострельность (больше выстрелов за четыре хода) и, следовательно, меньшую стоимость в ОД за выстрел в `BaseAPsToShootOrStab`. Эффективное значение должно быть положительным; `0` заставляет расчёт ОД вернуть `-1` (недопустимо).
- **Values:** Десятичное число; в поставляемых данных значения положительны для оружия и равны `0` для строки-заполнителя.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader and AP calculation); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubShotsPerBurst`

- **Type:** `Integer` (`UINT8`)
- **Description:** Базовое число снарядов, выпускаемых одной очередью. Значение времени выполнения — `GetShotsPerBurst`, которое добавляет бонус размера очереди от вложений.
- **Numeric semantics:** Большие значения означают больше снарядов в очереди; `0` означает, что у оружия нет режима очереди, если только его не даёт вложение.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GetShotsPerBurst`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubBurstPenalty`

- **Type:** `Integer` (`UINT8`)
- **Description:** Штраф к точности, применяемый к огню очередями. Вычитается из шанса попадания OCTH один раз за каждый выстрел после первого.
- **Only used with:** OCTH.
- **Numeric semantics:** Большие значения означают более сильный штраф за каждый дополнительный выстрел в очереди; игра уменьшает его на бонус попадания очередью и ограничивает результат снизу нулём.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; OCTH burst accuracy); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubBulletSpeed`

- **Type:** `Integer` (`UINT8`)
- **Description:** Управляет скоростью полёта пули на экране. Игра делит его (плюс бонус скорости пули от вложений и боеприпасов) на `10` и использует результат как интервал обновления на клетку; на урон и вероятность попадания это не влияет.
- **Numeric semantics:** Большее значение даёт более длинный интервал между шагами на одну клетку, поэтому снаряд выглядит медленнее, несмотря на название поля. Значение `0` даёт почти мгновенную визуальную пулю.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; bullet travel interval); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubImpact`

- **Type:** `Integer` (`UINT8`)
- **Description:** Базовый урон оружия до бонусов к урону от предмета, боеприпасов и вложений, модификаторов дальности и снижения бронёй.
- **Numeric semantics:** Большие значения означают более сильное базовое попадание; итоговый урон ограничивается `255` в `GetDamage`.
- **Values:** `0..250`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GetDamage`/`GetBasicDamage`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubDeadliness`

- **Type:** `Integer` (`UINT8`)
- **Description:** Сравнительная оценка смертоносности, используемая для привлекательности оружия у наёмника, оценки угрозы ИИ и оценки стоимости предмета. Это не прямое значение урона.
- **Numeric semantics:** Большие значения означают более желательное или более угрожающее оружие; значение — это рейтинг, а не величина урона.
- **Values:** `0..250`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; mercenary profile and AI threat code); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/bAccuracy`

- **Type:** `Integer` (`INT8`)
- **Description:** Значение точности для старого шанса попадания (OCTH). `nAccuracy` — аналог для NCTH.
- **Only used with:** OCTH.
- **Numeric semantics:** Большие значения делают оружие точнее; меньшие — менее точным, а отрицательные значения действуют как штраф к точности.
- **Values:** `-100..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; OCTH hit calculation); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubMagSize`

- **Type:** `Integer` (`UINT16`)
- **Description:** Базовая ёмкость магазина, используемая при заряжании оружия. `GetMagSize` добавляет бонус магазина от вложений; это поле описывает магазин оружия, а не строку предмета-магазина.
- **Numeric semantics:** Большие значения означают большую ёмкость; `0` оставляет оружие зависимым от магазина, предоставляемого вложением.
- **Values:** `0..65000`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GetMagSize`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/usRange`

- **Type:** `Integer` (`UINT16`)
- **Description:** Эффективная дальность в мировых единицах, где `10` единиц равны одной клетке. `GunRange` и `InRange` сравнивают её с расстоянием после применения бонусов к дальности.
- **Numeric semantics:** Большие значения означают большую дальность; `0` делает базовую дальность нулевой, хотя `GunRange` всё ещё может добавлять фиксированные бонусы к дальности.
- **Values:** `0..5000`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GunRange`/`GetModifiedGunRange`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/usReloadDelay`

- **Type:** `Integer` (`UINT16`)
- **Description:** Задержка анимации перезарядки. Игра прибавляет её к `MANDATORY_WEAPON_DELAY`, когда задаёт задержку перезарядки солдата; это значение представления и тайминга, а не стоимость перезарядки в ОД.
- **Numeric semantics:** Значение времени; большие значения означают более длинную анимацию перезарядки.
- **Values:** `0..500`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; reload animation timing); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/BurstAniDelay`

- **Type:** `Integer` (`INT16`)
- **Description:** Задержка анимации на кадр, используемая во время проигрывания анимации огня очередями. Хранится в `WEAPONTYPE.sAniDelay` и копируется в задержку анимации солдата.
- **Numeric semantics:** Значение времени; большие значения проигрывают анимацию очереди медленнее.
- **Values:** `0..500`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; soldier animation delay); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubAttackVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Громкость шума, создаваемого выстрелом из оружия. Она участвует в распространении звука и проверках слуха ИИ через `MakeNoise`.
- **Numeric semantics:** Большие значения означают более громкий выстрел; `0` делает выстрел беззвучным для системы шума.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; firing noise); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubHitVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Громкость шума при попадании пули в цель или постройку; передаётся в `MakeNoise` с `NOISE_BULLET_IMPACT`. Это не значение разрешения попадания и не точность.
- **Numeric semantics:** Большие значения делают попадания пуль громче для системы шума; `0` делает их беззвучными.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; bullet-impact noise); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/sSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID звука, проигрываемого для обычного выстрела. `0` (`NO_WEAPON_SOUND`) подавляет звук выстрела.
- **Values:** ID звука; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; shot sound selection); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/sBurstSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс в шаблонах имён файлов `gzBurstSndStrings[]`, используемых для звука очереди в исходной (не NWSS) звуковой системе. Это не ID звука `PlayJA2Sample`; `0` означает отсутствие звука очереди.
- **Values:** Индекс в таблице шаблонов звука очереди; `0` отключает звук очереди.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; original burst sound path); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/sSilencedBurstSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс в шаблонах имён файлов `gzBurstSndStrings[]`, используемых для звука беззвучной очереди в исходной (не NWSS) звуковой системе. Это не ID звука `PlayJA2Sample`; `0` означает отсутствие звука беззвучной очереди.
- **Values:** Индекс в таблице шаблонов звука очереди; `0` отключает звук беззвучной очереди.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; original silenced burst sound path); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/sReloadSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID звука, проигрываемого при перезарядке. `0` подавляет звук перезарядки.
- **Values:** ID звука; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; reload sound selection); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/sLocknLoadSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID звука, проигрываемого при досылании патрона или приведении оружия в готовность. `0` подавляет звук.
- **Values:** ID звука; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; lock-and-load animation); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/SilencedSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID звука, проигрываемый вместо `sSound`, когда выстрел считается беззвучным (шум ниже заданного порога или `ubAttackVolume` не больше `10`). Игра проигрывает его напрямую, без проверки на `0` и без отката к `sSound`.
- **Values:** ID звука; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; silenced shot sound); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/bBurstAP`

- **Type:** `Integer` (`UINT8`)
- **Description:** Базовая стоимость очереди в ОД. Загрузчик пересчитывает её от базовых 100 ОД к `AP_MAXIMUM`, а `CalcAPsToBurst` масштабирует её по очкам действия стрелка и применяет снижения стоимости очереди в ОД.
- **Numeric semantics:** Стоимость в очках действия; большие значения означают более дорогую очередь.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `CalcAPsToBurst`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/bAutofireShotsPerFiveAP`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число выстрелов автоматического огня на пять ОД. Это значение темпа, а не ёмкость магазина; `0` отключает автоматический огонь.
- **Numeric semantics:** Большие значения означают более высокий темп автоматического огня. Игра умножает значение на модификатор INI для типа, добавляет `bAutofireBulletsPer5APModifier` и ограничивает результат снизу единицей, если базовое значение положительно.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GetAutofireShotsPerFiveAPs` and autofire AP cost); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/APsToReload`

- **Type:** `Integer` (`UINT8`)
- **Description:** Базовая стоимость обычной перезарядки в ОД. Загрузчик пересчитывает её к `AP_MAXIMUM`; `GetAPsToReload` применяет модификатор перезарядки для типа и снижение времени перезарядки.
- **Numeric semantics:** Стоимость в очках действия; большие значения означают более дорогую перезарядку.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GetAPsToReload`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/SwapClips`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлен, перезарядка магазином того же размера и с теми же боеприпасами в пошаговом бою использует смену магазина (`RELOAD_SWAP`) вместо дозарядки оружия патрон за патроном; магазин неподходящего размера тогда использует стоимость `AP_WRONG_MAG`. Если снят, такая же перезарядка дозаряжает оружие; другой тип боеприпаса всё равно меняет магазин.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; reload-type selection); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/MaxDistForMessyDeath`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальная дистанция, на которой попадания в голову и торс могут вызвать особые эффекты мгновенного убийства и кровавой смерти (взрыв головы, взрыв груди). За пределами этой дистанции эти эффекты не срабатывают.
- **Numeric semantics:** Большие значения допускают эффекты на большей дистанции; `0` ограничивает их той же клеткой. Для оружия значение масштабируется модификатором дистанции для типа оружия перед проверкой дальности.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `CalcChanceToHitGun` hit-location specials); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/AutoPenalty`

- **Type:** `Integer` (`UINT8`)
- **Description:** Штраф к точности, применяемый к автоматическому огню. Вычитается из шанса попадания OCTH один раз за каждый дополнительный выстрел в серии.
- **Only used with:** OCTH.
- **Numeric semantics:** Большие значения означают более сильный штраф; `0` означает отсутствие штрафа за автоматический огонь. Игра уменьшает его на бонус попадания автоматическим огнём и ограничивает результат снизу нулём.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; OCTH autofire accuracy); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/NoSemiAuto`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлен, у оружия нет полуавтоматического режима огня; переключение на него выбирает автоматический огонь.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; weapon-mode selection); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/EasyUnjam`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлен, устранение заклинивания использует фиксированный модификатор проверки навыка `+100` вместо модификатора по надёжности, поэтому проверка ограничена `99%` и фактически всегда успешна. За устранение заклинивания такого оружия опыт не начисляется.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; unjam skill check); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/APsToReloadManually`

- **Type:** `Integer` (`UINT8`)
- **Description:** Стоимость в ОД на досылание или передёргивание патрона, когда в оружии есть боеприпасы, но патрон не в патроннике (ручное действие). Это не стоимость полной перезарядки.
- **Numeric semantics:** Стоимость в очках действия; загрузчик пересчитывает её к `AP_MAXIMUM`, а игра применяет модификатор ручной перезарядки для типа и снижения от черт «Снайпер» и «Рейнджер».
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GetAPsToAutoReload` chambering path); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ManualReloadSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID звука, проигрываемого при ручном досылании или передёргивании патрона.
- **Values:** ID звука; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; manual-reload sound); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/nAccuracy`

- **Type:** `Integer` (`INT8`)
- **Description:** Значение точности для нового шанса попадания (NCTH). `bAccuracy` — аналог для OCTH.
- **Only used with:** NCTH.
- **Numeric semantics:** Большие значения делают оружие точнее; меньшие — менее точным, а отрицательные значения действуют как штраф к точности.
- **Values:** `-128..127`; загрузчик сохраняет это поле как `INT8`, и метаданные поля не объявляют более узкого диапазона.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; NCTH accuracy); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/bRecoilX`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Горизонтальное отклонение отдачи, используемое NCTH. Положительные значения уводят вправо; итоговое значение ограничивается снизу нулём, поэтому отрицательное заданное значение не даёт увода влево.
- **Only used with:** NCTH.
- **Numeric semantics:** Большее значение труднее контролировать. В поставляемых данных значения неотрицательны; метаданные допускают `-20..20`, но `GetRecoil` ограничивает результат нулём.
- **Values:** `-20..20` в метаданных; наблюдаемые значения — `0..4`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GetRecoil`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/bRecoilY`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Вертикальное отклонение отдачи, используемое NCTH. Положительные значения уводят вверх; итоговое значение ограничивается снизу нулём, поэтому отрицательное заданное значение не даёт увода вниз.
- **Only used with:** NCTH.
- **Numeric semantics:** Большее значение труднее контролировать. В поставляемых данных значения неотрицательны; метаданные допускают `-20..20`, но `GetRecoil` ограничивает результат нулём.
- **Values:** `-20..20` в метаданных; наблюдаемые значения — `0..14`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GetRecoil`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubAimLevels`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число уровней прицеливания, доступных оружию в NCTH. Значение вне `1..8` заставляет игру вернуться к алгоритму по классу и дальности; результат времени выполнения всегда ограничен `1..8`.
- **Only used with:** NCTH.
- **Numeric semantics:** Большие значения допускают больше уровней прицеливания (точнее прицеливание); `0` или любое значение выше `8` означают автоматический выбор. Неогнестрельное оружие использует фиксированную базу `2`, а черта «Метание» может ещё уменьшить значение для метательного ножа (минимум `1`).
- **Values:** `0..8`; `0` означает автоматический выбор.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `AllowedAimingLevelsNCTH`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubRecoilDelay`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число первых пуль в серии, не дающих отдачи. `GetRecoil` возвращает ноль, пока номер пули не больше этого значения.
- **Only used with:** NCTH.
- **Numeric semantics:** Большие значения делают первые выстрелы очереди без отдачи; `0` оставляет без отдачи только первую пулю.
- **Values:** `0..5` в метаданных; наблюдаемые значения — `0`, `2` и `3`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GetRecoil`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/Handling`

- **Type:** `Integer` (`UINT8`)
- **Description:** Сложность обращения с оружием, используемая NCTH. Большие значения увеличивают слагаемое сложности оружия в `CalcNewChanceToHitGun`, что снижает шанс попадания.
- **Only used with:** NCTH.
- **Numeric semantics:** Большие значения означают оружие, которым труднее управлять; меньшие — проще.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; NCTH handling difficulty); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/usOverheatingJamThreshold`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Температура, при которой оружие начинает заклинивать в модели перегрева. Игра сравнивает её с `bTemperature` и возвращает отношение как процент заклинивания.
- **Numeric semantics:** Значение температуры в шкале `0..OVERHEATING_MAX_TEMPERATURE` (`60000`); большие значения допускают больше нагрева до заклинивания.
- **Values:** `0..60000`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GetOverheatJamThreshold`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/usOverheatingDamageThreshold`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Температура, при которой оружие начинает получать повреждения в модели перегрева. Игра сравнивает её с `bTemperature` и возвращает отношение как процент повреждений.
- **Numeric semantics:** Значение температуры в шкале `0..OVERHEATING_MAX_TEMPERATURE` (`60000`); большие значения допускают больше нагрева до повреждений.
- **Values:** `0..60000`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GetOverheatDamageThreshold`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/usOverheatingSingleShotTemperature`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Температура, добавляемая одним выстрелом в модели перегрева. Вложения и боеприпасы могут изменять её при использовании.
- **Numeric semantics:** Значение температуры в шкале `0..OVERHEATING_MAX_TEMPERATURE` (`60000`); большие значения добавляют больше нагрева за выстрел.
- **Values:** `0..60000`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `GetSingleShotTemperature`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/HeavyGun`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Помечает двуручное оружие как тяжёлое. Стоя из него нужно стрелять от бедра (альтернативное удержание оружия), и его нельзя упереть в плечо; флаг используется логикой альтернативного удержания, а не проверками силы или сошек.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; alternative weapon-holding and aim-level logic); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/fBurstOnlyByFanTheHammer`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлен, режим очереди доступен только владельцу черты GUNSLINGER, использующему оружие от бедра. Это условие — часть правил режимов огня оружия, а не настройка интерфейса: требуются новая система черт, параметр fan-the-hammer, черта GUNSLINGER, альтернативное удержание оружия и пустая вторая рука.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (weapon loader; `IsGunBurstCapable` and alternative holding); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/BarrelConfiguration`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число стволов/снарядов в одной конфигурации стволов. Элемент может повторяться, описывая несколько конфигураций; каждое ненулевое значение — выбираемое число стволов. Используется только оружием с классом `IC_BOBBY_GUN`.
- **Values:** `1..255`; загрузчик пропускает записи `0`. Если ненулевых значений нет, предполагается один ствол.
- **Default:** отсутствие или все нули означают один ствол; повторяющиеся элементы добавляют конфигурации стволов.
- **Source:** game code (weapon loader; `GetFittingBarrelMode`/`GetNextBarrelMode`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/szNWSSSound`

- **Type:** `String` (`CHAR8`)
- **Description:** Базовое имя, используемое новой звуковой системой оружия. Игра строит имя файла из калибра, этого имени и варианта выстрела (`loop`, `single` или вариант с помещением/эхом); если имя пустое или файл не существует, происходит откат к имени типа оружия, а затем к общему файлу.
- **Values:** Свободный текст, усекается до `MAX_WEAPON_NAME_LENGTH` (`20`) символов.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (weapon loader; NWSS filename construction); weapon table data.
- **Confidence:** `source`

## Relations

- `ubClassIndex` предмета-оружия из [Items](Items.md) указывает на `Weapons.uiIndex`.
- `ubCalibre` указывает на [AmmoStrings](AmmoStrings.md) и сравнивается с `Magazines.ubCalibre`; подпись калибра — отображаемые данные, а поведение боеприпасов задаётся [AmmoTypes](AmmoTypes.md) и строкой [Magazines](Magazines.md).
- `Magazines.ubAmmoType` выбирает поведение боеприпаса, загружаемое магазином; `Weapons.ubCalibre` выбирает только семейство калибра.
- Поля NCTH читаются только когда активна новая система шанса попадания; OCTH использует `bAccuracy`, `ubBurstPenalty` и `AutoPenalty`.

## Related formulas

- [Урон от боеприпасов и оружия](../../FORMULAS.md#ammo-and-weapon-damage-dealt)
- [Защита брони](../../FORMULAS.md#armour-protection)

## Localization

Подпись калибра берётся из `AmmoStrings` и локализуется там. `szWeaponName` сохраняется, но не отображается; отображаемые названия оружия находятся в главной таблице предметов. Числовые ID звуков и справочников не локализуются.

## Versions

В v4 и v5 одинаковые 48 путей полей. `BarrelConfiguration` повторяется в обеих версиях. В эталонных данных значения идентичны, кроме одной дополнительной записи оружия в v4.

## Sources

- Данные таблицы оружия.
- Комментарии схемы таблицы оружия.
- Код игры (загрузчик оружия, структура таблицы оружия, функции ОД, точности, перегрева и звука).
- Метаданные предметов и справочные таблицы.
- Типы и пределы полей: метаданные предметов; числовые пределы используют диапазоны проверки метаданных.
- Семантические описания используют комментарии схемы таблицы и код игры; записи с пометкой `inferred` не заявляют формулу, проверенную по коду.

## Open questions

Неразрешённой семантики полей не осталось. Единственная странность на уровне проектирования — `ubBulletSpeed`: код использует значение как интервал обновления на клетку (большие значения замедляют визуальный снаряд), что противоположно смыслу названия поля.
