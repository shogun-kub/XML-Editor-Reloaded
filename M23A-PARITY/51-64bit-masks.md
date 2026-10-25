# M23b4 — L-DATA-012: 64-битные lookup-id и маски Flags

**Дата:** 2026-10-25
**Гэп:** `LEGACY-PARITY-REPORT.md:400`, `LEGACY-PARITY-REPORT.md:619`; план — `PLAN.md:56`
**Область:** только анализ. Код, тесты, метаданные и `References/` не изменялись; `dotnet build`/`test` не запускались. Сканирование корпуса — read-only (PowerShell, `XDocument`/regex).

Соглашение: всё со ссылками `file:line` — проверенные факты (прочитанный код и просканированный корпус); §4–§6 — рекомендации и оценки на их основе, отдельно отмечены только те, что не следуют прямо из кода. Пути ниже — относительно корня репозитория; `src/` = `xml-editor-reloaded/src`, `tests/` = `xml-editor-reloaded/tests`.

## Сводка

Дефект состоит из трёх независимых ограничений: (1) `LookupEntry.Id` — знаковый `long` (`src/XmlEditorReloaded.Core/Abstractions/ILookupRepository.cs:14`), (2) маски `Flags` валидируются как `uint` (`src/XmlEditorReloaded.Xml/Documents/TableRecord.cs:644-645`, `src/XmlEditorReloaded.Xml/Validation/TableValidator.cs:606-607`), (3) битовый редактор 32-битный (`src/XmlEditorReloaded.App/ViewModels/BitEditorViewModel.cs:11,20-32,73-85`). В корпусе v4/v5 ни одно из ограничений не проявляется: lookup-id ≤ 2^33 (`Lookup/ItemFlag.xml`), все значения полей ≤ 2415919104 (0x90000000, `NPCInventory/Merchants.xml/uiFlags`); отрицательных `<id>` и отрицательных значений Lookup/Flags-полей нет. Утверждение отчёта «значения ≤ 2^25» неверно: `uiFlags` = 2415919104 > int.MaxValue, поэтому `uint` — не «запас», а минимально необходимый тип уже сегодня.

## 1. Текущий поток типов

### 1.1. Lookup-id: разбор → хранение → сравнение

| Этап | Место | Поведение |
|---|---|---|
| Контракт | `src/XmlEditorReloaded.Core/Abstractions/ILookupRepository.cs:14` (+doc `:9-12`) | `sealed record LookupEntry(long Id, string Name)`; doc уже обещает 64 бита |
| Разбор XML-lookup | `src/XmlEditorReloaded.Xml/Lookups/XmlFileLookupRepository.cs:171-175,183,190-200` | `long.TryParse(idText, NumberStyles.Integer, InvariantCulture, …)` → `xml.invalid_lookup_entry`; `new LookupEntry(id, name)`; `GroupBy(entry => entry.Id)` при слиянии оверлея |
| Курируемые значения | `src/XmlEditorReloaded.Data/Metadata/SqliteMetadataProvider.cs:242-248,253` | `SELECT Value, DisplayName … ORDER BY Ordinal, Value`; `new LookupEntry(reader.GetInt64(0), …)` |
| Схема `LookupValues` | `src/XmlEditorReloaded.Data/Metadata/Migrations/0014_M23b3ValueSources.sql:26-32` | `Value INTEGER NOT NULL`, `PRIMARY KEY (LookupName, Value)` |
| Проверка членства | `src/XmlEditorReloaded.Xml/Validation/TableValidator.cs:434-461` (`:438,:451`) | `long.TryParse(leaf.Value, …)`, затем `entries.All(entry => entry.Id != id)` |
| UI-модель колонки | `src/XmlEditorReloaded.App/ViewModels/LookupColumnData.cs:64,76-85,138-166,256-282` | `Dictionary<long, LookupOption> _byId`, `HashSet<long> knownById`, `long.TryParse` для неизвестных значений, `entry.Id == id`, `TryParseId(string?, out long id)` |
| Копирование строк | `src/XmlEditorReloaded.Xml/Copy/TableCopyRemapper.cs:181-189,206-210,231` | `long.TryParse(value, …)`, `entry.Id == sourceId`, `Select(entry => entry.Id).Distinct()`, запись — `matches[0].ToString(InvariantCulture)` |
| Классы ITEM | `src/XmlEditorReloaded.App/ViewModels/ItemWorkspaceViewModel.cs:54-55,1569-1570,2389-2391,2392-2413,2587-2588` | `Dictionary<long, string> _classNames/_classExactNames`; `private uint ParseClassBits(string?)`; `IsPowerOfTwo(long)`; `_classExactNames.TryGetValue(bits, …)` |
| Видимость секций | `src/XmlEditorReloaded.App/ViewModels/ItemWorkspaceViewModel.cs:1678,2042-2053,2223-2229,2422-2434`; `src/XmlEditorReloaded.Core/Metadata/ClassVisibilityRule.cs:68-72,82-83` | `IsVisible(long classBits)`; `long VisibleClasses`, **отрицательное значение = всегда видимо** |
| Прочие потребители | `src/XmlEditorReloaded.App/ViewModels/EnemyAmmoViewModel.cs:326` | `entry.Id.ToString(InvariantCulture)` — правок не требует |

