# M23a0: инвентаризация полей ItemDataForm (расширенная карта)

Источник: legacy `References/xml-editor`; аудит только чтением. Дата: 2026-10-24. Milestone: M23a0.

Основной инвентарь UI (`M23A-PARITY/00-inventory-ui.md`, строки `L-ITEM-001..046`) описывает `ItemDataForm` на уровне команд и группирует флаговые обработчики (в основном инвентаре указано 69, фактически 70) и 16 счётчиков в одну строку. Здесь те же элементы развёрнуты до уровня отдельного `Tag`-связывания и отдельного флага. Повторный аудит переписанного редактора не выполнялся.

## 1. Сводка

| Показатель | Значение |
|---|---:|
| `Tag`-присваиваний в `ItemDataForm.Designer.vb` | 354 |
| Уникальных имён полей (Tag) | 346 |
| Живых контролов с Tag | 349 |
| Мёртвых контролов с Tag (не добавлены в контейнер) | 5 (`TextBox3`, `TextBox4`, `TextBox5`, `TextBox6`, `TextBox7`) |
| Эффективных привязок через `BindControls` | 348 (`ListBox` не поддержан) |
| Обработчиков checkbox/radio флагов | 70 (66 `CheckBox` + 4 `RadioButton`), не 69 |
| Флагов в `ItemFlags` / `ItemFlags2` | 64 / 43 = 107 |
| Из них с отдельным контролом | 29 / 41 = 70 |
| Только через `CheckedListBox` | 35 / 2 = 37 |
| Реальных XML-флагов с UI | 105; ещё 2 (`Detonator_UNUSED`, `RemoteDetonator_UNUSED`) — только пункты списка без XML-поля |
| Счётчиков описаний | 16 |
| `ComboBox` с Tag | 66; `DataSource`/`DisplayMember`/`ValueMember` в дизайнере — 0 |
| `DataGridView` | 7 (Tag задаётся в runtime) |
| `CheckedListBox` | 3 (`ItemFlags`, `ItemFlags2`, `Drugs`) |
| Мёртвых контролов всего | 27 (включая 5 с Tag) |
| Дублирующихся Tag | 8 полей (5 дублей мертвы) |
| TLP-ячеек с наложением контрола и Label | 4 |

### 1.1. Расхождения с `00-inventory-ui.md`

| Строка основного инвентаря | В основном инвентаре | Фактически |
|---|---|---|
| `L-ITEM-020` | «Для 36 индексов без чекбоксов ничего не делает» | 35 `Case ItemFlags.*` (индексы 0–11, 13–33, 60–61) |
| `L-ITEM-022` | «69 однотипных обработчиков» | 70 обработчиков (29 для `ItemFlags` + 41 для `ItemFlags2`) |
| `L-ITEM-006` | 64 + 43 флага | верно |
| `L-ITEM-017` | 16 обработчиков счётчиков | верно |

Причина расхождения `L-ITEM-022`: `itemFlagsArray` содержит 29 контролов, `itemFlags2Array` — 41; всего 70. Проверено по `Handles <control>.CheckedChanged` (ItemDataForm.vb:387–779).

## 2. Соглашения по полям и видимости

`Tag` контрола — это имя колонки `DataView` (не всегда буквальное имя XML-элемента). `BaseDataForm.BindControls` (BaseDataForm.vb:52–91) выбирает свойство по типу контрола: `TextBox`/`Label`/`MaskedTextBox` → `Text`; `CheckBox`/`RadioButton` → `Checked`; `NumericUpDown` → `Value`; `PictureBox` → `Image`; `ComboBox` → `SelectedValue`. `ListBox` не поддержан: его Tag не связывается. Для `ComboBox` `DataSource`, `DisplayMember` и `ValueMember` выставляются только при наличии у колонки свойств `Lookup_Table`, `Lookup_TextColumn`, `Lookup_ValueColumn`, `Lookup_Filter`, `Lookup_Sort` (BaseDataForm.vb:65–80).

Префиксы имён колонок:

| Префикс Tag | Источник |
|---|---|
| `Weapon…` | `Weapons.xml` (1:1 к `ITEM` по `uiIndex`), имя поля — суффикс после `Weapon` |
| `Magazine…` | `Magazines.xml` |
| `Armour…` | `Armours.xml` |
| `Explosive…` | `Explosives.xml` |
| `LoadBearingEquipment…` | `LoadBearingEquipment.xml` |
| `GermanItem…`, `RussianItem…`, `PolishItem…`, `FrenchItem…`, `ItalianItem…`, `DutchItem…`, `ChineseItem…` | оверлей `<Language>.Items.xml`; суффикс — имя элемента (`szItemName`, `szLongItemName`, `szItemDesc`, `szBRName`, `szBRDesc`) |
| `DefaultAttachment0…9`, `AvailableAttachmentPoint0…9` | повторяющиеся элементы `Items.xml`; synthetic-колонки с суффиксом, обратное имя задаётся `SourceColumnName` в `ItemTable` |
| `FlatBase1…3`, `PercentBase1…3`, … , `AimLevels1…3` | `Items.xml`: `STAND_MODIFIERS` / `CROUCH_MODIFIERS` / `PRONE_MODIFIERS`; суффикс 1/2/3 — стойка |
| `ItemImage` | runtime-колонка `ITEM` (`System.Drawing.Image`), в XML не сохраняется (`ItemTable.AddImageColumn`, удаляется перед записью) |
| остальные | прямая колонка `Items.xml` (`ITEM`) |

Коды видимости в карте полей: `все классы`; `все, кроме …`; `Gun, Launcher, …`; `LANG: <язык>` — страница существует только при наличии `<Language>.Items.xml` (ItemDataForm.vb:16–55); `HEADER (всегда)` — элемент шапки формы; `DEAD` — контрол не добавлен ни в один контейнер. `Else` в `DisplayTabs` — классы `None`, `Money` и любые неизвестные значения (ItemDataForm.vb:1332–1345).

## 3. Карта полей (`L-IFIELD-001…354`)

Вкладка и группа определены по containment-дереву `Controls.Add` (ближайший предок `TabPage`/`GroupBox`), не по координатам. Порядок: по вкладкам `ItemTab`, затем по группам, внутри — по строке `Tag` в дизайнере. Label берётся из статического `Label` в той же ячейке/строке `TableLayoutPanel` либо из ближайшего статического `Label` того же контейнера; если однозначная связь не найдена, поле пустое. Для языковых вкладок указан overlay-файл и поле; для `Weapon*`, `Magazine*`, `Armour*`, `Explosive*`, `LoadBearingEquipment*` — соответствующая 1:1-таблица.


### Description > English / —

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-001 | Description > English | `NameTextBox` | TextBox | `szItemName` | Name: | Text | все классы | 1467 | base Items.xml: szItemName |
| L-IFIELD-002 | Description > English | `LongNameTextBox` | TextBox | `szLongItemName` | Long Name: | Text | все классы | 1487 | дубль Tag: Label143 (D12453); base Items.xml: szLongItemName |
| L-IFIELD-003 | Description > English | `DescriptionTextBox` | TextBox | `szItemDesc` | Description: | Text | все классы | 1508 | см. L-ICOUNT-001; base Items.xml: szItemDesc |

### Description > English / Bobby Ray's (GroupBox1)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-004 | Description > English | `BRNameTextBox` | TextBox | `szBRName` | Name: | Text | все классы | 1416 | base Items.xml: szBRName |
| L-IFIELD-005 | Description > English | `BRDescriptionTextBox` | TextBox | `szBRDesc` | Description: | Text | все классы | 1437 | см. L-ICOUNT-002; base Items.xml: szBRDesc |

### Description > German / —

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-006 | Description > German | `GermanNameTextBox` | TextBox | `GermanItemszItemName` | Name: | Text | LANG: German | 1647 | overlay German.Items.xml: szItemName |
| L-IFIELD-007 | Description > German | `GermanLongNameTextBox` | TextBox | `GermanItemszLongItemName` | Long Name: | Text | LANG: German | 1667 | overlay German.Items.xml: szLongItemName |
| L-IFIELD-008 | Description > German | `GermanDescriptionTextBox` | TextBox | `GermanItemszItemDesc` | Description: | Text | LANG: German | 1688 | см. L-ICOUNT-003; overlay German.Items.xml: szItemDesc |

### Description > German / Bobby Ray's (GroupBox2)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-009 | Description > German | `GermanBRNameTextBox` | TextBox | `GermanItemszBRName` | Name: | Text | LANG: German | 1596 | overlay German.Items.xml: szBRName |
| L-IFIELD-010 | Description > German | `GermanBRDescriptionTextBox` | TextBox | `GermanItemszBRDesc` | Description: | Text | LANG: German | 1617 | см. L-ICOUNT-004; overlay German.Items.xml: szBRDesc |

### Description > Russian / —

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-011 | Description > Russian | `RussianNameTextBox` | TextBox | `RussianItemszItemName` | Name: | Text | LANG: Russian | 1827 | overlay Russian.Items.xml: szItemName |
| L-IFIELD-012 | Description > Russian | `RussianLongNameTextBox` | TextBox | `RussianItemszLongItemName` | Long Name: | Text | LANG: Russian | 1847 | overlay Russian.Items.xml: szLongItemName |
| L-IFIELD-013 | Description > Russian | `RussianDescriptionTextBox` | TextBox | `RussianItemszItemDesc` | Description: | Text | LANG: Russian | 1868 | см. L-ICOUNT-005; overlay Russian.Items.xml: szItemDesc |

### Description > Russian / Bobby Ray's (GroupBox3)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-014 | Description > Russian | `RussianBRNameTextBox` | TextBox | `RussianItemszBRName` | Name: | Text | LANG: Russian | 1776 | overlay Russian.Items.xml: szBRName |
| L-IFIELD-015 | Description > Russian | `RussianBRDescriptionTextBox` | TextBox | `RussianItemszBRDesc` | Description: | Text | LANG: Russian | 1797 | см. L-ICOUNT-006; overlay Russian.Items.xml: szBRDesc |

### Description > Polish / —

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-016 | Description > Polish | `PolishNameTextBox` | TextBox | `PolishItemszItemName` | Name: | Text | LANG: Polish | 2007 | дубль Tag: TextBox5 (D12606); overlay Polish.Items.xml: szItemName |
| L-IFIELD-017 | Description > Polish | `PolishLongNameTextBox` | TextBox | `PolishItemszLongItemName` | Long Name: | Text | LANG: Polish | 2027 | дубль Tag: TextBox6 (D12624); overlay Polish.Items.xml: szLongItemName |
| L-IFIELD-018 | Description > Polish | `PolishDescriptionTextBox` | TextBox | `PolishItemszItemDesc` | Description: | Text | LANG: Polish | 2048 | см. L-ICOUNT-007; дубль Tag: TextBox7 (D12643); overlay Polish.Items.xml: szItemDesc |

### Description > Polish / Bobby Ray's (GroupBox35)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-019 | Description > Polish | `PolishBRNameTextBox` | TextBox | `PolishItemszBRName` | Name: | Text | LANG: Polish | 1946 | дубль Tag: TextBox3 (D12551); overlay Polish.Items.xml: szBRName |
| L-IFIELD-020 | Description > Polish | `PolishBRDescriptionTextBox` | TextBox | `PolishItemszBRDesc` | Description: | Text | LANG: Polish | 1967 | см. L-ICOUNT-008; дубль Tag: TextBox4 (D12570); overlay Polish.Items.xml: szBRDesc |

### Description > French / —

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-021 | Description > French | `FrenchNameTextBox` | TextBox | `FrenchItemszItemName` | Name: | Text | LANG: French | 2187 | overlay French.Items.xml: szItemName |
| L-IFIELD-022 | Description > French | `FrenchLongNameTextBox` | TextBox | `FrenchItemszLongItemName` | Long Name: | Text | LANG: French | 2207 | overlay French.Items.xml: szLongItemName |
| L-IFIELD-023 | Description > French | `FrenchDescriptionTextBox` | TextBox | `FrenchItemszItemDesc` | Description: | Text | LANG: French | 2228 | см. L-ICOUNT-009; overlay French.Items.xml: szItemDesc |

### Description > French / Bobby Ray's (GroupBox37)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-024 | Description > French | `FrenchBRNameTextBox` | TextBox | `FrenchItemszBRName` | Name: | Text | LANG: French | 2126 | overlay French.Items.xml: szBRName |
| L-IFIELD-025 | Description > French | `FrenchBRDescriptionTextBox` | TextBox | `FrenchItemszBRDesc` | Description: | Text | LANG: French | 2147 | см. L-ICOUNT-010; overlay French.Items.xml: szBRDesc |

### Description > Italian / —

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-026 | Description > Italian | `ItalianNameTextBox` | TextBox | `ItalianItemszItemName` | Name: | Text | LANG: Italian | 2367 | overlay Italian.Items.xml: szItemName |
| L-IFIELD-027 | Description > Italian | `ItalianLongNameTextBox` | TextBox | `ItalianItemszLongItemName` | Long Name: | Text | LANG: Italian | 2387 | overlay Italian.Items.xml: szLongItemName |
| L-IFIELD-028 | Description > Italian | `ItalianDescriptionTextBox` | TextBox | `ItalianItemszItemDesc` | Description: | Text | LANG: Italian | 2408 | см. L-ICOUNT-011; overlay Italian.Items.xml: szItemDesc |

### Description > Italian / Bobby Ray's (GroupBox38)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-029 | Description > Italian | `ItalianBRNameTextBox` | TextBox | `ItalianItemszBRName` | Name: | Text | LANG: Italian | 2306 | overlay Italian.Items.xml: szBRName |
| L-IFIELD-030 | Description > Italian | `ItalianBRDescriptionTextBox` | TextBox | `ItalianItemszBRDesc` | Description: | Text | LANG: Italian | 2327 | см. L-ICOUNT-012; overlay Italian.Items.xml: szBRDesc |

### Description > Dutch / —

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-031 | Description > Dutch | `DutchNameTextBox` | TextBox | `DutchItemszItemName` | Name: | Text | LANG: Dutch | 2537 | overlay Dutch.Items.xml: szItemName |
| L-IFIELD-032 | Description > Dutch | `DutchLongNameTextBox` | TextBox | `DutchItemszLongItemName` | Long Name: | Text | LANG: Dutch | 2567 | overlay Dutch.Items.xml: szLongItemName |
| L-IFIELD-033 | Description > Dutch | `DutchDescriptionTextBox` | TextBox | `DutchItemszItemDesc` | Description: | Text | LANG: Dutch | 2588 | см. L-ICOUNT-013; overlay Dutch.Items.xml: szItemDesc |

### Description > Dutch / Bobby Ray's (GroupBox39)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-034 | Description > Dutch | `DutchBRNameTextBox` | TextBox | `DutchItemszBRName` | Name: | Text | LANG: Dutch | 2486 | overlay Dutch.Items.xml: szBRName |
| L-IFIELD-035 | Description > Dutch | `DutchBRDescriptionTextBox` | TextBox | `DutchItemszBRDesc` | Description: | Text | LANG: Dutch | 2507 | см. L-ICOUNT-014; overlay Dutch.Items.xml: szBRDesc |

### Description > Chinese / —

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-036 | Description > Chinese | `ChineseNameTextBox` | TextBox | `ChineseItemszItemName` | Name: | Text | LANG: Chinese | 2717 | overlay Chinese.Items.xml: szItemName |
| L-IFIELD-037 | Description > Chinese | `ChineseLongNameTextBox` | TextBox | `ChineseItemszLongItemName` | Long Name: | Text | LANG: Chinese | 2747 | overlay Chinese.Items.xml: szLongItemName |
| L-IFIELD-038 | Description > Chinese | `ChineseDescriptionTextBox` | TextBox | `ChineseItemszItemDesc` | Description: | Text | LANG: Chinese | 2768 | см. L-ICOUNT-015; overlay Chinese.Items.xml: szItemDesc |

### Description > Chinese / Bobby Ray's (GroupBox57)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-039 | Description > Chinese | `ChineseBRNameTextBox` | TextBox | `ChineseItemszBRName` | Name: | Text | LANG: Chinese | 2666 | overlay Chinese.Items.xml: szBRName |
| L-IFIELD-040 | Description > Chinese | `ChineseBRDescriptionTextBox` | TextBox | `ChineseItemszBRDesc` | Description: | Text | LANG: Chinese | 2687 | см. L-ICOUNT-016; overlay Chinese.Items.xml: szBRDesc |

### General / Stats (GroupBox31)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-041 | General | `TransportGroupMaxProgressUpDown` | NumericUpDown | `TransportGroupMaxProgress` | Transport Group Max Progress: | Value | все классы | 2874 | — |
| L-IFIELD-042 | General | `TransportGroupMinProgressUpDown` | NumericUpDown | `TransportGroupMinProgress` | Transport Group Min Progress: | Value | все классы | 2885 | — |
| L-IFIELD-043 | General | `PriceUpDown` | NumericUpDown | `usPrice` | Price: $ | Value | все классы | 2934 | — |
| L-IFIELD-044 | General | `WeightUpDown` | NumericUpDown | `ubWeight` | Weight: | Value | все классы | 2958 | — |
| L-IFIELD-045 | General | `PerPocketUpDown` | NumericUpDown | `ubPerPocket` | # Per Pocket: | Value | все классы | 2993 | — |
| L-IFIELD-046 | General | `ItemSizeUpDown` | NumericUpDown | `ItemSize` | Size: | Value | все классы | 3016 | — |
| L-IFIELD-047 | General | `ReliabilityUpDown` | NumericUpDown | `bReliability` | Reliability: | Value | все классы | 3040 | — |
| L-IFIELD-048 | General | `RepairEaseUpDown` | NumericUpDown | `bRepairEase` | Repair Ease: | Value | все классы | 3064 | — |
| L-IFIELD-049 | General | `CoolnessUpDown` | NumericUpDown | `ubCoolness` | Coolness: | Value | все классы | 3087 | — |
| L-IFIELD-050 | General | `CursorComboBox` | ComboBox | `ubCursor` | Cursor: | SelectedValue | все классы | 3103 | — |

### General / Graphics (GroupBox50)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-051 | General | `BigItemImage` | PictureBox | `ItemImage` | Big Image: | Image | все классы | 3148 | Image + LoadImages (vb:797); CommitData пишет ItemImage из Image (vb:953); дубль Tag: HeaderImage (D12466) |
| L-IFIELD-052 | General | `ImageListBox` | ListBox | `ubGraphicNum` | Chooser: | — (не поддержан) | все классы | 3160 | Tag игнорируется BindControls; Tag не связывается: BindControls не поддерживает ListBox; выбор через SelectedIndex (vb:821); дубль Tag: GraphicIndexUpDown (D3233) |
| L-IFIELD-053 | General | `GraphicTypeComboBox` | ComboBox | `ubGraphicType` | Type: | SelectedValue | все классы | 3182 | SelectedIndex задаётся кодом (vb:73, 806, 955); Items — ручной список, не DataSource |
| L-IFIELD-054 | General | `GraphicIndexUpDown` | NumericUpDown | `ubGraphicNum` | Index: | Value | все классы | 3233 | Value + LoadImages (vb:75, 802); CommitData vb:954; дубль Tag: ImageListBox (D3160) |

