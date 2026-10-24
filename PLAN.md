# XML Editor Reloaded — план реализации backend

## Объём milestone

**Исторический объём M0–M10:** весь backend (`Core`, `Data`, `Xml`, `Imaging`) до рабочего состояния + минимальная WPF-оболочка (composition root, настройки, выбор каталога данных, список таблиц, заглушка детального вида). Сетки редактирования, формы предметов, локализация UI-интерфейса и image-UI были следующими milestone и реализованы в M12–M17; дальнейший roadmap — M18–M27.

**Решения по стеку (согласовано):** .NET 10; тесты — xUnit; shell — CommunityToolkit.Mvvm + Microsoft.Extensions.DependencyInjection.

**Текущее состояние:** M0–M17 завершены. Backend, WPF-оболочка, grid-редактор, локализация, верифицированные метаданные, типизированные редакторы, массовые операции с undo и read-only сравнение версий работают: выбор каталога, список таблиц, mapped/unmapped/языки, record count, malformed-диагностика, настройки, RESX-локализация, STI-браузер, discovery-схема, `TableWorkspace`, runtime-сессия датасета, lossless-патчер, вкладки таблиц с виртуализированной сеткой, sparse-правка, add/dup/delete, поиск/сортировка, chooser колонок, сохранение, валидация, панель деталей, переключение языковых оверлеев, маркеры «не переведено», отчёт о переводах, создание оверлея, lookup-комбо, Boolean-чекбоксы, именованные биты, bulk set/add/multiply с preview и undo/redo, сравнение двух каталогов `TableData` с экспортом отчёта. Метаданные: 502 проверенных `ColumnHints` (224 у `Items.Items`), 46 declaring-колонок для 23 lookup-таблиц, 11 flag-lookup-связей, 64-битные lookup ID, hints в валидаторе и на backend-пути правки. Дальнейший roadmap (M18–M27) и точка паритета с legacy-редактором зафиксированы ниже: **item-workflow parity — конец M19, full legacy parity — конец M23**.

**Ключевые правила (из AGENTS.md):**
- XML-целостность: не терять поля (включая неизвестные), не нормализовать sparse-записи, сохранять лексическую форму (`1.0` остаётся `1.0`), порядок элементов, регистр имён, XML-декларацию. BOM/переводы строк сохранять необязательно, но **файл с неизменённым содержимым не перезаписывается вообще**. Повреждённые файлы — диагностика без автопочинки.
- Метаданные и документ — единственные источники; никакого хардкода полей v4/v5.
- `References/` — только чтение; тесты работают на копиях; проекты из `References/` никогда не собираются.
- UTF-8 без BOM в исходниках; XML-документация на английском; async + `CancellationToken`; сборка `dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx` зелёная на каждом шаге.
- После завершения каждого milestone — локальный git-коммит (`M0: …`, `M1: …`); push только по явной просьбе.

---

## Ключевые design-решения

1. **Round-trip модель: `XDocument` + `LoadOptions.PreserveWhitespace`** (+ снятый при загрузке `FileFormatInfo`: кодировка, newline) + жёсткое «не писать, если не dirty» + корпусный harness на реальных данных. Сериализация — `XmlWriter` без отступов, с исходной кодировкой/newline/декларацией. Перед записью — перепарс output в памяти. Запасной вариант (только если корпус не удастся вывести в byte-identity): `ValueSpanPatcher` (патчинг текстовых диапазонов значений по позициям из `IXmlLineInfo`).
2. **Sparse-запись = реальный `XElement`**, `TableRecord` — фасад над ним. Никаких DTO-строк хранилищем; типизированные значения — проекция `LexicalValue` (raw-текст, invariant culture, форматирование по форме источника).
3. **Save-gate = `IsDirty`**: мутации только через `TableDocument`/`OverlayDocument`; чистый документ не пишется; побайтно идентичный output тоже не пишется; атомарная запись (tmp + replace); проверка внешних изменений по (length, mtime).
4. **Metadata-driven + document-driven**: SQLite-метаданные (таблицы, ключи, колонки, list/dict, языки, переводимые пути) + вывод из документа для немапленных таблиц (53 из 263) и неизвестных полей.
5. **Оверлеи**: ключи — `RecordKeySpec` в БД (scalar/nested/positional); merge через `OverlayMerger`; запись перевода только в оверлей (`OverlayWriter`); отчёты `TranslationAnalyzer`.
6. **Повреждённые файлы** (v5 `Lookup/AttachmentPoint.xml`): статус `Malformed`, документ несохраняемый, `xml.parse_error` — только диагностика.
7. **Разделение проектов**: весь XML-парсинг (включая лукапы) — в `Xml`; `Data` — только SQLite-метаданные + файловый каталог + настройки (JSON в `%AppData%`); контракты в `Core` с примитивными payload.
8. **Метабаза**: линейные embedded SQL-миграции (`SchemaMigrations`), seed из прототипной БД (53 таблицы) + вручную authored `TableKeys`/`Languages`/`TranslatableFields`.
9. **Фикстуры**: маленькие реальные сэмплы копируются в тест-проекты; полный корпус — env-gated тест `XER_CORPUS_DIR` (копия во temp перед любой записью).
10. **STI**: little-endian заголовок 64 байта, `appDataSize@48` (не баг STI.vb со смещением 34), палитра RGB×3, фрейм-директория 16 байт; ETRLE: `0` = конец строки, `>127` = transparent run (alpha 0), `1..127` = literal; индекс 0 всегда прозрачен; green-key только medium/small; на выходе RGBA-буферы, без `System.Drawing`.

---

## Milestones (порядок зависимостей)

### M-pre — Публикация плана (~5 мин; первое действие при выходе из plan mode)
Скопировать этот план в **`E:\Projects\JA2_1.13 Tools\PLAN.md`** (корень репозитория, UTF-8 без BOM), чтобы другие агенты могли ссылаться на него. Обновлять `PLAN.md` по завершении существенных milestone (отмечать сделанное, вносить изменения в course). Корень репозитория по правилам AGENTS.md предназначен для документации и конфигурации — это исключение допустимо.

### M0 — Scaffolding (~0.5 дн) — **выполнен**
`Directory.Build.props` (net10.0, nullable, docs XML для backend), `.editorconfig` (utf-8), 4 тест-проекта в `tests/` (`Core.Tests`, `Data.Tests`, `Xml.Tests`, `Imaging.Tests`; `Data.Tests` — новое относительно PROJECT-STRUCTURE.md) + xUnit-пакеты; `Microsoft.Data.Sqlite` → Data; MVVM-пакеты → App. Smoke-тест в каждом.
**Готово:** build + `dotnet test` зелёные.

### M1 — Core (~1 дн) — **выполнен**
Типы: `OperationResult`/`OperationResult<T>`/`ErrorCodes`; `TableName`, `TableMetadata`, `ColumnMetadata`, `FieldPath`, `RecordKeySpec`/`RecordKeySegment`, `ListFieldDefinition`, `DictFieldDefinition`; абстракции `ITableMetadataProvider`, `ITableCatalog`, `ITableRepository`, `ILookupRepository`, `ITranslationCatalog`, `ISettingsStore`; `LanguageDescriptor`, `OverlayDescriptor`, `TranslationReport`; `ValidationIssue`/`ValidationReport`; `RgbaBuffer`, `ImageSize`. Без I/O.
**Готово:** XML-доки на всех public-членах, тесты `TableName`/`FieldPath`/`RecordKeySpec`/`OperationResult` зелёные.

