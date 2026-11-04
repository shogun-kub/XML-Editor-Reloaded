---
name: ja2-editor-status
description: Current state of XML Editor Reloaded (M0–M28 done, B1–B2 closed, F0–F9 fixed, 98 UI-smoke scenarios, portable 1.0.0 release), approved M18–M28 roadmap and documentation policy.
metadata:
  node_type: memory
  type: project
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

**Current status (2026-11-05):** M0–M28 завершены; бэклог B1–B2 закрыт. Gate M23b пройден: сборка, 726 тестов,
corpus v4/v5 600/600, compare 1/1, copy 1/1, perf 9/9, UI-smoke 98 сценариев; маркер
`M23: legacy functional parity gate` поставлен 2026-11-03 (ручной smoke: S04, S22, S53, S74 на v4;
S83 автотест; колонки v5 для S22/S53/S74 не перепроверялись по решению пользователя).
M24a добавил `NestedTableWorkspaceViewModel` для `Map.Cities`/`Map.FacilityTypes`, resolved nested
primary path, root singleton `COUNTRY`, `ASSIGNMENT` и overlay tooltip; UI-smoke S93 — PASS на v4.
M24b добавил структурные `ASSIGNMENT` (add/delete), повторяемые блоки `PRODUCTIONLINE`, списки
`drugitem`/`usOptional_ItemUsedUp` и nested-перевод `CITYLIST/CITY/townName`; S94 — PASS на v4,
S50 подтверждает только заявленные файлы. M25a добавил сравнение значений записей (ключ или порядок),
содержимое оверлеев, корневые singleton-поля и ограниченные детали с точными счётчиками; S40–S42
дополнены, S41 проверяет вкладки. M25b добавил окно датасета: одна проверка всех таблиц с точными
счётчиками и экспортом Markdown/CSV, глобальный поиск по значениям базы и оверлеев с переходом к
записи; S95 — PASS на v4. M26a добавил управление значениями справочников: свободный бит для битовой
маски, максимум + 1 для последовательного, свободный 64-битный ID с проверкой дубликатов и
предупреждением о комбинации; S96 — PASS на v4. M26b добавил «Где используется» с переходом к записи,
запрет удаления и смены ID используемого значения и предупреждение о битах маски без имени; S97 — PASS
на v4. M27a добавил 60 монохромных иконок, тёмную/светлую тему и `System` по умолчанию, общие стили
контролов, иконочные тулбары с тултипами и равной высотой, каталог горячих клавиш с подписями в меню,
явную навигацию `Home`/`End`/`PageUp`/`PageDown` в гридах; S98 — PASS на v4. M27b добавил вкладки
«Дерево»/«Список», иерархию реальных папок и файлов по `RelativePath`, фильтр по имени, Fold/Unfold
all и «Find in tree» с очисткой фильтра; S100 — PASS на v4, S98 перепроверен. M27c добавил
глобальный переключатель Grid/XML справа от вкладок, read-only снимок базовой таблицы через
`IEditorTab.CreateXmlSnapshot`, поиск записи по ID/ключу с выделением открывающего тега, переход из
замечания валидации к строке, копирование и обновление; в XML-режиме скрыт весь грид-хром, включая
специализированные item/merc/map-редакторы; S101–S104 — PASS на v4, S101 и S68 перепроверены после
поиска и перехода. M28 добавил версию `1.0.0`, иконку, self-contained publish `win-x64`, portable ZIP
с SHA-256, README/руководство/changelog, лицензию GLWTPL, portable launcher и CI/release workflow;
release-скрипт прогоняет тесты и собирает пакет. S81 против опубликованного exe — PASS, S50 без
изменений в `References/`. Полный 98-сценарный UI-smoke и ручная перепроверка v5 S22/S53/S74, а также
ветка ошибки сохранения S13 остаются отдельным release-gate.
F0–F9 закрыты 2026-10-30 (`MANUAL-SMOKE-FIX-PLAN.md`); выпадающие значения фильтров и массовой
правки (B1–B2) реализованы 2026-11-03 через `AllowedValueOption` и editable ComboBox. Детали
раннера и UIA-грабли — [[ja2-editor-ui-smoke]], M24a —
[[m24a-nested-editors]], M24b — [[m24b-nested-collections]], M25a — [[m25a-comparison-v2]],
M25b — [[m25b-dataset-report]], M26a — [[m26a-lookup-ids]], M26b — [[m26b-lookup-protection]],
M27c — [[m27c-xml-view]], M28 — [[m28-packaging]].

**Approved M18–M28 roadmap (2026-10-24, plan-only, commit `60d6695`):** full working legacy parity against
`References/xml-editor`; hidden, dead and lossy legacy behavior is excluded. M18 relations + main-table
pickers; M19 item master-detail = item-workflow parity; M20 merc gear, shopkeeper/auxiliary tables and
language catalog; M21 multi-dataset and copy/merge; M22 grid/workflow polish; M23 audit = full legacy
parity. After parity: M24 nested/list/dict editors and Cities/facilities (M24a, M24b done), M25 comparison v2
(M25a done) and dataset-wide validation/search (M25b done), M26 lookup management (M26a, M26b done),
M27a UI foundation (done), M27b table tree (done), M27c read-only XML with search/validation jump (done), M28 packaging (done). Scope, acceptance criteria
and risks — `PLAN.md`.

**Documentation policy (2026-10-24, commit `d76e232`):** `PLAN.md` keeps current state, substeps, one-line
design decisions, the parity matrix, the remaining plan and verification commands. Completed milestones
get one short paragraph and no class names, counters or test inventories: code, tests and git history are
the source of truth. Keep the same style for new docs — condense instead of appending; do not repeat
rules that `AGENTS.md` already states.

**Maintenance:** update this file after each milestone; put technical detail into the matching topic file
and keep this summary to state, counts, next step and roadmap. Historical M17 backlogs, stale M14–M16
ordering notes and per-commit hash lists were dropped in the 2026-11-01 documentation compaction.
