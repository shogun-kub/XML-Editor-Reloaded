---
name: m30-table-help
description: M30 TableData help — B0–B8 approved 2026-11-08; all ninety-four documents reviewed, 239 tables, 2985 field paths, help-check tooling, clean table names, table_ids and option-gated fields.
metadata:
  node_type: memory
  type: project
---

**M30 (2026-11-07).** B0 справки TableData одобрен пользователем. Шесть документов в
`xml-editor-reloaded/help/tables/`: `Lookup.md`, `Items/Armours.md`, `Items/Magazines.md`,
`Items/Launchables.md`, `Map/Cities.md`, `Map/Facilities.md` — 28 таблиц, 78 полей. Общие
документы: `README.md`, `SOURCES.md`, `GLOSSARY.md`, `Localization.md`, `FORMULAS.md`
(одобрен отдельно). `INDEX.md` и `COVERAGE.md` генерируются.

**Формат.** В body — чистые имена таблиц (`Armours`, `Weapons`), точка означает поле
(`Weapons.ubWeaponClass`). Во front matter `tables` — чистые имена, `table_ids` — канонические
ID (`Items.Weapons`) только для `help-check`; `files` — относительные пути с расширением.
В таблицах документов две колонки: чистое имя и `File`.

**Инструменты.** `util-scripts/help-inventory.ps1` строит инвентарь v5 с fallback v4;
`help-check.ps1` проверяет front matter, поля, ссылки/анкоры, запрещает code file/line
references, с `-Write` обновляет Contents/INDEX/COVERAGE, с `-Release` требует все таблицы.
После одобрения B0 документы в статусе `reviewed`.

**Ограничение.** DSH preview не ставит `id` заголовкам, поэтому TOC-анкоры там не кликаются;
в GitHub/VS Code и в будущем окне справки они валидны.

**B1a (2026-11-07).** `Items/Items.md` одобрен: 254 скалярных поля + 3 structural
container, 257/257 путей, `status: reviewed`. В документе нет row counts и наблюдаемых
диапазонов; `Values` — только code/type/metadata limits и lookup lists. Есть `Game options`
(NCTH/OCTH, OIV/NIV, SciFi, Tons of Guns, disease); `ubPerPocket` проверен по C++:
OIV capacity, в NIV не capacity, но читается для `small`; `ItemSize == OLD_INVENTORY_ITEM_NUMBER`
— OIV-only marker. У опциональных полей есть отдельная строка `Only used with: ...`.

**B1b (2026-11-07, approved).** Пять документов: `Items/Weapons.md` (48), `AmmoTypes.md`
(38), `AmmoStrings.md` (4), `Explosives.md` (19), `ExplosionData.md` (8) — 117 путей.
`help-check -Write` проходит (452/2985); row counts и наблюдаемые диапазоны не добавлены.
NCTH/OCTH разведены в `Weapons`; `AmmoTypes` опирается на schema-комментарий; связи
`Explosives.ubAnimationID` → `ExplosionData`, `ubFragType` → `AmmoTypes`; у OCTH/NCTH-полей
стоит отдельная строка `Only used with: ...`. Одобрены 2026-11-07, `reviewed`.

**B2 (2026-11-07, reviewed).** 12 документов: `Attachments` (4), `AttachmentSlots` (9),
`AttachmentInfo` (5), `AttachmentComboMerges` (23), `IncompatibleAttachments` (2),
`CompatibleFaceItems` (2), `Merges` (6), `Item_Transformations` (15),
`StructureConstruct` (10), `StructureDeconstruct` (7), `StructureMove` (3), `Clothes` (4) —
90 путей. `help-check -Write` проходит (542/2985); row counts не добавлены. У NAS-полей
`Only used with: NAS`; повторяющиеся `allowedtile`/`*facingtile` описаны. Одобрены пользователем 2026-11-07, переведены в `reviewed`.