### M2 — Data (~2–3 дн; параллельно с M7) — **выполнен**
- DDL: `SchemaMigrations`, `Tables` (superset `TableMappings`), `TableKeys`, `Columns`, `ListFields`, `DictFields`, `Languages`, `TranslatableFields`. Embedded-миграции `Metadata/Migrations/000N_*.sql` + `DatabaseMigrator` (транзакция, downgrade = error).
- Seed: `0002_PrototypeSeed.sql` сгенерирован из прототипной БД + вручную: 73 таблицы (53 прототипных + 20 с оверлеями), 76 key-сегментов (nested/positional), `ru`→`Russian`, 61 переводимый путь 25 оверлеев, 12 list/dict полей.
- `SqliteMetadataProvider`, `FileSystemTableCatalog` (обход `TableData/**/*.xml`, исключение оверлеев по префиксу из `Languages`, optional metadata для `IsMapped`), `TranslationCatalog`, `JsonSettingsStore` (`%AppData%/XmlEditorReloaded/settings.json`, atomic write).
**Фактический результат:** миграции применяются идемпотентно, downgrade блокируется; свежая БД содержит 73 таблицы; провайдер читает ключи (включая `CITYLIST/CITY` и `GEARKIT`), list/dict-поля, переводимые пути и языки. Каталог на temp-фикстуре различает базу и оверлеи; на реальных данных v4 найдено 238 базовых таблиц и 25 оверлеев, v5 — 239 базовых и 0 оверлеев. Настройки сохраняются атомарно.
**Готово:** 14 Data-тестов зелёные; `PROJECT-STRUCTURE.md` содержит `Data.Tests`; следующий — M5, M7 можно вести параллельно.

### M7 — Imaging (~2–3 дн; параллельно с M2) — **выполнен**
`StciHeader`/`StciFlags`/`SubImageDescriptor`/`StciImage`/`StciParser` (ошибки `sti.bad_magic/truncated/unsupported_flags`); `EtrleDecoder` (control-byte семантика, overrun = diagnostic); `ItemImageCatalog` (Big: `BigItems/gun{NN}.sti`/`p{t}item{NN}.sti`, паддинг до 2 цифр только для <10, case-insensitive; Medium: `Interface/mdguns.sti`/`mdp{t}items.sti` frame=index; Small: `tilesets/0/smguns.sti`/`smp{t}items.sti`; прозрачность: индекс 0 везде, зелёный 0,255,0 — medium/small); `DecodedFrameCache` (LRU ~64–128 МБ, invalidation по mtime). Фикстуры: 2–3 маленьких реальных STI, скопированных в тест-проект.
**Фактический результат:** реализованы парсер STCI (canvas 640×480, palette RGB×3, directory 16 байт, `appDataSize` @48), ETRLE-декодер (0 = EOL, >127 = transparent run, 1..127 = literal; overrun/truncated диагностируются), каталог big/medium/small изображений, зелёный ключ для medium/small и LRU-кэш с invalidation по mtime. Фикстуры: реальные `P2ITEM35.STI`, `MDP3ITEMS.STI`, `SMP3ITEMS.STI`.
**Готово:** 18 Imaging-тестов, всего 110 тестов; нулевые ссылки на WPF/System.Drawing; build 0 warnings/errors.

### M3 — Xml: загрузка, round-trip, диагностика (~4–6 дн; самое сложное) — **выполнен**
`FileFormatInfo`, `TableFileLoader` (`XmlReaderSettings` + `PreserveWhitespace`, `XmlException` → `xml.parse_error` + line/position), `DocumentSerializer`, `TableFileSaver`, `TableDocument`, `TableRecord`, `LexicalValue`, `TableLoadResult` (`Loaded|LoadedReadOnly|Malformed`), `XmlFileLookupRepository`.
**Алгоритм сохранения:** dirty? → serialize → re-parse verify → сравнить с оригиналом → если идентично, не писать → атомарная запись → проверка (length, mtime).
**Тесты:** байтовая идентичность неотредактированной фикстуры; «не перезаписан вообще» (mtime); sparse-набор полей; `1.0`/пустые/повторяющиеся/неизвестные поля; порядок и регистр; v5 AttachmentPoint → ошибка без записи; `EditSingleValue_Save_ChangesOnlyThatValue`; BOM/newline; внешняя модификация. **Corpus harness** (`XER_CORPUS_DIR`, копия во temp): v4 — 263 файла, v5 — 239 файлов + 1 malformed; проверяется отсутствие перезаписи неизменённых файлов.
**Фактический результат:** загрузка, диагностика, редактирование одного значения, save-gate и corpus no-rewrite зелёные на v4 и v5. Добавлен `LosslessDocumentPatcher`: скалярные правки применяются к исходным байтам, поэтому declaration casing, пробелы в атрибутах, `&gt;`, `&#10;`, self-closing теги и остальное форматирование сохраняются. Для структурных изменений (add/delete/duplicate) используется `DocumentSerializer`, который нормализует форматирование; это закрывается в M4. `PrimaryKeyAllocator` реализован как `TableDocument.GetNextPrimaryKey`; `RowFactory` перенесён в M4.
**Готово (фактически):** corpus no-rewrite; повреждённый файл недеструктивен; скалярная правка меняет только своё значение, форматирование файла сохраняется.

### M4 — Xml: редактирование и строки (~1–1.5 дн) — **выполнен**
`SetFieldValue` (меняет только текст; отсутствующее поле не добавляется без явного `addFieldIfMissing`), `SetListField`, `SetDictField` (маска+индекс), `RecordOps` (Add max+1 / Duplicate deep-clone / Delete), `DocumentChangeTracker` (журнал для будущего undo), отклонение невалидных чисел (`xml.invalid_number`) без мутации документа.
**Фактический результат:** реализованы `TableRecord.SetFieldValue`/`SetListField`/`SetDictField`, `TableDocument.AddRecord`/`DuplicateRecord`/`DeleteRecord`, `DocumentChangeTracker`. `RowFactory` реализован как `AddRecord`/`DuplicateRecord`. Числовая валидация идёт по `ColumnMetadata`; невалидное значение не меняет документ. Добавлены тесты на edit/list/dict/add/dup/delete и сохранение структурных изменений.
**Готово:** 41 XML-тест, полный набор 65 тестов, corpus no-rewrite v4/v5 зелёные. Для структурных изменений по-прежнему используется `DocumentSerializer`, поэтому форматирование файла может нормализоваться; lossless-патчер для add/delete/duplicate остаётся follow-up (R1).

### M5 — Xml: оверлеи (~2–3 дн) — **выполнен**
`OverlayDocument`, `OverlayKeyResolver` (nested/positional, `UnresolvableKey` = diagnostic), `OverlayMerger` (base+overlay → merged cells с признаком перевода), `OverlayWriter` (запись только в оверлей; новая запись = ключ + правленое поле), `TableEditSession` (роутинг: переводимое+язык → оверлей, иначе base; независимые save), `TranslationAnalyzer` (missing/orphan/duplicate/empty/identical/KeyCountMismatch).
**Фактический результат:** реализованы загрузка оверлеев (`LoadOverlayAsync` + `OverlayDocument`), резолвер ключей (scalar/positional/singleton), merge с base-fallback и признаком перевода, запись переводов только в оверлей (новая запись — ключ + поле), маршрутизация `TableEditSession`, анализ переводов. Nested-кейсы: `GEARKIT` по позиции, `ASSIGNMENT` по `ubAssignmentType`, `CITYLIST/CITY` + `COUNTRY` singleton. Base-файл при translation-only save побайтово не изменяется.
**Готово:** 8 overlay-тестов, всего 49 XML-тестов и 86 тестов в решении; сценарии Items, PocketPopups, MercStartingGear, Cities проходят.

