# M23b4 — числовые границы полей (L-ITEM-047, L-MERC-014)

Research-записка. Только чтение: код, тесты, миграции и `References/` не изменялись; сборка и тесты не запускались.

## 1. Методика

- Разобраны `References/xml-editor/GUI/Data Forms/ItemDataForm.Designer.vb` (13 996 строк) и `MercGearDataForm.Designer.vb` (9 814): объявления `Friend WithEvents … As NumericUpDown`, строки `.Tag = "…"` и `New Decimal(New Integer() {…})`. В item-форме 174 NumericUpDown, у 155 есть `Tag`, у 126 — `Tag` и явный `Minimum`/`Maximum`; в merc-форме 175 NumericUpDown, явные границы только у 60 (цены и quantity).
- Разбор Decimal: 4-й элемент `Global.System.Int32.MinValue`/`-2147483648` — знак, поэтому `{100,0,0,Int32.MinValue}` = −100, а не 100 (проверено на `ReliabilityUpDown`, `:3035-3036`).
- `Tag` — имя колонки DataTable: `BaseDataForm.BindControls` делает `DataBindings.Add("Value", _view, ctl.Tag, …)` (`BaseDataForm.vb:52-62`). Для расширенных таблиц legacy сливала колонки в ITEM как `<TableName><Field>` (`ItemTable.vb:165-172`), поэтому Tags имеют префиксы `Weapon…`, `Explosive…`, `Magazine…`, `Armour…`, `LoadBearingEquipment…`; XML-элемент — остаток после префикса, сверено с `MakeColumn` в `MakeWeaponsTable`/`MakeMagazineTable`/`MakeExplosiveTable`/`MakeArmourTable`/`MakeItemsTable`/`MakeLoadBearingEquipmentTable` (`DatabaseCode.vb:914,982,1092,1203,1270,2184`). Привязка к таблице — по этим спискам полей, не по вкладкам; для всех 126 строк префикс дал существующее поле соответствующей `Make*Table`-таблицы.
- Стойки NCTH — исключение: Tags `FlatBase1..3` и т.п. не имена элементов; в XML это повторяющиеся элементы внутри `STAND_MODIFIERS`/`CROUCH_MODIFIERS`/`PRONE_MODIFIERS` (напр. `References/JA 2 1.13 v4/Data-1.13/TableData/Items/Items.xml:234`).
- Данные: `…v4|v5/Data-1.13/TableData/Items/{Items,Weapons,Magazines,Explosives,Armours,LoadBearingEquipment}.xml` и `…/Inventory/MercStartingGear.xml`, Python `ElementTree`, только чтение. Ключи: `uiIndex` (LBE — `lbeIndex`), merc — `mIndex` + номер `GEARKIT` (1..5). Пустые элементы пропускались; `n` — число записей с непустым полем. Оверлеи `Russian.*` не сканировались: это отдельные таблицы rewrite, в `Russian.Items.xml` только `uiIndex` и строковые поля.
- В таблицах `0*`/`100*` — WinForms-дефолты NumericUpDown (`Minimum=0`, `Maximum=100`), которые legacy применяла при отсутствии явного присваивания. `:N` — строка в `References/xml-editor/GUI/Data Forms/ItemDataForm.Designer.vb` (для merc — `MercGearDataForm.Designer.vb`); `†` — вложенный путь, не плоское поле. Границы прочитаны из строк; принадлежность таблице — вывод из `MakeColumn`, отмечен отдельно.

## 2. Границы NumericUpDown формы предмета

Статические границы, `ItemDataForm.Designer.vb`: 102 строки (91 плоский контрол + 11 групп стоек, всего 124 контрола). `ItemSizeUpDown` и `GraphicIndexUpDown` — в §2.1 (динамические). У `Items.Armours` (`ubProtection`, `ubCoverage`, `ubDegradePercent`) явных границ нет — в таблицу не входят. `uiIndex` — ключ `Items.Items`; одноимённые поля расширенных таблиц здесь не адресуются.

