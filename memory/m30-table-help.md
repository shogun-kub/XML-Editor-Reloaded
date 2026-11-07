---
name: m30-table-help
description: M30 TableData help — B0 and B1a approved 2026-11-07, seven documents, 29 tables, 335 field paths, help-check tooling, clean table names, table_ids and option-gated fields.
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
— OIV-only marker.

**Дальше.** B1b — `Weapons`, `AmmoTypes`, `AmmoStrings`, `Explosives`, `ExplosionData`
(117 полей, 5 таблиц).