### M6 — Xml: валидация (~1 дн; параллельно с M5) — **выполнен**
`TableValidator`: unknown-field = info (не error), bad type = warning с ключом+путём, lookup-ref, дубликаты ключей, форма dict-масок. Только чтение.
**Фактический результат:** `TableValidator.ValidateAsync` валидирует mapped-таблицы: unknown fields — Info, неверный тип/отсутствующий lookup/дубликат или отсутствие ключа/некорректная dict-маска — Warning. Для document-driven таблиц возвращается один Info `validation.unmapped`. Валидатор использует `ColumnMetadata` и `ILookupRepository`, не мутирует документ. Добавлены коды `ValidationCodes`.
**Готово:** 8 M6-тестов, всего 57 XML-тестов и 118 тестов в решении. Ограничение: seed пока не содержит `Columns`, поэтому на реальных данных проверки типов/lookup включатся после заполнения колоночных метаданных; unknown-field проверка выполняется только при наличии Columns.

### M8 — Wiring (~1–1.5 дн) — **выполнен**
`TableRepository` (реализация `ITableRepository`), `ServiceCollectionExtensions` в каждом backend-проекте; join правило: нет метаданных → document-driven, статус «unmapped». Интеграционные тесты (end-to-end load-edit-save, malformed fail-soft, lookup overlay, translation cycle через `TableEditSession`).
**Фактический результат:** `TableRepository` загружает mapped-таблицы с metadata и падает обратно в document-driven режим с диагностикой `metadata.unmapped`; `AddXmlEditorData`/`AddXmlEditorXml` собирают metadata provider, каталог, translation catalog, settings, repository и lookup; `FileSystemTableCatalog` и `XmlFileLookupRepository` сами получают языки из metadata provider. Добавлены 7 интеграционных тестов: load/edit/save, malformed fail-soft, unmapped fallback, lookup overlay, translation cycle, composition root.
**Готово:** 21 Data-тест, всего 93 теста; build 0 warnings/errors; corpus no-rewrite v4/v5 зелёный.

### M9 — Минимальный WPF shell (~1–1.5 дн) — **выполнен**
`App.xaml.cs` — composition root (`AddXmlEditorData/Xml/Imaging`); `IDialogService` (folder picker); `AppSettingsViewModel`; `MainViewModel` + список таблиц из `ITableCatalog` (имя, оверлей-языки, mapped/unmapped); выбор таблицы → record count или malformed-диагностика (async + CTS). Строки UI — сразу в RESX (`Strings.resx` + `Strings.ru.resx`).
**Фактический результат:** `App` собирает DI, применяет миграции, читает настройки и применяет `UiLanguageCode` до создания окна; `WpfDialogService` использует `OpenFolderDialog`; `MainViewModel` загружает каталог, показывает mapped/unmapped и языки, по выбору таблицы загружает документ, record count и диагностику, поддерживает отмену операций; UI-строки в RESX с английским и русским ресурсами и satellite-сборкой. Настройки хранятся через `JsonSettingsStore`; выбранный каталог сохраняется. Отдельный `AppSettingsViewModel` не понадобился: настройки ведёт `MainViewModel` + `ISettingsStore`.
**Готово:** приложение запускается (проверено), создаёт окно и остаётся работоспособным; добавлен `App.Tests` с проверкой реального каталога и загрузки записи. Всего 119 тестов.

### M10 — Hardening — **выполнен**
Perf-тесты (env-gated), cancellation-тесты, аудит XML-доков (CS1591), обновление документации, финальные `dotnet build`/`dotnet test` и оба корпусных прогона.
**Фактический результат:**
- Perf-гейты (`XER_CORPUS_DIR`, порог 2 с, `[Trait("Category","Performance")]`): `Items/Items.xml` — 1775 записей за 75 мс; `Items/Attachments.xml` — 6023 записи за 27 мс; load + правка + save `Items.xml` — 380 мс; каталог — 239 таблиц за 30 мс; метаданные — 73 таблицы за 32 мс.
- Отмена: pre-cancelled token проверяется в loader (до парсинга), saver (до сериализации, до записи и перед `File.Move`), catalog, metadata provider и `ItemImageCatalog`. Saver при отмене не оставляет ни изменённого файла, ни temp-файла; добавлены `<exception cref="OperationCanceledException">` в контракты Core и реализаций.
- Культура: decimal-правка хранит invariant-текст (`1.5`), запятая как разделитель отклоняется (`xml.invalid_number`, документ не меняется), `AddRecord` под `ar-SA` пишет ASCII-ключ.
- UI: generation-счётчик не даёт старой отменённой операции сбросить `IsBusy`/`StatusText` новой; неожиданные исключения каталога/загрузки выводятся в статус. Добавлены тесты на неверный каталог, unmapped-таблицу и падающий каталог.
- Build 0 warnings (`-warnaserror`), 135 тестов зелёные, корпуса v4 (263 файла) и v5 (239 + 1 malformed) без перезаписи.
- Ограничение: `Columns` в метабазе по-прежнему пуста; типизированные редакторы появятся с discovery в M11.
- Quick win: STI-браузер (см. ниже), после него в решении 148 тестов.

### Quick win — STI-браузер — **выполнен** (после M10)
Отдельное окно «Изображения предметов»: список `Items.Items` (`uiIndex`, `szItemName`), автовыбор первого предмета с `ubGraphicNum`, ручной ввод `ubGraphicType`/`ubGraphicNum`, три панели big/medium/small с шахматным фоном, масштаб 1–8× и пиксельным фильтром, previous/next, per-panel размеры или код ошибки. Слой App: `DataSetPaths` (TableData/Data-1.13), `WpfItemImageService` (кэш `ItemImageCatalog` по каталогу + RGBA→BGRA `BitmapSource`), `ImageViewerViewModel`/`ImageViewerWindow`, кнопка в главном окне, RESX en/ru. Композиция вынесена в `App.ConfigureServices` и покрыта тестом. Env-gated тест на реальном наборе проверяет список предметов и декодирование `P2ITEM35.STI`.

### M11 — Schema discovery, table workspace, lossless structural patcher — **выполнен**
- `TableSchema`/`TableSchemaBuilder`: объединение полей всех записей в порядке первого появления, presence count, max occurrences, вывод типа по значениям (Boolean/Integer/Decimal/String; ключ никогда не Boolean), признаки key/list/dict/translatable, overrides из `Columns` (тип, editor kind, ширина, lookup, tooltip, flags), declared-поля, отсутствующие в документе.
- `TableWorkspace` + `WorkspaceRow`: загрузка base + опциональный overlay по `LanguageDescriptor`, discovery-схема, merged rows (`GetValue`/`GetValues`/`IsTranslated`), root singleton cells, маршрутизация правок (`TableEditSession`), add/duplicate/delete с удалением перевода из оверлея, единое сохранение, валидация, `TranslationAnalyzer`.
- Runtime-биндинг: `IDataSetSessionFactory`/`DataSetSession` в App создают lookup и translation catalog под выбранный `TableData` (кэш сессий по каталогу) и открывают таблицу через workspace. DI-граф обновлён.
- R1: `LosslessDocumentPatcher` теперь применяет скалярные правки, вставки и удаления элементов байтовым splice, имитируя отступы соседей; новые записи pretty-print'ятся с сохранением стиля файла; после патча дерево сверяется с текущей моделью, при неоднозначности — fallback на serializer.
- `TableDocument.SynchronizeRecords` синхронизирует список записей после создания/удаления оверлейной записи.
**Фактический результат:** 175 тестов; workspace открывает `Items.xml` (1775 записей, 224 колонки) за 86 мс; add+delete записи на реальном `Items.xml` возвращает файл байт-в-байт; корпуса v4/v5 зелёные; build 0 warnings (`-warnaserror`). Ограничение: schema — снимок на момент загрузки (для новых полей нужен новый workspace); структурная замена блока (несколько удалений и вставок в одном родителе) уходит в serializer.

