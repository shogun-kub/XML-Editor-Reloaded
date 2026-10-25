---
name: context-stays-small
description: User wants per-session context kept small — prefer terse instructions files and trimming redundant memory index entries over verbose additions
metadata:
  node_type: memory
  type: feedback
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

The user asked on 2026-10-25 which files load every session and how to limit their size and count, and earlier cut the global rule to one paragraph because it "bloated every session's context".

**Why:** every always-loaded file is paid on each turn; the user treats context as a budget, not a place to store detail that can be read on demand.

**How to apply:** keep `AGENTS.md` and memory entries short; do not duplicate detail that lives in `PLAN.md`, `PROJECT-STRUCTURE.md`, code or git; when adding a rule, check whether it fits one line in an existing file instead of a new section. The user also audits the store directly: on 2026-10-25 they asked to display all project memory (25 entries) in one message, so entries must stay individually legible — summary first, detail compressed, no cross-references that only make sense inside another entry. The 25 files total ~95 KB, of which `ja2-editor-parity` (11.5 KB), `ja2-editor-metadata` (9.5 KB) and `ja2-editor-grid-polish` (7.2 KB) are the main candidates for future trimming. Findings and measured numbers are in [[zcode-context-budget]]; the compact-docs rule is [[documentation-stays-compact]], which also says compaction must keep user-owned steps such as the manual smoke visible.