### Attributes / Flags (GroupBox32)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-055 | Attributes | `BigGunListCheckBox` | CheckBox | `BigGunList` | Tons of Guns | Checked | все классы | 3319 | см. L-IFLAG-046 |
| L-IFIELD-056 | Attributes | `SciFiCheckBox` | CheckBox | `SciFi` | Sci-Fi | Checked | все классы | 3333 | см. L-IFLAG-095 |
| L-IFIELD-057 | Attributes | `ShowStatusCheckBox` | CheckBox | `ShowStatus` | Show Status | Checked | все классы | 3347 | см. L-IFLAG-040 |
| L-IFIELD-058 | Attributes | `NotBuyableCheckBox` | CheckBox | `NotBuyable` | Not Buyable | Checked | все классы | 3360 | см. L-IFLAG-043 |
| L-IFIELD-059 | Attributes | `DefaultUndroppableCheckBox` | CheckBox | `DefaultUndroppable` | Undroppable | Checked | все классы | 3374 | см. L-IFLAG-048 |
| L-IFIELD-060 | Attributes | `NotInEditorCheckBox` | CheckBox | `NotInEditor` | Not In Editor | Checked | все классы | 3387 | см. L-IFLAG-047 |
| L-IFIELD-061 | Attributes | `NewInventoryCheckBox` | CheckBox | `NewInv` | New Inventory Only | Checked | все классы | 3400 | см. L-IFLAG-096 |

### Attributes / Face Items (GroupBox7)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-062 | Attributes | `RobotRemoteControlCheckBox` | CheckBox | `RobotRemoteControl` | Robot Remote Control | Checked | все классы | 3445 | см. L-IFLAG-084 |
| L-IFIELD-063 | Attributes | `GasMaskCheckBox` | CheckBox | `GasMask` | Gas Mask | Checked | все классы | 3458 | см. L-IFLAG-055 |
| L-IFIELD-064 | Attributes | `WalkmanCheckBox` | CheckBox | `Walkman` | Walkman | Checked | все классы | 3471 | см. L-IFLAG-082 |

### Attributes / Battery Powered (GroupBox17)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-065 | Attributes | `BatteriesCheckBox` | CheckBox | `Batteries` | Is Battery | Checked | все классы | 3515 | см. L-IFLAG-071 |
| L-IFIELD-066 | Attributes | `NeedsBatteriesCheckBox` | CheckBox | `NeedsBatteries` | Needs Battery | Checked | все классы | 3528 | см. L-IFLAG-072 |

### Attributes / Properties (GroupBox9)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-067 | Attributes | `RepairableCheckBox` | CheckBox | `Repairable` | Repairable | Checked | все классы | 3583 | см. L-IFLAG-036 |
| L-IFIELD-068 | Attributes | `DamageableCheckBox` | CheckBox | `Damageable` | Damageable | Checked | все классы | 3596 | см. L-IFLAG-035 |
| L-IFIELD-069 | Attributes | `WaterDamagesCheckBox` | CheckBox | `WaterDamages` | Damaged by Water | Checked | все классы | 3610 | см. L-IFLAG-037 |
| L-IFIELD-070 | Attributes | `ElectronicCheckBox` | CheckBox | `Electronic` | Electronic | Checked | все классы | 3623 | см. L-IFLAG-050 |
| L-IFIELD-071 | Attributes | `UnaerodynamicCheckBox` | CheckBox | `Unaerodynamic` | Unaerodynamic | Checked | все классы | 3636 | см. L-IFLAG-049 |
| L-IFIELD-072 | Attributes | `SinksCheckBox` | CheckBox | `Sinks` | Sinks | Checked | все классы | 3650 | см. L-IFLAG-039 |
| L-IFIELD-073 | Attributes | `MetalCheckBox` | CheckBox | `Metal` | Metal | Checked | все классы | 3663 | см. L-IFLAG-038 |
| L-IFIELD-074 | Attributes | `TwoHandedCheckBox` | CheckBox | `TwoHanded` | Two-Handed | Checked | все классы | 3676 | см. L-IFLAG-042 |

### Attributes / Kits (GroupBox6)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-075 | Attributes | `usAdministrationModifierUpDown` | NumericUpDown | `usAdministrationModifier` | Administration Modifier: | Value | все классы | 3744 | — |
| L-IFIELD-076 | Attributes | `usHackingModifierUpDown` | NumericUpDown | `usHackingModifier` | Hacking Modifier: | Value | все классы | 3754 | — |
| L-IFIELD-077 | Attributes | `CamoKitCheckBox` | CheckBox | `CamouflageKit` | Camouflage Kit | Checked | все классы | 3789 | см. L-IFLAG-085 |
| L-IFIELD-078 | Attributes | `ToolKitCheckBox` | CheckBox | `ToolKit` | Tool Kit | Checked | все классы | 3802 | Tag "ToolKit"; DataTable/XML — "Toolkit" (vb:1061); см. L-IFLAG-075 |
| L-IFIELD-079 | Attributes | `HardwareCheckBox` | CheckBox | `Hardware` | Hardware | Checked | все классы | 3815 | см. L-IFLAG-089 |
| L-IFIELD-080 | Attributes | `LocksmithKitCheckBox` | CheckBox | `LocksmithKit` | Locksmith Kit | Checked | все классы | 3828 | см. L-IFLAG-086 |
| L-IFIELD-081 | Attributes | `MedicalKitCheckBox` | CheckBox | `MedicalKit` | Medical Kit | Checked | все классы | 3841 | см. L-IFLAG-077 |
| L-IFIELD-082 | Attributes | `FirstAidKitCheckBox` | CheckBox | `FirstAidKit` | First Aid Kit | Checked | все классы | 3854 | см. L-IFLAG-076 |
| L-IFIELD-083 | Attributes | `RepairModifierUpDown` | NumericUpDown | `RepairModifier` | Modifier (Tool Kit) | Value | все классы | 3891 | — |
| L-IFIELD-084 | Attributes | `LockPickModifierUpDown` | NumericUpDown | `LockPickModifier` | Modifier (Locksmith Kit) | Value | все классы | 3902 | — |
| L-IFIELD-085 | Attributes | `DisarmModifierUpDown` | NumericUpDown | `DisarmModifier` | Defusal Kit Bonus: | Value | все классы | 3924 | — |
| L-IFIELD-086 | Attributes | `SleepModifierUpDown` | NumericUpDown | `SleepModifier` | Sleep Modifier: | Value | все классы | 3946 | — |

### Abilities / Random Items (RandomItemsGroupBox)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-087 | Abilities | `randomitemComboBox` | ComboBox | `randomitem` | Random Item Class | SelectedValue | все, кроме Key, Else | 4026 | — |
| L-IFIELD-088 | Abilities | `randomitemcoolnessmodificatorUpDown` | NumericUpDown | `randomitemcoolnessmodificator` | Coolness Modifier: | Value | все, кроме Key, Else | 4049 | — |

### Abilities / Food: (FoodGroupBox)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-089 | Abilities | `FoodTypeComboBox` | ComboBox | `FoodType` |  | SelectedValue | все, кроме Key, Else | 4087 | — |

### Abilities / Drugs (GroupBox33)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-090 | Abilities | `AlcoholUpDown` | NumericUpDown | `Alcohol` | Alcohol: | Value | все, кроме Key, Else | 4133 | — |
| L-IFIELD-091 | Abilities | `CigaretteCheckBox` | CheckBox | `Cigarette` | Cigarette | Checked | все, кроме Key, Else | 4158 | см. L-IFLAG-104 |
| L-IFIELD-092 | Abilities | `MedicalCheckBox` | CheckBox | `Medical` | Medicine / Drug | Checked | все, кроме Key, Else | 4172 | см. L-IFLAG-090 |

### Abilities / Containers (GroupBox11)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-093 | Abilities | `CanteenCheckBox` | CheckBox | `Canteen` | Canteen | Checked | все, кроме Key, Else | 4224 | см. L-IFLAG-078 |
| L-IFIELD-094 | Abilities | `ContainsLiquidCheckBox` | CheckBox | `ContainsLiquid` | Contains Liquid | Checked | все, кроме Key, Else | 4236 | см. L-IFLAG-092 |
| L-IFIELD-095 | Abilities | `JarCheckBox` | CheckBox | `Jar` | Jar | Checked | все, кроме Key, Else | 4249 | см. L-IFLAG-079 |
| L-IFIELD-096 | Abilities | `GasCanCheckBox` | CheckBox | `GasCan` | Gas Can | Checked | все, кроме Key, Else | 4262 | см. L-IFLAG-091 |

### Abilities / Abilities (GroupBox49)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-097 | Abilities | `MetalDetectorCheckBox` | CheckBox | `MetalDetector` | Metal Detector | Checked | все, кроме Key, Else | 4312 | см. L-IFLAG-054 |
| L-IFIELD-098 | Abilities | `XRayCheckBox` | CheckBox | `XRay` | X-Ray Scanner | Checked | все, кроме Key, Else | 4325 | см. L-IFLAG-073 |
| L-IFIELD-099 | Abilities | `WireCuttersCheckBox` | CheckBox | `WireCutters` | Wire Cutters | Checked | все, кроме Key, Else | 4338 | см. L-IFLAG-074 |
| L-IFIELD-100 | Abilities | `DuckbillCheckBox` | CheckBox | `Duckbill` | Duckbill | Checked | все, кроме Key, Else | 4351 | см. L-IFLAG-060 |

### Abilities / Distractions (GroupBox10)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-101 | Abilities | `AttentionItemCheckBox` | CheckBox | `AttentionItem` | (AI) Attention Item | Checked | все, кроме Key, Else | 4396 | см. L-IFLAG-013 |
| L-IFIELD-102 | Abilities | `CanAndStringCheckBox` | CheckBox | `CanAndString` | Can & String | Checked | все, кроме Key, Else | 4409 | см. L-IFLAG-080 |
| L-IFIELD-103 | Abilities | `RockCheckBox` | CheckBox | `Rock` | Rock | Checked | все, кроме Key, Else | 4422 | см. L-IFLAG-093 |
| L-IFIELD-104 | Abilities | `MarblesCheckBox` | CheckBox | `Marbles` | Marbles | Checked | все, кроме Key, Else | 4435 | см. L-IFLAG-081 |

### Abilities / Detonators / Tripwires (GroupBox8)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-105 | Abilities | `TripWireCheckBox` | CheckBox | `TripWire` | Tripwire | Checked | все, кроме Key, Else | 4479 | см. L-IFLAG-100 |
| L-IFIELD-106 | Abilities | `TripWireActivationCheckBox` | CheckBox | `TripWireActivation` | Activated by Tripwire | Checked | все, кроме Key, Else | 4492 | см. L-IFLAG-099 |
| L-IFIELD-107 | Abilities | `RemoteTriggerCheckBox` | CheckBox | `RemoteTrigger` | Remote Trigger | Checked | все, кроме Key, Else | 4505 | см. L-IFLAG-083 |

### Abilities / Vision (GroupBox5)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-108 | Abilities | `ThermalOpticsCheckBox` | CheckBox | `ThermalOptics` | Thermal Optics | Checked | все, кроме Key, Else | 4585 | см. L-IFLAG-094 |
| L-IFIELD-109 | Abilities | `PercentTunnelVisionUpDown` | NumericUpDown | `PercentTunnelVision` | Tunnel Vision: | Value | все, кроме Key, Else | 4598 | — |
| L-IFIELD-110 | Abilities | `usSpottingUpDown` | NumericUpDown | `usSpotting` | Spotting Modifier: | Value | все, кроме Key, Else | 4609 | — |
| L-IFIELD-111 | Abilities | `FlashLightRangeUpDown` | NumericUpDown | `FlashLightRange` | Flashlight Range: | Value | все, кроме Key, Else | 4620 | — |
| L-IFIELD-112 | Abilities | `BrightLightVisionRangeBonusUpDown` | NumericUpDown | `BrightLightVisionRangeBonus` | Bright Light: | Value | все, кроме Key, Else | 4632 | — |
| L-IFIELD-113 | Abilities | `CaveVisionRangeBonusUpDown` | NumericUpDown | `CaveVisionRangeBonus` | Cave: | Value | все, кроме Key, Else | 4644 | — |
| L-IFIELD-114 | Abilities | `DayVisionRangeBonusUpDown` | NumericUpDown | `DayVisionRangeBonus` | Daytime: | Value | все, кроме Key, Else | 4656 | — |
| L-IFIELD-115 | Abilities | `NightVisionRangeBonusUpDown` | NumericUpDown | `NightVisionRangeBonus` | Nighttime: | Value | все, кроме Key, Else | 4668 | — |
| L-IFIELD-116 | Abilities | `VisionRangeBonusUpDown` | NumericUpDown | `VisionRangeBonus` | General: | Value | все, кроме Key, Else | 4752 | — |

### Abilities / Bonuses (GroupBox42)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-117 | Abilities | `PercentStatusDrainReductionUpDown` | NumericUpDown | `PercentStatusDrainReduction` | Kit Status: | Value | все, кроме Key, Else | 4840 | TLP cell (0,2) совпадает с Label107 "Kit Status:" (D4797/4802) — наложение |
| L-IFIELD-118 | Abilities | `APBonusUpDown` | NumericUpDown | `APBonus` | Action Points: | Value | все, кроме Key, Else | 4875 | — |
| L-IFIELD-119 | Abilities | `HearingRangeBonusUpDown` | NumericUpDown | `HearingRangeBonus` | Hearing Range: | Value | все, кроме Key, Else | 4886 | — |
| L-IFIELD-120 | Abilities | `ItemSizeBonusUpDown` | NumericUpDown | `ItemSizeBonus` | Size Adjust: | Value | все, кроме Key, Else | 4922 | — |

### Abilities / Camouflage (GroupBox4)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-121 | Abilities | `SnowCamoBonusUpDown` | NumericUpDown | `SnowCamoBonus` | Snow: | Value | все, кроме Key, Else | 4989 | — |
| L-IFIELD-122 | Abilities | `DesertCamoBonusUpDown` | NumericUpDown | `DesertCamoBonus` | Desert: | Value | все, кроме Key, Else | 5000 | — |
| L-IFIELD-123 | Abilities | `UrbanCamoBonusUpDown` | NumericUpDown | `UrbanCamoBonus` | Urban: | Value | все, кроме Key, Else | 5011 | — |
| L-IFIELD-124 | Abilities | `CamoBonusUpDown` | NumericUpDown | `CamoBonus` | Wood/Jungle: | Value | все, кроме Key, Else | 5070 | — |
| L-IFIELD-125 | Abilities | `StealthBonusUpDown` | NumericUpDown | `StealthBonus` | Stealth: | Value | все, кроме Key, Else | 5081 | — |
| L-IFIELD-126 | Abilities | `clothestypeComboBox` | ComboBox | `clothestype` | Clothes Type: | SelectedValue | все, кроме Key, Else | 5103 | — |

