# XML Editor Reloaded — план реализации backend

## Объём, стек и состояние

**Назначение:** десктопный редактор XML-таблиц (`TableData`) мода JA2 1.13 с сохранением структуры файлов, локализационными оверлеями, изображениями STI и сравнением версий. **Стек:** .NET 10, WPF, SQLite, xUnit, CommunityToolkit.Mvvm + Microsoft.Extensions.DependencyInjection.

**Состояние:** M0–M21 завершены. Работают: выбор датасета и список таблиц, lossless round-trip и атомарное сохранение, схема из документа + hints, grid-редактор (sparse-правка, поиск/сортировка, chooser колонок, bulk), панель деталей, STI-превью и браузер, lookup/Boolean/flag-редакторы, undo/redo, языковые оверлеи (merged-значения, маркеры, отчёт, создание), read-only сравнение v4↔v5 с экспортом, relation-модель с pickers и навигацией, item workspace для `Items.Items` со всеми 1:1/1:N таблицами и единым undo/save, merc gear workspace, обратная вкладка shopkeeper inventory и 104 вспомогательные таблицы, multi-dataset с селектором каталогов и кэшем сессий, cross-dataset copy с preview, политиками конфликтов, allocation ключей, remap по display-тексту и копированием связанных строк. Метаданные: 173 таблицы, 176 key segments, 1923 `ColumnHints` (224 у `Items.Items`), 46 declaring-колонок (23 lookup-таблицы), 11 flag-lookup-связей, 1020 `TableRelations` (924 `Reference`, 5 `SubTable`, 7 `ChildRows`, 84 `Reverse`), 7 языков (`ru`, `de`, `pl`, `fr`, `it`, `nl`, `zh`), 64-битные lookup ID. **M22 разбит на подшаги:** M22a — фильтры, multi-select/bulk и per-table layout — выполнен; M22b — settings dialog, close safety и external-change recovery — текущий подшаг. M23 также разбит: M23a — аудит паритета, M23b — исправления и gate. **Паритет:** item-workflow parity достигнут в M19; merc gear, shopkeeper/enemy/militia/IMP/sounds и language catalog — M20; multi-dataset/copy — M21; full legacy parity — M23b. **Проверка текущего состояния:** build `-warnaserror` (0/0), 370 тестов (Core 28, Imaging 19, Xml 155, Data 60, App 108), corpus v4/v5, сравнение v4↔v5, copy v4→v5 и perf-гейты зелёные.

**Правила (из AGENTS.md):**
- XML-целостность: не терять поля (включая неизвестные), не нормализовать sparse-записи, сохранять лексическую форму (`1.0`), порядок, регистр, декларацию. Неизменённый файл не перезаписывается вообще; повреждённый — только диагностика.
- Метаданные и документ — единственные источники; никакого хардкода полей v4/v5.
- `References/` только чтение; тесты работают на копиях; проекты из `References/` не собираются.
- UTF-8 без BOM, английская XML-документация публичных API, async + `CancellationToken`.
- Локальный коммит после milestone; push только по просьбе; обновлять этот файл, а `PROJECT-STRUCTURE.md` — только при изменении структуры решения.

---

## Ключевые design-решения

