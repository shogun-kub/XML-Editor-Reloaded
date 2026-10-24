# XML Editor Reloaded — план реализации

## Объём, стек и состояние

**Назначение:** десктопный редактор XML-таблиц (`TableData`) мода JA2 1.13 с сохранением структуры файлов, локализационными оверлеями, изображениями STI и сравнением версий. **Стек:** .NET 10, WPF, SQLite, xUnit, CommunityToolkit.Mvvm + Microsoft.Extensions.DependencyInjection.

**Состояние:** M0–M22c завершены. Работают: выбор датасета и список таблиц, lossless round-trip и атомарное сохранение, schema-driven grid с фильтрами/выделением/bulk/layout, панель деталей, STI-превью и браузер, lookup/Boolean/flag-редакторы, undo/redo, языковые оверлеи, read-only сравнение v4↔v5 с экспортом, relation-модель с pickers и навигацией, item workspace со всеми 1:1 и 1:N таблицами, merc gear workspace, shopkeeper inventory, 104 вспомогательные таблицы, multi-dataset, cross-dataset copy с preview и remap, settings dialog, безопасное закрытие и external-change recovery. Метаданные: 173 таблицы, 168 key segments, 28 полей восьми составных ключей, 1923 `ColumnHints`, 46 declaring-колонок (23 lookup-таблицы), 1020 `TableRelations`, 7 языков, 64-битные lookup ID. M22c заменил ошибочные однополевые ключи восьми таблиц (`Attachments`, `Launchables`, `IncompatibleAttachments`, `CompatibleFaceItems`, `Merges`, `Item_Transformations`, `StructureConstruct`, `StructureDeconstruct`) составными: item 12 показывает реальные вложения, ложные `validation.duplicate_key` устранены, остались только два известных дефекта данных v5 и один дубликат дилера.

**Подшаги M22:** M22a — фильтры, multi-select/bulk, per-table layout; M22b — settings, close safety, external-change recovery; M22c — compound record keys. Все три выполнены. **M23:** M23a — аудит паритета (после M22c), M23b — исправления и gate. M23a0 (инвентаризация), M23a1 (shell/grid), M23a2 (item workspace UI), M23a3 (item-таблицы и изображения), M23a4 (merc gear и вспомогательные таблицы) и M23a5 (backend-хранение и операции) выполнены; дальше — M23a6–M23a7. **Паритет:** item-workflow parity — M19; auxiliary parity — M20; full legacy parity — M23b.

**Проверка текущего состояния:** build `-warnaserror` (0/0), 415 тестов (Core 37, Imaging 19, Xml 167, Data 64, App 128), corpus v4/v5, сравнение v4↔v5, copy v4→v5 и perf-гейты зелёные.

Детали завершённых этапов здесь намеренно не дублируются: источник истины — код, тесты и git-история.

**Правила (из AGENTS.md):**
- XML-целостность: не терять поля (включая неизвестные), не нормализовать sparse-записи, сохранять лексическую форму (`1.0`), порядок, регистр, декларацию. Неизменённый файл не перезаписывается; повреждённый — только диагностика.
- Метаданные и документ — единственные источники; никакого хардкода полей v4/v5.
- `References/` только чтение; тесты работают на копиях; проекты из `References/` не собираются.
- UTF-8 без BOM, английская XML-документация публичных API, async + `CancellationToken`.
- Локальный коммит после milestone; push только по просьбе; `PROJECT-STRUCTURE.md` обновляется при изменении структуры решения.

---

## Ключевые design-решения

