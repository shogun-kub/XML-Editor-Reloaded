# M23a0 — UI command inventory

**Date:** 2026-10-24
**Files read fully:** `GUI/App Forms/MainForm.vb` (771), `GUI/Data Forms/BaseDataForm.vb` (176), `GUI/Forms/*.vb` (BitCodeMakerForm 95, ChangeClassForm 51, ColumnSelectForm 63, CustomFilterForm 32, EnterItemIDForm 38, LoadingForm 22, NewItemForm 47, SimpleFormBase 30), `GUI/Grid Forms/*.vb` (GridForm 351, GridUtility 196, ItemGridForm 85, DataGridForm 29, ChildDataGridForm 21, LookupGridForm 21, MercGearGridForm 35), `GUI/Main.vb`, `GUI/ApplicationEvents.vb`, `GUI/SettingsUtility.vb`, `GUI/IniFile.vb`, `GUI/App Forms/AboutForm.vb`, `GUI/App Forms/SplashForm.vb`, `GUI/Grid Forms/GridForm.Designer.vb` (149).
**Files grepped (not read fully):** `GUI/App Forms/MainForm.Designer.vb` (1847), `GUI/Data Forms/ItemDataForm.vb` (1708, целевые фрагменты), `ItemDataForm.Designer.vb` (13996), `MercGearDataForm.vb` (82), `MercGearDataForm.Designer.vb` (9814), `BaseDataForm.Designer.vb`, `GUI/Forms/*.Designer.vb`, `GUI/Grid Forms/*.Designer.vb`, `BackEnd/Data Access/Tables.vb`, `BackEnd/Data Access/Utility.vb` (`ItemClass`), `BackEnd/DatabaseCode.vb` (только `TableProperty.FileName`).
**Method:** дерево меню восстановлено из `MenuStrip.Items.AddRange` / `DropDownItems.AddRange` и свойств `Text`, `ShortcutKeys`, `Visible`, `Checked` в `MainForm.Designer.vb`; обработчики сопоставлены с `Handles ...Click` и `AddHandler` в `MainForm.vb`; для остальных форм извлечены `Handles`/`AddHandler`/`AddressOf` и тексты командных контролов. Строки `file:line` — объявление контрола в дизайнере и/или объявление обработчика. Координаты, размеры, шрифты и цвета игнорировались. Поведение не додумывалось: неясные места вынесены в «Open questions».

**Дополнения M23a0:** проверка инвентаря — `00-inventory-ui-verification.md`; расширенная карта полей `ItemDataForm` (`L-IFIELD`, `L-IFLAG`, `L-ICOUNT`) — `00-inventory-ui-item-fields.md`.

Обозначения: `Visible: да/нет` — с учётом скрытого родителя; `Enabled` у пунктов меню нигде не выставляется в `False`; `ПУСТО` — обработчик существует, но тело пустое; `НЕТ ОБРАБОТЧИКА` — нет ни `Handles`, ни `AddHandler`. ID в приложении и в сводке сокращений — ссылки на ID дерева меню; новых сущностей они не определяют.

## Summary

| Показатель | Значение |
|---|---|
| Пунктов меню `ToolStripMenuItem` | **183** (31 контейнер + 152 листовых команды) |
| Из них с обработчиком `Click` | **138** листовых |
| Листовых без обработчика | **14** |
| Листовых с пустым обработчиком | **3** (`Cut`, `Copy`, `Paste`) |
| Явно скрытых (`Visible = False`) пунктов | **10** (4 контейнера + 6 листовых) |
| Фактически недоступных пунктов (скрытый родитель) | **24** (5 контейнеров + 19 листовых) |
| Пунктов с `Enabled = False` | **0** |
| Листовых без работающего обработчика | **17** (14 без обработчика + 3 пустых) |
| Кнопок тулбара | **6** (3 видимых с обработчиком, 3 скрытых без обработчика) + 2 скрытых разделителя |
| Разделителей меню | **7** (`ToolStripSeparator3..9`) |
| Различных методов-обработчиков UI | **294** |
| Привязок событий UI | **306** |
| Строк в документе | SHELL 201, GRID 42, ITEM 46, MERC 8, AUX 31, SET 10, MISC 3 |
| Строк в приложении (ссылки на команды) | 112 |

Разбивка меню: File 19, View 4, Edit 7, Data 43, Items 40, Inventories 50, Windows 7, Profiles 4, Maps 5, Tools 2, Help 2. Из 152 листовых команд обработчик имеют 138; все 133 *доступные* листовые команды имеют непустой обработчик.

## MainForm surface

### Menu tree

Мнемоники (`&`) сохранены в колонке Label. Все подписи меню — жёстко заданные строки в дизайнере, не RESX; заголовки форм строятся из `DisplayText.*`.

