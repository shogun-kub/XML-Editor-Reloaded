# XML Editor Reloaded — Project Structure

> **Status:** design stage. The `xml-editor-reloaded/` folder is currently empty; this document defines the intended solution layout and the scope of the application. It must be updated when the solution is scaffolded or its structure changes.

## 1. Overview

**XML Editor Reloaded** is a desktop application for viewing and editing XML table data (the `TableData` folder) of the **Jagged Alliance 2 1.13** mod for **Jagged Alliance 2**. It replaces the legacy VB.NET XML Editor with a modern .NET 10 / WPF application and adds item image previews decoded from the game's STI format.

Main goals:

- Open one or several JA2 1.13 data directories (`Data-1.13`, custom mods) and browse their XML tables.
- View and edit table rows in virtualized grids and detail forms.
- Show item images (big / medium / small) decoded from `.sti` files.
- Edit lookup tables, bit flags, and related records without hand-editing XML.
- Save data back to XML while preserving the structure the game expects.
- Keep parsing, metadata access, and image decoding in reusable backend libraries independent of WPF.

Non-goals (for the first iterations):

- Editing game executables or binary data other than STI images.
- Running the game or validating game balance.
- Multi-user or networked editing.

## 2. Repository layout

```
JA2_1.13 Tools/
├── AGENTS.md                        # Rules and context for AI assistants
├── PROJECT-STRUCTURE.md             # This document
├── REFERENCE-PROJECTS-STRUCTURE.md  # Catalogue of projects under References/
├── .gitignore                       # References/ and build artifacts are ignored
├── References/                      # Read-only reference projects (not in git)
└── xml-editor-reloaded/             # Solution root — all project code lives here
```

Rules:

- All projects that are created or modified live under `xml-editor-reloaded/`.
- `References/` is never modified during work sessions; it is used only as a source of algorithms, UX ideas, and test data.
- The repository root contains documentation and configuration only.

## 3. Planned solution layout

```
xml-editor-reloaded/
├── XmlEditorReloaded.sln
├── Directory.Build.props             # Shared build settings (nullable, warnings, version)
├── .editorconfig                     # C# formatting and analyzer rules
├── README.md                         # Short project overview and build instructions
├── src/
│   ├── XmlEditorReloaded.Core/       # Domain model, metadata abstractions, result types
│   ├── XmlEditorReloaded.Xml/        # XML table reading/writing and validation
│   ├── XmlEditorReloaded.Data/       # SQLite metadata, lookups, settings storage
│   ├── XmlEditorReloaded.Imaging/    # STI/ETRLE decoding and item image catalog
│   └── XmlEditorReloaded.App/        # WPF application (views, view models, services)
└── tests/
    ├── XmlEditorReloaded.Core.Tests/
    ├── XmlEditorReloaded.Xml.Tests/
    └── XmlEditorReloaded.Imaging.Tests/
```

Project names, folder names, and the exact split are preliminary. The hard constraints are:

1. The GUI is a WPF project (`net10.0-windows`).
2. Backend functionality is implemented in separate class-library DLLs targeting `net10.0`.
3. A solution file exists inside `xml-editor-reloaded/` and includes every project.

## 4. Project responsibilities

### XmlEditorReloaded.Core (`net10.0`)

- Domain primitives: table identifiers, column definitions, field kinds (scalar, list, dictionary, nested element), lookup references.
- Metadata abstractions shared by all libraries (`ITableMetadataProvider`, `ITableRepository`, `ILookupRepository`).
- Validation primitives and the standard operation result type with `error_code`, `error_text`, `data`.
- Application settings contracts (game directory, table directory, language overrides, recent data sets).
- No file I/O, no SQLite, no WPF.

### XmlEditorReloaded.Xml (`net10.0`)

- Reading `TableData/**/*.xml` into typed rows driven by metadata.
- Writing rows back to XML with preservation of element order, name casing, and encoding.
- Handling of repeated fields (lists) and indexed fields (dictionaries), as used by tables such as `Items.Items`, `Items.Drugs`, and `Items.Pockets`.
- Serialization of nested elements (e.g. weapon position modifiers) and unknown elements (kept for round-tripping).
- Validation against metadata and referral integrity rules.
- Depends on `Core`.

### XmlEditorReloaded.Data (`net10.0`)

- SQLite metadata repository. The prototype schema from `References/JA_2_1.13_TableData_Editor` is the starting point:
  - `TableMappings` — XML table path, DTO/model name, lookup flag, XML root, list item name.
  - `ListFields` — XML field name ↔ model property for repeated elements.
  - `DictFields` — XML field mask (e.g. `ItemCapacityPerSize%Index%`) ↔ model property.