### M12 — Grid editor UI — **выполнен**
- Вкладки открытых таблиц: открытие двойным щелчком/Enter/кнопкой, закрытие с подтверждением несохранённых изменений, dirty-маркер в заголовке.
- Виртуализированная `DataGrid` на `TableWorkspace`: динамические `DataGridTextColumn` из схемы, привязка к индексатору строки, ленивое чтение sparse-значений, key/list/dict колонки read-only.
- Sparse-правка: ввод в пустую ячейку добавляет поле (`addIfMissing`); числовая валидация по выведенному типу (Integer/Decimal/Boolean, запрет запятой и группировки); невалидное значение не меняет документ и отображается в статусе.
- Chooser колонок: фильтр по имени/типу, чекбоксы, Default/All; по умолчанию key + 7 скалярных полей.
- Add/duplicate/delete с подтверждением удаления, удаление перевода из оверлея, поиск по key/видимым колонкам, сортировка с учётом типа, панель валидации, Ctrl+S, Save all, Reload.
- Язык: при наличии оверлея редактор открывается на `ActiveLanguageCode` из настроек и показывает merged-значения; переключатель языка — M14.
- `WorkspaceRow` отдаёт live-значения base/overlay после правки; для вновь созданной оверлейной записи строка обновляется через `GetRow(key)`.
**Фактический результат:** 185 тестов; реальный `Items.xml` открывается в редакторе (1775 строк, 224 колонки); v4/v5 корпуса и `-warnaserror` зелёные. Ограничения: нет undo/redo; list/dict/key поля редактируются только в будущих спец-редакторах; вложенные таблицы (Cities) пока read-only; раскладка колонок не сохраняется между сессиями; Boolean редактируется текстом 0/1; курируемые `Columns` overrides ещё не заполнены.

### M13 — Item detail form + STI (~2–3 дн) — **выполнен**
- Панель «Детали» выбранной строки внутри вкладки таблицы: группировка полей, фильтр по имени/типу, правка через `TableWorkspace`, пометка переводимых значений.
- Непродекларирующие `ColumnHints` в SQLite (миграция `0003_ColumnHints.sql`): тип, редактор, группа, порядок, lookup, tooltip, flags. Seed `0004_ItemColumnHints.sql` — 224 поля `Items.Items`, включая 7 масочных полей с `FlagEditor` (`usItemClass`, `AttachmentClass`, `nasAttachmentClass`, `nasLayoutClass`, `AvailableAttachmentPoint`, `AttachmentPoint`, `usActionItemFlag`).
- Схема: `GroupKey`/`GroupOrder`, определение вложенных элементов (`ColumnDataType.Nested`, read-only) и fallback-группы по типу для полей без подсказок.
- Три панели Big/Medium/Small по `ubGraphicType`/`ubGraphicNum` с общей отменой загрузки и per-panel кодами ошибок; панель изображений скрывается, если в таблице нет `ubGraphicNum`.
- Битовый редактор `uint` (32 бита, значения до `4294967295`), доступен только для флаговых колонок; подтверждённые и неподтверждённые правки не меняют sparse-структуру.
**Фактический результат:** 192 теста; реальный `Items.xml` загружается в редактор (1775 строк, 224 колонки, группы identity/attachments/stance/flags); v4 и v5 корпуса зелёные; build 0 warnings (`-warnaserror`). Ограничения: Boolean-флаги пока редактируются текстом `0`/`1` (checkboxes — в M15); вложенные структуры и list/dict остаются read-only до спец-редакторов.

### M14 — Localization UI (~2–3 дн) — **выполнен**
- Переключатель языка во вкладке: базовая таблица, существующие оверлеи и языки, для которых оверлей можно создать. Смена языка перезагружает workspace; несохранённые изменения требуют подтверждения.
- Merged-значения и маркеры «не переведено»: подсветка переводимых ячеек в сетке с tooltip, маркер в панели деталей; отсутствующие в базе поля не помечаются.
- Отчёт о переводах: окно со сводкой по типам (missing/orphan/duplicate/empty/identical/key-count/unresolvable) и списком записей/полей.
- Создание оверлея: `OverlayDocument.CreateAsync` пишет пустой корень (UTF-8 без BOM), записи и переводимые поля добавляются лениво при правке; `IDataSetSession.EnsureOverlayAsync` не перезаписывает существующий файл. Базовая таблица не копируется и не изменяется.
- Правка переводимого поля пишется только в оверлей; правка непереводимого поля в языковом режиме идёт в базовую таблицу и отмечается в статусе. Выбранный язык сохраняется в `AppSettings.ActiveLanguageCode` для новых вкладок.
**Фактический результат:** 205 тестов (22 Core, 90 Xml, 25 Data, 19 Imaging, 49 App); v4 corpus с реальным `Russian.Items` overlay и v5 corpus зелёные; build 0 warnings (`-warnaserror`). Ограничения: сообщения в отчёте приходят из backend-анализатора на английском; нет side-by-side сравнения и автозаполнения переводов; созданный оверлей пуст и наполняется по мере правок; list/dict/nested остаются read-only.

### M14.5 — Metadata seeding: Columns, hints, validation — **выполнен**
Закрывает ограничение M6/M10/M13: на реальных данных типовая и lookup-валидация фактически не работала, а lookup-редакторы M15 не имели метаданных. Seed сделан не слепым импортом, а через сверку с v4 и v5.

- **Инвентаризация:** `References/xml-editor/BackEnd/DatabaseCode.vb` — 710 вызовов `MakeColumn`; после раскрытия циклов получено 1883 определения колонок, из них 945 относятся к 47 таблицам, присутствующим в текущем seed (73 таблицы). Остальные legacy-таблицы (shopkeeper, enemy/militia choices, sounds и т. п.) не входят в seed и остаются document-driven.
- **Сверка:** каждое поле проверено по v4 и v5: поле встречается в документе, непустые значения парсятся в заявленный тип, а lookup-ссылки существуют в `Lookup/*.xml`. Результат: 564 подтверждённых поля у 47 таблиц; 0 type-mismatch; 0 lookup-mismatch для реальных lookup-файлов; 381 устаревшее поле исключено; 99 ссылок на основные таблицы (`ITEM`, `AMMO`, `ITEMTOEXPLOSIVE` и т. п.) не импортированы как lookup и задокументированы.
- **Политика:** `ColumnHints` — основной non-declaring слой; `Items.Items` остаётся на hints (224 поля). `Columns` заполнена только для 23 lookup-таблиц (46 колонок `id`/`name`), где объявление безопасно. `Items.ItemFlag.id` объявлен как `Raw`, потому что ID доходят до `8589934592`; остальные lookup `id` — `Integer`.
- **Миграция:** `0005_VerifiedColumnMetadata.sql` содержит 493 upsert'а hints (215 из них — типы/editor'ы для `Items.Items`) и 46 declaring-колонок. Итог: 502 hints у 25 таблиц. Для `Items.Items` типизированы все 224 поля: 105 Boolean, 87 Integer, 12 Decimal, 8 Flags, 3 Lookup, 6 String (тип наследуется discovery), 3 nested.
- **64-битные lookup ID:** `LookupEntry.Id` переведён с `int` на `long`; `XmlFileLookupRepository` читает 64-битные `id`. `Lookup/ItemFlag.xml` (ID `8589934592`) теперь загружается. Маски валидируются как `uint`, lookup-значения — как `long`, целые — как `int`.
- **Валидация:** `TableValidator` для present-полей сначала берёт `Columns`, затем `ColumnHints`; unknown-field выдаётся только для declaring-таблиц. `TableRecord.ValidateValue` и UI-валидация также используют hints, поэтому backend-правка не может записать `abc` в поле, типизированное как Integer.
- **Тесты:** 213 тестов; новые проверки metadata, `Items.Items` (224 hints, 8 FlagEditor, 3 Lookup), 64-битного lookup, hint-валидации в `TableValidator`/`TableRecord`, uint-диапазона флагов, а также env-gated `RealDataValidationTests` — v4 и v5 corpus без ложных `invalid_type`/`lookup_missing`.
- **Отчёт:** `M14.5-METADATA-REPORT.md` фиксирует методику, 381 устаревшее поле, 47 полей без legacy-описания и 99 main-table lookup-ссылок.
**Фактический результат:** 213 тестов (22 Core, 96 Xml, 27 Data, 19 Imaging, 49 App); v4 и v5 корпуса зелёные; build 0 warnings (`-warnaserror`). Ограничение: lookup-ссылки на основные таблицы (`ITEM`, `AMMO`, `ITEMTOEXPLOSIVE`) пока не используются как combo-источники; это задача M15/последующей модели связей.

