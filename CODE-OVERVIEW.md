# XML Editor Reloaded — обзор кода

> Состояние на 2026-11-07, ветка `dev`. Документ описывает **исходный код приложения**, а не
> пользовательскую инструкцию. Тесты, `References/`, `out/`, `bin/`, `obj/` и сгенерированные файлы
> исключены. Полный перечень `.cs`-файлов с типами и методами — в приложении в конце.

## 1. Масштаб и состав

В решении пять production-проектов:

| Проект | TFM | Назначение | Зависит от |
|---|---|---|---|
| `XmlEditorReloaded.Core` | `net10.0` | контракты, доменные типы, `OperationResult`, фильтры, layout, локализация, валидация | BCL, DI abstractions |
| `XmlEditorReloaded.Xml` | `net10.0` | загрузка/сохранение XML, workspace, оверлеи, сравнение, copy, поиск, валидация | Core |
| `XmlEditorReloaded.Data` | `net10.0` | SQLite-метабаза, каталог таблиц, настройки, layout, лог | Core, `Microsoft.Data.Sqlite` |
| `XmlEditorReloaded.Imaging` | `net10.0` | STI/ETRLE → RGBA, каталог item-изображений, кэш кадров | Core |
| `XmlEditorReloaded.App` | `net10.0-windows` | WPF shell, MVVM, диалоги, темы, RESX | Xml, Data, Imaging, Core |

Всего в `src/` около **235 C#-файлов**, **420 объявленных типов** и **≈1600 объявлений методов**
(точный машинный перечень — в приложении). Это важная цифра для оценки сложности: большая часть
кода сосредоточена в нескольких «тяжёлых» классах, а не распределена равномерно.

## 2. Архитектура и правило зависимостей

```
App (WPF, MVVM)
 ├── Xml (таблицы, оверлеи, сравнение, copy)
 ├── Data (SQLite, каталог, настройки)
 ├── Imaging (STI/ETRLE)
 └── Core (контракты и примитивы)
        ▲
        ├── Xml
        ├── Data
        └── Imaging
```

Правила, зафиксированные в `PROJECT-STRUCTURE.md` и `AGENTS.md`:

- backend-проекты не ссылаются на WPF и `System.Windows.*`;
- разбор XML, SQL и STI не выполняется в UI-слое;
- кросс-проектные контракты живут в `Core`;
- UI-строки — только в RESX;
- вывод сборки — в repository-level `out/`, а не в `bin/obj` проектов.

Такое разделение даёт три практических эффекта:

1. XML-движок можно тестировать без окна и без SQLite.
2. WPF-слой заменяем (в репозитории уже есть экспериментальная Avalonia-ветка, не входящая в
   решение).
3. Metadata и file-system каталог можно подменить fake-реализациями в тестах.

## 3. Запуск и DI

### `App.xaml.cs`

`App` — composition root и глобальный обработчик ошибок.

- `ConfigureServices(IServiceCollection, string databasePath)` регистрирует:
  - `AddXmlEditorCore`, `AddXmlEditorData`, `AddXmlEditorXml`;
  - `IDialogService` → `WpfDialogService`;
  - `IDataSetSessionFactory` → `DataSetSessionFactory`;
  - `ITableEditorFactory` → `TableEditorFactory`;
  - `IItemImageService` → `WpfItemImageService`;
  - `IImageViewerService` → `WpfImageViewerService`;
  - `ITableComparisonService` → `TableDataComparer`;
  - `IVersionCompareService`, `ITableCopyDialogService`, `ISettingsDialogService`;
  - `CompareViewModel`, `ImageViewerViewModel`, `MainViewModel`, `MainWindow`.
- `OnStartup`:
  1. применяет системную тему;
  2. создаёт `StartupTrace` **до** DI и лога, чтобы увидеть ошибку, если каталог данных недоступен;
  3. проверяет запись в `AppPaths.ApplicationDataDirectory`;
  4. ставит `OperationLog.Sink = new FileOperationLog(...)`;
  5. регистрирует глобальные обработчики;
  6. строит `ServiceProvider`, применяет миграции;
  7. загружает `settings.json`, выбирает UI-культуру;
  8. создаёт `MainWindow`, применяет сохранённую геометрию, показывает окно;
  9. вызывает `MainViewModel.InitializeAsync()`.
- `OnExit` освобождает `ServiceProvider` и снимает лог.
- `OnDispatcherUnhandledException` логирует и **не завершает** приложение: пользователь может
  сохранить открытые таблицы. `AppDomain`/`TaskScheduler` обработчики только логируют.

`OnStartup` — `async void`, это неизбежно для WPF; внутри есть общий `try/catch` и аварийный
`ShowFatalError`, поэтому необработанное исключение не теряется.

### `MainWindow.xaml.cs`

`MainWindow` — тонкая оболочка:

- `ApplySavedPlacement`/`CapturePlacement` — геометрия окна и maximized-состояние;
- `OnClosing` вызывает `MainViewModel.RequestCloseAsync(CapturePlacement())`; `RequestCloseAsync`
  подтверждает закрытие каждого dirty-редактора, затем вызывает `FlushLayoutsAsync` и
  `PersistWindowPlacementAsync`;
- обработчики дерева таблиц, двойных щелчков, `FindInTree`, fold/unfold;
- `OnPreviewKeyDown` маршрутизирует горячие клавиши в команды VM.

Основная логика окна остаётся в `MainViewModel`; code-behind занимается только WPF-событиями,
которые неудобно выражать командами.

## 4. Core: контракты и примитивы

`Core` намеренно не знает ни о файлах, ни о SQLite, ни о WPF. Его типы делятся на четыре группы.

### 4.1 Результат операции

`OperationResult` и `OperationResult<T>`:

- `Success()`, `Failure(errorCode, errorText)`, `Failure(other)`;
- `IsSuccess`, `ErrorCode`, `ErrorText`, `Data`;
- `Failure(other)` бросает `InvalidOperationException`, если исходный результат успешен.

Это выбранная стратегия для **ожидаемых** ошибок: «файл не найден», «неверный XML», «нет ключа»,
«конфликт ID». Исключения оставлены для ошибок программиста и неожиданных сбоев. Благодаря этому
вызывающий код обязан обработать ошибку, а тесты проверяют `error_code`, а не текст исключения.
Минус — обилие строковых кодов и необходимость вручную пробрасывать `Failure(other)`.

### 4.2 Идентичность таблиц и записей

- `TableName` — struct с `Parse`, `TryFromRelativePath`, `ToRelativePath`, `CompareTo`.
  Разделитель — точка (`Items.Items`), регистр сохраняется.
- `TableDescriptor` — table name + absolute path + relative path + `IsMapped` + overlays.
- `RecordKeySpec`, `RecordKeySegment`, `RecordKeyKind` — описание primary/nested ключей.
- `RecordKeyCodec` — `Compose`, `Decompose`, `Display`; ключ — не просто строка, а несколько
  сегментов, включая позиционный fallback.
- `FieldPath` — slash-separated путь (`GEARKIT/mGearKitName`), `Parse`, `TryParse`, `Equals`.
- `RecordFields` — имена виртуальных/служебных полей (например, текстовое поле записи).

Ключи — центральная абстракция: на них держатся оверлеи, сравнение, copy, undo и навигация.
Поэтому `RecordKeySpec` поддерживает не только одиночный `uiIndex`, но и compound/nested ключи
(`CITYLIST/CITY`, `ASSIGNMENT`, `GEARKIT`).

### 4.3 Метаданные и локализация

- `TableMetadata` — агрегат: `XmlRoot`, `RecordElement`, `IsLookup`, `RecordKey`, `Columns`,
  `ColumnHints`, `ListFields`, `DictFields`, `TranslatableFieldPaths`, `ClassVisibilityRules`,
  `SymmetricKeyPair`, `ExclusiveFlagGroups`, `ClassValueLimits`, `TextLimits`, `IsFlagLookup`.
  В конструкторе строит словари и методы `TryGetColumn`, `TryGetColumnHint`, `TryGetTextLimit`,
  `TryGetClassVisibilityRule`.
- `ColumnMetadata` — declaring-колонка: тип, editor kind, flags, lookup target, relation metadata.
- `ColumnHint` — curated-подсказка для non-declaring/document-driven поля.
- `ListFieldDefinition`/`DictFieldDefinition` — повторяемые элементы и indexed-поля с маской
  (`ItemCapacityPerSize%Index%`).
- `ClassVisibilityRule`, `ExclusiveFlagGroup`, `ClassValueLimit`, `ColumnTextLimit`,
  `SymmetricKeyPair` — правила, которые влияют на UI и валидацию, но не меняют XML.
- `LanguageDescriptor`/`OverlayDescriptor` — язык, префикс оверлея, base/overlay paths.
- `TranslationReport`/`TranslationIssue` — отчёт по недостающим/лишним/пустым переводам.

Метаданные намеренно **не являются DTO жёсткой схемы**: `Columns` описывает declaring-поля,
`ColumnHints` — дополнительные подсказки, а реальные поля всё равно берутся из XML. Это позволяет
открывать таблицы, которых нет в метабазе, и не терять новые поля.

### 4.4 Фильтры, layout, изображения, диагностика

- `RowFilterCriterion`, `RowFilterGroup`, `RowFilterOperator`, `RowFilterLogicalOperator` —
  переносимое описание фильтра, не зависящее от WPF.
- `TableLayout`, `ColumnLayout`, `TableSortDirection`, `ITableLayoutStore` — ширины/порядок/
  видимость колонок и сортировка; `JsonLayoutStore` хранит их по паре «каталог + таблица».
- `RgbaBuffer`, `ImageSize` — сырые RGBA-пиксели; UI конвертирует их в `BitmapSource`.
- `OperationLogEntry`, `IOperationLog`, `OperationLog`, `OperationLogLevel` — глобальный фасад
  логирования; `FileOperationLog` — файловая реализация.
- `FileAccessFailure` — объясняет `IOException`/sharing violation человеческим текстом.
- `WindowPlacement.FitToScreen` — геометрия окна с учётом виртуального экрана.
- `ValidationReport`, `ValidationIssue`, `ValidationSeverity`, `DatasetValidationReport` —
  общие модели отчётов.

### 4.5 Абстракции

`ITableCatalog`, `ITableRepository`, `ITableMetadataProvider`, `ILookupRepository`,
`ITranslationCatalog`, `ITableRelationResolver`, `ISettingsStore`, `ITableLayoutStore` — точки
замены для Data/Xml. `CoreServiceCollectionExtensions.AddXmlEditorCore` намеренно не регистрирует
реализаций: Core содержит только контракты и value-типы, а конкретные сервисы добавляют
`AddXmlEditorData`, `AddXmlEditorXml` и `AddXmlEditorImaging`.

## 5. Xml: движок таблиц

Это самая содержательная библиотека. Её можно читать слоями.

### 5.1 Документ и запись

**`TableDocument`** — владелец `XDocument`, исходных байтов и baseline-метаданных файла.

- конструктор `internal`: принимает descriptor, metadata, `XDocument`, `FileFormatInfo`, status,
  diagnostics, record element, original length/mtime/SHA-256 и original bytes;
- `CreateXmlSnapshot()` — сериализует текущее дерево (включая несохранённые правки) и находит
  позиции открывающих тегов для read-only XML-вью;
- `AddRecord`, `DuplicateRecord`, `DeleteRecord`, `FindRecord`, `GetNextPrimaryKey`,
  `SynchronizeRecords` — структурные операции;
- `MarkDirty`, `MarkClean`, `RecomputeDirty`, `AcceptSaved` — состояние сохранения и baseline;
- `OriginalXml` — копия исходного дерева, `OriginalBytes` — исходные байты;
- `ChangeTracker` — журнал изменений для диагностики/будущего undo.

`TableRecord` — фасад над `XElement`, не DTO:

- `GetValue`/`GetRawValue` (по имени и по `FieldPath`, с occurrence index);
- `GetKey`, `GetKeyComponents`, `KeyDisplay`, `IsKeyComplete`;
- `SetFieldValue` (валидирует тип, сохраняет raw-текст, поддерживает `addIfMissing`);
- `SetListField`, `GetRepeatedValues`, `SetRepeatedValues`, `AddElement`, `RemoveElement`;
- `SetDictField` (mask + index);
- `TrySetValue`, `ValidateValue`, `FindPath`, `CreatePath`.

Ключевое решение: запись **разрежена**. Отсутствующий элемент не создаётся, пока пользователь
явно не добавит его. `TableRecord` не сериализует запись целиком и не добавляет все колонки —
иначе round-trip менял бы файл.

### 5.2 Загрузка и сохранение

**`TableFileLoader`**:

- `LoadAsync` — читает байты, определяет формат, парсит `XDocument` с
  `LoadOptions.PreserveWhitespace | SetLineInfo`;
- `CreateReaderSettings` — `DtdProcessing.Prohibit`, `XmlResolver = null`, `IgnoreWhitespace =
  false`, `IgnoreComments = false`; это защита от XXE и сохранение комментариев;
- `LoadOverlayAsync` — тот же путь, но overlay не обязан содержать record element;
- `InferRecordElement` — document-driven fallback;
- `AddLegacyRepeatedKeyDiagnostic` — предупреждает о старом positional-ключе, не исправляя файл.

**`TableFileSaver`**:

- `SaveAsync(document, ct, forceOverwrite)`:
  1. отказывает, если документ не `Loaded`;
  2. не пишет чистый документ;
  3. проверяет внешние изменения по `(length, mtime)`;
  4. пробует `LosslessDocumentPatcher.TryPatch`;
  5. при неудаче — `DocumentSerializer.Serialize` (полная сериализация);
  6. делает `CanReparse` — байты обязаны снова парситься;
  7. сравнивает SHA-256 с original; если совпало — `MarkClean`;
  8. повторно проверяет внешние изменения;
  9. пишет временный файл и делает `File.Move(..., overwrite: true)`;
  10. `document.AcceptSaved(bytes, mtime, hash)`.

**`LosslessDocumentPatcher`** — внутренний оптимизатор:

- сравнивает original/current деревья;
- собирает `ScalarChange`, `ElementDeletion`, `ElementInsertion`;
- строит span tree по исходному тексту;
- применяет splice к исходным байтам, сохраняя declaration casing, entity references, empty-element
  style и отступы;
- проверяет, что пропатченное дерево эквивалентно текущему; иначе возвращает `false` и вызывается
  serializer.

Это компромисс: байтовый патчер сложен, но именно он обеспечивает главное требование «не ломать
XML». Полная сериализация остаётся безопасным fallback.

`DocumentSerializer` — `XDocument.ToString()` с сохранением declaration/encoding; используется
только когда lossless-патч невозможен.

### 5.3 Workspace и undo

**`TableWorkspace`** — основная единица редактирования одной таблицы. Он связывает:

- `BaseDocument` (`TableDocument`);
- `Overlay` (`OverlayDocument?`);
- `Session` (`TableEditSession`) для routing base/overlay;
- `History` (`TableEditHistory`);
- `Schema` (`TableSchema`);
- `_saver` (`TableFileSaver`).

Главные методы:

- `LoadAsync` — загружает base и, если выбран язык, overlay; проверяет metadata/overlay;
- `GetRows`/`GetRow` — строит `WorkspaceRow` из base + merged overlay;
- `SetValue` (несколько перегрузок для row/key/path/occurrence) — выбирает base/overlay, снимает snapshot, применяет изменение,
  записывает `TableEditTransaction` в историю;
- `DeleteRow`, `AddRecord`, `DuplicateRecord`, `DeleteRecord`, `SetListValues`;
- `GetNestedRecords`, `AddNestedRecord`, `DeleteNestedRecord`, `AddRepeatedBlock`,
  `DeleteRepeatedBlock`, `Get/SetNestedRepeatedValues`;
- `SetRootValue` — для singleton-полей (`COUNTRY/countryName`);
- `SaveAsync` — `Session.SaveAsync` или `_saver.SaveAsync`;
- `ValidateAsync` — `TableValidator`;
- `AnalyzeTranslations` — `TranslationAnalyzer`;
- `Undo`/`Redo` — делегирует `History`.

`TableWorkspace` разбит на partial-файлы:

- `TableWorkspace.Bulk.cs` — `PreviewBulk`, `ApplyBulk`, `EvaluateRow`, `TryComputeArithmetic`;
- `TableWorkspace.Lookups.cs` — `GetLookupIds`, `SuggestLookupId`, `AddLookupRecord`,
  `ChangeLookupId`, `HasLookupId`.

