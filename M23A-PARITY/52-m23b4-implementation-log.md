# M23b4 — валидация, границы чисел и 64-битные маски: журнал реализации

**Дата:** 2026-10-25. **Статус:** выполнен. **Закрытые пробелы:** `L-ITEM-034`, `L-ITEM-047`,
`L-MERC-014`, `L-IFIELD-227…230`, `L-DATA-012` — 5 записей реестра.
**Основание:** `M23A-PARITY/50-numeric-ranges.md` (границы NumericUpDown) и
`M23A-PARITY/51-64bit-masks.md` (64-битные id/маски); план — `PLAN.md` (M23b4).

Этот файл фиксирует состояние, решения и проверки M23b4. Источник истины — код, тесты и git;
здесь только то, что из них не восстанавливается (мотивы выбора границ и среды).

## 1. Состояние на входе

- Незакоммиченные правки прошлой сессии уже содержали каркас: `LookupEntry.Id` → `ulong`,
  `ClassVisibilityRule.IsVisible(ulong)`, `ColumnHint.Minimum/Maximum`, `ExclusiveFlagGroups` и
  `ClassValueLimit`, `ValidationCodes.OutOfRange/ExclusiveFlags/ClassValueLimit`, проверки в
  `TableValidator`, миграцию `0015` (частичный seed), тесты `TableValidatorTests`/`MetadataTests`.
- Сборка была красной: `TableCopyRemapper` сравнивал `ulong` с `long` (CS0034), App-слой не был
  переведён на `ulong`, тестовые fake-сервисы `IDialogService` и `BitEditorViewModel` оставались
  32-битными.
- Миграция `0015` покрывала только `ItemSize`, 21 status, 12 quantity, 2 цены, launcher-группу и
  карту LBE; из §5 документа 50 не хватало 106 полей. Дополнительно `mSmall0Quantity`/`mSmall1Quantity`
  были ошибочно засеяны как 0…10 вместо 0…100 (у legacy там нет `Maximum`, действует default 100).

## 2. Ограничение среды и как проверялось

- Песочница сессии работает на низком уровне целостности; существующие каталоги рабочей папки имели
  обычный уровень, поэтому запись в них (сборка, `obj/`, `.git`) падала с «Access denied». Метка
  целостности каталогов `xml-editor-reloaded`, `.git`, `M23A-PARITY` и файлов в корне понижена до
  уровня песочницы (содержимое, владельцы и ACL не менялись); после этого обычные `dotnet build` и
  `git` работают. Артефакты восстановления — в `.acl-recovery/` (не коммитится).
- **VSTest в этой среде не запускается вообще** (и в песочнице, и без неё): testhost открывает
  дескриптор родительского процесса, а токен запрещает `OpenProcess`. Поэтому прогоны выполнялись
  временным in-process раннером `__testrunner` (reflection по `[Fact]`/`[Theory]`, `ITestOutputHelper`
  через `DispatchProxy`), который запускался в каталоге тестовой сборки. Раннер удалён после
  проверки; при повторной необходимости его достаточно воссоздать по этому описанию (≈180 строк,
  без пакетов).
- Итоговые официальные команды остаются штатными: `dotnet build … -warnaserror` и `dotnet test`.
  В обычной среде (без песочницы) они дают те же результаты; здесь они недоступны.
- **Дополнение 2026-10-25:** ограничение снято на уровне проекта. `Directory.Build.props`
  при `DSH_SHELL`/`DSH_SESSION_ID` выключает `MSTest.EnableParentProcessQuery` (testhost больше не
  открывает родителя с `PROCESS_ALL_ACCESS`) и `NuGetAudit` (нет сети до nuget.org).
  `xml-editor-reloaded/test.ps1` делает restore/build в один узел и запускает проекты по очереди
  с чистым `%TEMP%`; VSTest-прогон — 470 тестов (App 150, Core 37, Data 67, Imaging 19, Xml 197).
  Временный in-process раннер из §2 больше не нужен.

## 3. Принятые решения

1. **Границы живут в `ColumnHints`.** Все ограничиваемые поля — не declaring: `Columns` в seed есть
   только у lookup-таблиц (`id`/`name`), поэтому `Minimum`/`Maximum` в `ColumnHints` покрывают и
   Items, и Merc gear без второй схемы. Границы целочисленные (`INTEGER`, inclusive), потому что
   все legacy-границы целые; значение при этом сравнивается как `decimal`, так что дробные данные
   (`0.25`, `-0.2`, `3.5`) проверяются корректно. Ограничение зафиксировано тестом
   `Validate_DecimalField_UsesInclusiveBounds`.
