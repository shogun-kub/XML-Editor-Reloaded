PRAGMA foreign_keys = ON;

-- M26a: distinguish bit-mask lookups from sequential enumerations so a new value receives a free
-- bit instead of max(id) + 1. The ids of these lookups are flags: single bits name one flag and
-- combinations name a prepared set, for example AttachmentPoint 3 = Small Molle | Medium Molle.
--
-- Evidence (v4 and v5 data):
--   Lookup/AttachmentPoint.xml  0, 1, 2, 3, 4, 7, 8, 16
--   Lookup/AttachmentClass.xml  up to 33554432
--   Lookup/DrugType.xml         up to 1081324
--   Lookup/ItemClass.xml        up to 1073741824
--   Lookup/ItemFlag.xml         up to 8589934592 (needs the full unsigned 64-bit range)
--   Lookup/NasAttachmentClass.xml, Lookup/NasLayoutClass.xml
--   Lookup/AmmoFlag.xml         named 0-4, while Items/AmmoTypes.xml already uses 8 and 16
--
-- Lookup.ItemFlag has no column that points at it, so usage-based detection would miss it; the kind
-- is curated here next to the other verified metadata instead of being guessed from the ids at run
-- time. Sequential lookups such as Lookup.Cursor keep the default 0.
ALTER TABLE Tables ADD COLUMN IsFlagLookup INTEGER NOT NULL DEFAULT 0;

UPDATE Tables
SET IsFlagLookup = 1
WHERE XmlTable IN (
    'Lookup.AmmoFlag',
    'Lookup.AttachmentClass',
    'Lookup.AttachmentPoint',
    'Lookup.DrugType',
    'Lookup.ItemClass',
    'Lookup.ItemFlag',
    'Lookup.NasAttachmentClass',
    'Lookup.NasLayoutClass'
);
