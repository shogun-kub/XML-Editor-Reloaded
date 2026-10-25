---
name: commit-after-each-milestone
description: User rule — after every completed milestone create a local git commit
  prefixed with the milestone id; push only on request
metadata:
  node_type: memory
  type: feedback
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

The user asked (2026-10-24) to check for a "commit after every milestone" rule and add it if missing. There was none, so `AGENTS.md` was changed: the previous blanket "do not run `git commit` / `git push` without an explicit request" became "do not `git push` without an explicit request; a local commit after each milestone is mandatory".

Rule now in `AGENTS.md` (section «🔨 Сборка и проверка»): after every milestone (M0, M1, …) with a green `dotnet build` + `dotnet test`, create a local commit whose message starts with the milestone id, e.g. `M3: XML loading and round-trip`. Push only on explicit request. The same note was added to `PLAN.md`.

**Why:** The user works milestone-by-milestone with several agents and wants each step recoverable on its own; an uncommitted working tree makes handoff and rollback unreliable.

**How to apply:** At the end of each milestone: build → test → update `PLAN.md`/`PROJECT-STRUCTURE.md` status → `git add` the milestone's files → commit with the `Mx: …` subject. Never push without being asked. Follow-up fixes inside the same milestone get their own commit with a suffix (`M3.1: …`). See [[ja2-editor-status]].