### 1.2. Маски Flags: валидация → правка → сортировка/фильтр/копия

| Этап | Место | Поведение |
|---|---|---|
| Тип значения | `src/XmlEditorReloaded.Core/Metadata/ColumnMetadata.cs:61-86` (`Flags` — `:78-79`) | enum `ColumnDataType`, «Integer used as a bit-flag mask» |
| Запись значения | `src/XmlEditorReloaded.Xml/Documents/TableRecord.cs:613,620,638-650,652-656` | тип: `column?.DataType ?? hint?.DataType`; `Flags` → `uint.TryParse` (`:644-645`), иначе `xml.invalid_number` |
| Валидация | `src/XmlEditorReloaded.Xml/Validation/TableValidator.cs:406,417-427,429-432,546-584,586-598,600-612` | тип: `hint?.DataType ?? column?.DataType`; `InvalidType`/`OutOfRange` — только предупреждения, значения не меняются; второй `IsValidValue` (`:606-607`) |
| Массовая правка | `src/XmlEditorReloaded.Xml/Workspace/TableWorkspace.Bulk.cs:379-391,407-422` | `Flags` → `uint.TryParse` (`:387-388`); арифметика только `Integer`/`Decimal` |
| Редактор в гриде | `src/XmlEditorReloaded.App/ViewModels/TableEditorViewModel.cs:2545-2565` | третий `IsValidValue`, `Flags` → `uint` (`:2558-2559`) |
| Сортировка и фильтры | `src/XmlEditorReloaded.App/ViewModels/TableEditorViewModel.cs:2651-2671`; `src/XmlEditorReloaded.Xml/Filtering/TableRowFilterMatcher.cs:99-122`; `src/XmlEditorReloaded.Core/Metadata/RelationFilter.cs:81-85,91-110` | `CompareFlags` → `uint` (`:2664-2665`), `CompareLookup` → `long`; фильтр строк — `decimal`; `HasFlag` уже на `ulong` |
| Membership | `src/XmlEditorReloaded.Xml/Validation/TableValidator.cs:434-436` | lookup-проверка только для `Lookup`/`Integer`; `Flags` намеренно не проверяется |
| Сравнение наборов | `src/XmlEditorReloaded.Xml/Comparison/TableSnapshotAnalyzer.cs:152-199`; `src/XmlEditorReloaded.Xml/Comparison/TableDataComparer.cs:234-265` | тип выводится только `Integer`/`Decimal`/`Boolean`/`Nested`/`String`; сравниваются тип и наличие поля, не значения |

### 1.3. Битовый редактор

| Этап | Место | Поведение |
|---|---|---|
| Модель | `src/XmlEditorReloaded.App/ViewModels/BitEditorViewModel.cs:11,20-39,65-67,73-85,96` | `List<BitItem>(32)`, ctor `uint value`, цикл `bit < 32` и `1u << bit`, `ComposeValue()` → `uint`, `BitItem` «0 to 31» |
| Контракт и реализация | `src/XmlEditorReloaded.App/Services/IDialogService.cs:74-86`; `src/XmlEditorReloaded.App/Services/WpfDialogService.cs:83-98` | «Shows a 32-bit bit-flags editor», `uint? EditBits(…, uint value, …)` → `BitEditorWindow(new BitEditorViewModel(…))` |
| Окно | `src/XmlEditorReloaded.App/Views/BitEditorWindow.xaml.cs:6-9,27-36`; `src/XmlEditorReloaded.App/Views/BitEditorWindow.xaml:8-11,25-41` | doc «32-bit», `public uint? Value`; `Width=560 Height=330 MinWidth=420 MinHeight=240`, `ScrollViewer` + `UniformGrid Columns="4"` |
| Хост и метки (таблицы) | `src/XmlEditorReloaded.App/ViewModels/TableEditorViewModel.cs:2260-2284,2286-2314` | `uint.TryParse(…) : 0u` (`:2269-2271`), запись `value.ToString` (`:2283`); `entry.Id > uint.MaxValue` (`:2296`), `(uint)entry.Id` (`:2301`), `Log2` (`:2307`), `bit < 32` (`:2308`) |
| Хост и метки (ITEM) | `src/XmlEditorReloaded.App/ViewModels/ItemWorkspaceViewModel.cs:1123-1154,2799-2827` | то же: `:1132-1134`, `:1153`, `:2809`, `:2814`, `:2820-2821` |
| Доступность | `src/XmlEditorReloaded.App/ViewModels/DetailFieldViewModel.cs:81-85,384`; `src/XmlEditorReloaded.App/ViewModels/IDetailFieldHost.cs:37` | `CanEditBits` при `DataType == Flags` или `EditorKind == FlagEditor` |

