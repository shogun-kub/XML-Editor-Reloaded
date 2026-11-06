# XML Editor Reloaded — Project Structure

Desktop editor for the XML table data (`TableData`) of the Jagged Alliance 2 1.13 mod, with STI item
image previews. The stack is .NET 10, C#, WPF, SQLite and xUnit: backend functionality lives in class
libraries, and the WPF application is the only frontend.

## Repository layout

```
JA2_1.13 Tools/
├── AGENTS.md                        # Rules for AI assistants
├── UI-RULES.md                      # UI spacing rules and per-form checklist
├── PLAN.md                          # Roadmap and current status
├── PROJECT-STRUCTURE.md             # This document
├── M14.5-METADATA-REPORT.md         # Metadata verification report
├── M22C-COMPOSITE-KEYS-PLAN.md      # Compound record keys plan
├── M23A-PARITY-AUDIT-PLAN.md        # Parity audit plan, substeps and agent protocol
├── MANUAL-SMOKE-FIX-PLAN.md         # Fix plan for the S01–S14 manual smoke findings
├── LEGACY-PARITY-REPORT.md          # Legacy parity matrix (M23a deliverable)
├── M23A-PARITY/                     # Parity audit sections, inventories and the manual smoke checklist
├── REFERENCE-PROJECTS-STRUCTURE.md  # Catalogue of References/
├── MEMORY.md                        # Index of the project memory (imported agent notes)
├── memory/                          # Topic notes linked from MEMORY.md
├── .github/workflows/               # CI build/test and tag-driven release
├── util-scripts/                    # Build+test, release packaging and `.tmp` cleanup
├── References/                      # Read-only reference projects and game data (not in git)
├── out/                             # Build output, publish and release ZIPs (not in git)
└── xml-editor-reloaded/             # Solution root — project code and release documents
```

`References/` is read-only and never referenced by the solution. `util-scripts/` holds the scripts
that operate on the repository as a whole; it is not part of the solution. Other repository-root
files are documentation and configuration only.

## Solution layout

```
xml-editor-reloaded/
├── XmlEditorReloaded.slnx
├── Directory.Build.props            # Nullable, implicit usings, code-style enforcement, version
├── .editorconfig
├── README.md                         # Download, build, release and data-safety overview
├── USER-GUIDE.md                     # Russian end-user guide
├── CHANGELOG.md                      # Release history
├── LICENSE                           # GLWTPL
├── THIRD-PARTY-NOTICES.md            # Runtime/test dependencies and licenses
├── licenses/                         # Full MIT and Apache-2.0 texts
├── packaging/start-portable.cmd      # Portable launcher using XER_APP_DATA
├── src/
│   ├── XmlEditorReloaded.Core/
│   ├── XmlEditorReloaded.Xml/
│   ├── XmlEditorReloaded.Data/
│   ├── XmlEditorReloaded.Imaging/
│   └── XmlEditorReloaded.App/
└── tests/
    ├── XmlEditorReloaded.Core.Tests/
    ├── XmlEditorReloaded.Xml.Tests/
    ├── XmlEditorReloaded.Data.Tests/
    ├── XmlEditorReloaded.Imaging.Tests/
    ├── XmlEditorReloaded.App.Tests/
    └── XmlEditorReloaded.UiSmoke/      # Console UI-smoke runner (not a test project)
```

## Projects

### XmlEditorReloaded.Core (`net10.0`)

Shared contracts and domain primitives: table and column identifiers, metadata and localization
abstractions, lookup and validation primitives, imaging contracts, settings, filter and grid-layout
contracts, the standard `OperationResult` (`error_code`, `error_text`, `data`), and the process-wide
operation log facade.

Folders: `Abstractions`, `Api`, `Diagnostics`, `Filtering`, `Imaging`, `Layout`, `Localization`,
`Metadata`, `Validation`. No file writing, SQLite, or WPF; the diagnostics helper only reads file
metadata to explain access failures.

### XmlEditorReloaded.Xml (`net10.0`)

XML table engine: loading and lossless saving, schema discovery, workspaces and edit history, language
overlays, table relations, cross-data-set copy, and cross-version comparison.

Folders: `Comparison`, `Copy`, `Documents`, `Filtering`, `IO`, `Loading`, `Lookups`, `Overlays`,
`Relations`, `Repository`, `Saving`, `Schema`, `Validation`, `Values`, `Workspace`.

### XmlEditorReloaded.Data (`net10.0`)