| ID | Menu path | Label | Shortcut | Handler | Legacy file:line | Visible / Enabled | Behaviour |
|---|---|---|---|---|---|---|---|
| L-SHELL-001 | File | `&File` | — | — | Designer:249; — | да / да | контейнер (8 п.) |
| L-SHELL-002 | File > Active Data Set | `&Active Data Set` | — | — | Designer:257; — | да / да | контейнер (10 п.) |
| L-SHELL-003 | File > Active Data Set > 1 - {0}  | `&1 - {0} ` | Ctrl+1 | DataToolStripMenuItem_Click | Designer:264; MainForm.vb:674 | да / да | Активный набор = GameData(Tag); снять Checked с остальных; заголовок окна |
| L-SHELL-004 | File > Active Data Set > 2 - {0} | `&2 - {0}` | Ctrl+2 | DataToolStripMenuItem_Click | Designer:275; MainForm.vb:674 | да / да | Активный набор = GameData(Tag); снять Checked с остальных; заголовок окна |
| L-SHELL-005 | File > Active Data Set > 3 - {0} | `&3 - {0}` | Ctrl+3 | DataToolStripMenuItem_Click | Designer:284; MainForm.vb:674 | да / да | Активный набор = GameData(Tag); снять Checked с остальных; заголовок окна |
| L-SHELL-006 | File > Active Data Set > 4 - {0} | `&4 - {0}` | Ctrl+4 | DataToolStripMenuItem_Click | Designer:293; MainForm.vb:674 | да / да | Активный набор = GameData(Tag); снять Checked с остальных; заголовок окна |
| L-SHELL-007 | File > Active Data Set > 5 - {0} | `&5 - {0}` | Ctrl+5 | DataToolStripMenuItem_Click | Designer:302; MainForm.vb:674 | да / да | Активный набор = GameData(Tag); снять Checked с остальных; заголовок окна |
| L-SHELL-008 | File > Active Data Set > 6 - {0} | `&6 - {0}` | Ctrl+6 | DataToolStripMenuItem_Click | Designer:311; MainForm.vb:674 | да / да | Активный набор = GameData(Tag); снять Checked с остальных; заголовок окна |
| L-SHELL-009 | File > Active Data Set > 7 - {0} | `&7 - {0}` | Ctrl+7 | DataToolStripMenuItem_Click | Designer:320; MainForm.vb:674 | да / да | Активный набор = GameData(Tag); снять Checked с остальных; заголовок окна |
| L-SHELL-010 | File > Active Data Set > 8 - {0} | `&8 - {0}` | Ctrl+8 | DataToolStripMenuItem_Click | Designer:329; MainForm.vb:674 | да / да | Активный набор = GameData(Tag); снять Checked с остальных; заголовок окна |
| L-SHELL-011 | File > Active Data Set > 9 - {0} | `&9 - {0}` | Ctrl+9 | DataToolStripMenuItem_Click | Designer:338; MainForm.vb:674 | да / да | Активный набор = GameData(Tag); снять Checked с остальных; заголовок окна |
| L-SHELL-012 | File > Active Data Set > 10 - {0} | `1&0 - {0}` | Ctrl+0 | DataToolStripMenuItem_Click | Designer:347; MainForm.vb:674 | да / да | Активный набор = GameData(Tag); снять Checked с остальных; заголовок окна |
| L-SHELL-013 | File > Reload | `&Reload` | Ctrl+R | OpenFile | Designer:356; MainForm.vb:120 | да / да | OpenFile: повторная загрузка всех наборов; Splash, главное окно скрыто |
| L-SHELL-014 | File > Save Active Data Set | `&Save Active Data Set` | Ctrl+S | SaveToolStripMenuItem_Click | Designer:371; MainForm.vb:689 | да / да | Save(False): сохранение активного набора; LoadingForm |
| L-SHELL-015 | File > Save All Data Sets | `Sa&ve All Data Sets` | Ctrl+Shift+S | SaveAllToolStripMenuItem_Click | Designer:380; MainForm.vb:693 | да / да | Save(True): сохранение всех наборов; LoadingForm |
| L-SHELL-016 | File > Print | `&Print` | Ctrl+P | — | Designer:395; — | нет (Visible=False) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-017 | File > Print Preview | `Print Pre&view` | — | — | Designer:405; — | нет (Visible=False) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-018 | File > Print Setup | `Print Setup` | — | — | Designer:414; — | нет (Visible=False) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-019 | File > Exit | `E&xit` | — | ExitToolsStripMenuItem_Click | Designer:427; MainForm.vb:151 | да / да | Close MainForm; MDI-дети закрываются, запроса сохранения нет |
| L-SHELL-020 | View | `&View` | — | — | Designer:433; — | да / да | контейнер (3 п.) |
| L-SHELL-021 | View > Toolbar | `&Toolbar` | — | ToolBarToolStripMenuItem_Click | Designer:440; MainForm.vb:167 | да / да | ToolStrip.Visible = Checked |
| L-SHELL-022 | View > Status Bar | `&Status Bar` | — | StatusBarToolStripMenuItem_Click | Designer:449; MainForm.vb:171 | да / да | StatusStrip.Visible = Checked |
| L-SHELL-023 | View > Background | `&Background` | — | BackgroundImageToolStripMenuItem_Click | Designer:458; MainForm.vb:665 | да / да | фон MDI-клиента DESKTOP вкл/выкл |
| L-SHELL-024 | Edit | `&Edit` | — | — | Designer:467; — | нет (Visible=False) / да | контейнер (6 п.) |
| L-SHELL-025 | Edit > Undo | `&Undo` | — | — | Designer:475; — | нет (родитель) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-026 | Edit > Redo | `&Redo` | — | — | Designer:483; — | нет (родитель) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-027 | Edit > Cut | `Cu&t` | — | CutToolStripMenuItem_Click | Designer:496; MainForm.vb:155 | нет (родитель) / да | ПУСТО: только комментарий |
| L-SHELL-028 | Edit > Copy | `&Copy` | — | CopyToolStripMenuItem_Click | Designer:504; MainForm.vb:159 | нет (родитель) / да | ПУСТО: только комментарий |
| L-SHELL-029 | Edit > Paste | `&Paste` | — | PasteToolStripMenuItem_Click | Designer:512; MainForm.vb:163 | нет (родитель) / да | ПУСТО: только комментарий |
| L-SHELL-030 | Edit > Select All | `Select &All` | — | — | Designer:525; — | нет (родитель) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-031 | Data | `&Data` | — | — | Designer:531; — | да / да | контейнер (12 п.) |
| L-SHELL-032 | Data > Ammo | `&Ammo` | — | — | Designer:538; — | да / да | контейнер (2 п.) |
| L-SHELL-033 | Data > Ammo > Calibers | `&Calibers` | — | — | Designer:545; — | да / да | контейнер (8 п.) |
| L-SHELL-034 | Data > Ammo > Calibers > English | `&English` | — | EnglishToolStripMenuItem_Click | Designer:552; MainForm.vb:470 | да / да | ShowDataGridForm · Tables.AmmoStrings |
| L-SHELL-035 | Data > Ammo > Calibers > German | `&German` | — | GermanToolStripMenuItem_Click | Designer:558; MainForm.vb:474 | да / да | ShowDataGridForm · Tables.GermanAmmoStrings |
| L-SHELL-036 | Data > Ammo > Calibers > Russian | `&Russian` | — | RussianToolStripMenuItem_Click, PolishToolStripMenuItem_Click, FrenchToolStripMenuItem_Click | Designer:564; MainForm.vb:478, MainForm.vb:482, MainForm.vb:486 | да / да | ShowDataGridForm · Tables.RussianAmmoStrings |
| L-SHELL-037 | Data > Ammo > Calibers > Polish | `&Polish` | — | PolishToolStripMenuItem_Click, FrenchToolStripMenuItem_Click | Designer:570; MainForm.vb:482, MainForm.vb:486 | да / да | ShowDataGridForm · Tables.PolishAmmoStrings |
| L-SHELL-038 | Data > Ammo > Calibers > French | `&French` | — | FrenchToolStripMenuItem_Click | Designer:576; MainForm.vb:486 | да / да | ShowDataGridForm · Tables.FrenchAmmoStrings |
| L-SHELL-039 | Data > Ammo > Calibers > Italian | `&Italian` | — | ItalianToolStripMenuItem_Click | Designer:582; MainForm.vb:490 | да / да | ShowDataGridForm · Tables.ItalianAmmoStrings |
| L-SHELL-040 | Data > Ammo > Calibers > Dutch | `&Dutch` | — | DutchToolStripMenuItem_Click | Designer:588; MainForm.vb:494 | да / да | ShowDataGridForm · Tables.DutchAmmoStrings |
| L-SHELL-041 | Data > Ammo > Calibers > Chinese | `&Chinese` | — | ChineseToolStripMenuItem_Click | Designer:594; MainForm.vb:498 | да / да | ShowDataGridForm · Tables.ChineseAmmoStrings |
| L-SHELL-042 | Data > Ammo > Types | `&Types` | — | TypesToolStripMenuItem_Click | Designer:600; MainForm.vb:408 | да / да | ShowDataGridForm · Tables.AmmoTypes |
| L-SHELL-043 | Data > Explosions | `&Explosions` | — | ExplosionsToolStripMenuItem_Click | Designer:606; MainForm.vb:404 | да / да | ShowDataGridForm · Tables.ExplosionData |
| L-SHELL-044 | Data > Sounds | `&Sounds` | — | — | Designer:612; — | да / да | контейнер (2 п.) |
| L-SHELL-045 | Data > Sounds > Standard | `&Standard` | — | StandardToolStripMenuItem_Click | Designer:619; MainForm.vb:412 | да / да | ShowDataGridForm · Tables.Sounds |
| L-SHELL-046 | Data > Sounds > Burst | `&Burst` | — | BurstToolStripMenuItem_Click | Designer:625; MainForm.vb:416 | да / да | ShowDataGridForm · Tables.BurstSounds |
| L-SHELL-047 | Data > Lookup Tables | `&Lookup Tables` | — | — | Designer:631; — | да / да | контейнер (15 п.) |
| L-SHELL-048 | Data > Lookup Tables > Ammo Flags | `Amm&o Flags` | — | AmmoFlagsToolStripMenuItem_Click | Designer:638; MainForm.vb:756 | да / да | ShowLookupGridForm · Tables.AmmoFlags |
| L-SHELL-049 | Data > Lookup Tables > Armour Classes | `A&rmour Classes` | — | ArmourClassesToolStripMenuItem_Click | Designer:644; MainForm.vb:332 | да / да | ShowLookupGridForm · Tables.ArmourClasses |
| L-SHELL-050 | Data > Lookup Tables > Attachment System | `A&ttachment System` | — | AttachmentSystemToolStripMenuItem_Click | Designer:650; MainForm.vb:372 | да / да | ShowLookupGridForm · Tables.AttachmentSystem |
| L-SHELL-051 | Data > Lookup Tables > Built-in Attachment Classes | `Built-in &Attachment Classes` | — | AttachmentClassesToolStripMenuItem_Click | Designer:656; MainForm.vb:368 | да / да | ShowLookupGridForm · Tables.AttachmentClasses |
| L-SHELL-052 | Data > Lookup Tables > Cursors | `&Cursors` | — | CursorsToolStripMenuItem_Click | Designer:662; MainForm.vb:376 | да / да | ShowLookupGridForm · Tables.Cursors |
| L-SHELL-053 | Data > Lookup Tables > Drug Types | `&Drug Types` | — | DrugTypesToolStripMenuItem_Click | Designer:668; MainForm.vb:724 | да / да | ShowDataGridForm · Tables.DrugTypes |
| L-SHELL-054 | Data > Lookup Tables > Explosion Types | `&Explosion Types` | — | ExplosionTypesToolStripMenuItem_Click | Designer:674; MainForm.vb:380 | да / да | ShowLookupGridForm · Tables.ExplosionTypes |
| L-SHELL-055 | Data > Lookup Tables > Separability States | `Se&parability States` | — | SeparabilityStatesToolStripMenuItem_Click | Designer:680; MainForm.vb:736 | да / да | ShowLookupGridForm · Tables.SeparabilityStates |
| L-SHELL-056 | Data > Lookup Tables > Item Classes | `&Item Classes` | — | ItemClassesToolStripMenuItem_Click | Designer:686; MainForm.vb:384 | да / да | ShowDataGridForm · Tables.ItemClasses |
| L-SHELL-057 | Data > Lookup Tables > Item Flags | `Item &Flags` | — | ItemFlagsToolStripMenuItem_Click | Designer:692; MainForm.vb:752 | да / да | ShowLookupGridForm · Tables.ItemFlags |
| L-SHELL-058 | Data > Lookup Tables > LBE Classes | `&LBE Classes` | — | LBEClassesToolStripMenuItem_Click | Designer:698; MainForm.vb:336 | да / да | ShowLookupGridForm · Tables.LbeClasses |
| L-SHELL-059 | Data > Lookup Tables > Merge Types | `&Merge Types` | — | MergeTypesToolStripMenuItem_Click | Designer:704; MainForm.vb:388 | да / да | ShowLookupGridForm · Tables.MergeTypes |
| L-SHELL-060 | Data > Lookup Tables > Skill Checks | `&Skill Checks` | — | SkillChecksToolStripMenuItem_Click | Designer:710; MainForm.vb:392 | да / да | ShowLookupGridForm · Tables.SkillCheckTypes |
| L-SHELL-061 | Data > Lookup Tables > Weapon Classes | `&Weapon Classes` | — | WeaponClassesToolStripMenuItem_Click | Designer:716; MainForm.vb:396 | да / да | ShowLookupGridForm · Tables.WeaponClasses |
| L-SHELL-062 | Data > Lookup Tables > Weapon Types | `Weapon &Types` | — | WeaponTypesToolStripMenuItem_Click | Designer:722; MainForm.vb:400 | да / да | ShowLookupGridForm · Tables.WeaponTypes |
| L-SHELL-063 | Data > New Attachment System | `&New Attachment System` | — | — | Designer:728; — | да / да | контейнер (3 п.) |
| L-SHELL-064 | Data > New Attachment System > Custom Attachment Classes | `&Custom Attachment Classes` | — | NasAttachmentClassesToolStripMenuItem_Click | Designer:735; MainForm.vb:344 | да / да | ShowDataGridForm · Tables.NasAttachmentClasses |
| L-SHELL-065 | Data > New Attachment System > Attachment Slots | `Attachment &Slots` | — | NasAttachmentSlotsToolStripMenuItem_Click | Designer:741; MainForm.vb:424 | да / да | ShowDataGridForm · Tables.AttachmentSlots |
| L-SHELL-066 | Data > New Attachment System > Attachment Class Code Maker | `Attachment Class Code Maker` | Ctrl+Alt+Shift+C | AttachmentClassCodeMakerToolStripMenuItem_Click | Designer:747; MainForm.vb:352 | нет (Visible=False) / да | BitCodeMakerForm по AttachmentPoint (скрыт) |
| L-SHELL-067 | Data > Pocket Types | `&Pocket Types` | — | PocketTypesToolStripMenuItem_Click | Designer:755; MainForm.vb:420 | да / да | ShowDataGridForm · Tables.Pockets |
| L-SHELL-068 | Data > Inventory Silhouettes | `&Inventory Silhouettes` | — | InventorySilhouettesToolStripMenuItem_Click | Designer:761; MainForm.vb:340 | да / да | ShowDataGridForm · Tables.Silhouettes |
| L-SHELL-069 | Data > Common Attachment Points | `&Common Attachment Points` | — | CommonAttachmentPointsToolStripMenuItem_Click | Designer:767; MainForm.vb:348 | да / да | ShowDataGridForm · Tables.AttachmentPoints |
| L-SHELL-070 | Data > Attachment Point Code Maker | `Attachment Point Code Maker` | Ctrl+Alt+Shift+A | AttachmentPointCodeMakerToolStripMenuItem_Click | Designer:773; MainForm.vb:360 | нет (Visible=False) / да | BitCodeMakerForm по AttachmentPoint (скрыт) |
| L-SHELL-071 | Data > Food | `Food` | — | FoodToolStripMenuItem_Click | Designer:781; MainForm.vb:740 | да / да | ShowDataGridForm · Tables.Food |
| L-SHELL-072 | Data > Clothes | `Clothes` | — | ClothesToolStripMenuItem_Click | Designer:787; MainForm.vb:744 | да / да | ShowDataGridForm · Tables.Clothes |
| L-SHELL-073 | Data > Disease | `Disease` | — | DiseaseMenuItem_Click | Designer:1610; MainForm.vb:768 | да / да | ShowDataGridForm · Tables.Disease |
| L-SHELL-074 | Items | `&Items` | — | — | Designer:793; — | да / да | контейнер (8 п.) |
| L-SHELL-075 | Items > Show | `&Show` | — | — | Designer:800; — | да / да | контейнер (4 п.) |
| L-SHELL-076 | Items > Show > All | `&All` | Ctrl+Alt+A | AllItemsToolStripMenuItem_Click | Designer:807; MainForm.vb:244 | да / да | ItemGridForm · ITEM; фильтр `(нет)`; подтаблица — |
| L-SHELL-077 | Items > Show > By ID | `By &ID` | Ctrl+I | ToolStripMenuItem2_Click | Designer:814; MainForm.vb:428 | да / да | EnterItemIDForm: ввод ID, открытие ItemDataForm |
| L-SHELL-078 | Items > Show > By Class | `By &Class` | — | — | Designer:821; — | да / да | контейнер (15 п.) |
| L-SHELL-079 | Items > Show > By Class > Guns | `&Guns` | Alt+G | GunsToolStripMenuItem_Click | Designer:828; MainForm.vb:256 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=2 AND rocketlauncher=0`; подтаблица WEAPON |
| L-SHELL-080 | Items > Show > By Class > Ammo | `&Ammo` | Alt+A | AmmoToolStripMenuItem_Click | Designer:835; MainForm.vb:260 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=1024`; подтаблица MAGAZINE |
| L-SHELL-081 | Items > Show > By Class > Launchers | `&Launchers` | Alt+L | LaunchersToolStripMenuItem_Click | Designer:842; MainForm.vb:264 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=16 OR rocketlauncher=1`; подтаблица WEAPON |
| L-SHELL-082 | Items > Show > By Class > Grenades | `Gre&nades` | Alt+N | GrenadesToolStripMenuItem_Click | Designer:849; MainForm.vb:268 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=256`; подтаблица EXPLOSIVE |
| L-SHELL-083 | Items > Show > By Class > Explosives | `&Explosives` | Alt+E | ExplosivesToolStripMenuItem_Click | Designer:856; MainForm.vb:272 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=512`; подтаблица EXPLOSIVE |
| L-SHELL-084 | Items > Show > By Class > Knives | `&Knives` | Alt+K | KnivesToolStripMenuItem_Click | Designer:863; MainForm.vb:276 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=4 OR usItemClass=8`; подтаблица WEAPON |
| L-SHELL-085 | Items > Show > By Class > Other Weapons | `&Other Weapons` | Alt+O | OtherWeaponsToolStripMenuItem_Click | Designer:870; MainForm.vb:280 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=64 OR usItemClass=128 OR usItemClass=32`; подтаблица WEAPON |
| L-SHELL-086 | Items > Show > By Class > Armour | `A&rmour` | Alt+R | ArmourToolStripMenuItem_Click | Designer:877; MainForm.vb:284 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=2048`; подтаблица ARMOUR |
| L-SHELL-087 | Items > Show > By Class > Facial Gear | `&Facial Gear` | Alt+F | FaceToolStripMenuItem_Click | Designer:884; MainForm.vb:288 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=32768`; подтаблица — |
| L-SHELL-088 | Items > Show > By Class > Kits | `K&its` | Alt+I | KitsToolStripMenuItem_Click | Designer:891; MainForm.vb:292 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=4096 OR usItemClass=8192`; подтаблица — |
| L-SHELL-089 | Items > Show > By Class > Keys | `Ke&ys` | Alt+Y | KeysToolStripMenuItem_Click | Designer:898; MainForm.vb:296 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=65536`; подтаблица — |
| L-SHELL-090 | Items > Show > By Class > Load Bearing | `Load &Bearing` | Alt+B | LoadBearingToolStripMenuItem_Click | Designer:905; MainForm.vb:300 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=131072`; подтаблица — |
| L-SHELL-091 | Items > Show > By Class > Misc | `&Misc` | Alt+M | MiscToolStripMenuItem_Click | Designer:912; MainForm.vb:304 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=268435456 OR usItemClass=536870912`; подтаблица — |
| L-SHELL-092 | Items > Show > By Class > Empty/None | `Empty/&None` | Ctrl+Alt+N | NoneItemToolStripMenuItem_Click | Designer:919; MainForm.vb:308 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=1`; подтаблица — |
| L-SHELL-093 | Items > Show > By Class > Random Items | `Random Items` | — | RandomItemsToolStripMenuItem_Click | Designer:926; MainForm.vb:760 | да / да | ItemGridForm · ITEM; фильтр `usItemClass=1073741824`; подтаблица — |
| L-SHELL-094 | Items > Show > By Other Filters | `By &Other Filters` | — | — | Designer:932; — | да / да | контейнер (7 п.) |
| L-SHELL-095 | Items > Show > By Other Filters > Attachments | `&Attachments` | Alt+T | AttachmentsToolStripMenuItem_Click | Designer:939; MainForm.vb:312 | да / да | ItemGridForm · ITEM; фильтр `Attachment=1`; подтаблица — |
| L-SHELL-096 | Items > Show > By Other Filters >  | `` | — | DrugsItemFilterToolStripMenuItem_Click | Designer:946; MainForm.vb:732 | да / да | ItemGridForm · ITEM; фильтр `Medical=1`; подтаблица — |
| L-SHELL-097 | Items > Show > By Other Filters > Tons Of Guns | `&Tons Of Guns` | — | TonsOfGunsToolStripMenuItem_Click | Designer:951; MainForm.vb:316 | да / да | ItemGridForm · ITEM; фильтр `BigGunList=1`; подтаблица — |
| L-SHELL-098 | Items > Show > By Other Filters > Normal Guns | `&Normal Guns` | — | NormalGunsToolStripMenuItem_Click | Designer:957; MainForm.vb:320 | да / да | ItemGridForm · ITEM; фильтр `BigGunList=0`; подтаблица — |
| L-SHELL-099 | Items > Show > By Other Filters > Sci-Fi | `&Sci-Fi` | — | SciFiToolStripMenuItem_Click | Designer:963; MainForm.vb:324 | да / да | ItemGridForm · ITEM; фильтр `SciFi=1`; подтаблица — |
| L-SHELL-100 | Items > Show > By Other Filters > Realistic | `&Realistic` | — | NonSciFiToolStripMenuItem_Click | Designer:969; MainForm.vb:328 | да / да | ItemGridForm · ITEM; фильтр `SciFi=0`; подтаблица — |
| L-SHELL-101 | Items > Show > By Other Filters > Custom Filter | `&Custom Filter` | Ctrl+F | CustomFilterToolStripMenuItem_Click | Designer:975; MainForm.vb:590 | да / да | CustomFilterForm.ShowDialog; ItemGridForm ITEM с введённым фильтром |
| L-SHELL-102 | Items > New | `&New` | Ctrl+N | CreateNewToolStripMenuItem_Click | Designer:982; MainForm.vb:433 | да / да | NewItemForm: новая запись ITEM + ItemDataForm |
| L-SHELL-103 | Items > Merges | `&Merges` | — | — | Designer:994; — | да / да | контейнер (2 п.) |
| L-SHELL-104 | Items > Merges > Standard | `&Standard` | — | StandardToolStripMenuItem1_Click | Designer:1001; MainForm.vb:438 | да / да | ShowDataGridForm · Tables.Merges |
| L-SHELL-105 | Items > Merges > Attachment | `&Attachment` | — | AttachmentToolStripMenuItem_Click | Designer:1007; MainForm.vb:442 | да / да | ShowDataGridForm · Tables.AttachmentComboMerges |
| L-SHELL-106 | Items > Attachments | `&Attachments` | — | — | Designer:1013; — | да / да | контейнер (3 п.) |
| L-SHELL-107 | Items > Attachments > Full List | `Full &List` | — | ListingToolStripMenuItem_Click | Designer:1020; MainForm.vb:446 | да / да | ShowChildDataGridForm · Tables.Attachments |
| L-SHELL-108 | Items > Attachments > Info | `&Info` | — | InfoToolStripMenuItem_Click | Designer:1026; MainForm.vb:450 | да / да | ShowDataGridForm · Tables.AttachmentInfo |
| L-SHELL-109 | Items > Attachments > Incompatibilities | `&Incompatibilities` | — | IncompatibleToolStripMenuItem_Click | Designer:1032; MainForm.vb:454 | да / да | ShowChildDataGridForm · Tables.IncompatibleAttachments |
| L-SHELL-110 | Items > Launchables | `&Launchables` | — | LaunchablesToolStripMenuItem_Click | Designer:1038; MainForm.vb:458 | да / да | ShowChildDataGridForm · Tables.Launchables |
| L-SHELL-111 | Items > Compatible Face Items | `&Compatible Face Items` | — | CompatibleFaceItemsToolStripMenuItem_Click | Designer:1044; MainForm.vb:462 | да / да | ShowChildDataGridForm · Tables.CompatibleFaceItems |
| L-SHELL-112 | Items > Transformations | `&Transformations` | — | TransformationsToolStripMenuItem_Click | Designer:1050; MainForm.vb:466 | да / да | ShowChildDataGridForm · Tables.ItemTransformations |
| L-SHELL-113 | Items > Random Items | `Random Items` | — | RandToolStripMenuItem_Click | Designer:1056; MainForm.vb:748 | да / да | ShowDataGridForm · Tables.RandomItems |
| L-SHELL-114 | Inventories | `I&nventories` | — | — | Designer:1062; — | да / да | контейнер (6 п.) |
| L-SHELL-115 | Inventories > IMP Items | `&IMP Items` | — | IMPItemsToolStripMenuItem_Click | Designer:1069; MainForm.vb:582 | да / да | ShowLookupGridForm · Tables.IMPItems |
| L-SHELL-116 | Inventories > Enemy Items | `&Enemy Items` | — | — | Designer:1075; — | да / да | контейнер (3 п.) |
| L-SHELL-117 | Inventories > Enemy Items > Guns | `&Guns` | — | — | Designer:1082; — | да / да | контейнер (4 п.) |
| L-SHELL-118 | Inventories > Enemy Items > Guns > Admin | `Admin` | — | EnemyGunsAdminToolStripMenuItem_Click | Designer:1089; MainForm.vb:598 | да / да | ShowLookupGridForm · Tables.EnemyGunsAdmin |
| L-SHELL-119 | Inventories > Enemy Items > Guns > Regular | `Regular` | — | EnemyGunsRegularToolStripMenuItem_Click | Designer:1095; MainForm.vb:601 | да / да | ShowLookupGridForm · Tables.EnemyGunsRegular |
| L-SHELL-120 | Inventories > Enemy Items > Guns > Elite | `Elite` | — | EnemyGunsEliteToolStripMenuItem_Click | Designer:1101; MainForm.vb:604 | да / да | ShowLookupGridForm · Tables.EnemyGunsElite |
| L-SHELL-121 | Inventories > Enemy Items > Guns > Default | `Default` | — | EnemyGunsDefaultToolStripMenuItem_Click | Designer:1107; MainForm.vb:595 | да / да | ShowLookupGridForm · Tables.EnemyGuns |
| L-SHELL-122 | Inventories > Enemy Items > Items | `&Items` | — | — | Designer:1113; — | да / да | контейнер (4 п.) |
| L-SHELL-123 | Inventories > Enemy Items > Items > Admin | `Admin` | — | EnemyItemsAdminToolStripMenuItem1_Click | Designer:1120; MainForm.vb:610 | да / да | ShowLookupGridForm · Tables.EnemyItemsAdmin |
| L-SHELL-124 | Inventories > Enemy Items > Items > Regular | `Regular` | — | EnemyItemsRegularToolStripMenuItem1_Click | Designer:1126; MainForm.vb:613 | да / да | ShowLookupGridForm · Tables.EnemyItemsRegular |
| L-SHELL-125 | Inventories > Enemy Items > Items > Elite | `Elite` | — | EnemyItemsEliteToolStripMenuItem1_Click | Designer:1132; MainForm.vb:616 | да / да | ShowLookupGridForm · Tables.EnemyItemsElite |
| L-SHELL-126 | Inventories > Enemy Items > Items > Default | `Default` | — | EnemyItemsDefaultToolStripMenuItem_Click | Designer:1138; MainForm.vb:607 | да / да | ShowLookupGridForm · Tables.EnemyItems |
| L-SHELL-127 | Inventories > Enemy Items > Ammo | `&Ammo` | — | EnemyAmmoToolStripMenuItem_Click | Designer:1144; MainForm.vb:619 | да / да | ShowLookupGridForm · Tables.EnemyAmmo |
| L-SHELL-128 | Inventories > Militia Items | `Militia Items` | — | — | Designer:1150; — | да / да | контейнер (2 п.) |
| L-SHELL-129 | Inventories > Militia Items > Guns | `Guns` | — | — | Designer:1157; — | да / да | контейнер (3 п.) |
| L-SHELL-130 | Inventories > Militia Items > Guns > Green | `Green` | — | MilitiaGunsGreenToolStripMenuItem_Click | Designer:1164; MainForm.vb:642 | да / да | ShowLookupGridForm · Tables.MilitiaGunsGreen |
| L-SHELL-131 | Inventories > Militia Items > Guns > Regular | `Regular` | — | MilitiaGunsRegularToolStripMenuItem_Click | Designer:1170; MainForm.vb:646 | да / да | ShowLookupGridForm · Tables.MilitiaGunsRegular |
| L-SHELL-132 | Inventories > Militia Items > Guns > Elite | `Elite` | — | MilitiaGunsEliteToolStripMenuItem_Click | Designer:1176; MainForm.vb:650 | да / да | ShowLookupGridForm · Tables.MilitiaGunsElite |
| L-SHELL-133 | Inventories > Militia Items > Items | `Items` | — | — | Designer:1182; — | да / да | контейнер (3 п.) |
| L-SHELL-134 | Inventories > Militia Items > Items > Green | `Green` | — | MilitiaItemsGreenToolStripMenuItem_Click | Designer:1189; MainForm.vb:654 | да / да | ShowLookupGridForm · Tables.MilitiaItemsGreen |
| L-SHELL-135 | Inventories > Militia Items > Items > Regular | `Regular` | — | MilitiaItemsRegularToolStripMenuItem_Click | Designer:1195; MainForm.vb:658 | да / да | ShowLookupGridForm · Tables.MilitiaItemsRegular |
| L-SHELL-136 | Inventories > Militia Items > Items > Elite | `Elite` | — | MilitiaItemsEliteToolStripMenuItem_Click | Designer:1201; MainForm.vb:662 | да / да | ShowLookupGridForm · Tables.MilitiaItemsElite |
| L-SHELL-137 | Inventories > Drop Rates | `&Drop Rates` | — | — | Designer:1207; — | да / да | контейнер (5 п.) |
| L-SHELL-138 | Inventories > Drop Rates > Weapons | `&Weapons` | — | EnemyWeaponDropRateToolStripMenuItem_Click | Designer:1214; MainForm.vb:623 | да / да | ShowLookupGridForm · Tables.EnemyWeaponDrops |
| L-SHELL-139 | Inventories > Drop Rates > Ammo | `&Ammo` | — | EnemyAmmoDropRateToolStripMenuItem_Click | Designer:1220; MainForm.vb:627 | да / да | ShowDataGridForm · Tables.EnemyAmmoDrops |
| L-SHELL-140 | Inventories > Drop Rates > Armour | `A&rmour` | — | EnemyArmourDropRateToolStripMenuItem_Click | Designer:1226; MainForm.vb:631 | да / да | ShowLookupGridForm · Tables.EnemyArmourDrops |
| L-SHELL-141 | Inventories > Drop Rates > Explosives | `&Explosives` | — | EnemyExplosiveDropRateToolStripMenuItem_Click | Designer:1232; MainForm.vb:635 | да / да | ShowLookupGridForm · Tables.EnemyExplosiveDrops |
| L-SHELL-142 | Inventories > Drop Rates > Misc | `&Misc` | — | EnemyMiscDropRateToolStripMenuItem_Click | Designer:1238; MainForm.vb:639 | да / да | ShowLookupGridForm · Tables.EnemyMiscDrops |
| L-SHELL-143 | Inventories > Shopkeepers | `&Shopkeepers` | — | — | Designer:1244; — | да / да | контейнер (19 п.) |
| L-SHELL-144 | Inventories > Shopkeepers > Alberto | `&Alberto` | — | AlbertoToolStripMenuItem_Click | Designer:1251; MainForm.vb:506 | да / да | ShowDataGridForm · Tables.AlbertoInventory |
| L-SHELL-145 | Inventories > Shopkeepers > Arnie | `&Arnie` | — | ArnieToolStripMenuItem_Click | Designer:1257; MainForm.vb:510 | да / да | ShowDataGridForm · Tables.ArnieInventory |
| L-SHELL-146 | Inventories > Shopkeepers > Carlo | `&Carlo` | — | CarloToolStripMenuItem_Click | Designer:1263; MainForm.vb:514 | да / да | ShowDataGridForm · Tables.CarloInventory |
| L-SHELL-147 | Inventories > Shopkeepers > Devin | `&Devin` | — | DevinToolStripMenuItem_Click | Designer:1269; MainForm.vb:518 | да / да | ShowDataGridForm · Tables.DevinInventory |
| L-SHELL-148 | Inventories > Shopkeepers > Elgin | `&Elgin` | — | ElginToolStripMenuItem_Click | Designer:1275; MainForm.vb:522 | да / да | ShowDataGridForm · Tables.ElginInventory |
| L-SHELL-149 | Inventories > Shopkeepers > Frank | `&Frank` | — | FrankToolStripMenuItem_Click | Designer:1281; MainForm.vb:526 | да / да | ShowDataGridForm · Tables.FrankInventory |
| L-SHELL-150 | Inventories > Shopkeepers > Franz | `&Franz` | — | FranzToolStripMenuItem_Click | Designer:1287; MainForm.vb:530 | да / да | ShowDataGridForm · Tables.FranzInventory |
| L-SHELL-151 | Inventories > Shopkeepers > Fredo | `&Fredo` | — | FredoToolStripMenuItem_Click | Designer:1293; MainForm.vb:534 | да / да | ShowDataGridForm · Tables.FredoInventory |
| L-SHELL-152 | Inventories > Shopkeepers > Gabby | `&Gabby` | — | GabbyToolStripMenuItem_Click | Designer:1299; MainForm.vb:538 | да / да | ShowDataGridForm · Tables.GabbyInventory |
| L-SHELL-153 | Inventories > Shopkeepers > Herve | `&Herve` | — | HerveToolStripMenuItem_Click | Designer:1305; MainForm.vb:542 | да / да | ShowDataGridForm · Tables.HerveInventory |
| L-SHELL-154 | Inventories > Shopkeepers > Howard | `&Howard` | — | HowardToolStripMenuItem_Click | Designer:1311; MainForm.vb:546 | да / да | ShowDataGridForm · Tables.HowardInventory |
| L-SHELL-155 | Inventories > Shopkeepers > Jake | `&Jake` | — | JakeToolStripMenuItem_Click | Designer:1317; MainForm.vb:550 | да / да | ShowDataGridForm · Tables.JakeInventory |
| L-SHELL-156 | Inventories > Shopkeepers > Keith | `&Keith` | — | KeithToolStripMenuItem_Click | Designer:1323; MainForm.vb:554 | да / да | ShowDataGridForm · Tables.KeithInventory |
| L-SHELL-157 | Inventories > Shopkeepers > Manny | `&Manny` | — | MannyToolStripMenuItem_Click | Designer:1329; MainForm.vb:558 | да / да | ShowDataGridForm · Tables.MannyInventory |
| L-SHELL-158 | Inventories > Shopkeepers > Mickey | `&Mickey` | — | MickeyToolStripMenuItem_Click | Designer:1335; MainForm.vb:562 | да / да | ShowDataGridForm · Tables.MickeyInventory |
| L-SHELL-159 | Inventories > Shopkeepers > Perko | `&Perko` | — | PerkoToolStripMenuItem_Click | Designer:1341; MainForm.vb:566 | да / да | ShowDataGridForm · Tables.PerkoInventory |
| L-SHELL-160 | Inventories > Shopkeepers > Peter | `&Peter` | — | PeterToolStripMenuItem_Click | Designer:1347; MainForm.vb:570 | да / да | ShowDataGridForm · Tables.PeterInventory |
| L-SHELL-161 | Inventories > Shopkeepers > Sam | `&Sam` | — | SamToolStripMenuItem_Click | Designer:1353; MainForm.vb:574 | да / да | ShowDataGridForm · Tables.SamInventory |
| L-SHELL-162 | Inventories > Shopkeepers > Tony | `&Tony` | — | TonyToolStripMenuItem_Click | Designer:1359; MainForm.vb:578 | да / да | ShowDataGridForm · Tables.TonyInventory |
| L-SHELL-163 | Inventories > Merc Starting Gear | `&Merc Starting Gear` | — | MercStartingGearToolStripMenuItem_Click | Designer:1365; MainForm.vb:586 | да / да | ShowMercGearGridForm · MERCGEAR |
| L-SHELL-164 | Windows | `&Windows` | — | — | Designer:1371; — | да / да | контейнер (6 п.) |
| L-SHELL-165 | Windows > New Window | `&New Window` | — | — | Designer:1378; — | нет (Visible=False) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-166 | Windows > Cascade | `&Cascade` | — | CascadeToolStripMenuItem_Click | Designer:1385; MainForm.vb:175 | да / да | LayoutMdi(Cascade) |
| L-SHELL-167 | Windows > Tile Vertical | `Tile &Vertical` | — | TileVerticleToolStripMenuItem_Click | Designer:1391; MainForm.vb:179 | да / да | LayoutMdi(TileVertical) |
| L-SHELL-168 | Windows > Tile Horizontal | `Tile &Horizontal` | — | TileHorizontalToolStripMenuItem_Click | Designer:1397; MainForm.vb:183 | да / да | LayoutMdi(TileHorizontal) |
| L-SHELL-169 | Windows > Close All | `C&lose All` | — | CloseAllToolStripMenuItem_Click | Designer:1403; MainForm.vb:191 | да / да | Закрыть все MDI-дети без запроса |
| L-SHELL-170 | Windows > Arrange Icons | `&Arrange Icons` | — | ArrangeIconsToolStripMenuItem_Click | Designer:1409; MainForm.vb:187 | да / да | LayoutMdi(ArrangeIcons) |
| L-SHELL-171 | Profiles | `&Profiles` | — | — | Designer:1415; — | нет (Visible=False) / да | контейнер (2 п.) |
| L-SHELL-172 | Profiles > Show | `&Show` | — | — | Designer:1423; — | нет (родитель) / да | контейнер (1 п.) |
| L-SHELL-173 | Profiles > Show > All | `&All` | — | — | Designer:1430; — | нет (родитель) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-174 | Profiles > New | `&New` | — | — | Designer:1436; — | нет (родитель) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-175 | Maps | `&Maps` | — | — | Designer:1442; — | нет (Visible=False) / да | контейнер (4 п.) |
| L-SHELL-176 | Maps > Alternate Sectors | `&Alternate Sectors` | — | — | Designer:1450; — | нет (родитель) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-177 | Maps > Cities | `&Cities` | — | — | Designer:1456; — | нет (родитель) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-178 | Maps > SAM Sites | `&SAM Sites` | — | — | Designer:1462; — | нет (родитель) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-179 | Maps > Movement Costs | `&Movement Costs` | — | — | Designer:1468; — | нет (родитель) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-180 | Tools | `&Tools` | — | — | Designer:1474; — | нет (Visible=False) / да | контейнер (1 п.) |
| L-SHELL-181 | Tools > Options | `&Options` | — | — | Designer:1482; — | нет (родитель) / да | НЕТ ОБРАБОТЧИКА |
| L-SHELL-182 | Help | `&Help` | — | — | Designer:1488; — | да / да | контейнер (1 п.) |
| L-SHELL-183 | Help > About ... | `&About ...` | — | AboutToolStripMenuItem_Click | Designer:1500; MainForm.vb:764 | да / да | AboutForm.Visible = True (default instance, немодально) |