| Контрол(ы) | Tag | Таблица | min | max | v4: n; min…max | v5: n; min…max | Строки |
|---|---|---|---|---|---|---|---|
| `AimBonusUpDown` | `AimBonus` | Items.Items | -100 | 100* | 134; -11…20 | 134; -11…20 | :6606 |
| `APBonusUpDown` | `APBonus` | Items.Items | -100 | 100* | 164; -11…0 | 164; -11…0 | :4871 |
| `AutoFireToHitBonusUpDown` | `AutoFireToHitBonus` | Items.Items | -100 | 100* | 68; 0…6 | 68; 0…6 | :6574 |
| `BestLaserRangeUpDown` | `BestLaserRange` | Items.Items | 0* | 10000 | 12; 0…400 | 12; 0…400 | :6616 |
| `BipodUpDown` | `Bipod` | Items.Items | -100 | 100* | 5; 0…10 | 5; 0…10 | :6742 |
| `BR_NewInventoryUpDown` | `BR_NewInventory` | Items.Items | -100 | 250 | 841; 0…40 | 840; 0…40 | :11033,11034 |
| `BR_ROFUpDown` | `BR_ROF` | Items.Items | 0* | 10000 | 381; 0…2500 | 380; 0…2500 | :7440 |
| `BR_UsedInventoryUpDown` | `BR_UsedInventory` | Items.Items | -100 | 250 | 595; 0…25 | 594; 0…25 | :11045,11046 |
| `ReliabilityUpDown` | `bReliability` | Items.Items | -10 | 10 | 363; -6…5 | 362; -6…5 | :3035,3036 |
| `RepairEaseUpDown` | `bRepairEase` | Items.Items | -10 | 10 | 603; -8…5 | 602; -8…5 | :3059,3060 |
| `BrightLightVisionRangeBonusUpDown` | `BrightLightVisionRangeBonus` | Items.Items | -1000 | 1000 | 19; -40…90 | 19; -40…90 | :4627,4628 |
| `BulletSpeedBonusUpDown` | `BulletSpeedBonus` | Items.Items | -100 | 100* | 39; -10…0 | 39; -10…0 | :6487 |
| `BurstSizeBonusUpDown` | `BurstSizeBonus` | Items.Items | -100 | 100* | 2; 0…3 | 2; 0…3 | :6387 |
| `BurstToHitBonusUpDown` | `BurstToHitBonus` | Items.Items | -100 | 100* | 68; 0…5 | 68; 0…5 | :6596 |
| `CamoBonusUpDown` | `CamoBonus` | Items.Items | -100 | 100* | 163; -20…100 | 163; -20…100 | :5066 |
| `CaveVisionRangeBonusUpDown` | `CaveVisionRangeBonus` | Items.Items | -1000 | 1000 | 14; -20…30 | 14; -20…30 | :4639,4640 |
| `DamageBonusUpDown` | `DamageBonus` | Items.Items | -100 | 100* | 9; -1…3 | 9; -1…3 | :6467 |
| `DayVisionRangeBonusUpDown` | `DayVisionRangeBonus` | Items.Items | -1000 | 1000 | 19; -20…100 | 19; -20…100 | :4651,4652 |
| `DesertCamoBonusUpDown` | `DesertCamoBonus` | Items.Items | -100 | 100* | 27; -20…100 | 27; -20…100 | :4996 |
| `FlashLightRangeUpDown` | `FlashLightRange` | Items.Items | -100 | 100* | 3; 0…18 | 3; 0…18 | :4616 |
| `HearingRangeBonusUpDown` | `HearingRangeBonus` | Items.Items | -100 | 100* | 2; 0…5 | 2; 0…5 | :4882 |
| `ItemSizeBonusUpDown` | `ItemSizeBonus` | Items.Items | -34 | 34 | 13; -1…1 | 13; -1…1 | :4917,4918 |
| `LockPickModifierUpDown` | `LockPickModifier` | Items.Items | -100 | 100* | 2; -10…0 | 2; -10…0 | :3898 |
| `MagSizeBonusUpDown` | `MagSizeBonus` | Items.Items | -250 | 1000 | 4; 0…70 | 4; 0…70 | :6397,6398 |
| `MeleeDamageBonusUpDown` | `MeleeDamageBonus` | Items.Items | -100 | 100* | 1; 0…0 | 1; 0…0 | :6206 |
| `MinRangeForAimBonusUpDown` | `MinRangeForAimBonus` | Items.Items | 0* | 1000 | 16; 0…70 | 16; 0…70 | :6720 |
| `LCUpDown` | `nasLayoutClass` | Items.Items | 0* | 2147483647 | 1557; 0…32 | 1556; 0…32 | :8835 |
| `NightVisionRangeBonusUpDown` | `NightVisionRangeBonus` | Items.Items | -1000 | 1000 | 20; -10…45 | 20; -10…45 | :4663,4664 |
| `CooldownModifierUpDown` | `overheatCooldownModificator` | Items.Items | -1 | 10 | 6; -0.2…0 | 6; -0.2…0 | :11890,11891 |
| `DamageThresholdModifierUpDown` | `overheatDamageThresholdModificator` | Items.Items | -1 | 10 | 1; 0…0 | 1; 0…0 | :11942,11943 |
| `JamThresholdModifierUpDown` | `overheatJamThresholdModificator` | Items.Items | -1 | 10 | 1; 0…0 | 1; 0…0 | :11916,11917 |
| `TemperatureModifierUpDown` | `overheatTemperatureModificator` | Items.Items | -1 | 10 | 1; 0…0 | 1; 0…0 | :11864,11865 |
| `PercentAccuracyModifierUpDown` | `PercentAccuracyModifier` | Items.Items | -100 | 100* | 43; -10…10 | 43; -10…10 | :5912 |
| `PercentAPReductionUpDown` | `PercentAPReduction` | Items.Items | -100 | 100* | 12; -9…20 | 12; -9…20 | :6911 |
| `PercentAutofireAPReductionUpDown` | `PercentAutofireAPReduction` | Items.Items | -100 | 100* | 2; 0…50 | 2; 0…50 | :6811 |
| `PercentBurstFireAPReductionUpDown` | `PercentBurstFireAPReduction` | Items.Items | -100 | 100* | 2; 0…80 | 2; 0…80 | :6821 |
| `PercentNoiseReductionUpDown` | `PercentNoiseReduction` | Items.Items | -100 | 100* | 106; 0…90 | 106; 0…90 | :6283 |
| `PercentReadyTimeAPReductionUpDown` | `PercentReadyTimeAPReduction` | Items.Items | -100 | 100* | 8; 0…33 | 8; 0…33 | :6841 |
| `PercentRecoilModifierUpDown` | `PercentRecoilModifier` | Items.Items | -100 | 100* | 59; -40…0 | 59; -40…0 | :5878 |
| `PercentReloadTimeAPReductionUpDown` | `PercentReloadTimeAPReduction` | Items.Items | -100 | 100* | 4; -30…0 | 4; -30…0 | :6831 |
| `PercentStatusDrainReductionUpDown` | `PercentStatusDrainReduction` | Items.Items | -100 | 100* | 3; -300…0 | 3; -300…0 | :4836 |
| `PercentTunnelVisionUpDown` | `PercentTunnelVision` | Items.Items | -100 | 100* | 33; 0…90 | 33; 0…90 | :4594 |
| `ProjectionFactorUpDown` | `ProjectionFactor` | Items.Items | 0* | 20 | 12; 0…3.5 | 12; 0…3.5 | :5806 |
| `randomitemcoolnessmodificatorUpDown` | `randomitemcoolnessmodificator` | Items.Items | -20 | 20 | 3; -1…1 | 3; -1…1 | :4044,4045 |
| `RangeBonusUpDown` | `RangeBonus` | Items.Items | -100 | 10000 | 85; -80…120 | 85; -80…120 | :6408,6409 |
| `RateOfFireBonusUpDown` | `RateOfFireBonus` | Items.Items | -100 | 100* | 1; 0…0 | 1; 0…0 | :6477 |
| `RecoilModifierXUpDown` | `RecoilModifierX` | Items.Items | -20 | 20 | 1; 0…0 | 1; 0…0 | :5830,5831 |
| `RecoilModifierYUpDown` | `RecoilModifierY` | Items.Items | -20 | 20 | 1; 0…0 | 1; 0…0 | :5855,5856 |
| `RepairModifierUpDown` | `RepairModifier` | Items.Items | -100 | 100* | 1; 0…0 | 1; 0…0 | :3887 |
| `BackpackWeightModifierUpDown` | `sBackpackWeightModifier` | Items.Items | -10000 | 10000 | 9; -1…0 | 9; -1…0 | :11257,11258 |
| `ScopeMagFactorUpDown` | `ScopeMagFactor` | Items.Items | 0* | 20 | 19; 0…10 | 19; 0…10 | :5782 |
| `SnowCamoBonusUpDown` | `SnowCamoBonus` | Items.Items | -100 | 100* | 13; -20…0 | 13; -20…0 | :4985 |
| `StealthBonusUpDown` | `StealthBonus` | Items.Items | -100 | 100* | 94; -33…25 | 94; -33…25 | :5077 |
| `ToHitBonusUpDown` | `ToHitBonus` | Items.Items | -100 | 100* | 42; -8…25 | 42; -8…25 | :6674 |
| `CoolnessUpDown` | `ubCoolness` | Items.Items | 0* | 10 | 1470; 0…10 | 1469; 0…10 | :3083 |
| `PerPocketUpDown` | `ubPerPocket` | Items.Items | 0* | 8 | 939; 0…8 | 939; 0…8 | :2989 |
| `WeightUpDown` | `ubWeight` | Items.Items | 0* | 32000 | 1720; 0…10000 | 1719; 0…10000 | :2954 |
| `uiIndexUpDown` | `uiIndex` | Items.Items | 0* | 16000 | 1776; 0…1775 | 1775; 0…1774 | :12472 |
| `UrbanCamoBonusUpDown` | `UrbanCamoBonus` | Items.Items | -100 | 100* | 32; -20…100 | 32; -20…100 | :5007 |
| `CooldownFactorUpDown` | `usOverheatingCooldownFactor` | Items.Items | 0* | 60000 | 1759; 100…137 | 1758; 100…137 | :11839 |
| `PriceUpDown` | `usPrice` | Items.Items | 0* | 1000000 | 1689; 0…70000 | 1688; 0…70000 | :2930 |
| `usSpottingUpDown` | `usSpotting` | Items.Items | -100 | 100* | 3; 0…70 | 3; 0…70 | :4605 |
| `VisionRangeBonusUpDown` | `VisionRangeBonus` | Items.Items | -1000 | 1000 | 5; 0…50 | 5; 0…50 | :4747,4748 |
| `bAccuracyUpDown` | `WeaponbAccuracy` | Items.Weapons | -100 | 100* | 270; -2…12 | 269; -2…12 | :7277 |
| `bRecoilXUpDown` | `WeaponbRecoilX` | Items.Weapons | -20 | 20 | 157; 0…4 | 157; 0…4 | :6012,6013 |
| `bRecoilYUpDown` | `WeaponbRecoilY` | Items.Weapons | -20 | 20 | 189; 0…14 | 189; 0…14 | :6037,6038 |
| `BurstAniDelayUpDown` | `WeaponBurstAniDelay` | Items.Weapons | 0* | 500 | 348; 0…125 | 347; 0…125 | :8010 |
| `MaxDistForMessyDeathUpDown` | `WeaponMaxDistForMessyDeath` | Items.Weapons | 0* | 500 | 389; 6…30 | 388; 6…30 | :7543 |
| `ubAimLevelsUpDown` | `WeaponubAimLevels` | Items.Weapons | 0* | 8 | 313; 0…5 | 312; 0…5 | :6060 |
| `ubDeadlinessUpDown` | `WeaponubDeadliness` | Items.Weapons | 0* | 250 | 386; 0…120 | 385; 0…120 | :7333 |
| `ubImpactUpDown` | `WeaponubImpact` | Items.Weapons | 0* | 250 | 365; 0…140 | 364; 0…140 | :7299 |
| `WeaponubMagSizeUpDown` | `WeaponubMagSize` | Items.Weapons | 0* | 65000 | 348; 1…200 | 347; 1…200 | :8099 |
| `ubRecoilDelayUpDown` | `WeaponubRecoilDelay` | Items.Weapons | 0* | 5 | 3; 0…3 | 3; 0…3 | :6082 |
| `DamageThresholdUpDown` | `WeaponusOverheatingDamageThreshold` | Items.Weapons | 0* | 60000 | 343; 2100…65650 | 342; 2100…65650 | :11729 |
| `JamThresholdUpDown` | `WeaponusOverheatingJamThreshold` | Items.Weapons | 0* | 60000 | 344; 4000…60000 | 343; 4000…60000 | :11704 |
| `PerShotTemperatureIncreaseUpDown` | `WeaponusOverheatingSingleShotTemperature` | Items.Weapons | 0* | 60000 | 342; 40…4000 | 341; 40…4000 | :11755 |
| `usRangeUpDown` | `WeaponusRange` | Items.Weapons | 0* | 5000 | 360; 0…3700 | 359; 0…3700 | :7532 |
| `usReloadDelayUpDown` | `WeaponusReloadDelay` | Items.Weapons | 0* | 500 | 387; 0…200 | 386; 0…200 | :8000 |
| `IndoorModifierUpDown` | `ExplosivebIndoorModifier` | Items.Explosives | -1 | 10 | 1; 0.25…0.25 | 1; 0.25…0.25 | :9952,9953 |
| `ubDamageUpDown` | `ExplosiveubDamage` | Items.Explosives | 0* | 250 | 106; 0…100 | 106; 0…100 | :9662 |
| `ubFragDamageUpDown` | `ExplosiveubFragDamage` | Items.Explosives | 0* | 250 | 106; 0…22 | 106; 0…22 | :9870 |
| `ubFragRangeUpDown` | `ExplosiveubFragRange` | Items.Explosives | 0* | 250 | 106; 0…100 | 106; 0…100 | :9893 |
| `NumericUpDown2` | `ExplosiveubHorizontalDegree` | Items.Explosives | 0* | 360 | 106; 0…360 | 106; 0…360 | :9577 |
| `ubStunDamageUpDown` | `ExplosiveubStunDamage` | Items.Explosives | 0* | 250 | 106; 0…200 | 106; 0…200 | :9684 |
| `NumericUpDown1` | `ExplosiveubVerticalDegree` | Items.Explosives | 0* | 360 | 106; 0…180 | 106; 0…180 | :9916 |
| `usNumFragmentsUpDown` | `ExplosiveusNumFragments` | Items.Explosives | 0* | 10000 | 106; 0…200 | 106; 0…200 | :9860 |
| `MagazineubMagSizeUpDown` | `MagazineubMagSize` | Items.Magazines | 0* | 65000 | 613; 1…5000 | 613; 1…5000 | :10178 |
| `lbeAvailableVolumeUpdown` | `LoadBearingEquipmentlbeAvailableVolume` | Items.LoadBearingEquipment | 0* | 255 | 108; 0…100 | 108; 0…100 | :11223 |
| `lbeComboUpDown` | `LoadBearingEquipmentlbeCombo` | Items.LoadBearingEquipment | 0* | 250 | 108; 0…4 | 108; 0…4 | :11176 |
| `lbeFilledSizeUpDown` | `LoadBearingEquipmentlbeFilledSize` | Items.LoadBearingEquipment | 0* | 65000 | 108; 0…34 | 108; 0…34 | :11187 |
| `lbePocketsAvailableUpDown` | `LoadBearingEquipmentlbePocketsAvailable` | Items.LoadBearingEquipment | 0* | 4095 | 108; 0…4067 | 108; 0…4067 | :11234 |
| `StandFlatBaseUpDown`, `CrouchFlatBaseUpDown`, `ProneFlatBaseUpDown` | `FlatBase1`/`FlatBase2`/`FlatBase3` | Items.Items | -101 | 100* | 3; 0…0 | 3; 0…0 | :5260,5270,5280 † |
| `StandPercentBaseUpDown`, `CrouchPercentBaseUpDown`, `PronePercentBaseUpDown` | `PercentBase1`/`PercentBase2`/`PercentBase3` | Items.Items | -101 | 100* | 261; -88…0 | 261; -88…0 | :5302,5312,5322 † |
| `StandFlatAimUpDown`, `CrouchFlatAimUpDown`, `ProneFlatAimUpDown` | `FlatAim1`/`FlatAim2`/`FlatAim3` | Items.Items | -101 | 100* | 3; 0…0 | 3; 0…0 | :5344,5354,5364 † |
| `StandPercentAimUpDown`, `CrouchPercentAimUpDown`, `PronePercentAimUpDown` | `PercentAim1`/`PercentAim2`/`PercentAim3` | Items.Items | -101 | 100* | 3; 0…0 | 3; 0…0 | :5638,5648,5658 † |
| `StandPercentCapUpDown`, `CrouchPercentCapUpDown`, `PronePercentCapUpDown` | `PercentCap1`/`PercentCap2`/`PercentCap3` | Items.Items | -101 | 100* | 311; -33…0 | 311; -33…0 | :5386,5396,5406 † |
| `StandPercentHandlingUpDown`, `CrouchPercentHandlingUpDown`, `PronePercentHandlingUpDown` | `PercentHandling1`/`PercentHandling2`/`PercentHandling3` | Items.Items | -101 | 100* | 84; -75…20 | 84; -75…20 | :5428,5438,5448 † |
| `StandPercentTargetTrackingSpeedUpDown`, `CrouchPercentTargetTrackingSpeedUpDown`, `PronePercentTargetTrackingSpeedUpDown` | `PercentTargetTrackingSpeed1`/`PercentTargetTrackingSpeed2`/`PercentTargetTrackingSpeed3` | Items.Items | -101 | 100* | 3; 0…0 | 3; 0…0 | :5470,5480,5490 † |
| `StandPercentDropCompensationUpDown`, `CrouchPercentDropCompensationUpDown`, `PronePercentDropCompensationUpDown` | `PercentDropCompensation1`/`PercentDropCompensation2`/`PercentDropCompensation3` | Items.Items | -101 | 100* | 3; 0…0 | 3; 0…0 | :5512,5522,5532 † |
| `StandPercentMaxCounterForceUpDown`, `CrouchPercentMaxCounterForceUpDown`, `PronePercentMaxCounterForceUpDown` | `PercentMaxCounterForce1`/`PercentMaxCounterForce2`/`PercentMaxCounterForce3` | Items.Items | -101 | 100* | 43; -30…100 | 43; -30…100 | :5554,5564,5574 † |
| `StandPercentCounterForceAccuracyUpDown`, `CrouchPercentCounterForceAccuracyUpDown`, `PronePercentCounterForceAccuracyUpDown` | `PercentCounterForceAccuracy1`/`PercentCounterForceAccuracy2`/`PercentCounterForceAccuracy3` | Items.Items | -101 | 100* | 363; -88…35 | 363; -88…35 | :5596,5606,5616 † |
| `StandAimLevelsUpDown`, `CrouchAimLevelsUpDown`, `ProneAimLevelsUpDown` | `AimLevels1`/`AimLevels2`/`AimLevels3` | Items.Items | -101 | 100* | 24; -1…3 | 24; -1…3 | :5680,5690,5700 † |

