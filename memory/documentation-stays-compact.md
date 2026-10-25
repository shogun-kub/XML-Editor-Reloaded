---
name: documentation-stays-compact
description: PLAN.md and PROJECT-STRUCTURE.md must stay compact; structure docs contain no implementation or milestone narratives, and compacting never drops the manual smoke step
metadata:
  node_type: memory
  type: feedback
  originSessionId: session_c682f856658cd9402e595ffb70c8a296
---

The user wants `PLAN.md` and `PROJECT-STRUCTURE.md` short and free of duplicated implementation detail, and asks for a compaction pass when completed milestones accumulate detail again.

**Why:** implementation is derivable from code, tests, and git history; verbose docs duplicate it and drift out of date. A separate session compacted PLAN.md only after it had grown to ~30 KB / 170 lines.

**How to apply:** Keep `PLAN.md` to current status, rules, key design decisions, one-line summaries of completed milestones (no class/file names, mechanism descriptions, closed-ID lists or per-test verification blocks), the remaining roadmap with gap IDs, parity, risks, and verification commands. Keep `PROJECT-STRUCTURE.md` to repository/solution layout, project responsibilities, dependency rules, build commands, conventions, and documentation pointers. Do not add class/file-level implementation narratives or milestone history to the structure file; link to [[ja2-editor-status]] for durable technical detail. **Compaction must preserve tasks that belong to the user — above all the manual WPF smoke (`M23A-PARITY/MANUAL-SMOKE-CHECKLIST.md`) — kept explicit in the status line, the audit milestone and the verification list.** Text of the last compaction: 30 KB/170 lines → ~17 KB/139 lines, committed as `7814e02 Docs: compact PLAN.md`.
