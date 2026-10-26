# M23b6 — shopkeeper и identity/CRUD lookup-строк: журнал реализации

**Дата:** 2026-10-25. **Статус:** выполнен. **Закрытые пробелы:** `L-ITEM-039`, `L-SHP-001`,
`L-AUX-036` — 3 записи реестра (3 ID).
**Основание:** `M23A-PARITY/20-item-ui.md` (L-ITEM-039), `M23A-PARITY/30-merc-aux.md`
(L-SHP-001), `M23A-PARITY/20-item-ui.md` / реестр `LEGACY-PARITY-REPORT.md` (L-AUX-036);
план — `PLAN.md` (M23b6).

Источник истины — код, тесты и git. Здесь зафиксированы мотивы решений, сверка с legacy и
остаточные ограничения.

## 1. Состояние на входе

- M23b5 закрыт, дерево чистое, HEAD `06ebea7`; сборка `-warnaserror` 0/0, 488 тестов
  (Core 37, Data 67, Imaging 24, Xml 197, App 163), corpus v4/v5 по 488, compare/copy 1/1,
  perf 9/9.
- Вкладка Shopkeepers показывала только существующие строки дилера и умела менять
  `ubOptimalNumber`; создать или удалить строку из формы предмета было нельзя (`L-ITEM-039`).
- Строка дилера адресовалась по `uiIndex`: `SetShopkeeperQuantity` вызывал
  `TableSetWorkspace.SetValue(table, RowKey, ...)`, а `TableDocument.FindRecord` возвращает
  первую запись с ключом. В `AdditionalDealer_43_Inventory.xml` обеих версий две строки
  `uiIndex=26` (предметы 1422 и 1456), поэтому правка item 1456 уходила в строку item 1422
  (`L-SHP-001`).
- `TableDocument.AddRecord` создавал только `<id>`; после сохранения такой lookup-файл
  отвергался `XmlFileLookupRepository` целиком (`xml.invalid_lookup_entry`), и потребители
  теряли combo до ручного исправления (`L-AUX-036`).

## 2. Сверка с legacy

- Форма предмета держала 19 фиксированных чекбоксов торговцев; при открытии строка искалась по
  `sItemIndex`, количество бралось из первой строки, иначе подставлялось значение колонки по
  умолчанию. При сохранении: checked + строка есть — обновить количество; checked + строки нет —
  создать строку с `sItemIndex` и количеством; unchecked — удалить первую строку
  (`References/xml-editor/GUI/Data Forms/ItemDataForm.vb:1368-1447`). Колонка
  `ubOptimalNumber` имела `DefaultValue = 0` (`References/xml-editor/BackEnd/DatabaseCode.vb:1628`),
  ключ `uiIndex` выдавался автоматически (`InventoryTable.vb`/`AutoIncrementTable.vb`).
- Legacy искала строку только по `sItemIndex` и при дубликатах `uiIndex` брала первую; это и было
  причиной `L-SHP-001` в rewrite, который расширил набор на 80 инвентарей, включая
  `AdditionalDealer_43`.
- Lookup-файл — пары `id`/`name`; `LookupFile.AddLookupData` принимал пустое имя, а
  `DataGridForm` давал новой строке `name=""` из `DefaultValue` колонки
  (`References/xml-editor/BackEnd/LookupFile.vb:11-35`,
  `References/xml-editor/GUI/Grid Forms/DataGridForm.vb:23`).

## 3. Принятые решения

1. **Item-центричное создание без 19-чекбоксной формы.** Вкладка Shopkeepers получает combo
   загруженных инвентарей, у которых ещё нет строки для выбранного предмета, поле количества
   (по умолчанию `0`, как legacy `DefaultValue`) и кнопку Add. Это покрывает сценарий «торговец
   продаёт предмет» и остаётся сверхмножеством legacy: rewrite показывает все 80 загруженных
   инвентарей, а не только 19 именованных. Список вариантов пересобирается после add/remove,
   undo/redo, смены предмета и reload; уже stocked торговец в него не попадает, а инвентарь,
   который загрузился read-only, не предлагается и не принимает Add.
2. **Удаление — точная строка, с подтверждением.** У каждой строки есть Remove; удаляется
   `WorkspaceRow`, а не первая запись с ключом. В отличие от legacy, удаление не происходит
   молча при снятии чекбокса: сначала подтверждение, затем одна undo-транзакция. Это осознанно
   более безопасное поведение.