### 1.4. Метаданные

| Факт | Место |
|---|---|
| `Lookup.ItemFlag.id` объявлен `Raw` именно из-за выхода за signed 32-bit | `src/XmlEditorReloaded.Data/Metadata/Migrations/0005_VerifiedColumnMetadata.sql:505-507,530` |
| Все остальные `id` lookup-таблиц объявлены `Integer` (потеря возможна только > 2^31−1) | `…/0005_VerifiedColumnMetadata.sql:508-553` |
| `Flags` существует **только** в `ColumnHints`, ни одна строка `Columns` его не использует | `…/0004_ItemColumnHints.sql:13-18,39`; `…/0005_VerifiedColumnMetadata.sql:60,128-129,196-197,201,252,373-374,391,395,434`; `…/0008_M20AuxiliaryTables.sql:1245` |
| Мэппинг масок на lookup-файлы для меток | `src/XmlEditorReloaded.Data/Metadata/Migrations/0006_FlagLookupMappings.sql:8-46` |
| `ColumnHints.DataType` читается через `Enum.TryParse`, ошибка игнорируется | `src/XmlEditorReloaded.Data/Metadata/SqliteMetadataProvider.cs:531,644` |
| `Minimum`/`Maximum` — `INTEGER` (signed 64-bit) | `…/0015_M23b4ValidationLimits.sql:18-19` (диапазоны `:72-100` не затрагивают `Flags`) |

## 2. Инвентарь влияния смены `LookupEntry.Id` на `ulong`

### 2.1. Production call sites (ожидаемые ошибки компиляции)

| # | Файл:строка | Что меняется |
|---|---|---|
| 1 | `src/XmlEditorReloaded.Core/Abstractions/ILookupRepository.cs:14` (+doc `:9-12`) | запись `LookupEntry(ulong Id, string Name)`, текст doc |
| 2 | `src/XmlEditorReloaded.Xml/Lookups/XmlFileLookupRepository.cs:171-175,183` | `ulong.TryParse`, текст ошибки, `new LookupEntry`; `:190-200` компилируется без правок (`GroupBy`/сравнение) |
| 3 | `src/XmlEditorReloaded.Data/Metadata/SqliteMetadataProvider.cs:253` | `unchecked((ulong)reader.GetInt64(0))` (см. §6) |
| 4 | `src/XmlEditorReloaded.Xml/Validation/TableValidator.cs:438-441,451` | `ulong.TryParse` (или `long \|\| ulong`, §4.3) и сравнение `entry.Id != id` |
| 5 | `src/XmlEditorReloaded.App/ViewModels/LookupColumnData.cs:64,76-85,138-151,162-166,256-264,277-282` | `Dictionary<ulong,…>`, `HashSet<ulong>`, `TryParseId(out ulong)`; `IdText` остаётся `string` (`:20-31`), форматирование — `:148` |
| 6 | `src/XmlEditorReloaded.Xml/Copy/TableCopyRemapper.cs:181,188,206-210,231` | `ulong.TryParse`, `entry.Id == sourceId`, `ulong[] matches`, `matches[0].ToString(InvariantCulture)` |
| 7 | `src/XmlEditorReloaded.App/ViewModels/ItemWorkspaceViewModel.cs:54-55,1569-1570` | `Dictionary<ulong,string>` — ключ `entry.Id` |
| 8 | `src/XmlEditorReloaded.App/ViewModels/ItemWorkspaceViewModel.cs:2389-2390,2392-2411,2413,2400,2406,2587-2588` | `ParseClassBits` → `ulong`, `IsPowerOfTwo(ulong)`, `bits` в `_classExactNames`/`_classNames`, сборка опций |
| 9 | `src/XmlEditorReloaded.App/ViewModels/ItemWorkspaceViewModel.cs:2422-2434` | `IsGroupVisibleForClass(…, ulong)`; сопоставление с `ClassVisibilityRule.VisibleClasses` (`long`) |
| 10 | `src/XmlEditorReloaded.App/ViewModels/TableEditorViewModel.cs:2296-2311` | фильтр `entry.Id > uint.MaxValue` и каст `(uint)` уходят |
| 11 | `src/XmlEditorReloaded.App/ViewModels/ItemWorkspaceViewModel.cs:2809-2824` | то же для ITEM-панели |
| 12 | `src/XmlEditorReloaded.Core/Metadata/ClassVisibilityRule.cs:68-72,82-83` | решать: остаётся `long` (сентинел −1) или `ulong` + отдельный флаг (§4.4) |
| 13 | `src/XmlEditorReloaded.App/ViewModels/EnemyAmmoViewModel.cs:326` | без правок: `ulong.ToString(InvariantCulture)` |