### NCTH / Stance Modifiers (GroupBox58)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-127 | NCTH | `StandFlatBaseUpDown` | NumericUpDown | `FlatBase1` | Flat Base — Standing | Value | все, кроме Key, LBE, RandomItem, Else | 5264 | XML: Items.xml/STAND_MODIFIERS/FlatBase |
| L-IFIELD-128 | NCTH | `CrouchFlatBaseUpDown` | NumericUpDown | `FlatBase2` | Flat Base — Crouching | Value | все, кроме Key, LBE, RandomItem, Else | 5274 | XML: Items.xml/CROUCH_MODIFIERS/FlatBase |
| L-IFIELD-129 | NCTH | `ProneFlatBaseUpDown` | NumericUpDown | `FlatBase3` | Flat Base — Prone | Value | все, кроме Key, LBE, RandomItem, Else | 5284 | XML: Items.xml/PRONE_MODIFIERS/FlatBase |
| L-IFIELD-130 | NCTH | `StandPercentBaseUpDown` | NumericUpDown | `PercentBase1` | Percent Base — Standing | Value | все, кроме Key, LBE, RandomItem, Else | 5306 | XML: Items.xml/STAND_MODIFIERS/PercentBase |
| L-IFIELD-131 | NCTH | `CrouchPercentBaseUpDown` | NumericUpDown | `PercentBase2` | Percent Base — Crouching | Value | все, кроме Key, LBE, RandomItem, Else | 5316 | XML: Items.xml/CROUCH_MODIFIERS/PercentBase |
| L-IFIELD-132 | NCTH | `PronePercentBaseUpDown` | NumericUpDown | `PercentBase3` | Percent Base — Prone | Value | все, кроме Key, LBE, RandomItem, Else | 5326 | XML: Items.xml/PRONE_MODIFIERS/PercentBase |
| L-IFIELD-133 | NCTH | `StandFlatAimUpDown` | NumericUpDown | `FlatAim1` | Flat Aim — Standing | Value | все, кроме Key, LBE, RandomItem, Else | 5348 | XML: Items.xml/STAND_MODIFIERS/FlatAim |
| L-IFIELD-134 | NCTH | `CrouchFlatAimUpDown` | NumericUpDown | `FlatAim2` | Flat Aim — Crouching | Value | все, кроме Key, LBE, RandomItem, Else | 5358 | XML: Items.xml/CROUCH_MODIFIERS/FlatAim |
| L-IFIELD-135 | NCTH | `ProneFlatAimUpDown` | NumericUpDown | `FlatAim3` | Flat Aim — Prone | Value | все, кроме Key, LBE, RandomItem, Else | 5368 | XML: Items.xml/PRONE_MODIFIERS/FlatAim |
| L-IFIELD-136 | NCTH | `StandPercentCapUpDown` | NumericUpDown | `PercentCap1` | Percent Cap — Standing | Value | все, кроме Key, LBE, RandomItem, Else | 5390 | XML: Items.xml/STAND_MODIFIERS/PercentCap |
| L-IFIELD-137 | NCTH | `CrouchPercentCapUpDown` | NumericUpDown | `PercentCap2` | Percent Cap — Crouching | Value | все, кроме Key, LBE, RandomItem, Else | 5400 | XML: Items.xml/CROUCH_MODIFIERS/PercentCap |
| L-IFIELD-138 | NCTH | `PronePercentCapUpDown` | NumericUpDown | `PercentCap3` | Percent Cap — Prone | Value | все, кроме Key, LBE, RandomItem, Else | 5410 | XML: Items.xml/PRONE_MODIFIERS/PercentCap |
| L-IFIELD-139 | NCTH | `StandPercentHandlingUpDown` | NumericUpDown | `PercentHandling1` | Percent Handling — Standing | Value | все, кроме Key, LBE, RandomItem, Else | 5432 | XML: Items.xml/STAND_MODIFIERS/PercentHandling |
| L-IFIELD-140 | NCTH | `CrouchPercentHandlingUpDown` | NumericUpDown | `PercentHandling2` | Percent Handling — Crouching | Value | все, кроме Key, LBE, RandomItem, Else | 5442 | XML: Items.xml/CROUCH_MODIFIERS/PercentHandling |
| L-IFIELD-141 | NCTH | `PronePercentHandlingUpDown` | NumericUpDown | `PercentHandling3` | Percent Handling — Prone | Value | все, кроме Key, LBE, RandomItem, Else | 5452 | XML: Items.xml/PRONE_MODIFIERS/PercentHandling |
| L-IFIELD-142 | NCTH | `StandPercentTargetTrackingSpeedUpDown` | NumericUpDown | `PercentTargetTrackingSpeed1` | Percent Tracking — Standing | Value | все, кроме Key, LBE, RandomItem, Else | 5474 | XML: Items.xml/STAND_MODIFIERS/PercentTargetTrackingSpeed |
| L-IFIELD-143 | NCTH | `CrouchPercentTargetTrackingSpeedUpDown` | NumericUpDown | `PercentTargetTrackingSpeed2` | Percent Tracking — Crouching | Value | все, кроме Key, LBE, RandomItem, Else | 5484 | XML: Items.xml/CROUCH_MODIFIERS/PercentTargetTrackingSpeed |
| L-IFIELD-144 | NCTH | `PronePercentTargetTrackingSpeedUpDown` | NumericUpDown | `PercentTargetTrackingSpeed3` | Percent Tracking — Prone | Value | все, кроме Key, LBE, RandomItem, Else | 5494 | XML: Items.xml/PRONE_MODIFIERS/PercentTargetTrackingSpeed |
| L-IFIELD-145 | NCTH | `StandPercentDropCompensationUpDown` | NumericUpDown | `PercentDropCompensation1` | Percent Drop Comp — Standing | Value | все, кроме Key, LBE, RandomItem, Else | 5516 | XML: Items.xml/STAND_MODIFIERS/PercentDropCompensation |
| L-IFIELD-146 | NCTH | `CrouchPercentDropCompensationUpDown` | NumericUpDown | `PercentDropCompensation2` | Percent Drop Comp — Crouching | Value | все, кроме Key, LBE, RandomItem, Else | 5526 | XML: Items.xml/CROUCH_MODIFIERS/PercentDropCompensation |
| L-IFIELD-147 | NCTH | `PronePercentDropCompensationUpDown` | NumericUpDown | `PercentDropCompensation3` | Percent Drop Comp — Prone | Value | все, кроме Key, LBE, RandomItem, Else | 5536 | XML: Items.xml/PRONE_MODIFIERS/PercentDropCompensation |
| L-IFIELD-148 | NCTH | `StandPercentMaxCounterForceUpDown` | NumericUpDown | `PercentMaxCounterForce1` | Percent Max CF — Standing | Value | все, кроме Key, LBE, RandomItem, Else | 5558 | XML: Items.xml/STAND_MODIFIERS/PercentMaxCounterForce |
| L-IFIELD-149 | NCTH | `CrouchPercentMaxCounterForceUpDown` | NumericUpDown | `PercentMaxCounterForce2` | Percent Max CF — Crouching | Value | все, кроме Key, LBE, RandomItem, Else | 5568 | XML: Items.xml/CROUCH_MODIFIERS/PercentMaxCounterForce |
| L-IFIELD-150 | NCTH | `PronePercentMaxCounterForceUpDown` | NumericUpDown | `PercentMaxCounterForce3` | Percent Max CF — Prone | Value | все, кроме Key, LBE, RandomItem, Else | 5578 | XML: Items.xml/PRONE_MODIFIERS/PercentMaxCounterForce |
| L-IFIELD-151 | NCTH | `StandPercentCounterForceAccuracyUpDown` | NumericUpDown | `PercentCounterForceAccuracy1` | Percent CF Accuracy — Standing | Value | все, кроме Key, LBE, RandomItem, Else | 5600 | XML: Items.xml/STAND_MODIFIERS/PercentCounterForceAccuracy |
| L-IFIELD-152 | NCTH | `CrouchPercentCounterForceAccuracyUpDown` | NumericUpDown | `PercentCounterForceAccuracy2` | Percent CF Accuracy — Crouching | Value | все, кроме Key, LBE, RandomItem, Else | 5610 | XML: Items.xml/CROUCH_MODIFIERS/PercentCounterForceAccuracy |
| L-IFIELD-153 | NCTH | `PronePercentCounterForceAccuracyUpDown` | NumericUpDown | `PercentCounterForceAccuracy3` | Percent CF Accuracy — Prone | Value | все, кроме Key, LBE, RandomItem, Else | 5620 | XML: Items.xml/PRONE_MODIFIERS/PercentCounterForceAccuracy |
| L-IFIELD-154 | NCTH | `StandPercentAimUpDown` | NumericUpDown | `PercentAim1` | Percent Aim — Standing | Value | все, кроме Key, LBE, RandomItem, Else | 5642 | XML: Items.xml/STAND_MODIFIERS/PercentAim |
| L-IFIELD-155 | NCTH | `CrouchPercentAimUpDown` | NumericUpDown | `PercentAim2` | Percent Aim — Crouching | Value | все, кроме Key, LBE, RandomItem, Else | 5652 | XML: Items.xml/CROUCH_MODIFIERS/PercentAim |
| L-IFIELD-156 | NCTH | `PronePercentAimUpDown` | NumericUpDown | `PercentAim3` | Percent Aim — Prone | Value | все, кроме Key, LBE, RandomItem, Else | 5662 | XML: Items.xml/PRONE_MODIFIERS/PercentAim |
| L-IFIELD-157 | NCTH | `StandAimLevelsUpDown` | NumericUpDown | `AimLevels1` | AimLevel Modifier — Standing | Value | все, кроме Key, LBE, RandomItem, Else | 5684 | XML: Items.xml/STAND_MODIFIERS/AimLevels |
| L-IFIELD-158 | NCTH | `CrouchAimLevelsUpDown` | NumericUpDown | `AimLevels2` | AimLevel Modifier — Crouching | Value | все, кроме Key, LBE, RandomItem, Else | 5694 | XML: Items.xml/CROUCH_MODIFIERS/AimLevels |
| L-IFIELD-159 | NCTH | `ProneAimLevelsUpDown` | NumericUpDown | `AimLevels3` | AimLevel Modifier — Prone | Value | все, кроме Key, LBE, RandomItem, Else | 5704 | XML: Items.xml/PRONE_MODIFIERS/AimLevels |

### NCTH / Items.xml (GroupBox59)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-160 | NCTH | `ScopeMagFactorUpDown` | NumericUpDown | `ScopeMagFactor` | Scope Mag Factor | Value | все, кроме Key, LBE, RandomItem, Else | 5786 | — |
| L-IFIELD-161 | NCTH | `ProjectionFactorUpDown` | NumericUpDown | `ProjectionFactor` | Laser Projection Factor | Value | все, кроме Key, LBE, RandomItem, Else | 5810 | — |
| L-IFIELD-162 | NCTH | `RecoilModifierXUpDown` | NumericUpDown | `RecoilModifierX` | Recoil X Modifier | Value | все, кроме Key, LBE, RandomItem, Else | 5835 | — |
| L-IFIELD-163 | NCTH | `RecoilModifierYUpDown` | NumericUpDown | `RecoilModifierY` | Recoil Y Modifier | Value | все, кроме Key, LBE, RandomItem, Else | 5860 | — |
| L-IFIELD-164 | NCTH | `PercentRecoilModifierUpDown` | NumericUpDown | `PercentRecoilModifier` | Recoil Modifier | Value | все, кроме Key, LBE, RandomItem, Else | 5882 | — |
| L-IFIELD-165 | NCTH | `PercentAccuracyModifierUpDown` | NumericUpDown | `PercentAccuracyModifier` | Accuracy Modifier | Value | все, кроме Key, LBE, RandomItem, Else | 5916 | — |

### NCTH / Weapon.xml (GroupBox60)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-166 | NCTH | `nAccuracyUpDown` | NumericUpDown | `WeaponnAccuracy` | NCTH Accuracy | Value | все, кроме Key, LBE, RandomItem, Else | 5992 | колонка Weapons.xml: nAccuracy |
| L-IFIELD-167 | NCTH | `bRecoilXUpDown` | NumericUpDown | `WeaponbRecoilX` | Recoil X | Value | все, кроме Key, LBE, RandomItem, Else | 6017 | колонка Weapons.xml: bRecoilX |
| L-IFIELD-168 | NCTH | `bRecoilYUpDown` | NumericUpDown | `WeaponbRecoilY` | Recoil Y | Value | все, кроме Key, LBE, RandomItem, Else | 6042 | колонка Weapons.xml: bRecoilY |
| L-IFIELD-169 | NCTH | `ubAimLevelsUpDown` | NumericUpDown | `WeaponubAimLevels` | Default Aim Levles | Value | все, кроме Key, LBE, RandomItem, Else | 6064 | колонка Weapons.xml: ubAimLevels |
| L-IFIELD-170 | NCTH | `ubRecoilDelayUpDown` | NumericUpDown | `WeaponubRecoilDelay` | Recoil Delay | Value | все, кроме Key, LBE, RandomItem, Else | 6086 | колонка Weapons.xml: ubRecoilDelay |
| L-IFIELD-171 | NCTH | `HandlingUpDown` | NumericUpDown | `WeaponHandling` | Weapon Handling | Value | все, кроме Key, LBE, RandomItem, Else | 6107 | колонка Weapons.xml: Handling |

### Bonuses / Melee Weapons (GroupBox51)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-172 | Bonuses | `CrowbarCheckBox` | CheckBox | `Crowbar` | Crowbar | Checked | все, кроме Key, LBE, RandomItem, Else | 6171 | см. L-IFLAG-067 |
| L-IFIELD-173 | Bonuses | `BrassKnucklesCheckBox` | CheckBox | `BrassKnuckles` | Brass Knuckles | Checked | все, кроме Key, LBE, RandomItem, Else | 6185 | см. L-IFLAG-066 |
| L-IFIELD-174 | Bonuses | `MeleeDamageBonusUpDown` | NumericUpDown | `MeleeDamageBonus` | Damage: | Value | все, кроме Key, LBE, RandomItem, Else | 6210 | — |
| L-IFIELD-175 | Bonuses | `CrowbarBonusUpDown` | NumericUpDown | `CrowbarModifier` | Crowbar Bonus: | Value | все, кроме Key, LBE, RandomItem, Else | 6232 | — |

### Bonuses / Silencing (GroupBox52)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-176 | Bonuses | `PercentNoiseReductionUpDown` | NumericUpDown | `PercentNoiseReduction` | Noise Reduction: | Value | все, кроме Key, LBE, RandomItem, Else | 6287 | — |
| L-IFIELD-177 | Bonuses | `HideMuzzleFlashCheckBox` | CheckBox | `HideMuzzleFlash` | Hide Muzzle Flash | Checked | все, кроме Key, LBE, RandomItem, Else | 6311 | см. L-IFLAG-063 |

### Bonuses / Ranged Weapons (GroupBox53)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-178 | Bonuses | `BurstSizeBonusUpDown` | NumericUpDown | `BurstSizeBonus` | Burst Size: | Value | все, кроме Key, LBE, RandomItem, Else | 6391 | — |
| L-IFIELD-179 | Bonuses | `MagSizeBonusUpDown` | NumericUpDown | `MagSizeBonus` | Mag Size: | Value | все, кроме Key, LBE, RandomItem, Else | 6402 | — |
| L-IFIELD-180 | Bonuses | `RangeBonusUpDown` | NumericUpDown | `RangeBonus` | Range: | Value | все, кроме Key, LBE, RandomItem, Else | 6413 | — |
| L-IFIELD-181 | Bonuses | `DamageBonusUpDown` | NumericUpDown | `DamageBonus` | Damage: | Value | все, кроме Key, LBE, RandomItem, Else | 6471 | — |
| L-IFIELD-182 | Bonuses | `RateOfFireBonusUpDown` | NumericUpDown | `RateOfFireBonus` | Shots / 4 Turns: | Value | все, кроме Key, LBE, RandomItem, Else | 6481 | — |
| L-IFIELD-183 | Bonuses | `BulletSpeedBonusUpDown` | NumericUpDown | `BulletSpeedBonus` | Bullet Speed: | Value | все, кроме Key, LBE, RandomItem, Else | 6491 | — |

### Bonuses / To-Hit (GroupBox54)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-184 | Bonuses | `AutoFireToHitBonusUpDown` | NumericUpDown | `AutoFireToHitBonus` | Autofire: | Value | все, кроме Key, LBE, RandomItem, Else | 6578 | — |
| L-IFIELD-185 | Bonuses | `BurstToHitBonusUpDown` | NumericUpDown | `BurstToHitBonus` | Burst: | Value | все, кроме Key, LBE, RandomItem, Else | 6600 | — |
| L-IFIELD-186 | Bonuses | `AimBonusUpDown` | NumericUpDown | `AimBonus` | Aimed Shot: | Value | все, кроме Key, LBE, RandomItem, Else | 6610 | — |
| L-IFIELD-187 | Bonuses | `BestLaserRangeUpDown` | NumericUpDown | `BestLaserRange` | Laser Range: | Value | все, кроме Key, LBE, RandomItem, Else | 6620 | — |
| L-IFIELD-188 | Bonuses | `ToHitBonusUpDown` | NumericUpDown | `ToHitBonus` | General: | Value | все, кроме Key, LBE, RandomItem, Else | 6678 | — |
| L-IFIELD-189 | Bonuses | `MinRangeForAimBonusUpDown` | NumericUpDown | `MinRangeForAimBonus` | Min. Range: | Value | все, кроме Key, LBE, RandomItem, Else | 6724 | — |
| L-IFIELD-190 | Bonuses | `BipodUpDown` | NumericUpDown | `Bipod` | Bipod/Prone: | Value | все, кроме Key, LBE, RandomItem, Else | 6746 | — |

### Bonuses / AP Reductions (GroupBox55)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-191 | Bonuses | `PercentAutofireAPReductionUpDown` | NumericUpDown | `PercentAutofireAPReduction` | Autofire: | Value | все, кроме Key, LBE, RandomItem, Else | 6815 | — |
| L-IFIELD-192 | Bonuses | `PercentBurstFireAPReductionUpDown` | NumericUpDown | `PercentBurstFireAPReduction` | Burst: | Value | все, кроме Key, LBE, RandomItem, Else | 6825 | — |
| L-IFIELD-193 | Bonuses | `PercentReloadTimeAPReductionUpDown` | NumericUpDown | `PercentReloadTimeAPReduction` | Reload: | Value | все, кроме Key, LBE, RandomItem, Else | 6835 | — |
| L-IFIELD-194 | Bonuses | `PercentReadyTimeAPReductionUpDown` | NumericUpDown | `PercentReadyTimeAPReduction` | Ready: | Value | все, кроме Key, LBE, RandomItem, Else | 6845 | — |
| L-IFIELD-195 | Bonuses | `PercentAPReductionUpDown` | NumericUpDown | `PercentAPReduction` | General: | Value | все, кроме Key, LBE, RandomItem, Else | 6915 | — |

### Weapon > General / Dirt (DirtGroupBox)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-196 | Weapon > General | `DirtDamageChanceUpDown` | NumericUpDown | `DamageChance` | Damage Chance: | Value | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7073 | — |
| L-IFIELD-197 | Weapon > General | `DirtIncreaseFactorUpDown` | NumericUpDown | `DirtIncreaseFactor` | Increase Factor: | Value | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7097 | — |

### Weapon > General / Throwing Knives (GroupBox40)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-198 | Weapon > General | `BloodiedItemComboBox` | ComboBox | `BloodiedItem` | Bloody Item: | SelectedValue | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7139 | TLP cell (0,0) совпадает с Label83 "Bloody Item:" (D7116/7117) — наложение |

### Weapon > General / Weapon Flags (GroupBox18)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-199 | Weapon > General | `RocketRifleCheckBox` | CheckBox | `RocketRifle` | Rocket Rifle | Checked | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7195 | см. L-IFLAG-052 |
| L-IFIELD-200 | Weapon > General | `FingerPrintIDCheckBox` | CheckBox | `FingerPrintID` | Fingerprint ID | Checked | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7208 | см. L-IFLAG-053 |
| L-IFIELD-201 | Weapon > General | `EasyUnJamCheckBox` | CheckBox | `WeaponEasyUnjam` | Easy UnJam | Checked | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7221 | прямая Tag-привязка (WeaponEasyUnjam); не входит в ItemFlags/ItemFlags2 и CheckedListBox; колонка Weapons.xml: EasyUnjam |
| L-IFIELD-202 | Weapon > General | `HeavyWeaponCheckBox` | CheckBox | `WeaponHeavyGun` | Heavy Weapon | Checked | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7234 | прямая Tag-привязка (WeaponHeavyGun); не входит в ItemFlags/ItemFlags2 и CheckedListBox; колонка Weapons.xml: HeavyGun |

### Weapon > General / Effectiveness (GroupBox24)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-203 | Weapon > General | `bAccuracyUpDown` | NumericUpDown | `WeaponbAccuracy` | Accuracy: | Value | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7281 | TLP cell (0,1) совпадает с Label87 "Accuracy:" (D7255/7258) — наложение; колонка Weapons.xml: bAccuracy |
| L-IFIELD-204 | Weapon > General | `ubImpactUpDown` | NumericUpDown | `WeaponubImpact` | Damage: | Value | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7303 | колонка Weapons.xml: ubImpact |
| L-IFIELD-205 | Weapon > General | `ubDeadlinessUpDown` | NumericUpDown | `WeaponubDeadliness` | Deadliness: | Value | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7337 | колонка Weapons.xml: ubDeadliness |

### Weapon > General / Speed (GroupBox26)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-206 | Weapon > General | `ubReadyTimeUpDown` | NumericUpDown | `WeaponubReadyTime` | Ready APs: | Value | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7421 | колонка Weapons.xml: ubReadyTime |
| L-IFIELD-207 | Weapon > General | `ubShotsPer4TurnsUpDown` | NumericUpDown | `WeaponubShotsPer4Turns` | Shots/4 Turns: | Value | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7433 | колонка Weapons.xml: ubShotsPer4Turns |
| L-IFIELD-208 | Weapon > General | `BR_ROFUpDown` | NumericUpDown | `BR_ROF` | BR ROF: | Value | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7444 | — |

### Weapon > General / Range (GroupBox25)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-209 | Weapon > General | `usRangeUpDown` | NumericUpDown | `WeaponusRange` | Range: | Value | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7536 | колонка Weapons.xml: usRange |
| L-IFIELD-210 | Weapon > General | `MaxDistForMessyDeathUpDown` | NumericUpDown | `WeaponMaxDistForMessyDeath` | Max Head-pop Distance: | Value | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7547 | колонка Weapons.xml: MaxDistForMessyDeath |