1. **Round-trip:** `XDocument` + `PreserveWhitespace` + исходный `FileFormatInfo` (кодировка, newline) + `IsDirty`-гейт. Сохранение — `LosslessDocumentPatcher` по исходным байтам (скалярные правки, вставки, удаления имитируют отступы соседей); перед записью output перепарсивается в памяти. Неоднозначная структурная замена уходит в `DocumentSerializer` (данные корректны, форматирование нормализуется).
2. **Sparse-запись = реальный `XElement`;** `TableRecord` — фасад, `LexicalValue` хранит raw-текст. Никаких DTO-строк в хранилище; числа только invariant culture.
3. **Save-gate:** мутации только через `TableDocument`/`OverlayDocument`; чистый или побайтно идентичный документ не пишется; запись атомарная (tmp + replace); внешние изменения проверяются по (length, mtime).
4. **Metadata-driven + document-driven:** SQLite (таблицы, ключи, колонки, list/dict, языки, переводимые пути) + вывод из документа для немапленных таблиц и неизвестных полей. `Columns` — declaring, `ColumnHints` — non-declaring (только уточняют присутствующие поля).
5. **Оверлеи:** ключи `RecordKeySpec` (scalar/nested/positional); merge через `OverlayMerger`; правка перевода пишется только в оверлей (`OverlayWriter`), базовая таблица не меняется; новый оверлей содержит только корень, записи добавляются лениво.
6. **Повреждённые файлы** (v5 `Lookup/AttachmentPoint.xml`): `Malformed`, несохраняемый, `xml.parse_error` только в диагностике.
7. **Разделение проектов:** XML-парсинг и lookups — `Xml`; SQLite, каталог, настройки — `Data`; контракты — `Core`; WPF — `App`. Backend не ссылается на WPF.
8. **Метабаза:** линейные embedded SQL-миграции (`SchemaMigrations`, downgrade = error), seed из прототипа + вручную проверенные ключи/языки/пути/hints/relations.
9. **Фикстуры:** маленькие реальные сэмплы в тест-проектах; полный корпус — env-gated `XER_CORPUS_DIR` (копия во temp перед любой записью).
10. **STI:** 64-байтный little-endian заголовок, `appDataSize@48`, палитра RGB×3, 16-байтная фрейм-директория; ETRLE: `0` = конец строки, `>127` = transparent run, `1..127` = literal; индекс 0 прозрачен, green-key только medium/small; на выходе RGBA без `System.Drawing`.

---

## Milestones

### M0–M10 — фундамент — выполнены
Scaffolding решения и тестов; Core-контракты (`OperationResult`/`ErrorCodes`, метаданные, ключи, пути, абстракции); SQLite-метабаза, seed, каталог, lookup/translation-каталог, настройки; Xml: загрузка/диагностика, save-gate, lossless-патчер, редактирование полей/списков/словарей, add/duplicate/delete, оверлеи и валидация; Imaging: STI-парсер, ETRLE, каталог big/medium/small, кэш; DI-wiring и document-driven fallback; WPF shell со списком таблиц, настройками, RESX en/ru; hardening: cancellation, invariant-числа, generation-защита от гонок, perf-гейты и corpus harness. После M10 дополнительно появился STI-браузер.

**Corpus-факты:** v4 — 238 базовых таблиц и 25 оверлеев; v5 — 239 базовых, 0 оверлеев, 1 malformed lookup. **Perf-гейты** (порог 2 с): `Items.xml` 75 мс, `Attachments.xml` 27 мс, load+edit+save 380 мс, каталог 239 таблиц/30 мс, метаданные 73/32 мс; bulk по ~1776 строкам: preview 28–40 мс, apply 91–107 мс, undo 34 мс; сравнение v4↔v5 ~1 с.

### M11–M17 — редактор, локализация, метаданные, bulk, сравнение — выполнены
- **M11:** discovery-схема (`TableSchemaBuilder`: union полей, типы, presence/max occurrences, key/list/dict/translatable, `Columns` overrides), `TableWorkspace` + `WorkspaceRow`, runtime `IDataSetSession`, lossless структурный патчер (add/delete/duplicate байтовым splice).
- **M12:** вкладки таблиц, виртуализированная schema-driven сетка, sparse-правка с `addIfMissing`, add/duplicate/delete, поиск/сортировка, chooser колонок, валидация, Save All.
- **M13:** панель деталей с группами/фильтром, непродекларирующие `ColumnHints`, три STI-превью, 32-битный битовый редактор.
- **M14:** переключатель языка, merged-значения и маркеры «не переведено», отчёт о переводах, создание оверлея, сохранение языка в настройках.
- **M14.5 — метаданные:** сверка legacy-схемы с v4 и v5. `ColumnHints` — основной слой (502 hints, `Items.Items` остаётся hints-only); `Columns` только для 23 lookup-таблиц (46 declaring-колонок); `LookupEntry.Id` расширен до `long`; hints используются валидатором и backend-правкой; реальные данные v4/v5 без ложных `invalid_type`/`lookup_missing`. Отчёт — `M14.5-METADATA-REPORT.md`.
- **M15:** lookup-комбо, Boolean-чекбоксы с sparse-семантикой, имена битов из lookup-масок, текстовый fallback при повреждённом lookup; связывание 11 flag-полей с lookup-файлами.
- **M16:** bulk set/add/multiply с preview и одной транзакцией, `TableEditHistory` (50 транзакций / 10 000 снимков) для ячеек, строк, bulk и переводов; undo/redo с пересчётом `IsDirty`.
- **M17:** read-only сравнение двух `TableData`: presence/counts/fields/types/cardinality/sparse profile/lookup-доступность, Markdown/CSV export, окно с двумя путями и диагностикой.