### Скрытые, недоступные и мёртвые пункты (явный перечень)

- **`Visible = False` (10):** `PrintToolStripMenuItem` (Ctrl+P), `PrintPreviewToolStripMenuItem`, `PrintSetupToolStripMenuItem`, `EditMenu`, `AttachmentClassCodeMakerToolStripMenuItem` (Ctrl+Alt+Shift+C), `AttachmentPointCodeMakerToolStripMenuItem` (Ctrl+Alt+Shift+A), `NewWindowToolStripMenuItem`, `ProfilesMenu`, `MapsMenu`, `ToolsMenu`.
- **Недоступны из-за скрытого родителя (ещё 14: 1 контейнер + 13 листовых):** контейнер `ShowToolStripMenuItem1` (Profiles > Show); листовые `UndoToolStripMenuItem`, `RedoToolStripMenuItem`, `CutToolStripMenuItem`, `CopyToolStripMenuItem`, `PasteToolStripMenuItem`, `SelectAllToolStripMenuItem` (родитель `Edit`); `AllToolStripMenuItem`, `NewToolStripMenuItem` (родитель `Profiles`); `AlternateSectorsToolStripMenuItem`, `CitiesToolStripMenuItem`, `SAMSitesToolStripMenuItem`, `MovementCostsToolStripMenuItem` (родитель `Maps`); `OptionsToolStripMenuItem` (родитель `Tools`). Итоговые 24 недоступных = 5 контейнеров + 19 листовых.
- **`Enabled = False`:** ни одного пункта меню; ни один пункт не отключается в коде.
- **Нет обработчика (14 листовых):** `Print`, `Print Preview`, `Print Setup`, `Undo`, `Redo`, `Select All`, `New Window`, `All` (Profiles), `New` (Profiles), `Alternate Sectors`, `Cities`, `SAM Sites`, `Movement Costs`, `Options`.
- **Пустые обработчики (3):** `Cut`, `Copy`, `Paste` — тела содержат только комментарии (MainForm.vb:155-165).
- **Скрытые, но с рабочим обработчиком (2):** оба code maker-а (L-SHELL-066, L-SHELL-070); горячие клавиши скрытых пунктов в WinForms не срабатывают, поэтому команды фактически недоступны.
- **Тулбар:** `Print`, `Print Preview`, `Help` скрыты и не имеют обработчиков; `ToolStripSeparator1/2` скрыты.
- **Все 18 присваиваний `Visible = False`:** 10 пунктов меню + 3 разделителя меню (`ToolStripSeparator3/4/5`, Designer:369, 393, 425) + 2 разделителя тулбара + 3 кнопки тулбара.
- **Мульти-`Handles` у языковых калибров (L-SHELL-036/037):** клик по `Russian` вызывает три обработчика (Russian, Polish, French), по `Polish` — два. Это legacy-дефект копирования `Handles`; appendix фиксирует «один handler = одна цель», а не «один пункт = одна цель».
- **Языковые калибры (L-SHELL-035..041) условны:** пункты German…Chinese удаляются при загрузке, если нет `<Language>.AmmoStrings.xml`; в v4 остаются English и Russian, в v5 — только English.