### 2.1. Динамические границы (вне статических метаданных)

- `ItemSizeUpDown` (`ItemSize`, `Items.Items`): в дизайнере `Maximum = 0` (`:3012`), это значение перекрывается в `Initialize`; при открытии `Maximum = ItemSizeMax` (`ItemDataForm.vb:62`), где `ItemSizeMax` = `MAX_ITEM_SIZE` из `ja2_options.ini` (`DatabaseCode.vb:2206-2216`), дефолт 99 при отсутствии ключа (`:2215`). В обеих сборках `Ja2_Options.INI:123` задаёт **34**; данные содержат 120 записей `ItemSize = 99` (uiIndex 128, 160 и 118 записей в диапазоне 839–1169); значений > 99 нет. Предлагается статическая граница 0..99 (§4, §6).
- `GraphicIndexUpDown` (`ubGraphicNum`, `Items.Items`): `Maximum = 5000` в дизайнере (`:3229`), но при загрузке заменяется на `SmallItems(...).Length - 1` (`ItemDataForm.vb:74`, `:807`) — динамика по числу изображений, M23b5; в метаданные не сеять.

## 3. Merc gear: статусы, quantity, цены

Привязка `Tag` = `<field><kit>` (`mHelmetStatus1`…`5`); в XML поле без суффикса, а загрузчик ищет колонку `<имя><kit>` (`MercStartingGearTable.vb:55-66`), поэтому в метаданных rewrite поля без суффикса (`0008_M20AuxiliaryTables.sql:1497-1563`). Все границы — в `MercGearDataForm.Designer.vb`.

