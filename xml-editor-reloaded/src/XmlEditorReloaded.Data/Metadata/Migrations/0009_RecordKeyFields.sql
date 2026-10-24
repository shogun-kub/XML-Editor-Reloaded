PRAGMA foreign_keys = ON;

-- M22c: compound record keys.
--
-- Eight item tables have a natural key that consists of several direct-child fields. The prototype
-- metadata declared only one of those fields, so records that share the first component were treated
-- as duplicates and overlay/row operations picked the first record of a group. The key fields below
-- follow the legacy editor (DatabaseCode.vb) where it defined a composite primary key; the two
-- structure tables are absent from the legacy schema and use the fields that are unique in v4 and v5.

CREATE TABLE RecordKeyFields (
    TableId INTEGER NOT NULL,
    Ordinal INTEGER NOT NULL,
    XmlFieldName TEXT NOT NULL,
    PRIMARY KEY (TableId, Ordinal),
    UNIQUE (TableId, XmlFieldName),
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

CREATE INDEX IX_RecordKeyFields_TableId ON RecordKeyFields (TableId);

-- A compound key replaces the single-field key of these tables; keeping both would let single-key code
-- silently pick the first row of a group.
DELETE FROM TableKeys
WHERE TableId IN (
    SELECT Id FROM Tables WHERE XmlTable IN (
        'Items.Attachments',
        'Items.CompatibleFaceItems',
        'Items.IncompatibleAttachments',
        'Items.Item_Transformations',
        'Items.Launchables',
        'Items.Merges',
        'Items.StructureConstruct',
        'Items.StructureDeconstruct'));

WITH keyFields(XmlTable, Ordinal, XmlFieldName) AS (VALUES
    ('Items.Attachments', 0, 'attachmentIndex'),
    ('Items.Attachments', 1, 'itemIndex'),
    ('Items.CompatibleFaceItems', 0, 'compatiblefaceitemIndex'),
    ('Items.CompatibleFaceItems', 1, 'itemIndex'),
    ('Items.IncompatibleAttachments', 0, 'itemIndex'),
    ('Items.IncompatibleAttachments', 1, 'incompatibleattachmentIndex'),
    ('Items.Item_Transformations', 0, 'usItem'),
    ('Items.Item_Transformations', 1, 'usResult1'),
    ('Items.Item_Transformations', 2, 'usResult2'),
    ('Items.Item_Transformations', 3, 'usResult3'),
    ('Items.Item_Transformations', 4, 'usResult4'),
    ('Items.Item_Transformations', 5, 'usResult5'),
    ('Items.Item_Transformations', 6, 'usResult6'),
    ('Items.Item_Transformations', 7, 'usResult7'),
    ('Items.Item_Transformations', 8, 'usResult8'),
    ('Items.Item_Transformations', 9, 'usResult9'),
    ('Items.Item_Transformations', 10, 'usResult10'),
    ('Items.Launchables', 0, 'launchableIndex'),
    ('Items.Launchables', 1, 'itemIndex'),
    ('Items.Merges', 0, 'firstItemIndex'),
    ('Items.Merges', 1, 'secondItemIndex'),
    ('Items.Merges', 2, 'firstResultingItemIndex'),
    ('Items.Merges', 3, 'secondResultingItemIndex'),
    ('Items.StructureConstruct', 0, 'usCreationItem'),
    ('Items.StructureConstruct', 1, 'usItemStatusLoss'),
    ('Items.StructureDeconstruct', 0, 'usDeconstructItem'),
    ('Items.StructureDeconstruct', 1, 'usItemToCreate'),
    ('Items.StructureDeconstruct', 2, 'usCreatedItemStatus'))
INSERT INTO RecordKeyFields (TableId, Ordinal, XmlFieldName)
SELECT t.Id, f.Ordinal, f.XmlFieldName
FROM keyFields f
INNER JOIN Tables t ON t.XmlTable = f.XmlTable;
