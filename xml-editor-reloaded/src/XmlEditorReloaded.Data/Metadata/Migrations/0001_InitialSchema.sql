PRAGMA foreign_keys = ON;

CREATE TABLE Tables (
    Id INTEGER NOT NULL PRIMARY KEY,
    XmlTable TEXT NOT NULL UNIQUE,
    DtoName TEXT NULL,
    IsLookup INTEGER NOT NULL DEFAULT 0,
    XmlRoot TEXT NOT NULL,
    RecordElement TEXT NOT NULL
);

CREATE TABLE TableKeys (
    TableId INTEGER NOT NULL,
    SegmentIndex INTEGER NOT NULL,
    Path TEXT NOT NULL,
    Kind TEXT NOT NULL,
    KeyField TEXT NULL,
    IsPrimary INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (TableId, SegmentIndex),
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

CREATE INDEX IX_TableKeys_TableId ON TableKeys (TableId);

CREATE TABLE Columns (
    TableId INTEGER NOT NULL,
    XmlFieldName TEXT NOT NULL,
    DataType TEXT NOT NULL,
    EditorKind TEXT NOT NULL DEFAULT 'TextBox',
    DisplayWidth INTEGER NULL,
    LookupTable TEXT NULL,
    TooltipKey TEXT NULL,
    Flags INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (TableId, XmlFieldName),
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

CREATE TABLE ListFields (
    TableId INTEGER NOT NULL,
    XmlFieldName TEXT NOT NULL,
    DisplayNameKey TEXT NULL,
    PRIMARY KEY (TableId, XmlFieldName),
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

CREATE TABLE DictFields (
    TableId INTEGER NOT NULL,
    XmlFieldMask TEXT NOT NULL,
    DisplayNameKey TEXT NULL,
    PRIMARY KEY (TableId, XmlFieldMask),
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

CREATE TABLE Languages (
    Code TEXT NOT NULL PRIMARY KEY,
    DisplayName TEXT NOT NULL,
    OverlayPrefix TEXT NOT NULL,
    Ordinal INTEGER NOT NULL
);

CREATE TABLE TranslatableFields (
    TableId INTEGER NOT NULL,
    FieldPath TEXT NOT NULL,
    PRIMARY KEY (TableId, FieldPath),
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

CREATE INDEX IX_TranslatableFields_TableId ON TranslatableFields (TableId);
