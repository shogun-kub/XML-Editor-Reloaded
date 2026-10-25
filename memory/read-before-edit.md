---
name: read-before-edit
description: Global rule — read a file before editing it, and the rule covers only Edit, not Write; keep it one short paragraph in ~/.zcode/AGENTS.md
metadata:
  node_type: memory
  type: feedback
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

The user repeatedly hit "File has not been read yet. Read it first before writing to it." errors and asked (2026-10-25) to record the rule **globally**: always read a file before editing it.

Where it lives: `~/.zcode/AGENTS.md` (created 2026-10-25, UTF-8 no BOM). It is user-scope, so it loads before every workspace `AGENTS.md` and applies in all projects. The rule is intentionally **not** duplicated in the project `AGENTS.md`, to keep that file compact (see [[documentation-stays-compact]]).

**Scope: `Edit` only.** When asked to compact the rule, my first draft also required a `Read` before overwriting an existing file with `Write`; the user corrected that (2026-10-25): "you don't have to read a file before rewriting it with write. I meant only Edit." Do not reintroduce the `Write` clause.

Current wording (588 bytes, one paragraph): the rule applies only to `Edit` — read the target file with `Read` first; on "File has not been read yet" do `Read` and retry with the exact text instead of guessing. It replaced a six-bullet ~1.9 KB version after the user said the description was too long.

**Why:** The failure mode is guessing line content instead of loading the file first; the user wants the instruction short enough that it does not bloat every session's context, and scoped to the tool that actually errors.

**How to apply:** Read the whole file when it is small and will be edited in several places (one read, then parallel edits); never batch an `Edit` on a file not yet read in this session. Keep future edits to this rule terse — one paragraph, no expanded bullet lists, no `Write` requirement. See [[ja2-editor-architecture]] for project-level rules that complement the global file.