- Lookup table access (`TableData/Lookup/*.xml`), including language-specific overrides.
- Persistence of application settings and recent directories (outside the game folder).
- Depends on `Core`.

### XmlEditorReloaded.Imaging (`net10.0`)

- STI container parser: `STCI` magic, 64-byte header, palette, sub-image directory, RGB and indexed variants.
- ETRLE decompression and conversion to RGBA pixel buffers.
- Item image catalog:
  - Big images: `BigItems\gun{NN}.sti`, `BigItems\p{type}item{NN}.sti`.
  - Medium images: `Interface\mdguns.sti`, `Interface\mdp{type}items.sti`.
  - Small images: `tilesets\0\smguns.sti`, `tilesets\0\smp{type}items.sti`.
- Caching of decoded frames and transparency handling (palette index 0 is transparent).
- Returns raw pixel data; no `System.Windows.Media` / WPF dependency.
- Depends on `Core`.

### XmlEditorReloaded.App (`net10.0-windows`)

- WPF application shell: main window, table browser, grids, detail forms, dialogs, settings.
- MVVM view models, commands, and UI services (dialogs, navigation, notifications).
- Conversion of backend pixel buffers to `BitmapSource` and image display.
- Dependency injection composition root and application configuration.
- Async loading with progress reporting and cancellation.
- Depends on all backend libraries.

### Test projects

- Unit tests for XML round-tripping, metadata mapping, validation, and STI decoding.
- Golden-file tests using small XML/STI samples (sample files must live in the test project, never in `References/`).
- Test framework and assertion library are chosen when the solution is scaffolded.

## 5. Dependency rules

```
XmlEditorReloaded.App  ──>  Xml, Data, Imaging, Core
XmlEditorReloaded.Xml  ──>  Core
XmlEditorReloaded.Data ──>  Core
XmlEditorReloaded.Imaging ──> Core
XmlEditorReloaded.Core ──>  (no project references)
```

- No backend library may reference WPF, `System.Windows.*`, or the App project.
- The App project must not contain XML parsing, metadata SQL, or STI decoding logic.
- Cross-library contracts live in `Core`; implementations stay in their own libraries.

## 6. Data formats and domain model

### 6.1 TableData XML

