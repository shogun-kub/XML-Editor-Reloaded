---
name: ja2-editor-xml-roundtrip
description: Lossless XML round-trip design for XML Editor Reloaded — sparse records, lexical values, byte patching, structural patching and editing API.
metadata:
  node_type: memory
  type: project
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

- **Round-trip model:** `XDocument` with `LoadOptions.PreserveWhitespace`; `TableRecord` is a facade over the real `XElement` (sparse records are never materialized into DTO rows); `LexicalValue` keeps raw text.
- **XmlWriter normalizes, so it cannot round-trip:** it lowercases the declaration encoding (`UTF-8` → `utf-8`), strips spaces around `=` in attributes, escapes `>` as `&gt;`, turns `&#10;` into a newline, rewrites `<Empty/>` as `<Empty />`, and drops the space in `<?xml … ?>`. 15 of 263 v4 files diverged byte-wise even with no edits. Solution: `LosslessDocumentPatcher` patches only the edited value ranges in the original bytes (segments discovered with `IXmlLineInfo`; reader position points at the name, not `<`, hence the search backwards to `<`).
- **Patcher baseline:** after each save the document's original bytes/tree are refreshed (`AcceptSaved` takes bytes, not a length), so a second edit and even a revert-to-original save still diff against the correct base.
- **R1 structural patching (commit `32954c3`):** `LosslessDocumentPatcher` diffs child sequences by signature (record key or name+occurrence), splices scalar edits, deletions (with preceding whitespace) and insertions (with neighbor indentation, pretty-printing new elements after stripping formatting whitespace from clones), verifies the patched tree against the in-memory model, and falls back to the serializer on ambiguity. Real v5 `Items.xml`: add + delete a record restores the original bytes exactly.
- **M4 editing API:** `TableRecord.SetFieldValue` / `SetListField` / `SetDictField` (mask `%Index%`) and `TableDocument.AddRecord` / `DuplicateRecord` / `DeleteRecord` (max+1 key, deep clone, snapshot on delete), plus `DocumentChangeTracker` journal for undo. Numeric validation comes from `ColumnMetadata` and returns `xml.invalid_number` without mutating the document; a missing field is only added with an explicit `addIfMissing`.
- **Fixtures:** real STI fixtures are copied into the test project; `.gitattributes` marks `tests/**/Fixtures/**` as `-text` (and the binary fixtures `-text -diff`) so BOM/CRLF/LF variants survive checkout.
- **How to apply:** scalar edits and single insert/delete/duplicate operations are lossless; ambiguous multi-element replacements fall back to the serializer (still data-correct, formatting normalizes). Never "just use XmlWriter" — it silently reintroduces formatting churn. See [[xml-integrity-is-mandatory]], [[ja2-editor-architecture]], [[ja2-editor-metadata]].