### Toolbar and status bar

| ID | Surface | Handler | Legacy file:line | Behaviour | Notes |
|---|---|---|---|---|---|
| L-SHELL-184 | Toolbar `OpenToolStripButton` («Reload», tooltip «Reload All Data») | `OpenFile` | Designer:1516; MainForm.vb:120 | То же, что File > Reload: `LoadData()` для всех наборов, Splash, скрытие/показ главного окна | `DisplayStyle = Image` |
| L-SHELL-185 | Toolbar `SaveToolStripButton` («Save», tooltip «Save Active Data Set») | `SaveToolStripMenuItem_Click` | Designer:1526; MainForm.vb:689 | `Save(False)` | `DisplayStyle = Image` |
| L-SHELL-186 | Toolbar `SaveAllToolStripButton` («Save», tooltip «Save All Data Sets») | `SaveAllToolStripMenuItem_Click` | Designer:1536; MainForm.vb:693 | `Save(True)` | Две кнопки с одинаковым `Text = "Save"` |
| L-SHELL-187 | Toolbar `PrintToolStripButton` | — | Designer:1552 | НЕТ ОБРАБОТЧИКА; `Visible = False` | Скрыта |
| L-SHELL-188 | Toolbar `PrintPreviewToolStripButton` | — | Designer:1562 | НЕТ ОБРАБОТЧИКА; `Visible = False` | Скрыта |
| L-SHELL-189 | Toolbar `HelpToolStripButton` | — | Designer:1578 | НЕТ ОБРАБОТЧИКА; `Visible = False` | Скрыта |
| L-SHELL-190 | Разделители тулбара `ToolStripSeparator1`, `ToolStripSeparator2` | — | Designer:1546, 1572 | `Visible = False` | Оба скрыты |
| L-SHELL-191 | `StatusLabel` (`ToolStripStatusLabel`) | — | Designer:1599 | Текст меняют формы: `MainForm_Load` — `DisplayText.Welcome`; `BaseDataForm.UpdateStatusBar` — «Editing …»; `GridForm.UpdateStatusBar` — «форма: N of M Rows»; при `VisibleChanged` скрытой формы текст очищается | Клика нет; второй `ToolStripStatusLabel` объявлен в дизайнере, но не добавлен в `StatusStrip.Items` (Designer:1604) |

### Startup, shutdown, dataset selection

| ID | Surface | Handler | Legacy file:line | Behaviour | Notes |
|---|---|---|---|---|---|
| L-SHELL-192 | `Main()` — точка входа (`StartupObject = GUI.Main`) | `Main` | Main.vb:16 | `CheckForDuplicateProcess`; культура `en-US`; `IniFile.ReadFile("XMLEditorInit.xml")`; при смене Major/Minor версии `useWorkingDir = False`; Splash; для каждого `Data_Directory_N` создаёт `DataManager`, `CreateDatabase()`, `LoadImages()`, `LoadData()`; `SettingsUtility.LoadSettings()`; `Application.Run(MainForm)`; `SettingsUtility.SaveSettings()` после выхода | Исключение ловится и показывается `ErrorHandler.ShowError` |
| L-SHELL-193 | `DB_AfterLoadAll` / `DB_LoadingTable` | `AddHandler` в `Main` | Main.vb:50-51, 83-90 | Обновляют текст Splash при загрузке таблиц | Только индикация |
| L-SHELL-194 | `MainForm_Load` | `MainForm_Load` | Designer:1626 (`IsMdiContainer=True`); MainForm.vb:7 | Заголовок `DisplayText.MainFormTitle` + имя набора; `RemoveUnusedLanguageSpecificAmmoStringMenuItems()`; восстановление `View_Background`, `View_Toolbar`, `View_StatusBar`, размеров/позиции/состояния; скрытие DataN с `Tag > GameDataCount-1`, подстановка имени набора в `{0}`; поиск `MdiClient`, фон DESKTOP; `StatusLabel = DisplayText.Welcome` | Пункт `Data1` по умолчанию `Checked` |
| L-SHELL-195 | `MainForm_FormClosing` | `MainForm_FormClosing` | MainForm.vb:84 | Сохраняет в `My.Settings` (`View_*`, `MainForm_Location`, `MainForm_Size`, `MainForm_State`) и вызывает `.Save()` | Запроса о несохранённых данных нет |
| L-SHELL-196 | `Save(saveAll)` | `Save` | MainForm.vb:697 | `LoadingForm.Show(True)`; `SaveData()` активного или всех наборов; при `saveAll OrElse GameDataCount = 1` пишет `Last_Version_Major/Minor`; закрывает LoadingForm | Вызывается только из Save/Save All |
| L-SHELL-197 | `RemoveUnusedLanguageSpecificAmmoStringMenuItems` + `LanguageSpecificFileExist` | `MainForm_Load` | MainForm.vb:45, 73 | Удаляет из Data > Ammo > Calibers пункты языков, для которых нет `<TableDirectory>\Items\<Language>.AmmoStrings.xml` | Проверка идёт только для `_activeDataSet` на момент `Load`; при смене набора меню не перестраивается |
| L-SHELL-198 | Выбор набора (File > Active Data Set > 1…10) | `DataToolStripMenuItem_Click` | MainForm.vb:674; Designer:264-347 | `_activeDataSet = GameData(Tag)`; заголовок окна; `Checked` остаётся только у выбранного | Открытые MDI-окна предыдущего набора не закрываются и продолжают работать со своим `DataManager` |
| L-SHELL-199 | `OpenFile` (Reload) | `OpenFile` | MainForm.vb:120 | Wait cursor, Splash, `Me.Hide()`, `Application.DoEvents()`, `GameData(i).LoadData()` для всех, `Me.Show()` | Нет сохранения и нет подтверждения; открытые формы не закрываются |
| L-SHELL-200 | `FormOpen` / `ShowForm` | `Friend` методы | MainForm.vb:135, 145 | `FormOpen` ищет MDI-ребёнка по точному `Text` и активирует его; `ShowForm` ставит `MdiParent`, подписывается на `Resize` и показывает форму | Дедупликация только по заголовку; повторный вызов с другим фильтром не обновляет уже открытую форму |
| L-SHELL-201 | `client_Paint` / `client_Resize` / `ChildForm_Resized` | `Handles _client.Paint`, `_client.Resize`, `AddHandler frm.Resize` | MainForm.vb:99, 112, 116 | Рисует фоновое изображение MDI-клиента и инвалидирует его при изменении размеров | В `client_Paint` ветка `Else` недостижима: `Static flag` никогда не становится `True` |

## Forms

### Grid form family

| Form | Inheritance / file | Grid mode at open | Add / Delete rows | Context menu | Double-click | Notes |
|---|---|---|---|---|---|---|
| `GridForm` | base, `GridForm.vb` | `ReadOnly = True`, `FullRowSelect`, `RowHeadersVisible = False` → в конструкторе не меняется | `AllowUserToAddRows = False`, `AllowUserToDeleteRows = False` (Designer:45-46) | 6 пунктов: Select Columns, Custom Filter, Filter by Selected Value, Filter Excluding Selected Value, Remove Filter, Apply Value to Selected Column | нет | `AllowDrop = True`, `AllowUserToOrderColumns = True`, `ClipboardCopyMode = EnableWithoutHeaderText`; размеры/позиция/состояние и ширины колонок сохраняются |
| `ItemGridForm` | `GridForm`, `ItemGridForm.vb` | наследует `ReadOnly = True`, `FullRowSelect`; `AllowUserToDeleteRows = True`; высота строки = `GreatestBigImageHeight` | delete: да после `Enable Editing` (через `Database.DeleteRow`); add: нет | базовые 6 + `View Details`, `New`, `Duplicate`, `Enable Editing` (добавляются в конструкторе, ItemGridForm.vb:26-32) | `Grid.CellMouseDoubleClick` → `ItemDataForm.Open` | `Enable Editing` переключает `ReadOnly` и `SelectionMode` (`RowHeaderSelect` ↔ `FullRowSelect`) и вызывает `EnableContextMenuItems` |
| `DataGridForm` | `GridForm`, `DataGridForm.vb` | `ReadOnly = False`, `RowHeaderSelect`, `RowHeadersVisible = True` | add: да; delete: да | базовые 6 | нет | `Grid.DefaultValuesNeeded` подставляет следующий PK только для таблицы с одним ключом |
| `LookupGridForm` | `GridForm`, `LookupGridForm.vb` | `ReadOnly = False`, `CellSelect`, `RowHeadersVisible = True` | add: нет; delete: нет | базовые 6 | нет | `EnableContextMenuItems` вызывается после смены режима |
| `ChildDataGridForm` | `LookupGridForm`, `ChildDataGridForm.vb` | `ReadOnly = False`, `RowHeaderSelect` | add: да; delete: да | базовые 6 | нет | Меняет только флаги добавления/удаления и режим выделения |
| `MercGearGridForm` | `GridForm`, `MercGearGridForm.vb` | `ReadOnly = True`, `FullRowSelect` | add: нет; delete: нет | базовые 6 + `View Details` (MercGearGridForm.vb:21) | `Grid.CellMouseDoubleClick` → `MercGearDataForm.Open` | `EnableContextMenuItems` скрывает три пункта, зависящих от редактируемого выделения |

### Dialogs and auxiliary forms