### M15 — Lookup, Boolean, and flag editors — **выполнен**
Зависит от M14.5 (нужны `LookupTable` и типы полей). Закрывает первую часть исходного M15; массовые операции и сравнение версий выделены в M16/M17, потому что bulk без undo рискован, а сравнение версий — независимый read-only отчёт.

- **Метаданные:** миграция `0006_FlagLookupMappings.sql` связывает 11 flag-полей с lookup-файлами: `usItemClass`/`pRestriction` → `Lookup.ItemClass`, `AttachmentClass` → `Lookup.AttachmentClass`, `nasAttachmentClass` → `Lookup.NasAttachmentClass`, `nasLayoutClass` → `Lookup.NasLayoutClass`, `AttachmentPoint`/`AvailableAttachmentPoint` → `Lookup.AttachmentPoint`, `DrugType` → `Lookup.DrugType`, `ammoflag` → `Lookup.AmmoFlag`. `usActionItemFlag` остаётся без имён: подходящего lookup-файла нет. Связи не меняют тип и не добавляют отсутствующие поля.
- **Lookup-данные:** `LookupColumnData` загружается на колонку с учётом выбранного языка; неизвестные значения сохраняются как raw и показываются с маркером `(unknown)`, известные — именем; ошибка загрузки (например, повреждённый v5 `Lookup/AttachmentPoint.xml`) переводит колонку в текстовый fallback с предупреждением в заголовке, не мешая открыть таблицу.
- **Сетка:** lookup-колонки используют `DataGridTemplateColumn` с combo, Boolean — checkbox, при ошибке lookup — текстовую колонку с `⚠`; сортировка и поиск продолжают работать по raw-значениям.
- **Панель деталей:** для lookup-поля показывается combo со всеми известными и неизвестными значениями колонки, для Boolean — checkbox с различением «не задано» и явного `0`, для остальных — текстовое поле. При ошибке lookup поле остаётся текстовым и помечается предупреждением.
- **Биты:** `BitEditorViewModel` принимает имена битов из lookup-масок (только записи с ID-степенью двойки), показывает точное имя составного значения (например, `All Molle`) и итоговое значение. Редактор по-прежнему 32-битный: реальные flag-поля имеют тип `uint`; 64-битные lookup ID относятся к таблицам справочников, не к маскам.
- **Тесты:** 220 тестов (22 Core, 96 Xml, 27 Data, 19 Imaging, 56 App), включая unknown-ID, lookup-ошибку, Boolean sparse-семантику, именованные биты и env-gated проверку реального `Items.xml` на v4/v5. Build 0 warnings (`-warnaserror`), оба корпуса зелёные.
**Ограничения:** combo доступен только для полей, у которых lookup объявлен в метаданных (15 Lookup-полей у 10 таблиц); lookup-ссылки на основные таблицы (`ITEM`, `AMMO`, `ITEMTOEXPLOSIVE`) всё ещё не импортированы, поэтому выбора предмета по имени в M15 нет. В combo нельзя ввести произвольный новый ID — только выбрать известное или уже присутствующее в колонке значение. Составные lookup-записи не раскладываются на отдельные биты, а показываются как точное имя значения. Управление lookup-таблицами (выделение ID для bitmask-справочников, защита от удаления используемых значений) и merc gear editor остаются отдельными задачами.

### M16 — Bulk operations and undo — **выполнен**
- **Backend:** `BulkEditRequest`/`BulkEditPreview`/`BulkEditPreviewRow`, `TableWorkspace.PreviewBulk`/`ApplyBulk`. Set/add/multiply по одной скалярной колонке; по умолчанию меняются только существующие поля, режим `IncludeMissingFields` создаёт отсутствующие (для арифметики отсутствующее значение считается нулём). Preview детерминирован и не мутирует документ; apply повторно валидирует запрос, отказывается при ошибках в отдельных строках или отсутствии изменений и применяет всё одной транзакцией. Ключи, list/dict/nested-колонки и неизвестные поля запрещены; числа — invariant culture без групповых разделителей.
- **Undo/redo:** `TableEditHistory` хранит снимки «до/после» затронутых записей (не более 50 транзакций и 10 000 снимков) для правок ячеек, set-nested/root, add/duplicate/delete, bulk и переводов. Одна bulk-операция — один шаг undo; undo пересчитывает `IsDirty` сравнением с исходным деревом, а повторное сохранение после отмены скалярной правки восстанавливает исходные байты через lossless-патчер. Новая правка очищает redo.
- **UI:** кнопка «Массовая правка…» и диалог с выбором колонки/операции/значения, live-preview (показываются только изменяемые и ошибочные строки), чекбокс создания отсутствующих полей, применение одной транзакцией; кнопки Undo/Redo, Ctrl+Z / Ctrl+Y / Ctrl+Shift+Z и обновление доступности команд после любой правки, сохранения и перезагрузки языка. Операции add/multiply автоматически сбрасываются на set при выборе нечисловой колонки.
- **Производительность (`Items.xml`, ~1776 строк):** base preview 28–40 мс, apply 91–107 мс, undo 34 мс; bulk переводов в v4 overlay — preview 39–68 мс, apply 751–790 мс, undo 23–27 мс; гейт 2 с. Base-путь использует индекс записей, overlay-путь — индекс верхнеуровневых элементов и одну синхронизацию после пакета.
- **Тесты:** 244 (22 Core, 19 Imaging, 27 Data, 113 Xml, 63 App), включая очистку redo новой правкой, undo после save с побайтовым восстановлением, отказ для key/list/unknown полей, culture-independent арифметику, overlay-bulk и env-gated perf-гейты. Корпуса v4 и v5 зелёные.
**Ограничения:** bulk применяется ко всем строкам текущего вида (с учётом поиска/сортировки), а не к выделению; диалог не предлагает пустой операнд для Set (backend его допускает); вложенные ключи оверлеев (`Cities`) остаются вне grid/bulk — как и одиночная правка, для них нужен отдельный nested-редактор.