3. **Exact-row адресация (`L-SHP-001`).** `ShopkeeperEntryViewModel` хранит `WorkspaceRow`;
   количество пишется через `TableSetWorkspace.SetValue(TableName, WorkspaceRow, ...)`, удаление —
   через `TableSetWorkspace.DeleteRow(TableName, WorkspaceRow)`, обновление — через
   `GetRow(TableName, WorkspaceRow)`. Ключ `uiIndex` больше не используется для поиска строки,
   поэтому дубликат ключа не меняет чужую строку. Дубликат остаётся предупреждением валидатора,
   rewrite не перенумеровывает данные (так закрыт открытый вопрос 6 аудита).
4. **Lookup: пустой `name` при добавлении (`L-AUX-036`).** `TableDocument.AddRecord` для таблицы
   с `Metadata.IsLookup` дописывает пустой `<name />` сразу после ключа. Формат lookup-файла
   (`id`/`name`) неизменен, не-lookup таблицы не затрагиваются, существующие записи не
   переписываются. Значение по умолчанию пустое, как в legacy, а не выдуманное имя.
5. **Количество валидируется до записи.** Принимается только неотрицательное целое в invariant
   culture; отрицательное, дробное или мусор отклоняются с сообщением, набор не становится dirty.
   Legacy `NumericUpDown` тоже не пропускал нечисло, но молча зажимал границы — здесь значение
   просто не пишется.
6. **Read-only инвентарь не предлагается.** После удаления последней строки файл дилера
   становится пустым; при следующей загрузке он помечается `LoadedReadOnly` (`L-STO-034`).
   Поэтому combo строится только из `Workspace.CanEdit`-инвентарей, а Add/Remove дополнительно
   проверяют это состояние. В текущей сессии, пока документ ещё `Loaded`, строку можно вернуть
   через Undo или добавить заново; после reload — только после M23b8. Тест
   `ShopkeeperTab_EmptiedDealerFile_HidesDealerAfterReload` фиксирует это поведение.
7. **Refresh не оставляет «призрачных» строк.** При history-change строка, чья запись исчезла
   или была пересоздана undo, вызывает пересборку entries/options, а не только обновление
   количества. Иначе после undo/redo количество могло временно опустеть, а удалённая строка
   оставалась бы в списке.
8. **Без новых миграций метаданных.** Все нужные сведения уже есть: `ShopkeeperInventory`
   relation, `Items.Items` key, `TableMetadata.IsLookup`. Поэтому M23b6 не меняет SQLite-схему и
   seed.

## 4. Что изменено

- `Xml/Documents/TableDocument.cs`: `AddRecord` создаёт пустой `name` для lookup-таблицы
  (локальная проверка не даёт добавить второй `name`).
- `Xml/Workspace/TableSetWorkspace.cs`: row-based API `GetRow(TableName, WorkspaceRow)`,
  `SetValue(TableName, WorkspaceRow, ...)`, `DeleteRow(TableName, WorkspaceRow)` с прежними
  кодами ошибок; используется item workspace, пригодно для других multi-table сценариев.
- `App/ViewModels/ShopkeeperEntryViewModel.cs`: хранит `WorkspaceRow`, `RowKey` выводится из
  строки, `RemoveCommand` делегирует удаление host-у.
- `App/ViewModels/ShopkeeperDealerOptionViewModel.cs`: вариант «торговец, у которого ещё нет
  строки».
- `App/ViewModels/ItemWorkspaceViewModel.cs`: `ShopkeeperDealerOptions` (только `CanEdit`),
  `SelectedShopkeeperDealer`, `NewShopkeeperQuantity`, `AddShopkeeperEntryCommand`,
  `RemoveShopkeeperEntry`, exact-row quantity/refresh/delete, пересборка вариантов и очистка при
  смене/сбросе предмета; сброс количества в `0` после добавления; подтверждение удаления
  называет торговца, предмет и количество; подписи вкладки.
- `App/Views/ItemWorkspaceView.xaml`: combo торговцев, подпись и поле количества, Add, колонка
  Remove; строка добавления видна только при выбранном предмете, кнопки учитывают `IsEditable`.
- `Strings.resx`/`Strings.ru.resx`: 8 новых строк (метка add, Add, Remove, «все торговцы уже
  продают», заголовок и текст подтверждения, добавлено, удалено).
