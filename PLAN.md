# XML Editor Reloaded — план реализации backend

## Объём milestone

Весь backend (`Core`, `Data`, `Xml`, `Imaging`) до рабочего состояния + минимальная WPF-оболочка (composition root, настройки, выбор каталога данных, список таблиц, заглушка детального вида). Сетки редактирования, формы предметов, локализация UI-интерфейса и image-UI — следующие milestone.

**Решения по стеку (согласовано):** .NET 10; тесты — xUnit; shell — CommunityToolkit.Mvvm + Microsoft.Extensions.DependencyInjection.

**Текущее состояние:** M0 и M1 завершены (коммит `27bcafe`), M3 реализован в объёме core: loader, document model, save gate, lookup repository, фикстуры и corpus no-rewrite тесты зелёные на v4/v5. `Data` и `Imaging` пока пустые class library, `App` — шаблонная WPF-заглушка. R1 (побайтовая идентичность отредактированных файлов) остаётся открытым и требует `ValueSpanPatcher`; следующий milestone — M4 (редактирование строк), R1 можно закрывать параллельно.

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

### M2 — Data (~2–3 дн; параллельно с M7)
- DDL: `SchemaMigrations`, `Tables` (superset `TableMappings`), `TableKeys`, `Columns`, `ListFields`, `DictFields`, `Languages`, `TranslatableFields`. Embedded-миграции `Metadata/Migrations/000N_*.sql` + `DatabaseMigrator` (транзакция, downgrade = error).
- Seed: генерация `Seed/0001_PrototypeSeed.sql` из прототипной БД (одноразовый dev-шаг, результат коммитится) + вручную: ключи оверлеев (`uiIndex/mIndex/pIndex/ubIndex/uiSlotIndex/SectorGrid`, nested/positional), `ru`→`Russian`, переводимые пути 25 оверлеев.
- `SqliteMetadataProvider`, `FileSystemTableCatalog` (обход `TableData/**/*.xml`, исключение оверлеев по префиксу из `Languages`), `TranslationCatalog`, `JsonSettingsStore` (`%AppData%/XmlEditorReloaded/settings.json`, atomic write).
**Готово:** свежая БД мигрирует, ≥53 таблицы, каталог на temp-фикстуре различает базу и оверлеи; `PROJECT-STRUCTURE.md` дополнен `Data.Tests`.

### M7 — Imaging (~2–3 дн; параллельно с M2)
`StciHeader`/`StciFlags`/`SubImageDescriptor`/`StciImage`/`StciParser` (ошибки `sti.bad_magic/truncated/unsupported_flags`); `EtrleDecoder` (control-byte семантика, overrun = diagnostic); `ItemImageCatalog` (Big: `BigItems/gun{NN}.sti`/`p{t}item{NN}.sti`, паддинг до 2 цифр только для <10, case-insensitive; Medium: `Interface/mdguns.sti`/`mdp{t}items.sti` frame=index; Small: `tilesets/0/smguns.sti`/`smp{t}items.sti`; прозрачность: индекс 0 везде, зелёный 0,255,0 — medium/small); `DecodedFrameCache` (LRU ~64–128 МБ, invalidation по mtime). Фикстуры: 2–3 маленьких реальных STI, скопированных в тест-проект.
**Готово:** все edge-кейсы покрыты, zero WPF-ссылок.

