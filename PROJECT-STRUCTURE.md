# XML Editor Reloaded — Project Structure

Desktop editor for the XML table data (`TableData`) of the **Jagged Alliance 2 1.13** mod, with STI item
image previews. The stack is .NET 10, C#, WPF, SQLite and xUnit. Parsing, metadata and image decoding
live in backend class libraries; the WPF application is the only frontend.

## Repository layout

```
JA2_1.13 Tools/
├── AGENTS.md                        # Rules for AI assistants
├── PLAN.md                          # Roadmap and design notes
├── PROJECT-STRUCTURE.md             # This document
├── M14.5-METADATA-REPORT.md         # Legacy metadata verification report
├── REFERENCE-PROJECTS-STRUCTURE.md  # Catalogue of References/
├── References/                      # Read-only reference projects and game data (not in git)
└── xml-editor-reloaded/             # Solution root — all project code
```

- `References/` is read-only and never referenced by the solution; algorithms and samples are copied in.
- Other repository-root files are documentation and configuration only.

## Solution layout

```
xml-editor-reloaded/
├── XmlEditorReloaded.slnx
├── Directory.Build.props            # nullable, implicit usings, code-style enforcement, version
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

Shared contracts and domain primitives: table/column identifiers, metadata abstractions, localization
keys and translatable paths, lookup and validation primitives, image data contracts, application
settings contracts, structured row-filter contracts, per-table grid layout contracts, and the standard
`OperationResult` (`error_code`, `error_text`, `data`).

Folders: `Abstractions`, `Api`, `Filtering`, `Imaging`, `Layout`, `Localization`, `Metadata`,
`Validation`. No file I/O, no SQLite, no WPF.

### XmlEditorReloaded.Xml (`net10.0`)

XML-table engine:

- loading and metadata-driven parsing, including schema discovery for unmapped tables;
- lossless saving that preserves sparse records, element order, lexical values and file formatting;
- base table plus language overlays, merged display values, and overlay-only translation writes;
- table workspaces with editing, bulk operations, undo/redo, validation, relation pickers, and
  multi-file sets with a shared history and atomic per-file saving;
- cross-data-set copy and merge with preview, conflict policies, key allocation, display-text remapping,
  optional related rows, and rollback of already replaced files on failure;
- cross-version comparison and report formatting.

Folders: `Comparison`, `Copy`, `Documents`, `Filtering`, `IO`, `Loading`, `Lookups`, `Overlays`,
`Relations`, `Repository`, `Saving`, `Schema`, `Validation`, `Values`, `Workspace`.

### XmlEditorReloaded.Data (`net10.0`)

SQLite metadata, table catalog, lookup access, language catalog, settings persistence, and per-table
grid layouts. Embedded, versioned migrations (`Metadata/Migrations/0001`–`0008`) create and seed the
database automatically.

Folders: `Catalog`, `Localization`, `Metadata`, `Settings`.

### XmlEditorReloaded.Imaging (`net10.0`)

STI container parsing, ETRLE decompression, transparency handling, item image catalog
(big/medium/small) and a decoded-frame cache. Output is raw RGBA pixel data.

Folders: `Cache`, `Catalog`, `Etrle`, `Sti`.

### XmlEditorReloaded.App (`net10.0-windows`)

WPF shell and MVVM layer: data-directory selector across several data sets, table browser and tabs keyed
by data set and table, generic grid editor with structured filters, multi-row selection, per-table layout
persistence, item workspace, mercenary gear workspace, detail panel, STI image viewer, cross-version
comparison window, cross-data-set copy dialog, dialogs, settings, and RESX localization (English and
Russian).

Folders: `Imaging`, `Resources`, `Services`, `ViewModels`, `Views`. All UI strings come from RESX;
views contain no business logic.

### Test projects

Five xUnit projects mirror the source projects. `Xml.Tests` and `Imaging.Tests` contain small real
fixtures; corpus, comparison, copy and performance tests are environment-gated and never write to the
reference data (corpus tests work on temporary copies).

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

## Data formats

### TableData XML

- A table such as `Items.Items` maps to `TableData/Items/Items.xml`; dots become directories.
- Records are sparse: a record keeps exactly the elements it had. Unknown fields, repeated elements,
  order, casing, lexical number forms and the XML declaration are preserved on save.
- Loading and saving without edits does not rewrite the file. Malformed files are reported, never
  repaired or overwritten.
- Localized tables use `<Language>.<BaseFile>.xml` overlays in the same directory. An overlay stores
  the record key and only overridden fields; edits to translations never touch the base table.
  Version 4 is the Russian data set (overlays present); version 5 is the English base (no overlays).
- Table handling is metadata-driven and version-independent; new or unknown fields remain editable
  and are never dropped.

### Metadata database

Stored outside the game directory. `SchemaMigrations` tracks embedded SQL migrations; the schema covers
`Tables`, `TableKeys`, `Columns`, `ColumnHints`, `ListFields`, `DictFields`, `Languages`,
`TranslatableFields`, and `TableRelations`. `Columns` is declaring metadata (it can expose an absent
field); `ColumnHints` only refines fields found in the document. Relations model references, 1:1 and
1:N sub-tables, child rows, and reverse views.

### Grid layouts

Presentation state of each standalone grid tab is stored outside the game data in
`%AppData%\XmlEditorReloaded\layouts.json`. The versioned JSON file is keyed by the normalized
`TableData` directory and table name and stores column visibility, width and order, sorting, and the
structured filter. Writes are atomic and UTF-8 without BOM; an unreadable file is reported and backed up
as `layouts.json.bad` before a fresh file is written. Embedded grids and the item/gear workspaces keep
their layout in memory only.

### STI images

64-byte little-endian header, palette, per-frame directory, ETRLE-compressed indexed frames, and RGBA
output without `System.Drawing`. The catalog resolves big images (`BigItems`), medium images
(`Interface`) and small images (`tilesets/0`); the WPF layer converts RGBA to `BitmapSource`.

## Build and test

```sh
# Build the whole solution with warnings as errors
dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx -warnaserror

