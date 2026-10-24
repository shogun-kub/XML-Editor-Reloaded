PRAGMA foreign_keys = ON;

-- Flag mask columns reuse the verified lookup files to provide bit labels in the flag editor.
-- Hints remain non-declaring: this migration only attaches a lookup source to fields that already
-- have a verified Flags hint, and it never changes the value type or the sparse document shape.
-- usActionItemFlag has no matching lookup file and stays unnamed.

UPDATE ColumnHints
SET LookupTable = 'Lookup.ItemClass'
WHERE XmlFieldName = 'usItemClass'
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints
SET LookupTable = 'Lookup.AttachmentClass'
WHERE XmlFieldName = 'AttachmentClass'
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints
SET LookupTable = 'Lookup.NasAttachmentClass'
WHERE XmlFieldName = 'nasAttachmentClass'
  AND TableId IN (SELECT Id FROM Tables WHERE XmlTable IN ('Items.Items', 'Items.AttachmentSlots'));

UPDATE ColumnHints
SET LookupTable = 'Lookup.NasLayoutClass'
WHERE XmlFieldName = 'nasLayoutClass'
  AND TableId IN (SELECT Id FROM Tables WHERE XmlTable IN ('Items.Items', 'Items.AttachmentSlots'));

UPDATE ColumnHints
SET LookupTable = 'Lookup.AttachmentPoint'
WHERE XmlFieldName IN ('AttachmentPoint', 'AvailableAttachmentPoint')
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints
SET LookupTable = 'Lookup.DrugType'
WHERE XmlFieldName = 'DrugType'
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Items');

UPDATE ColumnHints
SET LookupTable = 'Lookup.AmmoFlag'
WHERE XmlFieldName = 'ammoflag'
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.AmmoTypes');

UPDATE ColumnHints
SET LookupTable = 'Lookup.ItemClass'
WHERE XmlFieldName = 'pRestriction'
  AND TableId = (SELECT Id FROM Tables WHERE XmlTable = 'Items.Pockets');
