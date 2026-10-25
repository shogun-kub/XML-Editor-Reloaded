PRAGMA foreign_keys = ON;

-- M23b2: symmetric pair tables.
--
-- Legacy DuplicateEntryTable.VerifyData (References/xml-editor/BackEnd/Data Access/Tables/
-- DuplicateEntryTable.vb) kept a reversed duplicate for every row of these two tables before saving.
-- The rewrite maintains the pair during add, delete, and key edits, and validation reports existing
-- asymmetric data instead of rewriting it on load or save.
CREATE TABLE SymmetricKeyPairs (
    TableId INTEGER NOT NULL PRIMARY KEY,
    FirstField TEXT NOT NULL,
    SecondField TEXT NOT NULL,
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

INSERT INTO SymmetricKeyPairs (TableId, FirstField, SecondField)
SELECT Id, 'itemIndex', 'incompatibleattachmentIndex'
FROM Tables WHERE XmlTable = 'Items.IncompatibleAttachments';

INSERT INTO SymmetricKeyPairs (TableId, FirstField, SecondField)
SELECT Id, 'compatiblefaceitemIndex', 'itemIndex'
FROM Tables WHERE XmlTable = 'Items.CompatibleFaceItems';
