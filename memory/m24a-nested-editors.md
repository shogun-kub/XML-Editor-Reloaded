# M24a — вложенные редакторы городов и типов объектов

**Статус:** достигнут 2026-11-03. `Map.Cities` и `Map.FacilityTypes` открываются в отдельном
`NestedTableWorkspaceViewModel`, а не в плоском grid.

## Что сделано

- `TableDocument` разрешает объявленный primary-путь (`CITYLIST/CITY`) через `OverlayKeyResolver`,
  `SynchronizeRecords` и `AddRecord` работают с существующим контейнером (`CITYLIST`), а не только
  с корнем. `TableRecord` получил `GetChildFieldNames(FieldPath)`.
- `TableWorkspace.GetRootCells` отдаёт root singleton-ячейки и без оверлея; `SetRootValue` пишет
  базу или оверлей, `IsTranslatable`, `GetNestedRecords`, `GetNestedValue`, `IsNestedTranslated`
  покрывают `ASSIGNMENT` по ключу/occurrence.
- `NestedTableWorkspaceViewModel` + `NestedTableWorkspaceView`: список записей, группы полей,
  вложенные leaf-группы (`baseSector/x`, `townPoint/y`, `iconPosition/...`), `COUNTRY`, блоки
  `ASSIGNMENT` с `CONDITIONS`/`MORALE`/`LOYALTY_LOCAL`/`DRUNK`/`drugitems`; add/delete записей,
  undo/redo, save, reload, validate, language selector, external-change recovery.
- Переводимый `ASSIGNMENT/szTooltipText` пишется через `SetNestedValue(OverlayRecordKey(primary,
  [type]))`; база не меняется.
- Роутинг в `MainViewModel`/`TableEditorFactory`; DataTemplate в `MainWindow.xaml`.
- Строки: `NestedEditorSelectRecord`, `NestedEditorRecordAdded`, `NestedEditorRecordDeleted`,
  `EditorUndoDone`, `EditorRedoDone`, `EditorDeleteRowTitle` (en/ru).

## Проверка

- `dotnet build -warnaserror` — 0/0.
- `util-scripts/test.ps1` — 608 тестов; новые `NestedTableWorkspaceViewModelTests` (3),
  `OpenCities_OpensNestedWorkspace`, `RootSingleton_BaseEdit_IsUndoable` и
  `NestedCity_WithoutEdits_DoesNotRewrite` (загрузка/сохранение без правок не меняет байты).
- UI-smoke S93 (v4) — PASS; S50 подтверждает, что изменены только `Map/Cities.xml` и
  `Map/Russian.FacilityTypes.xml`.
- Ручной `ASSIGNMENT` add/delete и list/dict-поля — M24b.

## Грабли

- `Assigment`-поле нельзя писать через `TableWorkspace.SetValue` при выбранном оверлее и
  переводимом пути: нужен `SetNestedValue` с nested key, иначе значение уйдёт в базу.
- `GetNestedValue` для глубоких путей (`CONDITIONS/...`) оверлейных ячеек не имеет — падает на
  базовое значение; это ожидаемо, переводится только `szTooltipText`.
- В UI-smoke `SetTextAndCommit` (Tab) не всегда коммитит поле внутри `Expander`; сценарий S93
  явно переводит фокус на `NestedSearchBox` после ввода.
