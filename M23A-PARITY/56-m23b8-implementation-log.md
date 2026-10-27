# M23b8 — хранение, копирование и журналирование: журнал реализации

**Дата:** 2026-10-27. **Статус:** выполнен. **Закрытые пробелы:** `L-STO-034`,
`L-OPS-011`, `L-OPS-025` — 3 записи реестра (3 ID). Матрица и реестр
`LEGACY-PARITY-REPORT.md` остаются снимком M23a; факт закрытия фиксируют секция M23b8
отчёта и этот журнал.

## 1. Исходное состояние

M23b7 закрыт (`b20f8bc`), 532 теста, HEAD `73a19ac`. Следующий подшаг по `PLAN.md` —
M23b8. Пробелы затрагивают загрузку/сохранение XML, копирование строк между наборами
и сквозное журналирование; формат игровых файлов не меняется.

## 2. Сверка с legacy

### 2.1 Пустая таблица (`L-STO-034`)

- `References/xml-editor/BackEnd/Data Access/Tables/DefaultTable.vb:302-307`:
  `NewRow` создаёт строку на таблице без записей, поэтому legacy разрешал добавить
  первую запись в только что опустевший файл.
- `TableFileLoader` помечал файл с корнем и без элементов записей как
  `LoadedReadOnly`, а `TableDocument.AddRecord` требует `Loaded`. `TryEnableEditing`
  снимал только диагностику и был доступен лишь сервису копирования. В v4/v5 пустых
  корней нет (263/239 файлов), поэтому дефект проявлялся только после удаления
  последней строки пользователем.

### 2.2 Сопоставление по display-полю (`L-OPS-011`)

- `DefaultTable.vb:394-421`: при отсутствии целевой строки с ключом источника
  вызывается `AlternateRowSearch`; `:463-474` ищет первую строку с тем же
  `TableProperty.ComparisonField` и при `allowOverwrite` возвращает `Overwrite`,
  иначе `Skip`. Только после `Add` выделяется новый ключ (`:407-421`).
- `DatabaseCode.vb:840, 857, 872, 886`: все lookup-фабрики задают
  `ComparisonField = "name"`. В rewrite флаг `ColumnFlags.ComparisonField` был
  объявлен, но не засеян и не читался копированием; поиск по display-тексту
  существовал только для remap внешних ключей.

### 2.3 Журнал (`L-OPS-025`)

- `References/xml-editor/BackEnd/ErrorHandler.vb:202-229`: `XmlEditorLog.txt` в
  каталоге приложения, заголовок с версией, пользователем, компьютером, сетью и ОС;
  `LogString` (`:181-200`) пишет время, память, сообщение и, для ошибок, полный
  стек; опционально Windows Event Log и консоль.
- В rewrite не было ни `ILogger`, ни `Trace`, ни файла: ошибки оставались в
  `StatusText` вкладки, стартовом `MessageBox`, панели диагностики и списке
  валидации и исчезали после закрытия вкладки или приложения.

## 3. Реализация

### 3.1 Пустая плоская таблица

- `Xml/Loading/TableFileLoader.cs`: объявленный корень без дочерних элементов
  (`<ITEMLIST />`, только комментарии/PI) грузится как `Loaded` без диагностики,
  поэтому первая запись снова добавляется. Корень с посторонними дочерними
  элементами, чужой корень и файл без метаданных остаются `LoadedReadOnly`;
  таблицы с вложенным ключом (`RecordKey.Primary.Path`) проверку не проходят.
- `Xml/Schema/TableSchemaBuilder.cs`: у mapped-таблицы без единой записи схема
  строится из `ColumnHints` (сначала ключевые поля, затем по имени поля). Иначе у
  пустой не-lookup таблицы не было ни одной колонки и добавленная строка была бы
  невидима до переоткрытия. При наличии хотя бы одной записи hints по-прежнему
  ничего не объявляют.
- `Core/Metadata/TableMetadata.cs`: уточнена XML-документация `ColumnHints`.
- `App/ViewModels/TableEditorViewModel.cs`: `IsEditable = Workspace.CanEdit`, кроме
  таблиц с вложенными записями (их плоский грид не редактирует); статус
  `EditorReadOnlyNested` показывается только для них, `EditorReadOnly` — для
  остальных read-only таблиц. Кнопка «Добавить запись» и грид снова доступны для
  опустевшей плоской таблицы.

### 3.2 Сопоставление по comparison-полю

- `Data/Metadata/Migrations/0017_CopyComparisonFields.sql`: для всех 23 таблиц
  `IsLookup = 1` взводит у поля `name` флаг `ColumnFlags.ComparisonField`
  (`1 << 2 = 4`). `INSERT ... ON CONFLICT ... DO UPDATE SET Flags = Flags | 4`
  идемпотентен и сохраняет уже заданные тип, редактор, группу, подсказки и границы.
