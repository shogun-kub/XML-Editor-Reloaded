# M24b — структурные вложенные записи

**Статус:** достигнут 2026-11-03. `Map.FacilityTypes` получил структурные назначения, повторяемые
блоки и списки; `Map.Cities` — квалифицированный перевод `townName`.

## Что сделано

- Backend `TableRecord`: `GetOccurrenceCount`, `GetChildFieldNames(path, occurrence)`,
  `GetRepeatedValues`/`SetRepeatedValues` (повторяемый лист на вложенном пути), `AddElement`,
  `RemoveElement`. Все записи сохраняют порядок и чужие элементы.
- Backend `TableWorkspace`: `AddNestedRecord` (уникальный ключ, вставка после последнего
  одноимённого элемента), `DeleteNestedRecord` (удаляет и парную запись оверлея), `AddRepeatedBlock`,
  `DeleteRepeatedBlock`, `Get/SetRepeatedValues`, `Get/SetNestedRepeatedValues`; каждая операция —
  одна undo-транзакция с base/overlay-снимком.
- `TableSchemaBuilder` считает переводимым и квалифицированный путь (`CITYLIST/CITY/townName`), поэтому
  `townName` получает маркер перевода в схеме.
- `NestedTableWorkspaceViewModel`: ввод нового типа назначения и `AddAssignmentCommand`,
  `DeleteSectionCommand` для назначений/блоков, `BlockAddActions` (кнопки «Добавить <поле>»),
  секции повторяемых блоков с occurrence-aware чтением/записью, рекурсивный обход вложенных полей и
  списков, `NestedListViewModel` (add/remove значений, запись всего списка).
- XAML: вторая строка тулбара с типом назначения и кнопками блоков, кнопка удаления в заголовке
  секции, редакторы списков под полями.
- Новые строки RESX (en/ru): типы назначения, добавление/удаление назначения и блока, списки.

## Проверка

- `dotnet build -warnaserror` — 0/0.
- `util-scripts/test.ps1` — 618 тестов; новые backend-тесты: `AddNestedRecord_Assignment_*`,
  `DeleteNestedRecord_RemovesAssignmentAndOverlayEntry`,
  `RepeatedBlock_AddAndDeleteProductionLine`,
  `SetNestedRepeatedValues_DrugItems_ReplacesOnlyTargetChildren`,
  `SetRepeatedValues_ProductionLine_OnlySelectedBlock`,
  `CityTownName_OverlayEdit_WritesOnlyOverlay`; App-тесты:
  `FacilityWorkspace_AddsAndDeletesAssignment`,
  `FacilityWorkspace_EditsRepeatedListsAndBlocks`,
  `CityWorkspace_OverlayTownName_WritesOnlyOverlay`.
- UI-smoke S94 (v4) — PASS: новое назначение `TESTM24B`, `drugitems` дополнен `300`,
  `PRODUCTIONLINE/usItemToCreate=9999`, перевод tooltip в `Russian.FacilityTypes.xml`; S50 — ровно
  2 заявленных файла.

## Ограничения

- Новое назначение создаётся с ключом и пустым `szTooltipText`; остальные поля не изобретаются.
- Новый блок `PRODUCTIONLINE` получает пустые листья по шаблону первого существующего блока.
- Dict-полей с маской `%Index%` в этих таблицах нет — они остаются в общем grid.

## Грабли

- `TableRecord.GetRawValue("A/B")` трактует строку как имя прямого-child, а не путь; для путей нужен
  `GetRawValue(FieldPath.Parse("A/B"))`.
- `OverlayDocument.Xml` — internal; тесты проверяют оверлей через `Overlay.Document.Records`.
- Удаление назначения должно снимать base-снимок и overlay-снимок до мутации, иначе undo теряет
  парную запись перевода.
- Добавление пустого блока даёт секцию без полей; поэтому VM передаёт scaffold из существующего блока.
