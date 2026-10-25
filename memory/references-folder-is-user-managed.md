---
name: references-folder-is-user-managed
description: References/ holds read-only reference projects and game installs
  that only the user updates; the engine source needs explicit permission
metadata:
  node_type: memory
  type: project
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

`References/` in the JA2 1.13 Tools repo contains reference material — source projects, a native library fork, and two complete ~3.7 GB game installs — that must never be modified during work sessions; the user updates or adds to them and describes each addition. The folder is excluded from git by the root `.gitignore` and currently totals over 12 GB.

Read/analyze freely: Image viewer, xml-editor, TableData_Editor, FreeImage, v4, v5. **Not without explicit permission:** `Ja 2 1.13 source` (see [[ja2-source-needs-permission]]).

**Why:** Edits to the user's snapshots would be lost or corrupt the baseline, and game installs are the reference data the editor is judged against.

**How to apply:** Treat `References/` as read-only; never reference it from projects under `xml-editor-reloaded/` — copy algorithms, metadata, or sample data instead. For write tests, work on a copy, never the originals. Record new references in [[reference-projects-structure-doc]].