| ID | Surface | Handler / entry | Legacy file:line | Behaviour | Notes |
|---|---|---|---|---|---|
| L-AUX-001 | `SimpleFormBase` — OK | `OKButton_Click` | SimpleFormBase.vb:20; Designer:45 | `If OkAction() Then Close()`; `AcceptButton = btnOK` | Базовый класс для ChangeClass/ColumnSelect/CustomFilter/EnterItemID/NewItem |
| L-AUX-002 | `SimpleFormBase` — Cancel | `CancelButton_Click` | SimpleFormBase.vb:16; Designer:34 | `Close()` | Без подтверждения |
| L-AUX-003 | `SimpleFormBase` — Load | `SimpleFormBase_Load` | SimpleFormBase.vb:28 | `WindowState = Normal` | |
| L-AUX-004 | `BitCodeMakerForm` — Add To Table | `AddButton_Click` | BitCodeMakerForm.vb:72; Designer:72 | `AddCode()`: если Code и Name непусты и кода ещё нет — новая строка в исходной таблице, очистка полей, перестроение чекбоксов | Нет `AcceptChanges`/сохранения; таблица помечается изменённой |
| L-AUX-005 | `BitCodeMakerForm` — Close | `CloseButton_Click` | BitCodeMakerForm.vb:92; Designer:44 | `Me.Close()` | |
| L-AUX-006 | `BitCodeMakerForm` — динамические флаги | `AddHandler cb.CheckStateChanged` | BitCodeMakerForm.vb:50 | `Checkbox_Checked`: прибавляет/вычитает `Tag` из `CodeTextBox.Text`, без пересчёта из уже введённого кода | Кнопки создаются по строкам `AttachmentPoint` с `row(0) > 0`, группируются по первому слову `row(1)` |
| L-AUX-007 | `BitCodeMakerForm` — построение UI | `BuildCheckBoxes`, `CreateCheckBox` | BitCodeMakerForm.vb:20, 48 | Создаёт `CheckBox`-ы в `CheckBoxPanel` по таблице-источнику | Удалять/редактировать существующие коды нельзя |
| L-AUX-008 | `ChangeClassForm` — OK | `OkAction` override | ChangeClassForm.vb:35; Designer:80 | Записывает `usItemClass` в текущую строку, `EndEdit()`, закрывает исходный `ItemDataForm` и открывает новый на той же позиции | Пересоздание формы, а не обновление; `_view = Nothing` перед открытием |
| L-AUX-009 | `ChangeClassForm` — список классов | `ItemClassListBox` (`DataSource = Tables.ItemClasses`) | ChangeClassForm.vb:26-31 | `DisplayMember = name`, `ValueMember = id`, начальное значение = текущий класс | |
| L-AUX-010 | `ColumnSelectForm` — OK | `OkAction` override | ColumnSelectForm.vb:29 | `SetColumnProperties()`; `Database.SaveSchema()` | Видимость колонок хранится в схеме (`Grid_Hidden`), а не в настройках |
| L-AUX-011 | `ColumnSelectForm` — список | `GetColumnProperties` | ColumnSelectForm.vb:36 | Показывает колонки, у которых нет `SubTable`, либо колонки выбранной подтаблицы; `Checked = Not Grid_Hidden` | Для подтаблиц свойство пишется и в основную, и в подтаблицу |
| L-AUX-012 | `CustomFilterForm` — OK | `OkAction` override | CustomFilterForm.vb:19; Designer:69 | Всегда `True`; фильтр читается из `FilterTextBox.Text` | Валидация выражения выполняется позже в `GridForm.Filter` |
| L-AUX-013 | `EnterItemIDForm` — OK | `OkAction` override | EnterItemIDForm.vb:21; Designer:68 | Проверяет числовой ID, ищет `ITEM` по PK; при ошибке `ShowWarning` и `False`; иначе `ItemDataForm.Open` | Заголовок формы «Enter Item ID» |
| L-AUX-014 | `NewItemForm` — OK | `OkAction` override | NewItemForm.vb:25; Designer:105 | Требует класс и имя; `GetNextPrimaryKeyValue(ITEM)`, новая строка (ID, usItemClass, szLongItemName, ItemImage = `BigItemImage(0,0)`), `Rows.Add`, `ItemDataForm.Open` | При исключении `ShowError("Unable to add new item.")` |
| L-AUX-015 | `NewItemForm` — список классов | `ItemClassComboBox` | NewItemForm.vb:18-22 | `DataSource = Tables.ItemClasses`, `DisplayMember = name`, `ValueMember = id` | |
| L-AUX-016 | `LoadingForm` — показ | `Shadows Show(saving)` | LoadingForm.vb:4 | Меняет `InfoLabel` на `DisplayText.Saving`/`Loading`, затем `MyBase.Show()` | Немодально, без владельца |
| L-AUX-017 | `LoadingForm` — прогресс | `SetDataName` | LoadingForm.vb:14 | «Saving/Loading <dataName>» + `Application.DoEvents()` | Вызывается из `MainForm.Save` |
| L-AUX-018 | `AboutForm` — Load | `AboutBox1_Load` | AboutForm.vb:3; Designer:171 | Заголовок `DisplayText.About`; ProductName, Version, Copyright, CompanyName, Description из `My.Application.Info` | Открывается как default instance, не MDI-child |
| L-AUX-019 | `AboutForm` — OK | `OKButton_Click` | AboutForm.vb:22; Designer:154 | `Close()` | |
| L-AUX-020 | `SplashForm` — Load | `SplashScreen1_Load` | SplashForm.vb:6 | ApplicationTitle из `My.Application.Info.Title` или имени сборки; `Version` = `{0}.{1}`; пустые DataDirectoryLabel и Loading | `FormBorderStyle = None`, `ShowInTaskbar = False` |
| L-AUX-021 | `SplashForm` — текст загрузки | `UpdateLoadingText` | SplashForm.vb:35 | Устанавливает `Loading.Text` + `DoEvents()` | Вызывается из `Main` и `OpenFile` |
| L-AUX-022 | `SplashForm` — текущий каталог | `UpdateCurrentDirectory` | SplashForm.vb:40 | `DisplayText.CurrentDirectorySplash` + каталог без завершающего `\` | |

### BaseDataForm (общая база для ItemDataForm и MercGearDataForm)

| ID | Surface | Handler | Legacy file:line | Behaviour | Notes |
|---|---|---|---|---|---|
| L-AUX-023 | Apply | `ApplyButton_Click` | BaseDataForm.vb:117; Designer:52 | `CommitData()`; при успехе `ApplyButtonClicked()` и снова `BeginEdit()` | Не сохраняет на диск |
| L-AUX-024 | OK | `OKButton_Click` | BaseDataForm.vb:127; Designer:63 | `CommitData()`; `OKButtonClicked()`; `Close()`; `_view.Dispose()` | `DialogResult` не выставляется, поэтому `FormClosing` после OK тоже вызывает `CancelButtonClicked` + `CancelData` |
| L-AUX-025 | Cancel | `CancelButton_Click` | BaseDataForm.vb:138; Designer:42 | `CancelButtonClicked()`; `CancelData()`; `Close()` | |
| L-AUX-026 | FormClosing | `DataForm_FormClosing` | BaseDataForm.vb:144 | Если `CloseReason = UserClosing` и `DialogResult <> OK` — `CancelButtonClicked()` + `CancelData()` | Срабатывает и после OK, и после Cancel; при Cancel сценарий отмены вызывается повторно (кнопка + FormClosing) |
| L-AUX-027 | Activated | `DataForm_Activated` | BaseDataForm.vb:155 | `UpdateStatusBar()` → `MainWindow.StatusLabel.Text = "Editing " & Me.Text` | |
| L-AUX-028 | Load | `DataForm_Load` | BaseDataForm.vb:159 | `WindowState = Normal`; `LoadingForm.Close()` | Конструктор `New(manager, id, text)` сам показывает LoadingForm |
| L-AUX-029 | VisibleChanged | `Form_VisibleChanged` | BaseDataForm.vb:168 | При скрытии очищает `StatusLabel` | |
| L-AUX-030 | Bind / BindControls | `Bind`, `BindControls` | BaseDataForm.vb:46, 52 | `DataView` по таблице и фильтру; `_view(0).BeginEdit()`; привязка по `Tag` контрола к колонке (`Text`, `Checked`, `Value`, `Image`, `SelectedValue`); lookup-`ComboBox` из свойств колонки; tooltip | Требует хотя бы одну строку (`_view(0)`) |
| L-AUX-031 | CommitData / DoCancelData | `CommitData`, `CancelData` | BaseDataForm.vb:93, 99, 102 | `EndEdit` + `AcceptChanges`; отмена — `CancelEdit`, `DoCancelData`, `Dispose` | Переопределяются в `ItemDataForm` |

### ItemDataForm

Структура вкладок (`ItemTab`, Designer:1280-1297): `Description` (вложенный `TabControl2`: English, German, Russian, Polish, French, Italian, Dutch, Chinese — Designer:1322-1329), `General`, `Attributes`, `Abilities`, `NCTH`, `Bonuses`, `Weapon` (вложенный `WeaponTab`: General, Guns, Launchers, Sounds — Designer:6992-6995), `Attachments`, `Attachment Data` (вложенный `AttDataSubTab`: две страницы — Designer:9074-9075), `Explosives`, `Ammo`, `Armour`, `Face`, `Inventories`, `Pockets`, `Overheating`, `Flags` (GroupBox `ItemFlags` / `ItemFlags2` / `Drugs` — Designer:11973-12024), `Robot`. Набор видимых вкладок зависит от `usItemClass` (`DisplayTabs`, ItemDataForm.vb:1232).

| ID | Surface | Handler | Legacy file:line | Behaviour | Notes |
|---|---|---|---|---|---|
| L-ITEM-001 | Открытие формы | `Public Shared Sub Open` | ItemDataForm.vb:1352 | Текст `DisplayText.ItemDataFormText` (набор, ID, имя); если такой MDI-формы нет — создать и показать | Дедупликация по заголовку через `MainWindow.FormOpen` |
| L-ITEM-002 | Конструктор / `Initialize` | `New`, `Initialize` | ItemDataForm.vb:7, 60 | `Bind(ITEM, uiIndex=ID)`; `ClassNameLabel` привязан к `usItemClass`; graphics/robot combo; `DisplayTabs`; заполнение чекбоксов флагов; `SetupGrid` для 7 гридов; `LoadInventoryData` | Показывает LoadingForm (в `BaseDataForm.New`) |
| L-ITEM-003 | Языковые вкладки | `RemoveUnusedLanguageSpecificItemTabs`, `LanguageSpecificFileExist` | ItemDataForm.vb:16, 44 | Удаляет German/Russian/Polish/Italian/French/Chinese/Dutch страницы, если нет `<Language>.Items.xml` в каталоге набора | Вызывается до `Bind` |
| L-ITEM-004 | Привязка данных | `Bind` (inherited) | BaseDataForm.vb:46 | `DataView(ITEM, uiIndex=ID)`; `_view(0).BeginEdit()`; привязка контролов по `Tag` | Требует существующую строку |
| L-ITEM-005 | Состав вкладок по классу | `DisplayTabs` | ItemDataForm.vb:1232 | Для `Ammo`, `Armour`, `Bomb/Grenade`, `Gun`, `Launcher`, `Knife/ThrowingKnife/Thrown/Tentacle/Punch`, `Face`, `Kit/MedKit/Misc`, `Key`, `LBE`, `RandomItem`, `Else` удаляет разные наборы страниц; для большинства классов убирает `WeaponTemperatureGroupBox` из `Overheating` | Класс `Key`/`LBE`/`None/Money` урезаны сильнее всего; `LBE` сохраняет `AttachmentPage` (комментарий про MOLLE) |
| L-ITEM-006 | CommitData | `CommitData` override | ItemDataForm.vb:947 | Переносит в строку `ItemImage`, `ubGraphicType`, `ubGraphicNum`, Robot*SkillGrant; проверяет конфликт ID и предлагает обмен (`MessageBox YesNo`); `SaveInventoryData`; переносит 64 + 43 флага из CheckedListBox; `AcceptChanges`; `AcceptGridChanges` для 7 гридов | При отказе от обмена — `ShowWarning` и `False` (Apply/OK не закрываются) |
| L-ITEM-007 | Отмена | `DoCancelData` override | ItemDataForm.vb:1124 | `CancelGridChanges` для 7 гридов | |
| L-ITEM-008 | Apply | `ApplyButtonClicked` override | ItemDataForm.vb:1137 | Сбрасывает `GraphicIndexUpDown.Value` из строки | Вызывается после `CommitData` |
| L-ITEM-009 | Change Class… | `ChangeClassButton_Click` | ItemDataForm.vb:1141; Designer:12497 | Открывает `ChangeClassForm(_dm, _view, Me)` модально | Пересоздаёт форму после смены класса |
| L-ITEM-010 | Chooser типов графики | `PopulateGraphicsTypeComboBox` | ItemDataForm.vb:336 | Пункты: «Guns», затем `P{0}Items` для `0..ImageTypeCount` | |
| L-ITEM-011 | Robot skill combo | `PopulateRobotSkillComboBoxes` | ItemDataForm.vb:347 | Заполняет три ComboBox списком `SkillTraitNew` (24 значения) | |
| L-ITEM-012 | Индекс графики | `GraphicIndexUpDown_ValueChanged` | ItemDataForm.vb:802 | `LoadImages(type, index)` → Small/Medium/Big Image | |
| L-ITEM-013 | Тип графики | `GraphicTypeCombo_SelectedIndexChanged` | ItemDataForm.vb:806 | Меняет `GraphicIndexUpDown.Maximum`, вызывает `LoadImages`, заполняет `ImageListBox` большими изображениями | |
| L-ITEM-014 | Список изображений — выбор | `ImageListBox_SelectedIndexChanged` | ItemDataForm.vb:821 | `GraphicIndexUpDown.Value = SelectedIndex` | |
| L-ITEM-015 | Список изображений — измерение | `ImageListBox_MeasureItem` | ItemDataForm.vb:812 | Высота элемента по высоте изображения | |
| L-ITEM-016 | Список изображений — отрисовка | `ImageListBox_DrawItem` | ItemDataForm.vb:825 | Рисует изображение; синий фон для выбранного | |
| L-ITEM-017 | Счётчики символов описаний (16 обработчиков) | `DescriptionTextBox_TextChanged` … `ChineseBRDescriptionTextBox_TextChanged` | ItemDataForm.vb:843-903 | Для каждого языка: `<MaxLength> - <TextLength>` в соответствующий `*CharsLeftLabel` | Обработчики только обновляют счётчик |
| L-ITEM-018 | AP-калькулятор ShotsPer4Turns | `ubShotsPer4TurnsUpDown_ValueChanged` | ItemDataForm.vb:909 | Пишет `= N APs` в `APsShots4TurnsLabel` по формуле с `IniFile.APMaximum()` (по умолчанию 100) | |
| L-ITEM-019 | AP-калькулятор ReadyTime | `ubReadyTimeUpDown_ValueChanged` | ItemDataForm.vb:930 | Пишет `= N APs` в `APsReadyLabel`; при `APMaximum <= 0` берёт 25 | |
| L-ITEM-020 | Флаги ItemFlags (CheckedListBox) | `ItemFlag_ValueChanged` | ItemDataForm.vb:532 | Для 35 индексов без чекбоксов (0–11, 13–33, 60–61) ничего не делает; для остальных синхронизирует `itemFlagsArray(index).Checked` | Часть флагов представлена только CheckedListBox; точная карта — `00-inventory-ui-item-fields.md` |
| L-ITEM-021 | Флаги ItemFlags2 (CheckedListBox) | `ItemFlag2_ValueChanged` | ItemDataForm.vb:781 | `AntitankMine` и `DiseaseSystemExclusive` игнорируются; остальные → `itemFlags2Array(index).Checked` | |
| L-ITEM-022 | Синхронизация 70 чекбоксов/радиокнопок с FlagsTab | `AttentionItem_ValueChanged` … `RobotLaserBonus_ValueChanged` | ItemDataForm.vb:387-776 | Каждый обработчик вызывает `SetItemChecked(<flag>, <control>.Checked)` в `GroupBox64/ItemFlagsCheckedList` или `GroupBox67/ItemFlags2CheckedList` | 70 однотипных обработчиков (29 `ItemFlags` + 41 `ItemFlags2`); `CannonRadioButton`/`GrenadeLauncherRadioButton`/`MortarRadioButton`/`RocketLauncherRadioButton` тоже в этой группе; точная карта — `00-inventory-ui-item-fields.md` |
| L-ITEM-023 | SetupGrid | `SetupGrid` | ItemDataForm.vb:1149 | `InitializeGrid` c `DataView(table, itemIndexField=ID)`, скрывает ключевую колонку, `grid.Tag = itemIndexField`, `AddHandler DefaultValuesNeeded` | 7 гридов: AttachmentGrid, AttachmentInfoGrid, AttachToGrid, IncompatibleAttachmentGrid, LaunchableGrid, LauncherGrid, CompatibleFaceItemGrid |
| L-ITEM-024 | Значения по умолчанию в гридах | `Grid_DefaultValuesNeeded` | ItemDataForm.vb:1164 | В ключевое поле пишет `_id`; для одно-ключевой таблицы — `GetNextPrimaryKeyValue` | |
| L-ITEM-025 | Отмена изменений гридов | `CancelGridChanges` | ItemDataForm.vb:1178 | Временно `RowStateFilter = Added/ModifiedCurrent/Deleted`, `RejectChanges`, возврат `CurrentRows` | |
| L-ITEM-026 | Принятие изменений гридов | `AcceptGridChanges` | ItemDataForm.vb:1187 | Временно `RowStateFilter`, `AcceptChanges`, возврат `CurrentRows` | |
| L-ITEM-027 | AttachToGrid — двойной клик по заголовку | `AttachToGrid_RowHeaderMouseDoubleClick` | ItemDataForm.vb:1196 | `GridOpenItem(AttachToGrid, row, ATTACHMENT.itemIndex)` — открыть предмет, к которому крепится | Открывает вложенную `ItemDataForm` |
| L-ITEM-028 | AttachmentGrid — двойной клик | `AttachmentGrid_RowHeaderMouseDoubleClick` | ItemDataForm.vb:1200 | `GridOpenItem(..., ATTACHMENT.attachmentIndex)` | |
| L-ITEM-029 | IncompatibleAttachmentGrid — двойной клик | `IncompatibleAttachmentGrid_RowHeaderMouseDoubleClick` | ItemDataForm.vb:1204 | `GridOpenItem(..., INCOMPATIBLEATTACHMENT.incompatibleattachmentIndex)` | |
| L-ITEM-030 | LaunchableGrid — двойной клик | `LaunchableGrid_RowHeaderMouseDoubleClick` | ItemDataForm.vb:1208 | `GridOpenItem(..., LAUNCHABLE.launchableIndex)` | |
| L-ITEM-031 | LauncherGrid — двойной клик | `LauncherGrid_RowHeaderMouseDoubleClick` | ItemDataForm.vb:1212 | `GridOpenItem(..., LAUNCHABLE.itemIndex)` | |
| L-ITEM-032 | CompatibleFaceItemGrid — двойной клик | `CompatibleFaceItemGrid_RowHeaderMouseDoubleClick` | ItemDataForm.vb:1216 | `GridOpenItem(..., COMPATIBLEFACEITEM.compatiblefaceitemIndex)` | |
| L-ITEM-033 | GridOpenItem | `GridOpenItem` | ItemDataForm.vb:1220 | Находит `ITEM` по ID; если есть — `Open(_dm, id, name)` | |
| L-ITEM-034 | LBE class → доступные карманы | `lbeClassComboBox_SelectedIndexChanged` | ItemDataForm.vb:1566 | 0 — `DisableAllPockets`; 1 — Thigh Pack; 2 — Vest Pack; 3 — Combat Pack; 4 — Back Pack; другие значения — ничего | `DisableAllPockets` сбрасывает 12 combo в 0 |
| L-ITEM-035 | Статусная строка по ID | `uiIndexUpDown_ValueChanged` | ItemDataForm.vb:1584 | `UpdateStatusBar()` | |
| L-ITEM-036 | Загрузка инвентарей магазинов | `LoadInventoryData` | ItemDataForm.vb:1368 | Для 19 торговцев вызывает `LoadShopkeeperData` | |
| L-ITEM-037 | Сохранение инвентарей | `SaveInventoryData` | ItemDataForm.vb:1390 | Для 19 торговцев вызывает `SaveShopkeeperData` | Вызывается из `CommitData` |
| L-ITEM-038 | Загрузка одного магазина | `LoadShopkeeperData` | ItemDataForm.vb:1412 | Ищет строку `<Shop>Inventory` по `sItemIndex = _id`; `Checked`, `ubOptimalNumber`, `Enabled`; `AddHandler CheckedChanged` | Подписка добавляется при каждом вызове без отписки |
| L-ITEM-039 | Сохранение одного магазина | `SaveShopkeeperData` | ItemDataForm.vb:1430 | Checked: изменить/создать строку с `sItemIndex` и `ubOptimalNumber`; не Checked: удалить существующую | `NewRow` вставляет строку в таблицу (ключ автоматический) |
| L-ITEM-040 | Чекбокс магазина | `InventoryCheckBox_CheckedChanged` | ItemDataForm.vb:1363 | Включает/выключает `<Name>UpDown` по `Checked` | |
| L-ITEM-041 | Карманы: disable | `DisableAllPockets` | ItemDataForm.vb:1452 | Отключает 12 combo и ставит `SelectedValue = 0` | |
| L-ITEM-042 | Карманы: thigh pack | `EnableThighPackPockets` | ItemDataForm.vb:1482 | Включает первые combo, остальные отключает | |
| L-ITEM-043 | Карманы: vest pack | `EnableVestPackPockets` | ItemDataForm.vb:1509 | Аналогично для своего набора | |
| L-ITEM-044 | Карманы: combat pack | `EnableCombatPackPockets` | ItemDataForm.vb:1525 | Аналогично | |
| L-ITEM-045 | Карманы: back pack | `EnableBackPackPockets` | ItemDataForm.vb:1541 | Аналогично | |
| L-ITEM-046 | `FoodTableLayout_Paint` | — | ItemDataForm.vb:1590 | Мёртвый метод: нет `Handles`, нет `AddHandler`, нигде не вызывается | Кандидат в «Open questions» |

### MercGearDataForm

| ID | Surface | Handler | Legacy file:line | Behaviour | Notes |
|---|---|---|---|---|---|
| L-MERC-001 | Открытие формы | `Public Shared Sub Open` | MercGearDataForm.vb:11 | Текст `DisplayText.MercGearDataFormText` (набор, имя); дедупликация по `MainWindow.FormOpen` | |
| L-MERC-002 | Конструктор / привязка | `New` | MercGearDataForm.vb:4; BaseDataForm.vb:32 | `Bind(MERCGEAR, mIndex = ID)` | Наследует Apply/OK/Cancel и `CommitData` из `BaseDataForm` |
| L-MERC-003 | Имя набора 1 | `GearKitNameTextBox1_TextChanged` | MercGearDataForm.vb:20 | Переименовывает `GearKitPage1`: пусто → «GEARKIT 1», иначе текст | |
| L-MERC-004 | Имя набора 2 | `GearKitNameTextBox2_TextChanged` | MercGearDataForm.vb:27 | То же для `GearKitPage2` / «GEARKIT 2» | |
| L-MERC-005 | Имя набора 3 | `GearKitNameTextBox3_TextChanged` | MercGearDataForm.vb:34 | То же для `GearKitPage3` | |
| L-MERC-006 | Имя набора 4 | `GearKitNameTextBox4_TextChanged` | MercGearDataForm.vb:41 | То же для `GearKitPage4` | |
| L-MERC-007 | Имя набора 5 | `GearKitNameTextBox5_TextChanged` | MercGearDataForm.vb:48 | То же для `GearKitPage5` | |
| L-MERC-008 | Активация формы | `MercGearDataForm_Activated` | MercGearDataForm.vb:55 | Читает `ItemArray(3/68/133/198/263)` и выставляет заголовки пяти `GearKitPageN` (пусто → «GEARKIT N») | Жёсткие индексы колонок, без имён полей |

## Grid and table commands

Общее: `InitializeGrid` (GridUtility.vb:14) строит колонки вручную (`AutoGenerateColumns = False`), тип колонки выбирается по `DataType` и `Lookup_Table`: `CheckBox` для `Boolean`, `Image` для `Image`, `ComboBox` при наличии lookup-таблицы, иначе `TextBox`. `SortMode = Automatic` у всех колонок; `ToolTipText` = имя колонки + описание из свойств. Скрытые колонки (`Grid_Hidden`) не создаются, кроме первичного ключа (создаётся с `Visible = False`). Для `POCKET` при `ItemSizeMax > 250` задаётся `FillWeight = 1`.

| ID | Surface | Handler | Legacy file:line | Behaviour | Notes |
|---|---|---|---|---|---|
| L-GRID-001 | Контекстное меню — Select Columns | `SelectColumnsToolStripMenuItem_Click` | GridForm.vb:146; Designer:73 | `ColumnSelectForm(_dm, Grid, _subTable).ShowDialog`; затем `Hide`, `InitializeGrid` заново, `Show`; состояние окна восстанавливается | Модально; вызывает пересоздание колонок |
| L-GRID-002 | Контекстное меню — Custom Filter | `FilterToolStripMenuItem_Click` | GridForm.vb:204; Designer:79 | `CustomFilterForm` с текущим `Filter`; при `DialogResult.OK` присваивает `Me.Filter` | Валидация и сообщение об ошибке — в сеттере `Filter` |
| L-GRID-003 | Контекстное меню — Filter by Selected Value | `FilterBySelectedValueToolStripMenuItem_Click` | GridForm.vb:233; Designer:85 | `FilterByValue(False)` — `column = value` | Требует ровно одну выделенную ячейку |
| L-GRID-004 | Контекстное меню — Filter Excluding Selected Value | `FilterExcludingSelectedValueToolStripMenuItem_Click` | GridForm.vb:237; Designer:122 | `FilterByValue(True)` — `column <> value` | Строковые значения заключаются в `'…'` |
| L-GRID-005 | Контекстное меню — Remove Filter | `RemoveFilterToolStripMenuItem_Click` | GridForm.vb:229; Designer:91 | `Me.Filter = Nothing` → `RowFilter = _origFilter` | Базовый фильтр, заданный конструктором, сохраняется |
| L-GRID-006 | Контекстное меню — Apply Value to Selected Column | — (контейнер) | Designer:95 | Подменю из трёх пунктов | Виден только при `Not ReadOnly` и `SelectionMode` = `RowHeaderSelect`/`CellSelect` |
| L-GRID-007 | Apply Value — Add | `ValueAddToolStripMenuItem_Click` | GridForm.vb:271; Designer:104 | `ApplyValueToColumn(Add)`; `InputBox`, по умолчанию `0`; `drv(col) += value` для всех строк вида | Только числовые типы; без подтверждения |
| L-GRID-008 | Apply Value — Multiply | `ValueMultiplyToolStripMenuItem_Click` | GridForm.vb:275; Designer:110 | То же с `*=`; по умолчанию `1` | Только числовые типы |
| L-GRID-009 | Apply Value — Set | `ValueSetToolStripMenuItem_Click` | GridForm.vb:279; Designer:116 | `drv(col) = value` для всех строк вида; допускает строки и `Boolean` | `IsNumeric` проверяется только для не-строк/не-Boolean |
| L-GRID-010 | ItemGridForm — View Details | `ViewMenuItem_Click` | ItemGridForm.vb:29, 52 | `ItemDataForm.Open` по ID и имени выбранной строки | Пункт добавляется в `GridContextMenu` в конструкторе |
| L-GRID-011 | ItemGridForm — New | `NewMenuItem_Click` | ItemGridForm.vb:30, 58 | `NewItemForm`, показ через `MainWindow.FormOpen/ShowForm` | |
| L-GRID-012 | ItemGridForm — Duplicate | `DuplicateMenuItem_Click` | ItemGridForm.vb:31, 63 | `Database.DuplicateRow(ITEM, key)` и открытие копии в `ItemDataForm` | |
| L-GRID-013 | ItemGridForm — Enable Editing | `EnableEditingMenuItem_Click` | ItemGridForm.vb:26, 72 | Переключает `Checked`; `Grid.ReadOnly = Not Checked`; `SelectionMode` = `RowHeaderSelect`/`FullRowSelect`; `EnableContextMenuItems()` | Пункт создаётся кодом, не дизайнером |
| L-GRID-014 | ItemGridForm — двойной клик | `Grid_CellMouseDoubleClick` | ItemGridForm.vb:35 | При `RowIndex <> -1` → `ItemDataForm.Open(ID, Name)` | |
| L-GRID-015 | ItemGridForm — удаление: подготовка | `Grid_UserDeletingRow` | ItemGridForm.vb:41 | Запоминает `DataRow` по ID в `deletingRow` | |
| L-GRID-016 | ItemGridForm — удаление: подтверждение | `Grid_UserDeletedRow` | ItemGridForm.vb:45 | `RejectChanges()` у строки и `Database.DeleteRow(ITEM, ID)` | Удаление из БД, а не только из вида |
| L-GRID-017 | MercGearGridForm — View Details | `ViewMenuItem_Click` | MercGearGridForm.vb:21, 25 | `MercGearDataForm.Open(mIndex, mName)` | |
| L-GRID-018 | MercGearGridForm — двойной клик | `Grid_CellMouseDoubleClick` | MercGearGridForm.vb:31 | То же по строке | |
| L-GRID-019 | DataGridForm — значения по умолчанию | `Grid_DefaultValuesNeeded` | DataGridForm.vb:23 | Если PK один — `GetNextPrimaryKeyValue` в ячейку ключа новой строки | Позволяет добавлять строки в lookup/обычные таблицы |
| L-GRID-020 | Grid — SelectionChanged | `Grid_SelectionChanged` | GridForm.vb:134 | `UpdateStatusBar()` | |
| L-GRID-021 | Grid — UserAddedRow | `Grid_UserAddedRow` | GridForm.vb:138 | `UpdateStatusBar()` | |
| L-GRID-022 | Grid — UserDeletedRow (базовый) | `Grid_UserDeletedRow` | GridForm.vb:142 | `UpdateStatusBar()` | Перекрывается в `ItemGridForm` |
| L-GRID-023 | Grid — DragDrop | `Grid_DragDrop` | GridForm.vb:78 | Если `_mouseDown = False` — `Database.CopyRows(_view.Table, c.Rows, c.Database)` | Копирование строк между наборами |
| L-GRID-024 | Grid — DragEnter | `Grid_DragEnter` | GridForm.vb:85 | `Copy` только если БД-источник другая, строки есть и имя таблицы совпадает; иначе `None` | |
| L-GRID-025 | Grid — MouseMove (начало drag) | `Grid_MouseMove` | GridForm.vb:113 | При зажатой кнопке и попадании в строку собирает `GridCopyContainer` из выбранных строк и вызывает `DoDragDrop` | `_mouseDown` сбрасывается в конце |
| L-GRID-026 | Grid — MouseDown | `Grid_MouseDown` | GridForm.vb:105 | `_mouseDown = True` | |
| L-GRID-027 | Grid — MouseUp | `Grid_MouseUp` | GridForm.vb:130 | `_mouseDown = False` | |
| L-GRID-028 | Grid — MouseLeave | `Grid_MouseLeave` | GridForm.vb:109 | `_mouseDown = False` | |
| L-GRID-029 | Grid — KeyDown | `Grid_KeyDown` | GridForm.vb:97 | При `e.Alt` ставит `_keyDown = True` | `_keyDown` больше нигде не читается — мёртвый флаг |
| L-GRID-030 | Grid — KeyUp | `Grid_KeyUp` | GridForm.vb:101 | `_keyDown = False` | |
| L-GRID-031 | GridForm — Load | `GridForm_Load` | GridForm.vb:174 | Закрывает LoadingForm; восстанавливает `Top/Left/Height/Width/WindowState` из `SettingsUtility`; без сохранённых размеров считает по содержимому, ограничивает клиентской областью MainWindow | Начальное положение — 0,0 |
| L-GRID-032 | GridForm — FormClosing | `GridForm_FormClosing` | GridForm.vb:164 | Сохраняет `Height/Width/Top/Left/WindowState/OpenedBefore`; `SaveColumnWidths()` → `Database.SaveSchema()` | Настройки в `XMLEditorSettings.xml`, ширины — в схеме |
| L-GRID-033 | GridForm — Activated | `GridForm_Activated` | GridForm.vb:159 | `UpdateStatusBar()`; `Me.Refresh()` | |
| L-GRID-034 | GridForm — VisibleChanged | `Form_VisibleChanged` | GridForm.vb:218 | При скрытии очищает `StatusLabel` | |
| L-GRID-035 | Grid — DataError | `Grid_DataError` | GridUtility.vb:144; `AddHandler` GridUtility.vb:23 | `ArgumentException` → `e.Cancel = True`; `ConstraintException` → сообщение «This value has already been used…»; иначе `ShowError(e.Exception.Message)` | |
| L-GRID-036 | `Filter` (свойство) | setter | GridForm.vb:55-76 | Заменяет `"` на `'`; при непустом фильтре `RowFilter = _origFilter AND (custom)` или `custom`; при исключении — `ShowError` и `RowFilter = _origFilter` | `_customFilter` присваивается до `Try`, поэтому после ошибки хранит невалидное выражение |
| L-GRID-037 | `FilterByValue` | `FilterByValue` | GridForm.vb:241 | Требует одну ячейку; строит `column = '<value>'`/`<>`; добавляет через `AND` к текущему `Filter` | При отсутствии фильтра использует пустой |
| L-GRID-038 | `ApplyValueToColumn` | `ApplyValueToColumn`, `PromptForValue` | GridForm.vb:283, 320 | Add/Multiply запрещены для `String`/`Boolean`; `InputBox` с подсказкой и значением по умолчанию; цикл по `DataRowView` вида | Не делает `EndEdit`/`AcceptChanges` |
| L-GRID-039 | `EnableContextMenuItems` | `EnableContextMenuItems` | GridForm.vb:339 | Показывает три пункта (Apply Value, Filter by, Filter excluding) только при `Not ReadOnly` и `SelectionMode` = `RowHeaderSelect`/`CellSelect`, иначе скрывает | Вызывается из конструктора и после смены режима |
| L-GRID-040 | `UpdateStatusBar` | `UpdateStatusBar` | GridForm.vb:210 | `"<Text>: "` + (`SelectedCells(0).RowIndex + 1` + `" of "`) + `_view.Count` + `" Rows"` | |
| L-GRID-041 | `SaveColumnWidths` | `SaveColumnWidths`, `SetColumnWidth` | GridForm.vb:222; GridUtility.vb:170 | Для каждой колонки пишет `Width` в свойство колонки (или подтаблицы), затем `SaveSchema()` | Порядок колонок (`DisplayIndex`) не сохраняется |
| L-GRID-042 | `InitializeGrid` | `InitializeGrid` | GridUtility.vb:14 | Создание колонок, lookup `ComboBox` (`ValueMember`/`DisplayMember`/`DataSource`, пустая строка при `Lookup_AddBlank`, префикс первого значения), `AutoComplete = True`, `DisplayStyle` в зависимости от `ReadOnly` | Кэш `DataView` по lookup-таблицам |

