# XML Editor Reloaded — план реализации backend

## Объём milestone

Весь backend (`Core`, `Data`, `Xml`, `Imaging`) до рабочего состояния + минимальная WPF-оболочка (composition root, настройки, выбор каталога данных, список таблиц, заглушка детального вида). Сетки редактирования, формы предметов, локализация UI-интерфейса и image-UI — следующие milestone.

**Решения по стеку (согласовано):** .NET 10; тесты — xUnit; shell — CommunityToolkit.Mvvm + Microsoft.Extensions.DependencyInjection.

**Текущее состояние:** M0–M14 завершены. Backend, WPF-оболочка, grid-редактор и локализация работают: выбор каталога, список таблиц, mapped/unmapped/языки, record count, malformed-диагностика, настройки, RESX-локализация, STI-браузер, discovery-схема, `TableWorkspace`, runtime-сессия датасета, lossless-патчер, вкладки таблиц с виртуализированной сеткой, sparse-правка, add/dup/delete, поиск/сортировка, chooser колонок, сохранение, валидация, панель деталей, переключение языковых оверлеев, маркеры «не переведено», отчёт о переводах и создание оверлея. Дальше — M14.5 (метаданные и валидация), M15 (lookup/bulk).

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

### M14.5 — Metadata seeding: Columns, hints, validation (~1.5–2.5 дн)
Закрывает ограничение M6/M10/M13: таблица `Columns` пуста, поэтому на реальных данных типовая и lookup-валидация фактически не работает, а lookup-редакторы M15 не имеют метаданных. Задача обязательна, но seed не должен быть слепым импортом.

- **Источник:** `References/xml-editor/BackEnd/DatabaseCode.vb` — 710 вызовов `MakeColumn` в ~57 таблицах (`Items` — 257 колонок), плюс `TableMappings` прототипа. Папка `References/` только читается; одноразовый генератор создаёт embedded-миграцию `0005_*.sql`, как это делалось для прототипного seed.
- **Проверка на реальных данных:** каждое поле проверяется по v4 и v5 — значение парсится в заявленный тип, lookup-файл и ID существуют, поле реально встречается в документе. Устаревшие/отсутствующие поля (`AvailableAttachmentPoint0..9`, `BarrelConfiguration`, shopkeeper-таблицы) не попадают в seed автоматически; расхождения оформляются отчётом и курируются вручную.
- **Политика `Columns` vs `ColumnHints`:** `ColumnHints` остаётся основным non-declaring слоем (тип, lookup, editor, группа, ширина) для полей, присутствующих в документе. `Columns` заполняется только там, где объявление отсутствующих полей осознанно (lookup/конфиг-таблицы), либо после добавления явного флага «declaring». `Items.Items` остаётся на hints, чтобы не потерять sparse-семантику.
- **Валидация:** `TableValidator` использует hints для present-полей (тип, диапазон, lookup), unknown-field — только при наличии проверенного набора полей. Маски и `ULong` (`usItemClass` до `1073741824`) обрабатываются как `Flags`/`uint`, без переполнения `int`.
- **Тесты:** загрузка метаданных; `Items.Items` получает типы и lookup для 224 полей; v4/v5 corpus без ложных предупреждений; lookup ID резолвятся; seed не меняет XML-файлы.
- **Результат:** панель валидации осмысленна на реальных данных; M15 получает `LookupTable` для lookup-редакторов.

### M15 — Lookup/flag editors, bulk operations, cross-version compare (~2–3 дн)
Зависит от M14.5 (нужны `LookupTable` и типы полей). Редакторы lookup-таблиц, массовые операции над колонкой, сравнение наборов полей между версиями.

**Критический путь:** M0 → M1 → M3 → M4 → M5 → M8 → M9 → M10 → M11 → M12 → M13 → M14 → M14.5 → M15. Corpus harness запускается на каждом milestone; perf-гейты — на M10, M11 и перед релизом.

