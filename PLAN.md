# XML Editor Reloaded — план реализации

## Объём, стек и состояние

**Назначение:** десктопный редактор XML-таблиц (`TableData`) мода JA2 1.13 с сохранением структуры файлов, локализационными оверлеями, изображениями STI и сравнением версий. **Стек:** .NET 10, WPF, SQLite, xUnit, CommunityToolkit.Mvvm + Microsoft.Extensions.DependencyInjection.

**Состояние:** M0–M23b8 и M23a завершены; автоматические гейты M23b пройдены 2026-11-01, маркер full functional parity ставится после подписи ручного smoke. Части A–G ручного smoke автоматизированы (`xml-editor-reloaded/tests/XmlEditorReloaded.UiSmoke`, 48 сценариев: S01–S03, S05–S21, S23–S49 плюс целостность S50; части E–G — S33–S49 с 2026-11-01); вручную остаются S04, S08, S10–S14, S18–S21, S23–S35, S39 и сценарии M23b (S53, S55, S57–S92, включая S71–S75 и S82–S83). Исправления F0–F9 — в `MANUAL-SMOKE-FIX-PLAN.md`, бэклог B1–B2 — ниже.

**Реализовано:** датасеты и schema-grid, lossless round-trip/сохранение, детали и STI, lookup/flag-редакторы, undo/redo, оверлеи, сравнение v4↔v5, relation pickers, item/merc/shopkeeper workspaces, вспомогательные таблицы, multi-dataset copy с remap, settings и безопасное закрытие. Валидация предупреждает о границах legacy-спиннеров, группах флагов и карманах LBE, ничего не обнуляя; lookup-id и маски `Flags` 64-битные; Images показывает кадры типа, границы индекса и live-миниатюры; shopkeeper адресует строки дилера точной записью. Ошибки и preflight пишутся в лог с путями и причиной. Подробности — в коде, тестах и чек-листе.

**Метаданные:** 173 таблицы, 1042 `TableRelations`, 1981 `ColumnHint` (123 с границами min/max, 23 с `ComparisonField`), 46 declaring-колонок (23 lookup-таблицы), 25 правил видимости, 2 `SymmetricKeyPairs`, 1 `ExclusiveFlagGroups` (тип пусковой), 5 `ClassValueLimits` (класс LBE → карманы), 24 фиксированных lookup-значения, 7 `ColumnTextLimits` (лимиты ввода и счётчики символов, с китайским переопределением), 17 миграций, 7 языков, 64-битные lookup ID. Известные дефекты данных: два в v5, дубликат дилера, burst-звуки item 1761 и `mSmall1Status=600` (mIndex 237, kit 5) — единственное ожидаемое предупреждение диапазона.

**Проверка:** build `-warnaserror` (0/0), 600 тестов в базовом прогоне (`test.ps1`), корпусные прогоны v4/v5 (600/600), сравнение v4↔v5 (1/1), copy v4→v5 (1/1) и perf-гейты (9/9 на v4 и v5) пройдены 2026-11-01. UI-smoke: 48 автоматизированных сценариев (включая S50); в песочнице прогоняются 23, остальные требуют интерактивного рабочего стола. Детали завершённых этапов здесь не дублируются: источник истины — код, тесты и git-история.

**Правила:** XML-целостность, `References/`, кодировка и коммиты — в `AGENTS.md`; здесь не дублируются.

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

### M0–M22 — фундамент, grid, workspace и polish — выполнены
Скаффолдинг, Core-контракты, SQLite-метабаза и seed, lossless load/save, редактирование, оверлеи, валидация, STI, DI/shell и RESX; затем discovery-схема и workspace, вкладки и schema-driven grid, панель деталей, языковые оверлеи, verified metadata с 64-битными lookup ID, lookup/Boolean/flag-редакторы, bulk/undo и read-only сравнение с экспортом.
- **M18–M21:** relation-модель и pickers с навигацией; item master-detail workspace (**item-workflow parity**); merc gear, shopkeeper и вспомогательные таблицы (**auxiliary parity**); multi-dataset и copy/merge.
- **M22:** grid/workflow polish (фильтры, multi-select/bulk, layout), settings/safety и compound keys.

### M23a — Аудит паритета — выполнен (кроме ручного smoke)
`LEGACY-PARITY-REPORT.md` содержит полную матрицу legacy-функций и реестр пробелов; секции и протокол — в `M23A-PARITY/` и `M23A-PARITY-AUDIT-PLAN.md`. Поведение редактора не менялось. **Ручной smoke остаётся за пользователем.**

