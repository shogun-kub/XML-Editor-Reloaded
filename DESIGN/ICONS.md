# Icon set

Монохромный stroke-набор для действий редактора. Исходник — [Icons.xaml](../xml-editor-reloaded/src/XmlEditorReloaded.App/Themes/Icons.xaml); контактный лист — [icon-set.svg](icon-set.svg).

## Спецификация

- Сетка 24×24, рабочее поле 20×20 (отступ 2 px).
- Только stroke, без fill; цвет не зашит в геометрию, а приходит из `Theme.Icon` (`DynamicResource`).
- `StrokeThickness=1.75`, round caps/joins; рендер 18×18 в кнопке 28×28.
- 90° и 45° геометрия, без мелких деталей: набор проверяется на 16/18/20 px.
- Один набор для светлой и тёмной темы; цветовые исключения — только `Save` (accent), `Delete` (danger), `Validate` (success), `Problems` (warning).

## Правила использования

- Иконка ставится в тулбары и меню; в диалогах `OK`/`Cancel`/`Close` остаётся текст.
- Каждая icon-only кнопка обязана иметь `ToolTip` и `AutomationProperties.Name`; в тултип входит горячая клавиша.
- Для кнопки используется `Style="{StaticResource IconButton}"`, `Content="{StaticResource Icon.Save}"`.
- Смысловые цвета задаются через `Foreground="{DynamicResource Theme.Danger}"` и т.п., а не отдельной иконкой.

## Состав (60)

| Группа | Ключи |
|---|---|
| Файл и данные | `OpenFolder`, `Browse`, `RemoveFolder`, `Refresh`, `Reload`, `Save`, `SaveAll`, `Export`, `Settings`, `About`, `Donate`, `Exit` |
| Правка | `Undo`, `Redo`, `Add`, `AddRow`, `Duplicate`, `Delete`, `ChangeId`, `WhereUsed`, `Find` |
| Вид и фильтр | `Grid`, `Code`, `Problems`, `Preview`, `FilterAdd`, `FilterClear`, `FilterRemove`, `Columns`, `ColumnsAll`, `ColumnsDefault` |
| Инструменты | `BulkEdit`, `Compare`, `CopyData`, `Images`, `EnemyAmmo`, `Translation`, `DatasetCheck`, `DatasetSearch`, `ResetLayout` |
| Навигация | `GoToId`, `Previous`, `Next`, `OpenRecord`, `FindInFile` |
| Диалоги и статус | `OK`, `Apply`, `Cancel`, `Close`, `Warning`, `Error`, `Info`, `Validate` |
| Дерево и XML | `Folder`, `FolderOpen`, `FileXml`, `FoldAll`, `UnfoldAll`, `FindInTree`, `NewItem` |

## Замечания

- `Refresh` и `Reload` пока используют одну геометрию: обновить список таблиц и перечитать файл — разные команды, но визуально одно действие. Если понадобится различать, `Refresh` получит стрелки списка, `Reload` — стрелку файла.
- `ColumnsDefault` сознательно с маленькой звездой: «вернуть набор колонок по умолчанию».
- Превью показывает подписи по именам ключей; в приложении подписи берутся из RESX и локализуются.