### M18 — Relation model и main-table pickers — выполнен
- **Метаданные:** `TableRelations` (`Reference`/`SubTable`/`ChildRows`/`Reverse`, value/display-поля, структурированный фильтр `Equals`/`NotEquals`/`Contains`/`HasFlag`, `RemapByText`, `Ordinal`); 84 проверенные связи. `ITEMTOEXPLOSIVE` выражен как `HasFlag 768` по `Items`. Связи для shopkeeper/enemy/militia/IMP/sounds добавлены в M20 вместе с их таблицами (FK).
- **Backend/UI:** `TableRelationResolver` с кэшем target по штампу файла (length+mtime), оверлеями display-полей, поиском по имени/ID и remap; searchable `RelationPicker` в сетке и деталях, ручной ввод неизвестного ID, кнопка «→» и кросс-табличная навигация; `validation.lookup_missing` по полному набору ключей target.
- **Ограничение, снятое в M19:** `DefaultAttachment` был read-only как list field. Четыре placeholder-предмета без `ubClassIndex` не имеют строки подтаблицы — это ожидаемо.

### M19 — Item master-detail workspace — выполнен (item-workflow parity)
- **Вкладки:** `IEditorTab` (`TableName`, `Title`, `DisplayName`, `IsDirty`, `IsBusy`, `SaveAsync`, `SelectRecord`, события языка и навигации) реализуют `TableEditorViewModel` и `ItemWorkspaceViewModel`; `MainViewModel` хранит `IEditorTab`, двойной щелчок по `Items.Items` открывает item workspace, переход по relation на Items выбирает предмет. Save All, dirty-маркер и закрытие работают единообразно.
- **Backend:** `TableSetWorkspace` загружает Items и все `SubTable`/`ChildRows`-цели с одним общим `TableEditHistory`; отсутствующий target — warning, не ошибка. `DuplicateRecord`, `DeleteRecord`, `RenameRecord`, `SwapRecords`, `EnsureSubTableRecord` — многодокументные транзакции; `DescribeDependents` даёт состав удаления; `SaveAsync` пишет только dirty-файлы атомарно и агрегирует ошибки. Nested-пути создают отсутствующие промежуточные узлы; `SetListValues` заменяет repeated-поля. Класс-ключевые 1:1-строки (`ubClassIndex`) общие и не копируются/не удаляются с предметом; `Weapons` (ключ = item `uiIndex`) и child-строки копируются и удаляются. Rename remap-ит ссылки, конфликт 1:1 проверяется до мутации; swap меняет ID и зависимые ссылки.
- **Workspace:** список предметов с ленивыми миниатюрами и поиском; редактируемый ID; группы полей из hints с фильтром; class-aware секции Weapon/Magazine/Armour/Explosive/LBE с созданием отсутствующей строки; 1:N таблицы (Attachments, AttachmentInfo, IncompatibleAttachments, Launchables, CompatibleFaceItems, Item_Transformations, AttachmentComboMerges) как встроенные фильтрованные гриды с pickers; nested STAND/CROUCH/PRONE; список `DefaultAttachment`; превью Big/Medium/Small и picker типа графики; валидация всего набора; языковой селектор (существующие оверлеи и создание нового). Новый предмет — диалог ID/имя/класс.
- **Переиспользование:** `FieldEditor` вынесен из detail-панели; `FieldSourceSet` централизует lookup/relation-загрузку; `DetailFieldViewModel` принимает `IDetailFieldHost` и поддерживает nested `FieldPath`.
- **Ограничения:** placeholder-предметы без `ubClassIndex` показывают «no class index» и не создают фиктивных строк; `AvailableAttachmentPoint` остаётся одиночным Flags-полем (объявлен list, но max occurrences = 1); `DefaultAttachment` редактируется как список; nested-поля в generic detail-панели read-only (редактируются в item workspace); embedded child-гриды без панели деталей, Ctrl+S сохраняет весь набор; семь child-таблиц инициализируются при открытии workspace.