- `Xml/Copy/TableCopyService.cs`: `ResolveComparisonField` читает флаг из hints,
  затем из деклараций и игнорирует поле, совпадающее с ключом. При промахе по ключу
  `FindComparisonMatches` ищет целевые строки с тем же сырым значением
  (ordinal, без trim и смены регистра). Единственное совпадение становится целевой
  строкой: `Skip` пропускает, `Overwrite` перезаписывает, `Merge` дополняет
  отсутствующие поля, ключ остаётся целевым (`TargetKey` берётся у найденной
  строки), связанные записи адресуются её ключом. Поиск выполняется и в режиме
  `Allocate`: legacy не создавал дубликат имени даже при выделении нового ключа.
- Неоднозначность (несколько строк с тем же именем) не выбирает первую молча:
  строка добавляется, план получает предупреждение, и оно попадает в
  `TableCopyResult.Warnings` через дедуплицирующий `AddWarningOnce`.

### 3.3 Журналирование

- `Core/Diagnostics/OperationLogEntry.cs`: запись `TimestampUtc`/`Level`/`Source`/
  `Message`/`ErrorCode`/`Detail`, интерфейс `IOperationLog` и статический фасад
  `OperationLog` с методами `Info/Warning/Error/Fatal`. Без установленного sink
  вызовы — no-op (нужно unit-тестам); падение sink не превращается в новое
  исключение; некорректные аргументы по-прежнему проверяются.
- `Data/Diagnostics/FileOperationLog.cs`: UTF-8 без BOM,
  `%AppData%\XmlEditorReloaded\XmlEditorReloaded.log`, один заголовок сессии
  (версия, пользователь, ОС, runtime, процесс, working set, managed memory), затем
  записи с временем UTC, уровнем, источником, кодом, сообщением и стеком.
  Ротация при 4 MiB в `*.1`, запись под instance-lock; ошибки ввода-вывода
  проглатываются, чтобы журнал не ломал операцию.
- `App/App.xaml.cs`: установка sink до миграции БД; журнал старта/выхода; фатальные
  ошибки миграции и старта пишутся с исключением и показывают путь к файлу
  (`LogFileHintFormat`); перехват `DispatcherUnhandledException`,
  `AppDomain.UnhandledException` и `TaskScheduler.UnobservedTaskException`; sink
  освобождается в `OnExit`. `MainWindow` логирует ошибку закрытия окна.
- Остальные границы: `TableFileSaver` (несохранение, внешнее изменение,
  сериализация, I/O), `TableFileLoader` (I/O и отсутствие файла),
  `XmlFileLookupRepository` (нет файла, разбор, доступ, I/O), `TableRepository`
  (ошибка метаданных), `TableEditorViewModel` (открытие/инициализация),
  `MainViewModel` (refresh, смена каталога, каталог таблиц), `TableCopyService`
  (сбой откатов). В `Strings.resx`/`Strings.ru.resx` добавлена одна строка
  `LogFileHintFormat`; сам файл журнала — английский технический текст.

## 4. Осознанные отличия

1. **Журнал лежит в `%AppData%`, а не рядом с исполняемым файлом.** Legacy писал в
   текущий каталог, который может быть только для чтения (Program Files); имя
   файла — `XmlEditorReloaded.log` вместо `XmlEditorLog.txt`. Заголовок содержит
   версия/пользователь/ОС/runtime/процесс/память вместо компьютера и сети.
2. **Ротация 4 MiB с одним `*.1`** — в legacy файл рос без ограничения. Event Log и
   консоль не ведутся: WPF-приложение не имеет консоли, а Event Log требует прав.
3. **Необработанное UI-исключение логируется, показывается и помечается
   `Handled = true`**, чтобы пользователь мог сохранить открытые таблицы; legacy
   показывал фатальную ошибку и завершал процесс. Ошибки `AppDomain` и
   необслуженные задачи только журналируются.
4. **Неоднозначное comparison-совпадение** даёт добавление строки с предупреждением,
   а не выбор первой строки, как `FirstOrDefault` в legacy: данные не теряются,
   решение видно пользователю.
5. **Совпавший ключ остаётся главным.** Если `id` найден, но `name` отличается,
   rewrite применяет выбранную политику к строке с этим `id`; legacy в такой
   ситуации добавлял вторую строку с новым ключом и тем же именем.