# Unit and integration tests
dotnet test xml-editor-reloaded/XmlEditorReloaded.slnx

# Tests against a real TableData directory (v4 or v5)
XER_CORPUS_DIR="<path>/Data-1.13/TableData" dotnet test xml-editor-reloaded/XmlEditorReloaded.slnx

# v4 <-> v5 comparison test
XER_COMPARE_LEFT_DIR=<v4 TableData> XER_COMPARE_RIGHT_DIR=<v5 TableData> \
  dotnet test xml-editor-reloaded/tests/XmlEditorReloaded.Data.Tests --filter RealDataComparisonTests

# v4 -> v5 copy test (source stays read-only)
XER_COPY_SOURCE_DIR=<v4 TableData> XER_COPY_TARGET_DIR=<v5 TableData> \
  dotnet test xml-editor-reloaded/tests/XmlEditorReloaded.Data.Tests --filter CorpusCopyTests

# Performance gates
XER_CORPUS_DIR=<TableData> dotnet test xml-editor-reloaded/XmlEditorReloaded.slnx \
  --filter "Category=Performance"
```

Environment-gated tests copy data to a temporary directory and never write to the source.

## Conventions

- Source and documentation files are UTF-8 without BOM.
- Code, identifiers and XML documentation are English; Russian is the communication language.
- Public backend APIs carry XML documentation.
- Long operations are asynchronous and accept a `CancellationToken`.
- `bin/`, `obj/`, IDE caches and `References/` are not committed. Push only on explicit request.
- Update this document whenever the solution structure or project responsibilities change.

## Documentation

- `AGENTS.md` — working rules and mandatory XML-integrity constraints.
- `PLAN.md` — roadmap, design decisions and verification commands.
- `M14.5-METADATA-REPORT.md` — how the legacy metadata seed was verified.
- `REFERENCE-PROJECTS-STRUCTURE.md` — contents of the read-only `References/` folder.

## Reference projects

| Reference | Role |
|---|---|
| `References/xml-editor` | Legacy editor and functional/UX parity benchmark. |
| `References/JA_2_1.13_TableData_Editor` | Prototype metadata schema and XML mapping. |
| `References/JA 2 1.13 Image viewer` | STI/ETRLE decoding algorithm and image layout. |
| `References/FreeImage` | Native FreeImage 3.18 fork, optional for non-STI raster formats. |
| `References/JA 2 1.13 v4` | Russian data set with overlays. |
| `References/JA 2 1.13 v5` | English base data set. |
| `References/Ja 2 1.13 source` | Engine source; read only with explicit permission. |
