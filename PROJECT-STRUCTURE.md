# XML Editor Reloaded — Project Structure

Desktop editor for the XML table data (`TableData`) of the Jagged Alliance 2 1.13 mod, with STI item
image previews. The stack is .NET 10, C#, WPF, SQLite and xUnit: backend functionality lives in class
libraries, and the WPF application is the only frontend.

## Repository layout

```
JA2_1.13 Tools/
├── AGENTS.md                        # Rules for AI assistants
├── PLAN.md                          # Roadmap and current status
├── PROJECT-STRUCTURE.md             # This document
├── M14.5-METADATA-REPORT.md         # Metadata verification report
├── REFERENCE-PROJECTS-STRUCTURE.md  # Catalogue of References/
├── References/                      # Read-only reference projects and game data (not in git)
└── xml-editor-reloaded/             # Solution root — all project code
```

`References/` is read-only and never referenced by the solution. Other repository-root files are
documentation and configuration only.

## Solution layout

```
xml-editor-reloaded/
├── XmlEditorReloaded.slnx
├── Directory.Build.props            # Nullable, implicit usings, code-style enforcement, version
├── .editorconfig
├── README.md
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
    └── XmlEditorReloaded.App.Tests/
```

## Projects

### XmlEditorReloaded.Core (`net10.0`)

Shared contracts and domain primitives: table and column identifiers, metadata and localization
abstractions, lookup and validation primitives, imaging contracts, settings, filter and grid-layout
contracts, and the standard `OperationResult` (`error_code`, `error_text`, `data`).

Folders: `Abstractions`, `Api`, `Filtering`, `Imaging`, `Layout`, `Localization`, `Metadata`,
`Validation`. No file I/O, SQLite, or WPF.

### XmlEditorReloaded.Xml (`net10.0`)

XML table engine: loading and lossless saving, schema discovery, workspaces and edit history, language
overlays, table relations, cross-data-set copy, and cross-version comparison.

Folders: `Comparison`, `Copy`, `Documents`, `Filtering`, `IO`, `Loading`, `Lookups`, `Overlays`,
`Relations`, `Repository`, `Saving`, `Schema`, `Validation`, `Values`, `Workspace`.

### XmlEditorReloaded.Data (`net10.0`)

SQLite metadata and catalogs: versioned migrations and seed data, table/lookup/language access, and
persistence of application settings and per-table grid layouts.

Folders: `Catalog`, `Localization`, `Metadata`, `Settings`.

### XmlEditorReloaded.Imaging (`net10.0`)

STI container parsing, ETRLE decompression, item image catalog (big/medium/small) and a decoded-frame
cache. Output is raw RGBA pixel data.

Folders: `Cache`, `Catalog`, `Etrle`, `Sti`.

### XmlEditorReloaded.App (`net10.0-windows`)

WPF shell and MVVM layer: data-set and table browser, grid and workspace editors, detail panel, dialogs,
image viewer, settings, and RESX localization (English and Russian).

Folders: `Imaging`, `Resources`, `Services`, `ViewModels`, `Views`. All UI strings come from RESX.

### Test projects

Five xUnit projects mirror the source projects. Fixtures contain small real samples; corpus and
performance tests are environment-gated and never write to reference data.

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
dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx -warnaserror
dotnet test xml-editor-reloaded/XmlEditorReloaded.slnx
```

Corpus, comparison, copy and performance runs are documented in `PLAN.md`.

## Conventions

- Source and documentation files are UTF-8 without BOM.
- Code, identifiers and XML documentation are English; Russian is the communication language.
- Public backend APIs carry XML documentation.
- Long operations are asynchronous and accept a `CancellationToken`.
- `bin/`, `obj/`, IDE caches and `References/` are not committed. Push only on explicit request.
- Update this document whenever the solution structure or project responsibilities change.

## Documentation

- `AGENTS.md` — working rules and mandatory XML-integrity constraints.
- `PLAN.md` — roadmap, current status and verification commands.
- `M14.5-METADATA-REPORT.md` — how the metadata seed was verified.
- `M22C-COMPOSITE-KEYS-PLAN.md` — implementation plan for compound record keys.
- `REFERENCE-PROJECTS-STRUCTURE.md` — contents and roles of the read-only `References/` folder.