`FieldSourceSet.cs:179-181` работает с `RelationData.Entries` (не `LookupEntry`) — изменений не требует.

### 2.2. Тесты (те же сигнатуры)

`tests/XmlEditorReloaded.App.Tests`: `TableEditorViewModelTests.cs:1658,1668,1697-1705`; `CompareViewModelTests.cs:174,190`; `ItemWorkspaceViewModelTests.cs:1325,1350`; `MainViewModelTests.cs:884,920-924`; `MercGearWorkspaceViewModelTests.cs:276,297`; `TableCopyViewModelTests.cs:297,311`; `SettingsViewModelTests.cs:145,163`; `TableEditorCompoundKeyTests.cs:108,120`. Прямые `new LookupEntry(...)`: `tests/XmlEditorReloaded.Xml.Tests/XmlFileLookupRepositoryTests.cs:28-29,48-49,68,141,194`; `TableValidatorTests.cs:334`; `tests/XmlEditorReloaded.Data.Tests/MetadataTests.cs:306-307`; `RepositoryIntegrationTests.cs:127`; `tests/XmlEditorReloaded.App.Tests/LookupColumnDataTests.cs:22`; `TableCopyServiceTests.cs:524-528`.

### 2.3. Где отрицательные id возможны или предполагаются сегодня

| Место | Статус |
|---|---|
| `XmlFileLookupRepository.cs:171` | `long.TryParse` принимает `-1` — файл с отрицательным `<id>` сейчас **загружается** |
| `TableRecord.cs:643` | `Lookup` принимает отрицательные значения (валидация проходит) |
| `TableValidator.cs:438,451` | отрицательное значение не найдётся в таблице → предупреждение `LookupMissing`; `InvalidType` не выдаётся |
| `LookupColumnData.cs:162-166,277-282`; `TableCopyRemapper.cs:181` | отрицательный id разбирается, но ни с чем не совпадает (копирование не переименовывает) |
| `ClassVisibilityRule.cs:68-72,82-83` | отрицательное **правило** (не данные) означает «всегда видимо» — единственное место, где минус несёт смысл |
| Корпус v4/v5 | `<id>` отрицательных **нет** (23 файла, 335 записей в каждой версии); значений Lookup/Flags-полей отрицательных **нет** (пересечение множества отрицательных значений с 15 именами Lookup-полей из `ColumnHints`, 3 полями skill-grant из `0014:63-68` и 11 Flags-именами пусто) |

## 3. Максимумы корпуса v4/v5 (read-only)

Метод: полный разбор всех 263 (v4) и 239 (v5) XML `TableData`; для lookup — 23 файла `Lookup/*.xml` каждой версии.

**Lookup-id (v4 и v5 идентичны):**

| Файл | Записей | max id | 2^31−1 | uint.MaxValue | long.MaxValue | ulong.MaxValue |
|---|---|---|---|---|---|---|
| `Lookup/ItemFlag.xml` | 36 | **8589934592** (2^33) | больше | **больше** (`2^33`, `2^32`) | меньше | меньше |
| `Lookup/ItemClass.xml` | 23 | 1073741824 (2^30) | меньше | меньше | меньше | меньше |
| `Lookup/AttachmentClass.xml` | 36 | 33554432 (2^25) | меньше | меньше | меньше | меньше |
| `Lookup/DrugType.xml` | 26 | 1081324 | меньше | меньше | меньше | меньше |
| `Lookup/NasAttachmentClass.xml` | 16 | 16384 | меньше | меньше | меньше | меньше |
| Итого 23 файла | 335 | 8589934592 | — | 2 значения больше (`2^32`, `2^33`) | 0 | 0 |

**Flags-поля (максимумы v4 и v5 совпадают; отрицательных значений нет; «значений» — v4/v5, где различается):**