6. **Пустая mapped-таблица получает стартовую схему из hints** (legacy имел
   фиксированную схему). Поля, которых нет ни в hints, ни в существующих записях,
   в гриде не появляются; после сохранения и переоткрытия набор колонок строится по
   фактическим записям.
7. **Пустой корень с посторонними дочерними элементами и пустой файл без
   метаданных** остаются read-only: отличить «удалены все записи» от «другая
   структура файла» без метаданных нельзя.
8. **Comparison-флаг засеян для всех lookup-таблиц**, включая те, где поля `name`
   нет; такой hint не создаёт колонку и ни на что не влияет.

## 5. Проверка M23b8

**Коммит:** `3503485 M23b8: empty tables, lookup comparison and operation log`.

| Проверка | Результат |
|---|---|
| Сборка `-warnaserror` | 0 ошибок / 0 предупреждений |
| Базовые тесты | 564/564: Core 57, Data 82, Imaging 24, Xml 214, App 187 |
| Corpus v4 | 564/564 |
| Corpus v5 | 564/564 |
| Compare / copy / perf | 1/1, 1/1, 9/9 (v5) |
| BOM / `References/` | BOM нет; `References/` не изменён |

Ключевые тесты:

- Хранение: `TableFileLoaderTests.Load_EmptyDeclaredRoot_LoadsEditableAndAcceptsFirstRecord`,
  `Load_RootWithoutChildElements_LoadsEditable` (3 варианта),
  `Load_EmptyLookupRoot_LoadsEditableAndAddsIdAndName`,
  `Load_EmptyRootWithoutMetadata_StaysReadOnly`,
  `Load_EmptyRootWithWrongMetadata_StaysReadOnly`,
  `Load_RootWithForeignChild_MatchingMetadata_StaysReadOnly`;
  `TableSchemaBuilderTests.Build_EmptyMappedTable_UsesHintsAsStarterSchema`,
  `Build_EmptyNestedTable_DoesNotDeclareHintColumns`;
  `TableEditorViewModelTests.EmptyTable_ReopensEditableAndAddsFirstRow`;
  `ShopkeeperTab_EmptiedDealerFile_KeepsDealerAfterReload`.
- Копирование: `LookupCopy_SkipPolicy_SkipsExistingNameRow`,
  `LookupCopy_OverwritePolicy_UpdatesExistingNameRow`,
  `LookupCopy_AbsentDisplayName_AddsRowWithSourceKey`,
  `LookupCopy_AmbiguousDisplayName_AddsRowAndWarns`,
  `LookupCopy_AllocateMode_SkipsExistingNameRow`,
  `MetadataTests.ComparisonField_MarksLookupNamesOnly`,
  `MetadataTests.Migrate_OnExistingDatabase_AppliesNothingAndKeepsComparisonHints`,
  `RepositoryIntegrationTests.Copy_LookupRowWithAbsentId_MatchesSeededComparisonField`.
- Журнал: `OperationLogTests.Write_WithoutSink_IsNoOp`,
  `Write_WithSink_CapturesEntry`, `Helpers_MapLevels`,
  `Write_ThrowingSink_DoesNotThrow`, `Write_EmptySource_Throws`,
  `Write_NullMessage_Throws`; `FileOperationLogTests.Write_FirstEntry_WritesSessionHeaderOnce`,
  `Write_WithException_IncludesStackAndErrorCode`, `Write_AtSizeLimit_RotatesToBackup`,
  `Write_UnwritableDirectory_DoesNotThrow`,
  `Write_FromManyThreads_WritesEveryEntryOnce`,
  `DefaultFilePath_IsUnderApplicationData`.

## 6. Остаточные риски и задел

- **Стартовая схема пустой таблицы неполна:** в грид попадают только hinted-поля,
  остальные доступны лишь после того, как появятся в записи. Для не-lookup таблиц
  без hints файл после опустошения остаётся без колонок, хотя `AddRecord` уже
  работает.
- **Порядок колонок пустой таблицы** — ключ, затем алфавит; групповая раскладка
  hints на пустую таблицу не применяется.
- **Журнал содержит имя пользователя и пути**, как и legacy; отдельного
  редактирования/удаления из UI нет, файл доступен вручную.
- **Comparison-поиск — ordinal без trim/регистра:** имена, отличающиеся пробелом или
  регистром, считаются разными; в v4/v5 дубликатов имён в 23 lookup-файлах нет.
- **Статический `OperationLog.Sink`** устанавливается приложением один раз и
  очищается в `OnExit`; тесты обязаны восстанавливать предыдущий sink (все новые
  тесты делают это в `finally`).

**Следующий подшаг:** gate M23b — повторный полный прогон, обновление матрицы и
маркер full functional parity, коммит `M23: legacy functional parity gate`.
