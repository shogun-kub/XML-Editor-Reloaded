---
name: ja2-editor-status
description: Current state of XML Editor Reloaded (M0–M24b done, F0–F9 fixed, UI smoke parts A–G automated, M25a next), approved M18–M27 roadmap and documentation policy.
metadata:
  node_type: memory
  type: project
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

**Current status (2026-11-03):** M0–M24b завершены. Gate M23b пройден: сборка, 618 тестов,
corpus v4/v5 600/600, compare 1/1, copy 1/1, perf 9/9, UI-smoke 90 сценариев; маркер
`M23: legacy functional parity gate` поставлен 2026-11-03 (ручной smoke: S04, S22, S53, S74 на v4;
S83 автотест; колонки v5 для S22/S53/S74 не перепроверялись по решению пользователя).
M24a добавил `NestedTableWorkspaceViewModel` для `Map.Cities`/`Map.FacilityTypes`, resolved nested
primary path, root singleton `COUNTRY`, `ASSIGNMENT` и overlay tooltip; UI-smoke S93 — PASS на v4.
M24b добавил структурные `ASSIGNMENT` (add/delete), повторяемые блоки `PRODUCTIONLINE`, списки
`drugitem`/`usOptional_ItemUsedUp` и nested-перевод `CITYLIST/CITY/townName`; S94 — PASS на v4,
S50 подтверждает только заявленные файлы. Далее M25a: сравнение v2.
F0–F9 закрыты 2026-10-30 (`MANUAL-SMOKE-FIX-PLAN.md`); выпадающие значения фильтров и массовой
правки — бэклог B1–B2, не дефекты. Детали раннера и UIA-грабли — [[ja2-editor-ui-smoke]], M24a —
[[m24a-nested-editors]], M24b — [[m24b-nested-collections]].

**Approved M18–M27 roadmap (2026-10-24, plan-only, commit `60d6695`):** full working legacy parity against
`References/xml-editor`; hidden, dead and lossy legacy behavior is excluded. M18 relations + main-table
pickers; M19 item master-detail = item-workflow parity; M20 merc gear, shopkeeper/auxiliary tables and
language catalog; M21 multi-dataset and copy/merge; M22 grid/workflow polish; M23 audit = full legacy
parity. After parity: M24 nested/list/dict editors and Cities/facilities (M24a, M24b done), M25 comparison v2 and
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