### Сортировка, поиск, фильтры, колонки, строки, клавиатура

- **Сортировка.** `DataGridViewColumn.SortMode = Automatic` (GridUtility.vb:58) — сортировка по клику на заголовок; направление/выбранная колонка нигде не сохраняются. Программной сортировки или отдельных команд «сортировать по…» нет.
- **Поиск.** Отдельного поиска/`Find` в сетках нет: ни пункта меню, ни обработчика, ни `Ctrl+F` в гриде (`Ctrl+F` занят `Items > Show > By Other Filters > Custom Filter`).
- **Фильтры.** Базовый фильтр передаётся в конструктор (`rowFilter`), оборачивается в скобки в `_origFilter`; пользовательский фильтр — `DataView.RowFilter`; несколько фильтров объединяются через `AND`. Фильтр не сохраняется между сессиями. Ошибка выражения показывает сообщение и сбрасывает `RowFilter` к `_origFilter`.
- **Колонки.** Видимость — `Grid_Hidden` в свойствах колонки/подтаблицы, меняется через `ColumnSelectForm` и сохраняется `SaveSchema()`; ширины — `Width` в свойствах, сохраняются при закрытии формы; порядок колонок разрешён (`AllowUserToOrderColumns = True`), но не сохраняется.
- **Строки.** Добавление: `DataGridForm`, `ChildDataGridForm` (и `ItemGridForm` только через `New`/`Duplicate`, не пустой строкой). Удаление: `DataGridForm`, `ChildDataGridForm`, `ItemGridForm` (с `Database.DeleteRow`). Дублирование и открытие деталей — только `ItemGridForm`; `MercGearGridForm` — только детали. Копирование строк drag&drop между разными наборами при совпадении имени таблицы.
- **Контекстное меню.** `Select Columns`, `Custom Filter`, `Remove Filter` доступны всегда; `Filter by Selected Value`, `Filter Excluding Selected Value`, `Apply Value to Selected Column` — только в редактируемом режиме с `RowHeaderSelect`/`CellSelect`.
- **Клавиатура.** Явных `KeyDown`-команд в гридах нет (кроме отслеживания Alt, L-GRID-029). Работают унаследованные от `DataGridView` `Ctrl+C` (`ClipboardCopyMode = EnableWithoutHeaderText`) и навигация; вставка/вырезание из Edit-меню не подключены (L-SHELL-027..029 — пустые обработчики).
- **Статусная строка.** Строка/всего строк — L-GRID-040; заголовок формы — `MainForm.StatusLabel`.

### Клавиатурные сокращения меню (сводка)

