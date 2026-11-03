---
name: m25a-comparison-v2
description: M25a value-level comparison v2 (records by key or order, overlay cells, root singleton fields, bounded details, UI tabs) implemented 2026-11-03.
metadata:
  node_type: memory
  type: project
---

**M25a (2026-11-03).** `TableDataComparer` расширен с структурного сравнения до значений.

- `TableCells.cs` (`TableCellsReader`) обходит `TableDocument.Records` и листья; ключ берётся из
  метаданных (`RecordKey`/`RecordKeyFields`), иначе запись получает позиционный `#N`. Вложенные
  сегменты (`GEARKIT`, `ASSIGNMENT`) дают пути вида `GEARKIT[0]/mGearKitName`.
- `TableValueComparer` сопоставляет записи по ключу/порядку (дубликаты получают `#2`), сравнивает
  значения лексемно (`1.0` ≠ `1`), повторяющиеся поля — по порядку; добавленные/удалённые записи
  детализируются без полей, изменённые — списком отличающихся полей. Корневые singleton-поля
  (`COUNTRY/countryName`) сравниваются отдельно.
- `OverlayFileComparer` собирает языки из `TableDescriptor.Overlays`, грузит оверлеи через
  `LoadOverlayAsync`; ячейки сравниваются только когда обе стороны `Loaded`. Отсутствующий,
  повреждённый или нечитаемый оверлей показывается статусом стороны — списка ячеек нет, чтобы
  v4/v5-отчёт не раздувался.
- Границы: `TableComparisonLimits` — 300 символов на значение, 2000 записей, 20000 полей,
  2000 ячеек на язык. Счётчики точные; детали помечены `RecordsTruncated`/`CellsTruncated`.
- UI: вкладки «Записи» и «Оверлеи» (`ComparisonRecordItem`, `ComparisonValueItem`,
  `ComparisonOverlayItem`, `ComparisonOverlayCellItem`), сводка `CompareSummaryValuesText`,
  столбец `ValuesText` в списке таблиц. CSV: `Record`/`Value`/`RootField`/`Overlay`/`OverlayCell`;
  Markdown: `#### Value differences`, `#### Overlays`.
- Тесты: `TableDataComparerTests` (ключ, порядок, root, оверлеи, повреждённый оверлей, markdown),
  `CompareViewModelTests` (проекция записей/оверлеев). Реальный v4/v5 compare — 2 с.
  UI-smoke S40–S42 дополнены; S41 проверяет новые вкладки.
