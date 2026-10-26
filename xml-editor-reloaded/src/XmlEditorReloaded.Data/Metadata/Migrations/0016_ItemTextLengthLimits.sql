PRAGMA foreign_keys = ON;

-- M23b7 / L-ITEM-017: input length limits and the legacy "remaining characters" counters of the
-- item detail form. The limits restrict typing only; an existing longer value stays visible and is
-- written back unchanged, so the counters never truncate data.
--
-- Legacy evidence:
--   References/xml-editor/GUI/Data Forms/ItemDataForm.vb:843-905
--       sixteen counters, value = TextBox.MaxLength - TextBox.TextLength
--   References/xml-editor/GUI/Data Forms/ItemDataForm.Designer.vb:1412, 1432, 1463, 1483, 1503
--       names 80, descriptions 400 (base page)
--   References/xml-editor/GUI/Data Forms/ItemDataForm.Designer.vb:2662-2763
--       Chinese page: names 80, szItemDesc 63, szBRDesc 90
--   References/xml-editor/BackEnd/DatabaseCode.vb:1280-1284, 1570-1574
--       the same 80/80/400/80/400 limits were declared as DataColumn.MaxLength
--
-- The Chinese limits existed only in the legacy designer and are therefore modelled as a
-- language override instead of a code branch. An empty LanguageCode is the neutral limit used by
-- the base table and by every language without a specific row.
CREATE TABLE ColumnTextLimits (
    TableId INTEGER NOT NULL,
    XmlFieldName TEXT NOT NULL,
    LanguageCode TEXT NOT NULL DEFAULT '',
    MaxLength INTEGER NOT NULL,
    ShowCounter INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (TableId, XmlFieldName, LanguageCode),
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

CREATE INDEX IX_ColumnTextLimits_TableId ON ColumnTextLimits (TableId);

WITH limits(XmlFieldName, MaxLength, ShowCounter) AS (VALUES
    ('szItemName',     80,  0),
    ('szLongItemName', 80,  0),
    ('szItemDesc',     400, 1),
    ('szBRName',       80,  0),
    ('szBRDesc',       400, 1))
INSERT INTO ColumnTextLimits (TableId, XmlFieldName, LanguageCode, MaxLength, ShowCounter)
SELECT t.Id, l.XmlFieldName, '', l.MaxLength, l.ShowCounter
FROM limits l
JOIN Tables t ON t.XmlTable = 'Items.Items';

WITH limits(XmlFieldName, MaxLength) AS (VALUES
    ('szItemDesc', 63),
    ('szBRDesc',   90))
INSERT INTO ColumnTextLimits (TableId, XmlFieldName, LanguageCode, MaxLength, ShowCounter)
SELECT t.Id, l.XmlFieldName, 'zh', l.MaxLength, 1
FROM limits l
JOIN Tables t ON t.XmlTable = 'Items.Items';