### M20 — Merc gear, вспомогательные таблицы, language catalog — выполнен
- **Merc gear:** `MercGearWorkspaceViewModel`/`MercGearWorkspaceView` открываются вместо generic grid для `Inventory.MercStartingGear`: список наёмников с поиском, пять `GEARKIT` как вкладки, item pickers с фильтрами класса, status/quantity/drop, `mPriceMod`/`mAbsolutePrice`, kit name пишется в overlay при выбранном языке. Правки адресуют occurrence `GEARKIT` через `TableRecord`/`TableWorkspace`, делят один undo/save и не нормализуют sparse-поля.
- **Метаданные (миграция 0008):** 103 новые вспомогательные таблицы (20 choice/drop-файлов, 80 dealer inventory, `NPCInventory.Merchants`, `Sounds`/`BurstSounds`) плюс существующая `Inventory.MercStartingGear`; scalar/positional keys, hints, 936 новых relations (item pickers, drop lookups, 80 dealer references, 84 reverse) и 6 языковых кодов. Удалены 3 prototype-only mapping-а без файлов в v4/v5. `CONTROL`-часть shopkeeper-файлов сохраняется как есть, отдельного UI нет.
- **Item workspace:** reverse-связи загружаются как optional editable members; вкладка «Торговцы» показывает dealer rows выбранного предмета и правит `ubOptimalNumber`; rename/delete предмета обновляет/удаляет stock rows; отсутствующие reverse-файлы не порождают warnings.
- **Sounds:** text-only записи через виртуальное поле `Text` и positional keys, без добавления дочерних элементов.
- **Ограничения:** при редактировании отсутствующего у overlay комплекта positional key дополняется пустыми `GEARKIT`; duplicate `uiIndex` в `AdditionalDealer_43_Inventory` репортится, но не переписывается.
- **Тесты/acceptance:** все пять комплектов и overlay kit names на фикстурах и реальных v4/v5; 104 вспомогательные таблицы открываются с типами, ключами и без ложных предупреждений; item set с 80 dealer inventory грузится и сохраняет только изменённый dealer-файл; sounds round-trip.

### M21 — Multi-dataset и cross-dataset copy/merge — выполнен
- **Multi-dataset:** `DataDirectories` в шелле с recent-first порядком, переключение активного датасета без перезапуска и без закрытия вкладок; `DataSetSessionFactory` кэширует сессии по каталогу; вкладки идентифицируются парой `(TableDataDirectory, TableName)`, несут `DatasetLabel` и открываются/переиспользуются в своём датасете; навигация по relation остаётся внутри датасета; Save All охватывает все вкладки; активный каталог и recent сохраняются в настройках.
- **Backend copy (`Xml/Copy`):** `ITableCopyService` (`PreviewAsync`/`ApplyAsync`) переносит записи как XML-элементы, поэтому unknown-поля, repeated-элементы, sparse-форма и лексические значения сохраняются. Preview показывает действие по каждой записи, allocation ключей, конфликты и связанные строки без записи. Политики `Skip`/`Overwrite`/`Merge`; `Preserve`/`Allocate` ключей; `RemapByText`-relations и lookup-колонки переписываются по display-тексту; `SubTable`/`ChildRows` копируются опционально, parent-link обновляется на target-ключ.
- **Безопасность:** source только читается; target планируется в памяти и пишется после preview; многофайловая запись атомарна по файлу, при ошибке уже заменённые файлы восстанавливаются из исходных байт; пустой target-файл без записей открывается для вставки только при единственной диагностике `xml.record_element_missing`; одинаковые source/target отклоняются (`copy.same_directory`).
- **UI:** кнопка «Копировать данные…» открывает явный диалог: source/target (с browse и нормализацией до `TableData`), пересечение таблиц, фильтр ключей (пусто = все), политика, режим ключей, related rows, предпросмотр с remap/related/warnings; apply возвращает результат, после чего чистые вкладки target перезагружаются, а dirty-вкладки помечаются как не перезагруженные.
- **Тесты/acceptance:** unit-тесты копирования (skip/overwrite/merge, allocation, sub-tables, missing target, read-only source), тесты диалога и multi-dataset во вкладках, env-gated corpus copy v4→v5 на temp-копиях: предмет с Weapon и Attachments копируется с новыми ключами, `ubCalibre` remap-ится по `AmmoStrings`, source побайтно неизменён, target читается.