- Tables live in `<GameDir>\Data-1.13\TableData\` (or the `TableData` folder of another data directory).
- A table name such as `Items.Items` maps to `TableData\Items\Items.xml` (dots become directory separators).
- Each file has a root element (for example `ITEMLIST`) containing one element per record (for example `ITEM`).
- Records contain scalar elements, repeated elements, indexed element groups, and nested structures.
- Encoding and formatting of the original files must be preserved on save.
- The legacy editor also supports language-specific files (e.g. `German.Items.xml`) stored in separate game directories.

### 6.2 Metadata SQLite

The prototype database `References/JA_2_1.13_TableData_Editor/Metadata/table_metadata.sqlite` contains 53 table mappings:

| Table | Purpose | Columns |
|---|---|---|
| `TableMappings` | One row per XML table or lookup | `Id`, `xml_table`, `dto_name`, `is_lookup`, `xml_root`, `list_item_name` |
| `ListFields` | Repeated child elements | `id`, `table_id`, `xml_field_name`, `dto_field_name` |
| `DictFields` | Indexed child element groups | `id`, `table_id`, `xml_field_mask`, `dto_field_name` |

Example mappings: `Items.Items → DTO_Item / ITEMLIST / ITEM`, `Items.Weapons → DTO_Weapon / WEAPONLIST / WEAPON`, `Lookup.ItemClass → JA2Data / ItemClass`.

The production schema is expected to grow with column display metadata, lookup definitions, validation rules, and schema versioning.

### 6.3 STI images

- Container starts with the ASCII magic `STCI`.
- 64-byte main header: original/stored size, transparent value, flags, width, height, color count, sub-image count, channel depths, app data size.
- Palette follows the header: 3 bytes per color (BGR order in the legacy implementation).
- Sub-image directory: 16 bytes per frame (data offset, data length, X/Y offset, width, height).
- Indexed frames are ETRLE-compressed: a control byte with the high bit set means a run of transparent pixels, otherwise a run of palette indices.
- RGB frames are stored without a palette.
- The legacy `StiLib.dll` (2008, .NET Framework, x86) exposes `StciData`, `IndexedConverter`, `RGBConverter`, `ExtendedBitmap`, `ETRLEData`, and `ETRLEObject`. It cannot be referenced directly from .NET 10; its algorithms should be reimplemented in `XmlEditorReloaded.Imaging`.

### 6.4 Lookups and flags

- Lookup tables under `TableData\Lookup\*.xml` map numeric IDs to display names (`<id>`, `<name>` pairs).
- Bit-flag columns (item flags, ammo flags, attachment classes) are edited with a dedicated checkbox dialog rather than raw numbers.
- Lookups can be language-specific, similar to regular tables.

### 6.5 Legacy configuration and cache

The old editor used two auxiliary concepts worth preserving or replacing deliberately:

- `XMLEditorInit.xml` — data directories (`Data_Directory_1..10`), language-specific game directories, AP maximum, "hide nothing items" option.
- `XmlWorkingData\EditorData.xml` — a cache of the loaded data set that speeds up startup; it is invalidated when the editor version changes.

The new application should store user settings in a proper per-user location and make any cache explicit and disposable.

## 7. Planned feature set (parity with the legacy editor)

### Data sources and navigation

- Configure one or more game data directories; detect `TableData` automatically.
- Optional language-specific directories for localized table variants.
- Table list grouped by category (Items, Weapons, Inventory, Lookup, etc.).
- Open several tables in tabs/panes; remember layout between sessions.

### Grid editing

- Virtualized grid with sorting, filtering (by value, excluding value, custom expression), and column selection.
- Add, duplicate, and delete rows; automatic primary-key allocation.
- Bulk value operations: add, multiply, set for a selected column.
- Copy rows between data sets with overwrite rules.
- Item grid with inline image previews.

### Item and gear editors

- Item detail form with fields grouped by category (general, weapon, armour, flags, modifiers, food/drug, etc.).
- Item image preview (big, medium, small) with transparency.
- Attachment slots, default attachments, and compatible/incompatible attachment lists.
- Merc starting gear editor with several gear kits.
- Bit-flag editor, item class changer, new-item dialog, item ID lookup.

### Lookup tables

- Editable lookup grids with ID/name validation.
- Protection against deleting values referenced by other tables.

### Persistence

- Save individual tables or all modified tables.
- Detect external file changes and warn before overwriting.
- Preserve XML structure and encoding; round-trip unknown elements.
- Working-data cache for fast reloads.

### UX and infrastructure

- Asynchronous loading with progress and cancellation.
- Undo/redo for row-level edits where feasible.
- Clear validation messages tied to table, row, and column.
- Settings dialog for game paths, language directories, and display options.
- Crash-safe error reporting that does not discard unsaved changes silently.

## 8. Reference projects

| Reference | Role for the new project |
|---|---|
| `References/JA 2 1.13 Image viewer` | STI decoding algorithm, ETRLE handling, transparency rules, item image layout. |
| `References/xml-editor` | Functional and UX reference: table list, grid behaviors, item/gear forms, lookups, flags, filters, saving model. |
| `References/JA_2_1.13_TableData_Editor` | Metadata SQLite schema, DTO field names, XML-to-model mapping prototype. |

See `REFERENCE-PROJECTS-STRUCTURE.md` for complete trees and file-level notes.

## 9. Conventions

- **Language:** source code, identifiers, and XML documentation in English. UI strings may be localized later; Russian is the primary communication language for the team.
- **Encoding:** UTF-8 without BOM for all source and documentation files.
- **Documentation:** public APIs carry XML documentation; complex algorithms (STI, ETRLE, metadata mapping) carry explanatory comments.
- **Build:** `dotnet build xml-editor-reloaded/XmlEditorReloaded.sln` must succeed before a task is considered complete.
- **Git:** `References/`, `bin/`, `obj/`, and IDE caches are ignored. Commits and pushes only on explicit request.

## 10. Roadmap

1. Scaffold the solution, shared build props, and empty projects; verify the build.
2. Implement `Core` metadata abstractions and the standard result type.
3. Implement `Data` SQLite metadata repository; import the prototype schema.
4. Implement `Imaging` STI parser and ETRLE decoder with golden-file tests.
5. Implement `Xml` table loading and round-trip saving with metadata validation.
6. Build the WPF shell: settings, data directory selection, table list.
7. Add grid editing (sorting, filtering, row operations).
8. Add item detail form with images and flag editors.
9. Add merc gear editor and lookup management.
10. Add save/validation workflows, cache, and error reporting.