| Таблица/поле | Значений | max | vs uint.MaxValue (4294967295) | vs int.MaxValue (2147483647) |
|---|---|---|---|---|
| `NPCInventory/Merchants.xml` `uiFlags` | 69 | **2415919104** (0x90000000) | меньше | **больше** |
| `Items/Items.xml` `usItemClass` | 1776 / 1775 | 1073741824 (2^30) | меньше | меньше |
| `Inventory/EnemyMiscDrops.xml` `usItemClass` | 13 | 536870912 | меньше | меньше |
| `Items/Pockets.xml` `pRestriction` | 49 | 268601344 | меньше | меньше |
| `Items/Items.xml` `AttachmentClass` | 160 | 33554432 (2^25) | меньше | меньше |
| `Items/Items.xml` `usActionItemFlag` | 26 | 32768 | меньше | меньше |
| `Items/Items.xml`, `Items/AttachmentSlots.xml` `nasAttachmentClass` | 219 / 63 | 16384 | меньше | меньше |
| `Items/Items.xml`, `Items/AttachmentSlots.xml` `nasLayoutClass` | 1557/1556, 63 | 32 | меньше | меньше |
| `Items/Items.xml` `DrugType` | 25 | 21 | меньше | меньше |
| `Items/Items.xml` `AttachmentPoint`, `AvailableAttachmentPoint` | 47 / 25 | 16 | меньше | меньше |
| `Items/AmmoTypes.xml` `ammoflag` | 34 | 16 | меньше | меньше |

Элементов `AvailableAttachmentPoint0…9` в данных нет (метаданные разворачивают маску словаря, данные её не используют). Общий максимум **любого** числового листа вне `Lookup` — 2415919104; значений > uint.MaxValue — 0, > long.MaxValue — 0 в обеих версиях.

**Проверка утверждения «≤ 2^25 кроме `Lookup.ItemFlag`»:** для `AttachmentClass` (2^25) и `nasAttachmentClass` верно, но в целом **неверно**: `uiFlags` = 2415919104 > int.MaxValue, `usItemClass` = 2^30, `pRestriction` = 268601344. Следствие: `uint` уже сегодня обязателен (знаковый 32-бит исказил бы `uiFlags` в отрицательное), а расширение до `ulong` — запас на будущее, а не исправление наблюдаемых данных.

**Аномалия корпуса:** `References/JA 2 1.13 v5/Data-1.13/TableData/Lookup/AttachmentPoint.xml` не well-formed (лишний `</AttachmentPoint>`, строка 31), парсер возвращает `xml.parse_error`; 8 `<id>` извлечены только regex. На L-DATA-012 не влияет, уже зафиксировано в `M23A-PARITY/40-storage.md:31`.

## 4. Рекомендуемый порядок и точные изменения

Порядок: (1) маски → (2) битовый редактор → (3) lookup-id → (4) UI/тесты. Шаги 1–2 не меняют публичный контракт `LookupEntry` и дают закрытие «32-битных масок» независимо от шага 3.

### 4.1. Маски: `uint` → `ulong` в четырёх точках разбора

`TableRecord.cs:644-645`, `TableValidator.cs:606-607`, `TableWorkspace.Bulk.cs:387-388`, `TableEditorViewModel.cs:2558-2559`; сортировка — `TableEditorViewModel.cs:2662-2671` (`ulong.TryParse`, `CompareTo`). Формы: `ulong.TryParse(raw, NumberStyles.Integer, InvariantCulture, out _)`. `NumberStyles.Integer` уже включает ведущий/хвостовой пробел и знак; `-1` для `ulong` не парсится — поведение для масок совпадает с текущим `uint`. Диапазоны (`TableValidator.cs:546-598`) трогать не нужно: `decimal` покрывает 2^64−1, а `Minimum/Maximum` для `Flags` в метаданных не заданы.

**`ColumnDataType`: оставить `Flags`, не добавлять вариант.** Обоснование: `Flags` уже означает «unsigned bit-flag mask» (`ColumnMetadata.cs:78-79`), хранится как текст `'Flags'` только в `ColumnHints` (§1.4) и нигде не выводится из данных (`TableSchemaBuilder.cs:341-364`, `TableSnapshotAnalyzer.cs:176-199`), поэтому: инференс схемы не меняется; фильтры (`TableRowFilterMatcher.cs:99-122`, `RelationFilter.cs:81-85`) уже 64-битные; массовая правка арифметики для `Flags` не имеет (`TableWorkspace.Bulk.cs:407-422`); сравнение наборов структурное (`TableDataComparer.cs:248-265`); копирование/round-trip не затрагиваются. Новый вариант потребовал бы правок всех `switch` по `ColumnDataType` и миграции: `SqliteMetadataProvider.cs:531,644` вызывает `Enum.TryParse` с игнорированием результата, поэтому неизвестная строка молча станет `String` (тихая деградация вместо ошибки).