| Семейство | Поля | Явные границы | Диапазон | v4/v5: n; min…max | Строки |
|---|---|---|---|---|---|
| Standard status | `mHelmetStatus`, `mVestStatus`, `mLegStatus`, `mWeaponStatus` | нет → default 100 | 0..100 | 2106; 0…100 | `:1143`, `:1152`, `:1161`, `:1170`; киты 2–5: `:2727-2754`, `:4312-4339`, `:5897-5924`, `:7482-7509` |
| LBE status | `lVestStatus`, `lLeftThighStatus`, `lRightThighStatus`, `lCPackStatus`, `lBPackStatus` | нет → default 100 | 0..100 | 1872; 0…100 | `:1444-1480`; киты 2–5: `:3039-3075`, `:4613-4649`, `:6198-6234`, `:7783-7819` |
| Big status | `mBig0Status`…`mBig3Status` | нет → default 100 | 0..100 | 1767; 0…100 | `:1743-1770`; киты 2–5: `:3328-3355`, `:4913-4940`, `:6498-6525`, `:8083-8110` |
| Small status | `mSmall0Status`…`mSmall7Status` | нет → default 100 | 0..100 | 2448; 0…600 | `:2226-2289`; киты 2–5: `:3811-3874`, `:5396-5459`, `:6981-7044`, `:8566-8629` |
| Big quantity | `mBig0Quantity`…`mBig3Quantity` | max = 10 (явно) | 0..10 | 1982; 0…10 | `:1916-1946`; киты 2–5: `:3501-3531`, `:5086-5116`, `:6671-6701`, `:8256-8286` |
| Small quantity 2–7 | `mSmall2Quantity`…`mSmall7Quantity` | max = 10 (явно) | 0..10 | 1694; 0…6 | `:2493-2543`, max `:2489-2539`; киты 2–5: `:4074-4124`, `:5659-5709`, `:7244-7294`, `:8829-8879` |
| Small quantity 0–1 | `mSmall0Quantity`, `mSmall1Quantity` | нет → default 100 | 0..100 | 779; 0…5 | `:2474`, `:2483` (max нет) |
| Price mod | `mPriceMod` | min −32000, max 32000 | −32000..32000 | 327; 0…50 | `:1551-1552`; киты 2–5: `:3147-3148`, `:4721-4722`, `:6306-6307`, `:7891-7892` |
| Absolute price | `mAbsolutePrice` | min −1, max 32000 | −1..32000 | 1275; −1…−1 | `:1562-1563`; киты 2–5: `:3158-3159`, `:4743-4744`, `:6328-6329`, `:7913-7914` |

