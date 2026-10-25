PRAGMA foreign_keys = ON;

-- M23b4: non-destructive validation of numeric ranges, exclusive choices and class-dependent limits.
--
-- The legacy editor enforced these rules through WinForms NumericUpDown.Minimum/Maximum, radio
-- buttons and the lbeClass SelectedIndexChanged handler. The rewrite must never clamp or clear a
-- value: existing data (for example mSmall1Status = 600) is preserved and reported instead.
--
-- Legacy evidence:
--   References/xml-editor/BackEnd/DatabaseCode.vb:2207-2215   MAX_ITEM_SIZE, default 99
--   References/xml-editor/GUI/Data Forms/ItemDataForm.vb:62   ItemSizeUpDown.Maximum = ItemSizeMax
--   References/xml-editor/GUI/Data Forms/MercGearDataForm.Designer.vb:1140-1170, 1551-1563,
--                                                            1916-1946, 2489-2539
--   References/xml-editor/GUI/Data Forms/ItemDataForm.Designer.vb:8207-8246 (four launcher radios)
--   References/xml-editor/GUI/Data Forms/ItemDataForm.vb:1566 (lbeClass -> pockets)
--
-- The ranges below were extracted from the designer files and checked against both reference data
-- sets: every seeded range is inclusive and all v4/v5 values fall inside it, except the known
-- mSmall1Status = 600 (mIndex 237, kit 5) that stays a warning. Two legacy limits contradict the
-- shipped data (Items.Items.PercentStatusDrainReduction = -300 and
-- Items.Weapons.usOverheatingDamageThreshold = 65650) and are deliberately not seeded.

-- Inclusive bounds for a numeric field. Validation warns but never clamps; NULL means unbounded.
-- Every bound in this migration is integral, so validation compares the parsed value as a decimal.
ALTER TABLE ColumnHints ADD COLUMN Minimum INTEGER NULL;
ALTER TABLE ColumnHints ADD COLUMN Maximum INTEGER NULL;