### Weapon > General / General (GroupBox22)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-211 | Weapon > General | `ubWeaponClassComboBox` | ComboBox | `WeaponubWeaponClass` | Class: | SelectedValue | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7604 | колонка Weapons.xml: ubWeaponClass |
| L-IFIELD-212 | Weapon > General | `ubWeaponTypeComboBox` | ComboBox | `WeaponubWeaponType` | Type: | SelectedValue | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 7629 | колонка Weapons.xml: ubWeaponType |

### Weapon > Guns / Reloading (GroupBox30)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-213 | Weapon > Guns | `SwapClipsCheckBox` | CheckBox | `WeaponSwapClips` | Magazine Fed | Checked | Gun, Launcher | 7703 | прямая Tag-привязка (WeaponSwapClips); не входит в ItemFlags/ItemFlags2 и CheckedListBox; колонка Weapons.xml: SwapClips |
| L-IFIELD-214 | Weapon > Guns | `APsToReloadUpDown` | NumericUpDown | `WeaponAPsToReload` | Reload APs: | Value | Gun, Launcher | 7726 | колонка Weapons.xml: APsToReload |
| L-IFIELD-215 | Weapon > Guns | `APsToReloadManuallyUpDown` | NumericUpDown | `WeaponAPsToReloadManually` | Manual Reload APs: | Value | Gun, Launcher | 7735 | колонка Weapons.xml: APsToReloadManually |

### Weapon > Guns / Automatic Fire (GroupBox29)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-216 | Weapon > Guns | `bAutofireShotsPerFiveAPUpDown` | NumericUpDown | `WeaponbAutofireShotsPerFiveAP` | Shots / 5 APs: | Value | Gun, Launcher | 7802 | колонка Weapons.xml: bAutofireShotsPerFiveAP |
| L-IFIELD-217 | Weapon > Guns | `AutoPenaltyUpDown` | NumericUpDown | `WeaponAutoPenalty` | To-Hit Penalty: | Value | Gun, Launcher | 7811 | колонка Weapons.xml: AutoPenalty |
| L-IFIELD-218 | Weapon > Guns | `NoSemiAutoCheckBox` | CheckBox | `WeaponNoSemiAuto` | Full-Auto Only | Checked | Gun, Launcher | 7823 | прямая Tag-привязка (WeaponNoSemiAuto); не входит в ItemFlags/ItemFlags2 и CheckedListBox; колонка Weapons.xml: NoSemiAuto |

### Weapon > Guns / Burst Fire (GroupBox28)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-219 | Weapon > Guns | `ubShotsPerBurstUpDown` | NumericUpDown | `WeaponubShotsPerBurst` | Shots / Burst: | Value | Gun, Launcher | 7905 | колонка Weapons.xml: ubShotsPerBurst |
| L-IFIELD-220 | Weapon > Guns | `ubBurstPenaltyUpDown` | NumericUpDown | `WeaponubBurstPenalty` | To-Hit Penalty: | Value | Gun, Launcher | 7914 | колонка Weapons.xml: ubBurstPenalty |
| L-IFIELD-221 | Weapon > Guns | `bBurstAPUpDown` | NumericUpDown | `WeaponbBurstAP` | AP Cost: | Value | Gun, Launcher | 7923 | колонка Weapons.xml: bBurstAP |

### Weapon > Guns / Animation (GroupBox27)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-222 | Weapon > Guns | `usReloadDelayUpDown` | NumericUpDown | `WeaponusReloadDelay` | Reload Delay: | Value | Gun, Launcher | 8004 | колонка Weapons.xml: usReloadDelay |
| L-IFIELD-223 | Weapon > Guns | `BurstAniDelayUpDown` | NumericUpDown | `WeaponBurstAniDelay` | Burst Fire Delay: | Value | Gun, Launcher | 8014 | колонка Weapons.xml: BurstAniDelay |
| L-IFIELD-224 | Weapon > Guns | `ubBulletSpeedUpDown` | NumericUpDown | `WeaponubBulletSpeed` | Bullet Speed: | Value | Gun, Launcher | 8023 | колонка Weapons.xml: ubBulletSpeed |

### Weapon > Guns / Ammunition (GroupBox23)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-225 | Weapon > Guns | `WeaponubCalibreComboBox` | ComboBox | `WeaponubCalibre` | Caliber: | SelectedValue | Gun, Launcher | 8093 | колонка Weapons.xml: ubCalibre |
| L-IFIELD-226 | Weapon > Guns | `WeaponubMagSizeUpDown` | NumericUpDown | `WeaponubMagSize` | Capacity: | Value | Gun, Launcher | 8103 | колонка Weapons.xml: ubMagSize |

### Weapon > Launchers / Launcher Type (GroupBox20)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-227 | Weapon > Launchers | `GrenadeLauncherRadioButton` | RadioButton | `GrenadeLauncher` | Grenade Launcher | Checked | Launcher | 8207 | см. L-IFLAG-058 |
| L-IFIELD-228 | Weapon > Launchers | `RocketLauncherRadioButton` | RadioButton | `RocketLauncher` | Rocket Launcher | Checked | Launcher | 8220 | см. L-IFLAG-064 |
| L-IFIELD-229 | Weapon > Launchers | `MortarRadioButton` | RadioButton | `Mortar` | Mortar | Checked | Launcher | 8233 | см. L-IFLAG-059 |
| L-IFIELD-230 | Weapon > Launchers | `CannonRadioButton` | RadioButton | `Cannon` | Cannon | Checked | Launcher | 8246 | см. L-IFLAG-051 |

### Weapon > Launchers / Rocket Launcher Options (GroupBox19)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-231 | Weapon > Launchers | `SingleShotRocketLauncherCheckBox` | CheckBox | `SingleShotRocketLauncher` | Single Shot | Checked | Launcher | 8292 | см. L-IFLAG-065 |
| L-IFIELD-232 | Weapon > Launchers | `DiscardedLauncherItemComboBox` | ComboBox | `DiscardedLauncherItem` | Discarded Item: | SelectedValue | Launcher | 8319 | — |

### Weapon > Sounds / Sounds (GroupBox21)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-233 | Weapon > Sounds | `sSoundComboBox` | ComboBox | `WeaponsSound` | Sound: | SelectedValue | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 8486 | колонка Weapons.xml: sSound |
| L-IFIELD-234 | Weapon > Sounds | `sBurstSoundComboBox` | ComboBox | `WeaponsBurstSound` | Burst Sound: | SelectedValue | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 8499 | колонка Weapons.xml: sBurstSound |
| L-IFIELD-235 | Weapon > Sounds | `SilencedSoundComboBox` | ComboBox | `WeaponSilencedSound` | Silenced Sound: | SelectedValue | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 8512 | колонка Weapons.xml: SilencedSound |
| L-IFIELD-236 | Weapon > Sounds | `sSilencedBurstSoundComboBox` | ComboBox | `WeaponsSilencedBurstSound` | Silenced Burst Sound: | SelectedValue | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 8525 | колонка Weapons.xml: sSilencedBurstSound |
| L-IFIELD-237 | Weapon > Sounds | `sReloadSoundComboBox` | ComboBox | `WeaponsReloadSound` | Reload Sound: | SelectedValue | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 8538 | колонка Weapons.xml: sReloadSound |
| L-IFIELD-238 | Weapon > Sounds | `sLocknLoadSoundComboBox` | ComboBox | `WeaponsLocknLoadSound` | Lock 'n' Load Sound: | SelectedValue | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 8551 | колонка Weapons.xml: sLocknLoadSound |
| L-IFIELD-239 | Weapon > Sounds | `ManualReloadSoundComboBox` | ComboBox | `WeaponManualReloadSound` | Manual Reload Sound: | SelectedValue | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 8564 | колонка Weapons.xml: ManualReloadSound |
| L-IFIELD-240 | Weapon > Sounds | `ubAttackVolumeUpDown` | NumericUpDown | `WeaponubAttackVolume` | Attack Volume: | Value | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 8585 | колонка Weapons.xml: ubAttackVolume |
| L-IFIELD-241 | Weapon > Sounds | `ubHitVolumeUpDown` | NumericUpDown | `WeaponubHitVolume` | Hit Volume: | Value | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | 8606 | колонка Weapons.xml: ubHitVolume |

### Attachments / Available Attachment Points (GroupBox63)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-242 | Attachments | `AvailableAttachmentPoint1ComboBox` | ComboBox | `AvailableAttachmentPoint1` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8689 | повторяющийся XML-элемент <AvailableAttachmentPoint>; synthetic column |
| L-IFIELD-243 | Attachments | `AvailableAttachmentPoint2ComboBox` | ComboBox | `AvailableAttachmentPoint2` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8701 | повторяющийся XML-элемент <AvailableAttachmentPoint>; synthetic column |
| L-IFIELD-244 | Attachments | `AvailableAttachmentPoint3ComboBox` | ComboBox | `AvailableAttachmentPoint3` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8713 | повторяющийся XML-элемент <AvailableAttachmentPoint>; synthetic column |
| L-IFIELD-245 | Attachments | `AvailableAttachmentPoint4ComboBox` | ComboBox | `AvailableAttachmentPoint4` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8725 | повторяющийся XML-элемент <AvailableAttachmentPoint>; synthetic column |
| L-IFIELD-246 | Attachments | `AvailableAttachmentPoint5ComboBox` | ComboBox | `AvailableAttachmentPoint5` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8737 | повторяющийся XML-элемент <AvailableAttachmentPoint>; synthetic column |
| L-IFIELD-247 | Attachments | `AvailableAttachmentPoint6ComboBox` | ComboBox | `AvailableAttachmentPoint6` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8749 | повторяющийся XML-элемент <AvailableAttachmentPoint>; synthetic column |
| L-IFIELD-248 | Attachments | `AvailableAttachmentPoint7ComboBox` | ComboBox | `AvailableAttachmentPoint7` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8761 | повторяющийся XML-элемент <AvailableAttachmentPoint>; synthetic column |
| L-IFIELD-249 | Attachments | `AvailableAttachmentPoint8ComboBox` | ComboBox | `AvailableAttachmentPoint8` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8773 | повторяющийся XML-элемент <AvailableAttachmentPoint>; synthetic column |
| L-IFIELD-250 | Attachments | `AvailableAttachmentPoint9ComboBox` | ComboBox | `AvailableAttachmentPoint9` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8785 | повторяющийся XML-элемент <AvailableAttachmentPoint>; synthetic column |
| L-IFIELD-251 | Attachments | `AvailableAttachmentPoint0ComboBox` | ComboBox | `AvailableAttachmentPoint0` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8797 | повторяющийся XML-элемент <AvailableAttachmentPoint>; synthetic column |

### Attachments / Details (GroupBox61)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-252 | Attachments | `LCUpDown` | NumericUpDown | `nasLayoutClass` | Layout Class: | Value | все, кроме Ammo, Key, RandomItem, Else | 8839 | — |

### Attachments / Default Attachments (GroupBox34)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-253 | Attachments | `DefaultAttachment1ComboBox` | ComboBox | `DefaultAttachment1` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8949 | повторяющийся XML-элемент <DefaultAttachment>; synthetic column |
| L-IFIELD-254 | Attachments | `DefaultAttachment2ComboBox` | ComboBox | `DefaultAttachment2` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8961 | повторяющийся XML-элемент <DefaultAttachment>; synthetic column |
| L-IFIELD-255 | Attachments | `DefaultAttachment3ComboBox` | ComboBox | `DefaultAttachment3` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8973 | повторяющийся XML-элемент <DefaultAttachment>; synthetic column |
| L-IFIELD-256 | Attachments | `DefaultAttachment4ComboBox` | ComboBox | `DefaultAttachment4` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8985 | повторяющийся XML-элемент <DefaultAttachment>; synthetic column |
| L-IFIELD-257 | Attachments | `DefaultAttachment5ComboBox` | ComboBox | `DefaultAttachment5` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 8997 | повторяющийся XML-элемент <DefaultAttachment>; synthetic column |
| L-IFIELD-258 | Attachments | `DefaultAttachment6ComboBox` | ComboBox | `DefaultAttachment6` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 9009 | повторяющийся XML-элемент <DefaultAttachment>; synthetic column |
| L-IFIELD-259 | Attachments | `DefaultAttachment7ComboBox` | ComboBox | `DefaultAttachment7` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 9021 | повторяющийся XML-элемент <DefaultAttachment>; synthetic column |
| L-IFIELD-260 | Attachments | `DefaultAttachment8ComboBox` | ComboBox | `DefaultAttachment8` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 9033 | повторяющийся XML-элемент <DefaultAttachment>; synthetic column |
| L-IFIELD-261 | Attachments | `DefaultAttachment9ComboBox` | ComboBox | `DefaultAttachment9` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 9045 | повторяющийся XML-элемент <DefaultAttachment>; synthetic column |
| L-IFIELD-262 | Attachments | `DefaultAttachment0ComboBox` | ComboBox | `DefaultAttachment0` |  | SelectedValue | все, кроме Ammo, Key, RandomItem, Else | 9057 | повторяющийся XML-элемент <DefaultAttachment>; synthetic column |

### Attachment Data > Info / Details (GroupBox62)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-263 | Attachment Data > Info | `BlockIronSightCheckBox` | CheckBox | `BlockIronSight` | Block Iron Sight | Checked | все, кроме Key, LBE, RandomItem, Else | 9156 | см. L-IFLAG-102 |
| L-IFIELD-264 | Attachment Data > Info | `InseparableComboBox` | ComboBox | `Inseparable` | Separability: | SelectedValue | все, кроме Key, LBE, RandomItem, Else | 9172 | — |
| L-IFIELD-265 | Attachment Data > Info | `AttachmentPointComboBox` | ComboBox | `AttachmentPoint` | Attachment Point(s): | SelectedValue | все, кроме Key, LBE, RandomItem, Else | 9197 | — |
| L-IFIELD-266 | Attachment Data > Info | `AttachToPointAPCostUpDown` | NumericUpDown | `AttachToPointAPCost` | AP Cost to Attach to Point: | Value | все, кроме Key, LBE, RandomItem, Else | 9207 | — |
| L-IFIELD-267 | Attachment Data > Info | `HiddenAddonCheckBox` | CheckBox | `HiddenAddon` | Hidden Addon | Checked | все, кроме Key, LBE, RandomItem, Else | 9230 | см. L-IFLAG-041 |
| L-IFIELD-268 | Attachment Data > Info | `HiddenAttachmentCheckBox` | CheckBox | `HiddenAttachment` | Hidden Attachment | Checked | все, кроме Key, LBE, RandomItem, Else | 9244 | см. L-IFLAG-045 |
| L-IFIELD-269 | Attachment Data > Info | `AttachmentCheckBox` | CheckBox | `Attachment` | Attachment | Checked | все, кроме Key, LBE, RandomItem, Else | 9259 | см. L-IFLAG-044 |
| L-IFIELD-270 | Attachment Data > Info | `NasAttachmentClassComboBox` | ComboBox | `nasAttachmentClass` | Custom Attachment Class (NAS): | SelectedValue | все, кроме Key, LBE, RandomItem, Else | 9287 | — |
| L-IFIELD-271 | Attachment Data > Info | `AttachmentSystemComboBox` | ComboBox | `AttachmentSystem` | Attachment System: | SelectedValue | все, кроме Key, LBE, RandomItem, Else | 9312 | — |
| L-IFIELD-272 | Attachment Data > Info | `AttachmentClassComboBox` | ComboBox | `AttachmentClass` | Built-in Attachment Class: | SelectedValue | все, кроме Key, LBE, RandomItem, Else | 9337 | — |

### Explosives / Stats (GroupBox16)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-273 | Explosives | `NumericUpDown2` | NumericUpDown | `ExplosiveubHorizontalDegree` | Horz Degrees: | Value | Bomb, Grenade | 9581 | TLP cell (0,12) совпадает с Label245 "Horz Degrees:" (D9517/9518) — наложение; колонка Explosives.xml: ubHorizontalDegree |
| L-IFIELD-274 | Explosives | `ExplosiveubTypeComboBox` | ComboBox | `ExplosiveubType` | Type: | SelectedValue | Bomb, Grenade | 9618 | колонка Explosives.xml: ubType |
| L-IFIELD-275 | Explosives | `ubAnimationIDComboBox` | ComboBox | `ExplosiveubAnimationID` | Animation: | SelectedValue | Bomb, Grenade | 9643 | колонка Explosives.xml: ubAnimationID |
| L-IFIELD-276 | Explosives | `ubDamageUpDown` | NumericUpDown | `ExplosiveubDamage` | Damage: | Value | Bomb, Grenade | 9666 | колонка Explosives.xml: ubDamage |
| L-IFIELD-277 | Explosives | `ubStunDamageUpDown` | NumericUpDown | `ExplosiveubStunDamage` | Stun Damage: | Value | Bomb, Grenade | 9688 | колонка Explosives.xml: ubStunDamage |
| L-IFIELD-278 | Explosives | `ubStartRadiusUpDown` | NumericUpDown | `ExplosiveubStartRadius` | Start Radius: | Value | Bomb, Grenade | 9710 | колонка Explosives.xml: ubStartRadius |
| L-IFIELD-279 | Explosives | `ubRadiusUpDown` | NumericUpDown | `ExplosiveubRadius` | Final Radius: | Value | Bomb, Grenade | 9731 | колонка Explosives.xml: ubRadius |
| L-IFIELD-280 | Explosives | `ubDurationUpDown` | NumericUpDown | `ExplosiveubDuration` | Duration: | Value | Bomb, Grenade | 9753 | колонка Explosives.xml: ubDuration |
| L-IFIELD-281 | Explosives | `ExplosiveubVolumeUpDown` | NumericUpDown | `ExplosiveubVolume` | Volume: | Value | Bomb, Grenade | 9774 | колонка Explosives.xml: ubVolume |
| L-IFIELD-282 | Explosives | `ubVolatilityUpDown` | NumericUpDown | `ExplosiveubVolatility` | Volatility: | Value | Bomb, Grenade | 9796 | колонка Explosives.xml: ubVolatility |
| L-IFIELD-283 | Explosives | `ubMagSizeUpDown` | NumericUpDown | `ExplosiveubMagSize` | Mag Size: | Value | Bomb, Grenade | 9817 | колонка Explosives.xml: ubMagSize |
| L-IFIELD-284 | Explosives | `usNumFragmentsUpDown` | NumericUpDown | `ExplosiveusNumFragments` | # Fragments: | Value | Bomb, Grenade | 9864 | колонка Explosives.xml: usNumFragments |
| L-IFIELD-285 | Explosives | `ubFragDamageUpDown` | NumericUpDown | `ExplosiveubFragDamage` | Frag Damage: | Value | Bomb, Grenade | 9874 | колонка Explosives.xml: ubFragDamage |
| L-IFIELD-286 | Explosives | `ubFragRangeUpDown` | NumericUpDown | `ExplosiveubFragRange` | Frag Range: | Value | Bomb, Grenade | 9897 | колонка Explosives.xml: ubFragRange |
| L-IFIELD-287 | Explosives | `ubFragTypeComboBox` | ComboBox | `ExplosiveubFragType` | Frag Type: | SelectedValue | Bomb, Grenade | 9910 | колонка Explosives.xml: ubFragType |
| L-IFIELD-288 | Explosives | `NumericUpDown1` | NumericUpDown | `ExplosiveubVerticalDegree` | Vert Degrees: | Value | Bomb, Grenade | 9920 | колонка Explosives.xml: ubVerticalDegree |
| L-IFIELD-289 | Explosives | `IndoorModifierUpDown` | NumericUpDown | `ExplosivebIndoorModifier` | Indoor mod: | Value | Bomb, Grenade | 9957 | колонка Explosives.xml: bIndoorModifier |