### M22a — Grid: фильтры, multi-select/bulk, layout — выполнен
- **Фильтры:** модель `RowFilterOperator`/`RowFilterCriterion`/`RowFilterGroup` в Core, вычислитель `TableRowFilterMatcher` в Xml, UI-конструктор (`=`, `<>`, `contains`, числовые сравнения, AND/OR), фильтр по выбранной ячейке и исключение; без произвольного кода. Отсутствующее поле не проходит `=`, но проходит `<>`; числа — invariant culture; пустой операнд не применяется, пока пользователь не введёт значение.
- **Выделение и bulk:** `SelectionMode=Extended`, bulk по выделению, при пустом — по всему текущему виду (как в legacy); пустой Set через явный чекбокс «Очистить значение» (Add/Multiply по-прежнему требуют число); undo одной транзакцией.
- **Persistence:** per `(dataset, table)` видимость/ширина/порядок колонок, сортировка, фильтр; versioned `layouts.json` в `%AppData%`, tolerant load, backup `.bad` и сброс layout. Персистятся только standalone generic-вкладки.
- **Ограничения:** фильтр сравнивает эффективные raw-значения (display-текст lookup/relation не фильтруется); embedded-гриды и item/merc workspaces не персистят layout; delete/duplicate строк остаются однострочными.
- **Тесты:** matcher (типы, missing, AND/OR, культура), VM-фильтры, bulk по выделению и пустой Set, layout round-trip/restore/reset, XAML smoke; 370 тестов, corpus v4/v5 и perf-гейт фильтра `Items` зелёные.
- **Коммит:** `M22a: grid filters, multi-select, layout persistence`.

### M22b — Shell: настройки и безопасность
- **Settings dialog:** каталоги данных (добавить/удалить/активный), recent dirs, UI-язык en/ru с пометкой «после перезапуска», сброс всех layout; сохранение через `ISettingsStore`, применение активного датасета без закрытия вкладок.
- **Safety:** закрытие вкладки и приложения с несохранёнными изменениями — Save/Discard/Cancel; Save-ошибка не закрывает вкладку; перед выходом flush layout.
- **External change:** при `xml.file_changed_externally` — Reload (отбросить правки и перечитать) / Overwrite (записать поверх через `forceOverwrite`) / Cancel; обычный save-gate не ослабляется, multi-file ошибки агрегируются.
- **Тесты/acceptance:** settings round-trip, close-сценарии, force-overwrite и rebase baseline, внешние изменения для grid/item/merc; закрытие не теряет данные молча.
- **Коммит:** `M22b: settings, close safety, external-change recovery`.

### M23a — Аудит паритета
- `LEGACY-PARITY-REPORT.md`: построчная матрица «функция legacy → реализация / superseded / исключено» с файлами-доказательствами.
- Ручная проверка на v4 и v5: item workspace и sub-tables, merc gear, shopkeeper, pickers, copy, фильтры, изображения, локализация, настройки.
- Только документ и список пробелов, без изменения поведения. **Коммит:** `M23a: legacy parity audit`.

### M23b — Исправления и gate паритета
- Исправляются только найденные пробелы, каждый с тестом; крупный пробел фиксируется в отчёте и выносится в отдельный подшаг после согласования.
- Повторный полный прогон и обновление матрицы. **Маркер: full functional parity**, коммит `M23: legacy functional parity gate`.

### После паритета — M24–M27
Эти функции не нужны для паритета (в legacy их нет), они закрывают собственные пробелы rewrite.

- **M24a — Nested city/facility editors:** `Map.Cities` (`COUNTRY`/`CITYLIST/CITY`) и `FacilityTypes/ASSIGNMENT`. **M24b — Root/list/dict:** root singleton cells, list/dict-поля, nested overlay-переводы.
- **M25a — Comparison v2:** дифф значений, содержимое оверлеев, root singleton. **M25b — Dataset-wide:** валидация всего датасета одним отчётом, глобальный поиск.
- **M26a — Lookup CRUD:** создание/изменение/удаление lookup-таблиц, bitmask allocation ID, free-form ID. **M26b — Protection:** «где используется», защита используемых значений, валидация flag-lookup.
- **M27a — Packaging:** self-contained publish, portable zip, CI-гейты сборки/тестов/корпуса. **M27b — Release:** иконка/версия/changelog, README и user guide.