### 4.2. Битовый редактор: 64 бита

`BitEditorViewModel.cs`: `List<BitItem>(64)`, ctor `ulong value`, цикл `bit < 64`, `(value & (1UL << bit)) != 0`, `ComposeValue()` → `ulong` с `value |= 1UL << bit.Number`; doc-и: `:7,17,65-67,70,96` («0 to 63»). `IDialogService.cs:74-86`, `WpfDialogService.cs:84-98`, `BitEditorWindow.xaml.cs:6-9,30` — `ulong`/`ulong?`. Хосты: `TableEditorViewModel.cs:2269-2271` и `ItemWorkspaceViewModel.cs:1132-1134` → `ulong.TryParse(…) : 0UL`; запись значения — `.ToString(InvariantCulture)` (как сейчас, `TableEditorViewModel.cs:2283`, `ItemWorkspaceViewModel.cs:1153`).

`BuildBitLabels` (`TableEditorViewModel.cs:2286-2314`, `ItemWorkspaceViewModel.cs:2799-2827`): убрать `entry.Id > uint.MaxValue` (иначе теряются метки 2^32 и 2^33 из `Lookup.ItemFlag`), `entry.Id <= 0` → `entry.Id == 0`, `(uint)entry.Id` → `(ulong)entry.Id`, `bit < 32` → `bit < 64`. `BitOperations.Log2(ulong)` возвращает `int` 0…63 — тип менять не нужно, проверка степени двойки `(mask & (mask - 1)) != 0` сохраняется как есть.

**UI.** `BitEditorWindow.xaml:9-11` (`Height=330`, `MinHeight=240`) и `:25-41` (`ScrollViewer` + `UniformGrid Columns="4"`) на 64 элемента дают 16 строк: при высоте строки ~24–26 px нужно ≈400 px, а видимая область ≈240 px — редактор будет прокручиваться (оценка по `CheckBox Margin="2,3"`, `:34`). Рекомендация: `Columns="8"` (8 строк, ~200 px, без прокрутки) либо `Height=500 MinHeight=320` при `Columns="4"`; `ScrollViewer` оставить как страховку для крупного системного шрифта.

### 4.3. `Lookup` при отрицательных значениях

`LookupEntry.Id` — `ulong`, но `ColumnDataType.Lookup` **должен продолжать принимать отрицательные значения**: `long.TryParse(text, NumberStyles.Integer, …) || ulong.TryParse(text, NumberStyles.Integer, …)` в `TableRecord.cs:642-643`, `TableValidator.cs:434-435`, `TableWorkspace.Bulk.cs:385-386`, `TableEditorViewModel.cs:2556-2557`. Причины: (а) `-1` как «нет значения» уже используется в данных (`Items/ExplosionData.xml AltExplosionSoundID`, 13 записей, но это `Integer`/relation, не `Lookup`) и может появиться в будущих Lookup-полях; (б) иначе такие значения получат новый `InvalidType` вместо нынешнего `LookupMissing` — изменение смысла предупреждения; (в) членство всё равно проверяется только по неотрицательным id, поэтому «ложного совпадения» не возникает. Корпус v4/v5 не затронут: отрицательных Lookup-значений 0.

Отдельно: `XmlFileLookupRepository.cs:171` при переходе на `ulong.TryParse` начнёт отвергать файл с отрицательным `<id>` целиком (`xml.invalid_lookup_entry`, `:173-174`). Это осознанное изменение (иначе `-1` пришлось бы хранить как 2^64−1); в корпусе таких файлов нет. Альтернатива при желании сохранить устойчивость — пропускать такую запись с диагностикой, но это требует нового канала ошибок.

### 4.4. `ClassVisibilityRule` и видимость секций

`VisibleClasses` остаётся `long` с сентинелом «<0 = всегда видимо» (`ClassVisibilityRule.cs:68-72,82-83`), а `classBits` в `ItemWorkspaceViewModel.cs:2422-2434` расширяется до `ulong`; сравнение — `VisibleClasses < 0 || ((ulong)VisibleClasses & classBits) != 0`. Маски правил берутся из метаданных и не превышают 2^30 (данные `usItemClass`), поэтому приведение безопасно; если когда-нибудь понадобится бит 63 и выше, сентинел придётся заменить отдельным флагом «always visible».

### 4.5. Лексика и round-trip