### Explosives / Flags (GroupBox15)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-290 | Explosives | `DirectionalCheckBox` | CheckBox | `Directional` | Directional | Checked | Bomb, Grenade | 10004 | см. L-IFLAG-101 |
| L-IFIELD-291 | Explosives | `GLGrenadeCheckBox` | CheckBox | `GLGrenade` | GL Grenade | Checked | Bomb, Grenade | 10017 | см. L-IFLAG-068 |
| L-IFIELD-292 | Explosives | `MineCheckBox` | CheckBox | `Mine` | Mine | Checked | Bomb, Grenade | 10030 | см. L-IFLAG-087 |
| L-IFIELD-293 | Explosives | `LockBombCheckBox` | CheckBox | `LockBomb` | Shaped Charge | Checked | Bomb, Grenade | 10043 | см. L-IFLAG-056 |
| L-IFIELD-294 | Explosives | `FlareCheckBox` | CheckBox | `Flare` | Flare | Checked | Bomb, Grenade | 10056 | см. L-IFLAG-057 |
| L-IFIELD-295 | Explosives | `ExplodeOnImpactCheckBox` | CheckBox | `ExplosivefExplodeOnImpact` | Explode On Impact | Checked | Bomb, Grenade | 10069 | прямая Tag-привязка (ExplosivefExplodeOnImpact); не входит в ItemFlags/ItemFlags2 и CheckedListBox; колонка Explosives.xml: fExplodeOnImpact |

### Ammo / Stats (GroupBox14)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-296 | Ammo | `MagazineubCalibreComboBox` | ComboBox | `MagazineubCalibre` | Caliber: | SelectedValue | Ammo | 10135 | колонка Magazines.xml: ubCalibre |
| L-IFIELD-297 | Ammo | `MagazineubMagSizeUpDown` | NumericUpDown | `MagazineubMagSize` | Magazine Size: | Value | Ammo | 10182 | колонка Magazines.xml: ubMagSize |
| L-IFIELD-298 | Ammo | `ubAmmoTypeComboBox` | ComboBox | `MagazineubAmmoType` | Ammo Type: | SelectedValue | Ammo | 10195 | колонка Magazines.xml: ubAmmoType |
| L-IFIELD-299 | Ammo | `ubMagTypeComboBox` | ComboBox | `MagazineubMagType` | Magazine Type: | SelectedValue | Ammo | 10220 | колонка Magazines.xml: ubMagType |

### Armour / Stats (GroupBox13)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-300 | Armour | `ubProtectionUpDown` | NumericUpDown | `ArmourubProtection` | Protection: | Value | Armour | 10283 | колонка Armours.xml: ubProtection |
| L-IFIELD-301 | Armour | `ubCoverageUpDown` | NumericUpDown | `ArmourubCoverage` | Coverage: | Value | Armour | 10293 | колонка Armours.xml: ubCoverage |
| L-IFIELD-302 | Armour | `ubDegradePercentUpDown` | NumericUpDown | `ArmourubDegradePercent` | Degradation: | Value | Armour | 10303 | колонка Armours.xml: ubDegradePercent |
| L-IFIELD-303 | Armour | `ubArmourClassComboBox` | ComboBox | `ArmourubArmourClass` | Class: | SelectedValue | Armour | 10317 | колонка Armours.xml: ubArmourClass |

### Armour / Flags (GroupBox12)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-304 | Armour | `FlakJacketCheckBox` | CheckBox | `FlakJacket` | Flak Jacket | Checked | Armour | 10417 | см. L-IFLAG-069 |
| L-IFIELD-305 | Armour | `LeatherJacketCheckBox` | CheckBox | `LeatherJacket` | Leather Jacket | Checked | Armour | 10430 | см. L-IFLAG-070 |

### Inventories / Bobby Ray's (GroupBox41)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-306 | Inventories | `BR_NewInventoryUpDown` | NumericUpDown | `BR_NewInventory` | New: | Value | все, кроме RandomItem, Else | 11038 | — |
| L-IFIELD-307 | Inventories | `BR_UsedInventoryUpDown` | NumericUpDown | `BR_UsedInventory` | Used: | Value | все, кроме RandomItem, Else | 11050 | — |

### Pockets / Load Bearing Equipment (LBEGroupBox)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-308 | Pockets | `lbeClassComboBox` | ComboBox | `LoadBearingEquipmentlbeClass` | Class: | SelectedValue | LBE | 11133 | SelectedValue 0..4 -> доступные карманы (vb:1566); колонка LoadBearingEquipment.xml: lbeClass |
| L-IFIELD-309 | Pockets | `lbeComboUpDown` | NumericUpDown | `LoadBearingEquipmentlbeCombo` | Combo #: | Value | LBE | 11180 | колонка LoadBearingEquipment.xml: lbeCombo |
| L-IFIELD-310 | Pockets | `lbeFilledSizeUpDown` | NumericUpDown | `LoadBearingEquipmentlbeFilledSize` | Filled Size: | Value | LBE | 11191 | колонка LoadBearingEquipment.xml: lbeFilledSize |
| L-IFIELD-311 | Pockets | `lbeAvailableVolumeUpdown` | NumericUpDown | `LoadBearingEquipmentlbeAvailableVolume` | Available Volume: | Value | LBE | 11227 | колонка LoadBearingEquipment.xml: lbeAvailableVolume |
| L-IFIELD-312 | Pockets | `lbePocketsAvailableUpDown` | NumericUpDown | `LoadBearingEquipmentlbePocketsAvailable` | Pockets Available: | Value | LBE | 11238 | колонка LoadBearingEquipment.xml: lbePocketsAvailable |
| L-IFIELD-313 | Pockets | `BackpackWeightModifierUpDown` | NumericUpDown | `sBackpackWeightModifier` | Weight Modifier: | Value | LBE | 11262 | — |
| L-IFIELD-314 | Pockets | `AllowClimbingCheckbox` | CheckBox | `AllowClimbing` | Allow Climbing? | Checked | LBE | 11274 | см. L-IFLAG-103 |

### Pockets / Pocket Details (PocketsGroupBox)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-315 | Pockets | `lbePocketIndex1ComboBox` | ComboBox | `LoadBearingEquipmentlbePocketIndex1` | Pocket 1: | SelectedValue | LBE | 11491 | enable/disable и сброс в 0 по классу LBE (vb:1452-1564); колонка LoadBearingEquipment.xml: lbePocketIndex1 |
| L-IFIELD-316 | Pockets | `lbePocketIndex2ComboBox` | ComboBox | `LoadBearingEquipmentlbePocketIndex2` | Pocket 2: | SelectedValue | LBE | 11504 | enable/disable и сброс в 0 по классу LBE (vb:1452-1564); колонка LoadBearingEquipment.xml: lbePocketIndex2 |
| L-IFIELD-317 | Pockets | `lbePocketIndex3ComboBox` | ComboBox | `LoadBearingEquipmentlbePocketIndex3` | Pocket 3: | SelectedValue | LBE | 11517 | enable/disable и сброс в 0 по классу LBE (vb:1452-1564); колонка LoadBearingEquipment.xml: lbePocketIndex3 |
| L-IFIELD-318 | Pockets | `lbePocketIndex4ComboBox` | ComboBox | `LoadBearingEquipmentlbePocketIndex4` | Pocket 4: | SelectedValue | LBE | 11530 | enable/disable и сброс в 0 по классу LBE (vb:1452-1564); колонка LoadBearingEquipment.xml: lbePocketIndex4 |
| L-IFIELD-319 | Pockets | `lbePocketIndex5ComboBox` | ComboBox | `LoadBearingEquipmentlbePocketIndex5` | Pocket 5: | SelectedValue | LBE | 11543 | enable/disable и сброс в 0 по классу LBE (vb:1452-1564); колонка LoadBearingEquipment.xml: lbePocketIndex5 |
| L-IFIELD-320 | Pockets | `lbePocketIndex6ComboBox` | ComboBox | `LoadBearingEquipmentlbePocketIndex6` | Pocket 6: | SelectedValue | LBE | 11556 | enable/disable и сброс в 0 по классу LBE (vb:1452-1564); колонка LoadBearingEquipment.xml: lbePocketIndex6 |
| L-IFIELD-321 | Pockets | `lbePocketIndex7ComboBox` | ComboBox | `LoadBearingEquipmentlbePocketIndex7` | Pocket 7: | SelectedValue | LBE | 11569 | enable/disable и сброс в 0 по классу LBE (vb:1452-1564); колонка LoadBearingEquipment.xml: lbePocketIndex7 |
| L-IFIELD-322 | Pockets | `lbePocketIndex8ComboBox` | ComboBox | `LoadBearingEquipmentlbePocketIndex8` | Pocket 8: | SelectedValue | LBE | 11582 | enable/disable и сброс в 0 по классу LBE (vb:1452-1564); колонка LoadBearingEquipment.xml: lbePocketIndex8 |
| L-IFIELD-323 | Pockets | `lbePocketIndex9ComboBox` | ComboBox | `LoadBearingEquipmentlbePocketIndex9` | Pocket 9: | SelectedValue | LBE | 11595 | enable/disable и сброс в 0 по классу LBE (vb:1452-1564); колонка LoadBearingEquipment.xml: lbePocketIndex9 |
| L-IFIELD-324 | Pockets | `lbePocketIndex10ComboBox` | ComboBox | `LoadBearingEquipmentlbePocketIndex10` | Pocket 10: | SelectedValue | LBE | 11608 | enable/disable и сброс в 0 по классу LBE (vb:1452-1564); колонка LoadBearingEquipment.xml: lbePocketIndex10 |
| L-IFIELD-325 | Pockets | `lbePocketIndex11ComboBox` | ComboBox | `LoadBearingEquipmentlbePocketIndex11` | Pocket 11: | SelectedValue | LBE | 11621 | enable/disable и сброс в 0 по классу LBE (vb:1452-1564); колонка LoadBearingEquipment.xml: lbePocketIndex11 |
| L-IFIELD-326 | Pockets | `lbePocketIndex12ComboBox` | ComboBox | `LoadBearingEquipmentlbePocketIndex12` | Pocket 12: | SelectedValue | LBE | 11634 | enable/disable и сброс в 0 по классу LBE (vb:1452-1564); колонка LoadBearingEquipment.xml: lbePocketIndex12 |

### Overheating / Weapon Temperature Values (WeaponTemperatureGroupBox)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-327 | Overheating | `JamThresholdUpDown` | NumericUpDown | `WeaponusOverheatingJamThreshold` | Jam Threshold: | Value | Gun, Launcher | 11708 | колонка Weapons.xml: usOverheatingJamThreshold |
| L-IFIELD-328 | Overheating | `DamageThresholdUpDown` | NumericUpDown | `WeaponusOverheatingDamageThreshold` | Damage Threshold: | Value | Gun, Launcher | 11733 | колонка Weapons.xml: usOverheatingDamageThreshold |
| L-IFIELD-329 | Overheating | `PerShotTemperatureIncreaseUpDown` | NumericUpDown | `WeaponusOverheatingSingleShotTemperature` | Per shot Temperature Increase: | Value | Gun, Launcher | 11759 | колонка Weapons.xml: usOverheatingSingleShotTemperature |

### Overheating / Item Temperature Values (ItemTemperatureGroupBox)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-330 | Overheating | `BarrelCheckBox` | CheckBox | `barrel` | Barrel | Checked | все, кроме RandomItem | 11815 | Tag "barrel" в нижнем регистре; CommitData/enum — "Barrel" (vb:1084); см. L-IFLAG-098 |
| L-IFIELD-331 | Overheating | `CooldownFactorUpDown` | NumericUpDown | `usOverheatingCooldownFactor` | Cooldown Factor: | Value | все, кроме RandomItem | 11843 | — |
| L-IFIELD-332 | Overheating | `TemperatureModifierUpDown` | NumericUpDown | `overheatTemperatureModificator` | Temperature Modifier: | Value | все, кроме RandomItem | 11869 | — |
| L-IFIELD-333 | Overheating | `CooldownModifierUpDown` | NumericUpDown | `overheatCooldownModificator` | Cooldown Modifier: | Value | все, кроме RandomItem | 11895 | — |
| L-IFIELD-334 | Overheating | `JamThresholdModifierUpDown` | NumericUpDown | `overheatJamThresholdModificator` | Jam Threshold Modifier: | Value | все, кроме RandomItem | 11921 | — |
| L-IFIELD-335 | Overheating | `DamageThresholdModifierUpDown` | NumericUpDown | `overheatDamageThresholdModificator` | Damage Threshold Modifier: | Value | все, кроме RandomItem | 11947 | — |

### Robot / Bonuses (RobotBonusGroupBox)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-336 | Robot | `RobotDamageReductionUpDown` | NumericUpDown | `RobotDamageReduction` | Robot Damage Reduction: | Value | все классы | 12109 | — |
| L-IFIELD-337 | Robot | `RobotUtilitySkillGrantComboBox` | ComboBox | `RobotUtilitySkillGrant` | Utility Slot Bonus Trait: | SelectedValue | все классы | 12132 | Items вручную (vb:347); SelectedIndex читается/пишется (vb:80, 958); Tag SelectedValue сомнителен |
| L-IFIELD-338 | Robot | `RobotChassisSkillGrantComboBox` | ComboBox | `RobotChassisSkillGrant` | Chassis Slot Bonus Trait: | SelectedValue | все классы | 12155 | Items вручную (vb:347); SelectedIndex читается/пишется (vb:79, 957); Tag SelectedValue сомнителен |
| L-IFIELD-339 | Robot | `RobotTargetingSkillGrantComboBox` | ComboBox | `RobotTargetingSkillGrant` | Targeting Slot Bonus Trait: | SelectedValue | все классы | 12178 | Items вручную (vb:347); SelectedIndex читается/пишется (vb:78, 956); Tag SelectedValue сомнителен |
| L-IFIELD-340 | Robot | `RobotAgiBonusUpDown` | NumericUpDown | `RobotAgiBonus` | Agility: | Value | все классы | 12199 | — |
| L-IFIELD-341 | Robot | `RobotStrBonusUpDown` | NumericUpDown | `RobotStrBonus` | Strength: | Value | все классы | 12220 | — |
| L-IFIELD-342 | Robot | `RobotDexBonusUpDown` | NumericUpDown | `RobotDexBonus` | Dexterity: | Value | все классы | 12253 | — |

### Robot / Flags (RobotFlagsGroupBox)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-343 | Robot | `ProvidesRobotCamoCheckBox` | CheckBox | `ProvidesRobotCamo` | Provides Robot Camo | Checked | все классы | 12297 | см. L-IFLAG-105 |
| L-IFIELD-344 | Robot | `ProvidesRobotNightVisionCheckBox` | CheckBox | `ProvidesRobotNightVision` | Provides Robot Nightvision | Checked | все классы | 12310 | см. L-IFLAG-106 |
| L-IFIELD-345 | Robot | `ProvidesRobotLaserBonusCheckBox` | CheckBox | `ProvidesRobotLaserBonus` | Provides Robot Laser Bonus | Checked | все классы | 12323 | см. L-IFLAG-107 |

### Шапка формы / —

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-346 | Шапка формы | `Label143` | Label | `szLongItemName` |  | Text | HEADER (всегда) | 12453 | элемент шапки формы; Label.Text; статический заголовок "Item Name" перезаписывается значением szLongItemName; дубль Tag: LongNameTextBox (D1487); base Items.xml: szLongItemName; заголовок формы |
| L-IFIELD-347 | Шапка формы | `HeaderImage` | PictureBox | `ItemImage` |  | Image | HEADER (всегда) | 12466 | элемент шапки формы; Image; только отображение, обратно не пишется; дубль Tag: BigItemImage (D3148) |
| L-IFIELD-348 | Шапка формы | `uiIndexUpDown` | NumericUpDown | `uiIndex` | ID: | Value | HEADER (всегда) | 12476 | элемент шапки формы; Value; PK, CommitData обрабатывает смену/обмен ID (vb:949-978); ValueChanged -> статус (vb:1584) |
| L-IFIELD-349 | Шапка формы | `usItemClassComboBox` | ComboBox | `usItemClass` |  | SelectedValue | HEADER (всегда) | 12487 | элемент шапки формы; Enabled=False, Size=(0,25) (D12480/12485); ClassNameLabel показывает Text (vb:69); класс меняется через ChangeClassForm |

### — / Bobby Ray's (GroupBox36)

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-350 | — | `TextBox3` | TextBox | `PolishItemszBRName` | Name: | Text | DEAD (не добавлен в контейнер) | 12551 | мертвый контрол: не добавлен ни в один контейнер; Tag не связывается; дубль Tag: PolishBRNameTextBox (D1946); overlay Polish.Items.xml: szBRName |
| L-IFIELD-351 | — | `TextBox4` | TextBox | `PolishItemszBRDesc` | Description: | Text | DEAD (не добавлен в контейнер) | 12570 | мертвый контрол: не добавлен ни в один контейнер; Tag не связывается; дубль Tag: PolishBRDescriptionTextBox (D1967); overlay Polish.Items.xml: szBRDesc |

### — / —

| ID | Вкладка / группа | Контрол | Тип | XML-поле (Tag) | Label | Binding | Видимость | Designer | Примечания |
|---|---|---|---|---|---|---|---|---:|---|
| L-IFIELD-352 | — | `TextBox5` | TextBox | `PolishItemszItemName` |  | Text | DEAD (не добавлен в контейнер) | 12606 | мертвый контрол: не добавлен ни в один контейнер; Tag не связывается; дубль Tag: PolishNameTextBox (D2007); overlay Polish.Items.xml: szItemName |
| L-IFIELD-353 | — | `TextBox6` | TextBox | `PolishItemszLongItemName` |  | Text | DEAD (не добавлен в контейнер) | 12624 | мертвый контрол: не добавлен ни в один контейнер; Tag не связывается; дубль Tag: PolishLongNameTextBox (D2027); overlay Polish.Items.xml: szLongItemName |
| L-IFIELD-354 | — | `TextBox7` | TextBox | `PolishItemszItemDesc` |  | Text | DEAD (не добавлен в контейнер) | 12643 | мертвый контрол: не добавлен ни в один контейнер; Tag не связывается; дубль Tag: PolishDescriptionTextBox (D2048); overlay Polish.Items.xml: szItemDesc |

## 4. Карта флагов (`L-IFLAG-001…107`)