### M17 — Cross-version compare — **выполнен**
- **Backend:** `ITableComparisonService`/`TableDataComparer`, `TableSnapshotAnalyzer`, `TableComparisonReportFormatter`, модели `TableComparisonReport`, `TableComparisonSummary`, `TableComparisonEntry`, `TableComparisonSide`, `TableSideComparison`, `FieldComparison`, `LookupReferenceComparison`. Сравнение читает только; ни один файл не открывается на запись.
- **Таблицы и записи:** оба каталога обходятся `ITableCatalog`, таблицы объединяются по имени; для каждой стороны файл загружается `TableFileLoader` с метаданными. Malformed или ошибка I/O одной стороны не прерывают отчёт: сторона получает статус `Malformed`/`Failed`, другая анализируется. Для таблиц с nested-ключом (`Cities`, `CITYLIST/CITY`) записи берутся из потомков, для остальных — прямые дочерние элементы root. Поля sparse-записей объединяются в порядке первого появления.
- **Метрики:** record count, field count, число различных наборов полей, min/max/avg полей на запись, пустые записи, языковые оверлеи. Типы выводятся по значениям (Boolean/Integer/Decimal/String/Nested); key-поле не становится Boolean. Сравниваются наличие поля, выведенный тип и кратность (скаляр ↔ повторяющийся элемент). Поля не сравниваются, если одна сторона не разобралась (иначе все поля выглядели бы удалёнными).
- **Lookup-связи:** собираются из `Columns` и `ColumnHints`; для каждой ссылки проверяется доступность справочника на обеих сторонах: `Available`/`Missing`/`Unreadable`. В v5 `Lookup.AttachmentPoint` даёт `Unreadable`, а не «отсутствует».
- **Экспорт:** Markdown (сводка, lookup-ссылки, список таблиц и детали только по изменённым таблицам) и CSV long-format (`Table`/`Field`/`Lookup`/`Warning` строки, RFC 4180 quoting). Оба формата детерминированы и используют invariant-числа; файл пишется в UTF-8 без BOM.
- **UI:** кнопка «Сравнить версии…» на главном окне, отдельное окно с двумя каталогами, отменой, сводкой, таблицей таблиц, вкладками «Поля»/«Lookup-связи», sparse-профилем и диагностикой выбранной таблицы, экспортом через save dialog. Все строки — в RESX (en/ru).
- **Тесты:** 19 новых (13 Xml, 5 App, 1 env-gated Data); всего 264. Env-gated сравнение v4↔v5 (`XER_COMPARE_LEFT_DIR`/`XER_COMPARE_RIGHT_DIR`) проверяет 238/239 таблиц, 238 общих, одну right-only (`Email.EmailSenderNameList`), 25 таблиц с русскими оверлеями, malformed `Lookup.AttachmentPoint` в v5, record counts `Items.Items` 1776/1775, `Items.Weapons` 389/388, `Items.Attachments` 6079/6023, `AttachmentPoint`-lookup как `Unreadable` справа и отсутствие изменений в обоих каталогах. Сравнение ~1 с.
**Ограничения:** значения ячеек не сравниваются, только структура и типы; singleton-поля nested-таблиц вне record scope (например `COUNTRY`) в профиль не попадают; наличие оверлеев сравнивается, но их содержимое — нет; отчёт не поддерживает трёхстороннее слияние и экспорт в Excel.

---

## Функциональный паритет с legacy-редактором

**Эталон:** `References/xml-editor` — единственный реально работающий legacy-редактор (VB.NET/WinForms, 123 `DataTable`, 97 `DataRelation`). `References/JA_2_1.13_TableData_Editor` — незавершённый прототип с одной кнопкой «Test» и неработающим парсером, поэтому эталоном паритета он не считается и используется только как источник имён полей.

**Определение.** Функциональный паритет означает, что каждая реально доступная и работающая пользовательская функция legacy либо реализована в XML Editor Reloaded, либо заменена более безопасным эквивалентом. Bug-for-bug совместимость не требуется. Критерий проверяется по фактическому поведению legacy-кода, а не по пунктам его меню.

**Вне паритета (осознанно):**
- скрытые и мёртвые меню legacy: Edit (undo/redo/cut/copy/paste), Print/Preview/Setup, Profiles, Tools/Options, Maps/Cities/SAM/Movement, New Window;
- неработающие функции: BitCodeMaker (скрыт и подключён не к той таблице), working-data cache (перекрывает актуальную схему), сохранение layout в неверный путь;
- деструктивное поведение: молчаливая потеря unknown-тегов, каскадное удаление без предупреждения, перезапись `ubClassIndex`, сброс отсутствующих изображений в `0/0`;
- сервисные/вычисляемые настройки (`AP_Maximum`, `Hide_Nothing_Items`, working cache) — заменяются отображением, поиском и текущими perf-гейтами;
- Cities/facilities, value-level compare, lookup management и packaging не требуются для паритета — они запланированы в M24–M27; undo/redo, оверлеи, структурное сравнение и валидация уже сильнее legacy.

**Маркеры:**
- **M19 — Item-workflow parity:** Items + все 1:1/1:N sub-tables, item pickers, nested modifiers, локализованные поля, изображения и item-workflow. Вкладка shopkeeper inventory относится к M20 и в этот маркер не входит.
- **M23 — Full functional parity:** все рабочие функции legacy, включая merc gear, shopkeeper/enemy/militia/IMP/sounds, multi-dataset/copy, фильтры/выделение/layout. Ориентировочно 22–31 рабочий день после M17.

### Сводная матрица паритета

| Область legacy | Текущее состояние | Milestone |
|---|---|---|
| XML round-trip, undo, overlays, compare, валидация | уже сильнее legacy | — |
| Grid: sort/search/chooser/bulk/add/delete | частично; нет фильтров и multi-select | M22 |
| Item master-detail и sub-tables | отсутствует; есть generic grid и detail-панель | M19 |
| Main-table pickers (`ITEM`, `AMMO`, `ITEMTOEXPLOSIVE`) | отсутствуют | M18 |
| Merc gear | таблица открывается, `GEARKIT` read-only | M20 |
| Shopkeeper/enemy/militia/IMP/sounds | каталог видит файлы, метаданных нет | M20 |
| Lookup-таблицы | редактируются generic grid | M20 / M26 |
| Локализация | overlays сильнее legacy; настроен только `ru` | M20 |
| Multi-dataset и copy строк | есть только compare | M21 |
| Item images | detail-панель и viewer; нет grid-thumbnail и picker | M19 |
| Settings/layout | частично; нет settings dialog и persistence | M22 |
| Packaging | нет у обоих | M27 |

### M18 — Relation model и main-table pickers — **следующий**
- **Метаданные:** миграция `0007_TableRelations.sql`; таблица `TableRelations` (`SourceTableId`, `SourceField`, `TargetTableId`, `TargetValueField`, `TargetDisplayField`, `Kind` = `Reference`/`SubTable`/`ChildRows`/`Reverse`, `FilterField`/`FilterOperator`/`FilterValue`, `LabelKey`, `RemapByText`, `Ordinal`). Фильтры структурированные (`=`, `<>`, `contains`, bitmask-has), без SQL и произвольных выражений.
- **Импорт связей:** 99 ссылок на `ITEM`, `AMMO`, `AMMOTYPE`, `ITEMTOEXPLOSIVE` из отчёта M14.5 и legacy `DatabaseCode.MakeColumn`, плюс 1:1/1:N связи item-подтаблиц. Каждая ссылка проверяется на v4 и v5. `ITEMTOEXPLOSIVE` заменяется фильтром по `Items`, отдельный файл не создаётся.
- **Backend:** `TableRelation`, `ITableRelationResolver` с кэшем target-таблиц, поиском по display-тексту и ID, remap по имени. Ошибка загрузки target не ломает открытие исходной таблицы.
- **UI:** searchable combo для relation-полей в сетке и панели деталей, переход к target-записи, unknown-ID остаётся raw с предупреждением. Значение пишется в исходное поле как ID.
- **Валидация:** `lookup_missing` для relation-ссылок, без автозаполнения и без создания отсутствующих записей.
- **Тесты:** metadata-провайдер, resolver, фильтры, remap, picker VM, env-gated проверки `Items → Weapons/Ammo/Explosives` на v4/v5.
- **Acceptance:** поля-ссылки показывают имена; выбор пишет ID; неизвестный ID виден как raw; неизменённые файлы не перезаписываются.