- У статусов нет ни одного `Maximum`/`Minimum` (0 совпадений `Status\d\.(Maximum|Minimum)`), поэтому действует дефолт 100. `Maximum = 10` явно только у 10 из 12 quantity: `mBig0..3Quantity` (`:1916-1946`) и `mSmall2..7Quantity` (`:2489-2539`); у `mSmall0Quantity`/`mSmall1Quantity` присваивания нет → default 100 (`:2474`, `:2483`).
- `mAbsolutePrice` default −1 (`DatabaseCode.vb:2261`). Несоответствие дизайнера: `PriceModUpDown3/4/5.Tag = "mPriceMod2"` (`:4726`, `:6311`, `:7896`) — киты 3–5 привязаны к колонке 2-го набора; на границы не влияет, но Tag нельзя считать именем XML-поля без проверки.

## 4. Скан корпуса и нарушения

Записей: Items 1776/1775, Weapons 389/388, Magazines 613/613, Explosives 106/106, LBE 108/108, MercStartingGear 255/255 (по 5 `GEARKIT`). Все границы §2–§3 проверены на v4 и v5; ниже — только нарушения.

| Поле | Граница | v4 | v5 | Ключи | Вывод |
|---|---|---|---|---|---|
| `Items.Items.PercentStatusDrainReduction` | min −100 | min −300 (2) | min −300 (2) | uiIndex 1753, 1754 (Bandages) | false positive; не сеять |
| `Items.Weapons.usOverheatingDamageThreshold` | max 60000 | 65650 | 65650 | uiIndex 774 | false positive; не сеять |
| `Inventory.MercStartingGear.mSmall1Status` | max 100 | 600 | 600 | mIndex 237, kit 5 | известное нарушение; сеять 0..100 (warning) |
| `Items.Items.ItemSize` | max 99 | 99 (120 записей) | 99 (120) | 128, 160, 839–1169 | не нарушение (ровно на границе); при 34 — 120 нарушений |