1. **Round-trip:** `XDocument` + `PreserveWhitespace` + исходный `FileFormatInfo` + `IsDirty`-гейт; сохранение — байтовый `LosslessDocumentPatcher`, неоднозначная структурная замена уходит в serializer.
2. **Sparse-запись = реальный `XElement`;** `TableRecord` — фасад, `LexicalValue` хранит raw-текст; числа только invariant culture.
3. **Save-gate:** мутации только через workspace; чистый или побайтно идентичный документ не пишется; запись атомарная; внешние изменения проверяются по `(length, mtime)`, force-overwrite только после явного выбора пользователя.
4. **Metadata-driven + document-driven:** SQLite (таблицы, ключи, колонки, lookup, языки, relations) плюс вывод схемы из документа. `Columns` — declaring, `ColumnHints` — non-declaring.
5. **Оверлеи:** merge по ключу, правка перевода пишется только в оверлей, базовая таблица не меняется; новый оверлей создаётся лениво.
6. **Повреждённые файлы** показываются диагностикой, не сохраняются и не перезаписываются.
7. **Разделение проектов:** контракты — `Core`, XML/парсинг/оверлеи — `Xml`, SQLite/каталог/настройки/layout — `Data`, STI — `Imaging`, WPF — `App`; backend не ссылается на WPF.
8. **Метабаза:** линейные embedded SQL-миграции, seed из проверенных метаданных, 64-битные lookup ID.
9. **Фикстуры:** небольшие реальные сэмплы в тестах; полный корпус — env-gated `XER_CORPUS_DIR` на временной копии.
10. **STI:** заголовок, палитра, ETRLE и каталоги big/medium/small; на выходе RGBA без `System.Drawing`.

---

## Milestones

### M0–M10 — фундамент — выполнены
Скаффолдинг решения и тестов; контракты `Core`; SQLite-метабаза, seed, каталог, lookup/translation-каталог, настройки; загрузка/диагностика XML, save-gate, lossless-патчер, редактирование полей/списков/словарей, add/duplicate/delete, оверлеи и валидация; STI-парсер, ETRLE и кэш; DI-wiring и document-driven fallback; WPF shell, RESX en/ru; cancellation, invariant-числа, perf-гейты и corpus harness. После M10 добавлен STI-браузер.

### M11–M17 — редактор, локализация, метаданные, bulk, сравнение — выполнены
- **M11:** discovery-схема, `TableWorkspace`, runtime `IDataSetSession`, lossless структурный патчер.
- **M12:** вкладки таблиц, виртуализированная schema-driven сетка, sparse-правка, row-операции, поиск/сортировка, chooser колонок, валидация, Save All.
- **M13:** панель деталей с группами/фильтром, непродекларирующие `ColumnHints`, STI-превью, 32-битный битовый редактор.
- **M14:** языковой селектор, merged-значения и маркеры «не переведено», отчёт о переводах, создание оверлея.
- **M14.5:** сверка metadata с v4/v5; hints как основной слой, `Columns` только для lookup-таблиц, 64-битные lookup ID; реальные данные без ложных предупреждений. Отчёт — `M14.5-METADATA-REPORT.md`.
- **M15:** lookup-комбо, Boolean-чекбоксы с sparse-семантикой, имена битов, fallback при повреждённом lookup.
- **M16:** bulk set/add/multiply с preview, undo/redo для ячеек, строк, bulk и переводов.
- **M17:** read-only сравнение двух `TableData` с Markdown/CSV-экспортом.

### M18 — Relation model и main-table pickers — выполнен
Метаданные `TableRelations` (`Reference`/`SubTable`/`ChildRows`/`Reverse`, фильтры, remap); `ITEMTOEXPLOSIVE` выражен через `HasFlag`. Searchable pickers в сетке и деталях, поиск по имени/ID, ручной unknown ID, кросс-табличная навигация и `validation.lookup_missing`.

### M19 — Item master-detail workspace — выполнен (item-workflow parity)
`IEditorTab` унифицировал grid и workspace-вкладки. `TableSetWorkspace` загружает Items и все 1:1/1:N-таблицы с общим undo/save; duplicate/delete/rename/swap и связанные строки — многодокументные транзакции. Item workspace: список с миниатюрами, field groups, class-aware секции, встроенные child-гриды, nested/list-поля, изображения и переводы, создание предмета.

