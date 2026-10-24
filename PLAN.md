# XML Editor Reloaded — план реализации backend

## Объём milestone

Весь backend (`Core`, `Data`, `Xml`, `Imaging`) до рабочего состояния + минимальная WPF-оболочка (composition root, настройки, выбор каталога данных, список таблиц, заглушка детального вида). Сетки редактирования, формы предметов, локализация UI-интерфейса и image-UI — следующие milestone.

**Решения по стеку (согласовано):** .NET 10; тесты — xUnit; shell — CommunityToolkit.Mvvm + Microsoft.Extensions.DependencyInjection.

**Текущее состояние:** M0/M1/M2/M3/M4/M5/M6/M7/M8/M9 завершены. Backend и минимальная WPF-оболочка работают: выбор каталога, список таблиц, mapped/unmapped/языки, record count, malformed-диагностика, настройки и RESX-локализация. Редакторы сеток/форм, image UI и R1 structural patcher остаются следующими задачами.

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

### M10 — Hardening (~1–2 дн)
Perf-тесты (env-gated: `Items.xml` < 2 с, `Attachments.xml` 6k строк), cancellation-тесты, аудит XML-доков (CS1591), обновление `PROJECT-STRUCTURE.md` и `README.md`, финальные `dotnet build` + `dotnet test` + оба корпусных прогона.

**Критический путь:** M0 → M1 → M3 → M4 → M5 → M8 → M9 → M10. Corpus harness запустить в конце первого дня M3 — риск round-trip надо снять до всего, что строится на `TableDocument`. Итого ≈ 17–24 implementer-дней.

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

- **R1 writer quirks → byte-identity на edited-файлах**: для скалярных правок закрыт через `LosslessDocumentPatcher`; тест `quirky.xml` подтверждает сохранение declaration casing, атрибутов, `&gt;`, `&#10;` и `<Empty/>`. Для структурных изменений (add/delete/duplicate) пока используется обычный serializer — форматирование нормализуется; lossless-патчер структурных операций остаётся follow-up.
- **R2 большие файлы**: lazy-парсинг значений, key-index, async; perf-гейт M10.
- **R3 случайная «починка» битого файла**: non-saveable malformed + тесты.
- **R4 dict/list маски**: целевые тесты, правки только по конкретному имени элемента.
- **R5 вложенные ключи оверлеев**: сегментированные `RecordKeySpec` + реальные фикстуры v4.
- **R6 53/263 таблицы в метаданных**: document-driven fallback, миграции растут итеративно.
- **R7 запись в References/ из тестов**: только temp-копии, проверка в done-when M3.
- **R9 внешние изменения**: fingerprint (length, mtime) до записи.

---

## Проверка (verification)

1. На каждом milestone: `dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx` — 0 ошибок.
2. `dotnet test` — все юнит-тесты зелёные.
3. M3/M10: `XER_CORPUS_DIR="…/References/JA 2 1.13 v4/Data-1.13/TableData" dotnet test` (и v5) — corpus harness проходит на копии; в `References/` нет изменений (проверка git-статуса невозможно — папка в .gitignore; проверять отсутствием новых `bin/`/`obj/`).
4. M9: вручную — запуск приложения, выбор каталога v5, открытие `Lookup.AttachmentPoint` → диагностика без записи; открытие `Items.Items` → 1775 записей; правка поля в тестовой копии → сохранение меняет только это поле.
5. `Get-ChildItem References -Recurse -Include bin,obj` — пусто.