Разбор значений не участвует в сохранении: неотредактированные значения пишутся из исходного текста (`LosslessDocumentPatcher`), а `.ToString(InvariantCulture)` применяется только к явно изменённому пользователем значению (`TableEditorViewModel.cs:2283`, `ItemWorkspaceViewModel.cs:1153`, `TableWorkspace.Bulk.cs`). Требования: не нормализовать `007`, `+5`, ` 16 ` при чтении; `ulong.ToString` не даёт экспоненты, `+` или разделителей; не добавлять `NumberStyles.AllowThousands`; при ошибке разбора значение и файл не меняются (валидация только предупреждает — `TableValidator.cs:419-425,546-584`).

## 5. Тесты: что обновить и что добавить

**Обновить (ассерты `uint`/`long`):**
- `tests/XmlEditorReloaded.Xml.Tests/TableRecordTests.cs:80-102` — сейчас принимает `4294967295` и отвергает `4294967296` (`:95-96`); станет наоборот.
- `tests/XmlEditorReloaded.Xml.Tests/TableValidatorTests.cs:102-126` (маска не проверяется как lookup-id), `:191-244` (`ExclusiveFlagGroups`), `:345-361` (`LookupMissing`).
- `tests/XmlEditorReloaded.Xml.Tests/TableSchemaBuilderTests.cs:148-185` — `Flags`/`FlagEditor` из hint.
- `tests/XmlEditorReloaded.Xml.Tests/XmlFileLookupRepositoryTests.cs:62-68` (уже 2^33), `:141,194`.
- `tests/XmlEditorReloaded.App.Tests/TableEditorViewModelTests.cs:1258-1260` (`uint.MaxValue` в `ComposeValue`), `:1668,1697-1705` (fake `IDialogService`), `:910-912,997-1004,1204-1210`.
- Fake-реализации `IDialogService` (§2.2, 8 файлов) — сигнатура `ulong?`/`ulong`.
- `tests/XmlEditorReloaded.App.Tests/LookupColumnDataTests.cs:17-35` — опции неизвестных значений.
- `tests/XmlEditorReloaded.Data.Tests/MetadataTests.cs:306-307`, `RealDataValidationTests.cs:138` (8 `FlagEditor`), `RepositoryIntegrationTests.cs:127`, `CorpusCopyTests.cs`.
- `tests/XmlEditorReloaded.Xml.Tests/TableCopyServiceTests.cs`, `UndoAndBulkTests.cs`, `tests/XmlEditorReloaded.App.Tests/BulkEditViewModelTests.cs`, `tests/XmlEditorReloaded.Core.Tests/RelationFilterTests.cs:55-70` — проверить на разбор `Flags`/`Lookup`.

**Добавить:**
1. `Flags` = `18446744073709551615` принимается, `18446744073709551616` → `xml.invalid_number` (`TableRecordTests`).
2. `Flags` = `4294967296` и `8589934592` в `TableValidator` не дают `InvalidType`.
3. `BitEditorViewModel` на 64 элемента: `ComposeValue` для 2^63, 2^63+1 и `ulong.MaxValue`; метки для id `2^32`/`2^33` присутствуют (регрессия на текущий фильтр `> uint.MaxValue`).
4. `LookupColumnData`: `GetExactEntryName`/`GetTooltip` для id > 2^63−1; неизвестное значение `18446744073709551615` не путается с известным.
5. `XmlFileLookupRepository`: `<id>18446744073709551615</id>` загружается, `18446744073709551616` → `xml.invalid_lookup_entry`, `<id>-1</id>` → `xml.invalid_lookup_entry` (§4.3).
6. `TableCopyRemapper`: remap по имени, когда id = 2^33 (не представляется в `long`), и когда id > 2^63−1.
7. `TableValidator`: `Lookup`-значение `-1` → только `LookupMissing`, без `InvalidType` (§4.3).
8. Сортировка `RowValueComparer.CompareFlags` на парах 2^63−1 / 2^63 / `ulong.MaxValue`.
9. `ItemWorkspaceViewModel`: `ParseClassBits` для 2^30 и 2^40, видимость секции по `ClassVisibilityRule` с сентинелом −1.
10. Корпусный тест: все значения `Flags`-полей v4/v5 разбираются как `ulong`, `uiFlags = 2415919104` принимается без предупреждения.

## 6. Риски и открытые вопросы