### M20 — Merc gear, вспомогательные таблицы, language catalog — выполнен (auxiliary parity)
Merc gear workspace: пять `GEARKIT`, item pickers, status/quantity/drop, цены, overlay-имена. Миграция 0008 добавила вспомогательные таблицы (choice/drop, dealer inventory, merchants, sounds), relations и языковые коды. Item workspace получил вкладку «Торговцы»; shopkeeper `CONTROL` сохраняется без отдельного UI.

### M21 — Multi-dataset и cross-dataset copy/merge — выполнен
Вкладки привязаны к `(TableDataDirectory, TableName)` и переключаются без закрытия; сессии кэшируются, настройки хранят активный и recent-каталоги. Copy-диалог: preview, `Skip`/`Overwrite`/`Merge`, `Preserve`/`Allocate`, remap по display-тексту, опциональные related rows, атомарная многофайловая запись и восстановление при ошибке. Source только читается.

### M22a — Grid: фильтры, multi-select/bulk, layout — выполнен
Структурированные фильтры с AND/OR и фильтром по ячейке, extended selection и bulk по выделению или текущему виду, пустой `Set`. Per-table layout (колонки, ширина, порядок, сортировка, фильтр) в versioned `%AppData%/XmlEditorReloaded/layouts.json` с reset и `.bad` backup.

### M22b — Shell: настройки и безопасность — выполнен
Settings dialog управляет каталогами, recent, UI-языком (en/ru, после перезапуска) и сбросом layout; активный датасет применяется без закрытия вкладок. Закрытие вкладки и приложения — Save/Discard/Cancel с flush layout; ошибка сохранения оставляет вкладку открытой. При `xml.file_changed_externally` — Reload/Overwrite/Cancel; force-overwrite пересчитывает baseline, multi-file ошибки агрегируются.

### M22c — Compound record keys — выполнен
Восемь таблиц получают составной естественный ключ вместо ошибочного одиночного: `Items.Attachments` (`attachmentIndex,itemIndex`), `Launchables` (`launchableIndex,itemIndex`), `IncompatibleAttachments` (`itemIndex,incompatibleattachmentIndex`), `CompatibleFaceItems` (`compatiblefaceitemIndex,itemIndex`), `Merges` (4 поля), `Item_Transformations` (`usItem,usResult1..10`), `StructureConstruct` (`usCreationItem,usItemStatusLoss`), `StructureDeconstruct` (`usDeconstructItem,usItemToCreate,usCreatedItemStatus`). Миграция 0009 добавляет `RecordKeyFields` и убирает старые `TableKeys`; `RecordKeyCodec` кодирует compound-ключ, `TableDocument`/workspace работают по ссылке на запись, key-колонки compound-таблиц редактируемы, `Duplicate` отключён. Реальные дубликаты (`Attachments` v5 — 2 группы, `AdditionalDealer_43` — 1) остаются предупреждениями. Таблицы без оверлея читают строки напрямую (без overlay-merge), правки compound-key идут по ссылке на запись, а undo восстанавливает позицию элемента вместе с форматирующими пробелами. Подробный план — `M22C-COMPOSITE-KEYS-PLAN.md`. **Коммит:** `M22c: compound record keys`.

### M23a — Аудит паритета
- Выполняется после M22c; отчёт создаётся заново с учётом исправленных compound-ключей.
- `LEGACY-PARITY-REPORT.md`: построчная матрица «функция legacy → реализация / superseded / исключено» с файлами-доказательствами.
- Ручная проверка на v4 и v5: item workspace и sub-tables, merc gear, shopkeeper, pickers, copy, фильтры, изображения, локализация, настройки.
- Только документ и список пробелов, без изменения поведения. **Коммит:** `M23a: legacy parity audit`.
- **Подшаги и протокол субагентов — `M23A-PARITY-AUDIT-PLAN.md`.** M23a0 — инвентаризация UI и таблиц; M23a1 — shell/grid; M23a2 — item UI; M23a3 — item-таблицы и изображения; M23a4 — merc gear и вспомогательные таблицы; M23a5 — backend-хранение и операции; M23a6 — консолидация; M23a7 — проверка v4/v5. Секции — в `M23A-PARITY/`; каждый подшаг коммитится отдельно.