### M19 — Item master-detail workspace — **item-workflow parity** (без shopkeeper-вкладки; она в M20)
- **Архитектура:** новый тип вкладки `ItemWorkspace` в главном `TabControl`, открывается двойным щелчком по `Items.Items`. Общий интерфейс `IEditorTab` (`IsDirty`, `Title`, `SaveAsync`, `Close`) реализуют и `TableEditorViewModel`, и item workspace; благодаря этому Save All, dirty-маркер и закрытие вкладки работают единообразно.
- **Backend:** композитный `TableSetWorkspace` агрегирует Items и связанные документы; 1:1 sub-table находятся по `uiIndex`, 1:N child rows — через M18. Единый dirty/save; композитная транзакция undo для операций, затрагивающих несколько файлов; per-file atomic save с отчётом об ошибках.
- **UI:** заголовок записи, группы полей из hints, секции `Weapon`, `Magazine`, `Armour`, `Explosive`, `LBE`, attachments/attachment info, shopkeeper inventory (M20), class-aware видимость. Вложенные `MODIFIERS` и list-поля `DefaultAttachment`/`AvailableAttachmentPoint` становятся редактируемыми (backend `SetNestedValue` для base и `SetListField`).
- **Item-workflow:** new item с классом/именем, duplicate с дочерними записями, relation-aware delete с подтверждением, find by ID, change class без перезаписи `ubClassIndex`, обработка коллизии `uiIndex` со swap.
- **STI:** превью Big/Medium/Small, picker graphic type/index, inline thumbnail в сетке Items, неразрушающий список отсутствующих изображений.
- **Локализация:** существующие оверлеи; переводы пишутся только в overlay, базовая таблица не меняется.
- **Тесты:** композитный save/undo, duplicate/delete, nested/list round-trip, реальные v4/v5 без перезаписи неизменённых файлов.
- **Acceptance:** любое поле предмета и его sub-table редактируется из одной вкладки; Save All сохраняет все связанные документы; после полной отмены составная операция не оставляет частичных изменений.

### M20 — Merc gear, вспомогательные таблицы, language catalog
- **Merc gear:** редактор пяти `GEARKIT` (item pickers, quantity/status, price modifiers, kit name), base + overlay; без нормализации 4-vs-5 kits и без потери полей.
- **Метаданные legacy-таблиц:** 19 shopkeeper inventory и `AdditionalDealer_*` (общая metadata-форма), 14 enemy/militia gun/item choices, 6 enemy ammo/drops, IMP choices, `Sounds`/`BurstSounds`; keys, hints, relations; чистка устаревших mapping-ов. `CONTROL`-часть shopkeeper-файлов сохраняется как есть, отдельного UI нет (в legacy его тоже не было).
- **Item workspace:** обратная связь `Items → shopkeeper inventory` — вкладка с количеством у дилеров.
- **Локализация:** добавить в `Languages` коды `de/pl/fr/it/nl/zh` с overlay-префиксами и автодискавери существующих оверлеев; `Russian.AmmoStrings` и другие оверлеи редактируются через overlay. В v4/v5 реально есть только `ru` — новые файлы не создаются.
- **Тесты:** nested merc gear round-trip, верификация метаданных на v4/v5, вспомогательные таблицы без ложных предупреждений.
- **Acceptance:** merc gear редактируется по всем пяти комплектам; shopkeeper/enemy/militia/IMP/sounds открываются с типами, ключами и item pickers; существующие значения не нормализуются.

### M21 — Multi-dataset и cross-dataset copy/merge
- **Multi-dataset:** список `DataDirectories`, переключение активного датасета без перезапуска, кэш сессий по каталогу, вкладки с ключом `(datasetPath, tableName)`, Save All по всем открытым вкладкам, recent directories в UI.
- **Copy service:** `ITableCopyService` копирует выбранные строки/таблицу из source в target; preview конфликтов; политики skip/overwrite/merge; allocation ключей; remap lookup и relation по display-тексту; опциональное копирование связанных строк Items (sub-tables и shopkeeper inventory).
- **Безопасность:** source всегда read-only; запись только в target после preview; ошибка не оставляет частично записанный файл. Legacy drag-copy заменяется явным диалогом с preview — это безопасный эквивалент, а не потеря функции.
- **Тесты:** remap, конфликты, source не изменён, копирование v4→v5 на temp-копиях.
- **Acceptance:** предмет с sub-tables копируется из v4 в v5, ссылки remap-ятся, target валиден, source не изменён.

### M22 — Grid/workflow parity polish
- **Фильтры:** структурированный конструктор (`=`, `<>`, `contains`, числовые сравнения, AND/OR-группы), фильтр по выбранному значению и исключение. Legacy-выражение DataView заменяется безопасным конструктором без произвольного кода.
- **Выделение и bulk:** multi-select в сетке; bulk применяется к выделенным строкам, при пустом выделении — ко всему текущему виду (как в legacy); пустой Set в диалоге.
- **Persistence:** per `(dataset, table)` — видимость/ширина/порядок колонок, сортировка, фильтр; отдельный versioned `layouts.json` в `%AppData%`, команда сброса.
- **Settings dialog:** data directories, активный датасет, UI-язык en/ru, recent dirs; смена UI-языка применяется после перезапуска, если runtime-переключение окажется слишком инвазивным.
- **Safety:** prompt при закрытии приложения с несохранёнными вкладками; при внешнем изменении файла — reload/discard/overwrite с подтверждением.
- **Тесты:** семантика фильтров, bulk по выделению, settings/layout round-trip.
- **Acceptance:** фильтр и выделение дают предсказуемый набор строк; layout восстанавливается после перезапуска; закрытие приложения не теряет несохранённые данные молча.

### M23 — Аудит и gate паритета
- `LEGACY-PARITY-REPORT.md`: построчная матрица «функция legacy → реализация / superseded / исключено» с файлами-доказательствами.
- Ручная проверка на v4 и v5: item workspace и все sub-tables, merc gear, shopkeeper, pickers, copy, фильтры, изображения, локализация, настройки.
- Исправляются только найденные пробелы; при крупном пробеле маркер паритета сдвигается.
- **Маркер: full functional parity.** Коммит `M23: legacy functional parity gate`.

### После паритета — M24–M27
Паритет не требует этих функций, потому что в legacy их нет; они закрывают собственные пробелы rewrite.

- **M24 — Nested/list/dict editor (4–5 дн):** универсальный редактор `Map.Cities` (`COUNTRY`/`CITYLIST/CITY`), `FacilityTypes/ASSIGNMENT`, root singleton cells и list/dict-полей; nested overlay-переводы. Опирается на base nested write из M19 и gear-редактор M20.
- **M25 — Comparison v2 и dataset-wide validation (3–4 дн):** дифф значений (не только структуры), содержимое оверлеев, root singleton; валидация всех таблиц датасета одним отчётом; глобальный поиск по таблицам.
- **M26 — Lookup management и reference protection (3–4 дн):** CRUD lookup-таблиц, bitmask allocation ID, free-form ID, «где используется», защита от удаления используемых значений, валидация flag-lookup.
- **M27 — Packaging/release (2 дн):** self-contained publish, иконка/версия/changelog, README и user guide, portable zip, CI-гейты сборки/тестов/корпуса.

**Критический путь:** M0 → M1 → M3 → M4 → M5 → M8 → M9 → M10 → M11 → M12 → M13 → M14 → M14.5 → M15 → M16 → M17 → M18 → **M19 (item parity)** → M20 → M21 → M22 → **M23 (full legacy parity)** → M24 → M25 → M26 → M27. Corpus harness запускается на каждом milestone; perf-гейты — на M10, M11, M19–M22 и перед релизом.

---

## Отложено за пределы этого milestone (осознанно)

