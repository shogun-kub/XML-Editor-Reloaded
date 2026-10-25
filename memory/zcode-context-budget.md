---
name: zcode-context-budget
description: What ZCode loads into context every session, the measured effect of
  local tool search and file/plugin trims (29.4k→12.8k input tokens), and the
  remaining safe levers
metadata:
  node_type: memory
  type: project
  originSessionId: session_3fa5682a33cd32e7be05d85f6bc4d252
---

Measured 2026-10-25 for this workspace at session start (first model request, 29 416 input tokens before tool search; 14 968 after tool search; 12 791 after disabling three unused plugins and compacting `AGENTS.md`), at the user's request to see what loads every time and how to limit it.

**Always loaded files — only three:** global `~/.zcode/AGENTS.md` (588 B), workspace `AGENTS.md` (11 137 B, ~6 890 chars), and the memory index `MEMORY.md` (~3.3 KB). No nested `AGENTS.md`/`CLAUDE.md` exist; the per-platform cap for one instructions file is 100 KB. Not loaded: `PLAN.md`, `PROJECT-STRUCTURE.md`, `REFERENCE-PROJECTS-STRUCTURE.md` (77 KB), `LEGACY-PARITY-REPORT.md` (370 KB), `M23A-PARITY/`, `.zcode/plans/`. Only `MEMORY.md` is loaded from memory; the 26 memory files (~95 KB total) arrive only on recall.

**The real cost is generated content, not files:** before tool search, tool schemas were ~77 KB across 40 tools; after `local` in a new chat they are 17 144 chars across 9 tools (Agent, AskUserQuestion, Bash, Edit, Read, Skill, Write, LocalToolSearch, DeferredToolCall) plus a 543-char deferred-tools announcement. After disabling spreadsheets/browser-use/skill-creator: skill catalog 7 238 chars (18 skills; GitHub 10, zcode-guide 6, visualize 1, dynamic-workflows 1), agent list 997 (Explore + general-purpose). Base system prompt ~11 KB; workspace `AGENTS.md` 6 976 chars.

**Levers:**
- Disable unused plugins in `~/.zcode/cli/config.json` → `plugins.enabledPlugins` (or Settings → Plugin Management): spreadsheets + browser-use + skill-creator saved 2 177 input tokens together with the `AGENTS.md` trim (14 968→12 791); zcode-guide was kept (≈2.7 KB), github remains (≈3.6 KB). `node_repl` is deferred now too.
- `toolSearchMode` in `~/.zcode/v2/setting.json` (allowed `off|auto|local`) — the biggest lever. The user set it to `local` on 2026-10-25 and restarted, but **restarting is not enough**: the mode is baked into a session's runtime config at creation, so a *restored* conversation keeps `off` (verified: identical 40 tools / 77 208 schema chars, no `LocalToolSearch`). Only a **new chat** picks it up. Verified in a new chat on 2026-10-25 08:35: 9 resident tools, 17 144 schema chars, 14 968 input tokens (29 416 before, −49%). A second fresh chat at 08:44, after disabling spreadsheets/browser-use/skill-creator and compacting `AGENTS.md`, used 12 791 input tokens; skills 7 238 (18), agents 997 (2), workspace `AGENTS.md` 6 976, tools unchanged 17 144 (9). `local` works for any provider supporting tool calls (DeepSeek included); `native` requires an Anthropic Messages provider and errors otherwise. Compare `inputTokens` in `~/.zcode/cli/rollout/model-io-*.jsonl`; resumed and older sessions also append to the *same* file, so check newest lines only.
- `skills.metadataBudget` (default 20 000 chars) should stay above the current ~7.2 KB catalog: below it the catalog falls back to bare names/paths and skills stop triggering well.
- Global `~/.zcode/AGENTS.md` should not be merged into the project file (saves <0.6 KB, breaks portability).

**Why:** the user explicitly wants to keep per-session context small; knowing the split prevents optimizing the wrong layer — tool schemas dominated before tool search, instruction/meta dominates after it — and prevents expecting tool search to help a resumed chat.

**How to apply:** quote the rollout-log split rather than file sizes. With tool search on, the remaining overhead is instruction/meta: trim `AGENTS.md` and the memory index, and disable unused plugins (skills + duplicate agents); `skills.metadataBudget` stays above the catalog. This complements [[documentation-stays-compact]] and [[read-before-edit]] (compact global file) and [[ja2-editor-status]], whose milestone recap can be trimmed from the index.
