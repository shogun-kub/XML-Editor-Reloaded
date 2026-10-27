PRAGMA foreign_keys = ON;

-- M23b8 / L-OPS-011: a copied lookup row whose target id is absent but whose display name already
-- exists must update or skip the existing row instead of adding a second row with the same name.
--
-- Legacy evidence:
--   References/xml-editor/BackEnd/Data Access/Tables/DefaultTable.vb:394-474
--       CopyRows looks the target row up by primary key and, on a miss, calls AlternateRowSearch
--       (:463-474), which matches TableProperty.ComparisonField and then overwrites
--       (allowOverwrite) or skips that row instead of appending a duplicate.
--   References/xml-editor/BackEnd/DatabaseCode.vb:840, 857, 872, 886
--       every lookup factory sets TableProperty.ComparisonField = "name".
--
-- The comparison field is carried by ColumnFlags.ComparisonField, whose value is the base-two bit
-- 1 << 2 = 4. Only lookup tables receive the hint; on conflict the bit is OR-ed into the existing
-- Flags, so a curated data type, editor, group, tooltip, and numeric bounds stay untouched.
INSERT INTO ColumnHints (TableId, XmlFieldName, DataType, EditorKind, Flags)
SELECT Id, 'name', 'String', 'TextBox', 4
FROM Tables
WHERE IsLookup = 1
ON CONFLICT(TableId, XmlFieldName) DO UPDATE SET
    Flags = COALESCE(ColumnHints.Flags, 0) | 4;
