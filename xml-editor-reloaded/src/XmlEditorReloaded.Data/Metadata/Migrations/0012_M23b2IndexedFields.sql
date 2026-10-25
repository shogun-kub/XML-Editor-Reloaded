PRAGMA foreign_keys = ON;

-- M23b2: indexed (dict) fields are edited as their concrete expanded columns, for example
-- ItemCapacityPerSize3. The schema already detects the family through DictFields, but validation only
-- consults exact-name metadata, so the 35 pocket capacities had no type check. These hints do not
-- declare columns; they refine the fields that exist in a document.
WITH RECURSIVE sizes(n) AS (
    SELECT 0
    UNION ALL
    SELECT n + 1 FROM sizes WHERE n < 34
)
INSERT INTO ColumnHints (TableId, XmlFieldName, DataType, EditorKind, GroupKey, GroupOrder)
SELECT t.Id, 'ItemCapacityPerSize' || sizes.n, 'Integer', 'IntegerBox', 'lists', 200
FROM Tables t
CROSS JOIN sizes
WHERE t.XmlTable = 'Items.Pockets'
ON CONFLICT(TableId, XmlFieldName) DO UPDATE SET
    DataType = excluded.DataType,
    EditorKind = excluded.EditorKind,
    GroupKey = excluded.GroupKey,
    GroupOrder = excluded.GroupOrder;