- Тесты: `TableWorkspaceTests.Lookup_AddRecord_CreatesEmptyNameAndSavesLoadableFile`,
  `NonLookup_AddRecord_DoesNotAddNameElement`;
  `ItemWorkspaceViewModelTests.ShopkeeperTab_AddsAndRemovesDealerRow` (default 0, reset, cancel
  removal, negative/non-numeric quantity, items file untouched),
  `ShopkeeperTab_AddDealerRow_UndoAndRedoRestoreChoices`,
  `ShopkeeperTab_RepeatedDealerKey_EditsAndRemovesExactRow`,
  `ShopkeeperTab_EmptiedDealerFile_HidesDealerAfterReload` (плюс существующий
  `ShopkeeperTab_ListsDealersAndSavesQuantity`).

## 5. Проверка M23b6

**Коммит:** `e1a086e M23b6: shopkeeper row CRUD and lookup identity`.

| Проверка | Результат |
|---|---|
| Сборка `-warnaserror` | 0 ошибок / 0 предупреждений |
| Базовые тесты | 494/494: Core 37, Data 67, Imaging 24, Xml 199, App 167 |
| Corpus v4 | 494/494 |
| Corpus v5 | 494/494 |
| Compare / copy / perf | 1/1, 1/1, 9/9 (v5) |
| `References/` | не изменён; corpus/compare читают корпус, copy работает на temp-копиях |

Ключевые тесты: `ShopkeeperTab_AddsAndRemovesDealerRow`,
`ShopkeeperTab_AddDealerRow_UndoAndRedoRestoreChoices`,
`ShopkeeperTab_RepeatedDealerKey_EditsAndRemovesExactRow`,
`ShopkeeperTab_EmptiedDealerFile_HidesDealerAfterReload`,
`ShopkeeperTab_ListsDealersAndSavesQuantity`,
`Lookup_AddRecord_CreatesEmptyNameAndSavesLoadableFile`,
`NonLookup_AddRecord_DoesNotAddNameElement`.

## 6. Остаточные риски и задел

- **Пустой файл дилера.** Если у инвентаря вообще нет записей, загрузчик помечает таблицу
  `LoadedReadOnly` (`L-STO-034`), и добавить первую строку из item workspace нельзя. Такой
  инвентарь теперь не предлагается в combo и не принимает Add/Remove, поэтому ошибка не
  маскируется; в текущей сессии строку можно вернуть Undo, после reload — только после M23b8.
  В v4/v5 все 80 инвентарей непустые, поэтому сценарий не проявляется без ручного удаления
  последней строки.
- **Количество по умолчанию `0`.** Это паритет с legacy, но `0` означает «не продаётся» для
  игрового баланса; пользователь должен изменить число. Альтернатива (`1`) была бы удобнее, но
  изменила бы поведение без основания.
- **Пустое имя lookup.** Новая строка сохраняется с `name=""` и читается репозиторием, но в combo
  выглядит пустой подписью. Защита от дубликатов имён и удаления используемых lookup-значений —
  M26a/M26b, не M23b6.
- **Нет отдельной валидации lookup перед сохранением.** Защищён путь `AddRecord`; если запись без
  `name` появится из другого источника (ручная правка XML, будущий copy-сценарий), репозиторий
  по-прежнему отвергнет файл. В M26a стоит добавить диагностику до записи.
- **Exact-row не меняет диагностику дубликата.** `validation.duplicate_key` остаётся
  предупреждением, данные не перенумеровываются; это соответствует правилам целостности и
  решению открытого вопроса 6.
- **19 против 80.** Rewrite показывает все загруженные инвентари; legacy — только 19 именованных.
  Это осознанное расширение, уже зафиксированное в M20/M23a4.
- **Начальные значения `AddRecord`.** `TableWorkspace.AddRecord` не проверяет результат
  `TrySetValue` для initial values. Для shopkeeper это целые значения в лексически допустимом
  виде, поэтому при текущих метаданных запись не отклоняется; если появятся range-ограничения
  или нечисловые поля, результат стоит проверять.

**Следующий подшаг:** M23b7 — shell, field UI и полировка merc gear (`L-SHELL-183`,
`L-AUX-018/019`, `L-ITEM-017`, `L-MERC-003…007/013`, `L-SET-009`).