**`TableEditHistory`**:

- хранит `TableEditTransaction` (набор `ElementSnapshotState`);
- `Record`, `Undo`, `Redo`, `Clear`;
- ограничения: 50 транзакций и 10 000 undo-состояний;
- `ElementSnapshotState` хранит элемент до/после, умеет `CaptureInsertion`, `MarkRemoved`,
  `CompleteAfter`;
- `TableEditTransaction` применяет изменения к документам и вызывает `RefreshDocuments`.

История **не thread-safe**; это осознанно: workspace живёт на UI-потоке. Для table set история
общая, поэтому одно действие, затронувшее item и дочернюю таблицу, отменяется одним шагом.

**`TableSetWorkspace`** — набор связанных таблиц (item + weapon/magazine/attachment и т.п.):

- `LoadAsync` загружает primary и relation-членов (`SubTable`, `ChildRows`, при необходимости
  `Reverse`);
- `Members`, `Warnings`, `IsDirty`, `CanEdit`, `History`;
- `SetValue`, `DeleteRow`, `AddRecord`, `DuplicateRecord`, `DeleteRecord`, `RenameRecord`,
  `SwapRecords`, `EnsureSubTableRecord`;
- `SaveDetailedAsync` сохраняет каждый dirty-файл независимо и возвращает
  `TableSetSaveReport` с ошибками по файлам;
- `SaveAsync` сворачивает отчёт в один `OperationResult`;
- `FindDependingRecords`/`DescribeDependents` — preflight удаления и зависимые записи;
- `Capture`/`RewriteDependent` — поддержка изменения ключей и зависимых ссылок.

`TableSetSaveReport` специально не бросает исключение на первый сбой: один внешне изменённый файл
не должен скрывать успешное сохранение остальных. Обратная сторона — набор не является
транзакционным целиком; это одна из главных зон для улучшения.

### 5.4 Оверлеи

- `OverlayDocument` — отдельный `TableDocument` для overlay-файла; `LoadAsync`, `CreateAsync`,
  `TryDelete`, `SaveAsync`.
- `OverlayKeyResolver` — `ResolvePrimary`, `ResolveNested`, `ResolveSingleton`; находит запись
  overlay по primary/nested/compound key.
- `OverlayMerger` — `Merge(base, overlay)`; возвращает `MergedTable`/`MergedRecord`/`MergedCell`
  (base value + overlay value + effective value).
- `OverlayWriter` — `ForRecord`, `SetValue`, `SetRootValue`, `SetLeaf`, `CreatePrimaryRecord`,
  `FindOrCreateNested`, `FindOrCreatePositional`, `FindOrCreateScalar`; создаёт минимальную
  overlay-запись только из ключа и переопределённых полей.
- `TableEditSession` — routing:
  - `Merge()`;
  - `SetValue` — если overlay выбран и путь translatable, пишет в overlay, иначе в base;
  - `SetNestedValue`, `SetRootValue`;
  - `SaveAsync` — base и overlay сохраняются независимо;
  - `IsTranslatable` — сравнение пути с `TranslatableFieldPaths`.

`TranslationAnalyzer` ищет missing/orphan/duplicate/empty переводы; `OverlayFileComparer`
сравнивает overlay-файлы по записанным ячейкам.

### 5.5 Схема, связи, lookup

- `TableSchemaBuilder.Build` — выводит `TableSchema` из metadata + `TableDocument`: declaring
  columns, hints, типы, editor kind, группы, dict-паттерны, вложенные ключи.
- `TableSchema.FindColumn` — поиск колонки.
- `TableRelationResolver.ResolveAsync` — по `TableRelation` загружает target table, кэширует
  target по file stamp, умеет `Invalidate`; `RelationTarget`/`RelationTargetEntry` хранят
  допустимые значения.
- `XmlFileLookupRepository` — читает lookup-таблицы из XML, `GetEntriesAsync`; `Merge` объединяет
  base и overlay.
- `LookupUsageService` — `FindUsagesAsync` ищет ссылки на lookup ID по колонкам/подсказкам,
  возвращает `LookupUsageReport`.
- `LookupIdAllocator` — `TryParseId`, `SuggestNextId`, `IsSingleBit`: для flag-lookup свободный
  бит, для sequential — max + 1.

### 5.6 Валидация, поиск, сравнение, copy

- `TableValidator` — проверяет lookup, диапазоны, флаги, exclusive groups, class limits, dict
  masks, symmetric pairs; `ValidateAsync` overloads.
- `DatasetValidator` — проходит все таблицы, кэширует lookup по языку, превращает unreadable/
  malformed файлы в issues и возвращает `DatasetValidationReport`.
- `DatasetValidationReportFormatter` — Markdown/CSV.
- `DatasetSearchService` — `SearchAsync`, `SearchDocument`, `SearchElement`; точные счётчики,
  ограниченный список hits (`MaxHits`, `MaxHitsPerTable`, `MaxValueLength`), поиск по base и
  overlays.
- `TableDataComparer` — `CompareAsync`: загружает стороны, строит `TableComparisonReport`;
  `TableSnapshotAnalyzer`, `TableCellsReader`, `TableValueComparer`, `OverlayFileComparer`
  дают структуру, значения и overlay-различия.
- `TableComparisonReportFormatter` — Markdown/CSV.
- `TableCopyService` — `PreviewAsync` (план без записи) и `ApplyAsync` (запись); внутри
  `CopyContext`, `DataSetAccessor`, `TableCopyRemapper`, `CopyRecordPlan`/`RelatedPlan`.
  `TableCopyRemapper` маппит lookup/relation/record keys и display values.

### 5.7 Прочие элементы

- `FileFormatInfo` — BOM/encoding detection (`Detect`, `DetectAsync`, `CreateEncoding`).
- `LexicalValue` — raw text + `TryGetInt32`/`TryGetDecimal`; числа парсятся invariant culture.
- `TableRowFilterMatcher` — фильтрация строк по `RowFilterGroup`.
- `WorkspaceRow` — key + base record + merged record; `IsTranslated`, `IsTopLevelField`.
- `XmlSnapshot`/`XmlRecordSpan` — read-only снимок и позиции записей.
- `DocumentChangeTracker` — `FieldValueChanged`, `ListFieldChanged`, `RecordAdded`,
  `RecordDuplicated`, `RecordDeleted`.
- `BulkEditModels`/`TableWorkspace.Bulk` — preview/apply массовых операций.

## 6. Data: SQLite, каталог, настройки, лог

### 6.1 Метабаза

**`DatabaseMigrator`**:

- `MigrateAsync` создаёт каталог, открывает SQLite, создаёт `SchemaMigrations`, читает applied
  versions;
- `LoadMigrations` находит embedded `.sql`-ресурсы (`Metadata.Migrations.*.sql`), сортирует по
  версии;
- каждая миграция выполняется в транзакции; при ошибке rollback;
- отказ при downgrade (БД новее кода);
- возвращает количество применённых миграций.

18 линейных миграций (`0001`–`0018`) описывают таблицы, seed, hints, relations, record keys,
class visibility, symmetric pairs, value sources, validation limits, copy fields, lookup kinds:

`0001_InitialSchema`, `0002_PrototypeSeed`, `0003_ColumnHints`, `0004_ItemColumnHints`,
`0005_VerifiedColumnMetadata`, `0006_FlagLookupMappings`, `0007_TableRelations`,
`0008_M20AuxiliaryTables`, `0009_RecordKeyFields`, `0010_ItemClassVisibility`,
`0011_M23b2ItemRelations`, `0012_M23b2IndexedFields`, `0013_M23b2SymmetricPairs`,
`0014_M23b3ValueSources`, `0015_M23b4ValidationLimits`, `0016_ItemTextLengthLimits`,
`0017_CopyComparisonFields`, `0018_LookupKinds`.

Линейная схема проста и воспроизводима, но требует дисциплины: изменения только новой миграцией.

**`SqliteMetadataProvider`** реализует `ITableMetadataProvider`:

- `GetTableAsync`, `GetAllTablesAsync`, `GetLanguagesAsync`, `GetRelationsAsync`,
  `GetLookupValuesAsync`;
- приватные `ReadTableAsync`, `ReadTableDetailsAsync`, `ReadColumnsAsync`, `ReadColumnHintsAsync`,
  `ReadListFieldsAsync`, `ReadDictFieldsAsync`, `ReadTranslatableFieldsAsync`,
  `ReadRecordKeyFieldsAsync`, `ReadRecordKeyAsync`, `ReadColumnTextLimitsAsync`,
  `ReadExclusiveFlagGroupsAsync`, `ReadClassValueLimitsAsync`, `ReadSymmetricKeyPairAsync`,
  `ReadClassVisibilityRulesAsync`;
- каждое чтение открывает новое `SqliteConnection`; `GetAllTablesAsync` сначала читает строки,
  затем для каждой вызывает `ReadTableDetailsAsync`.

Плюс: провайдер не держит долгоживущее соединение и хорошо работает с параллельными чтениями.
Минус: N+1 запросов на `GetAllTablesAsync`; для 173 таблиц это заметно.

### 6.2 Каталог и переводы

- `FileSystemTableCatalog.GetTablesAsync` рекурсивно сканирует `*.xml`, исключает overlay-имена
  по префиксам языков, строит `TableDescriptor` с overlays, сортирует по имени, помечает
  `IsMapped` по метабазе.
- `TranslationCatalog` объединяет metadata и catalog: `GetOverlayDescriptorsAsync`,
  `GetTranslatableFieldsAsync`, `GetLanguagesForAsync`.

### 6.3 Настройки и layout

- `AppPaths` — `%AppData%\XmlEditorReloaded`, override `XER_APP_DATA`; свойства
  `ApplicationDataDirectory`, `SettingsFilePath`, `LayoutsFilePath`, `LogFilePath`.
- `JsonSettingsStore` — `LoadAsync`, `SaveAsync`, `LoadCoreAsync`, `SaveCoreAsync`; DTO
  `SettingsDto`, `WindowPlacementDto`; camelCase JSON; atomic temp+move; `SemaphoreSlim _fileGate`
  сериализует чтение и запись. Это исправление гонки, найденной тестом языка.
- `JsonLayoutStore` — версионированный `layouts.json`, `SchemaVersion = 1`;
  `LoadAsync`/`SaveAsync`/`RemoveAsync`/`ResetAllAsync`; `SemaphoreSlim _gate`; при нечитаемом
  файле пытается сделать backup; layout привязан к `(dataSetDirectory, tableName)`.

### 6.4 Диагностика

- `FileOperationLog` — append-only UTF-8 без BOM, session header, ротация 4 MiB в `*.1`,
  `lock` вокруг записи, ошибки логирования проглатываются.
- `StartupDiagnostics` — `ProbeWriteAccess`, `DescribeEnvironment`, `DescribeFile`,
  `DescribeOwner`; проверяет запись и ACL/owner.
- `StartupTrace` — ранний текстовый trace до DI/лога; `RecreateIfOversized`.

## 7. Imaging: STI/ETRLE

- `StciParser.ParseAsync`/`Parse` — проверяет `STCI` magic, 64-байтный header, флаги, палитру,
  frame directory; возвращает `StciImage`.
- `StciHeader`, `SubImageDescriptor`, `StciImage`, `StciFlags` — разобранные структуры.
- `EtrleDecoder.DecodeFrame` — ETRLE → RGBA; обрабатывает overrun и прозрачность.
- `StiDecoder` — фасад `ParseAsync`, `DecodeFrame`, `LoadFrameAsync`.
- `ItemImageCatalog` — определяет путь Big/Medium/Small по `ubGraphicType`/`ubGraphicNum`,
  `GetImageAsync`, `GetFrameCountAsync`, `GetMaxIndexAsync`; кэширует кадры по path+mtime.
- `DecodedFrameCache` — size-bounded LRU (`capacityBytes`, по умолчанию 128 MiB), `TryGet`,
  `Set`, `Clear`, `Remove`; `Entry` хранит mtime и размер; при переполнении вытесняет самые
  старые кадры.
- `ImageTransparency.ApplyGreenKey` — green-key для Medium/Small.
- `ImagingServiceCollectionExtensions.AddXmlEditorImaging`.

Выход — `RgbaBuffer`, без `System.Drawing`; WPF-конвертация отдельно в
`RgbaBufferConverter.ToBgra32`/`ToBitmapSource`.

## 8. App: shell, сервисы, view models

### 8.1 Сервисы

- `IDataSetSession`/`IDataSetSessionFactory` и `DataSetSession`/`DataSetSessionFactory`:
  - `DataSetSession` привязан к `TableData`; создаёт `XmlFileLookupRepository`,
    `TranslationCatalog`, `TableRelationResolver`;
  - `OpenTableAsync` → `TableWorkspace.LoadAsync`;
  - `OpenTableSetAsync` → `TableSetWorkspace.LoadAsync`;
  - `EnsureOverlayAsync` создаёт overlay при первом переводе;
  - `DataSetSessionFactory` кэширует сессии по абсолютному пути.
- `ITableEditorFactory`/`TableEditorFactory` — создаёт `TableEditorViewModel`,
  `ItemWorkspaceViewModel`, `MercGearWorkspaceViewModel`, `NestedTableWorkspaceViewModel`.
- `IDialogService`/`WpfDialogService` — folder/file pickers, confirm, unsaved changes,
  external changes, bits, about, translation report, dataset, enemy ammo, bulk edit, item
  creation, lookup ID/usages.
- `IItemImageService`/`WpfItemImageService` — `GetImageAsync`, `GetFrameCountAsync`; держит
  `ItemImageCatalog` по data directory и конвертирует RGBA в `BitmapSource`.
- `IImageViewerService`/`WpfImageViewerService` — окно просмотра.
- `IVersionCompareService`/`WpfVersionCompareService` — окно сравнения.
- `ITableCopyDialogService`/`WpfTableCopyDialogService` — окно copy.
- `ISettingsDialogService`/`WpfSettingsDialogService` — настройки.
- `FieldSourceSet` — загружает допустимые значения (lookup/relation) для detail-полей.
- `AllowedValueOption`, `BulkEditContext`, `DataSetPaths`, `DatasetNames` — вспомогательные
  модели/резолверы.

### 8.2 `MainViewModel`

Самый важный координатор shell. Зависит от `IDataSetSessionFactory`, `ITableEditorFactory`,
`ISettingsStore`, `IDialogService`, `IImageViewerService`, `IVersionCompareService`,
`ITableCopyDialogService`, `ISettingsDialogService`, `ITableLayoutStore`, `DatasetValidator`,
`DatasetSearchService`.

Основные методы:

- `InitializeAsync` — загружает settings, каталоги, последний dataset, таблицы;
- `BrowseAsync`, `RemoveDataDirectoryAsync`, `ActivateDataDirectoryAsync`, `RefreshAsync`;
- `OpenTableAsync`, `OpenEditorByNameAsync`, `CloseEditorAsync`, `RequestCloseAsync`,
  `SaveSelectedEditorAsync`, `SaveAllAsync`;
- `OpenSettingsAsync`, `ApplySettingsAsync`, `PersistWindowPlacementAsync`, `FlushLayoutsAsync`;
- `ValidateDataset`, `SearchDataset`, `ShowDatasetWindow`;
- `CompareVersions`, `CopyDataAsync`, `OpenImageViewer`, `OpenEnemyAmmoAsync`, `BulkEdit`,
  `ResetLayout`, `ShowTranslationReport`, `OpenAbout`;
- XML: `RefreshXmlSnapshotAsync`, `FindXmlRecord`, `IsXmlMode`;
- language: `ResolveLanguageCode`, `AttachEditor`/`DetachEditor`, `OnEditorLanguageChanged`,
  `PersistLanguageAsync`;
- `IsBusy`, `SelectedEditor`, `Editors`, `Tables`, `SelectedTable`, `DataDirectory`, `StatusText`.

`MainViewModel` не знает, как устроен конкретный редактор: он работает через `IEditorTab`
(`SaveWithRecoveryAsync`, `IsDirty`, `CreateXmlSnapshot`, `Close`, `LanguageChanged`,
`TableNavigationRequested`).

### 8.3 `TableEditorViewModel` (3068 строк)

Универсальный grid-редактор. Зависит от `IDataSetSession`, `TableDescriptor`, `IDialogService`,
`IItemImageService`, `ITableLayoutStore`, `LookupUsageService`.

Ключевые методы:

- `InitializeAsync`/`InitializeFromWorkspaceAsync` → `InitializeCoreAsync`;
- `SaveAsync`/`SaveWithRecoveryAsync`/`ResolveExternalChangeAsync`;
- `AddRow`, `AddRowWithDefaults`, `DuplicateRow`, `DeleteRowAsync`, `ChangeLookupIdAsync`,
  `ShowLookupUsagesAsync`;
- `Undo`, `Redo`, `BulkEdit`, `ValidateAsync`, `ReloadAsync`;
- `SetCellValue`, `SetDetailValue`, `NavigateToRelation`, `GoToTarget`, `EditBits`;
- `ClearFilter`, `FilterByValue`, `ExcludeValue`, `TableFilterViewModel`;
- `BuildColumns`, `ApplyDefaultColumns`, `ShowAllColumns`, `HideAllColumns`, `SortBy`,
  `ApplyColumnOrder`, `ApplyColumnWidths`, `ScheduleLayoutSave`, `FlushLayoutAsync`;
- `BuildLanguageOptions`, `ChangeLanguageAsync`, `ReloadForLanguageAsync`;
- `LoadDetailImagesAsync`, `LoadThumbnailAsync`;
- `RowValueComparer` — сортировка с учётом lookup/flags/decimal.

### 8.4 `ItemWorkspaceViewModel` (3385 строк)

Самый большой VM. Реализует `IEditorTab`, `ITableCellEditor`, `IDetailFieldHost`.

Он держит:

- `TableSetWorkspace _set` — item + подтаблицы;
- `ItemListEntry`/`ItemListSectionViewModel` — список предметов и фильтры;
- `DetailFieldViewModel`/`DetailFieldGroupViewModel` — форма;
- `ItemChildSectionViewModel`/`ItemSubTableSectionViewModel` — дочерние grid-ы;
- `LookupColumnData`/`RelationColumnData` — pickers;
- `ItemImageOption`/`GraphicTypeOption` — изображения;
- `LanguageOptionViewModel` — язык.

Основные группы методов:

- `InitializeAsync`/`InitializeCoreAsync`/`SaveAsync`/`SaveWithRecoveryAsync`/`Reload`;
- `NewItem`, `DuplicateItem`, `DeleteItem`, `ChangeItemId`, `GoToItemId`, `SelectRecord`;
- `SetDetailValue`, `SetCellValue`, `SetItemValue`, `SetListValues`, `SetShopkeeperQuantity`,
  `AddShopkeeperEntry`, `RemoveShopkeeperEntry`;
- `BuildItemHeader`, `RebuildItemList`, `ApplyItemFilter`, `ItemListFilterPresets`;
- `LoadItemForm`, `AddFieldGroups`, `RebuildSections`, `BuildSubTableSections`;
- `BuildLanguageOptions`, `ChangeLanguageAsync`, `ReloadPreservingSelectionAsync`;
- `LoadGraphicImageThumbnailAsync`, `LoadImagesAsync`, `CancelImageLoads`;
- `Undo`, `Redo`, `ValidateAsync`, `NavigateToValidationIssue`, `NavigateToRelation`;
- `CreateSubTableRow`, `AddNestedGroup`, `InferNestedType`, `ParseClassBits`.

Сложность обусловлена не одной функцией, а комбинацией: master-detail, 1:1/1:N подтаблицы,
shopkeeper reverse relations, class visibility, translation, images, filters и shared undo.

### 8.5 `MercGearWorkspaceViewModel` (1132 строки)

Редактор `Inventory.MercStartingGear`:

- `InitializeAsync`/`InitializeCoreAsync`, `SaveAsync`, `Reload`, `Undo`, `Redo`, `Validate`;
- `RebuildMercList`, `ApplyMercFilter`, `LoadMercForm`;
- `BuildKit`, `ReadKitState`, `GetKitValue`, `RefreshKit`, `RefreshAllKits`;
- `BuildSlotDefinitions`/`SlotDefinition`;
- `ChangeLanguageAsync`.

Модель: один merc-record, пять `GEARKIT`-комплектов, слоты; `MercGearKitViewModel` и
`MercGearSlotViewModel` представляют UI-состояние.

### 8.6 `NestedTableWorkspaceViewModel` (1620 строк)

Редактор таблиц с вложенными структурами (`Map.Cities`, `Map.FacilityTypes`):

- `InitializeAsync`, `SaveAsync`/`SaveWithRecoveryAsync`, `Reload`, `Undo`, `Redo`, `Validate`;
- `AddRecord`, `DeleteRecord`, `AddAssignment`, `DeleteSection`, `AddBlock`;
- `BuildForm`, `AddRootSections`, `AddMainSections`, `AddNestedLeafSections`,
  `AddRepeatedBlockSections`, `AddNestedCollectionSections`;
- `SetRootValue`, `SetMainValue`, `SetNestedField`, `SetNestedListValues`;
- `ResolveAssignmentSegmentIndex`, `IsRepeatedBlock`, `IsKeySegment`, `RebuildBlockActions`;
- `ChangeLanguageAsync`.

### 8.7 Dataset, compare, copy, settings

- `DatasetViewModel` — окно проверки/поиска: `InitializeAsync`, `ValidateAsync`, `CancelValidation`,
  `ApplyReport`, `ApplyIssueFilter`, `OpenIssue`, `ExportAsync`, `SearchAsync`, `OpenHit`;
  использует `DatasetValidator`, `DatasetSearchService`, `DatasetValidationReportFormatter`,
  `IDataSetSession`.
- `CompareViewModel` — `SetInitialDirectory`, `BrowseLeft/Right`, `CompareAsync`, `CancelCompare`,
  `ExportAsync`, `ShowReport`; использует `ITableComparisonService`.
- `ComparisonItems.cs` — display-модели отчёта: `ComparisonTableItem`, `ComparisonFieldItem`,
  `ComparisonLookupItem`, `ComparisonRecordItem`, `ComparisonValueItem`, `ComparisonOverlayItem`,
  `ComparisonOverlayCellItem`.
- `TableCopyViewModel` — `InitializeAsync`, `PreviewAsync`, `ApplyAsync`, `BrowseSource/Target`,
  `ParseRecordKeys`, `DescribeAction`, `DescribeRemaps`; использует `ITableCopyService`.
- `SettingsViewModel` — каталоги, тема, UI-язык, `ResetLayouts`, `CreateResult`;
  `SettingsDirectoryItem`, `UiLanguageOption`, `ThemeOption`.
- `TranslationReportViewModel`, `ValidationIssueItem`, `LookupIdViewModel`,
  `LookupUsageViewModel`, `BulkEditViewModel`, `EnemyAmmoViewModel`, `ItemCreationViewModel`.

### 8.8 Views, converters, behaviors, themes

XAML-слой в основном следует MVVM:

| View | ViewModel/роль |
|---|---|
| `App.xaml` | application resources, startup/theme dictionaries |
| `MainWindow.xaml` | `MainViewModel`, shell, дерево/список таблиц |
| `TableView.xaml` | `TableEditorViewModel`, динамические колонки и grid |
| `ItemWorkspaceView.xaml` | `ItemWorkspaceViewModel` |
| `MercGearWorkspaceView.xaml` | `MercGearWorkspaceViewModel` |
| `NestedTableWorkspaceView.xaml` | `NestedTableWorkspaceViewModel` |
| `XmlTextView.xaml` | read-only XML snapshot |
| `FieldEditor.xaml` | detail-поле, lookup/relation pickers |
| `RelationPicker.xaml` | кастомный picker с поиском |
| `DatasetWindow.xaml` | проверка/поиск датасета |
| `CompareWindow.xaml` | сравнение версий |
| `TableCopyWindow.xaml` | копирование строк |
| `SettingsWindow.xaml` | настройки |
| `AboutWindow`, `BitEditorWindow`, `BulkEditWindow`, `EnemyAmmoWindow`, `ExternalChangeWindow`, `ImageViewerWindow`, `ItemCreationWindow`, `LookupIdWindow`, `LookupUsageWindow`, `TranslationReportWindow`, `UnsavedChangesWindow` | диалоги |

`TableView.xaml.cs` (679 строк) — самый тяжёлый code-behind: строит колонки, шаблоны
lookup/relation/thumbnail, синхронизирует ширины/порядок, обрабатывает sorting/selection/editing.
`RelationPicker.xaml.cs` (332 строки) — кастомный контрол с popup, клавиатурой и commit typed
value. Остальные code-behind — тонкие обработчики кнопок/закрытия.

Вспомогательные типы:

- `DataGridNavigation` — attached behavior для page up/down и навигации;
- `ValidationIssueNavigation` — attached behavior перехода из issue в grid;
- `ShortcutCatalog`, `GestureTextExtension`, `ShortcutTipExtension` — централизованные
  горячие клавиши и подсказки;
- `ThemeManager` — применяет dark/light/system, `DwmSetWindowAttribute`, chrome;
- `Resources/Strings.resx` и `Strings.ru.resx` — UI-строки; `Strings` читает их через
  `ResourceManager` и явную UI-культуру;
- `Themes/Theme.Dark.xaml`, `Theme.Light.xaml`, `Controls.xaml`, `Icons.xaml` — словари стилей;
- `Assets/app.ico` — иконка exe и окна;
- `RgbaBufferConverter` — RGBA → BGRA/BitmapSource;
- `GridValueConverters`, `TranslationStateConverter`, `CharacterCounterConverter`,
  `InverseBooleanToVisibilityConverter` — XAML-конвертеры;
- `CharacterCounter`, `DetailTextLimits` — лимиты ввода/счётчики.

## 9. Build, scripts, workflows, packaging

- `xml-editor-reloaded/XmlEditorReloaded.slnx` — solution: пять production-проектов, пять
  xUnit-проектов и console-проект `XmlEditorReloaded.UiSmoke`.
- `xml-editor-reloaded/Directory.Build.props` — `net10.0`, nullable, implicit usings,
  code-style enforcement, version `1.0.0`, `ArtifactsPath = out/`, sandbox workarounds.
- `xml-editor-reloaded/.editorconfig` — стиль C#/XAML; `.gitignore`/`.gitattributes` — гигиена
  репозитория и line endings.
- `src/*/*.csproj` — пять проектов; App использует `CommunityToolkit.Mvvm 8.4.0` и
  `Microsoft.Extensions.DependencyInjection 10.0.0`; Data — `Microsoft.Data.Sqlite 10.0.0` и
  override `SQLitePCLRaw.lib.e_sqlite3 2.1.14`; SQL-миграции — embedded resources.
- `util-scripts/test.ps1` — restore/build с `-m:1` (sandbox), затем последовательный запуск
  пяти `*.Tests.csproj` с `--no-build --no-restore`; отдельный `.tmp/run-*` на прогон.
- `util-scripts/release.ps1` — проверка версии, тесты, два `dotnet publish` (self-contained и
  framework-dependent), проверка runtime/PDB/version, staging документов и launcher, ZIP,
  SHA-256.
- `util-scripts/clean-tmp.ps1` — безопасная очистка `.tmp` с keep-политикой.
- `util-scripts/manual-smoke.ps1`/`.cmd` — ручной smoke на копии `References/`, отдельный
  `XER_APP_DATA`.
- `.github/workflows/ci.yml` — restore/build/test для `main`, `dev`, `release/**`; без artifacts.
- `.github/workflows/release.yml` — `workflow_dispatch` из `release/<version>` для pre-release и
  push `v<version>` для финального GitHub Release; `checkout@v5`, `setup-dotnet@v5`,
  `upload-artifact@v6`.
- `packaging/start-portable.cmd` — `XER_APP_DATA=%~dp0data`, запуск exe рядом.
- `README.md`/`README.ru.md` — GitHub-обзор; `RELEASING.md` — branch/release процесс;
  `USER-GUIDE.md` — английская инструкция, `USER-GUIDE-RU.md` — русская; `CHANGELOG.md`,
  `LICENSE`, `THIRD-PARTY-NOTICES.md`.

## 10. Сквозные сценарии взаимодействия

### 10.1 Старт приложения

```
App.OnStartup
  → AppPaths / StartupTrace / StartupDiagnostics
  → OperationLog.Sink = FileOperationLog
  → ConfigureServices → ServiceProvider
  → DatabaseMigrator.MigrateAsync
  → ISettingsStore.LoadAsync
  → Strings.SetUiCulture
  → MainWindow.ApplySavedPlacement
  → MainWindow.Show
  → MainViewModel.InitializeAsync
       → settings → DataDirectoryItem → DataSetSessionFactory.OpenAsync
       → FileSystemTableCatalog.GetTablesAsync
       → TableTreeViewModel.Load
```

### 10.2 Открытие таблицы

```
MainViewModel.OpenTableAsync / LoadSelectedTableAsync
  → ITableEditorFactory.Create / CreateItemWorkspace / CreateMercGearWorkspace / CreateNestedWorkspace
  → TableEditorViewModel.InitializeAsync (или InitializeFromWorkspaceAsync)
       → IDataSetSession.OpenTableAsync / OpenTableSetAsync
       → TableWorkspace.LoadAsync / TableSetWorkspace.LoadAsync
       → TableFileLoader.LoadAsync
       → TableDocument (XDocument + records)
       → TableSchemaBuilder.Build
       → RebuildRows / BuildColumns
```

### 10.3 Правка ячейки

```
TableView cell commit
  → TableEditorViewModel.SetCellValue
  → TableWorkspace.SetValue
  → TableEditSession.SetValue (base/overlay routing)
  → TableRecord.SetFieldValue
  → TableDocument.MarkDirty + ChangeTracker.Record
  → ElementSnapshotState.Capture / CompleteAfter
  → TableEditHistory.Record(transaction)
  → RefreshRows / RefreshFieldValues
```

### 10.4 Сохранение

```
IEditorTab.SaveWithRecoveryAsync
  → TableWorkspace.SaveAsync
  → TableEditSession.SaveAsync
  → TableFileSaver.SaveAsync
  → LosslessDocumentPatcher.TryPatch (fallback DocumentSerializer)
  → CanReparse
  → SHA-256 compare
  → temp file + File.Move
  → TableDocument.AcceptSaved
  → ApplySavedState / RelationResolver.Invalidate
```

При `xml.file_changed_externally` UI показывает `ExternalChangeWindow`: Reload, Overwrite или
Cancel. `forceOverwrite` используется только после явного выбора.

### 10.5 Перевод

```
LanguageOptionViewModel changed
  → TableEditorViewModel/ItemWorkspace/Nested ChangeLanguageAsync
  → DataSetSession.EnsureOverlayAsync (если overlay ещё нет)
  → TableWorkspace.LoadAsync(language)
  → OverlayDocument.LoadAsync/CreateAsync
  → OverlayMerger.Merge
  → TableEditSession.SetValue
       translatable → OverlayWriter → OverlayDocument
       ordinary     → TableRecord → base TableDocument
  → SaveAsync: base и overlay независимо
```

### 10.6 Item workspace

```
ItemWorkspaceViewModel.InitializeCoreAsync
  → DataSetSession.OpenTableSetAsync(includeReverseRelations: true)
  → TableSetWorkspace.LoadAsync
       → primary TableWorkspace
       → relation members (weapon/magazine/attachment/shopkeeper/...)
       → shared TableEditHistory
  → LoadClassNamesAsync / LoadFieldSourcesAsync
  → InitializeChildEditorsAsync
  → RebuildItemList / LoadItemForm / RebuildSections
```

`TableSetWorkspace` держит общую историю и сохраняет все dirty-файлы; `ItemWorkspaceViewModel`
только координирует UI и вызывает `SetValue`/`SetNestedValue`/`DeleteRow`.

### 10.7 Сравнение и copy

```
CompareViewModel.CompareAsync
  → TableDataComparer.CompareAsync
       → TableFileLoader.LoadAsync обеих сторон
       → TableSnapshotAnalyzer / TableCellsReader
       → TableValueComparer
       → OverlayFileComparer
       → TableComparisonReport
  → ComparisonItems → CompareWindow / Export

TableCopyViewModel.PreviewAsync
  → TableCopyService.PreviewAsync
       → TableCopyRemapper (lookup/relation/key)
       → TableCopyPreview (без записи)
TableCopyViewModel.ApplyAsync
  → TableCopyService.ApplyAsync
       → plan → clone → save target / backup restore
```

### 10.8 Проверка и поиск

