PRAGMA foreign_keys = ON;

-- M23b3: value sources for pickers whose options the referenced table cannot supply on its own.
--
-- Three metadata gaps remain after M23b2:
--   * Robot skill grants are fixed values of the game enum SkillTraitNew. They have no Lookup XML
--     file, so LookupValues carries the curated list and the item hints point at it.
--   * Item sounds are stored as indices into Sounds.Sounds/Sounds.BurstSounds, whose records are
--     positional and have no key element. Those relations address the reserved #Key field and use
--     the record text as display field.
--   * Some pickers need the legacy "nothing selected" behaviour: filtered merc gear slots offered
--     item 0 (Nothing) as well, and ExplosionData.AltExplosionSoundID uses -1 for "no sound".
--
-- Legacy evidence:
--   References/xml-editor/GUI/Data Forms/ItemDataForm.vb:347 (SkillTraitNew combo values)

-- Picker behaviour. IncludeKeyZero keeps the target record 0 selectable even when it fails the
-- relation filter. NoneValue/NoneDisplay add one synthetic "no reference" option that validation
-- accepts although the target document has no such record.
ALTER TABLE TableRelations ADD COLUMN IncludeKeyZero INTEGER NOT NULL DEFAULT 0;
ALTER TABLE TableRelations ADD COLUMN NoneValue TEXT NULL;
ALTER TABLE TableRelations ADD COLUMN NoneDisplay TEXT NULL;

-- Curated values for lookups that have no TableData/Lookup XML file. Ordinal preserves the intended
-- order independently of the numeric value.
CREATE TABLE LookupValues (
    LookupName TEXT NOT NULL,
    Value INTEGER NOT NULL,
    DisplayName TEXT NOT NULL,
    Ordinal INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (LookupName, Value)
);

-- SkillTraitNew in game enum order. The legacy editor populated the three robot skill combo boxes
-- from this exact list; the names are stored verbatim because they are the in-game identifiers.
INSERT INTO LookupValues (LookupName, Value, DisplayName, Ordinal) VALUES
    ('Lookup.SkillTrait', 0, 'NoTrait', 0),
    ('Lookup.SkillTrait', 1, 'AutoWeapons', 1),
    ('Lookup.SkillTrait', 2, 'HeavyWeapons', 2),
    ('Lookup.SkillTrait', 3, 'Marksman', 3),
    ('Lookup.SkillTrait', 4, 'Hunter', 4),
    ('Lookup.SkillTrait', 5, 'Gunslinger', 5),
    ('Lookup.SkillTrait', 6, 'HandToHand', 6),
    ('Lookup.SkillTrait', 7, 'Deputy', 7),
    ('Lookup.SkillTrait', 8, 'Technician', 8),
    ('Lookup.SkillTrait', 9, 'Paramedic', 9),
    ('Lookup.SkillTrait', 10, 'Ambidextrous', 10),
    ('Lookup.SkillTrait', 11, 'Melee', 11),
    ('Lookup.SkillTrait', 12, 'Throwing', 12),
    ('Lookup.SkillTrait', 13, 'NightOps', 13),
    ('Lookup.SkillTrait', 14, 'Stealthy', 14),
    ('Lookup.SkillTrait', 15, 'Athletics', 15),
    ('Lookup.SkillTrait', 16, 'Bodybuilding', 16),
    ('Lookup.SkillTrait', 17, 'Demolitions', 17),
    ('Lookup.SkillTrait', 18, 'Teaching', 18),
    ('Lookup.SkillTrait', 19, 'Scouting', 19),
    ('Lookup.SkillTrait', 20, 'Covert', 20),
    ('Lookup.SkillTrait', 21, 'RadioOperator', 21),
    ('Lookup.SkillTrait', 22, 'Snitch', 22),
    ('Lookup.SkillTrait', 23, 'Survival', 23);