Других нарушений нет: 102 item-строки и 60 merc-контролов. Значения на границе (`CamoBonus`/`UrbanCamoBonus` = 100, `usOverheatingJamThreshold` = 60000, `mAbsolutePrice` = −1) нарушением не являются. Все перечисленные значения (600, 65650, −300) rewrite обязан сохранять; предлагаемая валидация — только warning, без clamp и без перезаписи.

## 5. Рекомендуемый seed для 0015

Политика: сеять обе стороны, которые legacy реально применяла (явные и дефолтные 0/100), кроме перечисленного ниже. `ColumnHints` пока без min/max (`0003_ColumnHints.sql:5-18`); 0015 добавляет nullable `Minimum`/`Maximum`, `TableValidator` — warning, без clamp.

| Таблица | Поля | Min | Max |
|---|---|---|---|
| Items.Items | `sBackpackWeightModifier` | -10000 | 10000 |
| Items.Items | `BrightLightVisionRangeBonus`, `CaveVisionRangeBonus`, `DayVisionRangeBonus`, `NightVisionRangeBonus`, `VisionRangeBonus` | -1000 | 1000 |
| Items.Items | `MagSizeBonus` | -250 | 1000 |
| Items.Items | `APBonus`, `AimBonus`, `AutoFireToHitBonus`, `Bipod`, `BulletSpeedBonus`, `BurstSizeBonus`, `BurstToHitBonus`, `CamoBonus`, `DamageBonus`, `DesertCamoBonus`, `FlashLightRange`, `HearingRangeBonus`, `LockPickModifier`, `MeleeDamageBonus`, `PercentAPReduction`, `PercentAccuracyModifier`, `PercentAutofireAPReduction`, `PercentBurstFireAPReduction`, `PercentNoiseReduction`, `PercentReadyTimeAPReduction`, `PercentRecoilModifier`, `PercentReloadTimeAPReduction`, `PercentTunnelVision`, `RateOfFireBonus`, `RepairModifier`, `SnowCamoBonus`, `StealthBonus`, `ToHitBonus`, `UrbanCamoBonus`, `usSpotting` | -100 | 100 |
| Items.Items | `BR_NewInventory`, `BR_UsedInventory` | -100 | 250 |
| Items.Items | `RangeBonus` | -100 | 10000 |
| Items.Items | `ItemSizeBonus` | -34 | 34 |
| Items.Items | `RecoilModifierX`, `RecoilModifierY`, `randomitemcoolnessmodificator` | -20 | 20 |
| Items.Items | `bReliability`, `bRepairEase` | -10 | 10 |
| Items.Items | `overheatCooldownModificator`, `overheatDamageThresholdModificator`, `overheatJamThresholdModificator`, `overheatTemperatureModificator` | -1 | 10 |
| Items.Items | `ubPerPocket` | 0 | 8 |
| Items.Items | `ubCoolness` | 0 | 10 |
| Items.Items | `ProjectionFactor`, `ScopeMagFactor` | 0 | 20 |
| Items.Items | `ItemSize` | 0 | 99 |
| Items.Items | `MinRangeForAimBonus` | 0 | 1000 |
| Items.Items | `BR_ROF`, `BestLaserRange` | 0 | 10000 |
| Items.Items | `ubWeight` | 0 | 32000 |
| Items.Items | `usOverheatingCooldownFactor` | 0 | 60000 |
| Items.Items | `usPrice` | 0 | 1000000 |
| Items.Weapons | `bAccuracy` | -100 | 100 |
| Items.Weapons | `bRecoilX`, `bRecoilY` | -20 | 20 |
| Items.Weapons | `ubRecoilDelay` | 0 | 5 |
| Items.Weapons | `ubAimLevels` | 0 | 8 |
| Items.Weapons | `ubDeadliness`, `ubImpact` | 0 | 250 |
| Items.Weapons | `BurstAniDelay`, `MaxDistForMessyDeath`, `usReloadDelay` | 0 | 500 |
| Items.Weapons | `usRange` | 0 | 5000 |
| Items.Weapons | `usOverheatingJamThreshold`, `usOverheatingSingleShotTemperature` | 0 | 60000 |
| Items.Weapons | `ubMagSize` | 0 | 65000 |
| Items.Explosives | `bIndoorModifier` | -1 | 10 |
| Items.Explosives | `ubDamage`, `ubFragDamage`, `ubFragRange`, `ubStunDamage` | 0 | 250 |
| Items.Explosives | `ubHorizontalDegree`, `ubVerticalDegree` | 0 | 360 |
| Items.Explosives | `usNumFragments` | 0 | 10000 |
| Items.Magazines | `ubMagSize` | 0 | 65000 |
| Items.LoadBearingEquipment | `lbeCombo` | 0 | 250 |
| Items.LoadBearingEquipment | `lbeAvailableVolume` | 0 | 255 |
| Items.LoadBearingEquipment | `lbePocketsAvailable` | 0 | 4095 |
| Items.LoadBearingEquipment | `lbeFilledSize` | 0 | 65000 |
| Inventory.MercStartingGear | `mHelmetStatus`, `mVestStatus`, `mLegStatus`, `mWeaponStatus`, `lVestStatus`, `lLeftThighStatus`, `lRightThighStatus`, `lCPackStatus`, `lBPackStatus`, `mBig0Status`, `mBig1Status`, `mBig2Status`, `mBig3Status`, `mSmall0Status`, `mSmall1Status`, `mSmall2Status`, `mSmall3Status`, `mSmall4Status`, `mSmall5Status`, `mSmall6Status`, `mSmall7Status` | 0 | 100 |
| Inventory.MercStartingGear | `mBig0Quantity`, `mBig1Quantity`, `mBig2Quantity`, `mBig3Quantity`, `mSmall2Quantity`, `mSmall3Quantity`, `mSmall4Quantity`, `mSmall5Quantity`, `mSmall6Quantity`, `mSmall7Quantity` | 0 | 10 |
| Inventory.MercStartingGear | `mSmall0Quantity`, `mSmall1Quantity` | 0 | 100 |
| Inventory.MercStartingGear | `mPriceMod` | -32000 | 32000 |
| Inventory.MercStartingGear | `mAbsolutePrice` | -1 | 32000 |

