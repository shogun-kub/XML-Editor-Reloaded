---
name: xml-integrity-is-mandatory
description: User rule — the editor must never break XML tables; no field removal,
  sparse records, lexical types preserved
metadata:
  node_type: memory
  type: feedback
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

The user's rule (2026-10-24): "the editor should not break the XML files" — it must NOT delete columns/fields inside XML tables and must preserve data types. Added to `AGENTS.md` as section «🛡 Целостность XML-данных» and to `PROJECT-STRUCTURE.md` as §6.7.

Concretely, for every read/write path:
- Never drop an element, including fields unknown to metadata/DTO and elements from other versions.
- Never normalize sparse records to a fixed column set (add missing or drop absent fields).
- Preserve lexical form and type: `1.0` stays `1.0`, `-0.2` stays `-0.2`, empties stay empty; format numbers with invariant culture.
- Preserve element order, casing, XML declaration, encoding, BOM, and per-file line endings; edit only user-changed values; never reserialize the whole document.
- Load → save without edits must reproduce the file; malformed files are reported, never auto-repaired or overwritten.

**Status (2026-10-24, M3):** implemented as `TableFileSaver` save-gate (clean document never written; idempotent output not written; length+mtime external-change check; atomic temp+replace) plus `LosslessDocumentPatcher`, which patches scalar text edits into the original bytes so declaration casing, attribute spacing, `&gt;`, `&#10;`, `<Empty/>` style, BOM, and EOL survive. Structural changes (add/delete/duplicate) still go through `XmlWriter` normalization — deferred to M4. Corpus harness (`XER_CORPUS_DIR`, runs on a temp copy) proves no-rewrite for all 263 v4 and 239 v5 files.

**Why:** The game reads these tables directly and record contents vary per row (see [[ja2-v4-v5-data-facts]]); a 1.0 → 1 or a dropped element silently changes game behavior, and the old editor's typed-DataSet approach is exactly what caused this risk.

**How to apply:** Treat it as a blocking constraint in design, code review, and tests. Verify with the corpus harness after any change to loading/saving; declare a malformed file non-saveable instead of repairing it. When metadata lacks a field, preserve it as an unknown node rather than discarding it. See [[ja2-engine-consumes-tables]] and [[ja2-editor-xml-roundtrip]].
