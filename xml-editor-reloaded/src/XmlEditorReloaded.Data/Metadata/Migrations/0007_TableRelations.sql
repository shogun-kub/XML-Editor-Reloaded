PRAGMA foreign_keys = ON;

-- Relations describe how tables reference each other. They replace the hardcoded main-table
-- pickers of the legacy editor (References/xml-editor/BackEnd/DatabaseCode.vb).
--
-- The seeded reference rows cover every main-table column whose source table exists in the seed:
-- ITEM (Items.Items), AMMO (Items.AmmoStrings), AMMOTYPE (Items.AmmoTypes), and the auxiliary
-- Food/Clothes/RandomItem tables. Legacy ITEMTOEXPLOSIVE was a virtual table built from Items rows
-- whose usItemClass is Grenade (256) or Bomb (512); it becomes a HasFlag filter over Items.Items.
-- Relations for shopkeeper, enemy/militia, IMP, and sound tables are added together with those
-- tables in M20, because TableRelations has foreign keys to Tables.
--
-- Filters are structured predicates. They only narrow picker options: values that already exist in
-- the document remain resolvable and are still validated against the unfiltered target. A single
-- predicate is stored per relation; the legacy "value OR uiIndex=0" expressions are represented by
-- their class predicate, and the None row stays selectable whenever the document already uses it.