**B3 (2026-11-07, reviewed).** 7 документов: `Pockets` (41), `PocketPopups` (8),
`LoadBearingEquipment` (18), `Food` (6), `FoodOpinion` (3), `Drugs` (16), `RandomItem` (22) —
114 путей. `help-check -Write` проходит (656/2985). NIV/food/disease-поля помечены
`Only used with`; capacity/random slots сгенерированы. Одобрены пользователем 2026-11-07, переведены в `reviewed`.

**B4 (2026-11-07, reviewed).** 11 документов: `MercProfiles` (106), `MercStartingGear` (67),
`MercOpinions` (3), `MercQuote` (9), `MercAvailability` (10), `AIMAvailability` (4),
`FaceGear` (3), `RPCFacesSmall` (7), `IMPPortraits` (12), `IMPVoices` (3), `OldAIMArchive`
(5) — 229 путей. `help-check -Write` проходит (885/2985); `GEARKIT` и `l*` NIV-слоты
описаны. Одобрены пользователем 2026-11-08, переведены в `reviewed`.

**B5 (2026-11-08, reviewed).** 5 документов: `Backgrounds` (95), `DifficultySettings` (71),
`Disease` (47), `RandomStats` (16), `MilitiaIndividual` (15) — 244 пути. `help-check -Write`
проходит (1129/2985). Disease-поля помечены `DISEASE` (special-флаги — ещё
`DISEASE_SEVERE_LIMITATIONS`), RandomStats — `MERCS_RANDOM_STATS`, Militia —
`INDIVIDUAL_MILITIA`, NCTH-константы — `NCTH`; `Backgrounds` — `ENABLE_BACKGROUNDS`. Одобрены пользователем 2026-11-08, переведены в `reviewed`.

**B6 (2026-11-08, reviewed).** 15 документов, 34 таблицы, 180 уникальных путей:
`FacilityTypes` (55, включая пустой `ASSIGNMENT/CONDITIONS`), `SectorNames` (21),
`SectorLoadscreens` (9), `MovementCosts` (7), `HeliSites` (7), `SamSites` (8),
`DeliveryMethods` (5), `Minerals` (3), `ShippingDestinations` (6), `InteractiveActions` (9),
`BloodcatPlacements` (22), `CreaturePlacements` (21), `AltSectors` (1 matrix field),
`CoolnessBySector` (1 matrix field), `ExtraItems` (20 файлов, 5 полей). `help-check -Write`
проходит (1404/2985); 20 файлов ExtraItems дают 100 табличных путей. Facility chances
интерпретированы как шкала 0–1000 (`500` = 50%) и это вынесено в Open questions.
Одобрены пользователем 2026-11-08, документы переведены в `reviewed`.

**B7 (2026-11-08, reviewed).** 14 документов, 117 таблиц, 238 уникальных путей (1469 табличных
путей): четыре армейских документа, `SoldierProfiles` (6 файлов, 8 полей), пять drop-документов
(по 4 поля), `GunChoices` (13 таблиц: 7 живых + 6 backup-алиасов), `ItemChoices` (7 таблиц),
`IMPItemChoices` (54 поля) и `Traders` (81 таблица: 20 полей `Merchants` + 80 инвентарей по
3 поля, таблица «файл → дилер»). `help-check -Write` проходит (2873/2985). Одобрены
пользователем 2026-11-08, документы переведены в `reviewed`. Backup-папка — алиас; точные
merchant flags/coolness formula вынесены в `Open questions`.

**B8 (2026-11-08, reviewed).** 18 табличных документов + обновлённый `help/Localization.md`;
19 таблиц, 112 путей, полное покрытие `2985/2985` и `239/239`. `Email` — 3 документа,
`BriefingRoom` — 8, `History` — 2, `CampaignStatsEvents` — 12, `LoadScreenHints` — 12,
`LayoutMainMenu` — 13, `ActionItems` — 5, `EnemyNames`/`EnemyRank` — по 4, `RandomTeams` — 1,
`Sounds`/`BurstSounds` — общий документ по 1 полю, `HiddenNames` — 3, `CivGroupNames` — 6,
`SquadNames` — 2, `SpreadPatterns` — 7, `Vehicles` — 23. Одобрены пользователем
2026-11-08, документы переведены в `reviewed`; `help-check -Release` проходит.