```
DatasetViewModel.ValidateAsync
  → DatasetValidator.ValidateAsync
       → TableFileLoader.LoadAsync per table
       → TableValidator.ValidateAsync
       → DatasetValidationReport
  → ApplyReport / ExportAsync

DatasetViewModel.SearchAsync
  → DatasetSearchService.SearchAsync
       → TableFileLoader + XDocument traversal
       → DatasetSearchReport
  → OpenHit → MainViewModel → target editor
```

### 10.9 Изображения

```
WpfItemImageService.GetImageAsync
  → ItemImageCatalog.GetImageAsync
       → Resolve path (Big/Medium/Small)
       → DecodedFrameCache.TryGet
       → StiDecoder.LoadFrameAsync
            → StciParser.ParseAsync
            → EtrleDecoder.DecodeFrame
       → ImageTransparency.ApplyGreenKey
  → RgbaBuffer → RgbaBufferConverter.ToBitmapSource
```

## 11. Почему решения именно такие

1. **Lossless-сохранение вместо полной сериализации.** Игра чувствительна к форматированию,
   порядку и лексике; `XDocument` + `PreserveWhitespace` + байтовый patcher сохраняют то, что
   сериализатор изменил бы. Fallback на serializer оставлен для структурных изменений, которые
   нельзя безопасно выразить splice-ем.
2. **Sparse-записи и `TableRecord`-фасад.** DTO со всеми колонками добавил бы отсутствующие
   элементы и изменил файл. `TableRecord` работает с реальными `XElement` и создаёт поля только
   по явному запросу.
3. **Metadata-driven + document-driven.** Метабаза даёт проверенные описания, но не является
   жёсткой схемой: таблица без metadata всё равно открывается, а новые поля сохраняются. Это
   важно для v4/v5 и будущих модов.
4. **Оверлеи только для переводов.** База и перевод живут в разных файлах; правка перевода не
   меняет base. Это соответствует формату игры и позволяет сравнивать/валидировать их отдельно.
5. **`OperationResult` для ожидаемых ошибок.** Ошибки файлов/XML/ключей — нормальный сценарий
   для редактора; исключения оставлены для нарушений контракта. UI показывает `error_code` и
   `error_text`, тесты проверяют коды.
6. **SQLite + embedded migrations.** Метабаза воспроизводима, версионируется и не требует
   ручного SQL; миграции лежат в сборке и применяются при старте.
7. **Raw RGBA в Imaging.** Backend не зависит от WPF и `System.Drawing`; UI сам решает, как
   показать буфер. Это упрощает тестирование STI/ETRLE.
8. **MVVM + service interfaces.** View models не создают окна напрямую; `IDialogService`,
   `IItemImageService`, `IVersionCompareService`, `ITableCopyDialogService` позволяют тестировать
   сценарии без реального UI.
9. **Общая история для table set.** Item и его подтаблицы — одно пользовательское действие;
   одна транзакция undo/redo сохраняет согласованность.
10. **Два release-варианта.** Self-contained удобен пользователю, framework-dependent — меньше;
    release-скрипт проверяет наличие/отсутствие runtime и версию exe.
11. **`XER_APP_DATA`.** Позволяет portable-режиму и UI-smoke работать без записи в профиль
    пользователя, что критично в песочнице и на CI.
12. **Явный external-change gate.** Редактор не перезаписывает файл, изменённый другим
    процессом, без выбора пользователя; это защита от потери чужих правок.

## 12. Что можно улучшить

Ниже — не список «переписать всё», а приоритизированные точки, которые дадут наибольший эффект.

### P0 — корректность данных

1. **Атомарность сохранения table set.** `TableSetWorkspace.SaveDetailedAsync` сохраняет файлы
   по одному; при сбое посередине часть файлов уже заменена. Нужен journal/backup всех target-
   файлов, commit после успешной записи всех, restore при ошибке. Сейчас корректность частично
   компенсируется `TableCopyService` (там есть restore), но не общим save.
2. **Проверка внешних изменений по SHA-256, а не только `(length, mtime)`.** `OriginalSha256`
   уже есть, но `TableFileSaver.DetectExternalChange` сравнивает длину и mtime. Одинаковый размер
   и сохранённый mtime могут скрыть изменение. Перед записью стоит хешировать текущий файл.
3. **Единая очередь асинхронных операций редактора.** `ChangeLanguageAsync` вызывается из
   setter как fire-and-forget, `OnEditorLanguageChanged` делает `_ = PersistLanguageAsync()`.
   Гонка языка уже исправлена сериализацией store, но общий паттерн остаётся хрупким. Нужен
   `AsyncCommand`/serial queue с отменой и наблюдаемыми ошибками.
4. **Транзакционность copy на уровне всех затронутых файлов.** Проверить и покрыть тестами
   restore при сбое после первого `File.Move`; сейчас контракт обещает восстановление, это
   критичный участок.
5. **Явная политика backup при сохранении.** Автоматического backup XML нет; можно добавить
   опциональный `.bak`/journal рядом с файлом или в `XER_APP_DATA` (с настройкой), не меняя
   формат игры.

### P1 — сопровождаемость

6. **Разделить крупные view models.** `ItemWorkspaceViewModel` (3385), `TableEditorViewModel`
   (3068), `NestedTableWorkspaceViewModel` (1620), `MercGearWorkspaceViewModel` (1132) и
   `MainViewModel` (1740) совмещают слишком много обязанностей. Естественные границы:
   `EditorSaveCoordinator`, `EditorLanguageCoordinator`, `EditorFilterController`,
   `ItemListController`, `ItemFormBuilder`, `ItemImageController`, `TableSetSaveCoordinator`.
7. **Вынести общий base/композицию для `IEditorTab`.** Четыре редактора повторяют
   `SaveWithRecoveryAsync`, `Reload`, `Undo/Redo`, `Validate`, `ChangeLanguage`,
   `RefreshHistoryState`, external-change handling. Базовый класс или набор сервисов уберёт
   расхождения.
8. **Унифицировать lookup/relation pickers.** `LookupColumnData`, `RelationColumnData`,
   `AllowedValueOption`, `RelationOption` и `RelationPicker` решают похожие задачи. Один
   `ChoiceSource`/`ChoiceItem` с разными провайдерами сократит дублирование.
9. **Убрать дублирование сканирования датасета.** `DatasetValidator` и `DatasetSearchService`
   отдельно обходят таблицы, загружают metadata и overlays. Общий `DatasetScanner` с
   callback-ами уменьшит I/O и расхождения.
10. **Типизированные error codes.** `ErrorCodes` — строковые константы; опечатка не ловится
    компилятором. `readonly record struct ErrorCode` или enum + converter к строке сохранит
    совместимость и даст проверку.
11. **Сузить `ITableMetadataProvider`.** Один интерфейс с 10+ методами сложно подменять;
    разделить на `ITableMetadataReader`, `ILanguageMetadataReader`, `IRelationMetadataReader`,
    `ILookupMetadataReader`.
12. **Инъекция `IOperationLog` и локализации.** `OperationLog` — статический sink, `Strings` —
    статический `ResourceManager`. В тестах и при нескольких экземплярах это глобальное
    состояние; лучше DI-сервисы.

### P2 — производительность и память

13. **Убрать N+1 в `SqliteMetadataProvider.GetAllTablesAsync`.** Сейчас строки читаются одним
    запросом, затем `ReadTableDetailsAsync` вызывается на каждую таблицу. Можно загрузить
    связанные данные batch-запросами или кэшировать `TableMetadata` после первого чтения.
14. **Ленивая копия `OriginalXml`.** `TableDocument` держит и `XDocument` current, и полную
    копию original, и `OriginalBytes`. Для больших таблиц это тройная память. Можно хранить
    original bytes и строить дерево только при необходимости patch/undo.
15. **Ограничить параллелизм загрузки изображений.** Кэш кадров уже bounded-LRU, но UI может
    одновременно запускать несколько декодирований/миниатюр; семафор на небольшое число
    параллельных задач снизит конкуренцию за диск и CPU на больших item-списках.
16. **XML snapshot по требованию.** `CreateXmlSnapshot` сериализует весь документ и повторно
    парсит его для line spans. Для больших таблиц это дорого; делать только при открытии XML-
    вкладки/поиске, с отменой и индикатором.
17. **Кэш каталога и metadata на уровне приложения.** `FileSystemTableCatalog.GetTablesAsync`
    сканирует диск при каждом открытии dataset; каталог можно инвалидировать по mtime корня.

### P3 — качество, тесты, поставка

18. **Fuzz/property-тесты для round-trip.** 726 тестов — хорошая база, но lossless-патчер и
    overlay key resolver стоит проверить случайными XML-структурами (FsCheck/Property-Based)
    на инвариант «load → save без правок = byte-identical».
19. **Покрытие и статические анализаторы.** Добавить coverlet/ReportGenerator и порог покрытия
    для Core/Xml; включить `Microsoft.CodeAnalysis.NetAnalyzers` с `-warnaserror`. Сейчас есть
    code-style enforcement, но не все анализаторы.
20. **UI-smoke в CI.** 98 сценариев не входят в CI, потому что нужен интерактивный desktop.
    Можно выделить headless-часть (ViewModel + файловые проверки) в обычные тесты, а UI-smoke
    оставить nightly/self-hosted.
21. **Подпись и обновление.** Сборка не подписана, SmartScreen предупреждает; нет installer/
    auto-update. Для публичного продукта — code signing, portable updater или MSIX.
22. **Синхронизация двуязычных руководств.** `USER-GUIDE.md` и `USER-GUIDE-RU.md` нужно
    обновлять вместе; добавить проверку ссылок/скриншотов и явный чек-лист в release-процесс.
23. **SBOM и проверка зависимостей.** `THIRD-PARTY-NOTICES` есть, но нет SBOM/cyclic dependency
    audit в release; можно генерировать `dotnet sbom`/CycloneDX.
24. **Документация API.** Публичные backend-типы имеют XML-doc, но часть internal-классов
    (например, `LosslessDocumentPatcher`) описана только summary; для сложных алгоритмов нужны
    короткие invariants/диаграммы в `docs/`.

## 13. Итоговая оценка

Сильные стороны:

- чёткое разделение Core/Xml/Data/Imaging/App;
- lossless-подход и защита от потери XML;
- metadata-driven схема с document fallback;
- продуманные оверлеи и undo;
- `OperationResult` и разнообразные error codes;
- хорошее покрытие тестами и продуманная упаковка;
- воспроизводимый release и branch-модель.

Главные риски:

- чрезмерно крупные view models и дублирование сценариев сохранения/языка/фильтров;
- нет полной транзакционности multi-file save;
- внешние изменения проверяются недостаточно строго;
- N+1 metadata queries и дорогие snapshot/копии деревьев;
- UI-smoke не входит в CI, а installer/signing отсутствуют.

Если выбирать три улучшения на ближайший цикл: **(1)** транзакционный save table set, **(2)**
выделение общего editor-session координатора и разбиение `ItemWorkspaceViewModel`, **(3)**
hash-based external-change check + fuzz round-trip. Они дают максимальный эффект на надёжность
данных и стоимость дальнейших изменений.

---

## Приложение. Полный инвентарь `.cs`-файлов

Ниже перечислены все production-файлы `src/` (без тестов). Для каждого файла указаны объявленные
типы и извлечённые имена методов; конструкторы входят в список, свойства/события и методы
интерфейсов без тела не перечислены. Это навигационный индекс, а не замена точным сигнатурам:
для контрактов и XML-doc используйте исходник.

### XmlEditorReloaded.App (117 файлов)

