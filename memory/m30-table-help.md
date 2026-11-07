---
name: m30-table-help
description: M30 TableData help — B0/B1a/B1b approved, B2/B3 draft 2026-11-07; thirty-one documents, 53 tables, 656 field paths, help-check tooling, clean table names, table_ids and option-gated fields.
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

**B2 (2026-11-07, draft).** 12 документов: `Attachments` (4), `AttachmentSlots` (9),
`AttachmentInfo` (5), `AttachmentComboMerges` (23), `IncompatibleAttachments` (2),
`CompatibleFaceItems` (2), `Merges` (6), `Item_Transformations` (15),
`StructureConstruct` (10), `StructureDeconstruct` (7), `StructureMove` (3), `Clothes` (4) —
90 путей. `help-check -Write` проходит (542/2985); row counts не добавлены. У NAS-полей
`Only used with: NAS`; повторяющиеся `allowedtile`/`*facingtile` описаны. Ждут ревью.

**B3 (2026-11-07, draft).** 7 документов: `Pockets` (41), `PocketPopups` (8),
`LoadBearingEquipment` (18), `Food` (6), `FoodOpinion` (3), `Drugs` (16), `RandomItem` (22) —
114 путей. `help-check -Write` проходит (656/2985). NIV/food/disease-поля помечены
`Only used with`; capacity/random slots сгенерированы. Ждут ревью.

**Дальше.** B4 — `MercProfiles`, `MercStartingGear`, `MercOpinions`, `MercQuote`,
`MercAvailability`, `AIMAvailability`, `FaceGear`, `RPCFacesSmall`, `IMPPortraits`,
`IMPVoices`, `OldAIMArchive`.