-- Groups of boolean fields where at most one field may be set. The launcher type is the first user.
CREATE TABLE ExclusiveFlagGroups (
    TableId INTEGER NOT NULL,
    GroupKey TEXT NOT NULL,
    XmlFieldName TEXT NOT NULL,
    Ordinal INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (TableId, GroupKey, XmlFieldName),
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

CREATE INDEX IX_ExclusiveFlagGroups_TableId ON ExclusiveFlagGroups (TableId);

-- Limits of indexed fields that depend on a class value. Legacy disabled pockets that a class does
-- not support; the rewrite keeps existing values and warns about the ones above the limit.
CREATE TABLE ClassValueLimits (
    TableId INTEGER NOT NULL,
    ClassField TEXT NOT NULL,
    ClassValue TEXT NOT NULL,
    FieldMask TEXT NOT NULL,
    MaxOrdinal INTEGER NOT NULL,
    PRIMARY KEY (TableId, ClassField, ClassValue),
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

CREATE INDEX IX_ClassValueLimits_TableId ON ClassValueLimits (TableId);

-- The four launcher types used to share one radio group; "N/A" (all four unset) stays valid.
WITH fields(XmlFieldName, Ordinal) AS (VALUES
    ('Cannon', 0),
    ('Mortar', 1),
    ('GrenadeLauncher', 2),
    ('RocketLauncher', 3))
INSERT INTO ExclusiveFlagGroups (TableId, GroupKey, XmlFieldName, Ordinal)
SELECT t.Id, 'launcher_type', f.XmlFieldName, f.Ordinal
FROM fields f
JOIN Tables t ON t.XmlTable = 'Items.Items';

-- Legacy map: class 0 disables every pocket, class 1 enables the first five (thigh pack), class 2
-- and 4 enable all twelve (vest and backpack), class 3 enables the first seven (combat pack).
-- Branches 2 and 4 are identical in the legacy code and in the data, so the limit is identical.
WITH limits(ClassValue, MaxOrdinal) AS (VALUES
    ('0', 0),
    ('1', 5),
    ('2', 12),
    ('3', 7),
    ('4', 12))
INSERT INTO ClassValueLimits (TableId, ClassField, ClassValue, FieldMask, MaxOrdinal)
SELECT t.Id, 'lbeClass', l.ClassValue, 'lbePocketIndex%Index%', l.MaxOrdinal
FROM limits l
JOIN Tables t ON t.XmlTable = 'Items.LoadBearingEquipment';

-- Inclusive numeric ranges of the fields that the legacy forms bounded. Values are never clamped:
-- a value outside its range is reported by validation and written back unchanged.

-- Items.Items: general stats, combat modifiers and overheat settings.
UPDATE ColumnHints SET Minimum = -10000, Maximum = 10000
WHERE XmlFieldName IN ('sBackpackWeightModifier')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = -1000, Maximum = 1000
WHERE XmlFieldName IN (
    'BrightLightVisionRangeBonus', 'CaveVisionRangeBonus', 'DayVisionRangeBonus',
    'NightVisionRangeBonus', 'VisionRangeBonus')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = -250, Maximum = 1000
WHERE XmlFieldName IN ('MagSizeBonus')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = -100, Maximum = 100
WHERE XmlFieldName IN (
    'APBonus', 'AimBonus', 'AutoFireToHitBonus', 'Bipod', 'BulletSpeedBonus', 'BurstSizeBonus',
    'BurstToHitBonus', 'CamoBonus', 'DamageBonus', 'DesertCamoBonus', 'FlashLightRange',
    'HearingRangeBonus', 'LockPickModifier', 'MeleeDamageBonus', 'PercentAPReduction',
    'PercentAccuracyModifier', 'PercentAutofireAPReduction', 'PercentBurstFireAPReduction',
    'PercentNoiseReduction', 'PercentReadyTimeAPReduction', 'PercentRecoilModifier',
    'PercentReloadTimeAPReduction', 'PercentTunnelVision', 'RateOfFireBonus', 'RepairModifier',
    'SnowCamoBonus', 'StealthBonus', 'ToHitBonus', 'UrbanCamoBonus', 'usSpotting')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = -100, Maximum = 250
WHERE XmlFieldName IN ('BR_NewInventory', 'BR_UsedInventory')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = -100, Maximum = 10000
WHERE XmlFieldName IN ('RangeBonus')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = -34, Maximum = 34
WHERE XmlFieldName IN ('ItemSizeBonus')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = -20, Maximum = 20
WHERE XmlFieldName IN ('RecoilModifierX', 'RecoilModifierY', 'randomitemcoolnessmodificator')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = -10, Maximum = 10
WHERE XmlFieldName IN ('bReliability', 'bRepairEase')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = -1, Maximum = 10
WHERE XmlFieldName IN (
    'overheatCooldownModificator', 'overheatDamageThresholdModificator',
    'overheatJamThresholdModificator', 'overheatTemperatureModificator')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = 0, Maximum = 8
WHERE XmlFieldName IN ('ubPerPocket')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = 0, Maximum = 10
WHERE XmlFieldName IN ('ubCoolness')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = 0, Maximum = 20
WHERE XmlFieldName IN ('ProjectionFactor', 'ScopeMagFactor')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

-- MAX_ITEM_SIZE defaults to 99; the shipped ini uses 34 and 120 records carry 99, so 99 is the
-- bound that matches both the legacy default and the data.
UPDATE ColumnHints SET Minimum = 0, Maximum = 99
WHERE XmlFieldName IN ('ItemSize')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = 0, Maximum = 1000
WHERE XmlFieldName IN ('MinRangeForAimBonus')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = 0, Maximum = 10000
WHERE XmlFieldName IN ('BR_ROF', 'BestLaserRange')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = 0, Maximum = 32000
WHERE XmlFieldName IN ('ubWeight')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = 0, Maximum = 60000
WHERE XmlFieldName IN ('usOverheatingCooldownFactor')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints SET Minimum = 0, Maximum = 1000000
WHERE XmlFieldName IN ('usPrice')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

-- Items.Weapons: accuracy, recoil, overheating thresholds and range.
UPDATE ColumnHints SET Minimum = -100, Maximum = 100
WHERE XmlFieldName IN ('bAccuracy')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Weapons');

UPDATE ColumnHints SET Minimum = -20, Maximum = 20
WHERE XmlFieldName IN ('bRecoilX', 'bRecoilY')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Weapons');

UPDATE ColumnHints SET Minimum = 0, Maximum = 5
WHERE XmlFieldName IN ('ubRecoilDelay')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Weapons');

UPDATE ColumnHints SET Minimum = 0, Maximum = 8
WHERE XmlFieldName IN ('ubAimLevels')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Weapons');

UPDATE ColumnHints SET Minimum = 0, Maximum = 250
WHERE XmlFieldName IN ('ubDeadliness', 'ubImpact')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Weapons');

UPDATE ColumnHints SET Minimum = 0, Maximum = 500
WHERE XmlFieldName IN ('BurstAniDelay', 'MaxDistForMessyDeath', 'usReloadDelay')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Weapons');

UPDATE ColumnHints SET Minimum = 0, Maximum = 5000
WHERE XmlFieldName IN ('usRange')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Weapons');

UPDATE ColumnHints SET Minimum = 0, Maximum = 60000
WHERE XmlFieldName IN ('usOverheatingJamThreshold', 'usOverheatingSingleShotTemperature')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Weapons');

UPDATE ColumnHints SET Minimum = 0, Maximum = 65000
WHERE XmlFieldName IN ('ubMagSize')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Weapons');

-- Items.Explosives: damage, fragmentation cone and the legacy indoor modifier.
UPDATE ColumnHints SET Minimum = -1, Maximum = 10
WHERE XmlFieldName IN ('bIndoorModifier')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Explosives');

UPDATE ColumnHints SET Minimum = 0, Maximum = 250
WHERE XmlFieldName IN ('ubDamage', 'ubFragDamage', 'ubFragRange', 'ubStunDamage')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Explosives');

UPDATE ColumnHints SET Minimum = 0, Maximum = 360
WHERE XmlFieldName IN ('ubHorizontalDegree', 'ubVerticalDegree')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Explosives');

UPDATE ColumnHints SET Minimum = 0, Maximum = 10000
WHERE XmlFieldName IN ('usNumFragments')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Explosives');

-- Items.Magazines and Items.LoadBearingEquipment.
UPDATE ColumnHints SET Minimum = 0, Maximum = 65000
WHERE XmlFieldName IN ('ubMagSize')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Magazines');

UPDATE ColumnHints SET Minimum = 0, Maximum = 250
WHERE XmlFieldName IN ('lbeCombo')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.LoadBearingEquipment');

UPDATE ColumnHints SET Minimum = 0, Maximum = 255
WHERE XmlFieldName IN ('lbeAvailableVolume')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.LoadBearingEquipment');

UPDATE ColumnHints SET Minimum = 0, Maximum = 4095
WHERE XmlFieldName IN ('lbePocketsAvailable')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.LoadBearingEquipment');

UPDATE ColumnHints SET Minimum = 0, Maximum = 65000
WHERE XmlFieldName IN ('lbeFilledSize')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.LoadBearingEquipment');

-- Inventory.MercStartingGear: every status is a WinForms default 0..100 spinner.
UPDATE ColumnHints SET Minimum = 0, Maximum = 100
WHERE XmlFieldName IN (
    'mHelmetStatus', 'mVestStatus', 'mLegStatus', 'mWeaponStatus',
    'lVestStatus', 'lLeftThighStatus', 'lRightThighStatus', 'lCPackStatus', 'lBPackStatus',
    'mBig0Status', 'mBig1Status', 'mBig2Status', 'mBig3Status',
    'mSmall0Status', 'mSmall1Status', 'mSmall2Status', 'mSmall3Status',
    'mSmall4Status', 'mSmall5Status', 'mSmall6Status', 'mSmall7Status')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Inventory.MercStartingGear');

-- The big pockets and the small pockets 3..8 used an explicit maximum of 10. The first two small
-- pockets kept the WinForms default of 100.
UPDATE ColumnHints SET Minimum = 0, Maximum = 10
WHERE XmlFieldName IN (
    'mBig0Quantity', 'mBig1Quantity', 'mBig2Quantity', 'mBig3Quantity',
    'mSmall2Quantity', 'mSmall3Quantity', 'mSmall4Quantity', 'mSmall5Quantity',
    'mSmall6Quantity', 'mSmall7Quantity')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Inventory.MercStartingGear');

UPDATE ColumnHints SET Minimum = 0, Maximum = 100
WHERE XmlFieldName IN ('mSmall0Quantity', 'mSmall1Quantity')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Inventory.MercStartingGear');

-- Price limits: mPriceMod is -32000..32000 and mAbsolutePrice is -1..32000.
UPDATE ColumnHints SET Minimum = -32000, Maximum = 32000
WHERE XmlFieldName IN ('mPriceMod')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Inventory.MercStartingGear');

UPDATE ColumnHints SET Minimum = -1, Maximum = 32000
WHERE XmlFieldName IN ('mAbsolutePrice')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Inventory.MercStartingGear');