**Критический путь:** M0 → M1 → M3 → M4 → M5 → M8 → M9 → M10 → M11 → M12 → M13 → M14 → M14.5 → M15 → M16 → M17 → M18 → **M19 (item parity)** → M20 → M21 → M22a → M22b → M23a → **M23b (full legacy parity)** → M24a → M24b → M25a → M25b → M26a → M26b → M27a → M27b. Corpus-гейты — на каждом milestone; perf-гейты — на M10, M11, M19–M22 и перед релизом.

---

## Функциональный паритет с legacy-редактором

**Эталон:** `References/xml-editor` (VB.NET/WinForms, 123 `DataTable`, 97 `DataRelation`). `References/JA_2_1.13_TableData_Editor` — незавершённый прототип, эталоном не считается.

**Определение.** Каждая реально доступная функция legacy либо реализована, либо заменена более безопасным эквивалентом; bug-for-bug совместимость не требуется. Критерий — фактическое поведение кода, а не пункты меню.

**Вне паритета (осознанно):**
- скрытые/мёртвые меню legacy: Edit (undo/redo/cut/copy/paste), Print/Preview/Setup, Profiles, Tools/Options, Maps/Cities/SAM/Movement, New Window;
- неработающие функции: BitCodeMaker, working-data cache, сохранение layout в неверный путь;
- деструктивное поведение: молчаливая потеря unknown-тегов, каскадное удаление без предупреждения, перезапись `ubClassIndex`, сброс отсутствующих изображений в `0/0`;
- сервисные/вычисляемые настройки (`AP_Maximum`, `Hide_Nothing_Items`) заменяются отображением, поиском и perf-гейтами;
- Cities/facilities, value-level compare, lookup management и packaging — M24–M27. Undo/redo, оверлеи, структурное сравнение и валидация уже сильнее legacy.

**Маркеры:** **M19 — item-workflow parity (достигнут):** Items + все 1:1/1:N sub-tables, item pickers, nested modifiers, локализованные поля, изображения, item-workflow. **M20 — auxiliary parity (достигнут):** merc gear, shopkeeper-вкладка, 104 вспомогательные таблицы, 7 языков. **M23 — full functional parity:** все рабочие функции legacy, включая multi-dataset/copy, фильтры/выделение/layout.

### Сводная матрица паритета

| Область legacy | Текущее состояние | Milestone |
|---|---|---|
| XML round-trip, undo, overlays, compare, валидация | сильнее legacy | — |
| Grid: sort/search/chooser/bulk/add/delete | частично; фильтры, multi-select и layout persistence — M22a | M22a |
| Item master-detail и sub-tables | реализованы в item workspace, включая nested/list | M19 |
| Main-table pickers (`ITEM`, `AMMO`, `ITEMTOEXPLOSIVE`) | searchable pickers, имена, переход, unknown raw | M18 |
| Merc gear | редактор пяти комплектов, item pickers, base + overlay | выполнен (M20) |
| Shopkeeper/enemy/militia/IMP/sounds | 104 таблицы с типами, ключами, pickers и shopkeeper-вкладкой | выполнен (M20) |
| Lookup-таблицы | редактируются generic grid; метаданные auxiliary-таблиц | M20; CRUD — M26a |
| Локализация | 7 языковых кодов с autodiscovery; в v4/v5 есть только `ru` | выполнен (M20) |
| Multi-dataset и copy строк | селектор датасетов, вкладки по датасетам, явный copy-диалог с preview и remap | выполнен (M21) |
| Item images | превью Big/Medium/Small, picker типа, миниатюры в списке | M19 |
| Settings/layout | layout — M22a; settings dialog и safety — M22b | M22a–M22b |
| Packaging | нет у обоих | M27a–M27b |

---

## Отложено за пределы текущих milestone (осознанно)

| Фича | Причина |
|---|---|
| Слияние подтаблиц предметов в ITEM | item workspace даёт view-проекцию; слияние в модель хранения — риск для целостности |
| Shopkeeper `CONTROL` UI | в legacy его не было; `Inventory` редактируется (M20), `CONTROL` сохраняется как есть |
| Working-data cache | perf-гейты его не требуют; кэш рискует устаревшими данными |
| Cities/facilities, value compare, lookup management, packaging | M24–M27 (в legacy отсутствуют) |
| Three-way merge, Excel-экспорт сравнения, общий STI/Interface/tileset browser | backlog после M27 |