Не сеять:

- `Items.Items.ubGraphicNum` — динамика по изображениям (§2.1).
- `Items.Items.nasLayoutClass` — `Maximum = Int32.MaxValue` (`:8835`), верхняя граница бессмысленна.
- `Items.Items.uiIndex` — ключ/идентификатор; 16000 (`:12472`) — защита legacy, не ограничение значения.
- 33 контрола стоек (`FlatBase*`…`AimLevels*`) — вложенные пути, `ColumnHints` плоский; M23b5.
- `Items.Items.PercentStatusDrainReduction`, `Items.Weapons.usOverheatingDamageThreshold` — нарушения §4.
- 29 item-контролов с `Tag`, но без явных границ (напр. `DisarmModifierUpDown`, `SleepModifierUpDown`) — только дефолт 0..100, «default-100 noise»; в §2 они не входят.

## 6. Открытые вопросы и риски

1. **`MAX_ITEM_SIZE` vs статические 99.** В обеих сборках ini задаёт 34, а данные содержат 120 записей `ItemSize = 99`; выбор 99 безопасен (0 нарушений), выбор 34 даёт 120 warning'ов. Читать ли ini в rewrite и переопределять ли статическую границу — решение не принято; сейчас планируется 99.
2. **Политика default-100.** Для полей с явной одной стороной вторая сторона — дефолт 0/100; seed включает её для паритета. Если такие границы сочтут шумом, их можно не сеять — данные v4/v5 допускают оба варианта.
3. **Известные warning'и.** Три значения вне legacy-диапазона (1753/1754, 774, mIndex 237 kit 5) останутся предупреждениями на shipped-данных; для двух item-полей рекомендуется не сеять границу, для status — сеять (warning ожидаем).
4. **Вложенные стойки.** Если M23b5 будет валидировать `STAND/CROUCH/PRONE_MODIFIERS`, `ColumnHints(XmlFieldName)` не хватит: нужен путь или отдельная таблица.
5. **Decimal-поля.** `bIndoorModifier`, `ProjectionFactor`, `overheat*Modificator` содержат дробные значения (`0.25`, `3.5`, `-0.2`); `Minimum`/`Maximum` в SQLite должны быть числовыми, а не только INTEGER.
6. **`ItemSizeBonus` = ±34** — литерал дизайнера (`:4917-4918`), совпадает с ini-значением 34 случайно; связь с `MAX_ITEM_SIZE` не доказана.

## 7. Реализация (M23b4)

Реализовано в `0015_M23b4ValidationLimits.sql` и `TableValidator`; решения и проверки — в
`52-m23b4-implementation-log.md`.

- Засеяны все 123 поля §5, с исправлением: `mSmall0Quantity`/`mSmall1Quantity` — 0…100 (у legacy нет
  `Maximum`, действует default 100), остальные quantity — 0…10. `ItemSize` — 0…99 (§6.1); чтение
  `MAX_ITEM_SIZE` из ini не реализовано.
- Два противоречащих данным предела не засеяны (§4); 33 вложенных контрола стоек, `ubGraphicNum`,
  `nasLayoutClass` и `uiIndex` — тоже (M23b5 / ключ / бессмысленная граница).
- `Minimum`/`Maximum` — целые (все legacy-границы целые), но значение сравнивается как `decimal`,
  поэтому §6.5 закрыт для дробных данных; дробные *границы* по-прежнему не выражаются.
- Единственное предупреждение на shipped-данных — `mSmall1Status` = 600 (mIndex 237, kit 5),
  закреплено corpus-тестом; оба false positive исключены, их предупреждений нет.