CREATE TABLE TableRelations (
    Id INTEGER NOT NULL PRIMARY KEY,
    SourceTableId INTEGER NOT NULL,
    SourceField TEXT NOT NULL,
    TargetTableId INTEGER NOT NULL,
    TargetValueField TEXT NOT NULL,
    TargetDisplayField TEXT NULL,
    Kind TEXT NOT NULL DEFAULT 'Reference',
    FilterField TEXT NULL,
    FilterOperator TEXT NULL,
    FilterValue TEXT NULL,
    LabelKey TEXT NULL,
    RemapByText INTEGER NOT NULL DEFAULT 0,
    Ordinal INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (SourceTableId) REFERENCES Tables (Id) ON DELETE CASCADE,
    FOREIGN KEY (TargetTableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

CREATE INDEX IX_TableRelations_SourceTableId ON TableRelations (SourceTableId);
CREATE INDEX IX_TableRelations_TargetTableId ON TableRelations (TargetTableId);

CREATE UNIQUE INDEX UX_TableRelations_SourceField
    ON TableRelations (SourceTableId, SourceField, Kind, TargetTableId, TargetValueField);

-- Reference relations: the source field stores the target key.
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'firstItemIndex', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Merges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'secondItemIndex', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Merges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'firstResultingItemIndex', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Merges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'secondResultingItemIndex', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Merges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'ubCalibre', t.Id, 'uiIndex', 'AmmoCaliber', 'Reference', NULL, NULL, NULL, 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Magazines' AND t.XmlTable = 'Items.AmmoStrings';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'ubAmmoType', t.Id, 'uiIndex', 'name', 'Reference', NULL, NULL, NULL, 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Magazines' AND t.XmlTable = 'Items.AmmoTypes';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'launchableIndex', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'HasFlag', '768', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Launchables' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'itemIndex', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'HasFlag', '16', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Launchables' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'itemIndex', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.IncompatibleAttachments' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'incompatibleattachmentIndex', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.IncompatibleAttachments' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'ubFragType', t.Id, 'uiIndex', 'name', 'Reference', NULL, NULL, NULL, 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Explosives' AND t.XmlTable = 'Items.AmmoTypes';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'compatiblefaceitemIndex', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'HasFlag', '32768', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.CompatibleFaceItems' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'itemIndex', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'HasFlag', '32768', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.CompatibleFaceItems' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usItem', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentInfo' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usItem', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment1', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment2', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment3', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment4', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment5', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment6', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment7', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment8', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment9', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment10', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment11', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment12', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment13', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment14', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment15', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment16', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment17', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment18', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment19', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usAttachment20', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usResult', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AttachmentComboMerges' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'attachmentIndex', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Attachments' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'itemIndex', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Attachments' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'highExplosive', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'HasFlag', '768', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.AmmoTypes' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'DiscardedLauncherItem', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'HasFlag', '268435456', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'buddyitem', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'BloodiedItem', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'HasFlag', '8', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'DefaultAttachment', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'Attachment', 'Equals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'FoodType', t.Id, 'uiIndex', 'szName', 'Reference', NULL, NULL, NULL, 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Food';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'clothestype', t.Id, 'uiIndex', 'szName', 'Reference', NULL, NULL, NULL, 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Clothes';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'randomitem', t.Id, 'uiIndex', 'szName', 'Reference', NULL, NULL, NULL, 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.RandomItem';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usItem', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Item_Transformations' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usResult1', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Item_Transformations' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usResult2', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Item_Transformations' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usResult3', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Item_Transformations' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usResult4', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Item_Transformations' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usResult5', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Item_Transformations' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usResult6', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Item_Transformations' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usResult7', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Item_Transformations' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usResult8', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Item_Transformations' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usResult9', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Item_Transformations' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'usResult10', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Item_Transformations' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'item1', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.RandomItem' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'item2', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.RandomItem' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'item3', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.RandomItem' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'item4', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.RandomItem' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'item5', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.RandomItem' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'item6', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.RandomItem' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'item7', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.RandomItem' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'item8', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.RandomItem' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'item9', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.RandomItem' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'item10', t.Id, 'uiIndex', 'szLongItemName', 'Reference', 'usItemClass', 'NotEquals', '1', 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.RandomItem' AND t.XmlTable = 'Items.Items';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'ubCalibre', t.Id, 'uiIndex', 'AmmoCaliber', 'Reference', NULL, NULL, NULL, 1, 0
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Weapons' AND t.XmlTable = 'Items.AmmoStrings';

-- 1:1 sub-tables. Weapons match the item id; the other item sub-tables match ubClassIndex.
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'uiIndex', t.Id, 'uiIndex', NULL, 'SubTable', NULL, NULL, NULL, 0, 10
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Weapons';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'ubClassIndex', t.Id, 'uiIndex', NULL, 'SubTable', NULL, NULL, NULL, 0, 10
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Magazines';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'ubClassIndex', t.Id, 'uiIndex', NULL, 'SubTable', NULL, NULL, NULL, 0, 10
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Armours';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'ubClassIndex', t.Id, 'uiIndex', NULL, 'SubTable', NULL, NULL, NULL, 0, 10
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Explosives';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'ubClassIndex', t.Id, 'lbeIndex', NULL, 'SubTable', NULL, NULL, NULL, 0, 10
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.LoadBearingEquipment';

-- 1:N child rows shown for one item. The target value field stores the parent key.
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'uiIndex', t.Id, 'itemIndex', 'attachmentIndex', 'ChildRows', NULL, NULL, NULL, 0, 20
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Attachments';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'uiIndex', t.Id, 'usItem', 'uiIndex', 'ChildRows', NULL, NULL, NULL, 0, 20
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.AttachmentInfo';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'uiIndex', t.Id, 'itemIndex', 'incompatibleattachmentIndex', 'ChildRows', NULL, NULL, NULL, 0, 20
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.IncompatibleAttachments';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'uiIndex', t.Id, 'launchableIndex', 'itemIndex', 'ChildRows', NULL, NULL, NULL, 0, 20
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Launchables';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'uiIndex', t.Id, 'compatiblefaceitemIndex', 'itemIndex', 'ChildRows', NULL, NULL, NULL, 0, 20
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.CompatibleFaceItems';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'uiIndex', t.Id, 'usItem', 'szMenuRowText', 'ChildRows', NULL, NULL, NULL, 0, 20
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Item_Transformations';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'uiIndex', t.Id, 'usItem', 'usResult', 'ChildRows', NULL, NULL, NULL, 0, 20
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.AttachmentComboMerges';

-- Incoming references: records of the target table that point at one item.
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'uiIndex', t.Id, 'attachmentIndex', 'itemIndex', 'Reverse', NULL, NULL, NULL, 0, 30
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Attachments';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'uiIndex', t.Id, 'incompatibleattachmentIndex', 'itemIndex', 'Reverse', NULL, NULL, NULL, 0, 30
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.IncompatibleAttachments';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'uiIndex', t.Id, 'itemIndex', 'launchableIndex', 'Reverse', NULL, NULL, NULL, 0, 30
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Launchables';
INSERT INTO TableRelations (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField, Kind, FilterField, FilterOperator, FilterValue, RemapByText, Ordinal)
SELECT s.Id, 'uiIndex', t.Id, 'itemIndex', 'compatiblefaceitemIndex', 'Reverse', NULL, NULL, NULL, 0, 30
FROM Tables s, Tables t WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.CompatibleFaceItems';
