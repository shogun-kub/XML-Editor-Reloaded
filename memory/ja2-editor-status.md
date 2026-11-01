---
name: ja2-editor-status
description: Current state of XML Editor Reloaded (M0–M23b8 and M23a done, F0–F9 fixed, UI smoke parts A–G automated, M23b gate next), approved M18–M27 roadmap and documentation policy.
metadata:
  node_type: memory
  type: project
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

**Current status (2026-11-01):** M0–M23b8 и M23a завершены; следующий шаг — gate M23b (полный прогон,
обновление матрицы, маркер `M23: legacy functional parity gate`). 598 тестов зелёные (Core 62, Data 93,
Imaging 24, Xml 219, App 200), сборка 0/0 с `-warnaserror`, корпусные/compare/copy/perf-гейты проходят.
UI-smoke `XmlEditorReloaded.UiSmoke` автоматизирует части A–G чек-листа: S01–S03, S05–S21, S23–S49
и целостность S50; вручную остаются S04, S22, S71–S75, S82–S83 и ветка ошибки сохранения S13. F0–F9
закрыты 2026-10-30 (`MANUAL-SMOKE-FIX-PLAN.md`); выпадающие значения фильтров и массовой правки — бэклог
B1–B2, не дефекты. S41–S49 проверены прогоном 2026-11-01; S44 нашёл дефект языка диалогов, исправлен
явной культурой в `Strings`. Детали раннера и UIA-грабли — [[ja2-editor-ui-smoke]].

**Approved M18–M27 roadmap (2026-10-24, plan-only, commit `60d6695`):** full working legacy parity against
`References/xml-editor`; hidden, dead and lossy legacy behavior is excluded. M18 relations + main-table
pickers; M19 item master-detail = item-workflow parity; M20 merc gear, shopkeeper/auxiliary tables and
language catalog; M21 multi-dataset and copy/merge; M22 grid/workflow polish; M23 audit = full legacy
parity. After parity: M24 nested/list/dict editors and Cities/facilities, M25 comparison v2 and
dataset-wide validation, M26 lookup management, M27 packaging. Scope, acceptance criteria and risks —
`PLAN.md`.

**Documentation policy (2026-10-24, commit `d76e232`):** `PLAN.md` keeps current state, substeps, one-line
design decisions, the parity matrix, the remaining plan and verification commands. Completed milestones
get one short paragraph and no class names, counters or test inventories: code, tests and git history are
the source of truth. Keep the same style for new docs — condense instead of appending; do not repeat
rules that `AGENTS.md` already states.

**Maintenance:** update this file after each milestone; put technical detail into the matching topic file
and keep this summary to state, counts, next step and roadmap. Historical M17 backlogs, stale M14–M16
ordering notes and per-commit hash lists were dropped in the 2026-11-01 documentation compaction.