Все 107 индексов двух массивов и обоих `CheckedListBox`. `ItemFlagsCheckedList` содержит 64 пункта (`Designer:12032`), `ItemFlags2CheckedList` — 43 (`Designer:11981`). `itemFlagsArray` — 64 элемента (29 контролов + 35 заглушек), `itemFlags2Array` — 43 (41 контрол + 2 заглушки). Контролы-флаги распределены по вкладкам, а не собраны на `Flags`; на самой вкладке `Flags` только списки.

| ID | Control (Designer) | Handler (vb:line) | CheckedListBox / array | Index | Flag text | XML flag field | Notes |
|---|---|---|---|---:|---|---|---|
| L-IFLAG-001 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 0 | Bloodbag | `Bloodbag` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-002 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 1 | Manpad | `Manpad` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-003 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 2 | Beartrap | `Beartrap` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-004 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 3 | Camera | `Camera` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-005 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 4 | Water Drum | `Waterdrum` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-006 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 5 | Bloodcat Meat | `BloodcatMeat` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-007 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 6 | Cow Meat | `CowMeat` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-008 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 7 | Belt Fed | `Beltfed` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-009 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 8 | Ammo Belt | `Ammobelt` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-010 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 9 | Ammo Belt Vest | `AmmobeltVest` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-011 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 10 | Camo Remover | `CamoRemoval` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-012 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 11 | Cleaning kit | `Cleaningkit` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-013 | `AttentionItemCheckBox` (D4396; Abilities) | AttentionItem_ValueChanged (vb:387) | ItemFlagsCheckedList / itemFlagsArray | 12 | Interesting Item (AI) / контрол: "(AI) Attention Item" | `AttentionItem` | checkbox ↔ list |
| L-IFLAG-014 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 13 | Garotte | `Garotte` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-015 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 14 | Covert Item | `Covert` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-016 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 15 | Corpse | `Corpse` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-017 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 16 | Bloodcat Skin | `BloodcatSkin` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-018 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 17 | No Metal Detection | `NoMetalDetection` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-019 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 18 | Jumping Grenade | `JumpGrenade` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-020 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 19 | Handcuffs | `Handcuffs` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-021 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 20 | Taser | `Taser` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-022 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 21 | Scuba Bottle | `ScubaBottle` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-023 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 22 | Scuba Mask | `ScubaMask` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-024 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 23 | Scuba Fins | `ScubaFins` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-025 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 24 | Tripwire Roll | `TripwireRoll` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-026 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 25 | Radio Set | `Radioset` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-027 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 26 | Signal Shell | `SignalShell` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-028 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 27 | Soda | `Soda` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-029 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 28 | Roof collapse item | `RoofcollapseItem` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-030 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 29 | Disease protection face | `DiseaseprotectionFace` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-031 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 30 | Disease protection hand | `DiseaseprotectionHand` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-032 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 31 | LBE Explosion proof | `LBEexplosionproof` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-033 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 32 | Empty Bloodbag | `EmptyBloodbag` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-034 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 33 | Medical Splint | `MedicalSplint` | только CheckedListBox; обработчик не синхронизирует |
| L-IFLAG-035 | `DamageableCheckBox` (D3596; Attributes) | Damageable_ValueChanged (vb:392) | ItemFlagsCheckedList / itemFlagsArray | 34 | Damageable | `Damageable` | checkbox ↔ list |
| L-IFLAG-036 | `RepairableCheckBox` (D3583; Attributes) | Repairable_ValueChanged (vb:397) | ItemFlagsCheckedList / itemFlagsArray | 35 | Repairable | `Repairable` | checkbox ↔ list |
| L-IFLAG-037 | `WaterDamagesCheckBox` (D3610; Attributes) | WaterDamages_ValueChanged (vb:402) | ItemFlagsCheckedList / itemFlagsArray | 36 | Water damaged / контрол: "Damaged by Water" | `WaterDamages` | checkbox ↔ list |
| L-IFLAG-038 | `MetalCheckBox` (D3663; Attributes) | Metal_ValueChanged (vb:407) | ItemFlagsCheckedList / itemFlagsArray | 37 | Metal | `Metal` | checkbox ↔ list |
| L-IFLAG-039 | `SinksCheckBox` (D3650; Attributes) | Sinks_ValueChanged (vb:412) | ItemFlagsCheckedList / itemFlagsArray | 38 | Sinks | `Sinks` | checkbox ↔ list |
| L-IFLAG-040 | `ShowStatusCheckBox` (D3347; Attributes) | ShowStatus_ValueChanged (vb:417) | ItemFlagsCheckedList / itemFlagsArray | 39 | Show Status | `ShowStatus` | checkbox ↔ list |
| L-IFLAG-041 | `HiddenAddonCheckBox` (D9230; Attachment Data / Info) | HiddenAddon_ValueChanged (vb:422) | ItemFlagsCheckedList / itemFlagsArray | 40 | Hidden addon / контрол: "Hidden Addon" | `HiddenAddon` | checkbox ↔ list |
| L-IFLAG-042 | `TwoHandedCheckBox` (D3676; Attributes) | TwoHanded_ValueChanged (vb:427) | ItemFlagsCheckedList / itemFlagsArray | 41 | Two-handed / контрол: "Two-Handed" | `TwoHanded` | checkbox ↔ list |
| L-IFLAG-043 | `NotBuyableCheckBox` (D3360; Attributes) | NotBuyable_ValueChanged (vb:432) | ItemFlagsCheckedList / itemFlagsArray | 42 | Not Buyable | `NotBuyable` | checkbox ↔ list |
| L-IFLAG-044 | `AttachmentCheckBox` (D9259; Attachment Data / Info) | Attachment_ValueChanged (vb:437) | ItemFlagsCheckedList / itemFlagsArray | 43 | Attachment | `Attachment` | checkbox ↔ list |
| L-IFLAG-045 | `HiddenAttachmentCheckBox` (D9244; Attachment Data / Info) | HiddenAttachment_ValueChanged (vb:442) | ItemFlagsCheckedList / itemFlagsArray | 44 | Hidden Attachment | `HiddenAttachment` | checkbox ↔ list |
| L-IFLAG-046 | `BigGunListCheckBox` (D3319; Attributes) | BigGunList_ValueChanged (vb:447) | ItemFlagsCheckedList / itemFlagsArray | 45 | Tons of Guns | `BigGunList` | checkbox ↔ list |
| L-IFLAG-047 | `NotInEditorCheckBox` (D3387; Attributes) | NotInEditor_ValueChanged (vb:452) | ItemFlagsCheckedList / itemFlagsArray | 46 | Not in Editor / контрол: "Not In Editor" | `NotInEditor` | checkbox ↔ list |
| L-IFLAG-048 | `DefaultUndroppableCheckBox` (D3374; Attributes) | DefaultUndroppable_ValueChanged (vb:457) | ItemFlagsCheckedList / itemFlagsArray | 47 | Undroppable | `DefaultUndroppable` | checkbox ↔ list |
| L-IFLAG-049 | `UnaerodynamicCheckBox` (D3636; Attributes) | Unaerodynamic_ValueChanged (vb:462) | ItemFlagsCheckedList / itemFlagsArray | 48 | Unaerodynamic | `Unaerodynamic` | checkbox ↔ list |
| L-IFLAG-050 | `ElectronicCheckBox` (D3623; Attributes) | Electronic_ValueChanged (vb:467) | ItemFlagsCheckedList / itemFlagsArray | 49 | Electronic | `Electronic` | checkbox ↔ list |
| L-IFLAG-051 | `CannonRadioButton` (D8246; Weapon / Launchers) | Cannon_ValueChanged (vb:472) | ItemFlagsCheckedList / itemFlagsArray | 50 | Cannon | `Cannon` | radio ↔ list |
| L-IFLAG-052 | `RocketRifleCheckBox` (D7195; Weapon / General) | RocketRifle_ValueChanged (vb:477) | ItemFlagsCheckedList / itemFlagsArray | 51 | Rocketrifle / контрол: "Rocket Rifle" | `RocketRifle` | checkbox ↔ list |
| L-IFLAG-053 | `FingerPrintIDCheckBox` (D7208; Weapon / General) | FingerPrintID_ValueChanged (vb:482) | ItemFlagsCheckedList / itemFlagsArray | 52 | Uses fingerprintID / контрол: "Fingerprint ID" | `FingerPrintID` | checkbox ↔ list |
| L-IFLAG-054 | `MetalDetectorCheckBox` (D4312; Abilities) | MetalDetector_ValueChanged (vb:487) | ItemFlagsCheckedList / itemFlagsArray | 53 | Metal Detector | `MetalDetector` | checkbox ↔ list |
| L-IFLAG-055 | `GasMaskCheckBox` (D3458; Attributes) | GasMask_ValueChanged (vb:492) | ItemFlagsCheckedList / itemFlagsArray | 54 | Gasmask / контрол: "Gas Mask" | `GasMask` | checkbox ↔ list |
| L-IFLAG-056 | `LockBombCheckBox` (D10043; Explosives) | LockBomb_ValueChanged (vb:497) | ItemFlagsCheckedList / itemFlagsArray | 55 | Lockbomb / контрол: "Shaped Charge" | `LockBomb` | checkbox ↔ list |
| L-IFLAG-057 | `FlareCheckBox` (D10056; Explosives) | Flare_ValueChanged (vb:502) | ItemFlagsCheckedList / itemFlagsArray | 56 | Flare | `Flare` | checkbox ↔ list |
| L-IFLAG-058 | `GrenadeLauncherRadioButton` (D8207; Weapon / Launchers) | GrenadeLauncher_ValueChanged (vb:507) | ItemFlagsCheckedList / itemFlagsArray | 57 | Grenade Launcher | `GrenadeLauncher` | radio ↔ list |
| L-IFLAG-059 | `MortarRadioButton` (D8233; Weapon / Launchers) | Mortar_ValueChanged (vb:512) | ItemFlagsCheckedList / itemFlagsArray | 58 | Mortar | `Mortar` | radio ↔ list |
| L-IFLAG-060 | `DuckbillCheckBox` (D4351; Abilities) | Duckbill_ValueChanged (vb:517) | ItemFlagsCheckedList / itemFlagsArray | 59 | Duckbill | `Duckbill` | checkbox ↔ list |
| L-IFLAG-061 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 60 | UNUSED | `— (CommitData: Detonator)` | только CheckedListBox; обработчик не синхронизирует; XML-поля нет; init закомментирован (vb:260–261); CommitData сохраняет состояние списка (vb:1044–1045), по умолчанию False |
| L-IFLAG-062 | — (список D12032; Flags / ItemFlags) | ItemFlag_ValueChanged (vb:532) — нет Case | ItemFlagsCheckedList / itemFlagsArray | 61 | UNUSED | `— (CommitData: RemoteDetonator)` | только CheckedListBox; обработчик не синхронизирует; XML-поля нет; init закомментирован (vb:260–261); CommitData сохраняет состояние списка (vb:1044–1045), по умолчанию False |
| L-IFLAG-063 | `HideMuzzleFlashCheckBox` (D6311; Bonuses) | HideMuzzleFlash_ValueChanged (vb:522) | ItemFlagsCheckedList / itemFlagsArray | 62 | Hide Muzzleflash / контрол: "Hide Muzzle Flash" | `HideMuzzleFlash` | checkbox ↔ list |
| L-IFLAG-064 | `RocketLauncherRadioButton` (D8220; Weapon / Launchers) | RocketLauncher_ValueChanged (vb:527) | ItemFlagsCheckedList / itemFlagsArray | 63 | Rocketlauncher / контрол: "Rocket Launcher" | `RocketLauncher` | radio ↔ list |
| L-IFLAG-065 | `SingleShotRocketLauncherCheckBox` (D8292; Weapon / Launchers) | SingleShotRocketLauncher_ValueChanged (vb:576) | ItemFlags2CheckedList / itemFlags2Array | 0 | Single Shot (в списке: SingleShotRocketLauncher) | `SingleShotRocketLauncher` | checkbox ↔ list |
| L-IFLAG-066 | `BrassKnucklesCheckBox` (D6185; Bonuses) | BrassKnuckles_ValueChanged (vb:581) | ItemFlags2CheckedList / itemFlags2Array | 1 | Brass Knuckles (в списке: BrassKnuckles) | `BrassKnuckles` | checkbox ↔ list |
| L-IFLAG-067 | `CrowbarCheckBox` (D6171; Bonuses) | Crowbar_ValueChanged (vb:586) | ItemFlags2CheckedList / itemFlags2Array | 2 | Crowbar | `Crowbar` | checkbox ↔ list |
| L-IFLAG-068 | `GLGrenadeCheckBox` (D10017; Explosives) | GLGrenade_ValueChanged (vb:591) | ItemFlags2CheckedList / itemFlags2Array | 3 | GL Grenade (в списке: GLGrenade) | `GLGrenade` | checkbox ↔ list |
| L-IFLAG-069 | `FlakJacketCheckBox` (D10417; Armour) | Flakjacket_ValueChanged (vb:596) | ItemFlags2CheckedList / itemFlags2Array | 4 | Flak Jacket (в списке: FlakJacket) | `FlakJacket` | checkbox ↔ list |
| L-IFLAG-070 | `LeatherJacketCheckBox` (D10430; Armour) | LeatherJacket_ValueChanged (vb:601) | ItemFlags2CheckedList / itemFlags2Array | 5 | Leather Jacket (в списке: LeatherJacket) | `LeatherJacket` | checkbox ↔ list |
| L-IFLAG-071 | `BatteriesCheckBox` (D3515; Attributes) | Batteries_ValueChanged (vb:606) | ItemFlags2CheckedList / itemFlags2Array | 6 | Is Battery (в списке: Batteries) | `Batteries` | checkbox ↔ list |
| L-IFLAG-072 | `NeedsBatteriesCheckBox` (D3528; Attributes) | NeedsBatteries_ValueChanged (vb:611) | ItemFlags2CheckedList / itemFlags2Array | 7 | Needs Battery (в списке: NeedsBatteries) | `NeedsBatteries` | checkbox ↔ list |
| L-IFLAG-073 | `XRayCheckBox` (D4325; Abilities) | XRay_ValueChanged (vb:616) | ItemFlags2CheckedList / itemFlags2Array | 8 | X-Ray Scanner (в списке: XRay) | `XRay` | checkbox ↔ list |
| L-IFLAG-074 | `WireCuttersCheckBox` (D4338; Abilities) | Wirecutters_ValueChanged (vb:621) | ItemFlags2CheckedList / itemFlags2Array | 9 | Wire Cutters (в списке: WireCutters) | `WireCutters` | checkbox ↔ list |
| L-IFLAG-075 | `ToolKitCheckBox` (D3802; Attributes) | Toolkit_ValueChanged (vb:626) | ItemFlags2CheckedList / itemFlags2Array | 10 | Tool Kit (в списке: Toolkit) | `Toolkit` | checkbox ↔ list; Tag контрола "ToolKit", DataTable/XML — "Toolkit" |
| L-IFLAG-076 | `FirstAidKitCheckBox` (D3854; Attributes) | FirstAidKit_ValueChanged (vb:631) | ItemFlags2CheckedList / itemFlags2Array | 11 | First Aid Kit (в списке: FirstAidKit) | `FirstAidKit` | checkbox ↔ list |
| L-IFLAG-077 | `MedicalKitCheckBox` (D3841; Attributes) | MedicalKit_ValueChanged (vb:636) | ItemFlags2CheckedList / itemFlags2Array | 12 | Medical Kit (в списке: MedicalKit) | `MedicalKit` | checkbox ↔ list |
| L-IFLAG-078 | `CanteenCheckBox` (D4224; Abilities) | Canteen_ValueChanged (vb:641) | ItemFlags2CheckedList / itemFlags2Array | 13 | Canteen | `Canteen` | checkbox ↔ list |
| L-IFLAG-079 | `JarCheckBox` (D4249; Abilities) | Jar_ValueChanged (vb:646) | ItemFlags2CheckedList / itemFlags2Array | 14 | Jar | `Jar` | checkbox ↔ list |
| L-IFLAG-080 | `CanAndStringCheckBox` (D4409; Abilities) | CanAndString_ValueChanged (vb:651) | ItemFlags2CheckedList / itemFlags2Array | 15 | Can & String (в списке: CanAndString) | `CanAndString` | checkbox ↔ list |
| L-IFLAG-081 | `MarblesCheckBox` (D4435; Abilities) | Marbles_ValueChanged (vb:656) | ItemFlags2CheckedList / itemFlags2Array | 16 | Marbles | `Marbles` | checkbox ↔ list |
| L-IFLAG-082 | `WalkmanCheckBox` (D3471; Attributes) | Walkman_ValueChanged (vb:661) | ItemFlags2CheckedList / itemFlags2Array | 17 | Walkman | `Walkman` | checkbox ↔ list |
| L-IFLAG-083 | `RemoteTriggerCheckBox` (D4505; Abilities) | RemoteTrigger_ValueChanged (vb:666) | ItemFlags2CheckedList / itemFlags2Array | 18 | Remote Trigger (в списке: RemoteTrigger) | `RemoteTrigger` | checkbox ↔ list |
| L-IFLAG-084 | `RobotRemoteControlCheckBox` (D3445; Attributes) | RobotRemoteControl_ValueChanged (vb:671) | ItemFlags2CheckedList / itemFlags2Array | 19 | Robot Remote Control (в списке: RobotRemoteControl) | `RobotRemoteControl` | checkbox ↔ list |
| L-IFLAG-085 | `CamoKitCheckBox` (D3789; Attributes) | CamoKit_ValueChanged (vb:676) | ItemFlags2CheckedList / itemFlags2Array | 20 | Camouflage Kit (в списке: CamouflageKit) | `CamouflageKit` | checkbox ↔ list |
| L-IFLAG-086 | `LocksmithKitCheckBox` (D3828; Attributes) | LocksmithKit_ValueChanged (vb:681) | ItemFlags2CheckedList / itemFlags2Array | 21 | Locksmith Kit (в списке: LocksmithKit) | `LocksmithKit` | checkbox ↔ list |
| L-IFLAG-087 | `MineCheckBox` (D10030; Explosives) | Mine_ValueChanged (vb:686) | ItemFlags2CheckedList / itemFlags2Array | 22 | Mine | `Mine` | checkbox ↔ list |
| L-IFLAG-088 | — (список D11981; Flags / ItemFlags2) | ItemFlag2_ValueChanged (vb:781) — индекс игнорируется | ItemFlags2CheckedList / itemFlags2Array | 23 | AntitankMine | `AntitankMine` | только CheckedListBox; сохраняется через CommitData |
| L-IFLAG-089 | `HardwareCheckBox` (D3815; Attributes) | Hardware_ValueChanged (vb:691) | ItemFlags2CheckedList / itemFlags2Array | 24 | Hardware | `Hardware` | checkbox ↔ list |
| L-IFLAG-090 | `MedicalCheckBox` (D4172; Abilities) | Medical_ValueChanged (vb:696) | ItemFlags2CheckedList / itemFlags2Array | 25 | Medicine / Drug (в списке: Medical) | `Medical` | checkbox ↔ list |
| L-IFLAG-091 | `GasCanCheckBox` (D4262; Abilities) | GasCan_ValueChanged (vb:701) | ItemFlags2CheckedList / itemFlags2Array | 26 | Gas Can (в списке: GasCan) | `GasCan` | checkbox ↔ list |
| L-IFLAG-092 | `ContainsLiquidCheckBox` (D4236; Abilities) | ContainsLiquid_ValueChanged (vb:706) | ItemFlags2CheckedList / itemFlags2Array | 27 | Contains Liquid (в списке: ContainsLiquid) | `ContainsLiquid` | checkbox ↔ list |
| L-IFLAG-093 | `RockCheckBox` (D4422; Abilities) | Rock_ValueChanged (vb:711) | ItemFlags2CheckedList / itemFlags2Array | 28 | Rock | `Rock` | checkbox ↔ list |
| L-IFLAG-094 | `ThermalOpticsCheckBox` (D4585; Abilities) | ThermalOptics_ValueChanged (vb:716) | ItemFlags2CheckedList / itemFlags2Array | 29 | Thermal Optics (в списке: ThermalOptics) | `ThermalOptics` | checkbox ↔ list |
| L-IFLAG-095 | `SciFiCheckBox` (D3333; Attributes) | Scifi_ValueChanged (vb:721) | ItemFlags2CheckedList / itemFlags2Array | 30 | Sci-Fi (в списке: SciFi) | `SciFi` | checkbox ↔ list |
| L-IFLAG-096 | `NewInventoryCheckBox` (D3400; Attributes) | NewInv_ValueChanged (vb:726) | ItemFlags2CheckedList / itemFlags2Array | 31 | New Inventory Only (в списке: NewInv) | `NewInv` | checkbox ↔ list |
| L-IFLAG-097 | — (список D11981; Flags / ItemFlags2) | ItemFlag2_ValueChanged (vb:781) — индекс игнорируется | ItemFlags2CheckedList / itemFlags2Array | 32 | DiseaseSystemExclusive | `DiseaseSystemExclusive` | только CheckedListBox; сохраняется через CommitData |
| L-IFLAG-098 | `BarrelCheckBox` (D11815; Overheating) | Barrel_ValueChanged (vb:731) | ItemFlags2CheckedList / itemFlags2Array | 33 | Barrel | `Barrel` | checkbox ↔ list |
| L-IFLAG-099 | `TripWireActivationCheckBox` (D4492; Abilities) | TripWireActivation_ValueChanged (vb:736) | ItemFlags2CheckedList / itemFlags2Array | 34 | Activated by Tripwire (в списке: TripWireActivation) | `TripWireActivation` | checkbox ↔ list |
| L-IFLAG-100 | `TripWireCheckBox` (D4479; Abilities) | TripWire_ValueChanged (vb:741) | ItemFlags2CheckedList / itemFlags2Array | 35 | Tripwire (в списке: TripWire) | `TripWire` | checkbox ↔ list |
| L-IFLAG-101 | `DirectionalCheckBox` (D10004; Explosives) | Directional_ValueChanged (vb:746) | ItemFlags2CheckedList / itemFlags2Array | 36 | Directional | `Directional` | checkbox ↔ list |
| L-IFLAG-102 | `BlockIronSightCheckBox` (D9156; Attachment Data / Info) | BlockIronsight_ValueChanged (vb:751) | ItemFlags2CheckedList / itemFlags2Array | 37 | Block Iron Sight (в списке: BlockIronSight) | `BlockIronSight` | checkbox ↔ list |
| L-IFLAG-103 | `AllowClimbingCheckbox` (D11274; Pockets) | AllowClimbing_ValueChanged (vb:756) | ItemFlags2CheckedList / itemFlags2Array | 38 | Allow Climbing? (в списке: AllowClimbing) | `AllowClimbing` | checkbox ↔ list |
| L-IFLAG-104 | `CigaretteCheckBox` (D4158; Abilities) | Cigarette_ValueChanged (vb:761) | ItemFlags2CheckedList / itemFlags2Array | 39 | Cigarette | `Cigarette` | checkbox ↔ list |
| L-IFLAG-105 | `ProvidesRobotCamoCheckBox` (D12297; Robot) | RobotCamo_ValueChanged (vb:766) | ItemFlags2CheckedList / itemFlags2Array | 40 | Provides Robot Camo (в списке: ProvidesRobotCamo) | `ProvidesRobotCamo` | checkbox ↔ list |
| L-IFLAG-106 | `ProvidesRobotNightVisionCheckBox` (D12310; Robot) | RobotNightVision_ValueChanged (vb:771) | ItemFlags2CheckedList / itemFlags2Array | 41 | Provides Robot Nightvision (в списке: ProvidesRobotNightVision) | `ProvidesRobotNightVision` | checkbox ↔ list |
| L-IFLAG-107 | `ProvidesRobotLaserBonusCheckBox` (D12323; Robot) | RobotLaserBonus_ValueChanged (vb:776) | ItemFlags2CheckedList / itemFlags2Array | 42 | Provides Robot Laser Bonus (в списке: ProvidesRobotLaserBonus) | `ProvidesRobotLaserBonus` | checkbox ↔ list |

