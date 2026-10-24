PRAGMA foreign_keys = ON;

-- M23b1: class-aware visibility of item-workspace sections.
--
-- The legacy item form physically removed tab pages for each usItemClass value
-- (References/xml-editor/GUI/Data Forms/ItemDataForm.vb, DisplayTabs). The rewrite keeps the pages
-- but hides the matching field groups and related sections. The mapping stays in metadata so the UI
-- layer has no hardcoded class names; a section without a row is always visible.
--
-- VisibleClasses is a bit mask over ItemClass values. A negative value means "always visible",
-- including records where usItemClass is absent. Masks are supersets of the classes that legacy showed:
-- a group that merged several legacy pages stays visible when at least one of them was visible.

CREATE TABLE ClassVisibilityRules (
    TableId INTEGER NOT NULL,
    Kind TEXT NOT NULL,
    SectionKey TEXT NOT NULL,
    VisibleClasses INTEGER NOT NULL,
    PRIMARY KEY (TableId, Kind, SectionKey),
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE,
    CHECK (Kind IN ('Group', 'SubTable', 'ChildTable'))
);

-- Field groups of Items.Items.
-- identity, classification, inventory, flags, and robot correspond to pages that legacy never removed.
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'identity', -1 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'classification', -1 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'inventory', -1 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'flags', -1 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'robot', -1 FROM Tables WHERE XmlTable = 'Items.Items';

-- stance and combat mirror the NCTH and Bonuses pages: only weapon, ammo, armour, explosive, face,
-- kit, and misc classes keep them; keys, LBE, random items, and empty/money classes do not.
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'stance', 268484606 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'combat', 268484606 FROM Tables WHERE XmlTable = 'Items.Items';

-- vision and food come from the Abilities and Bonuses pages; random items keep the Abilities page.
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'vision', 1342226430 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'food', 1342226430 FROM Tables WHERE XmlTable = 'Items.Items';

-- attachments merges the Attachments and Attachment Data pages. It stays visible for ammo because
-- legacy kept the data page there, and for LBE because the attachment page was kept for MOLLE.
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'attachments', 268615678 FROM Tables WHERE XmlTable = 'Items.Items';

-- modifiers approximates the Bonuses and NCTH pages: hidden for keys, LBE, random items, and
-- empty/money. A few fields whose legacy pages were split differently remain in the nearest group;
-- the masks are supersets of the classes that legacy showed, never subsets.
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'modifiers', 268484606 FROM Tables WHERE XmlTable = 'Items.Items';

-- The Overheating page survives for every class except random items; it is split out of the mixed
-- modifiers group so keys and LBE keep their temperature values while losing the combat modifiers.
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'overheating', 805552127 FROM Tables WHERE XmlTable = 'Items.Items';

-- Pockets exist only for LBE; sBackpackWeightModifier is the only ITEM field of that page.
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'Group', 'pockets', 131072 FROM Tables WHERE XmlTable = 'Items.Items';

UPDATE ColumnHints
SET GroupKey = 'overheating', GroupOrder = 90
WHERE TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items')
  AND XmlFieldName IN (
      'usOverheatingCooldownFactor',
      'overheatTemperatureModificator',
      'overheatCooldownModificator',
      'overheatJamThresholdModificator',
      'overheatDamageThresholdModificator');

UPDATE ColumnHints
SET GroupKey = 'pockets', GroupOrder = 35
WHERE TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items')
  AND XmlFieldName = 'sBackpackWeightModifier';

-- 1:1 sub-table sections replace the Weapon, Ammo, Armour, Explosives, and Pockets pages.
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'SubTable', 'Items.Weapons', 254 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'SubTable', 'Items.Magazines', 1024 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'SubTable', 'Items.Armours', 2048 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'SubTable', 'Items.Explosives', 768 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'SubTable', 'Items.LoadBearingEquipment', 131072 FROM Tables WHERE XmlTable = 'Items.Items';

-- 1:N child sections replace the Attachments, Attachment Data, Explosives launcher, and Face pages.
-- Ammo keeps the attachment data page but legacy removed its Attachments page, so the available-
-- attachments child section is hidden for ammo while the attachment field group stays visible.
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'ChildTable', 'Items.Attachments', 268614654 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'ChildTable', 'Items.AttachmentInfo', 268484606 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'ChildTable', 'Items.IncompatibleAttachments', 268484606 FROM Tables WHERE XmlTable = 'Items.Items';
-- The current Launchables child relation lists the launchers of the selected item, which corresponds
-- to the LauncherGrid on the legacy Explosives page. The Launchers page's LaunchableGrid is a separate
-- direction that M23b2 adds; until then only Grenade and Bomb classes show this section.
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'ChildTable', 'Items.Launchables', 768 FROM Tables WHERE XmlTable = 'Items.Items';
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'ChildTable', 'Items.CompatibleFaceItems', 32768 FROM Tables WHERE XmlTable = 'Items.Items';