| Фича | Статус и причина |
|---|---|
| Слияние подтаблиц предметов в ITEM (WEAPON/MAGAZINE/…) | M19 реализует view-проекцию как item workspace; слияние в модель хранения остаётся вне core — риск для целостности |
| Shopkeeper ControlTable/InventoryTable | Inventory — M20 через generic grid с item pickers; `CONTROL` сохраняется как есть, отдельного UI нет (в legacy его тоже не было) |
| Working-data cache | Не вводится: perf-гейты M10/M11/M16 уже закрывают загрузку; кэш — риск устаревших данных при сохранении |
| Copy между датасетами (ComparisonField, lookup remap) | Запланирован в M21 |
| Cities/facilities, value-level compare, lookup management, packaging | Post-parity M24–M27: в legacy этих функций нет |
| Three-way merge, Excel-экспорт сравнения, общий STI/Interface/tileset browser | Backlog после M27; для паритета не требуются |

Включено осознанно: PK max+1, add/duplicate/delete, discovery оверлеев, ColumnMetadata.

---

## Основные риски

- **R1 writer quirks → byte-identity на edited-файлах**: скалярные правки, вставка/дублирование/удаление записи и добавление поля сохраняют байты вне изменённого участка (проверено фикстурами и реальным `Items.xml`). Неоднозначная структурная замена (несколько вставок и удалений в одном родителе) уходит в serializer и нормализует форматирование.
- **R2 большие файлы**: закрыт perf-гейтами M10 (`Items.xml` 75 мс, `Attachments.xml` 27 мс, load+edit+save 380 мс); порог 2 с с запасом.
- **R3 случайная «починка» битого файла**: non-saveable malformed + тесты.
- **R4 dict/list маски**: целевые тесты, правки только по конкретному имени элемента.
- **R5 вложенные ключи оверлеев**: сегментированные `RecordKeySpec` + реальные фикстуры v4.
- **R6 неполные метаданные**: seed покрывает 73 таблицы против 238/239 в v4/v5; document-driven fallback, M20 добавляет legacy-таблицы, миграции растут итеративно.
- **R7 запись в References/ из тестов**: только temp-копии, проверка в done-when M3.
- **R9 внешние изменения**: fingerprint (length, mtime) до записи.
- **R10 культура и отмена**: invariant-числа с запретом запятой/групп, pre-cancelled token во всех I/O-сервисах; проверено тестами M10.
- **R11 relations drift (M18)**: 99 main-table ссылок и связи item-подтаблиц могут дать ложные или устаревшие связи; каждая проверяется на v4/v5, resolver и фильтры покрываются env-gated тестами, неизвестный target не ломает открытие таблицы.
- **R12 перезапись при copy (M21)**: cross-dataset copy может затереть target; source всегда read-only, preview и явная политика skip/overwrite/merge, тесты на temp-копиях.
- **R13 многофайловое сохранение (M19)**: item workspace пишет несколько XML; каждый файл сохраняется атомарно, ошибки возвращаются по файлам, частичное сохранение не маскируется.
- **R14 base nested/list-правки (M19)**: изменения вложенных и повторяющихся элементов идут через lossless-патчер и round-trip тесты; неизменённые файлы не перезаписываются.

---

## Проверка (verification)

1. На каждом milestone: `dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx` — 0 ошибок (на M10 дополнительно `-warnaserror` — 0 предупреждений).
2. `dotnet test` — все юнит-тесты зелёные.
3. M3/M10/M11+: `XER_CORPUS_DIR="…/References/JA 2 1.13 v4/Data-1.13/TableData" dotnet test` (и v5) — corpus harness проходит на копии; в `References/` нет изменений (проверять отсутствием новых `bin/`/`obj/`).
4. M10+: perf-гейты — `XER_CORPUS_DIR="…v5/Data-1.13/TableData" dotnet test --filter "Category=Performance"`; порог 2 с, фактические значения фиксируются в этом файле. M11: workspace открывает `Items.xml` за 86 мс. M16: bulk по ~1776 строкам `Items.xml` — preview 28–40 мс, apply 91–107 мс, undo 34 мс; bulk переводов v4 — preview 39–68 мс, apply 751–790 мс, undo 23–27 мс. M17: сравнение v4↔v5 (477 файлов) — ~1 с.
5. M9/M10/M12/M13/M14/M14.5/M15: вручную — запуск приложения, выбор каталога v5, открытие `Lookup.AttachmentPoint` → диагностика без записи; открытие `Lookup.ItemFlag` → lookup с ID > `int.MaxValue` читается без ошибки; двойной щелчок по `Items.Items` → вкладка с 1775 строками; правка ячейки → сохранение меняет только это поле; add/delete записи → файл остаётся валидным; выбор строки в сетке → панель «Детали» с группами identity/attachments/stance/flags, три превью Big/Medium/Small и кнопка «Биты» только у флаговых полей (значения > `int.MaxValue` открываются корректно); вложенные поля (`MODIFIERS`) read-only; панель валидации на неизменённом `Items.Items` не показывает `invalid_type`/`lookup_missing`, а внесённый вручную `abc` в целочисленное поле даёт предупреждение и не сохраняется. Для v4: переключение языка на Russian показывает merged-значения, непереведённые поля подсвечены, «Переводы» открывает отчёт; правка `szItemName` меняет только `Russian.Items.xml`; для v5 выбор Russian предлагает создать пустой оверлей, после создания и правки файл содержит ключ и только переведённое поле. «Изображения предметов» → предмет 311 (String) или тип 2 / индекс 35 → три панели; отсутствующие размеры показывают код ошибки. В M15: `ubCursor` в `Items.Items` показывает имена из `Lookup.Cursor` и пишет ID; неизвестный ID отображается как `raw (unknown)`; в v5 поля `AttachmentPoint`/`AvailableAttachmentPoint` показывают `⚠` из-за повреждённого lookup-файла и остаются текстовыми; Boolean-поля (`AllowClimbing` и др.) переключаются чекбоксом, пустое поле не превращается в `0` без действия пользователя; кнопка «Биты» у `usItemClass` показывает имена классов (`Gun`, `Knife`, …), а у `usActionItemFlag` — только номера битов. В M16: «Массовая правка…» на отфильтрованном виде показывает preview только изменяемых/ошибочных строк и применяет set/add/multiply одной транзакцией; Ctrl+Z/Ctrl+Y отменяют и повторяют всю операцию, после полной отмены документ снова чистый; bulk перевода в v4 пишет только `Russian.Items.xml`, базовая таблица не меняется. В M17: «Сравнить версии…» → левый путь v4, правый v5 → «Сравнить» показывает 238 общих таблиц, 25 таблиц с overlay `ru`, `ITEMS.Items` 1776→1775, `Lookup.AttachmentPoint` справа «Повреждена», lookup-ссылки `AttachmentPoint` справа «Не читается»; «Экспорт…» сохраняет .md/.csv; после сравнения файлы в `References/` не изменены.
6. M18–M23 (parity gates): M18 — item/ammo/explosive pickers показывают имена, unknown-ID остаётся raw; M19 — item workspace редактирует Items и все связанные sub-tables, duplicate/delete и Save All не оставляют частичных изменений, **item-workflow parity**; M20 — merc gear пять GEARKIT, shopkeeper/enemy/militia/IMP/sounds открываются с метаданными и pickers, ложных предупреждений нет; M21 — copy v4→v5 с remap и неизменённым source; M22 — фильтры, multi-select, layout persistence и settings/exit prompt; M23 — `LEGACY-PARITY-REPORT.md` не содержит открытых пробелов, **full legacy parity**. На M19–M22 также прогоняются v4/v5 corpus и perf-гейты.
7. `find References -type d \( -name bin -o -name obj \)` — пусто.