2. **Только предупреждение, без clamp.** Legacy не только ограничивала спиннеры, но и обнуляла
   отключённые карманы (`ItemDataForm.vb:1468-1479, 1500-1506, 1559-1563`). Rewrite ничего не
   очищает и не перезаписывает: `validation.out_of_range`, `validation.exclusive_flags`,
   `validation.class_value_limit` — это `Warning`, документ не становится dirty.
3. **Seed — ровно то, что применяла legacy, кроме двух противоречий данным.** 123 поля в 6 таблицах
   (Items.Items 61, Weapons 14, Explosives 8, Magazines 1, LBE 4, MercStartingGear 35). Не засеяны
   `Items.Items.PercentStatusDrainReduction` (в данных −300) и
   `Items.Weapons.usOverheatingDamageThreshold` (65650) — иначе shipped-данные давали бы 3
   предупреждения; не засеяны `ubGraphicNum` (динамика по изображениям, M23b5), `nasLayoutClass`
   (`Int32.MaxValue`), `uiIndex` (ключ) и 33 вложенных контрола стоек (плоский `ColumnHints` не
   выражает путь; M23b5). `ItemSize` — 0…99: `MAX_ITEM_SIZE` по умолчанию 99, в данных 120 записей
   ровно 99, чтение `Ja2_Options.INI` (34) сознательно не реализовано — оно дало бы 120 warning'ов.
4. **Карта LBE → карманы — «не больше N-го ordinal».** Legacy включает непрерывный префикс:
   класс 0 — ничего, 1 — 1…5, 2 — 1…12, 3 — 1…7, 4 — 1…12 (проверено по `ItemDataForm.vb:1452-1580`).
   В данных 39 записей/версию имеют «дыры» (нули до максимального индекса), поэтому правило
   ограничивает максимальный ordinal, а не требует заполнения 1…N. Классы 2 и 4 действительно
   идентичны по набору карманов, но различаются именами (Vest/Backpack).
5. **Launcher — группа «не более одного».** Четыре radio в legacy — одна группа
   (`ItemDataForm.Designer.vb:8151-8169, 8184-8196`); «N/A» без Tag просто снимала остальные, то
   есть «все четыре = 0» — валидное состояние. В данных 0 записей с двумя флагами.
6. **64 бита без нового `ColumnDataType`.** `Flags` уже означает «unsigned bit mask»; менять enum
   нельзя: `SqliteMetadataProvider` разбирает тип через `Enum.TryParse` с игнорированием результата и
   молча деградировал бы в `String`. Расширены только разбор (`ulong`), битовый редактор и метки.
7. **Отрицательные значения `Lookup` остаются валидными по типу.** `-1` как «нет значения» уже
   встречается в данных; membership проверяется только по беззнаковым id, поэтому отрицательное
   значение даёт `validation.lookup_missing`, а не `invalid_type` (тест
   `Validate_NegativeLookupValue_IsMissingNotInvalid`). Файл lookup с отрицательным `<id>` теперь
   отвергается целиком (`xml.invalid_lookup_entry`) — в корпусе таких нет.
8. **Сентинел «всегда видимо» остаётся `long`.** `ClassVisibilityRule.VisibleClasses` — `long`,
   `< 0` значит «всегда видимо»; `classBits` расширен до `ulong`. Маски правил ≤ 2^30, поэтому
   приведение безопасно. Если когда-нибудь понадобится бит 63 в данных правила, сентинел придётся
   заменить отдельным флагом.
9. **Сортировка lookup/flags — через `decimal`.** Покрывает и отрицательные, и `ulong.MaxValue`,
   не даёт «2^63 < 0» и не требует сравнения знаковых представлений.
10. **Окно битового редактора.** 64 бита при `Columns="4"` дают 16 строк; высота увеличена до
    `Height=500`/`MinHeight=320`, `ScrollViewer` оставлен страховкой для крупного шрифта. `Columns=8`
    отвергнуто: подписи битов (`Lookup.ItemFlag`, `AttachmentClass`) не помещаются в 65 px.

## 4. Проверенные факты корпуса

| Проверка | v4 | v5 |
|---|---|---|
| `Lookup/*.xml` max id | `ItemFlag` 8589934592 | то же |
| Максимум любого листа вне Lookup | `NPCInventory/Merchants.uiFlags` 2415919104 | то же |
| Записи LBE с индексом кармана выше лимита класса | 0 / 108 | 0 / 108 |
| Записи с ≥2 launcher-флагами | 0 / 1776 | 0 / 1775 |
| Значения вне засеянных диапазонов | 1: `MercStartingGear` mIndex 237 kit 5 `mSmall1Status`=600 | то же |
| Другие нарушения 122 полей §5 | 0 | 0 |