---

## Основные риски

- **R1 остаточный:** неоднозначная структурная замена (несколько вставок/удалений в одном родителе) уходит в serializer и нормализует форматирование; данные при этом корректны.
- **R6 неполные метаданные:** seed покрывает 173 таблицы из 238/239 в v4/v5; document-driven fallback работает, остальные legacy-таблицы добавляются по мере необходимости (M21–M23), миграции растут итеративно.
- **R12 copy (закрыт в M21):** cross-dataset copy может затереть target; source read-only, обязательный preview, явные политики, восстановление уже записанных файлов при ошибке, тесты на temp-копиях, включая corpus v4→v5.
- **R13 многофайловое сохранение:** item workspace пишет несколько XML; per-file atomic save, ошибки агрегируются, частичное сохранение не маскируется.
- **R14 nested/list-правки:** идут через lossless-патчер и round-trip тесты; неизменённые файлы не перезаписываются.
- **Закрыты:** byte-identity и perf (R2), malformed-файлы (R3), dict/list-маски (R4), вложенные ключи оверлеев (R5), запись в `References/` (R7), внешние изменения (R9), культура/отмена (R10), relations drift (R11) — покрыты фикстурами, corpus- и perf-гейтами.

---

## Проверка

1. `dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx -warnaserror` — 0 ошибок/0 предупреждений.
2. `dotnet test xml-editor-reloaded/XmlEditorReloaded.slnx` — все тесты зелёные (сейчас 346).
3. Corpus: `XER_CORPUS_DIR="…/v4/Data-1.13/TableData" dotnet test` (и v5) — прогон на temp-копиях, без изменений в `References/`.
4. Сравнение: `XER_COMPARE_LEFT_DIR=<v4> XER_COMPARE_RIGHT_DIR=<v5>` — `RealDataComparisonTests` зелёный.
5. Copy: `XER_COPY_SOURCE_DIR=<v4> XER_COPY_TARGET_DIR=<v5>` — `CorpusCopyTests` копирует предмет с Weapon/Attachments на temp-копиях, проверяет remap и неизменность source.
6. Perf: `XER_CORPUS_DIR=<v5> dotnet test --filter "Category=Performance"` — порог 2 с (фактические значения см. раздел M0–M10).
7. Ручной smoke текущего состояния: v5 `Lookup/AttachmentPoint` открывается с диагностикой и без записи; v4 переключение на Russian показывает merged-значения, правка `szItemName` меняет только `Russian.Items.xml`; `Items.Items` открывается как item workspace, правка предмета и Weapon сохраняется одной кнопкой, duplicate/delete отменяются целиком; `Inventory.MercStartingGear` открывается как merc gear workspace, правка любого из пяти комплектов и kit name в Russian сохраняется одной кнопкой; вкладка «Торговцы» показывает дилеров выбранного предмета и пишет количество только в dealer-файл; `Sounds.Sounds` редактируется как text-only таблица; сравнение v4↔v5 экспортирует .md/.csv и не меняет файлы; селектор каталогов переключает v4/v5 без закрытия вкладок, а copy-диалог показывает preview и пишет только target; отсутствующие изображения показывают код ошибки, а не ломают редактор.
8. M22a smoke: фильтр по значению и исключение через контекстное меню; AND/OR и числовые операторы; bulk по выделенным строкам и по всему виду; пустой Set; после переоткрытия вкладки восстанавливаются колонки/ширина/сортировка/фильтр, reset возвращает дефолты.
9. M22b smoke: settings dialog меняет каталоги/активный датасет/recent, UI-язык применяется после перезапуска; закрытие вкладки и приложения с несохранёнными правками предлагает Save/Discard/Cancel; внешнее изменение файла при сохранении предлагает Reload/Overwrite/Cancel, Cancel оставляет вкладку dirty.
10. `References/` не изменён; `bin`/`obj` в `References/JA 2 1.13 v4|v5` не появляются (в vendored `References/JA 2 1.13 Image viewer` они поставляются вместе с исходниками и не считаются нарушением).
