# Changelog

All notable changes to this project. Format based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
versions follow [Semantic Versioning](https://semver.org/).

**English** | [Русский](CHANGELOG.md)

## [1.1.1] — 2026-11-14

Patch release after 1.1.0 with small fixes and an English changelog.

### Added

- English `CHANGELOG.en.md`; GitHub release notes now use it, and both changelog files are included
  in the release packages.

### Fixed

- `LookupIdViewModel.TryAccept` now checks the `TryParseId` result before accepting the value.
- Explicit lambda parameter types in table comparison and copy previews reduce compiler type
  inference work (`CS9236`).

### Changed

- In the Russian help and UI, `attachment` is now translated as «навеска» (singular only);
  the `Attachments` table is labelled «Навеска», not «Навески».
- Remaining production SonarQube findings were triaged: `CA1806` and `CS9236` were fixed;
  `IDE0028`, `CA1822`, `CA1068`, and `CA1859` were consciously accepted with documented reasons.

### Verification

- Build: 0 errors, 0 warnings.
- 760 unit tests in five xUnit projects.
- SonarQube: 174 → 172 issues; remaining accepted/deferred findings are documented in `SONAR.md`.

## [1.1.0] — 2026-11-12

Contextual help for `TableData` fields and a complete Russian help edition.

### Added

- Built-in help for 239 tables and 2,985 field paths: 94 documents with descriptions, values,
  ranges, sources, and links to related tables.
- Modeless help window, `F1`, a **?** button in the details panel, and column-header tooltips.
- Search across tables, fields, and descriptions; navigation through related tables; opening a
  table from help in the main window.
- **Glossary**, **Index**, **Coverage**, **Formulas**, **Sources**, **Game options**,
  **Localization**, and **Email texts** pages.
- Complete Russian help under `help/ru` with the same paths, anchors, and fields; selection of
  `help.json`/`help.ru.json` by UI language with an English fallback.
- `help-check -Lang ru` for structural parity, translation status, and generated Russian
  index/coverage.

### Changed

- The `Launchables` string in the Russian UI follows the glossary: «Боеприпасы» (Ammunition).
- USER-GUIDE clarifies that the help language follows the UI language after restart.

### Fixed

- The **Glossary**/**Index** window now renders Markdown as a formatted document: headings,
  tables, lists, links, code, and emphasis instead of raw markup.
- Links in **Index** open help for the selected table, while page links (**Formulas** and others)
  open a separate document window. Context-help sections (`Tables`, `Structure`, `Relations`, and
  others) also render Markdown; relative links resolve from the current document, and internal
  anchors scroll the page.
- The mouse wheel in the contextual help window scrolls the shared panel even over nested Markdown
  sections; the scrollbar and keys already worked.

### Verification

- 760 unit tests in five xUnit projects.
- `help-check -Release` and `help-check -Lang ru -Release`: 94/94 documents, 239/239 tables,
  2,985/2,985 paths.
- `help.json` and `help.ru.json --check`: no discrepancies.
- UI smoke of the published build: S81 (About shows `1.1.0`) and S105 — PASS.
- UI smoke S105 (v4, ru): Russian tooltip, F1, search, link, table opening, glossary, index, and
  closing.

## [1.0.0] — 2026-11-05

First public portable release of the editor.

### Added

- Self-contained `win-x64` build: the .NET runtime is included, so no installation is required.
- Framework-dependent `win-x64` build: smaller, requires .NET 10 Desktop Runtime x64.
- Both archives contain the same README/USER-GUIDE/CHANGELOG/LICENSE, launcher, and a separate
  SHA-256 checksum.
- Portable archive with README, user guide, changelog, license, and third-party notices.
- `start-portable.cmd` for storing settings, layout, metadata database, and log next to the
  application.
- SHA-256 checksum for each archive.
- Application icon and version `1.0.0` in build metadata and the About window.
- CI build/test gates and a tag-driven `v*` release workflow.

### Editor features

- View and edit `TableData` v4 and v5 tables in a schema-driven grid.
- Table tree and list, search, tabs, and several data sets open at once.
- Item master-detail, merc gear, shopkeeper, auxiliary and nested editors (`Map.Cities`,
  `Map.FacilityTypes`).
- Lossless round-trip, saving only changed files, undo/redo, bulk edits, and filters.
- Translation overlays: translations are written only to the overlay; the base table is unchanged.
- Lookup and flag editors, free ID allocation, **Where used** with value protection.
- v4↔v5 structural and value comparison, row copying with remap.
- Whole-dataset validation report and global search across tables and overlays.
- STI image preview for Big/Medium/Small images with frame selection.
- Global Grid/XML: read-only snapshot of the base table, ID/key search, and navigation from a
  validation finding to the record.
- Dark, light, and system themes; Russian and English UI.

### Verification

- `dotnet build -warnaserror`: 0 errors, 0 warnings.
- 726 unit tests in five xUnit projects.
- Full UI smoke: 99/99 (98 automated scenarios + S50), 0 failures.
- Manual v5 gate: S04/S22/S53 — PASS, S74 — N/A (no separate grid), save-error branch S13 —
  PASS; baseline `TableData` unchanged.
- Self-contained and framework-dependent publish; contents of both ZIPs and checksums verified by
  the release script.
- Corpus runs v4/v5 — 726/726, compare — 1/1, copy — 1/1, perf — 9/9 on each corpus; they require
  local copies of `References/` and are not part of regular CI.

### Known limitations

- No automatic XML backup and no three-way merge.
- XML mode only reads the base table; overlays and child tables are not included in the snapshot.
- Builds are not signed with a commercial certificate; Windows SmartScreen may show a warning.