---

## Отложено за пределы этого milestone (осознанно)

| Фича | Почему позже |
|---|---|
| Слияние подтаблиц предметов в ITEM (WEAPON/MAGAZURE/…) | Это view-проекция поверх пяти открытых документов, не модель хранения; встроенная в core — риск для целостности |
| Shopkeeper ControlTable/InventoryTable | Узкоспециальные обработчики, ломают metadata-driven подход |
| Working-data cache | Преждевременно до замеров M10; кэш = риск устаревших данных при сохранении |
| Copy между датасетами (ComparisonField, lookup remap) | Workflow над стабильными load/save API; первый кандидат после shell |

Включено осознанно: PK max+1, add/duplicate/delete, discovery оверлеев, ColumnMetadata.

---

## Основные риски

- **R1 writer quirks → byte-identity на edited-файлах**: скалярные правки, вставка/дублирование/удаление записи и добавление поля сохраняют байты вне изменённого участка (проверено фикстурами и реальным `Items.xml`). Неоднозначная структурная замена (несколько вставок и удалений в одном родителе) уходит в serializer и нормализует форматирование.
- **R2 большие файлы**: закрыт perf-гейтами M10 (`Items.xml` 75 мс, `Attachments.xml` 27 мс, load+edit+save 380 мс); порог 2 с с запасом.
- **R3 случайная «починка» битого файла**: non-saveable malformed + тесты.
- **R4 dict/list маски**: целевые тесты, правки только по конкретному имени элемента.
- **R5 вложенные ключи оверлеев**: сегментированные `RecordKeySpec` + реальные фикстуры v4.
- **R6 53/263 таблицы в метаданных**: document-driven fallback, миграции растут итеративно.
- **R7 запись в References/ из тестов**: только temp-копии, проверка в done-when M3.
- **R9 внешние изменения**: fingerprint (length, mtime) до записи.
- **R10 культура и отмена**: invariant-числа с запретом запятой/групп, pre-cancelled token во всех I/O-сервисах; проверено тестами M10.

---

## Проверка (verification)

1. На каждом milestone: `dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx` — 0 ошибок (на M10 дополнительно `-warnaserror` — 0 предупреждений).
2. `dotnet test` — все юнит-тесты зелёные.
3. M3/M10/M11+: `XER_CORPUS_DIR="…/References/JA 2 1.13 v4/Data-1.13/TableData" dotnet test` (и v5) — corpus harness проходит на копии; в `References/` нет изменений (проверять отсутствием новых `bin/`/`obj/`).
4. M10+: perf-гейты — `XER_CORPUS_DIR="…v5/Data-1.13/TableData" dotnet test --filter "Category=Performance"`; порог 2 с, фактические значения фиксируются в этом файле. M11: workspace открывает `Items.xml` за 86 мс.
5. M9/M10/M12/M13/M14: вручную — запуск приложения, выбор каталога v5, открытие `Lookup.AttachmentPoint` → диагностика без записи; двойной щелчок по `Items.Items` → вкладка с 1775 строками; правка ячейки → сохранение меняет только это поле; add/delete записи → файл остаётся валидным; выбор строки в сетке → панель «Детали» с группами identity/attachments/stance/flags, три превью Big/Medium/Small и кнопка «Биты» только у флаговых полей (значения > `int.MaxValue` открываются корректно); вложенные поля (`MODIFIERS`) read-only. Для v4: переключение языка на Russian показывает merged-значения, непереведённые поля подсвечены, «Переводы» открывает отчёт; правка `szItemName` меняет только `Russian.Items.xml`; для v5 выбор Russian предлагает создать пустой оверлей, после создания и правки файл содержит ключ и только переведённое поле. «Изображения предметов» → предмет 311 (String) или тип 2 / индекс 35 → три панели; отсутствующие размеры показывают код ошибки.
6. `find References -type d \( -name bin -o -name obj \)` — пусто.
