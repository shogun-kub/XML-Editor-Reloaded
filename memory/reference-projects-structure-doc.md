---
name: reference-projects-structure-doc
description: REFERENCE-PROJECTS-STRUCTURE.md catalogues each reference project;
  extend it when new ones arrive
metadata:
  node_type: memory
  type: project
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

`REFERENCE-PROJECTS-STRUCTURE.md` (repo root) documents every project under `References/`: purpose, type/framework, dependencies, what to reuse, caveats, a directory tree, a summary table, a comparison table for editor implementations and one for data sets, and a "Notes for future reference projects" checklist.

As of 2026-10-24 it covers seven references:

| Reference | Kind | Notes |
|---|---|---|
| `JA 2 1.13 Image viewer` | C#/WinForms utility | STI decoding, `net10.0-windows7.0` |
| `xml-editor` | VB.NET legacy editor | full editor, `net9.0-windows7.0`, UX source |
| `JA_2_1.13_TableData_Editor` | C# prototype | SQLite metadata schema, `net5.0-windows` |
| `FreeImage` | native C/C++ fork 3.18.0 | 42 MB, PNG etc., C# wrapper is old .NET Framework |
| `Ja 2 1.13 source` | C++ engine + data | 5.4 GB, permission required (see [[ja2-source-needs-permission]]) |
| `JA 2 1.13 v4` | full game install | Russian build, 25 `Russian.*` overlays (see [[ja2-v4-v5-data-facts]], [[ja2-localization-overlay-model]]) |
| `JA 2 1.13 v5` | full game install | English base build, no overlays (intentional) |

**Why:** The user adds references incrementally and expects the doc to stay complete; the first version covered only three.

**How to apply:** For editor-sized references, add a section with the same fields and a full tree. For multi-gigabyte ones (FreeImage, engine source, game installs), trees are structural — directory names plus file counts, expanding only editor-relevant folders — never a literal listing of 100k+ files. Record version, file/record counts, XML tag-set differences, BOM/EOL characteristics, and malformed files for every data set. See [[references-folder-is-user-managed]].