- `XmlEditorReloaded.App/App.xaml.cs` — типы: class `App`. Методы: ConfigureServices, OnStartup, OnExit, RegisterGlobalExceptionHandlers, OnDispatcherUnhandledException, OnAppDomainUnhandledException, OnUnobservedTaskException, ShowFatalError, BuildErrorMessage, BuildErrorMessage
- `XmlEditorReloaded.App/AssemblyInfo.cs` — типы: —. Методы: —
- `XmlEditorReloaded.App/Behaviors/DataGridNavigation.cs` — типы: class `DataGridNavigation`. Методы: GetIsEnabled, SetIsEnabled, OnIsEnabledChanged, OnPreviewKeyDown, PageRowCount, SelectRow, IsTextEditing
- `XmlEditorReloaded.App/Imaging/RgbaBufferConverter.cs` — типы: class `RgbaBufferConverter`. Методы: ToBgra32, ToBitmapSource
- `XmlEditorReloaded.App/MainWindow.xaml.cs` — типы: class `MainWindow`. Методы: MainWindow, ApplySavedPlacement, CapturePlacement, OnClosing, OnExitMenuClick, OnTableTreeSelectedItemChanged, OnTableTreeItemDoubleClick, OnTableTreePreviewKeyDown, OnFindInTreeClick, OnFoldAllClick, OnUnfoldAllClick, FindInTree, SelectTreeNode, Contains, OnPreviewKeyDown, OnTablesGridDoubleClick
- `XmlEditorReloaded.App/Resources/CharacterCounter.cs` — типы: class `CharacterCounter`. Методы: Format
- `XmlEditorReloaded.App/Resources/Strings.cs` — типы: class `Strings`. Методы: SetUiCulture, Get, Format
- `XmlEditorReloaded.App/Services/AllowedValueOption.cs` — типы: record `AllowedValueOption`. Методы: AllowedValueOption, ToString, GetDisplay, Resolve
- `XmlEditorReloaded.App/Services/BulkEditContext.cs` — типы: class `BulkEditContext`; record `BulkEditColumnOption`. Методы: BulkEditContext, BulkEditColumnOption
- `XmlEditorReloaded.App/Services/DataSetPaths.cs` — типы: class `DataSetPaths`. Методы: ResolveTableDataDirectory, ResolveGameDataDirectory
- `XmlEditorReloaded.App/Services/DataSetSession.cs` — типы: class `DataSetSession`; class `DataSetSessionFactory`. Методы: DataSetSession, OpenTableAsync, GetLanguagesAsync, OpenTableSetAsync, OpenTableSetAsync, GetRelationsAsync, EnsureOverlayAsync, DataSetSessionFactory, OpenAsync
- `XmlEditorReloaded.App/Services/DatasetNames.cs` — типы: class `DatasetNames`. Методы: LabelFor
- `XmlEditorReloaded.App/Services/FieldSourceSet.cs` — типы: class `FieldSourceSet`. Методы: FieldSourceSet, LoadAsync
- `XmlEditorReloaded.App/Services/IDataSetSession.cs` — типы: interface `IDataSetSession`; interface `IDataSetSessionFactory`. Методы: —
- `XmlEditorReloaded.App/Services/IDialogService.cs` — типы: enum `UnsavedChangesChoice`; enum `ExternalChangeChoice`; interface `IDialogService`. Методы: —
- `XmlEditorReloaded.App/Services/IImageViewerService.cs` — типы: interface `IImageViewerService`. Методы: —
- `XmlEditorReloaded.App/Services/IItemImageService.cs` — типы: interface `IItemImageService`. Методы: —
- `XmlEditorReloaded.App/Services/ISettingsDialogService.cs` — типы: record `SettingsRequest`; record `SettingsResult`; interface `ISettingsDialogService`. Методы: SettingsRequest, SettingsResult
- `XmlEditorReloaded.App/Services/ITableCopyDialogService.cs` — типы: record `TableCopyDialogRequest`; record `TableCopyDialogOutcome`; interface `ITableCopyDialogService`. Методы: TableCopyDialogRequest, TableCopyDialogOutcome
- `XmlEditorReloaded.App/Services/ITableEditorFactory.cs` — типы: interface `ITableEditorFactory`; class `TableEditorFactory`. Методы: TableEditorFactory, Create, CreateItemWorkspace, CreateMercGearWorkspace, CreateNestedWorkspace
- `XmlEditorReloaded.App/Services/IVersionCompareService.cs` — типы: interface `IVersionCompareService`. Методы: —
- `XmlEditorReloaded.App/Services/ItemCreationRequest.cs` — типы: record `ItemCreationRequest`. Методы: ItemCreationRequest
- `XmlEditorReloaded.App/Services/LookupIdEditRequest.cs` — типы: record `LookupIdEditRequest`. Методы: LookupIdEditRequest
- `XmlEditorReloaded.App/Services/WpfDialogService.cs` — типы: class `WpfDialogService`. Методы: PickFolder, PickSaveFile, Confirm, ShowMessage, ConfirmUnsavedChanges, ResolveExternalChange, EditBits, ShowAbout, ShowTranslationReport, ShowDataset, ShowEnemyAmmo, EditBulk, CreateItem, EditLookupId, ShowLookupUsages
- `XmlEditorReloaded.App/Services/WpfImageViewerService.cs` — типы: class `WpfImageViewerService`. Методы: WpfImageViewerService, Show
- `XmlEditorReloaded.App/Services/WpfItemImageService.cs` — типы: class `WpfItemImageService`. Методы: GetImageAsync, GetFrameCountAsync
- `XmlEditorReloaded.App/Services/WpfSettingsDialogService.cs` — типы: class `WpfSettingsDialogService`. Методы: WpfSettingsDialogService, Show
- `XmlEditorReloaded.App/Services/WpfTableCopyDialogService.cs` — типы: class `WpfTableCopyDialogService`. Методы: WpfTableCopyDialogService, ShowAsync
- `XmlEditorReloaded.App/Services/WpfVersionCompareService.cs` — типы: class `WpfVersionCompareService`. Методы: WpfVersionCompareService, Show
- `XmlEditorReloaded.App/Shortcuts/GestureTextExtension.cs` — типы: class `GestureTextExtension`. Методы: ProvideValue
- `XmlEditorReloaded.App/Shortcuts/ShortcutCatalog.cs` — типы: class `ShortcutCatalog`. Методы: Get, GetText, Format, KeyText
- `XmlEditorReloaded.App/Shortcuts/ShortcutTipExtension.cs` — типы: class `ShortcutTipExtension`. Методы: ProvideValue
- `XmlEditorReloaded.App/Themes/ThemeManager.cs` — типы: class `ThemeManager`. Методы: Apply, ApplyWindowChrome, OnWindowSourceInitialized, DwmSetWindowAttribute, SystemPrefersDark, FindThemeDictionary
- `XmlEditorReloaded.App/ViewModels/AboutViewModel.cs` — типы: class `AboutViewModel`. Методы: AboutViewModel, ReadInformationalVersion
- `XmlEditorReloaded.App/ViewModels/BitEditorViewModel.cs` — типы: class `BitEditorViewModel`; class `BitItem`. Методы: BitEditorViewModel, ComposeValue, BitItem
- `XmlEditorReloaded.App/ViewModels/BulkEditViewModel.cs` — типы: class `BulkEditViewModel`; class `BulkOperationOption`; class `BulkEditPreviewRowViewModel`. Методы: BulkEditViewModel, BeginOperandEdit, EndOperandEdit, CreateRequest, RefreshPreview, BulkOperationOption, BulkEditPreviewRowViewModel
- `XmlEditorReloaded.App/ViewModels/CompareViewModel.cs` — типы: class `CompareViewModel`. Методы: CompareViewModel, SetInitialDirectory, CancelRunningComparison, BrowseLeft, BrowseRight, CompareAsync, CanCompare, CancelCompare, CanCancelCompare, ExportAsync, CanExport, ShowReport
- `XmlEditorReloaded.App/ViewModels/ComparisonItems.cs` — типы: class `ComparisonTableItem`; class `ComparisonFieldItem`; class `ComparisonLookupItem`; class `ComparisonRecordItem`; class `ComparisonValueItem`; class `ComparisonOverlayItem`; class `ComparisonOverlayCellItem`; class `ComparisonDisplayText`. Методы: ComparisonTableItem, ResolveStatusText, BuildProfileText, AppendSide, ResolveSideStatus, BuildDiagnosticsText, AppendDiagnostics, FormatCount, FormatValues, ComparisonFieldItem, FormatType, FormatPresence, ComparisonLookupItem, FormatAvailability, ComparisonRecordItem, ComparisonValueItem, FormatDifference, ComparisonOverlayItem, FormatSide, ComparisonOverlayCellItem, FormatValue
- `XmlEditorReloaded.App/ViewModels/DataDirectoryItem.cs` — типы: class `DataDirectoryItem`. Методы: DataDirectoryItem, ToString
- `XmlEditorReloaded.App/ViewModels/DatasetItems.cs` — типы: class `DatasetIssueItem`; class `DatasetSearchHitItem`; class `DatasetDisplayText`. Методы: DatasetIssueItem, Matches, DatasetSearchHitItem, SeverityLabel
- `XmlEditorReloaded.App/ViewModels/DatasetViewModel.cs` — типы: enum `DatasetWindowTab`; record `DatasetNavigationRequest`; record `DatasetSeverityOption`; class `DatasetViewModel`. Методы: DatasetNavigationRequest, DatasetSeverityOption, DatasetViewModel, InitializeAsync, CancelRunningOperations, SelectFirstIssue, SelectFirstHit, ValidateAsync, CanValidate, CancelValidation, CanCancelValidation, OnValidationProgress, ApplyReport, ApplyIssueFilter, OpenIssue, CanOpenIssue, ExportAsync, CanExport, SearchAsync, CanSearch, CancelSearch, CanCancelSearch, OnSearchProgress, OpenHit, CanOpenHit
- `XmlEditorReloaded.App/ViewModels/DetailFieldGroupViewModel.cs` — типы: class `DetailFieldGroupViewModel`. Методы: DetailFieldGroupViewModel
- `XmlEditorReloaded.App/ViewModels/DetailFieldViewModel.cs` — типы: class `DetailFieldViewModel`. Методы: DetailFieldViewModel, BeginValueEdit, EndValueEdit, Refresh, EditBits, GoToTarget
- `XmlEditorReloaded.App/ViewModels/DetailImagePanelViewModel.cs` — типы: class `DetailImagePanelViewModel`. Методы: DetailImagePanelViewModel
- `XmlEditorReloaded.App/ViewModels/DetailTextLimits.cs` — типы: class `DetailTextLimits`. Методы: Resolve
- `XmlEditorReloaded.App/ViewModels/EnemyAmmoViewModel.cs` — типы: class `EnemyAmmoViewModel`; record `EnemyAmmoRowViewModel`; class `EnemyAmmoCellViewModel`. Методы: EnemyAmmoViewModel, InitializeAsync, SaveAsync, ReloadAsync, Save, Reload, TrySetValue, AddWarning, EnemyAmmoRowViewModel, EnemyAmmoCellViewModel
- `XmlEditorReloaded.App/ViewModels/FilterCriterionViewModel.cs` — типы: class `FilterOperatorOption`; class `FilterLogicalOperatorOption`; class `FilterCriterionViewModel`. Методы: FilterOperatorOption, FilterLogicalOperatorOption, FilterCriterionViewModel, BeginOperandEdit, EndOperandEdit, Remove, ToCriterion, Load, RebuildAllowedValues, RebuildOperators, GetSupportedOperators, GetOperatorText
- `XmlEditorReloaded.App/ViewModels/GraphicTypeOption.cs` — типы: class `GraphicTypeOption`. Методы: GraphicTypeOption
- `XmlEditorReloaded.App/ViewModels/IDetailFieldHost.cs` — типы: interface `IDetailFieldHost`. Методы: —
- `XmlEditorReloaded.App/ViewModels/IEditorTab.cs` — типы: interface `IEditorTab`. Методы: —
- `XmlEditorReloaded.App/ViewModels/ITableCellEditor.cs` — типы: interface `ITableCellEditor`. Методы: —
- `XmlEditorReloaded.App/ViewModels/ImageViewerViewModel.cs` — типы: class `ImageViewerViewModel`. Методы: ImageViewerViewModel, InitializeAsync, RefreshAsync, PreviousItem, NextItem, LoadImagesSafeAsync, LoadImagesAsync, Apply, ClearImages, MoveSelection, BeginLoad
- `XmlEditorReloaded.App/ViewModels/ItemChildSectionViewModel.cs` — типы: class `ItemChildSectionViewModel`. Методы: ItemChildSectionViewModel, RefreshCount
- `XmlEditorReloaded.App/ViewModels/ItemCreationViewModel.cs` — типы: class `ItemCreationViewModel`. Методы: ItemCreationViewModel, ToRequest
- `XmlEditorReloaded.App/ViewModels/ItemGraphicsOption.cs` — типы: class `ItemGraphicsOption`. Методы: ItemGraphicsOption, FromRecord, ParseNonNegative
- `XmlEditorReloaded.App/ViewModels/ItemImageOption.cs` — типы: class `ItemImageOption`. Методы: ItemImageOption, RequestThumbnail, ConfigureThumbnail, LoadThumbnailAsync
- `XmlEditorReloaded.App/ViewModels/ItemListEntry.cs` — типы: class `ItemListEntry`. Методы: ItemListEntry, RequestThumbnail, ConfigureThumbnail, InvalidateThumbnail, LoadThumbnailAsync
- `XmlEditorReloaded.App/ViewModels/ItemListFilterPresets.cs` — типы: class `ItemListFilterOption`; class `ItemListFilterPresets`. Методы: ItemListFilterOption, CreateClassOptions, CreateFlagOptions, Criterion, RocketLauncherCriterion, All, Any
- `XmlEditorReloaded.App/ViewModels/ItemListSectionViewModel.cs` — типы: class `ItemListSectionViewModel`; class `ItemListValueViewModel`. Методы: ItemListSectionViewModel, Add, Remove, Apply, ItemListValueViewModel
- `XmlEditorReloaded.App/ViewModels/ItemSubTableSectionViewModel.cs` — типы: class `ItemSubTableSectionViewModel`. Методы: ItemSubTableSectionViewModel, Create
- `XmlEditorReloaded.App/ViewModels/ItemWorkspaceViewModel.cs` — типы: class `ItemWorkspaceViewModel`. Методы: ItemWorkspaceViewModel, CreateXmlSnapshot, InitializeAsync, InitializeCoreAsync, SaveAsync, SaveWithRecoveryAsync, ReloadFromDiskAsync, ResolveExternalChangeAsync, FormatReport, ApplySavedState, Undo, Redo, ValidateAsync, ReloadAsync, NewItem, DuplicateItem, DeleteItem, ChangeItemId, GoToItemId, SelectRecord, NavigateToValidationIssue, SetDetailValue, NavigateToRelation, EditBits, CreateSubTableRow, SetListValues, SetListValues, SetShopkeeperQuantity, AddShopkeeperEntry, RemoveShopkeeperEntry, SetCellValue, SetItemValue, OnPresetFilterChanged, ChangeLanguageAsync, RestoreLanguageSelection, FindLanguageOption, ReloadPreservingSelectionAsync, ClearAllSections, LoadClassNamesAsync, LoadFieldSourcesAsync, InitializeChildEditorsAsync, ChildSectionMembers, IsReverseSection, IsShopkeeperRelation, OnChildEditorNavigationRequested, IsRowOfSelectedItem, RefreshChildEditors, RefreshChildSectionState, RefreshVisibleChildSections, RebuildItemList, EnsureItemFilterOptions, EnsureCustomItemFilter, ApplyItemFilter, ApplyFilterGroup, OnCustomItemFilterChanged, ResetItemFilters, ResetItemFilterState, RevealItem, ClearItemFilters, LoadItemForm, RebuildShopkeeperEntries, RebuildShopkeeperDealerOptions, RefreshShopkeeperQuantities, FindReverseMember, DisplayFieldOf, DealerNameOf, NotifyShopkeeperChanged, RebuildSections, AddFieldGroups, MatchesFieldFilter, IsGroupExpandedByDefault, ListSectionLabel, AddNestedGroup, BuildSubTableSections, BuildSubTableFields, BuildSubTableListSections, CreateRow, FindRecordByValue, InferNestedType, ParseClassBits, ResolveClassText, IsPowerOfTwo, BuildItemHeader, IsGroupVisibleForClass, IsSectionVisibleForClass, LabelFor, SectionKeyFor, AllWorkspaces, OnHistoryChanged, ApplyHistory, RefreshHistoryState, BuildLanguageOptions, BuildClassOptions, BuildGraphicTypes, SyncGraphicType, RefreshGraphicImagesAsync, LoadGraphicImageThumbnailAsync, SyncGraphicImageSelection, ClearGraphicImages, CancelGraphicImageListLoad, BeginGraphicImageListLoad, LoadImagesAsync, LoadImageAsync, LoadItemThumbnailAsync, TryGetGraphic, GetGraphicType, GetGraphicIndex, ClearImages, CancelImageLoads, BeginImageLoad, ClearAllSections
- `XmlEditorReloaded.App/ViewModels/LanguageOptionViewModel.cs` — типы: class `LanguageOptionViewModel`. Методы: LanguageOptionViewModel
- `XmlEditorReloaded.App/ViewModels/LookupColumnData.cs` — типы: class `LookupOption`; class `LookupColumnData`. Методы: LookupOption, LookupColumnData, Create, Failed, Display, GetName, GetTooltip, GetExactEntryName, TryGetOption, TryParseId
- `XmlEditorReloaded.App/ViewModels/LookupIdViewModel.cs` — типы: class `LookupIdViewModel`. Методы: LookupIdViewModel, TryAccept, Validate, SetMessages
- `XmlEditorReloaded.App/ViewModels/LookupUsageViewModel.cs` — типы: record `LookupUsageNavigationRequest`; class `LookupUsageViewModel`; class `LookupUsageItem`. Методы: LookupUsageNavigationRequest, LookupUsageViewModel, OpenSelected, LookupUsageItem
- `XmlEditorReloaded.App/ViewModels/MainViewModel.cs` — типы: class `MainViewModel`. Методы: MainViewModel, InitializeAsync, BrowseAsync, RemoveDataDirectoryAsync, CanRemoveDataDirectory, FindReplacementDataDirectory, IsUsableDataDirectory, RemovePath, ClearActiveDataSet, OpenImageViewer, OpenEnemyAmmoAsync, CompareVersions, CopyDataAsync, OpenAbout, BulkEdit, ResetLayout, ShowTranslationReport, CanBulkEdit, CanResetLayout, CanShowTranslationReport, ValidateDataset, SearchDataset, ShowDatasetWindow, OnDatasetNavigationRequested, OpenSettingsAsync, ApplySettingsAsync, SaveSelectedEditorAsync, RefreshXmlSnapshotAsync, FindXmlRecord, CloseSelectedEditorAsync, OpenTableAsync, OpenEditorByNameAsync, CloseEditorAsync, RequestCloseAsync, PersistWindowPlacementAsync, FlushLayoutsAsync, SaveAllAsync, ConfirmCloseEditorAsync, RefreshAsync, ActivateDataDirectoryAsync, ShowTablesAsync, LoadSelectedTableAsync, ResolveLanguageCode, AttachEditor, DetachEditor, OnTableNavigationRequested, OnEditorLanguageChanged, PersistLanguageAsync, CompleteOperation, RebuildDataDirectories, EnsureDataDirectoryItem, SelectDataDirectorySilently, PersistActiveDataDirectoryAsync, SamePath, CancelOperation
- `XmlEditorReloaded.App/ViewModels/MercGearKitViewModel.cs` — типы: record `MercGearEntryViewModel`; class `MercGearSlotViewModel`; record `MercGearSlotState`; record `MercGearKitState`; class `MercGearKitViewModel`; class `MercGearColumnHeaders`. Методы: MercGearEntryViewModel, MercGearSlotViewModel, Refresh, MercGearSlotState, MercGearKitState, MercGearKitViewModel, Refresh, MercGearColumnHeaders
- `XmlEditorReloaded.App/ViewModels/MercGearWorkspaceViewModel.cs` — типы: class `MercGearWorkspaceViewModel`; record `SlotDefinition`. Методы: MercGearWorkspaceViewModel, CreateXmlSnapshot, InitializeAsync, InitializeCoreAsync, SaveAsync, SaveWithRecoveryAsync, ReloadFromDiskAsync, ResolveExternalChangeAsync, ApplySavedState, Undo, Redo, ValidateAsync, ReloadAsync, SelectRecord, NavigateToValidationIssue, SetKitField, SetKitName, LoadFieldSourcesAsync, ChangeLanguageAsync, RestoreLanguageSelection, FindLanguageOption, ReloadPreservingSelectionAsync, RebuildMercList, ApplyMercFilter, LoadMercForm, BuildKit, ReadKitState, GetKitValue, RefreshKit, RefreshAllKits, ApplyHistory, OnHistoryChanged, RefreshHistoryState, BuildLanguageOptions, BuildSlotDefinitions, SlotDefinition, ResolveLabel
- `XmlEditorReloaded.App/ViewModels/NestedBlockAddViewModel.cs` — типы: class `NestedBlockAddViewModel`. Методы: NestedBlockAddViewModel
- `XmlEditorReloaded.App/ViewModels/NestedFieldViewModel.cs` — типы: class `NestedFieldViewModel`. Методы: NestedFieldViewModel, Refresh
- `XmlEditorReloaded.App/ViewModels/NestedFormSectionViewModel.cs` — типы: class `NestedFormSectionViewModel`. Методы: NestedFormSectionViewModel
- `XmlEditorReloaded.App/ViewModels/NestedListViewModel.cs` — типы: class `NestedListViewModel`; class `NestedListValueViewModel`. Методы: NestedListViewModel, Add, Remove, Apply, NestedListValueViewModel
- `XmlEditorReloaded.App/ViewModels/NestedRecordItemViewModel.cs` — типы: class `NestedRecordItemViewModel`. Методы: NestedRecordItemViewModel
- `XmlEditorReloaded.App/ViewModels/NestedTableWorkspaceViewModel.cs` — типы: class `NestedTableWorkspaceViewModel`. Методы: NestedTableWorkspaceViewModel, CreateXmlSnapshot, InitializeAsync, SaveAsync, SaveWithRecoveryAsync, ReloadAsync, ReloadFromDiskAsync, SelectRecord, NavigateToValidationIssue, Undo, Redo, ValidateAsync, Reload, AddRecord, DeleteRecord, AddAssignment, CanAddAssignment, DeleteSection, CanDeleteSection, AddBlock, CanAddRecord, CanDeleteRecord, ResolveExternalChangeAsync, ApplySavedState, ApplyHistoryResult, LoadWorkspaceAsync, BuildLanguageOptions, ResolveAssignmentSegmentIndex, RebuildBlockActions, IsRepeatedBlock, IsKeySegment, RebuildRecords, ApplyFilter, BuildDisplayName, OnHistoryChanged, RefreshHistoryState, ChangeLanguageAsync, BuildForm, AddRootSections, CreateRootField, FindRootCell, SetRootValue, AddMainSections, AddMainListSections, SetMainListValues, GetMainValue, IsMainTranslated, SetMainValue, AddNestedLeafSections, AddRepeatedBlockSections, CollectBlockFields, SetBlockField, SetBlockListValues, IsNestedCollection, AddNestedCollectionSections, CollectNestedFields, CollectNestedChildren, SetNestedListValues, GetNestedValue, SetNestedField, AfterEdit, CreateField, GroupLabel, Sanitize
- `XmlEditorReloaded.App/ViewModels/RelationColumnData.cs` — типы: class `RelationOption`; class `RelationColumnData`. Методы: RelationOption, RelationColumnData, Create, Failed, Display, GetName, GetTooltip, Filter, TryGetTargetValue
- `XmlEditorReloaded.App/ViewModels/SettingsViewModel.cs` — типы: class `SettingsDirectoryItem`; record `UiLanguageOption`; record `ThemeOption`; class `SettingsViewModel`. Методы: SettingsDirectoryItem, UiLanguageOption, ThemeOption, SettingsViewModel, AddDirectory, RemoveDirectory, SetActiveDirectory, ResetLayouts, CreateResult, SelectDirectory, RefreshActiveFlags, SamePath
- `XmlEditorReloaded.App/ViewModels/ShopkeeperDealerOptionViewModel.cs` — типы: class `ShopkeeperDealerOptionViewModel`. Методы: ShopkeeperDealerOptionViewModel
- `XmlEditorReloaded.App/ViewModels/ShopkeeperEntryViewModel.cs` — типы: class `ShopkeeperEntryViewModel`. Методы: ShopkeeperEntryViewModel, Refresh, Remove
- `XmlEditorReloaded.App/ViewModels/TableColumnViewModel.cs` — типы: class `TableColumnViewModel`. Методы: TableColumnViewModel, SetDisplayWidth, SetVisibleWithoutNotification
- `XmlEditorReloaded.App/ViewModels/TableCopyViewModel.cs` — типы: class `TableCopyViewModel`; record `TableCopyTableItem`; record `TableCopyPolicyOption`; record `TableCopyKeyModeOption`; class `TableCopyPreviewRow`. Методы: TableCopyViewModel, InitializeAsync, LoadTablesAsync, UpdateKeyModeAvailabilityAsync, CanPreview, PreviewAsync, CanApply, ApplyAsync, BrowseSource, BrowseTarget, Browse, CreateRequest, ParseRecordKeys, ApplyPreview, ClearPreview, DescribeAction, DescribeRemaps, DescribeRelated, Normalize, SamePath, TableCopyTableItem, ToString, TableCopyPolicyOption, ToString, TableCopyKeyModeOption, ToString, TableCopyPreviewRow
- `XmlEditorReloaded.App/ViewModels/TableEditorViewModel.cs` — типы: class `TableEditorViewModel`; class `RowValueComparer`. Методы: TableEditorViewModel, CreateXmlSnapshot, InitializeAsync, InitializeFromWorkspaceAsync, SaveAsync, SaveWithRecoveryAsync, ReloadFromDiskAsync, ResolveExternalChangeAsync, ApplySavedState, AddRow, AddRowWithDefaults, DuplicateRow, ChangeLookupIdAsync, ShowLookupUsagesAsync, OnLookupUsageNavigationRequested, TryGetSelectedLookupId, ConfirmLookupNotUsedAsync, ScanLookupUsageAsync, DeleteRowAsync, Undo, Redo, BulkEdit, ValidateAsync, ReloadAsync, ClearFilter, FilterByValue, ExcludeValue, ResetLayoutAsync, UpdateSelectedRows, ApplyColumnOrder, FlushLayoutAsync, CanFilterByValue, ApplyValueFilter, OnFilterChanged, LoadLayoutAsync, ApplyLayout, CaptureLayout, ScheduleLayoutSave, SaveLayoutAfterDelayAsync, SaveLayoutAsync, ShowTranslationReport, ReloadForLanguageAsync, ApplyHistory, RefreshHistoryState, ShowDefaultColumns, ShowAllColumns, HideAllColumns, SortBy, SetCellValue, SetTransientStatus, ResetStatusAfterDelayAsync, BuildLanguageOptions, FindLanguageOption, ChangeLanguageAsync, RestoreLanguageSelection, LoadFieldSourcesAsync, BuildColumns, GetFilterColumns, ApplyDefaultColumns, RefreshVisibleColumns, OnColumnVisibilityChanged, UpdateVisibleColumnCount, ApplyColumnFilter, RebuildRows, IndexOfRecord, SelectKeyFor, ApplyRowView, SetDetailValue, NavigateToRelation, NavigateToRelationTarget, GoToTarget, TryNavigateToRelation, SelectRecord, NavigateToValidationIssue, RefreshRows, RefreshFieldValues, RefreshRowFilter, EditBits, RefreshDetails, ApplyDetailFilter, IsExpandedByDefault, LoadDetailImagesAsync, LoadDetailImageAsync, ClearDetailImages, CancelDetailImageLoads, BeginImageLoad, TryGetGraphic, TryGetGraphic, LoadThumbnailAsync, IsValidValue, UpdateRecordCount, Matches, RowValueComparer, Compare, CompareSigned, CompareLookup, CompareFlags, CompareDecimals
- `XmlEditorReloaded.App/ViewModels/TableFilterViewModel.cs` — типы: class `TableFilterViewModel`. Методы: TableFilterViewModel, AddCriterion, Clear, Load, ToGroup, RemoveCriterion, OnCriterionChanged, NotifySummaryChanged
- `XmlEditorReloaded.App/ViewModels/TableListItem.cs` — типы: class `TableListItem`. Методы: TableListItem
- `XmlEditorReloaded.App/ViewModels/TableNavigationRequest.cs` — типы: record `TableNavigationRequest`. Методы: TableNavigationRequest
- `XmlEditorReloaded.App/ViewModels/TableRowViewModel.cs` — типы: class `TableRowViewModel`. Методы: TableRowViewModel, GetStoredValue, GetValues, IsTranslated, RequestThumbnail, ConfigureThumbnail, InvalidateThumbnail, LoadThumbnailAsync, ConfigureTranslation, Refresh, RecomputeTranslationState
- `XmlEditorReloaded.App/ViewModels/TableTreeItem.cs` — типы: class `TableTreeItem`. Методы: TableTreeItem, Folder, Leaf
- `XmlEditorReloaded.App/ViewModels/TableTreeViewModel.cs` — типы: class `TableTreeViewModel`. Методы: Load, ClearFilter, ExpandAll, CollapseAll, Select, FindInTree, ApplyFilter, SetVisibleRoots, FilterNodes, Matches, RestoreVisible, Sort, SetExpanded, ClearSelection, FindLeaf, CountLeaves
- `XmlEditorReloaded.App/ViewModels/TranslationReportViewModel.cs` — типы: class `TranslationReportViewModel`; record `TranslationReportGroupViewModel`; class `TranslationReportIssueViewModel`. Методы: TranslationReportViewModel, TranslationReportGroupViewModel, TranslationReportIssueViewModel
- `XmlEditorReloaded.App/ViewModels/ValidationIssueItem.cs` — типы: class `ValidationIssueItem`. Методы: ValidationIssueItem, BuildText
- `XmlEditorReloaded.App/ViewModels/XmlTextSelection.cs` — типы: record `XmlTextSelection`. Методы: XmlTextSelection
- `XmlEditorReloaded.App/Views/AboutWindow.xaml.cs` — типы: class `AboutWindow`. Методы: AboutWindow
- `XmlEditorReloaded.App/Views/BitEditorWindow.xaml.cs` — типы: class `BitEditorWindow`. Методы: BitEditorWindow, OnOk, OnCancel
- `XmlEditorReloaded.App/Views/BulkEditWindow.xaml.cs` — типы: class `BulkEditWindow`. Методы: BulkEditWindow, OnOperandFocusWithinChanged, OnOperandDropDownClosed, OnApply, OnCancel
- `XmlEditorReloaded.App/Views/CharacterCounterConverter.cs` — типы: class `CharacterCounterConverter`. Методы: Convert, ConvertBack
- `XmlEditorReloaded.App/Views/CompareWindow.xaml.cs` — типы: class `CompareWindow`. Методы: CompareWindow, OnClose
- `XmlEditorReloaded.App/Views/DatasetWindow.xaml.cs` — типы: class `DatasetWindow`. Методы: DatasetWindow, OnLoaded, OnIssuesGridDoubleClick, OnHitsGridDoubleClick, OnClose
- `XmlEditorReloaded.App/Views/EnemyAmmoWindow.xaml.cs` — типы: class `EnemyAmmoWindow`. Методы: EnemyAmmoWindow, OnClosing, OnCloseClick
- `XmlEditorReloaded.App/Views/ExternalChangeWindow.xaml.cs` — типы: class `ExternalChangeWindow`. Методы: ExternalChangeWindow, OnReload, OnOverwrite, OnCancel
- `XmlEditorReloaded.App/Views/FieldEditor.xaml.cs` — типы: class `FieldEditor`. Методы: FieldEditor, OnValueFocusWithinChanged, OnValueDropDownClosed
- `XmlEditorReloaded.App/Views/GridValueConverters.cs` — типы: class `LookupDisplayConverter`; class `LookupTooltipConverter`; class `RelationDisplayConverter`; class `RelationTooltipConverter`; class `BooleanValueConverter`. Методы: LookupDisplayConverter, Convert, ConvertBack, LookupTooltipConverter, Convert, ConvertBack, RelationDisplayConverter, Convert, ConvertBack, RelationTooltipConverter, Convert, ConvertBack, Convert, ConvertBack
- `XmlEditorReloaded.App/Views/ImageViewerWindow.xaml.cs` — типы: class `ImageViewerWindow`. Методы: ImageViewerWindow
- `XmlEditorReloaded.App/Views/InverseBooleanToVisibilityConverter.cs` — типы: class `InverseBooleanToVisibilityConverter`. Методы: Convert, ConvertBack
- `XmlEditorReloaded.App/Views/ItemCreationWindow.xaml.cs` — типы: class `ItemCreationWindow`. Методы: ItemCreationWindow, OnOk, OnCancel
- `XmlEditorReloaded.App/Views/ItemWorkspaceView.xaml.cs` — типы: class `ItemWorkspaceView`. Методы: ItemWorkspaceView, OnItemThumbnailLoaded, OnItemThumbnailDataContextChanged, OnItemListSelectionChanged, OnGraphicImageThumbnailLoaded, OnGraphicImageThumbnailDataContextChanged
- `XmlEditorReloaded.App/Views/LookupIdWindow.xaml.cs` — типы: class `LookupIdWindow`. Методы: LookupIdWindow, OnOk, OnCancel
- `XmlEditorReloaded.App/Views/LookupUsageWindow.xaml.cs` — типы: class `LookupUsageWindow`. Методы: LookupUsageWindow, OnRowDoubleClick, OnNavigationRequested, OnOpen, OnClose
- `XmlEditorReloaded.App/Views/MercGearWorkspaceView.xaml.cs` — типы: class `MercGearWorkspaceView`. Методы: MercGearWorkspaceView
- `XmlEditorReloaded.App/Views/NestedTableWorkspaceView.xaml.cs` — типы: class `NestedTableWorkspaceView`. Методы: NestedTableWorkspaceView
- `XmlEditorReloaded.App/Views/RelationPicker.xaml.cs` — типы: class `RelationPicker`. Методы: RelationPicker, OnRelationDataChanged, OnSelectedValueChanged, OnIsReadOnlyChanged, OnAutomationIdChanged, UpdateText, OnGotKeyboardFocus, OnTextChanged, OnDropDownClick, ShowOptions, OnPreviewKeyDown, OnOptionClicked, CommitTyped, Commit, OnPopupClosed, CloseDropDown
- `XmlEditorReloaded.App/Views/SettingsWindow.xaml.cs` — типы: class `SettingsWindow`. Методы: SettingsWindow, OnOk, OnCancel
- `XmlEditorReloaded.App/Views/TableCopyWindow.xaml.cs` — типы: class `TableCopyWindow`. Методы: TableCopyWindow, OnCompleted, OnClose
- `XmlEditorReloaded.App/Views/TableView.xaml.cs` — типы: class `TableView`. Методы: TableView, OnDataContextChanged, AttachToViewModel, DetachFromViewModel, OnVisibleColumnsChanged, BuildColumns, BuildThumbnailColumn, OnThumbnailCellLoaded, BuildRelationCellTemplate, BuildRelationEditingTemplate, BuildLookupCellTemplate, BuildLookupEditingTemplate, BuildTranslationCellStyle, ThemeBrush, OnFilterOperandFocusWithinChanged, OnFilterOperandDropDownClosed, OnSorting, OnSelectionChanged, OnCurrentCellChanged, OnColumnReordered, OnColumnWidthChanged, PushColumnWidths, OnRowsGridBeginningEdit, OnPreviewMouseRightButtonDown, OnRowsGridDoubleClick, OnPreviewKeyDown, Execute, FocusFilterOperand, IsTextEditing
- `XmlEditorReloaded.App/Views/TranslationReportWindow.xaml.cs` — типы: class `TranslationReportWindow`. Методы: TranslationReportWindow, OnClose
- `XmlEditorReloaded.App/Views/TranslationStateConverter.cs` — типы: class `TranslationStateConverter`. Методы: Convert, ConvertBack
- `XmlEditorReloaded.App/Views/UnsavedChangesWindow.xaml.cs` — типы: class `UnsavedChangesWindow`. Методы: UnsavedChangesWindow, OnSave, OnDiscard, OnCancel
- `XmlEditorReloaded.App/Views/ValidationIssueNavigation.cs` — типы: class `ValidationIssueNavigation`. Методы: GetNavigateOnSelection, SetNavigateOnSelection, OnNavigateOnSelectionChanged, OnSelectionChanged
- `XmlEditorReloaded.App/Views/XmlTextView.xaml.cs` — типы: class `XmlTextView`. Методы: XmlTextView, OnCopyXmlClick, OnDataContextChanged, OnLoaded, OnUnloaded, AttachViewModel, OnViewModelPropertyChanged, ApplySelection