**B9 (2026-11-08, done для английского канона).** 225 уникальных cross-document links в 82/94
документах (было 123/40); устаревшие «future» ссылки заменены; 12 самодостаточных документов
оставлены без cross-links; `calibre`/`defence` унифицированы. Карта — `help/REFERENCE-MAP.md`,
генератор — `util-scripts/help-crossrefs.ps1` (`-Write`/`-Check`). `help-check -Release`
проходит; v4/v5 покрытие сверено.

**B10 (2026-11-08, in progress).** B0 правок не потребовал. B1: исправлены знаки burst/auto
penalty, `ubAimLevels`, recoil, `ubShotsPer4Turns`, `ubVolatility`, `ubMagSize`, `bIndoorModifier`,
`DamageKeyFrame`; у `Items` — `ItemSize`, `usSpotting` (clamp `0..100`), `usPortionSize`
(дефолт `100`), `usOverheatingCooldownFactor` (градусы за тик), `ubWeight`, `usPrice`,
`ubClassIndex`, `bSoundType`, hacking/burial/administration modifiers. B2: `uiSlotIndex` `UINT32`,
skill-check типы и знак, AP-cost. B3: `Food` типы/`bMoraleMod` (не читается), `FoodOpinion`
`UINT8` и перезапись, `Drugs` chance (`0`/`100`/omitted = всегда). B4: 59 backing-типов
`MercProfiles`, `bHated`/`bHatedTime`/`bTownAttachment`, цены `MercStartingGear`,
`MercOpinions` как repeated `AnOpinion` с атрибутами. B5: 92 value-типа `Backgrounds` → `INT16`,
ключевые `DifficultySettings` → `UINT16`/`UINT8`/`INT8`; в `Disease` исправлены `uiIndex`,
stat/effect-типы и `moralemodifier`. B6: у `FacilityTypes` подтверждён делитель `ubChance`
(INI, обычно `1000`), но загрузчик кастует в `UINT8` (значения >255 обрезаются), а `usChance`
не читается. B7: `ArmyComposition` — `INT8`-проценты с clamp `0..100` и популяции с clamp в `1`;
`GarrisonGroups.Composition` — `UINT8`; `PatrolGroups.Size` — база ±1; `Traders` — флаги,
clamp `1..10`, `addToCoolness` `0..10`, формула coolness; `UniformColors` — шесть обязательных
блоков и закрытый case-sensitive список палитр; пять enemy-drop таблиц — `UINT32`/`UINT8`,
`EnemyMiscDrops.usItemClass` — bitmask `ItemClass`; choice-таблицы — `bItemNo*` `INT16`,
`ubChoices`/`ubNumItems` `UINT8`, `uiIndex` `UINT32`; `SoldierProfiles` `UINT16`/`UINT8`;
`Traders` inventory `UINT32`/`INT16`/`UINT8`. B8: `CampaignStatsEvents` `UINT16`/`UINT8`;
`EnemyRank` `UINT16`/`BOOLEAN`/`CHAR16`, `ExpLevel` `1..9`; `EnemyNames.Sector` — `A1..P16`
с проверкой диапазона; `BriefingRoom.Hidden` инвертирован (`0` = скрыто, non-zero = видимо);
`History` — `UINT16`/`CHAR16`; `LoadScreenHints` — category-биты, `item` не читается,
рабочий элемент — `items`. В `GLOSSARY.md`
добавлены `recoil`, `aim level`, `burst penalty`. Остались непроверенные поля
B1/B3/B4/B5 и часть B7/B8.
**Дальше.** Продолжить B1/B3/B4/B5/B7/B8; после B10 — повторный B9-гейт и финальный коммит.