1. **`LookupValues.Value` — SQLite `INTEGER` знаковый 64-битный** (`0014_M23b3ValueSources.sql:28`), значения > 2^63−1 в нём не представимы; `reader.GetInt64(0)` (`SqliteMetadataProvider.cs:253`) физически не вернёт больше. Рекомендация: **не расширять колонку** и считать `LookupValues` источником только для id ≤ 2^63−1 (сегодня там лишь `Lookup.SkillTrait` 0…23, `0014:36-60`; lookup-файлы уже дают 2^33): (а) хранить значение как есть, читать `unchecked((ulong)reader.GetInt64(0))` (для неотрицательных эквивалентно `(ulong)`), писать `unchecked((long)id)`; (б) в следующей миграции добавить `CHECK (Value >= 0)`, чтобы попытка записать id > 2^63−1 падала явно, а не превращалась в отрицательное число; (в) **не** хранить two's complement: это ломает `ORDER BY Ordinal, Value` (`SqliteMetadataProvider.cs:247`), сравнения в SQL и читаемость данных в git; (г) если курируемые id > 2^63−1 всё же понадобятся — добавить `ValueText TEXT NULL` (канонический decimal, invariant), читать его при `NOT NULL`, а `Value` оставить для малых id и сортировки. Второй канал (`ValueText`) стоит вводить только вместе с реальной потребностью, иначе появится два источника истины.
2. **`Minimum`/`Maximum INTEGER`** (`0015:18-19`) — та же граница: диапазон для масок выше 2^63−1 не выразить; сейчас диапазонов для `Flags` нет, так что риск отложенный.
3. **Отрицательные `<id>` в будущих данных.** После §4.3 файл с `-1` перестанет читаться. Нужно решение parity-владельца: строгий отказ (рекомендуется, файл не перезаписывается) или мягкий пропуск записи с диагностикой.
4. **Порядок разрешения типа различается:** `TableRecord.cs:620` берёт `column` раньше `hint`, `TableValidator.cs:406` — наоборот. Сегодня `Flags` встречается только в hints (§1.4), поэтому расхождения нет; при добавлении `Flags` в `Columns` поведение записи и валидации разойдётся — это надо зафиксировать тестом или унифицировать.
5. **Бит 31 уже занят в данных:** `uiFlags = 2415919104` (0x90000000) при любом переходе на знаковый тип станет отрицательным; «нормализация» и `int`-касты недопустимы (AGENTS: не нормализовать, только предупреждать).
6. **`ClassVisibilityRule`:** сентинел −1 несовместим с данными, у которых установлен бит 63; при 64-битных масках секций потребуется явный флаг «always visible».
7. **Открытый вопрос (из `LEGACY-PARITY-REPORT.md:669`, `M23A-PARITY/30-merc-aux.md:199`):** нужны ли значения выше 32 бит для `AttachmentClass`/`nasAttachmentClass` в будущих версиях? Корпус отвечает «нет» (2^25 / 2^14), поэтому `ulong` — страховка совместимости с legacy `ULong`, а не требование данных.
8. **Объём:** смена `LookupEntry.Id` затрагивает 8 production-файлов (строки 1–11 §2.1; три строки — один `ItemWorkspaceViewModel`) и 13 тестовых (§2.2); полный набор с масками и битовым редактором — 15 production-файлов (§4.1–4.2). Шаги §4.1–4.2 закрывают наблюдаемую часть L-DATA-012 без смены `LookupEntry`, поэтому их стоит выпускать отдельным коммитом.

## 7. Реализация (M23b4)

Реализовано одним коммитом вместе с §4.1–4.2 и сменой `LookupEntry.Id`; решения и проверки — в
`52-m23b4-implementation-log.md`.

- §4.1: `Flags` разбираются как `ulong` в четырёх точках (запись, валидация, bulk, грид), сортировка
  сравнивает значения как `decimal`. Новый вариант `ColumnDataType` не вводился.
- §4.2: битовый редактор 64-битный, метки битов больше не фильтруются по `uint.MaxValue`; окно
  увеличено до `Height=500`/`MinHeight=320` при `Columns="4"` (вариант `Columns="8"` отвергнут —
  подписи не помещаются).
- §4.3: отрицательные значения `Lookup` остаются валидными по типу и дают только `lookup_missing`;
  lookup-файл с отрицательным `<id>` отвергается.
- §4.4: `classBits` — `ulong`, сентинел `-1` сохранён; бит 63 в правилах видимости остаётся
  ограничением (§6.6).
- §6.1: `LookupValues.Value` не расширялась; id > 2^63−1 в ней не представимы, второй канал не
  вводится без реальной потребности.
- §5: добавлены тесты на `ulong.MaxValue`, id 2^32/2^33, remap по имени, отрицательные lookups и
  отсутствие `InvalidType` для значений вида `uiFlags`.