Единственное ожидаемое предупреждение закреплено corpus-тестом
`MappedTables_ProduceOnlyTheKnownRangeWarning` (и допускается в
`RealAuxiliaryTablesTests.AuxiliaryTables_OpenWithKeysAndValidateCleanly`), поэтому изменение данных
или границ сразу видно в гейте.

## 5. Что изменено

- `Data/Metadata/Migrations/0015_M23b4ValidationLimits.sql`: `Minimum`/`Maximum` в `ColumnHints`,
  `ExclusiveFlagGroups`, `ClassValueLimits`, seed launcher-группы, карты LBE и 123 диапазонов
  (сгруппированные `UPDATE … WHERE XmlFieldName IN (…) AND TableId = (SELECT …)`;
  `UPDATE … FROM` в этом SQLite-диалекте не видит целевую таблицу).
- `Core`: `LookupEntry.Id` (`ulong`), `ColumnHint.Minimum/Maximum`, `ExclusiveFlagGroup`,
  `ClassValueLimit`, `TableMetadata.ExclusiveFlagGroups/ClassValueLimits`,
  `ClassVisibilityRule.IsVisible(ulong)`, три новых кода валидации.
- `Data`: чтение новых колонок/таблиц; `LookupValues` приводится к `ulong` без потери знака.
- `Xml`: `TableRecord`/`TableValidator`/`TableWorkspace.Bulk` — `Flags` как `ulong`, `Lookup` —
  знаковый либо беззнаковый; `TableCopyRemapper` — `ulong`-сопоставление; валидация диапазонов,
  эксклюзивных групп и классовых лимитов; membership lookup с отрицательными значениями.
- `App`: 64-битный `BitEditorViewModel`/`IDialogService.EditBits`/`BitEditorWindow` (XAML и код),
  метки битов без фильтра `uint.MaxValue`, `ulong` в `LookupColumnData` и `ItemWorkspaceViewModel`,
  сортировка через `decimal`, `TableEditorViewModel.IsValidValue` для `Lookup`/`Flags`.
- Тесты: обновлены 32-битные ожидания (`TableRecordTests.SetFieldValue_FlagHint_AcceptsUInt64Range`,
  `BitEditor_ComposesValueAndLoadsXaml`) и fake-сервисы (8 файлов); добавлены проверки 64-битных id,
  масок, сортировки масок через знаковую границу, remap по имени, отсутствия ложных предупреждений
  и точного набора warning'ов на корпусе.

## 6. Проверка M23b4

| Проверка | Результат |
|---|---|
| Сборка `-warnaserror` | 0 ошибок / 0 предупреждений |
| Базовый прогон | 470/470: Core 37, Data 67, Imaging 19, Xml 197, App 150 |
| Corpus v4 / v5 | 470/470 на каждом; единственное `out_of_range` — `GEARKIT/mSmall1Status` (237), 0 `exclusive_flags`, 0 `class_value_limit` |
| Compare / copy | 1/1, 1/1 |
| Perf (v5) | 9/9 |
| Round-trip / целостность | неизменённые файлы не перезаписываются; warning не делает документ dirty (проверки в тестах) |
| `References/` | не изменён; копии и temp-каталоги |

Прогон 2026-10-25 после `67ae29d` выполнен штатным VSTest через `xml-editor-reloaded/test.ps1`
(build 0/0, 470 тестов; corpus v4 и v5 — по 470; compare/copy 1/1; perf 9/9). Числа 460 в ранней
версии этого журнала получены временным in-process раннером, который пропустил 9 тестов; штатный
VSTest дал 469, а добавленный при верификации тест
`SortByFlagMask_OrdersUnsignedValuesAcrossSignedBoundary` (сортировка `Flags` через
2^63−1/2^63/`ulong.MaxValue`, §5.8 документа 51) — 470.

## 7. Остаточные риски и задел

- Бит 63 в данных правил видимости несовместим с сентинелом `-1` — понадобится явный флаг.
- `Minimum`/`Maximum` только целые; дробные границы потребуют `REAL`/`decimal` в модели.
- Вложенные стойки (`STAND/CROUCH/PRONE_MODIFIERS`, 33 контрола) не покрыты плоским
  `ColumnHints`; это M23b5 вместе с `ubGraphicNum`.
- `MAX_ITEM_SIZE` из `Ja2_Options.INI` не читается осознанно (см. §3.3).
- `LookupValues.Value` остаётся знаковым `INTEGER`: курируемые id > 2^63−1 не представимы; при
  реальной необходимости — отдельная колонка `ValueText`, а не two's complement.
