PRAGMA foreign_keys = ON;

-- Curated, non-declaring hints for fields discovered in table documents. A hint refines a field
-- only when the document contains it; absent fields are not added to the schema.
CREATE TABLE ColumnHints (
    TableId INTEGER NOT NULL,
    XmlFieldName TEXT NOT NULL,
    DataType TEXT NULL,
    EditorKind TEXT NULL,
    DisplayWidth INTEGER NULL,
    LookupTable TEXT NULL,
    TooltipKey TEXT NULL,
    Flags INTEGER NULL,
    GroupKey TEXT NULL,
    GroupOrder INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (TableId, XmlFieldName),
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

CREATE INDEX IX_ColumnHints_TableId ON ColumnHints (TableId);
