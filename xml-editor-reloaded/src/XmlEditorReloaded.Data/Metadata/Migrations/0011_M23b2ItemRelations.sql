PRAGMA foreign_keys = ON;

-- M23b2: item relation sections and navigation.
--
-- The legacy item form showed two directions of the Launchables table and filtered
-- CompatibleFaceItems by the selected item. The rewrite seeded the CompatibleFaceItems child rows in
-- the opposite direction, and the reverse relations of tables that were already loaded as child rows
-- were dropped by TableSetWorkspace. This migration aligns the metadata with the legacy grids; the
-- workspace now loads a reverse view over an already loaded child workspace.
--
-- Legacy evidence:
--   References/xml-editor/GUI/Data Forms/ItemDataForm.vb:325-331 (SetupGrid calls)
--   References/xml-editor/GUI/Data Forms/ItemDataForm.vb:1196-1217 (row-header navigation)

-- CompatibleFaceItemGrid was filtered by itemIndex and showed compatiblefaceitemIndex values.
UPDATE TableRelations
SET TargetValueField = 'itemIndex',
    TargetDisplayField = 'compatiblefaceitemIndex'
WHERE Kind = 'ChildRows'
  AND SourceTableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items')
  AND TargetTableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.CompatibleFaceItems');

-- The reverse direction is the other half: rows whose compatiblefaceitemIndex points at the item.
UPDATE TableRelations
SET TargetValueField = 'compatiblefaceitemIndex',
    TargetDisplayField = 'itemIndex'
WHERE Kind = 'Reverse'
  AND SourceTableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items')
  AND TargetTableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.CompatibleFaceItems');

-- Reverse relations with a label key become editable item-workspace sections. Only the two legacy
-- grids are marked: AttachToGrid ("Attachable Items") and LaunchableGrid ("Ammunition").
UPDATE TableRelations
SET LabelKey = 'ItemSectionReverse.Items_Attachments'
WHERE Kind = 'Reverse'
  AND SourceTableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items')
  AND TargetTableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Attachments');

UPDATE TableRelations
SET LabelKey = 'ItemSectionReverse.Items_Launchables'
WHERE Kind = 'Reverse'
  AND SourceTableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items')
  AND TargetTableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Launchables');

-- Reverse sections use a direction-qualified key so both directions of one table can have their own
-- class rule. AttachToGrid lived on the Attachment Data page, which legacy removed for keys and LBE.
-- LaunchableGrid lived on the Launchers page, which only the Launcher class kept.
INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'ChildTable', 'Items.Attachments|attachmentIndex', 268484606
FROM Tables WHERE XmlTable = 'Items.Items';

INSERT INTO ClassVisibilityRules (TableId, Kind, SectionKey, VisibleClasses)
SELECT Id, 'ChildTable', 'Items.Launchables|itemIndex', 16
FROM Tables WHERE XmlTable = 'Items.Items';
