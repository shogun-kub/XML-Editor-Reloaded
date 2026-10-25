---
name: ja2-source-needs-permission
description: References/Ja 2 1.13 source (5.4 GB C++ engine) may only be explored
  after explicit user permission, per task
metadata:
  node_type: memory
  type: feedback
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

The user's rule (2026-10-24): `References/Ja 2 1.13 source` is the main 1.13 mod source (C++, ~5.4 GB, 123k files) and is a "last resort to deep dive exploration on how data are used". **Always ask before exploring it** — it is potentially very resource-heavy.

Permission was granted once, for the 2026-10-24 task of describing the project. That grant covers only that description and does not carry over to later work.

**Why:** Searches across 123k files are slow and expensive, and the user wants to control when that cost is paid.

**How to apply:** Before any search, read, or analysis inside that folder, request permission for the specific question; do not include it in broad greps over `References/`. Once permitted, target the XML loaders (`Tactical/XML_*.cpp`, `Strategic/XML_*.cpp`, `Laptop/XML_*.cpp`, `Ja2/XML_*.cpp`, `TileEngine/XML_*.cpp`), `Tactical/XML.h`, and `Utils/XML*.cpp` rather than the whole tree. This restriction is recorded in `AGENTS.md` section «Работа со справочными проектами». See [[references-folder-is-user-managed]].