| Shortcut | Команда | ID |
|---|---|---|
| Ctrl+R | File > Reload | L-SHELL-013 |
| Ctrl+S | File > Save Active Data Set | L-SHELL-014 |
| Ctrl+Shift+S | File > Save All Data Sets | L-SHELL-015 |
| Ctrl+P | File > Print (скрыт) | L-SHELL-016 |
| Ctrl+1…Ctrl+9, Ctrl+0 | File > Active Data Set > 1…10 | L-SHELL-003..012 |
| Ctrl+Alt+A | Items > Show > All | L-SHELL-076 |
| Ctrl+I | Items > Show > By ID | L-SHELL-077 |
| Ctrl+N | Items > New | L-SHELL-102 |
| Ctrl+F | Items > Show > By Other Filters > Custom Filter | L-SHELL-101 |
| Alt+G/A/L/N/E/K/O/R/F/I/Y/B/M/T | Items > Show > By Class / Attachments | L-SHELL-079..095 |
| Ctrl+Alt+N | Items > Show > By Class > Empty/None | L-SHELL-092 |
| Ctrl+Alt+Shift+C | Data > New Attachment System > Attachment Class Code Maker (скрыт) | L-SHELL-066 |
| Ctrl+Alt+Shift+A | Data > Attachment Point Code Maker (скрыт) | L-SHELL-070 |

## Settings and persistence

| ID | Surface | Handler / entry | Legacy file:line | Behaviour | Notes |
|---|---|---|---|---|---|
| L-SET-001 | Загрузка настроек | `SettingsUtility.LoadSettings` | SettingsUtility.vb:34; Main.vb:69 | Создаёт `DataTable("Settings")` с ключом `Name`, читает `XMLEditorSettings.xml`; `FileNotFoundException` игнорируется | Файл лежит рядом с приложением |
| L-SET-002 | Чтение значения | `GetSettingsValue` | SettingsUtility.vb:26 | `Rows.Find(name)`; при отсутствии — `defaultValue` | Значения хранятся как строки |
| L-SET-003 | Запись значения | `SetSettingsValue` | SettingsUtility.vb:78 | Обновляет или добавляет строку `Name/Value` | Сохранение на диск — только `SaveSettings` |
| L-SET-004 | Сохранение настроек | `SettingsUtility.SaveSettings` | SettingsUtility.vb:54; Main.vb:77 | `_settingsTable.WriteXml("XMLEditorSettings.xml")` после выхода из `Application.Run` | Вызывается не при закрытии каждой формы |
| L-SET-005 | Чтение `XMLEditorInit.xml` | `IniFile.ReadFile` | IniFile.vb:89; Main.vb:28 | Разбирает `AP_Maximum`, `Use_Working_Directory`, `Hide_Nothing_Items`, `Data_Directory_N`, `GameDir_<Language>_Path_N` (N = 1…10) | Индексы массивов 0-based, из файла 1-based |
| L-SET-006 | `AP_Maximum` | `IniFile.APMaximum` | IniFile.vb:29 | Возвращает `apMax`; используется калькуляторами AP в `ItemDataForm` | В `XMLEditorInit.xml` = 100 |
| L-SET-007 | `Use_Working_Directory` | `IniFile.UseWorkingDirectory` | IniFile.vb:23; Main.vb:29-34 | Если версия Major/Minor изменилась — принудительно `False` | Влияет на `DataManager` |
| L-SET-008 | `Hide_Nothing_Items` | `IniFile.Hide_Nothing_Items` | IniFile.vb:35; Main.vb:49 | Передаётся в конструктор `DataManager` | По умолчанию `True` |
| L-SET-009 | Геометрия и View в `My.Settings` | `MainForm_Load`, `MainForm_FormClosing` | MainForm.vb:13-22, 84-96 | `View_Background`, `View_Toolbar`, `View_StatusBar`, `MainForm_Size/Location/State`; `.Save()` при закрытии | Отдельно от `XMLEditorSettings.xml` |
| L-SET-010 | Версия данных | `Save` + `Main` | MainForm.vb:712-717; Main.vb:31-34 | `Last_Version_Major/Minor` пишутся после Save All или при единственном наборе; при несовпадении на старте отключается рабочий каталог | Условие `saveAll OrElse GameDataCount = 1` |

## Misc

| ID | Surface | Handler / entry | Legacy file:line | Behaviour | Notes |
|---|---|---|---|---|---|
| L-MISC-001 | Защита от второго экземпляра | `CheckForDuplicateProcess` | Main.vb:99 | Считает процессы с именем сборки; при `count > 1` — `End` | Проверка до создания Splash |
| L-MISC-002 | Фатальная ошибка | `ExitDueToError` + `AddHandler ErrorHandler.FatalError` | Main.vb:20, 92 | `End` | Подписка внутри `Try` в `Main` |
| L-MISC-003 | `MyApplication` | — | ApplicationEvents.vb:8 | Пустой partial-класс; обработчиков `Startup`/`Shutdown`/`UnhandledException` нет | Вся инициализация — в `Main()` |

## Target appendix: menu command -> target data

Одна строка на команду, которая открывает данные. ID совпадает с ID в дереве меню. `kind` — класс формы (`ItemGridForm` = ITEM + подтаблица, `DataGridForm` = редактируемая таблица с добавлением/удалением, `LookupGridForm` = таблица без добавления, `ChildDataGridForm` = таблица с добавлением/удалением, `MercGearGridForm` = MERCGEAR). Пути к файлам — из `TableProperty.FileName` (`DatabaseCode.vb`); для lookup-таблиц файл строится как `Lookup/<TableName>.xml` (`DatabaseCode.vb:839`).