### 4.1. Итоги по флагам

- 70 отдельных контролов: 66 `CheckBox` + 4 `RadioButton` (`Cannon`, `GrenadeLauncher`, `Mortar`, `RocketLauncher`).
- 35 флагов `ItemFlags` доступны только через `ItemFlagsCheckedList` (включая два `UNUSED`); 2 флага `ItemFlags2` — только через `ItemFlags2CheckedList` (`AntitankMine`, `DiseaseSystemExclusive`).
- 105 реальных XML-флагов имеют UI. Флагов без UI в `ItemFlags`/`ItemFlags2` нет; два `UNUSED` не имеют XML-поля.
- 5 дополнительных флагов оружия/взрывчатки связаны напрямую через `Tag`, вне массивов и вкладки `Flags`: `WeaponSwapClips`, `WeaponNoSemiAuto`, `WeaponEasyUnjam`, `WeaponHeavyGun`, `ExplosivefExplodeOnImpact`.
- `DrugsCheckedList` (32 пункта) не входит в массивы и не сохраняется: код загрузки/сохранения закомментирован (vb:313–322, 1096–1105). Это не 32 рабочих флага, а мёртвый список.
- `itemFlagsArray`/`itemFlags2Array` объявлены `Public` (vb:57–58), без инкапсуляции.

## 5. Счётчики описаний (`L-ICOUNT-001…016`)

| ID | TextBox | Counter label | MaxLength source | Handler line |
|---|---|---|---:|---:|
| L-ICOUNT-001 | `DescriptionTextBox` | `DescriptionCharsLeftLabel` | 400 (D1503) | 843 |
| L-ICOUNT-002 | `BRDescriptionTextBox` | `BRDescriptionCharsLeftLabel` | 400 (D1432) | 847 |
| L-ICOUNT-003 | `GermanDescriptionTextBox` | `GermanDescriptionCharsLeftLabel` | 400 (D1683) | 851 |
| L-ICOUNT-004 | `GermanBRDescriptionTextBox` | `GermanBRDescriptionCharsLeftLabel` | 400 (D1612) | 855 |
| L-ICOUNT-005 | `RussianDescriptionTextBox` | `RussianDescriptionCharsLeftLabel` | 400 (D1863) | 859 |
| L-ICOUNT-006 | `RussianBRDescriptionTextBox` | `RussianBRDescriptionCharsLeftLabel` | 400 (D1792) | 863 |
| L-ICOUNT-007 | `PolishDescriptionTextBox` | `PolishDescriptionCharsLeftLabel` | 400 (D2043) | 867 |
| L-ICOUNT-008 | `PolishBRDescriptionTextBox` | `PolishBRDescriptionCharsLeftLabel` | 400 (D1962) | 871 |
| L-ICOUNT-009 | `FrenchDescriptionTextBox` | `FrenchDescriptionCharsLeftLabel` | 400 (D2223) | 875 |
| L-ICOUNT-010 | `FrenchBRDescriptionTextBox` | `FrenchBRDescriptionCharsLeftLabel` | 400 (D2142) | 879 |
| L-ICOUNT-011 | `ItalianDescriptionTextBox` | `ItalianDescriptionCharsLeftLabel` | 400 (D2403) | 883 |
| L-ICOUNT-012 | `ItalianBRDescriptionTextBox` | `ItalianBRDescriptionCharsLeftLabel` | 400 (D2322) | 887 |
| L-ICOUNT-013 | `DutchDescriptionTextBox` | `DutchDescriptionCharsLeftLabel` | 400 (D2583) | 891 |
| L-ICOUNT-014 | `DutchBRDescriptionTextBox` | `DutchBRDescriptionCharsLeftLabel` | 400 (D2502) | 895 |
| L-ICOUNT-015 | `ChineseDescriptionTextBox` | `ChineseDescriptionCharsLeftLabel` | 63 (D2763) | 899 |
| L-ICOUNT-016 | `ChineseBRDescriptionTextBox` | `ChineseBRDescriptionCharsLeftLabel` | 90 (D2682) | 903 |

Все 16 обработчиков только пишут `MaxLength - TextLength` в соответствующий `*CharsLeftLabel`; обратной записи в данные нет. У остальных языков `MaxLength=400`, у китайского — 63 (`ChineseDescriptionTextBox`) и 90 (`ChineseBRDescriptionTextBox`).

## 6. Видимость вкладок (`DisplayTabs`)

`DisplayTabs` (ItemDataForm.vb:1232–1348) вызывается после `Bind` и после `RemoveUnusedLanguageSpecificItemTabs` (vb:16–42, 65). Удалённые страницы остаются в дизайнере и сохраняют `DataBindings`, но не отображаются.

| Вкладка / страница | Заголовок | Видима для классов | Designer | Ветка DisplayTabs |
|---|---|---|---:|---|
| `DescriptionPage` | Description | все классы; вложенные языковые страницы — см. ниже | D1312 | 1232–1348 |
| `EnglishPage` | English | все классы (базовые поля) | D1353 | 1322 |
| `GermanPage` | German | LANG: German | D1533 | 1323 |
| `RussianPage` | Russian | LANG: Russian | D1713 | 1324 |
| `PolishPage` | Polish | LANG: Polish | D1893 | 1325 |
| `FrenchPage` | French | LANG: French | D2073 | 1326 |
| `ItalianPage` | Italian | LANG: Italian | D2253 | 1327 |
| `DutchPage` | Dutch | LANG: Dutch | D2433 | 1328 |
| `ChinesePage` | Chinese | LANG: Chinese | D2613 | 1329 |
| `GeneralPage` | General | все классы | D2786 | 1281 |
| `AttributesPage` | Attributes | все классы | D3264 | 1282 |
| `AbilitiesPage` | Abilities | все, кроме Key, Else; LBE и RandomItem видны | D3962 | 1283; 1295–1307, 1332–1345 |
| `NCTHPage` | NCTH | все, кроме Key, LBE, RandomItem, Else | D5112 | 1284; 1295–1318, 1319–1345 |
| `BonusesPage` | Bonuses | все, кроме Key, LBE, RandomItem, Else | D6118 | 1285; 1295–1318, 1319–1345 |
| `WeaponPage` | Weapon | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | D6982 | 1286; 1257–1279 |
| `GeneralWeaponPage` | General (внутри Weapon) | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | D7015 | 6992–6995; 1257–1279 |
| `GunPage` | Guns (внутри Weapon) | Gun, Launcher | D7640 | 6992–6995; 1257–1264, 1271–1279 |
| `LauncherPage` | Launchers (внутри Weapon) | Launcher | D8112 | 6992–6995; 1257–1270 |
| `SoundPage` | Sounds (внутри Weapon) | Gun, Launcher, Knife, ThrowingKnife, Thrown, Tentacle, Punch | D8326 | 6992–6995; 1257–1279 |
| `AttachmentPage` | Attachments | все, кроме Ammo, Key, RandomItem, Else | D8616 | 1287; 1235–1242, 1295–1345 |
| `AttachmentDataPage` | Attachment Data | все, кроме Key, LBE, RandomItem, Else | D9064 | 1288; 1295–1345 |
| `TabPage1` | Info (внутри Attachment Data) | как AttachmentDataPage | D9090 | 9074–9075; 1288 |
| `TabPage2` | Items (внутри Attachment Data) | как AttachmentDataPage | D9387 | 9074–9075; 1288 |
| `ExplosivePage` | Explosives | Bomb, Grenade | D9461 | 1289; 1250–1256 |
| `AmmoPage` | Ammo | Ammo | D10078 | 1290; 1235–1242 |
| `ArmourPage` | Armour | Armour | D10228 | 1291; 1243–1249 |
| `FacePage` | Face | Face | D10439 | 1292; 1280–1286 |
| `InventoryPage` | Inventories | все, кроме RandomItem, Else | D10482 | 1293; 1319–1345 |
| `LBEPage` | Pockets | LBE | D11071 | 1294; 1308–1318 |
| `OverheatingTabPage` | Overheating | все, кроме RandomItem; у Else остаётся пустая страница | D11643 | 1295; 1319–1345 |
| `FlagsTab` | Flags | все классы | D11957 | 1296 |
| `RobotTab` | Robot | все классы | D12046 | 1297 |

Группы: `WeaponTemperatureGroupBox` (`D11655`) остаётся только для `Gun` и `Launcher`; во всех остальных ветках удаляется (vb:1242, 1249, 1256, 1279, 1286, 1294, 1307, 1318, 1345). `ItemTemperatureGroupBox` (`D11767`) не удаляется и виден вместе с `OverheatingTabPage`. Групп, удаляемых для всех классов, нет. `GroupBox36` (`Bobby Ray's`, `D12520`) не добавлен ни в один контейнер и не виден ни для одного класса; `GroupBox65` (`Drugs`) остаётся видимым всегда, но его `DrugsCheckedList` функционально мёртв.

## 7. Lookup- и image-контролы

### 7.1. ComboBox

В дизайнере **нет** ни одного `DataSource`, `DisplayMember` или `ValueMember` (0 совпадений). Для 62 lookup-комбобоксов источник задаёт `BaseDataForm` из свойств колонки: `DataSource = New DataView(Database.Table(Lookup_Table), Lookup_Filter, Lookup_Sort)`, `ValueMember = Lookup_ValueColumn`, `DisplayMember = Lookup_TextColumn` (BaseDataForm.vb:65–80). Если метаданные колонки не содержат `Lookup_Table`, комбобокс остаётся пустым. Точные таблицы/колонки в исходниках не зашиты; ниже — семейства по именам полей и константам `Tables.vb`.

| Семейство | Контролы | Кол-во | Источник данных |
|---|---|---:|---|
| Ручной список | `GraphicTypeComboBox` (`ubGraphicType`) | 1 | `Items.Add("Guns")`, затем `P{n}Items` (vb:336–345); выбранный индекс пишется в `ubGraphicType` (vb:955) |
| Ручной список | `RobotTargetingSkillGrantComboBox`, `RobotChassisSkillGrantComboBox`, `RobotUtilitySkillGrantComboBox` | 3 | `NewSkillTraits` — 24 значения (vb:347–384); `SelectedIndex` (vb:78–80, 956–958) |
| Lookup (метаданные) | `CursorComboBox` (`ubCursor`) | 1 | `Cursor`/`Cursors` (Tables.vb) |
| Lookup (метаданные) | `usItemClassComboBox` (`usItemClass`) | 1 | `ItemClass`; контрол скрыт (`Enabled=False`, Size 0), класс меняется через `ChangeClassForm` |
| Lookup (метаданные) | `clothestypeComboBox`, `FoodTypeComboBox`, `randomitemComboBox`, `BloodiedItemComboBox`, `DiscardedLauncherItemComboBox`, `InseparableComboBox` | 6 | `CLOTHES`, `FOOD`, `RANDOMITEM`, `ITEM`, `ITEM`, `Separability` (ожидаемо по именам) |
| Lookup (метаданные) | `ubArmourClassComboBox` | 1 | `ArmourClass`/`Armours.xml` |
| Lookup (метаданные) | `MagazineubCalibreComboBox`, `ubAmmoTypeComboBox`, `ubMagTypeComboBox` | 3 | `AmmoTypes.xml`, `AMMOTYPE`/`MagazineType` (ожидаемо) |
| Lookup (метаданные) | `ExplosiveubTypeComboBox`, `ubAnimationIDComboBox`, `ubFragTypeComboBox` | 3 | `ExplosionData.xml`, `ExplosionType` (ожидаемо) |
| Lookup (метаданные) | `ubWeaponClassComboBox`, `ubWeaponTypeComboBox`, `WeaponubCalibreComboBox` | 3 | `WeaponClass`, `WeaponType`, `AmmoTypes` (ожидаемо) |
| Lookup (метаданные) | 7 sound-комбобоксов (`sSound`, `sBurstSound`, `SilencedSound`, `sSilencedBurstSound`, `sReloadSound`, `sLocknLoadSound`, `ManualReloadSound`) | 7 | `SOUND` / `BURSTSOUND` (Tables.vb) |
| Lookup (метаданные) | `AttachmentSystemComboBox`, `AttachmentClassComboBox`, `NasAttachmentClassComboBox`, `AttachmentPointComboBox` | 4 | `AttachmentSystem`, `AttachmentClass`, `NasAttachmentClass`, `AttachmentPoint` |
| Lookup (метаданные) | `DefaultAttachment0…9ComboBox` | 10 | `ITEM`; synthetic-колонки `DefaultAttachment0…9` (повторяющийся XML-элемент) |
| Lookup (метаданные) | `AvailableAttachmentPoint0…9ComboBox` | 10 | `ITEM`/`AttachmentPoint`; synthetic-колонки (повторяющийся XML-элемент) |
| Lookup (метаданные) | `lbeClassComboBox`, `lbePocketIndex1…12ComboBox` | 13 | `LBEClass`, `POCKET`; `SelectedValue` 0..4 управляет доступностью карманов (vb:1566–1580) |

Designer-строки всех 66 `ComboBox` приведены в карте полей (§3); в таблице выше — семейства и источники. Итого: 66 `ComboBox`; 4 с ручными `Items`, 62 с lookup из метаданных. Для 10 `DefaultAttachment` и 10 `AvailableAttachmentPoint` в данных v4/v5 максимум 10 и 1 повторение соответственно, поэтому 10 контролов покрывают оба набора.

### 7.2. Image-контролы