-- The robot skill grants are curated enum values, not references to a table.
UPDATE ColumnHints
SET DataType = 'Lookup',
    EditorKind = 'LookupCombo',
    LookupTable = 'Lookup.SkillTrait'
WHERE TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items')
  AND XmlFieldName IN ('RobotTargetingSkillGrant', 'RobotUtilitySkillGrant', 'RobotChassisSkillGrant');

-- Every SOUND record is one text value and the list key is positional, so the picker stores the
-- zero-based record position in #Key and displays the record text.
WITH fields(SourceField, TargetTable) AS (VALUES
    ('sSound', 'Sounds.Sounds'),
    ('SilencedSound', 'Sounds.Sounds'),
    ('sReloadSound', 'Sounds.Sounds'),
    ('sLocknLoadSound', 'Sounds.Sounds'),
    ('ManualReloadSound', 'Sounds.Sounds'),
    ('sBurstSound', 'Sounds.BurstSounds'),
    ('sSilencedBurstSound', 'Sounds.BurstSounds'))
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, f.SourceField, t.Id, '#Key', 'Text', 'Reference', NULL, NULL, NULL, 0, 40
FROM fields f
JOIN Tables s ON s.XmlTable = 'Items.Weapons'
JOIN Tables t ON t.XmlTable = f.TargetTable;

-- Explosion sounds address the same positional sound list. ExplosionSoundID always names a sound;
-- AltExplosionSoundID additionally uses -1 for "no alternate sound".
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'ExplosionSoundID', t.Id, '#Key', 'Text', 'Reference', NULL, NULL, NULL, 0, 40
FROM Tables s, Tables t
WHERE s.XmlTable = 'Items.ExplosionData' AND t.XmlTable = 'Sounds.Sounds';

INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal, NoneValue, NoneDisplay)
SELECT s.Id, 'AltExplosionSoundID', t.Id, '#Key', 'Text', 'Reference', NULL, NULL, NULL, 0, 40, '-1', 'None'
FROM Tables s, Tables t
WHERE s.XmlTable = 'Items.ExplosionData' AND t.XmlTable = 'Sounds.Sounds';

-- The explosive's animation is the explosion-data row with the same uiIndex.
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'ubAnimationID', t.Id, 'uiIndex', 'name', 'Reference', NULL, NULL, NULL, 1, 40
FROM Tables s, Tables t
WHERE s.XmlTable = 'Items.Explosives' AND t.XmlTable = 'Items.ExplosionData';

-- Load-bearing equipment stores up to twelve pocket indices; pockets are addressed by pIndex.
WITH fields(SourceField) AS (VALUES
    ('lbePocketIndex1'),
    ('lbePocketIndex2'),
    ('lbePocketIndex3'),
    ('lbePocketIndex4'),
    ('lbePocketIndex5'),
    ('lbePocketIndex6'),
    ('lbePocketIndex7'),
    ('lbePocketIndex8'),
    ('lbePocketIndex9'),
    ('lbePocketIndex10'),
    ('lbePocketIndex11'),
    ('lbePocketIndex12'))
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, f.SourceField, t.Id, 'pIndex', 'pName', 'Reference', NULL, NULL, NULL, 1, 40
FROM fields f
JOIN Tables s ON s.XmlTable = 'Items.LoadBearingEquipment'
JOIN Tables t ON t.XmlTable = 'Items.Pockets';

-- Legacy gear-slot pickers offered item 0 (Nothing) in addition to the class-filtered items, so a
-- slot could always be cleared. The unfiltered gear slots already list item 0.
UPDATE TableRelations
SET IncludeKeyZero = 1
WHERE SourceTableId = (SELECT Id FROM Tables WHERE XmlTable = 'Inventory.MercStartingGear')
  AND SourceField IN (
      'mHelmet', 'mVest', 'mLeg', 'lVest',
      'lLeftThigh', 'lRightThigh', 'lCPack', 'lBPack');
