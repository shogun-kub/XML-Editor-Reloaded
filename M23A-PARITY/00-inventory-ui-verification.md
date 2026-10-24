# M23a0 — UI inventory verification

**Date:** 2026-10-24
**Verdict:** pass with corrections

## Verified counts

| Claim | Inventory | Actual | Result |
|---|---|---|---|
| Полей `ToolStripMenuItem` в `MainForm.Designer.vb` | 183 | 183 (конструкторы `New ToolStripMenuItem()`); все 183 сопоставлены с деревом ровно по одному разу | совпадает |
| Контейнеров (пунктов с `DropDownItems`) | 31 | 31 | совпадает |
| Листовых команд | 152 | 152 (183 − 31) | совпадает |
| Листовых с `Click`-обработчиком | 138 | 138 | совпадает |
| Листовых без обработчика | 14 | 14 | совпадает |
| Листовых с пустым обработчиком | 3 | 3 (`Cut`, `Copy`, `Paste`) | совпадает |
| Явно скрытых пунктов (`Visible = False`) | 10 (4 контейнера + 6 листовых) | 10 (4 + 6) | совпадает |
| Фактически недоступных из-за скрытого предка | 24 (5 контейнеров + 19 листовых) | 24 (5 + 19) | совпадает |
| Пунктов с `Enabled = False` | 0 | 0 (ни одного присваивания в дизайнере и в `MainForm.vb`) | совпадает |
| Разделителей меню | 7 (`ToolStripSeparator3..9`) | 7 | совпадает |
| Разбивка по корневым меню | File 19, View 4, Edit 7, Data 43, Items 40, Inventories 50, Windows 7, Profiles 4, Maps 5, Tools 2, Help 2 | то же (DFS по `AddRange`) | совпадает |
| Строк в target appendix | 112 | 112 (112 handler-методов с data-opening вызовами) | совпадает |
| Все 18 присваиваний `Visible = False` | сведены только как 10 пунктов + 5 элементов тулбара | 18 = 10 пунктов меню + `ToolStripSeparator3/4/5` + `ToolStripSeparator1/2` + 3 кнопки тулбара | требует дополнения (см. #2) |
| Сверка `Handler` + `MainForm.vb:line` по всем строкам дерева | 183 строки | 0 расхождений: имя метода и строка соответствуют `Handles ...Click`; 14 строк «НЕТ ОБРАБОТЧИКА» действительно без `Handles`, 3 «ПУСТО» имеют только комментарий в теле | совпадает |
| «294 метода-обработчика / 306 привязок» | 294 / 306 | не проверялось: метрика охватывает все формы, вне scope проверки `MainForm` | n/a |

Ручная сверка колонок `Menu path`, `Label`, `Shortcut` для всех 183 строк дала 0 расхождений; единственная «пустая» подпись — `DrugsItemFilterToolStripMenuItem` (`Text` не задан, Designer:946), корректно вынесена в Open questions (Q1).

## Discrepancies

| # | Inventory row/section | Claim | Actual (evidence file:line) | Required correction |
|---|---|---|---|---|
| 1 | «Скрытые, недоступные и мёртвые пункты», bullet «Недоступны из-за скрытого родителя» (строка 226) | «ещё 14, все листовые»; перечислено 13 листовых | Реально 14 пунктов: 1 контейнер `ShowToolStripMenuItem1` + 13 листовых. `ShowToolStripMenuItem1` входит в `ProfilesMenu.DropDownItems` (Designer:1417, 1425-1428) и скрыт вместе с родителем `ProfilesMenu.Visible = False` (Designer:1421) | Заменить на «ещё 14: 1 контейнер (`Profiles > Show`) + 13 листовых» и добавить `ShowToolStripMenuItem1` в перечень скрытых контейнеров. Итоговые 24 (5 + 19) в Summary при этом верны |
| 2 | То же, перечень `Visible = False` | Перечислены 10 пунктов и отдельно тулбар (3 кнопки + 2 разделителя); разделители меню не классифицированы | В дизайнере 18 присваиваний: `ToolStripSeparator3/4/5` (Designer:369, 393, 425), `Print/PrintPreview/PrintSetup` (403, 412, 419), `EditMenu` (473), оба code maker-а (753, 779), `NewWindow` (1383), `ProfilesMenu` (1421), `MapsMenu` (1448), `ToolsMenu` (1480), `ToolStripSeparator1/2` (1550, 1576), 3 кнопки тулбара (1560, 1570, 1586) | Добавить классификацию всех 18: 10 пунктов + 3 разделителя меню + 2 разделителя тулбара + 3 кнопки тулбара |
| 3 | L-SHELL-036/037/038; appendix строки 490-491 | У пункта один целевой обработчик/таблица; Behaviour указывает только «свою» таблицу | `RussianToolStripMenuItem.Click` обрабатывают три метода: `RussianToolStripMenuItem_Click` (MainForm.vb:478), `PolishToolStripMenuItem_Click` (482), `FrenchToolStripMenuItem_Click` (486). `PolishToolStripMenuItem.Click` — два: 482 и 486. Клик по Russian открывает три формы (`Russian/Polish/French.AmmoStrings`), по Polish — две (`Polish/French`) | Добавить примечание к Behaviour строк 036-037 и к appendix: это legacy-дефект копирования `Handles`; зафиксировать продуктовый вопрос, воспроизводить ли его в rewrite. Строки appendix по-прежнему корректны как «один handler = одна команда», но не как «один пункт = одна цель» |
| 4 | Target appendix, строки L-SHELL-035..041 | Строки выглядят безусловными | Пункты German/Russian/Polish/French/Italian/Dutch/Chinese удаляются при `MainForm_Load`, если нет `<TableDirectory>\Items\<Language>.AmmoStrings.xml` (MainForm.vb:45-69). В v4 есть только `Russian.AmmoStrings.xml`, в v5 — только базовый `AmmoStrings.xml` (проверено в `References/JA 2 1.13 v4/v5`) | Пометить 7 строк как условные: в v4 остаются English + Russian, в v5 — только English; учесть, что при смене набора меню не перестраивается |

## Unaccounted menu fields

Нет. Скриптовая сверка 183 конструкторов `ToolStripMenuItem` с 183 строками дерева (через `Designer:NNN` → comment-header блока) не нашла ни одного поля без строки и ни одной строки без поля. `DiseaseMenuItem` (Designer:1610, `WithEvents` Designer:1845) учтён как L-SHELL-073 «Data > Disease», хотя его блок свойств вынесен в конец дизайнера после `StatusLabel`.

## Target appendix check (missing / duplicate / wrong)

- **Missing: 0.** В `MainForm.vb` 112 различных Sub с вызовами `ShowItemGridForm`/`ShowLookupGridForm`/`ShowMercGearGridForm`/`ShowDataGridForm`/`ShowChildDataGridForm`/`BitCodeMakerForm`/`NewItemForm`/`EnterItemIDForm`; в appendix ровно 112 строк. Множества handler-имён совпадают. Закомментированный `DrugsToolStripMenuItem_Click` (MainForm.vb:728-730) не считается — обработчика и пункта нет.
- **Duplicate: 0.** Ни один `L-SHELL-ID` и ни одно handler-имя не повторяются; ни один пункт не имеет двух строк appendix.
- **Wrong: 0.** Для всех 112 строк имя таблицы совпадает с константой `Tables.*` (Tables.vb), путь — с `TableProperty.FileName` (DatabaseCode.vb), фильтр/подтаблица — с телом handler-а и `ItemClass` (Utility.vb:59-81), вид формы — с фактическим вызовом. Три «несовпадения» вида (`L-SHELL-077` ItemDataForm/EnterItemIDForm, `L-SHELL-101` ItemGridForm/CustomFilterForm, `L-SHELL-102` ItemDataForm/NewItemForm) — это принятая в инвентаре составная нотация, не ошибка.
- **Оговорки:** L-SHELL-036/037 — неполный набор целей на уровне пункта (см. #3); L-SHELL-035..041 — условные строки (см. #4); L-SHELL-127 `ENEMYAMMOCHOICES` без `FileName` — appendix корректно указывает «—», вопрос Q2 подтверждён (`MakeEnemyAmmoTable`, DatabaseCode.vb:2070-2088).

## Spot checks

| # | Пункт → handler → `Tables.X` → XML | Evidence | Result |
|---|---|---|---|
| 1 | Items > Show > By Class > Guns (L-SHELL-079) → `GunsToolStripMenuItem_Click` → `ITEM` (+ `WEAPON`) → `Items/Items.xml` (+ `Items/Weapons.xml`) | MainForm.vb:256-258; `Tables.Items.Name` Tables.vb:6; `Tables.Weapons.Name` Tables.vb:25; DatabaseCode.vb:1273, 1206; фильтр `usItemClass=2 AND rocketlauncher=0`, `ItemClass.Gun=2` Utility.vb:61 | совпадает |
| 2 | Items > Show > By Class > Ammo (L-SHELL-080) → `AmmoToolStripMenuItem_Click` → `MAGAZINE` → `Items/Magazines.xml` | MainForm.vb:260-262; `ItemClass.Ammo=1024`; `Tables.Magazines.Name` Tables.vb:46; DatabaseCode.vb:917 | совпадает |
| 3 | Inventories > Merc Starting Gear (L-SHELL-163) → `MercStartingGearToolStripMenuItem_Click` → `MERCGEAR` → `Inventory/MercStartingGear.xml` | MainForm.vb:586-588; wrapper MainForm.vb:219-226; `Tables.MercStartingGear.Name` Tables.vb:223; DatabaseCode.vb:2252 | совпадает |
| 4 | Inventories > Shopkeepers > Jake (L-SHELL-155) → `JakeToolStripMenuItem_Click` → `JakeInventory` → `NPCInventory/JakeInventory.xml` | MainForm.vb:550-552; `Tables.JakeInventory` Tables.vb:153; шаблон DatabaseCode.vb:1621 + `ShopKeepers.Jake` Utility.vb:49 | совпадает |
| 5 | Data > Lookup Tables > Ammo Flags (L-SHELL-048) → `AmmoFlagsToolStripMenuItem_Click` → `AmmoFlag` → `Lookup/AmmoFlag.xml` | MainForm.vb:756-758; `Tables.AmmoFlags` Tables.vb:240; generic `MakeLookupTable` DatabaseCode.vb:837-839 | совпадает |

## Open questions

1. **Горячие клавиши скрытых пунктов.** Утверждение инвентаря, что shortcut-ы `Visible = False` пунктов в WinForms не срабатывают и потому оба code maker-а «фактически недоступны», исходником не подтверждается: это runtime-поведение WinForms, а не свойство legacy-кода. Если shortcut всё же срабатывает, `Attachment Class Code Maker` (Ctrl+Alt+Shift+C) и `Attachment Point Code Maker` (Ctrl+Alt+Shift+A) достижимы, и оценка «24 фактически недоступных» требует уточнения. Нужна проверка на живой сборке.
2. **Мульти-`Handles` для аммо-калибров.** `Russian` и `Polish` имеют несколько обработчиков на один `Click` (MainForm.vb:482, 486). Это выглядит как ошибка копирования, но менять поведение без решения пользователя нельзя: воспроизводить ли в rewrite три/две открываемые формы или исправить на одну?
3. **Условное меню Calibers.** Семь языковых пунктов удаляются на `Load` по наличию файла (MainForm.vb:45-69), причём проверка идёт только для `_activeDataSet` на момент загрузки. Нужно решить, как rewrite строит это подменю при старте и при смене набора, и считать ли appendix-строки L-SHELL-035..041 статическими или условными.