### XmlEditorReloaded.Core (47 файлов)

- `XmlEditorReloaded.Core/Abstractions/ILookupRepository.cs` — типы: record `LookupEntry`; interface `ILookupRepository`. Методы: LookupEntry
- `XmlEditorReloaded.Core/Abstractions/ISettingsStore.cs` — типы: enum `EditorThemeMode`; record `AppSettings`; interface `ISettingsStore`. Методы: AppSettings
- `XmlEditorReloaded.Core/Abstractions/ITableCatalog.cs` — типы: interface `ITableCatalog`. Методы: —
- `XmlEditorReloaded.Core/Abstractions/ITableMetadataProvider.cs` — типы: interface `ITableMetadataProvider`. Методы: —
- `XmlEditorReloaded.Core/Abstractions/ITableRelationResolver.cs` — типы: interface `ITableRelationResolver`. Методы: —
- `XmlEditorReloaded.Core/Abstractions/ITableRepository.cs` — типы: enum `TableDocumentStatus`; record `LoadDiagnostic`; record `TableLoadResult`; interface `ITableRepository`. Методы: LoadDiagnostic, TableLoadResult
- `XmlEditorReloaded.Core/Abstractions/ITranslationCatalog.cs` — типы: interface `ITranslationCatalog`. Методы: —
- `XmlEditorReloaded.Core/Abstractions/WindowPlacement.cs` — типы: record `WindowPlacement`. Методы: WindowPlacement, FitToScreen
- `XmlEditorReloaded.Core/Api/ErrorCodes.cs` — типы: class `ErrorCodes`. Методы: —
- `XmlEditorReloaded.Core/Api/OperationResult.cs` — типы: class `OperationResult`; class `OperationResult`. Методы: OperationResult, OperationResult, Success, Failure, Failure, OperationResult, OperationResult, Success, Failure, Failure
- `XmlEditorReloaded.Core/CoreServiceCollectionExtensions.cs` — типы: class `CoreServiceCollectionExtensions`. Методы: AddXmlEditorCore
- `XmlEditorReloaded.Core/Diagnostics/FileAccessFailure.cs` — типы: class `FileAccessFailure`. Методы: IsSharingViolation, DescribeWriteFailure, IsReadOnlyFile
- `XmlEditorReloaded.Core/Diagnostics/OperationLogEntry.cs` — типы: enum `OperationLogLevel`; record `OperationLogEntry`; interface `IOperationLog`; class `OperationLog`. Методы: OperationLogEntry, Info, Warning, Error, Fatal, Write
- `XmlEditorReloaded.Core/Filtering/RowFilterCriterion.cs` — типы: class `RowFilterCriterion`. Методы: RowFilterCriterion
- `XmlEditorReloaded.Core/Filtering/RowFilterGroup.cs` — типы: class `RowFilterGroup`. Методы: RowFilterGroup
- `XmlEditorReloaded.Core/Filtering/RowFilterOperator.cs` — типы: enum `RowFilterOperator`; enum `RowFilterLogicalOperator`. Методы: —
- `XmlEditorReloaded.Core/Imaging/RgbaBuffer.cs` — типы: enum `ImageSize`; class `RgbaBuffer`. Методы: RgbaBuffer
- `XmlEditorReloaded.Core/Layout/ColumnLayout.cs` — типы: class `ColumnLayout`. Методы: ColumnLayout
- `XmlEditorReloaded.Core/Layout/ITableLayoutStore.cs` — типы: interface `ITableLayoutStore`. Методы: —
- `XmlEditorReloaded.Core/Layout/TableLayout.cs` — типы: class `TableLayout`. Методы: TableLayout
- `XmlEditorReloaded.Core/Layout/TableSortDirection.cs` — типы: enum `TableSortDirection`. Методы: —
- `XmlEditorReloaded.Core/Localization/LanguageDescriptor.cs` — типы: record `LanguageDescriptor`. Методы: LanguageDescriptor, ToOverlayFileName
- `XmlEditorReloaded.Core/Localization/OverlayDescriptor.cs` — типы: record `OverlayDescriptor`. Методы: OverlayDescriptor
- `XmlEditorReloaded.Core/Localization/TranslationReport.cs` — типы: record `TranslationReport`; record `TranslationIssue`; enum `TranslationIssueKind`. Методы: TranslationReport, TranslationIssue
- `XmlEditorReloaded.Core/Metadata/ClassValueLimit.cs` — типы: class `ClassValueLimit`. Методы: ClassValueLimit
- `XmlEditorReloaded.Core/Metadata/ClassVisibilityRule.cs` — типы: enum `ClassVisibilitySectionKind`; class `ClassVisibilityRule`. Методы: ClassVisibilityRule, IsVisible
- `XmlEditorReloaded.Core/Metadata/ColumnHint.cs` — типы: class `ColumnHint`. Методы: ColumnHint
- `XmlEditorReloaded.Core/Metadata/ColumnMetadata.cs` — типы: class `ColumnMetadata`; enum `ColumnDataType`; enum `ColumnEditorKind`; enum `ColumnFlags`. Методы: ColumnMetadata
- `XmlEditorReloaded.Core/Metadata/ColumnTextLimit.cs` — типы: class `ColumnTextLimit`. Методы: ColumnTextLimit
- `XmlEditorReloaded.Core/Metadata/ExclusiveFlagGroup.cs` — типы: class `ExclusiveFlagGroup`. Методы: ExclusiveFlagGroup
- `XmlEditorReloaded.Core/Metadata/FieldPath.cs` — типы: class `FieldPath`. Методы: FieldPath, Parse, TryParse, Equals, Equals, GetHashCode, ToString
- `XmlEditorReloaded.Core/Metadata/ListFieldDefinition.cs` — типы: class `ListFieldDefinition`; class `DictFieldDefinition`. Методы: ListFieldDefinition, DictFieldDefinition, Expand
- `XmlEditorReloaded.Core/Metadata/RecordFields.cs` — типы: class `RecordFields`. Методы: —
- `XmlEditorReloaded.Core/Metadata/RecordKeyCodec.cs` — типы: class `RecordKeyCodec`. Методы: Compose, Decompose, Display
- `XmlEditorReloaded.Core/Metadata/RecordKeySegment.cs` — типы: class `RecordKeySegment`; enum `RecordKeyKind`. Методы: RecordKeySegment
- `XmlEditorReloaded.Core/Metadata/RecordKeySpec.cs` — типы: class `RecordKeySpec`. Методы: RecordKeySpec
- `XmlEditorReloaded.Core/Metadata/RelationFilter.cs` — типы: enum `RelationFilterOperator`; class `RelationFilter`. Методы: RelationFilter, Matches, ValuesEqual, TryParseSigned, TryParseUnsigned
- `XmlEditorReloaded.Core/Metadata/RelationTarget.cs` — типы: record `RelationTargetEntry`; class `RelationTarget`. Методы: RelationTargetEntry, RelationTarget, Find, Contains, FindValueByDisplay, Search
- `XmlEditorReloaded.Core/Metadata/SymmetricKeyPair.cs` — типы: class `SymmetricKeyPair`. Методы: SymmetricKeyPair
- `XmlEditorReloaded.Core/Metadata/TableDescriptor.cs` — типы: record `TableDescriptor`. Методы: TableDescriptor
- `XmlEditorReloaded.Core/Metadata/TableMetadata.cs` — типы: class `TableMetadata`. Методы: TableMetadata, TryGetTextLimit, TryGetClassVisibilityRule, TryGetColumnHint, TryGetColumn
- `XmlEditorReloaded.Core/Metadata/TableName.cs` — типы: struct `TableName`. Методы: ToRelativePath, Parse, TryFromRelativePath, CompareTo, ToString
- `XmlEditorReloaded.Core/Metadata/TableRelation.cs` — типы: class `TableRelation`. Методы: TableRelation
- `XmlEditorReloaded.Core/Metadata/TableRelationKind.cs` — типы: enum `TableRelationKind`. Методы: —
- `XmlEditorReloaded.Core/Validation/DatasetValidationReport.cs` — типы: enum `TableValidationState`; record `TableValidationSummary`; record `DatasetValidationReport`; record `DatasetScanProgress`. Методы: TableValidationSummary, DatasetValidationReport, DatasetScanProgress
- `XmlEditorReloaded.Core/Validation/ValidationCodes.cs` — типы: class `ValidationCodes`. Методы: —
- `XmlEditorReloaded.Core/Validation/ValidationReport.cs` — типы: enum `ValidationSeverity`; record `ValidationIssue`; record `ValidationReport`. Методы: ValidationIssue, ValidationReport