| Контрол | Тип | Tag | Источник | Поведение |
|---|---|---|---|---|
| `HeaderImage` | `PictureBox` | `ItemImage` | runtime `ITEM.ItemImage` | только чтение; `D12466` |
| `BigItemImage` | `PictureBox` | `ItemImage` | runtime `ITEM.ItemImage` | `LoadImages` (vb:797); при `CommitData` изображение пишется обратно в строку (vb:953) |
| `SmallItemImage` | `PictureBox` | — | `LoadImages` | `_dm.ItemImages.SmallItemImage(type,index)` (vb:797) |
| `MediumItemImage` | `PictureBox` | — | `LoadImages` | `_dm.ItemImages.MediumItemImage(type,index)` (vb:798) |
| `ImageListBox` | `ListBox` | `ubGraphicNum` | `BigItems(type)` (`Image` objects) | `DataSource` в `GraphicTypeComboBox_SelectedIndexChanged` (vb:809); выбор задаёт `GraphicIndexUpDown.Value` (vb:822); Tag не связывается |
| `GraphicTypeComboBox` | `ComboBox` | `ubGraphicType` | ручной список типов | `SelectedIndex` (vb:73/806/955); `Maximum` индекса из `SmallItems(type).Length - 1` |
| `GraphicIndexUpDown` | `NumericUpDown` | `ubGraphicNum` | `Value` | `LoadImages` (vb:802); `CommitData` vb:954 |

### 7.3. Гриды

| Грид | Таблица | Фильтр `SetupGrid` | `grid.Tag` (runtime) | Двойной клик по заголовку | Designer |
|---|---|---|---|---|---|
| `AttachmentGrid` | `ATTACHMENT` | itemIndex = _id | `itemIndex` | attachmentIndex — открыть вложение | D8877 |
| `AttachmentInfoGrid` | `ATTACHMENTINFO` | usItem = _id | `usItem` | — | D9375 |
| `AttachToGrid` | `ATTACHMENT` | attachmentIndex = _id | `attachmentIndex` | itemIndex — открыть предмет | D9418 |
| `IncompatibleAttachmentGrid` | `INCOMPATIBLEATTACHMENT` | itemIndex = _id | `itemIndex` | incompatibleattachmentIndex | D9448 |
| `LaunchableGrid` | `LAUNCHABLE` | itemIndex = _id | `itemIndex` | launchableIndex | D8143 |
| `LauncherGrid` | `LAUNCHABLE` | launchableIndex = _id | `launchableIndex` | itemIndex — открыть предмет | D9492 |
| `CompatibleFaceItemGrid` | `COMPATIBLEFACEITEM` | itemIndex = _id | `itemIndex` | compatiblefaceitemIndex | D10470 |

`SetupGrid` (vb:1149–1162) создаёт `DataView` по таблице и фильтру, скрывает ключевую колонку, пишет `grid.Tag = itemIndexField` и подписывает `DefaultValuesNeeded` (vb:1161). `Grid_DefaultValuesNeeded` (vb:1164–1176) заполняет ключ `_id` и для одноключевой таблицы берёт `GetNextPrimaryKeyValue`. Двойные клики: vb:1196–1218; `GridOpenItem` vb:1220–1228. `AttachmentInfoGrid` двойного клика не имеет.

## 8. Контролы без Tag, но функциональные

| Контрол(ы) | Тип | Где | Назначение | Покрытие в `L-ITEM-*` |
|---|---|---|---|---|
| 19 пар `<Shop>CheckBox` + `<Shop>UpDown` (`Alberto`, `Arnie`, `Carlo`, `Devin`, `Elgin`, `Frank`, `Franz`, `Fredo`, `Gabby`, `Herve`, `Howard`, `Jake`, `Keith`, `Manny`, `Mickey`, `Perko`, `Peter`, `Sam`, `Tony`) | `CheckBox` + `NumericUpDown` | Inventories / Shopkeepers (`D10508–10535`) | Включение магазина и количество | L-ITEM-036–040; имена ищутся строками (`D10508`, vb:1413–1415) |
| `AttachmentGrid`, `AttachmentInfoGrid`, `AttachToGrid`, `IncompatibleAttachmentGrid`, `LaunchableGrid`, `LauncherGrid`, `CompatibleFaceItemGrid` | `DataGridView` | Attachments / Attachment Data / Explosives / Face (`D8877`, `D9375`, `D9418`, `D9448`, `D8143`, `D9492`, `D10470`) | Таблицы связей | L-ITEM-023–033 |
| `ItemFlagsCheckedList`, `ItemFlags2CheckedList` | `CheckedListBox` | Flags / ItemFlags, ItemFlags2 (`D12032`, `D11981`) | 64 и 43 флага | L-ITEM-020–022 |
| `DrugsCheckedList` | `CheckedListBox` | Flags / Drugs (`GroupBox65`, D11994) | 32 пункта, загрузка/сохранение закомментированы | Не покрыт; мёртвый |
| `SmallItemImage`, `MediumItemImage` | `PictureBox` | General / Graphics (`D3199`, `D3167`) | Превью | L-ITEM-012 (косвенно) |
| `NotALauncherRadioButton` | `RadioButton` | Weapon > Launchers / Launcher Type (`D8191`) | «N/A»; `Checked=True` по умолчанию, Tag/обработчика нет; косвенно снимает флаги при переключении | Не покрыт |
| `ClassNameLabel` | `Label` | Шапка формы (`D12504`) | Показывает текст `usItemClassComboBox`; привязка в коде (vb:69) | Не покрыт |
| `ChangeClassButton` | `Button` | Шапка формы (`D12494`) | Открывает `ChangeClassForm` | L-ITEM-009 |
| `ItemTab`, `TabControl2`, `WeaponTab`, `AttDataSubTab` | `TabControl` | `D1302`, `D1335`, `D6999`, `D9079` | Навигация | L-ITEM-003/005 (частично) |
| 56 `TableLayoutPanel` | layout | все вкладки | Только компоновка | — |
| 27 мёртвых контролов | см. §9.2 | — | Не отображаются | — |

## 9. Аномалии

### 9.1. Данные и функциональность

1. **`Detonator`/`RemoteDetonator` отсутствуют в схеме.** `DatabaseCode.vb:1463–1464` добавляет их колонки только в закомментированном виде; в `Items.xml` v4/v5 таких элементов нет. При этом `CommitData` безусловно обращается к `_view(0)("Detonator")` и `"RemoteDetonator"` (vb:1044–1045), а инициализация списка закомментирована (vb:260–261). Если колонок действительно нет, любой Apply/OK падает с `ArgumentException`; если они есть в метаданных, при открытии они не загружаются (init закомментирован), а при сохранении пишется текущее состояние списка — по умолчанию `False`. Требует проверки на живой сборке (не запускалась).
2. **`DrugsCheckedList` — мёртвый UI.** 32 пункта (`D12006`), `GroupBox65` всегда виден, но `CheckOnClick` не задан, обработчика `ItemCheck` нет, загрузка/сохранение `DrugType` закомментированы (vb:313–322, 1096–1105). Пункты можно отметить вручную, но состояние не загружается и не сохраняется.
3. **Два UNUSED-флага кликабельны, но не загружаются.** `ItemFlagsCheckedList` индексы 60/61 (`Detonator_UNUSED`, `RemoteDetonator_UNUSED`) — `UNUSED`; `ItemFlag_ValueChanged` их игнорирует (vb:568–569), init закомментирован; `CommitData` сохраняет текущее состояние списка (по умолчанию `False`). Это потенциальная потеря значения, если поле когда-либо появится.
4. **5 флагов вне `FlagsTab`.** `WeaponSwapClips`, `WeaponNoSemiAuto`, `WeaponEasyUnjam`, `WeaponHeavyGun`, `ExplosivefExplodeOnImpact` — обычные `Tag`-`CheckBox`, не входят в `itemFlagsArray`/`itemFlags2Array` и не синхронизируются со списками. Для паритета их нельзя терять при переносе «флаговой» логики.
5. **`ItemImage` — runtime-колонка.** `BigItemImage.Image` пишется в `_view("ItemImage")` (vb:953); колонка добавляется `ItemTable.AddImageColumn` и удаляется перед сохранением. `HeaderImage` — второй `PictureBox` на то же поле, только чтение.
6. **Регистр Tag расходится с записью.** `BarrelCheckBox.Tag = "barrel"`, а `CommitData`/enum — `"Barrel"` (vb:1084); `ToolKitCheckBox.Tag = "ToolKit"`, `CommitData` — `"Toolkit"` (vb:1061), XML — `Toolkit`. `DataColumn` обычно регистронезависим, но в новой реализации нужен явный маппинг.
7. **8 дублирующихся Tag.** Живые: `ItemImage` (`HeaderImage` + `BigItemImage`), `szLongItemName` (`LongNameTextBox` + `Label143`), `ubGraphicNum` (`GraphicIndexUpDown` + `ImageListBox`, у последнего Tag мёртв). Мёртвые: `PolishItemszBRName`, `PolishItemszBRDesc`, `PolishItemszItemName`, `PolishItemszLongItemName`, `PolishItemszItemDesc` — по два контрола, второй не добавлен в контейнер. Полная таблица — §9.3.
8. **Скрытый `usItemClassComboBox`.** `Enabled=False`, `Size=(0,25)` (D12480/12485); класс нельзя изменить через него, только через `ChangeClassForm`; при этом `ClassNameLabel` привязан к его `Text` (vb:69), а сам combo — к `usItemClass` через `Tag`.
9. **`NotALauncherRadioButton` не привязан.** `Checked=True` по умолчанию (`D8187`), нет Tag и обработчика; работает только как четвёртый radio, снимающий остальные. Если все четыре флага `False`, в данных нет отдельного признака «N/A» — это нормально, но поведение неявное.

### 9.2. Мёртвые контролы (27)

| Группа | Контролы | Designer | Причина |
|---|---|---|---|
| Старый English-блок | `TextBox1`, `TextBox2`, `Label15`, `Label16`, `Label17`, `Label18` | D12331–12375 | Не добавлены ни в один контейнер |
| Старый German/Russian-блок | `TextBox8`, `TextBox9`, `Label28`, `Label29`, `Label30`, `Label31` | D12384–12428 | Не добавлены ни в один контейнер |
| `GroupBox36` (`Bobby Ray's`) | `GroupBox36`, `TextBox3` (Tag `PolishItemszBRName`), `TextBox4` (Tag `PolishItemszBRDesc`), `Label163`, `Label167`, `Label171`, `Label172` | D12520–12576 | Группа не добавлена в форму; её дети не видны, Tag не связывается |
| Старый Polish-блок | `TextBox5` (Tag `PolishItemszItemName`), `TextBox6` (Tag `PolishItemszLongItemName`), `TextBox7` (Tag `PolishItemszItemDesc`), `Label173`–`Label177` | D12585–12649 | Не добавлены ни в один контейнер |

Мёртвые контролы всё равно создаются в `InitializeComponent`, занимают память и попадают в перебор, если новая реализация будет искать Tag рефлексией. Их Tag-поля нельзя считать покрытыми UI.

### 9.3. Дубли Tag

| Поле | Контрол 1 | Контрол 2 | Статус |
|---|---|---|---|
| `ItemImage` | `BigItemImage` (D3148) | `HeaderImage` (D12466) | Живой дубль; Big пишет, Header только читает |
| `szLongItemName` | `LongNameTextBox` (D1487) | `Label143` (D12453) | Живой дубль; заголовок формы |
| `ubGraphicNum` | `GraphicIndexUpDown` (D3233) | `ImageListBox` (D3160) | Tag ListBox не связывается |
| `PolishItemszBRName` | `PolishBRNameTextBox` (D1946) | `TextBox3` (D12551) | `TextBox3` мёртв |
| `PolishItemszBRDesc` | `PolishBRDescriptionTextBox` (D1967) | `TextBox4` (D12570) | `TextBox4` мёртв |
| `PolishItemszItemName` | `PolishNameTextBox` (D2007) | `TextBox5` (D12606) | `TextBox5` мёртв |
| `PolishItemszLongItemName` | `PolishLongNameTextBox` (D2027) | `TextBox6` (D12624) | `TextBox6` мёртв |
| `PolishItemszItemDesc` | `PolishDescriptionTextBox` (D2048) | `TextBox7` (D12643) | `TextBox7` мёртв |

### 9.4. Компоновка и hard-coded индексы

1. **4 наложения в `TableLayoutPanel`:**
   - `PercentStatusDrainReductionUpDown` добавлен в ячейку (0,2), там же `Label107 "Kit Status:"` (D4797/4802); по координатам контрол должен быть в колонке 1.
   - `bAccuracyUpDown` — ячейка (0,1), там же `Label87 "Accuracy:"` (D7255/7258).
   - `NumericUpDown2` (`ExplosiveubHorizontalDegree`) — ячейка (0,12), там же `Label245 "Horz Degrees:"` (D9517/9518).
   - `BloodiedItemComboBox` — ячейка (0,0), там же `Label83 "Bloody Item:"` (D7116/7117).
   Визуальный эффект не проверялся запуском; в дизайнере это одна ячейка на два контрола.
2. **Жёсткие индексы флагов:** `CommitData` читает `ItemFlagsCheckedList.GetItemChecked(0…63)` (vb:984–1047) и `ItemFlags2CheckedList.GetItemChecked(0…42)` (vb:1051–1093). Порядок должен совпадать с `Items.AddRange` (D12032/D11981) и с массивами (vb:85–196). Любая вставка пункта в список сдвигает все последующие поля.
3. **`itemFlagsArray` индекс 0 — заглушка:** `ItemFlags.Bloodbag` на месте 0, хотя отдельного контрола нет; это сделано для совпадения индексов. При изменении enum легко получить сдвиг.
4. **LBE:** `Case 0…4` (vb:1566–1580) и 12 карманов (vb:1452–1564) зашиты; `lbePocketIndex1…12` включаются/выключаются по классу. `SelectedValue=0` используется как «ничего».
5. **Default/Available Attachment:** 10 + 10 контролов и synthetic-колонки 0…9; для `AvailableAttachmentPoint` порядок в UI — 1…9, затем 0 (`D8797`).
6. **19 магазинов** перечислены вручную в `LoadInventoryData`/`SaveInventoryData` (vb:1368–1410); `InventoryCheckBox_CheckedChanged` ищет `<cb.Text>UpDown` (vb:1363–1366), а `LoadShopkeeperData` — `<name>CheckBox` (vb:1414). Переименование/локализация текста ломает связь.
7. **`_view(0)`** предполагает ровно одну строку; `Bind` фильтрует по `uiIndex=ID`, но при отсутствии строки падает.
8. **`GraphicTypeComboBox.SelectedIndex`** может стать `-1` при значении вне диапазона; тогда `SmallItems(-1)` (vb:74/807) бросит исключение. Аналогично `GraphicIndexUpDown.Maximum = Length - 1` при пустом массиве.
9. **`Robot*SkillGrantComboBox.SelectedIndex = -1`** может быть записан в XML как `-1` (vb:78–80, 956–958), если значение в строке не соответствует списку.

### 9.5. События и подписки

- `SetupGrid` добавляет `AddHandler grid.DefaultValuesNeeded` (vb:1161) для каждого из 7 гридов; `RemoveHandler` нет. Подписка живёт до закрытия формы, повторной подписки при одном вызове `SetupGrid` нет.
- `LoadShopkeeperData` добавляет `AddHandler cb.CheckedChanged` (vb:1427) для каждого магазина; `RemoveHandler` нет. `LoadInventoryData` вызывается один раз из `Initialize`, поэтому дублей подписки в текущем сценарии нет, но при повторном `LoadInventoryData` обработчик задвоится.
- Остальные обработчики объявлены через `Handles`; отписки не требуются.

## 10. Открытые вопросы

| ID | Вопрос | Почему важно |
|---|---|---|
| Q-01 | Существуют ли колонки `Detonator`/`RemoteDetonator` в реальной схеме `ITEM`? | Если нет — `CommitData` не может сохранить ни один предмет; если да — при открытии не загружаются, при сохранении пишется состояние списка (по умолчанию `False`). Проверять на живой сборке/схеме. |
| Q-02 | Заполнены ли `Lookup_Table`/`Lookup_TextColumn`/`Lookup_ValueColumn` для всех 62 lookup-комбобоксов в метаданных? | В исходниках таблиц нет; без метаданных комбобоксы пусты. Нужна схема SQLite/XML метаданных. |
| Q-03 | Наложения TLP (4 шт.) видны в собранном приложении? | В дизайнере контрол и Label в одной ячейке; возможно, `TableLayoutPanel` их раздвигает/обрезает. Нужен скриншот. |
| Q-04 | Приводит ли `Tag`-привязка `SelectedValue` для `GraphicTypeComboBox`/robot-комбо к записи `DBNull` в строку? | У комбо нет `DataSource`; `SelectedValue` может быть `null`. Нужен runtime-тест. |
| Q-05 | Что происходит при `GraphicTypeComboBox.SelectedIndex = -1`? | Потенциальный `ArgumentOutOfRangeException` в `SmallItems(-1)`. |
| Q-06 | Является ли `Label143` (заголовок `szLongItemName`) намеренным дублем? | Влияет на паритет шапки; заголовок «Item Name» затирается. |
| Q-07 | Должны ли 5 direct-bound флагов оружия быть на `FlagsTab`? | Иначе паритет флагов будет неполным при переносе. |
| Q-08 | Нужно ли сохранять/восстанавливать `DrugsCheckedList` или удалить вкладку `Drugs`? | Сейчас UI обещает функциональность, которой нет. |
| Q-09 | Почему у китайских описаний `MaxLength` 63/90, а у остальных 400? | Возможны ограничения кодировки/длины; влияет на счётчики и ввод. |
| Q-10 | Есть ли элементы `AvailableAttachmentPoint` больше 1 на запись в других версиях? | В v4/v5 максимум 1, UI имеет 10; запас есть, но hard-coded 10. |

## 11. Приложение: перечень Tag-полей, не упомянутых в коде `ItemDataForm.vb`

Большинство Tag-полей не упоминаются в коде явно: их связывает общий `BaseDataForm.BindControls` по `Tag`. Это нормально. Отдельно стоит рассматривать только случаи, где Tag не даёт рабочей привязки:

| Tag | Контрол | Designer | Причина |
|---|---|---|---|
| `ubGraphicNum` | `ImageListBox` | D3160 | `ListBox` не поддержан `BindControls`; реальная привязка — `GraphicIndexUpDown` |
| `PolishItemszBRName` | `TextBox3` | D12551 | Мёртвый контрол; живая привязка — `PolishBRNameTextBox` |
| `PolishItemszBRDesc` | `TextBox4` | D12570 | Мёртвый контрол; живая привязка — `PolishBRDescriptionTextBox` |
| `PolishItemszItemName` | `TextBox5` | D12606 | Мёртвый контрол; живая привязка — `PolishNameTextBox` |
| `PolishItemszLongItemName` | `TextBox6` | D12624 | Мёртвый контрол; живая привязка — `PolishLongNameTextBox` |
| `PolishItemszItemDesc` | `TextBox7` | D12643 | Мёртвый контрол; живая привязка — `PolishDescriptionTextBox` |
| `szLongItemName` | `Label143` | D12453 | Работает через `Label.Text`, но дублирует `LongNameTextBox` и затирает заголовок |
| `ItemImage` | `HeaderImage` | D12466 | Только чтение; `CommitData` берёт `BigItemImage` |
| `usItemClass` | `usItemClassComboBox` | D12487 | Скрытый/disabled контрол; класс меняется через `ChangeClassForm` |

