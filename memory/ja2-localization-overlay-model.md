---
name: ja2-localization-overlay-model
description: v4 is the Russian build and v5 the English base by design; localization
  lives in per-file <Language>.<BaseFile>.xml overlays merged by record key
metadata:
  node_type: memory
  type: project
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

The user stated (2026-10-24) that v5 is the English (unlocalized) build and v4 is the Russian one — deliberate, not missing data — and asked for localization to be implemented inside the editor.

Measured in v4 `Data-1.13/TableData`: exactly 25 `Russian.*` overlay files, no other language prefixes. Localized data is **overlay files** named `<Language>.<BaseFile>.xml` next to the base file (e.g. `Items/Russian.Items.xml`, `Map/Russian.Cities.xml`). Each overlay carries the record key plus only overridden fields, and coverage is incomplete.

Record keys per file: `uiIndex` (most tables), `mIndex` (`Inventory/MercStartingGear`), `pIndex` (`Items/Pockets`, `Items/PocketPopups`), `ubIndex` (`Map/FacilityTypes`), `uiSlotIndex` (`Items/AttachmentSlots`), `SectorGrid` (`Map/SectorNames`). Nested structures need their own keys: `CITYLIST/CITY` by `uiIndex`, `FACILITYTYPE/ASSIGNMENT` by `ubAssignmentType` (`szTooltipText`), `GEARKIT` by position (some overlays have 4 kits where the base has 5).

Coverage gaps: `Russian.Items.xml` 1,742/1,776, `Russian.Backgrounds.xml` 316/357, `Russian.LoadScreenHints.xml` 133/159, plus smaller gaps in Disease, EnemyRank, History, OldAIMArchive, AmmoStrings, AttachmentSlots, Pockets, FacilityTypes.

Non-text structural fields appear in overlays (`Enabled`, `Sector`, `popup`, `generator`) and must be preserved. Overlay encoding is mixed: 4 of 25 have BOM, 16 CRLF, 9 LF. v5 `Email/EmailSenderNameList.xml` is a normal base table, not a localization file.

**Why:** Treating overlays as standalone tables, or writing translations into the base table, would duplicate data and break the game's language lookup; assuming full coverage leaves untranslated records silently showing English.

Implemented in the `Xml` project (M5, commit `6122dee`): `OverlayKeyResolver` + `OverlayMerger` + `OverlayWriter` + `TableEditSession` + `TranslationAnalyzer`, keyed by the `TableKeys`/`TranslatableFields` seed rows. Saving a translation leaves the base file byte-identical (verified by tests for Items, PocketPopups, MercGear, Cities).

**How to apply:** Overlays are overlays, not tables — never list them as separate tables. Merge base (schema + defaults) with overlay by key; mark missing translations; if a translation is edited, write only that field to the overlay file under the same XML-integrity rules ([[xml-integrity-is-mandatory]]). New overlays contain keys + translatable fields only, never a full copy. Report missing translations, orphan keys, duplicates, and empty values — never auto-fill. Localize the editor UI itself via RESX (English + Russian minimum). Details in `REFERENCE-PROJECTS-STRUCTURE.md` §6.5 and `PROJECT-STRUCTURE.md` §6.8; see [[ja2-v4-v5-data-facts]].