| ID (ссылка на дерево) | Handler | Form kind | Table / XML file | Subtable / filter / note | Menu path |
|---|---|---|---|---|---|
| L-SHELL-034 | `EnglishToolStripMenuItem_Click` | ShowDataGridForm | `AMMO` → `Items/AmmoStrings.xml` | — | Data > Ammo > Calibers > English |
| L-SHELL-035 | `GermanToolStripMenuItem_Click` | ShowDataGridForm | `GermanAmmo` → `Items/German.AmmoStrings.xml` | — | Data > Ammo > Calibers > German |
| L-SHELL-036 | `RussianToolStripMenuItem_Click` | ShowDataGridForm | `RussianAmmo` → `Items/Russian.AmmoStrings.xml` | — | Data > Ammo > Calibers > Russian |
| L-SHELL-037 | `PolishToolStripMenuItem_Click` | ShowDataGridForm | `PolishAmmo` → `Items/Polish.AmmoStrings.xml` | — | Data > Ammo > Calibers > Polish |
| L-SHELL-038 | `FrenchToolStripMenuItem_Click` | ShowDataGridForm | `FrenchAmmo` → `Items/French.AmmoStrings.xml` | — | Data > Ammo > Calibers > French |
| L-SHELL-039 | `ItalianToolStripMenuItem_Click` | ShowDataGridForm | `ItalianAmmo` → `Items/Italian.AmmoStrings.xml` | — | Data > Ammo > Calibers > Italian |
| L-SHELL-040 | `DutchToolStripMenuItem_Click` | ShowDataGridForm | `DutchAmmo` → `Items/Dutch.AmmoStrings.xml` | — | Data > Ammo > Calibers > Dutch |
| L-SHELL-041 | `ChineseToolStripMenuItem_Click` | ShowDataGridForm | `ChineseAmmo` → `Items/Chinese.AmmoStrings.xml` | — | Data > Ammo > Calibers > Chinese |
| L-SHELL-042 | `TypesToolStripMenuItem_Click` | ShowDataGridForm | `AMMOTYPE` → `Items/AmmoTypes.xml` | — | Data > Ammo > Types |
| L-SHELL-043 | `ExplosionsToolStripMenuItem_Click` | ShowDataGridForm | `EXPDATA` → `Items/ExplosionData.xml` | — | Data > Explosions |
| L-SHELL-045 | `StandardToolStripMenuItem_Click` | ShowDataGridForm | `SOUND` → `Sounds/Sounds.xml` | — | Data > Sounds > Standard |
| L-SHELL-046 | `BurstToolStripMenuItem_Click` | ShowDataGridForm | `BURSTSOUND` → `Sounds/BurstSounds.xml` | — | Data > Sounds > Burst |
| L-SHELL-048 | `AmmoFlagsToolStripMenuItem_Click` | ShowLookupGridForm | `AmmoFlag` → `Lookup/AmmoFlag.xml` | — | Data > Lookup Tables > Ammo Flags |
| L-SHELL-049 | `ArmourClassesToolStripMenuItem_Click` | ShowLookupGridForm | `ArmourClass` → `Lookup/ArmourClass.xml` | — | Data > Lookup Tables > Armour Classes |
| L-SHELL-050 | `AttachmentSystemToolStripMenuItem_Click` | ShowLookupGridForm | `AttachmentSystem` → `Lookup/AttachmentSystem.xml` | — | Data > Lookup Tables > Attachment System |
| L-SHELL-051 | `AttachmentClassesToolStripMenuItem_Click` | ShowLookupGridForm | `AttachmentClass` → `Lookup/AttachmentClass.xml` | — | Data > Lookup Tables > Built-in Attachment Classes |
| L-SHELL-052 | `CursorsToolStripMenuItem_Click` | ShowLookupGridForm | `Cursor` → `Lookup/Cursor.xml` | — | Data > Lookup Tables > Cursors |
| L-SHELL-053 | `DrugTypesToolStripMenuItem_Click` | ShowDataGridForm | `DrugType` → `Lookup/DrugType.xml` | — | Data > Lookup Tables > Drug Types |
| L-SHELL-054 | `ExplosionTypesToolStripMenuItem_Click` | ShowLookupGridForm | `ExplosionType` → `Lookup/ExplosionType.xml` | — | Data > Lookup Tables > Explosion Types |
| L-SHELL-055 | `SeparabilityStatesToolStripMenuItem_Click` | ShowLookupGridForm | `Separability` → `Lookup/Separability.xml` | — | Data > Lookup Tables > Separability States |
| L-SHELL-056 | `ItemClassesToolStripMenuItem_Click` | ShowDataGridForm | `ItemClass` → `Lookup/ItemClass.xml` | — | Data > Lookup Tables > Item Classes |
| L-SHELL-057 | `ItemFlagsToolStripMenuItem_Click` | ShowLookupGridForm | `ItemFlag` → `Lookup/ItemFlag.xml` | — | Data > Lookup Tables > Item Flags |
| L-SHELL-058 | `LBEClassesToolStripMenuItem_Click` | ShowLookupGridForm | `LBEClass` → `Lookup/LBEClass.xml` | — | Data > Lookup Tables > LBE Classes |
| L-SHELL-059 | `MergeTypesToolStripMenuItem_Click` | ShowLookupGridForm | `MergeType` → `Lookup/MergeType.xml` | — | Data > Lookup Tables > Merge Types |
| L-SHELL-060 | `SkillChecksToolStripMenuItem_Click` | ShowLookupGridForm | `SkillCheckType` → `Lookup/SkillCheckType.xml` | — | Data > Lookup Tables > Skill Checks |
| L-SHELL-061 | `WeaponClassesToolStripMenuItem_Click` | ShowLookupGridForm | `WeaponClass` → `Lookup/WeaponClass.xml` | — | Data > Lookup Tables > Weapon Classes |
| L-SHELL-062 | `WeaponTypesToolStripMenuItem_Click` | ShowLookupGridForm | `WeaponType` → `Lookup/WeaponType.xml` | — | Data > Lookup Tables > Weapon Types |
| L-SHELL-064 | `NasAttachmentClassesToolStripMenuItem_Click` | ShowDataGridForm | `NasAttachmentClass` → `Lookup/NasAttachmentClass.xml` | — | Data > New Attachment System > Custom Attachment Classes |
| L-SHELL-065 | `NasAttachmentSlotsToolStripMenuItem_Click` | ShowDataGridForm | `ATTACHMENTSLOT` → `Items/AttachmentSlots.xml` | — | Data > New Attachment System > Attachment Slots |
| L-SHELL-066 | `AttachmentClassCodeMakerToolStripMenuItem_Click` | BitCodeMakerForm | `AttachmentPoint` → `Lookup/AttachmentPoint.xml` | скрыт | Data > New Attachment System > Attachment Class Code Maker |
| L-SHELL-067 | `PocketTypesToolStripMenuItem_Click` | ShowDataGridForm | `POCKET` → `Items/Pockets.xml` | — | Data > Pocket Types |
| L-SHELL-068 | `InventorySilhouettesToolStripMenuItem_Click` | ShowDataGridForm | `Silhouette` → `Lookup/Silhouette.xml` | — | Data > Inventory Silhouettes |
| L-SHELL-069 | `CommonAttachmentPointsToolStripMenuItem_Click` | ShowDataGridForm | `AttachmentPoint` → `Lookup/AttachmentPoint.xml` | — | Data > Common Attachment Points |
| L-SHELL-070 | `AttachmentPointCodeMakerToolStripMenuItem_Click` | BitCodeMakerForm | `AttachmentPoint` → `Lookup/AttachmentPoint.xml` | скрыт | Data > Attachment Point Code Maker |
| L-SHELL-071 | `FoodToolStripMenuItem_Click` | ShowDataGridForm | `FOOD` → `Items/Food.xml` | — | Data > Food |
| L-SHELL-072 | `ClothesToolStripMenuItem_Click` | ShowDataGridForm | `CLOTHES` → `Items/Clothes.xml` | — | Data > Clothes |
| L-SHELL-073 | `DiseaseMenuItem_Click` | ShowDataGridForm | `DISEASE` → `Disease.xml` | — | Data > Disease |
| L-SHELL-076 | `AllItemsToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | без фильтра и подтаблицы | Items > Show > All |
| L-SHELL-077 | `ToolStripMenuItem2_Click` | ItemDataForm (EnterItemIDForm) | `ITEM` → `Items/Items.xml` | ID из ввода | Items > Show > By ID |
| L-SHELL-079 | `GunsToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=2 AND rocketlauncher=0`; подтаблица `WEAPON` | Items > Show > By Class > Guns |
| L-SHELL-080 | `AmmoToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=1024`; подтаблица `MAGAZINE` | Items > Show > By Class > Ammo |
| L-SHELL-081 | `LaunchersToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=16 OR rocketlauncher=1`; подтаблица `WEAPON` | Items > Show > By Class > Launchers |
| L-SHELL-082 | `GrenadesToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=256`; подтаблица `EXPLOSIVE` | Items > Show > By Class > Grenades |
| L-SHELL-083 | `ExplosivesToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=512`; подтаблица `EXPLOSIVE` | Items > Show > By Class > Explosives |
| L-SHELL-084 | `KnivesToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=4 OR usItemClass=8`; подтаблица `WEAPON` | Items > Show > By Class > Knives |
| L-SHELL-085 | `OtherWeaponsToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=64 OR usItemClass=128 OR usItemClass=32`; подтаблица `WEAPON` | Items > Show > By Class > Other Weapons |
| L-SHELL-086 | `ArmourToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=2048`; подтаблица `ARMOUR` | Items > Show > By Class > Armour |
| L-SHELL-087 | `FaceToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=32768`; подтаблица `—` | Items > Show > By Class > Facial Gear |
| L-SHELL-088 | `KitsToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=4096 OR usItemClass=8192`; подтаблица `—` | Items > Show > By Class > Kits |
| L-SHELL-089 | `KeysToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=65536`; подтаблица `—` | Items > Show > By Class > Keys |
| L-SHELL-090 | `LoadBearingToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=131072`; подтаблица `—` | Items > Show > By Class > Load Bearing |
| L-SHELL-091 | `MiscToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=268435456 OR usItemClass=536870912`; подтаблица `—` | Items > Show > By Class > Misc |
| L-SHELL-092 | `NoneItemToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=1`; подтаблица `—` | Items > Show > By Class > Empty/None |
| L-SHELL-093 | `RandomItemsToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `usItemClass=1073741824`; подтаблица `—` | Items > Show > By Class > Random Items |
| L-SHELL-095 | `AttachmentsToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `Attachment=1`; подтаблица `—` | Items > Show > By Other Filters > Attachments |
| L-SHELL-096 | `DrugsItemFilterToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `Medical=1`; подтаблица `—` | Items > Show > By Other Filters >  |
| L-SHELL-097 | `TonsOfGunsToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `BigGunList=1`; подтаблица `—` | Items > Show > By Other Filters > Tons Of Guns |
| L-SHELL-098 | `NormalGunsToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `BigGunList=0`; подтаблица `—` | Items > Show > By Other Filters > Normal Guns |
| L-SHELL-099 | `SciFiToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `SciFi=1`; подтаблица `—` | Items > Show > By Other Filters > Sci-Fi |
| L-SHELL-100 | `NonSciFiToolStripMenuItem_Click` | ShowItemGridForm | `ITEM` → `Items/Items.xml` | фильтр `SciFi=0`; подтаблица `—` | Items > Show > By Other Filters > Realistic |
| L-SHELL-101 | `CustomFilterToolStripMenuItem_Click` | ItemGridForm (CustomFilterForm) | `ITEM` → `Items/Items.xml` | пользовательский `RowFilter` | Items > Show > By Other Filters > Custom Filter |
| L-SHELL-102 | `CreateNewToolStripMenuItem_Click` | ItemDataForm (NewItemForm) | `ITEM` → `Items/Items.xml` | новая строка, следующий PK | Items > New |
| L-SHELL-104 | `StandardToolStripMenuItem1_Click` | ShowDataGridForm | `MERGE` → `Items/Merges.xml` | — | Items > Merges > Standard |
| L-SHELL-105 | `AttachmentToolStripMenuItem_Click` | ShowDataGridForm | `ATTACHMENTCOMBOMERGE` → `Items/AttachmentComboMerges.xml` | — | Items > Merges > Attachment |
| L-SHELL-107 | `ListingToolStripMenuItem_Click` | ShowChildDataGridForm | `ATTACHMENT` → `Items/Attachments.xml` | — | Items > Attachments > Full List |
| L-SHELL-108 | `InfoToolStripMenuItem_Click` | ShowDataGridForm | `ATTACHMENTINFO` → `Items/AttachmentInfo.xml` | — | Items > Attachments > Info |
| L-SHELL-109 | `IncompatibleToolStripMenuItem_Click` | ShowChildDataGridForm | `INCOMPATIBLEATTACHMENT` → `Items/IncompatibleAttachments.xml` | — | Items > Attachments > Incompatibilities |
| L-SHELL-110 | `LaunchablesToolStripMenuItem_Click` | ShowChildDataGridForm | `LAUNCHABLE` → `Items/Launchables.xml` | — | Items > Launchables |
| L-SHELL-111 | `CompatibleFaceItemsToolStripMenuItem_Click` | ShowChildDataGridForm | `COMPATIBLEFACEITEM` → `Items/CompatibleFaceItems.xml` | — | Items > Compatible Face Items |
| L-SHELL-112 | `TransformationsToolStripMenuItem_Click` | ShowChildDataGridForm | `TRANSFORM` → `Items/Item_Transformations.xml` | — | Items > Transformations |
| L-SHELL-113 | `RandToolStripMenuItem_Click` | ShowDataGridForm | `RANDOMITEM` → `Items/RandomItem.xml` | — | Items > Random Items |
| L-SHELL-115 | `IMPItemsToolStripMenuItem_Click` | ShowLookupGridForm | `IMPITEMCHOICES` → `Inventory/IMPItemChoices.xml` | — | Inventories > IMP Items |
| L-SHELL-118 | `EnemyGunsAdminToolStripMenuItem_Click` | ShowLookupGridForm | `ENEMYGUNCHOICESADMIN` → `Inventory/GunChoices_Enemy_Admin.xml` | — | Inventories > Enemy Items > Guns > Admin |
| L-SHELL-119 | `EnemyGunsRegularToolStripMenuItem_Click` | ShowLookupGridForm | `ENEMYGUNCHOICESREGULAR` → `Inventory/GunChoices_Enemy_Regular.xml` | — | Inventories > Enemy Items > Guns > Regular |
| L-SHELL-120 | `EnemyGunsEliteToolStripMenuItem_Click` | ShowLookupGridForm | `ENEMYGUNCHOICESELITE` → `Inventory/GunChoices_Enemy_Elite.xml` | — | Inventories > Enemy Items > Guns > Elite |
| L-SHELL-121 | `EnemyGunsDefaultToolStripMenuItem_Click` | ShowLookupGridForm | `ENEMYGUNCHOICESDEFAULT` → `Inventory/EnemyGunChoices.xml` | — | Inventories > Enemy Items > Guns > Default |
| L-SHELL-123 | `EnemyItemsAdminToolStripMenuItem1_Click` | ShowLookupGridForm | `ENEMYITEMCHOICESADMIN` → `Inventory/ItemChoices_Enemy_Admin.xml` | — | Inventories > Enemy Items > Items > Admin |
| L-SHELL-124 | `EnemyItemsRegularToolStripMenuItem1_Click` | ShowLookupGridForm | `ENEMYITEMCHOICESREGULAR` → `Inventory/ItemChoices_Enemy_Regular.xml` | — | Inventories > Enemy Items > Items > Regular |
| L-SHELL-125 | `EnemyItemsEliteToolStripMenuItem1_Click` | ShowLookupGridForm | `ENEMYITEMCHOICESELITE` → `Inventory/ItemChoices_Enemy_Elite.xml` | — | Inventories > Enemy Items > Items > Elite |
| L-SHELL-126 | `EnemyItemsDefaultToolStripMenuItem_Click` | ShowLookupGridForm | `ENEMYITEMCHOICESDEFAULT` → `Inventory/EnemyItemChoices.xml` | — | Inventories > Enemy Items > Items > Default |
| L-SHELL-127 | `EnemyAmmoToolStripMenuItem_Click` | ShowLookupGridForm | `ENEMYAMMOCHOICES` → `— (FileName не задан в DatabaseCode)` | — | Inventories > Enemy Items > Ammo |
| L-SHELL-130 | `MilitiaGunsGreenToolStripMenuItem_Click` | ShowLookupGridForm | `MILITIAGUNCHOICESGREEN` → `Inventory/GunChoices_Militia_Green.xml` | — | Inventories > Militia Items > Guns > Green |
| L-SHELL-131 | `MilitiaGunsRegularToolStripMenuItem_Click` | ShowLookupGridForm | `MILITIAGUNCHOICESREGULAR` → `Inventory/GunChoices_Militia_Regular.xml` | — | Inventories > Militia Items > Guns > Regular |
| L-SHELL-132 | `MilitiaGunsEliteToolStripMenuItem_Click` | ShowLookupGridForm | `MILITIAGUNCHOICESELITE` → `Inventory/GunChoices_Militia_Elite.xml` | — | Inventories > Militia Items > Guns > Elite |
| L-SHELL-134 | `MilitiaItemsGreenToolStripMenuItem_Click` | ShowLookupGridForm | `MILITIAITEMCHOICESGREEN` → `Inventory/ItemChoices_Militia_Green.xml` | — | Inventories > Militia Items > Items > Green |
| L-SHELL-135 | `MilitiaItemsRegularToolStripMenuItem_Click` | ShowLookupGridForm | `MILITIAITEMCHOICESREGULAR` → `Inventory/ItemChoices_Militia_Regular.xml` | — | Inventories > Militia Items > Items > Regular |
| L-SHELL-136 | `MilitiaItemsEliteToolStripMenuItem_Click` | ShowLookupGridForm | `MILITIAITEMCHOICESELITE` → `Inventory/ItemChoices_Militia_Elite.xml` | — | Inventories > Militia Items > Items > Elite |
| L-SHELL-138 | `EnemyWeaponDropRateToolStripMenuItem_Click` | ShowLookupGridForm | `EnemyWeaponDrops` → `Inventory/EnemyWeaponDrops.xml` | — | Inventories > Drop Rates > Weapons |
| L-SHELL-139 | `EnemyAmmoDropRateToolStripMenuItem_Click` | ShowDataGridForm | `EnemyAmmoDrops` → `Inventory/EnemyAmmoDrops.xml` | — | Inventories > Drop Rates > Ammo |
| L-SHELL-140 | `EnemyArmourDropRateToolStripMenuItem_Click` | ShowLookupGridForm | `EnemyArmourDrops` → `Inventory/EnemyArmourDrops.xml` | — | Inventories > Drop Rates > Armour |
| L-SHELL-141 | `EnemyExplosiveDropRateToolStripMenuItem_Click` | ShowLookupGridForm | `EnemyExplosiveDrops` → `Inventory/EnemyExplosiveDrops.xml` | — | Inventories > Drop Rates > Explosives |
| L-SHELL-142 | `EnemyMiscDropRateToolStripMenuItem_Click` | ShowLookupGridForm | `EnemyMiscDrops` → `Inventory/EnemyMiscDrops.xml` | — | Inventories > Drop Rates > Misc |
| L-SHELL-144 | `AlbertoToolStripMenuItem_Click` | ShowDataGridForm | `AlbertoInventory` → `NPCInventory/AlbertoInventory.xml` | — | Inventories > Shopkeepers > Alberto |
| L-SHELL-145 | `ArnieToolStripMenuItem_Click` | ShowDataGridForm | `ArnieInventory` → `NPCInventory/ArnieInventory.xml` | — | Inventories > Shopkeepers > Arnie |
| L-SHELL-146 | `CarloToolStripMenuItem_Click` | ShowDataGridForm | `CarloInventory` → `NPCInventory/CarloInventory.xml` | — | Inventories > Shopkeepers > Carlo |
| L-SHELL-147 | `DevinToolStripMenuItem_Click` | ShowDataGridForm | `DevinInventory` → `NPCInventory/DevinInventory.xml` | — | Inventories > Shopkeepers > Devin |
| L-SHELL-148 | `ElginToolStripMenuItem_Click` | ShowDataGridForm | `ElginInventory` → `NPCInventory/ElginInventory.xml` | — | Inventories > Shopkeepers > Elgin |
| L-SHELL-149 | `FrankToolStripMenuItem_Click` | ShowDataGridForm | `FrankInventory` → `NPCInventory/FrankInventory.xml` | — | Inventories > Shopkeepers > Frank |
| L-SHELL-150 | `FranzToolStripMenuItem_Click` | ShowDataGridForm | `FranzInventory` → `NPCInventory/FranzInventory.xml` | — | Inventories > Shopkeepers > Franz |
| L-SHELL-151 | `FredoToolStripMenuItem_Click` | ShowDataGridForm | `FredoInventory` → `NPCInventory/FredoInventory.xml` | — | Inventories > Shopkeepers > Fredo |
| L-SHELL-152 | `GabbyToolStripMenuItem_Click` | ShowDataGridForm | `GabbyInventory` → `NPCInventory/GabbyInventory.xml` | — | Inventories > Shopkeepers > Gabby |
| L-SHELL-153 | `HerveToolStripMenuItem_Click` | ShowDataGridForm | `HerveInventory` → `NPCInventory/HerveInventory.xml` | — | Inventories > Shopkeepers > Herve |
| L-SHELL-154 | `HowardToolStripMenuItem_Click` | ShowDataGridForm | `HowardInventory` → `NPCInventory/HowardInventory.xml` | — | Inventories > Shopkeepers > Howard |
| L-SHELL-155 | `JakeToolStripMenuItem_Click` | ShowDataGridForm | `JakeInventory` → `NPCInventory/JakeInventory.xml` | — | Inventories > Shopkeepers > Jake |
| L-SHELL-156 | `KeithToolStripMenuItem_Click` | ShowDataGridForm | `KeithInventory` → `NPCInventory/KeithInventory.xml` | — | Inventories > Shopkeepers > Keith |
| L-SHELL-157 | `MannyToolStripMenuItem_Click` | ShowDataGridForm | `MannyInventory` → `NPCInventory/MannyInventory.xml` | — | Inventories > Shopkeepers > Manny |
| L-SHELL-158 | `MickeyToolStripMenuItem_Click` | ShowDataGridForm | `MickeyInventory` → `NPCInventory/MickeyInventory.xml` | — | Inventories > Shopkeepers > Mickey |
| L-SHELL-159 | `PerkoToolStripMenuItem_Click` | ShowDataGridForm | `PerkoInventory` → `NPCInventory/PerkoInventory.xml` | — | Inventories > Shopkeepers > Perko |
| L-SHELL-160 | `PeterToolStripMenuItem_Click` | ShowDataGridForm | `PeterInventory` → `NPCInventory/PeterInventory.xml` | — | Inventories > Shopkeepers > Peter |
| L-SHELL-161 | `SamToolStripMenuItem_Click` | ShowDataGridForm | `SamInventory` → `NPCInventory/SamInventory.xml` | — | Inventories > Shopkeepers > Sam |
| L-SHELL-162 | `TonyToolStripMenuItem_Click` | ShowDataGridForm | `TonyInventory` → `NPCInventory/TonyInventory.xml` | — | Inventories > Shopkeepers > Tony |
| L-SHELL-163 | `MercStartingGearToolStripMenuItem_Click` | ShowMercGearGridForm | `MERCGEAR` → `Inventory/MercStartingGear.xml` | — | Inventories > Merc Starting Gear |

## Open questions

1. **`DrugsItemFilterToolStripMenuItem` без `Text`.** В дизайнере заданы только `Name` и `Size` (Designer:946) — пункт «By Other Filters» отображается пустым. Присвоения `Text` в коде не найдено; назначение подписи неясно.
2. **`Tables.EnemyAmmo` (`ENEMYAMMOCHOICES`) без `TableProperty.FileName`.** В `DatabaseCode.vb:2070-2088` файл не задан, тогда как соседние drop-таблицы его имеют. Где хранится/откуда загружается `ENEMYAMMOCHOICES` — не установлено.
3. **`FoodTableLayout_Paint`** (ItemDataForm.vb:1590) не имеет `Handles` и не подключается через `AddHandler`; вызывается ли он дизайнером (имя не совпадает с типичным `Paint`-событием) — неясно.
4. **`_keyDown`** (GridForm.vb:19, 98, 102) выставляется при Alt, но нигде не читается. Предполагаемое поведение drag&drop при Alt не реализовано.
5. **Двойной вызов отмены в `BaseDataForm`.** Кнопка Cancel вызывает `CancelButtonClicked` + `CancelData` (BaseDataForm.vb:138-142), затем `FormClosing` при `UserClosing` и `DialogResult <> OK` вызывает их повторно (144-149). Идемпотентность `CancelData`/`DoCancelData` для конкретных форм не проверялась.
6. **Закрытие `ItemDataForm`/`MercGearDataForm` без OK.** `FormClosing` вызывает `CancelData` → `CancelEdit` + `DoCancelData` (откат гридов). Есть ли сценарий, где пользователь ожидает сохранение при закрытии крестиком, — вопрос продуктового поведения; подтверждения/предупреждения нет.
7. **Невалидный фильтр.** В сеттере `Filter` (GridForm.vb:60-71) `_customFilter` присваивается до `Try`; при ошибке `RowFilter` сбрасывается к `_origFilter`, но `_customFilter` остаётся невалидным. Последующее «Remove Filter» очистит его, но промежуточное состояние в UI не определено.
8. **Повторное открытие Custom Filter.** `ShowItemGridForm` дедуплицирует по заголовку (`MainForm.FormOpen`); если форма «Items - Custom Filter» уже открыта, новый фильтр из диалога не применяется (MainForm.vb:202-209, 590-594). Ожидаемое поведение не задокументировано.
9. **Смена активного набора.** `DataToolStripMenuItem_Click` (MainForm.vb:674) не закрывает MDI-окна и не пересобирает меню Calibers; окна остаются привязанными к прежнему `DataManager`. Нужно ли закрывать/переоткрывать их — неясно.
10. **Два code maker-а на одну таблицу.** `AttachmentClassCodeMaker` и `AttachmentPointCodeMaker` оба создают `BitCodeMakerForm(_activeDataSet, text, Tables.AttachmentPoints)` (MainForm.vb:355, 363), хотя название первого предполагает `AttachmentClass`. Оба пункта скрыты.
11. **`AboutForm.Visible = True`** (MainForm.vb:765) использует default instance без `MdiParent`/owner; поведение при повторных кликах и при закрытии главного окна не проверялось.
12. **Сохранение схемы.** `ColumnSelectForm.OkAction` и `GridForm.SaveColumnWidths` вызывают `Database.SaveSchema()`; путь записи схемы (`WorkingDirectory` vs `TableDirectory`) в этом инвентаре не проверялся.