SQLite metadata and catalogs: versioned migrations and seed data, table/lookup/language access,
persistence of application settings and per-table grid layouts, and the file-backed operation log.
`Settings/AppPaths` resolves the application-data directory (`%AppData%\XmlEditorReloaded`) for the
settings, layouts, metadata database and log, and honours the `XER_APP_DATA` environment variable,
which the UI smoke uses to isolate each scenario.

Folders: `Catalog`, `Diagnostics`, `Localization`, `Metadata`, `Settings`.

### XmlEditorReloaded.Imaging (`net10.0`)

STI container parsing, ETRLE decompression, item image catalog (big/medium/small) and a decoded-frame
cache. Output is raw RGBA pixel data.

Folders: `Cache`, `Catalog`, `Etrle`, `Sti`.

### XmlEditorReloaded.App (`net10.0-windows`)

WPF shell and MVVM layer: data-set and table browser, grid and workspace editors, detail panel, dialogs,
image viewer, settings, and RESX localization (English and Russian).

Folders: `Assets`, `Behaviors`, `Imaging`, `Resources`, `Services`, `Shortcuts`, `Themes`,
`ViewModels`, `Views`. All UI strings come from RESX. `Assets/app.ico` is both the executable icon
and the window icon resource.

### Test projects

Five xUnit projects mirror the source projects. Fixtures contain small real samples; corpus and
performance tests are environment-gated and never write to reference data.

### XmlEditorReloaded.UiSmoke (`net10.0-windows`, console)

UI-automation runner for the manual WPF smoke checklist. It starts the real application with an
isolated application-data directory, drives it through UI Automation and synthetic input, compares the
visible texts with the application RESX files, and writes `report.md`/`report.json`. The reference data
sets are copied per scenario, and `References/` is hashed before and after the run. It is not an xUnit
project and is not executed by `test.ps1`. It covers 98 automated scenarios from the checklist,
including S50 integrity and the M23b–M27c blocks; S04, S22, S53 and S74 are manual (v4 2026-11-03;
v5 S04/S22/S53 2026-11-06, S74 N/A). It accepts
`--app <exe>` to run the same scenarios against a published self-contained build.

## Dependency rules

```
XmlEditorReloaded.App  ──>  Xml, Data, Imaging, Core
XmlEditorReloaded.Xml  ──>  Core
XmlEditorReloaded.Data ──>  Core
XmlEditorReloaded.Imaging ──> Core
XmlEditorReloaded.Core ──>  (none)
```

- Backend projects must not reference WPF or `System.Windows.*`.
- Cross-library contracts live in `Core`; implementations stay in their own projects.
- XML parsing, metadata SQL, and STI decoding belong to the backend, not the WPF layer.

## Build and test

```sh
dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx -warnaserror -m:1 --no-restore
pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/test.ps1
```

`util-scripts/test.ps1` builds with one MSBuild node and runs the five xUnit projects sequentially;
it resolves the solution and `.tmp` from the repository root, so it works from any directory. Sandbox
restrictions and their workarounds are described in `AGENTS.md` («Песочница»).

The UI smoke runs on an interactive desktop and takes focus:
`out/bin/XmlEditorReloaded.UiSmoke/debug/XmlEditorReloaded.UiSmoke.exe [--only S12,S25] [--work .tmp/ui-smoke/run] [--keep] [--list]`.
Fixtures and `report.md` go to `.tmp/ui-smoke/<run>/`, and settings are isolated with `XER_APP_DATA`.

`Directory.Build.props` puts every build output under `out/`; run the application from
`out/bin/XmlEditorReloaded.App/<Configuration>/XmlEditorReloaded.App.exe`. Keep it there: a copy inside
the workspace starts at low integrity and cannot write `%APPDATA%` (`SQLite error 14`).

Corpus, comparison, copy and performance runs are documented in `PLAN.md`.

## Release

```sh
pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/release.ps1
```

The script validates the version in `Directory.Build.props`, runs the xUnit suite, publishes a
self-contained `win-x64` build, stages the release documents and the portable launcher, and writes the
portable ZIP plus SHA-256 to `out/release/`. `-SkipTests` is only for an already verified build.
`.github/workflows/release.yml` runs the same script for tags matching `v*` and publishes a GitHub
release when the repository has a remote.

## Conventions

- Source and documentation files are UTF-8 without BOM.
- Code, identifiers and XML documentation are English; Russian is the communication language.
- Public backend APIs carry XML documentation.
- Long operations are asynchronous and accept a `CancellationToken`.
- `bin/`, `obj/`, `out/`, IDE caches and `References/` are not committed. Push only on explicit request.
- Update this document whenever the solution structure or project responsibilities change.