### XmlEditorReloaded.Data (11 файлов)

- `XmlEditorReloaded.Data/Catalog/FileSystemTableCatalog.cs` — типы: class `FileSystemTableCatalog`. Методы: FileSystemTableCatalog, GetTablesAsync, IsOverlayFileName
- `XmlEditorReloaded.Data/DataServiceCollectionExtensions.cs` — типы: class `DataServiceCollectionExtensions`. Методы: AddXmlEditorData
- `XmlEditorReloaded.Data/Diagnostics/FileOperationLog.cs` — типы: class `FileOperationLog`. Методы: FileOperationLog, Write, RotateIfNeeded, AppendSessionHeader, AppendEntry, AppendIndented, ToMegabytes
- `XmlEditorReloaded.Data/Diagnostics/StartupDiagnostics.cs` — типы: record `StartupWriteProbe`; class `StartupDiagnostics`. Методы: StartupWriteProbe, ProbeWriteAccess, DescribeEnvironment, DescribeFile, DescribeOwner
- `XmlEditorReloaded.Data/Diagnostics/StartupTrace.cs` — типы: class `StartupTrace`. Методы: StartupTrace, Write, RecreateIfOversized
- `XmlEditorReloaded.Data/Localization/TranslationCatalog.cs` — типы: class `TranslationCatalog`. Методы: TranslationCatalog, GetOverlayDescriptorsAsync, GetTranslatableFieldsAsync, GetLanguagesForAsync
- `XmlEditorReloaded.Data/Metadata/DatabaseMigrator.cs` — типы: class `DatabaseMigrator`; record `EmbeddedMigration`. Методы: MigrateAsync, BuildConnectionString, CreateMigrationTableAsync, ReadAppliedVersionsAsync, LoadMigrations, EmbeddedMigration
- `XmlEditorReloaded.Data/Metadata/SqliteMetadataProvider.cs` — типы: class `SqliteMetadataProvider`; record `TableRow`. Методы: SqliteMetadataProvider, GetTableAsync, GetAllTablesAsync, GetLanguagesAsync, GetRelationsAsync, GetLookupValuesAsync, ReadTableAsync, ReadTableRow, ReadTableDetailsAsync, ReadColumnTextLimitsAsync, ReadExclusiveFlagGroupsAsync, ReadClassValueLimitsAsync, ReadSymmetricKeyPairAsync, ReadClassVisibilityRulesAsync, ReadRecordKeyFieldsAsync, ReadRecordKeyAsync, ReadColumnsAsync, ReadListFieldsAsync, ReadDictFieldsAsync, ReadTranslatableFieldsAsync, ReadColumnHintsAsync, TableRow
- `XmlEditorReloaded.Data/Settings/AppPaths.cs` — типы: class `AppPaths`. Методы: —
- `XmlEditorReloaded.Data/Settings/JsonLayoutStore.cs` — типы: class `JsonLayoutStore`; class `LayoutsFileDto`; class `LayoutEntryDto`; class `ColumnDto`; class `SortDto`; class `FilterDto`; class `CriterionDto`. Методы: JsonLayoutStore, LoadAsync, SaveAsync, RemoveAsync, ResetAllAsync, ReadFileAsync, CreateEmptyFile, WriteFileAsync, NormalizePath, FindEntry, FindIndex, Matches, Map, Map, UnsupportedVersionFailure, TryBackupUnreadableFile
- `XmlEditorReloaded.Data/Settings/JsonSettingsStore.cs` — типы: class `JsonSettingsStore`; class `SettingsDto`; class `WindowPlacementDto`. Методы: JsonSettingsStore, LoadAsync, LoadCoreAsync, SaveAsync, SaveCoreAsync, Map, Map, Map

### XmlEditorReloaded.Imaging (10 файлов)

- `XmlEditorReloaded.Imaging/Cache/DecodedFrameCache.cs` — типы: class `DecodedFrameCache`; record `Entry`. Методы: DecodedFrameCache, TryGet, Set, Clear, Remove, Entry
- `XmlEditorReloaded.Imaging/Catalog/ItemImageCatalog.cs` — типы: class `ItemImageCatalog`. Методы: ItemImageCatalog, GetImageAsync, GetFrameCountAsync, GetMaxIndexAsync, FormatIndex, FindFile
- `XmlEditorReloaded.Imaging/Etrle/EtrleDecoder.cs` — типы: class `EtrleDecoder`. Методы: DecodeFrame, Overrun
- `XmlEditorReloaded.Imaging/ImageTransparency.cs` — типы: class `ImageTransparency`. Методы: ApplyGreenKey
- `XmlEditorReloaded.Imaging/ImagingServiceCollectionExtensions.cs` — типы: class `ImagingServiceCollectionExtensions`. Методы: AddXmlEditorImaging
- `XmlEditorReloaded.Imaging/StiDecoder.cs` — типы: class `StiDecoder`. Методы: StiDecoder, ParseAsync, DecodeFrame, LoadFrameAsync
- `XmlEditorReloaded.Imaging/Sti/StciFlags.cs` — типы: enum `StciFlags`. Методы: —
- `XmlEditorReloaded.Imaging/Sti/StciHeader.cs` — типы: record `StciHeader`; record `SubImageDescriptor`. Методы: StciHeader, SubImageDescriptor
- `XmlEditorReloaded.Imaging/Sti/StciImage.cs` — типы: class `StciImage`. Методы: StciImage
- `XmlEditorReloaded.Imaging/Sti/StciParser.cs` — типы: class `StciParser`. Методы: ParseAsync, Parse, Truncated

### XmlEditorReloaded.Xml (50 файлов)