### M23b — Исправления и gate паритета
- Исправляются только найденные пробелы, каждый с тестом; крупный пробел фиксируется в отчёте и выносится в отдельный подшаг после согласования.
- Повторный полный прогон и обновление матрицы. **Маркер: full functional parity**, коммит `M23: legacy functional parity gate`.

### После паритета — M24–M27
Эти функции не нужны для паритета (в legacy их нет), они закрывают собственные пробелы rewrite.

- **M24a — Nested city/facility editors:** `Map.Cities` (`COUNTRY`/`CITYLIST/CITY`) и `FacilityTypes/ASSIGNMENT`. **M24b — Root/list/dict:** root singleton cells, list/dict-поля, nested overlay-переводы.
- **M25a — Comparison v2:** дифф значений, содержимое оверлеев, root singleton. **M25b — Dataset-wide:** валидация всего датасета одним отчётом, глобальный поиск.
- **M26a — Lookup CRUD:** создание/изменение/удаление lookup-таблиц, bitmask allocation ID, free-form ID. **M26b — Protection:** «где используется», защита используемых значений, валидация flag-lookup.
- **M27a — Packaging:** self-contained publish, portable zip, CI-гейты сборки/тестов/корпуса. **M27b — Release:** иконка/версия/changelog, README и user guide.

**Критический путь:** M0 → … → M19 (item parity) → M20 → M21 → M22a → M22b → M22c → M23a → M23b (full legacy parity) → M24a → M24b → M25a → M25b → M26a → M26b → M27a → M27b. Corpus-гейты — на каждом milestone; perf-гейты — на M10, M11, M19–M22c и перед релизом.

---

## Функциональный паритет с legacy-редактором

**Эталон:** `References/xml-editor` (VB.NET/WinForms, 123 `DataTable`, 97 `DataRelation`). `References/JA_2_1.13_TableData_Editor` — незавершённый прототип, эталоном не считается.

**Определение.** Каждая реально доступная функция legacy либо реализована, либо заменена более безопасным эквивалентом; bug-for-bug совместимость не требуется. Критерий — фактическое поведение кода, а не пункты меню.

**Вне паритета (осознанно):**
- скрытые/мёртвые меню legacy (Edit, Print/Preview/Setup, Profiles, Tools/Options, Maps/Cities/SAM/Movement, New Window) и неработающие функции (BitCodeMaker, working-data cache, сохранение layout в неверный путь);
- деструктивное поведение: молчаливая потеря unknown-тегов, каскадное удаление без предупреждения, перезапись `ubClassIndex`, сброс отсутствующих изображений в `0/0`;
- сервисные/вычисляемые настройки (`AP_Maximum`, `Hide_Nothing_Items`) заменяются отображением, поиском и perf-гейтами;
- Cities/facilities, value-level compare, lookup management и packaging — M24–M27. Undo/redo, оверлеи, структурное сравнение и валидация уже сильнее legacy.

**Маркеры:** **M19 — item-workflow parity (достигнут).** **M20 — auxiliary parity (достигнут).** **M23 — full functional parity:** все рабочие функции legacy, включая multi-dataset/copy, фильтры/выделение/layout.

### Сводная матрица паритета

| Область legacy | Текущее состояние | Milestone |
|---|---|---|
| XML round-trip, undo, overlays, compare, валидация | сильнее legacy | — |
| Grid: sort/search/chooser/bulk/add/delete | реализовано, включая фильтры, multi-select и layout | M22a |
| Item master-detail и sub-tables | item workspace, включая nested/list | M19 |
| Main-table pickers (`ITEM`, `AMMO`, `ITEMTOEXPLOSIVE`) | searchable pickers, имена, переход, unknown raw | M18 |
| Merc gear | пять комплектов, item pickers, base + overlay | M20 |
| Shopkeeper/enemy/militia/IMP/sounds | 104 таблицы с типами, ключами, pickers и shopkeeper-вкладкой | M20 |
| Lookup-таблицы | generic grid и метаданные auxiliary-таблиц; CRUD — M26a | M20 |
| Локализация | 7 языковых кодов; в v4/v5 присутствует только `ru` | M20 |
| Multi-dataset и copy строк | селектор, вкладки по датасетам, copy-диалог с preview и remap | M21 |
| Item images | превью Big/Medium/Small, picker типа, миниатюры | M19 |
| Settings/layout/safety | каталоги, язык, сброс layout, safe close, external-change recovery | M22a–M22b |
| Packaging | нет у обоих | M27a–M27b |