### M3 — Xml: загрузка, round-trip, диагностика (~4–6 дн; самое сложное) — **выполнен (core), R1 открыт**
`FileFormatInfo`, `TableFileLoader` (`XmlReaderSettings` + `PreserveWhitespace`, `XmlException` → `xml.parse_error` + line/position), `DocumentSerializer`, `TableFileSaver`, `TableDocument`, `TableRecord`, `LexicalValue`, `TableLoadResult` (`Loaded|LoadedReadOnly|Malformed`), `XmlFileLookupRepository`.
**Алгоритм сохранения:** dirty? → serialize → re-parse verify → сравнить с оригиналом → если идентично, не писать → атомарная запись → проверка (length, mtime).
**Тесты:** байтовая идентичность неотредактированной фикстуры; «не перезаписан вообще» (mtime); sparse-набор полей; `1.0`/пустые/повторяющиеся/неизвестные поля; порядок и регистр; v5 AttachmentPoint → ошибка без записи; `EditSingleValue_Save_ChangesOnlyThatValue`; BOM/newline; внешняя модификация. **Corpus harness** (`XER_CORPUS_DIR`, копия во temp): v4 — 263 файла, v5 — 239 файлов + 1 malformed; проверяется отсутствие перезаписи неизменённых файлов.
**Фактический результат:** загрузка, диагностика, редактирование одного значения, save-gate и corpus no-rewrite зелёные на v4 и v5. Побайтовая идентичность после редактирования не достигнута: стандартный `XmlWriter` нормализует регистр encoding в declaration, пробелы вокруг `=`, `&gt;`, `&#10;`, пробел в self-closing тегах и форматирование атрибутов. Это R1; `ValueSpanPatcher` остаётся отдельным follow-up. `PrimaryKeyAllocator` реализован как `TableDocument.GetNextPrimaryKey`; `RowFactory` перенесён в M4.
**Готово (фактически):** corpus no-rewrite; повреждённый файл недеструктивен; load/edit/save сохраняет поля и значения, но может нормализовать форматирование отредактированного файла.

### M4 — Xml: редактирование и строки (~1–1.5 дн)
`SetFieldValue` (меняет только текст; отсутствующее поле не добавляется без явного `addFieldIfMissing`), `SetListField`, `SetDictField` (маска+индекс), `RecordOps` (Add max+1 / Duplicate deep-clone / Delete), `DocumentChangeTracker` (журнал для будущего undo), отклонение невалидных чисел (`xml.invalid_number`) без мутации документа.
**Готово:** тесты add/dup/delete/edit зелёные, round-trip фикстур не сломан.

### M5 — Xml: оверлеи (~2–3 дн)
`OverlayDocument`, `OverlayKeyResolver` (nested/positional, `UnresolvableKey` = diagnostic), `OverlayMerger` (base+overlay → merged cells с признаком перевода), `OverlayWriter` (запись только в оверлей; новая запись = ключ + правленое поле), `TableEditSession` (роутинг: переводимое+язык → оверлей, иначе base; независимые save), `TranslationAnalyzer` (missing/orphan/duplicate/empty/identical/KeyCountMismatch).
**Готово:** цикл merge→edit→save на реальных фикстурах (Cities, PocketPopups, MercStartingGear, Items-оверлей); base-файл побайтово нетронут.

### M6 — Xml: валидация (~1 дн; параллельно с M5)
`TableValidator`: unknown-field = info (не error), bad type = warning с ключом+путём, lookup-ref, дубликаты ключей, форма dict-масок. Только чтение.

### M8 — Wiring (~1–1.5 дн)
`TableRepository` (реализация `ITableRepository`), `ServiceCollectionExtensions` в каждом backend-проекте; join правило: нет метаданных → document-driven, статус «unmapped». Интеграционные тесты (end-to-end load-edit-save, malformed fail-soft, lookup overlay, translation cycle через `TableEditSession`).

### M9 — Минимальный WPF shell (~1–1.5 дн)
`App.xaml.cs` — composition root (`AddXmlEditorData/Xml/Imaging`); `IDialogService` (folder picker); `AppSettingsViewModel`; `MainViewModel` + список таблиц из `ITableCatalog` (имя, оверлей-языки, mapped/unmapped); выбор таблицы → record count или malformed-диагностика (async + CTS). Строки UI — сразу в RESX (`Strings.resx` + `Strings.ru.resx`).
**Готово:** приложение запускается, выбирает `Data-1.13`, список таблиц реальный, детали/диагностика отображаются; backend без WPF.

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

- **R1 writer quirks → byte-identity на edited-файлах**: подтверждено на корпусе. `XmlWriter` меняет declaration encoding case, пробелы в атрибутах, `&gt;`, `&#10;`, self-closing `/>`; 15 из 263 файлов v4 расходятся уже без правок. Неизменённые файлы не перезаписываются, поэтому это не риск потери данных. Для byte-identity edited-файлов нужен `ValueSpanPatcher` (патчинг исходных байтовых диапазонов) — отдельный follow-up до M4/M10.
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