### M23b — Исправления и gate паритета
Исправляются только найденные пробелы, каждый с тестом. Завершены:
- **M23b1:** фильтры item workspace и class-aware видимость полей.
- **M23b2:** item-relation-секции и навигация, list/dict-редакторы, симметричные пары, enemy ammo, диагностика legacy repeated key.
- **M23b3:** источники значений и pickers (звуки, анимация, pocket-поля, robot skill grants, `Nothing`/`None`).
- **M23b4:** валидация legacy-границ чисел, эксклюзивные группы флагов, карта LBE-класс→карманы и 64-битные lookup-id/маски (`L-ITEM-034/047`, `L-MERC-014`, `L-IFIELD-227…230`, `L-DATA-012`).
- **M23b5:** изображения — список кадров текущего типа, динамический максимум индекса из small-листа, границы и загрузка без гонки, инвалидация миниатюр грида/списка (`L-ITEM-014`, `L-IMG-005/020`).
- **M23b6:** shopkeeper — item-центричное создание/удаление строки дилера, exact-row адресация дубликата ключа; lookup — новая строка получает `name=""`, файл остаётся читаемым (`L-ITEM-039`, `L-SHP-001`, `L-AUX-036`).
- **M23b7:** shell — окно About и Help > About, сохранение геометрии окна; field UI — лимиты ввода и счётчики символов; merc gear — имя набора в заголовке и `MaxLength = 80` (`L-SHELL-183`, `L-AUX-018/019`, `L-ITEM-017`, `L-MERC-003…007/013`, `L-SET-009`).
- **M23b8:** хранение — пустой объявленный корень снова редактируется, стартовая схема пустой таблицы из hints; копирование — поиск отсутствующего id по `ComparisonField`; журналирование — файловый лог с заголовком сессии и стеком, глобальные обработчики исключений (`L-STO-034`, `L-OPS-011/025`).

Осталось:
- **Ручной smoke:** 25 сценариев A–G и сценарии M23b S51–S92; после подписи — **маркер full functional parity**, коммит `M23: legacy functional parity gate`.

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
| Item images | превью Big/Medium/Small, picker типа, список кадров типа, границы индекса, live-миниатюры | M19/M23b5 |
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
- **R6 неполные метаданные:** seed покрывает 173 таблицы из 238/239 в v4/v5; document-driven fallback работает, остальные таблицы добавляются по мере необходимости, миграции растут итеративно.
- **R13 многофайловое сохранение:** item workspace пишет несколько XML; per-file atomic save, ошибки агрегируются, частичное сохранение не маскируется.
- **R14 nested/list-правки:** идут через lossless-патчер и round-trip тесты; неизменённые файлы не перезаписываются.
- **Закрыты:** byte-identity и perf (R2), malformed-файлы (R3), dict/list-маски (R4), вложенные ключи оверлеев (R5), запись в `References/` (R7), внешние изменения (R9), культура/отмена (R10), relations drift (R11), copy (R12), compound keys (R15) — покрыты фикстурами, corpus-, compare-, copy- и perf-гейтами.

---

## Проверка

1. `dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx -warnaserror` — 0 ошибок/0 предупреждений
   (в песочнице — `-m:1 --no-restore`, см. `AGENTS.md`).
2. `pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/test.ps1` — сборка одним узлом и
   600 тестов в пяти xUnit-проектах, все зелёные (обход политики PowerShell — там же).
3. Corpus: `XER_CORPUS_DIR="…/v4/Data-1.13/TableData" dotnet test` (и v5) — 600 тестов на каждом
   корпусе, temp-копии без изменений в `References/`; единственное ожидаемое предупреждение
   диапазона — `mSmall1Status=600` (mIndex 237, kit 5) в обеих версиях.
4. Сравнение: `XER_COMPARE_LEFT_DIR=<v4> XER_COMPARE_RIGHT_DIR=<v5>` — `RealDataComparisonTests`.
5. Copy: `XER_COPY_SOURCE_DIR=<v4> XER_COPY_TARGET_DIR=<v5>` — `CorpusCopyTests` (temp-копии, remap, source неизменён).
6. Perf: `XER_CORPUS_DIR=<v5> dotnet test --filter "Category=Performance"` — порог 2 с.
7. **Автоматический UI-smoke:** `out/bin/XmlEditorReloaded.UiSmoke/debug/XmlEditorReloaded.UiSmoke.exe`
   — 48 сценариев частей A–G (S01–S03, S05–S21, S23–S49 + целостность S50); запускать на
   интерактивном рабочем столе (`--only`, `--list`, `--keep`), результат — `report.md` в `.tmp/ui-smoke/<timestamp>/`.
8. **Ручной smoke (выполняет пользователь):** `M23A-PARITY/MANUAL-SMOKE-CHECKLIST.md` — S04, S22,
   S71–S75, S82–S83, ветка ошибки сохранения S13 и сценарии M23b1–M23b8. Результат и подпись вносятся
   в checklist и `LEGACY-PARITY-REPORT.md`.
9. `References/` не изменён; артефакты `bin/`/`obj/` в git не попадают.

---

## Бэклог после smoke S01–S14

Не дефекты, а единообразие редактирования значений; в F-исправления не входят.

- **B1. Выпадающие допустимые значения в фильтрах (S08).** Поле «Значение» в условии фильтра
  должно предлагать тот же набор, что и редактор ячейки: lookup-имена, флаги, enum, Boolean, — с
  сохранением свободного ввода для остальных колонок. Сейчас `ubWeaponType` — число, хотя сетка
  показывает строковое имя и выпадающий список.
- **B2. Выпадающие допустимые значения в массовой правке (S09).** Поле «Значение» в форме
  массовой правки должно предлагать значения выбранной колонки из того же источника, что и
  редактор ячейки; для арифметических операций свободный числовой ввод сохраняется.