- `XmlEditorReloaded.Xml/Comparison/ITableComparisonService.cs` — типы: interface `ITableComparisonService`. Методы: —
- `XmlEditorReloaded.Xml/Comparison/OverlayFileComparer.cs` — типы: class `OverlayFileComparer`; record `LoadedOverlay`; record `FlatCell`. Методы: CompareAsync, AddLanguages, LoadSideAsync, ReadOverlayXmlAsync, FirstDiagnosticMessage, Compare, UnionIdentities, Flatten, LoadedOverlay, FlatCell
- `XmlEditorReloaded.Xml/Comparison/TableCells.cs` — типы: record `TableFieldValue`; record `TableRecordCells`; record `TableCells`; class `TableCellsReader`; class `ComparisonText`. Методы: TableFieldValue, TableRecordCells, TableCells, Read, Read, CollectFields, AppendElement, ReadRootFields, FirstPathPart, Truncate
- `XmlEditorReloaded.Xml/Comparison/TableComparisonModels.cs` — типы: enum `TableComparisonStatus`; enum `TableSideStatus`; enum `FieldComparisonStatus`; enum `ComparisonDifferenceKind`; enum `RecordIdentityKind`; enum `OverlayComparisonStatus`; enum `OverlayFileStatus`; enum `LookupAvailability`; record `TableComparisonSide`; record `TableSideComparison`; record `FieldComparison`; record `RecordFieldChange`; record `RecordComparison`; record `TableValueComparison`; record `OverlaySide`; record `OverlayCellChange`; record `OverlayComparison`; class `TableComparisonLimits`; record `TableComparisonEntry`; record `LookupReferenceComparison`; record `TableComparisonSummary`; record `TableComparisonReport`. Методы: TableComparisonSide, TableSideComparison, FieldComparison, RecordFieldChange, RecordComparison, TableValueComparison, OverlaySide, OverlayCellChange, OverlayComparison, TableComparisonEntry, LookupReferenceComparison, TableComparisonSummary, TableComparisonReport
- `XmlEditorReloaded.Xml/Comparison/TableComparisonReportFormatter.cs` — типы: class `TableComparisonReportFormatter`. Методы: FormatMarkdown, FormatCsv, AppendValueCsvRows, AppendOverlayCsvRows, AppendSummary, AppendMetric, AppendWarnings, AppendLookupReferences, AppendTableList, AppendTableDetails, AppendValueDetails, AppendOverlayDetails, AppendSide, FormatDiagnostic, GetEntryStatusText, FormatAvailability, FormatType, FormatPresence, FormatCount, EscapeCell, AppendCsvRow, EscapeCsv
- `XmlEditorReloaded.Xml/Comparison/TableDataComparer.cs` — типы: class `TableDataComparer`; record `SideLoad`. Методы: TableDataComparer, CompareAsync, LoadSideAsync, CompareFields, GetFieldStatus, BuildLookupReferences, GetAvailability, BuildSummary, SideLoad
- `XmlEditorReloaded.Xml/Comparison/TableSnapshotAnalyzer.cs` — типы: class `TableSnapshotAnalyzer`; class `FieldStatistics`; record `AnalyzedField`; record `AnalyzedTable`. Методы: Analyze, ResolveRecords, HasNestedKey, ObserveValue, InferDataType, AnalyzedField, AnalyzedTable
- `XmlEditorReloaded.Xml/Comparison/TableValueComparer.cs` — типы: class `TableValueComparer`. Методы: Compare, UnionIdentities, CompareFieldValues, AddChange, SequenceEqual, JoinValues
- `XmlEditorReloaded.Xml/Copy/ITableCopyService.cs` — типы: interface `ITableCopyService`. Методы: —
- `XmlEditorReloaded.Xml/Copy/TableCopyModels.cs` — типы: enum `TableCopyConflictPolicy`; enum `TableCopyKeyMode`; enum `TableCopyAction`; enum `TableCopyRemapStatus`; record `TableCopyRequest`; record `TableCopyRemap`; record `TableCopyRelatedPreview`; record `TableCopyRecordPreview`; record `TableCopyPreview`; record `TableCopyAppliedTable`; record `TableCopyResult`. Методы: TableCopyRequest, TableCopyRemap, TableCopyRelatedPreview, TableCopyRecordPreview, TableCopyPreview, TableCopyAppliedTable, TableCopyResult
- `XmlEditorReloaded.Xml/Copy/TableCopyRemapper.cs` — типы: class `TableCopyRemapper`; struct `DisplayMapKey`; class `DisplayMap`; record `DocumentTable`. Методы: TableCopyRemapper, MapRecordAsync, ResolveFieldAsync, ResolveRelationAsync, ResolveLookupAsync, GetDisplayMapAsync, LoadDisplayMapAsync, GetLookupAsync, CollectFields, PrepareRelationsAsync, LoadRelationsAsync, DisplayMap, DocumentTable
- `XmlEditorReloaded.Xml/Copy/TableCopyService.cs` — типы: class `TableCopyService`; class `CopyContext`; class `DataSetAccessor`; record `TableLoad`; class `CopyRecordPlan`; class `RelatedPlan`; class `RelatedRowPlan`; class `TableCounts`. Методы: TableCopyService, PreviewAsync, ApplyAsync, BuildPlanAsync, LoadRelatedRelationsAsync, BuildRelatedPlansAsync, CreateRelatedPlan, CreateRelatedPreview, SelectRecords, FindSourceRecord, FindComparisonMatches, GetFieldsToCopy, ReadDisplay, BuildRelatedTargetKey, AllocateKey, ComputeNextKey, ApplyRemapsToClone, ApplyFields, CloneField, SetSingleField, MarkDirty, GetCounts, AddRemapWarnings, DescribeRemap, SameDirectory, InferKeyField, InferDisplayField, ResolveComparisonField, CopyContext, GetRemapperAsync, AddWarningOnce, DataSetAccessor, LoadAsync, LoadDocumentTableAsync, LoadLookupAsync, LoadLookupCoreAsync, LoadCoreAsync, TableLoad
- `XmlEditorReloaded.Xml/Documents/DocumentChangeTracker.cs` — типы: record `DocumentChange`; record `FieldValueChanged`; record `ListFieldChanged`; record `RecordAdded`; record `RecordDuplicated`; record `RecordDeleted`; class `DocumentChangeTracker`. Методы: DocumentChange, Record, Clear
- `XmlEditorReloaded.Xml/Documents/TableDocument.cs` — типы: class `TableDocument`. Методы: TableDocument, CreateXmlSnapshot, LocateRecordSpans, BuildLineStarts, OffsetOf, AlignToOpeningTag, OpeningTagLength, ResolveRecordElements, ResolveRecordContainer, FindRecord, GetNextPrimaryKey, SynchronizeRecords, AddRecord, DuplicateRecord, DuplicateRecordPreservingKey, DeleteRecord, DeleteRecord, TryEnableEditing, InsertRecord, MarkDirty, MarkClean, RecomputeDirty, AcceptSaved
- `XmlEditorReloaded.Xml/Documents/TableRecord.cs` — типы: class `TableRecord`. Методы: TableRecord, HasField, GetValue, GetValue, GetChildFieldNames, GetChildFieldNames, ContainerChildNames, GetValues, GetRawValue, GetRawValue, GetRawValue, GetKeyComponents, GetKey, GetPositionalIndex, SetFieldValue, SetFieldValue, SetFieldValue, SetFieldValueUnchecked, SetListField, GetOccurrenceCount, GetChildFieldNames, GetRepeatedValues, SetRepeatedValues, AddElement, RemoveElement, ContainerPath, SetDictField, SetDictField, TrySetValue, TrySetValue, SetElementValue, FormatChangePath, ValidateValue, FindDirectChild, FindPath, FindPath, CreatePath
- `XmlEditorReloaded.Xml/Documents/XmlSnapshot.cs` — типы: record `XmlRecordSpan`; record `XmlSnapshot`. Методы: XmlRecordSpan, XmlSnapshot, FindRecord
- `XmlEditorReloaded.Xml/Filtering/TableRowFilterMatcher.cs` — типы: class `TableRowFilterMatcher`. Методы: Matches, MatchesCriterion, ValuesEqual, CompareNumeric, IsNumericType, TryParseNumeric
- `XmlEditorReloaded.Xml/IO/FileFormatInfo.cs` — типы: class `FileFormatInfo`. Методы: FileFormatInfo, CreateEncoding, Detect, DetectAsync
- `XmlEditorReloaded.Xml/Loading/TableFileLoader.cs` — типы: class `TableFileLoader`. Методы: LoadAsync, LoadOverlayAsync, LoadCoreAsync, CreateReaderSettings, InferRecordElement, AddLegacyRepeatedKeyDiagnostic, CreateElementDiagnostic
- `XmlEditorReloaded.Xml/Lookups/LookupIdAllocator.cs` — типы: class `LookupIdAllocator`. Методы: TryParseId, SuggestNextId, IsSingleBit
- `XmlEditorReloaded.Xml/Lookups/LookupUsageService.cs` — типы: record `LookupUsage`; record `LookupUsageReport`; class `LookupUsageService`. Методы: LookupUsage, LookupUsageReport, LookupUsageService, FindUsagesAsync, FindLookupFields, Matches
- `XmlEditorReloaded.Xml/Lookups/XmlFileLookupRepository.cs` — типы: class `XmlFileLookupRepository`. Методы: XmlFileLookupRepository, GetEntriesAsync, ReadEntriesAsync, Merge
- `XmlEditorReloaded.Xml/Overlays/OverlayDocument.cs` — типы: class `OverlayDocument`. Методы: OverlayDocument, MarkDirty, LoadAsync, CreateAsync, TryDelete, SaveAsync
- `XmlEditorReloaded.Xml/Overlays/OverlayKeyResolver.cs` — типы: record `ResolvedRecord`; class `OverlayKeyResolver`. Методы: ResolvedRecord, OverlayKeyResolver, ResolvePrimary, ResolveNested, ResolveSingleton, FindPrimary, ResolveElements, CreateCompoundResolved, CreateResolved
- `XmlEditorReloaded.Xml/Overlays/OverlayMerger.cs` — типы: record `MergedCell`; class `MergedRecord`; class `MergedTable`; class `OverlayMerger`. Методы: MergedCell, MergedRecord, MergedTable, OverlayMerger, Merge, CollectCells
- `XmlEditorReloaded.Xml/Overlays/OverlayWriter.cs` — типы: record `OverlayRecordKey`; class `OverlayWriter`. Методы: OverlayRecordKey, ForRecord, SetValue, SetValue, SetRootValue, SetLeaf, CreatePrimaryRecord, FindOrCreateNested, FindOrCreatePositional, FindOrCreateScalar, FindOrCreateSingleton, StripSegmentPrefix
- `XmlEditorReloaded.Xml/Overlays/TableEditSession.cs` — типы: class `TableEditSession`. Методы: TableEditSession, Merge, SetValue, SetValue, SetNestedValue, SetRootValue, SaveAsync, IsTranslatable
- `XmlEditorReloaded.Xml/Overlays/TranslationAnalyzer.cs` — типы: class `TranslationAnalyzer`. Методы: Analyze, AddNestedCountIssues, AddCellIssues
- `XmlEditorReloaded.Xml/Relations/TableRelationResolver.cs` — типы: class `TableRelationResolver`; struct `TargetCacheKey`; struct `TargetCacheEntry`. Методы: TableRelationResolver, ResolveAsync, Invalidate, FindDescriptorAsync, BuildTarget, GetTargetValue, BuildStamp, FileStamp
- `XmlEditorReloaded.Xml/Repository/TableRepository.cs` — типы: class `TableRepository`. Методы: TableRepository, LoadTableAsync, SaveTableAsync
- `XmlEditorReloaded.Xml/Saving/DocumentSerializer.cs` — типы: class `DocumentSerializer`. Методы: Serialize
- `XmlEditorReloaded.Xml/Saving/LosslessDocumentPatcher.cs` — типы: class `LosslessDocumentPatcher`; record `PatchOperation`; record `ScalarChange`; record `ElementDeletion`; record `ElementInsertion`; struct `Splice`; class `SpanNode`. Методы: TryPatch, TryPatchCore, CollectOperations, BuildSignatures, BuildRecordSignatureKey, Append, AttributesEqual, CommentsEqual, NonElementContentEqual, TryCreateScalarSplice, TryCreateDeletionSplice, TryCreateInsertionSplice, ResolveIndentUnit, IndentationAfterLastNewLine, GetLineIndent, AddInsertion, CanApplySplices, SerializeElement, CloneWithoutFormattingWhitespace, RemoveFormattingWhitespace, MatchesCurrentTree, TreesEquivalent, IsSignificantNode, IsXmlWhitespace, Escape, BuildSpanTree, BuildLineStarts, GetOffset, FindTagStart, FindTagEnd, FindNode, SpanNode
- `XmlEditorReloaded.Xml/Saving/TableFileSaver.cs` — типы: class `TableFileSaver`. Методы: TableFileSaver, SaveAsync, Fail, LogAndReturn, DetectExternalChange, CanReparse
- `XmlEditorReloaded.Xml/Schema/TableSchema.cs` — типы: class `TableSchema`. Методы: TableSchema, FindColumn
- `XmlEditorReloaded.Xml/Schema/TableSchemaBuilder.cs` — типы: class `TableSchemaBuilder`; class `ColumnStatistics`; record `DictMaskPattern`. Методы: HasNestedRecordKey, Build, ResolveRecordElements, DefaultEditorKind, ResolveGroupKey, ResolveGroupOrder, ObserveValue, BuildDictPatterns, FindDictMask, InferDataType, DictMaskPattern, Matches
- `XmlEditorReloaded.Xml/Schema/TableSchemaColumn.cs` — типы: class `TableSchemaColumn`. Методы: TableSchemaColumn
- `XmlEditorReloaded.Xml/Search/DatasetSearchService.cs` — типы: record `DatasetSearchHit`; record `DatasetSearchReport`; class `DatasetSearchService`. Методы: DatasetSearchHit, DatasetSearchReport, DatasetSearchService, SearchAsync, SearchDocument, SearchElement, LoadOverlayAsync, GetMetadataAsync, Truncate
- `XmlEditorReloaded.Xml/Validation/DatasetValidationReportFormatter.cs` — типы: class `DatasetValidationReportFormatter`. Методы: FormatMarkdown, FormatCsv, AppendMetric, EscapeCell, AppendCsvRow, EscapeCsv
- `XmlEditorReloaded.Xml/Validation/DatasetValidator.cs` — типы: class `DatasetValidator`; class `IssueCollector`. Методы: DatasetValidator, ValidateAsync, ValidateTableAsync, ValidateOverlayAsync, GetMetadataAsync, CountRecords, FormatDiagnostics, Add, AddRange, ToSummary
- `XmlEditorReloaded.Xml/Validation/TableValidator.cs` — типы: class `TableValidator`. Методы: ValidateAsync, ValidateAsync, ValidateSymmetricPairs, ValidateExclusiveFlagGroups, ValidateClassValueLimits, IsFlagSet, IsZeroOrEmpty, ValidateDictMasks, ValidateElementAsync, ValidateLeafAsync, GetLookupAsync, IsKnownField, IsExpandedDictField, ValidateRange, TryGetNumericValue, IsValidValue, ContainsLookupId, ValidateFlagMask, FormatBits
- `XmlEditorReloaded.Xml/Values/LexicalValue.cs` — типы: record `LexicalValue`. Методы: LexicalValue, TryGetInt32, TryGetDecimal, ToString
- `XmlEditorReloaded.Xml/Workspace/BulkEditModels.cs` — типы: enum `BulkEditOperation`; enum `BulkEditRowStatus`; record `BulkEditRequest`; record `BulkEditPreviewRow`; class `BulkEditPreview`. Методы: BulkEditRequest, BulkEditPreviewRow, BulkEditPreview
- `XmlEditorReloaded.Xml/Workspace/NestedWorkspaceRecord.cs` — типы: class `NestedWorkspaceRecord`. Методы: NestedWorkspaceRecord
- `XmlEditorReloaded.Xml/Workspace/TableEditHistory.cs` — типы: class `TableEditHistory`; class `TableEditTransaction`; class `ElementSnapshotState`. Методы: Redo, Undo, Clear, Record, Add, Undo, Redo, RefreshDocuments, ElementSnapshotState, Capture, CaptureInsertion, CompleteAfter, MarkRemoved, Undo, Redo, ElementIndex, Apply, ApplyContent, Attach
- `XmlEditorReloaded.Xml/Workspace/TableSetWorkspace.cs` — типы: record `TableSetMember`; record `TableDependentDeletion`; record `TableSetSaveFailure`; record `TableSetSaveReport`; class `TableSetWorkspace`. Методы: TableSetMember, TableDependentDeletion, TableSetSaveFailure, TableSetSaveReport, TableSetWorkspace, LoadAsync, GetWorkspace, GetRow, GetMembers, SetValue, SetValue, SetValue, DeleteRow, SetListValues, AddRecord, Undo, Redo, DescribeDependents, DuplicateRecord, DeleteRecord, RenameRecord, SwapRecords, EnsureSubTableRecord, SaveDetailedAsync, SaveAsync, ResolveLanguage, GetKeyedRelations, FindDependingRecords, FindDependingKeys, FindValue, Capture, RewriteDependent, CaptureDeletion
- `XmlEditorReloaded.Xml/Workspace/TableWorkspace.Bulk.cs` — типы: class `TableWorkspace`; struct `OperandValue`. Методы: PreviewBulk, ApplyBulk, EvaluateRow, TryValidateOperand, TryComputeArithmetic
- `XmlEditorReloaded.Xml/Workspace/TableWorkspace.Lookups.cs` — типы: class `TableWorkspace`. Методы: GetLookupIds, SuggestLookupId, AddLookupRecord, ChangeLookupId, HasLookupId
- `XmlEditorReloaded.Xml/Workspace/TableWorkspace.cs` — типы: class `TableWorkspace`; struct `SymmetricPairValues`. Методы: TableWorkspace, LoadAsync, GetRows, GetRootCells, IsTranslatable, GetRow, GetRow, SetValue, SetValue, DeleteRow, SetValue, SetValue, Undo, Redo, SetValue, SetValue, SetNestedValue, GetNestedRecords, GetNestedValue, IsNestedTranslated, AddNestedRecord, DeleteNestedRecord, AddRepeatedBlock, DeleteRepeatedBlock, GetRepeatedValues, SetRepeatedValues, GetNestedRepeatedValues, SetNestedRepeatedValues, ApplyRecordEdit, SetRootValue, AddRecord, AddRecord, SetListValues, FindRecord, DuplicateRecord, DeleteRecord, SaveAsync, ValidateAsync, AnalyzeTranslations, SetValueCore, CaptureBaseState, CaptureOverlayState, CompleteBaseState, CompleteOverlayState, FindOverlayTopLevel, CaptureOverlayRootElement, CompleteOverlayRootElement, FindOverlayRootElement, EnsureSymmetricCounterpart, CaptureSymmetricPairValues, FindSymmetricCounterpart, ReconcileSymmetricPair, DeleteSymmetricCounterpart
- `XmlEditorReloaded.Xml/Workspace/WorkspaceRow.cs` — типы: class `WorkspaceRow`. Методы: WorkspaceRow, GetValue, GetValues, IsTranslated, IsTopLevelField, FindTopLevelLeaf
- `XmlEditorReloaded.Xml/XmlServiceCollectionExtensions.cs` — типы: class `XmlServiceCollectionExtensions`. Методы: AddXmlEditorXml