---

## Отложено за пределы текущих milestone (осознанно)

| Фича | Причина |
|---|---|
| Слияние подтаблиц предметов в ITEM | item workspace даёт view-проекцию; слияние в модель хранения — риск для целостности |
| Shopkeeper `CONTROL` UI | в legacy его не было; `Inventory` редактируется, `CONTROL` сохраняется как есть |
| Working-data cache | perf-гейты его не требуют; кэш рискует устаревшими данными |
| Cities/facilities, value compare, lookup management, packaging | M24–M27 (в legacy отсутствуют) |
| Three-way merge, Excel-экспорт сравнения, общий STI/Interface/tileset browser | backlog после M27 |

---

## Основные риски

- **R1 остаточный:** неоднозначная структурная замена уходит в serializer и нормализует форматирование; данные при этом корректны.
- **R6 неполные метаданные:** seed покрывает 173 таблицы из 238/239 в v4/v5; document-driven fallback работает, остальные таблицы добавляются по мере необходимости (M23), миграции растут итеративно.
- **R13 многофайловое сохранение:** item workspace пишет несколько XML; per-file atomic save, ошибки агрегируются, частичное сохранение не маскируется.
- **R14 nested/list-правки:** идут через lossless-патчер и round-trip тесты; неизменённые файлы не перезаписываются.
- **R15 compound keys:** закрыт в M22c: `RecordKeyFields`, row-reference операции, no-overlay fast path и byte-safe patcher; приёмка — item 12, точное удаление строки и corpus baseline v4/v5.
- **Закрыты:** byte-identity и perf (R2), malformed-файлы (R3), dict/list-маски (R4), вложенные ключи оверлеев (R5), запись в `References/` (R7), внешние изменения (R9), культура/отмена (R10), relations drift (R11), copy (R12) — покрыты фикстурами, corpus-, compare-, copy- и perf-гейтами.

---

## Проверка

1. `dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx -warnaserror` — 0 ошибок/0 предупреждений.
2. `dotnet test xml-editor-reloaded/XmlEditorReloaded.slnx` — 415 тестов, все зелёные.
2a. M22c: `RealRecordKeyTests` — duplicate-key baseline (v4: только `AdditionalDealer_43`; v5: `Items.Attachments` 2 + `AdditionalDealer_43`), item 12 attachments = сырой XML count, точное удаление строки и undo; приёмка описана в `M22C-COMPOSITE-KEYS-PLAN.md`.
3. Corpus: `XER_CORPUS_DIR="…/v4/Data-1.13/TableData" dotnet test` (и v5) — прогон на temp-копиях, без изменений в `References/`.
4. Сравнение: `XER_COMPARE_LEFT_DIR=<v4> XER_COMPARE_RIGHT_DIR=<v5>` — `RealDataComparisonTests`.
5. Copy: `XER_COPY_SOURCE_DIR=<v4> XER_COPY_TARGET_DIR=<v5>` — `CorpusCopyTests` (temp-копии, remap, source неизменён).
6. Perf: `XER_CORPUS_DIR=<v5> dotnet test --filter "Category=Performance"` — порог 2 с.
7. Ручной smoke: v4/v5, item и merc workspace, языковые оверлеи, copy, фильтры/layout, settings, close/save и external-change сценарии; malformed lookup только диагностируется.
8. `References/` не изменён; артефакты `bin/`/`obj/` в git не попадают.
