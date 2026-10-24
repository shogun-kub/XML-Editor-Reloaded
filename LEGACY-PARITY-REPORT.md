# Legacy parity report (M23a)

**Status:** in progress — M23a0 inventories complete (UI, tables, UI verification, item field map); M23a1 shell/grid section complete; M23a2 item workspace UI section complete; M23a3 next.
**Reference implementation:** `References/xml-editor` (VB.NET/WinForms), read-only. `References/JA_2_1.13_TableData_Editor` is a prototype and is not a reference.
**Scope:** every user-visible and backend behaviour that is actually reachable in the legacy editor is either implemented in the rewrite, replaced by a safer equivalent, or explicitly excluded. Bug-for-bug compatibility is not required. M23a changes no product behaviour.
**Method and substeps:** `M23A-PARITY-AUDIT-PLAN.md`. Working evidence lives in `M23A-PARITY/`; this report is assembled from those sections.

## Verdicts

| Verdict | Meaning |
|---|---|
| `реализовано` | The behaviour is reachable in the rewrite and covers the substance of the legacy feature. Evidence is a reachable code path; tests only corroborate. |
| `superseded` | Replaced by a safer or more general equivalent; the difference is stated. |
| `исключено` | Dead/hidden feature, destructive behaviour, or an item explicitly outside parity in `PLAN.md`. Reason and evidence are required. |
| `пробел` | The feature is real and reachable in legacy but absent or incomplete in the rewrite. It goes to the gap register for M23b. |

## Evidence rules

- Every row has an ID from the inventory, a legacy citation (`References/xml-editor/...:line`), a rewrite citation (`xml-editor-reloaded/src/...:line` or `tests/...:line`) or `нет`, and a verdict.
- `реализовано` without a code citation is rejected. A test alone is not proof of a reachable path.
- `исключено` requires a cause: dead code, hidden control, destructive effect, or a documented M24–M27 deferral.
- Unverifiable claims go to the open-questions list of the section, never into a verdict.
- Repetitive legacy table-openers may share one row, but every legacy table must appear in `M23A-PARITY/00-inventory-tables.md`.

## Sections

| Section | Area | File | Status |
|---|---|---|---|
| 00 | UI command inventory | `M23A-PARITY/00-inventory-ui.md` | done |
| 00 | UI inventory verification | `M23A-PARITY/00-inventory-ui-verification.md` | done |
| 00 | Item form field and flag map | `M23A-PARITY/00-inventory-ui-item-fields.md` | done |
| 00 | Legacy table inventory and coverage | `M23A-PARITY/00-inventory-tables.md` | done |
| 10 | Shell and grid | `M23A-PARITY/10-shell-grid.md` | done |
| 20 | Item workspace UI | `M23A-PARITY/20-item-ui.md` | done |
| 21 | Item tables and images | `M23A-PARITY/21-item-data-images.md` | pending |
| 30 | Merc gear and auxiliary tables | `M23A-PARITY/30-merc-aux.md` | pending |
| 40 | Backend: storage | `M23A-PARITY/40-storage.md` | pending |
| 41 | Backend: operations | `M23A-PARITY/41-operations.md` | pending |

## Gap register

Assembled in M23a6 from the sections. Each gap: ID, area, severity (P1 blocks parity / P2 incomplete / P3 cosmetic), substance, expected layer, proposed M23b substep.

| ID | Area | Severity | Substance | Expected layer | M23b step |
|---|---|---|---|---|---|
| — | — | — | — | — | — |

## v4/v5 verification

Filled in M23a7: build/tests, corpus v4 and v5, compare v4↔v5, copy v4→v5, performance gates, M22c acceptance (item 12 shows 27 attachments; duplicate baseline unchanged), and the manual WPF smoke checklist result.

## Excluded by decision

The legacy exclusions from `PLAN.md` (hidden/dead menus, destructive behaviours, service/computed settings, and features deferred to M24–M27) are recorded as rows in the relevant sections with the cause, not repeated here.
