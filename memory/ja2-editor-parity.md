---
name: ja2-editor-parity
description: Legacy parity audit results (M23a0–M23a7) and M23b1–M23b8 fixes — counts, closed gap IDs, deliberate differences and the next gate.
metadata:
  node_type: memory
  type: project
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

- **M23a audit (2026-10-24, commits `ef97af7`…`5d2bd23`):** legacy has 122 tables / 1021 relations; the
  rewrite has 173 tables (87 mapped, 19 absent shopkeeper `CONTROL` tables). Sub-steps M23a0–M23a7 went
  area by area (`M23A-PARITY-AUDIT-PLAN.md`, sections in `M23A-PARITY/`); the consolidated result is
  `LEGACY-PARITY-REPORT.md`: 440 matrix rows / 588 IDs (115 `реализовано`, 329 `superseded`, 83
  `исключено`, 61 `пробел`), every inventory ID appears once, every row has `file:line` evidence.
- **Gap register:** 39 entries / 88 IDs (P1 0, P2 55, P3 33) grouped into M23b1–M23b8; each sub-step
  closed its IDs and added tests, a report section and smoke scenarios. Details, deliberate differences
  and residuals live in the report and in `M23A-PARITY/52…56-*-implementation-log.md`; they are not
  duplicated here.
- **M23b1–M23b8 closed (2026-10-24…27):** item filters/class-aware fields, relations/navigation/list-dict
  editors, value sources and pickers, validation limits and 64-bit masks, images, shopkeeper/lookup
  identity, shell/field UI/merc polish, storage/copy/logging. Baseline before the gate: 600 tests,
  corpus v4/v5, compare v4↔v5 and copy v4→v5 1/1, perf 9/9, `References/` untouched.
- **Known residuals:** big-image corner-colour transparency and nested stance limits are out of scope;
  empty lookup names have no duplicate/usage protection (M26a/M26b); `CONTROL` tables are excluded on
  purpose. **Gate M23b (2026-11-01):** automated gates green — build, 600 tests, corpus v4/v5
  (600/600), compare 1/1, copy 1/1, perf 9/9 both versions, 45 sandbox UI scenarios in two runs
  (head 22/22, tail 25/25; mandatory S50 in both; S59–S70 and S76–S80 added 2026-11-02); see
  `LEGACY-PARITY-REPORT.md`, «Проверка M23b (gate)». Marker
  `M23: legacy functional parity gate` is pending the user-signed manual smoke (S04, S22, S53, S55, S57,
  S71–S75, S82–S83, S81, S84–S92). See [[ja2-editor-status]],
  [[ja2-editor-grid-polish]], [[ja2-editor-item-merc]], [[ja2-editor-metadata]].
