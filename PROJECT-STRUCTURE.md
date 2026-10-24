# XML Editor Reloaded — Project Structure

> **Status:** M0/M1/M3/M4 complete. `XmlEditorReloaded.slnx` contains the 5 source projects and 4 test projects; Core primitives and the XML layer (loader, document model, validated editing, record operations, save gate, lookup repository, lossless scalar patch writer) are implemented and tested. `Data` and `Imaging` are still class-library stubs, and the WPF shell is a template. This document defines the intended layout and must be updated when the structure changes.

## 1. Overview

**XML Editor Reloaded** is a desktop application for viewing and editing XML table data (the `TableData` folder) of the **Jagged Alliance 2 1.13** mod for **Jagged Alliance 2**. It replaces the legacy VB.NET XML Editor with a modern .NET 10 / WPF application and adds item image previews decoded from the game's STI format.

Main goals:

- Open one or several JA2 1.13 data directories (`Data-1.13`, custom mods) and browse their XML tables.
- Support the two reference data sets, v4 and v5, and remain compatible with other versions whose tables add or remove fields.
- Edit the English base tables and the language overlay files (v4 is Russian-localized, v5 is the English base), and localize the editor UI.
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
├── References/                      # Read-only reference projects and game data (not in git)
└── xml-editor-reloaded/             # Solution root — all project code lives here
```

Rules:

- All projects that are created or modified live under `xml-editor-reloaded/`.
- `References/` is never modified during work sessions; it is used only as a source of algorithms, UX ideas, and test data.
- The repository root contains documentation and configuration only.

## 3. Planned solution layout

```
xml-editor-reloaded/
├── XmlEditorReloaded.slnx             # Solution (XML-based .slnx format)
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
    ├── XmlEditorReloaded.Data.Tests/
    ├── XmlEditorReloaded.Xml.Tests/
    └── XmlEditorReloaded.Imaging.Tests/
```

Project names, folder names, and the exact split are preliminary. The hard constraints are:

1. The GUI is a WPF project (`net10.0-windows`).
2. Backend functionality is implemented in separate class-library DLLs targeting `net10.0`.
3. A solution file exists inside `xml-editor-reloaded/` and includes every project.
4. Projects must not reference files under `References/`; algorithms, metadata, and sample data are copied into the solution.

## 4. Project responsibilities

### XmlEditorReloaded.Core (`net10.0`)

- Domain primitives: table identifiers, column definitions, field kinds (scalar, list, dictionary, nested element), lookup references.
- Localization primitives: language code, overlay descriptor, record key definition, translatable field paths.
- Metadata abstractions shared by all libraries (`ITableMetadataProvider`, `ITableRepository`, `ILookupRepository`, `ITranslationCatalog`).
- Validation primitives and the standard operation result type with `error_code`, `error_text`, `data`.
- Application settings contracts (game directory, table directory, language overrides, recent data sets).
- No file I/O, no SQLite, no WPF.

### XmlEditorReloaded.Xml (`net10.0`)

- Reading `TableData/**/*.xml` into typed rows driven by metadata.
- Writing rows back to XML with preservation of element order, name casing, and encoding.
- Handling of repeated fields (lists) and indexed fields (dictionaries), as used by tables such as `Items.Items`, `Items.Drugs`, and `Items.Pockets`.
- Serialization of nested elements (e.g. weapon position modifiers) and unknown elements (kept for round-tripping).
- Localized overlay support: reading `<Language>.<BaseFile>.xml` files, merging them with the base table by record key, and writing only the edited translation fields back to the overlay.
- Validation against metadata and referral integrity rules.
- Depends on `Core`.

### XmlEditorReloaded.Data (`net10.0`)

- SQLite metadata repository. The prototype schema from `References/JA_2_1.13_TableData_Editor` is the starting point:
  - `TableMappings` — XML table path, DTO/model name, lookup flag, XML root, list item name.
  - `ListFields` — XML field name ↔ model property for repeated elements.
  - `DictFields` — XML field mask (e.g. `ItemCapacityPerSize%Index%`) ↔ model property.
- Lookup table access (`TableData/Lookup/*.xml`), including language-specific overrides.
- Language catalog: discovery of `<Language>.<BaseFile>.xml` overlays in each data directory, available languages per table, and metadata describing translation keys and translatable paths.
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
- PNG and other raster formats used by the game (`Interface/*.png`) are decoded by the WPF layer or by an optional native FreeImage 3.18 wrapper; STI always uses the custom decoder.
- Returns raw pixel data; no `System.Windows.Media` / WPF dependency.
- Depends on `Core`.

### XmlEditorReloaded.App (`net10.0-windows`)

- WPF application shell: main window, table browser, grids, detail forms, dialogs, settings.
- MVVM view models, commands, and UI services (dialogs, navigation, notifications).
- UI localization from RESX resources (English and Russian initially), with a language switch in settings.
- Translation workflow: language selector, merged base/translation display, untranslated markers, and a missing-translation report.
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
- Records are sparse. In the reference data, `Items.xml` has about 1,776 records but 640 distinct field sets: most records contain only a small subset of the union of fields. A record must keep exactly the set of elements it had.
- Encoding and formatting of the original files must be preserved on save. The reference data mixes UTF-8 with and without BOM, CRLF and LF line endings, and one file is malformed; see 6.6 and 6.7.
- The legacy editor also supports language-specific files (e.g. `Russian.Items.xml`) stored in separate game directories.

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
- The game also ships PNG assets (for example backgrounds and posters in `Data-1.13\Interface`). PNG is not an STI variant; it can be decoded by WPF or by FreeImage.
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

### 6.6 Reference data sets (v4 and v5)

Two complete game installations are available under `References/`:

| Property | v4 | v5 |
|---|---:|---:|
| Install size | 3.7 GB | 3.6 GB |
| TableData files | 264 (263 XML + 1 readme) | 240 (239 XML + 1 readme) |
| TableData size | 12 MB | 10 MB |
| Lookup tables | 23 | 23 |
| Localized files | 25 `Russian.*` overlay files | none — English base data set |
| BigItems images | 1,568 `.STI` (includes `GUN375.sti`) | 1,567 |
| Interface assets | 25 files | 25 files |
| Small item images (`tilesets/0`) | 4 `.STI` | 4 `.STI` |

Comparison of the 239 common XML files:

- 71 files are byte-identical; 168 differ.
- All 239 common files have identical XML tag sets, so the column schema is the same.
- Record-count differences are limited to `Items/Items.xml` (1,776 → 1,775), `Items/Weapons.xml` (389 → 388), and `Items/Attachments.xml` (6,079 → 6,023).
- v4 contains 25 `Russian.*` localized files; v5 does not. v5 adds `Email/EmailSenderNameList.xml`.
- v4 has 162 files with UTF-8 BOM; 91 files use CRLF and 172 use LF. v5 has 159 files with BOM and all 239 files use CRLF.
- v5 `Lookup/AttachmentPoint.xml` is malformed: it contains an extra `</AttachmentPoint>` closing tag, so a strict XML parser rejects it. v4's copy is valid.

The data bundled with `References/Ja 2 1.13 source` (`gamedir/Base/TableData`, 277 files) is a third data set and shows that schemas differ across versions: 17 files shared with v5 have different tag sets (for example, v5 adds `szNWSSSound`, `lbeCombo`, and `GrowthModifier*`; the source data has `Detonator`/`RemoteDetonator` and `bEvolution`). The editor must therefore be metadata-driven and version-independent.

### 6.7 XML integrity requirements

The editor must never break a table file. These requirements are mandatory for every read/write path:

1. **No field removal.** Every element present in the source must be written back, including fields unknown to the metadata, empty elements, and elements from newer versions.
2. **No schema normalization.** Do not expand a sparse record to the union of all fields, and do not add default elements that were absent.
3. **Lexical types preserved.** Keep the original text representation: `1.0` stays `1.0`, `-0.2` stays `-0.2`, empty stays empty. Parse and format numbers with invariant culture.
4. **Structure preserved.** Element order, repeated elements, name casing, attributes, XML declaration, and comments (where feasible) survive a round-trip.
5. **File format preserved.** Keep the file's encoding, BOM presence, and line endings (CRLF or LF) as found; do not rewrite untouched files.
6. **Minimal writes.** Only the value the user edited is changed. Unknown nodes are retained in a side model such as a raw XML fragment.
7. **Round-trip test.** Load → save with no edits must produce an identical file (byte-for-byte when possible; otherwise identical after normalizing only the editor's own formatting, with BOM/EOL verified separately).
8. **No silent repair.** A malformed file is reported to the user; the editor does not "fix" or overwrite it automatically.
9. **Version independence.** Field handling comes from metadata plus the document itself, never from hardcoded v4/v5 lists.

### 6.8 Localization

v4 is the Russian-localized data set; v5 is the English base data set with no overlays. This is intentional.

**File naming.** Localized files are overlays named `<Language>.<BaseFile>.xml` in the same directory as the base file:

```
Items/Items.xml                 base
Items/Russian.Items.xml         Russian overlay
Map/Cities.xml                  base
Map/Russian.Cities.xml          Russian overlay
```

The legacy editor supports German, Russian, Polish, Italian, French, Chinese, Taiwanese, and Dutch. The language list should be metadata-driven so new languages can be added without code changes.

**Overlay model.** An overlay contains a record key plus only the fields it overrides. It is not a complete table:

| Overlay | Key | Translated fields | Coverage |
|---|---|---|---|
| `Items/Russian.Items.xml` | `uiIndex` | `szItemName`, `szLongItemName`, `szItemDesc`, `szBRName`, `szBRDesc` | 1,742 / 1,776 |
| `Items/Russian.AmmoStrings.xml` | `uiIndex` | `AmmoCaliber`, `BRCaliber` | 50 / 51 |
| `Items/Russian.AttachmentSlots.xml` | `uiSlotIndex` | `szSlotName` | 62 / 63 |
| `Items/Russian.Pockets.xml` | `pIndex` | `pName` | 61 / 62 |
| `Items/Russian.PocketPopups.xml` | `pIndex` | `pName`; `popup`/`generator` are structural and must be preserved | 20 / 20 |
| `Inventory/Russian.MercStartingGear.xml` | `mIndex` | `mName`; `GEARKIT/mGearKitName` | 255 / 255 |
| `Russian.Backgrounds.xml` | `uiIndex` | `szName`, `szShortName`, `szDescription` | 316 / 357 |
| `Russian.Disease.xml` | `uiIndex` | `szName`, `szFatName`, `szDescription` | 18 / 20 |
| `Russian.EnemyRank.xml` | `uiIndex` | `szRank` | 9 / 10 |
| `Russian.History.xml` | `uiIndex` | `sHistory` | 80 / 81 |
| `Russian.LoadScreenHints.xml` | `uiIndex` | `szName`, `lore` | 133 / 159 |
| `Russian.OldAIMArchive.xml` | `uiIndex` | `NickName`, `Name`, `Bio` | 25 / 28 |
| `Map/Russian.FacilityTypes.xml` | `ubIndex` | `szFacilityName`, `szFacilityShortName`; `ASSIGNMENT` by `ubAssignmentType` → `szTooltipText` | 22 / 25 |
| `Map/Russian.Cities.xml` | `COUNTRY` singleton; `CITYLIST/CITY` by `uiIndex` | `countryName`, `countryNoun`, `townName` | nested |

The remaining overlays (`CampaignStatsEvents`, `CivGroupNames`, `DifficultySettings`, `EnemyNames`, `MercProfiles`, `Vehicles`, both Email tables, `Minerals`, `SectorNames`, `ShippingDestinations`) cover all base records.

**Merge rules.**

- The base table provides the schema and defaults; the overlay overrides fields by key.
- A missing overlay record or field shows the base value with an "untranslated" marker.
- Nested repeated elements are merged by their own key: `CITY/uiIndex`, `ASSIGNMENT/ubAssignmentType`, `GEARKIT` by position (validate counts: some overlays have four kits where the base has five).
- Overlay fields that are not text (`Enabled`, `Sector`, `popup`, `generator`) are preserved as-is.

**Editing rules.**

- Editing a translated field writes only to the overlay; the base file is untouched.
- Editing a base-only field while a translation language is selected either edits the base with a warning or is disabled, depending on the chosen mode.
- New translations are added as key + translated fields, never as a full copy of the base table.
- Overlay files follow the same encoding/BOM/EOL preservation rules as base tables (v4 overlays: 4 BOM, 16 CRLF, 9 LF).

**Validation and reports.**

- Missing translations: base keys absent from the overlay.
- Orphan translations: overlay keys absent from the base.
- Duplicate keys, empty translations, and values identical to the base.
- No automatic translation or auto-fill.

**Editor UI localization.**

- All UI strings live in RESX resources; no hardcoded strings in views or view models.
- English and Russian initially; the language list should be extensible.
- Language switch in settings, applied at runtime where feasible.

## 7. Planned feature set (parity with the legacy editor)

### Data sources and navigation

- Configure one or more game data directories; detect `TableData` automatically.
- Optional language-specific directories for localized table variants.
- Discover language overlays (`<Language>.<BaseFile>.xml`) per table; choose a display/edit language.
- Open v4, v5, and future versions side by side; report files and fields that differ between data sets.
- Table list grouped by category (Items, Weapons, Inventory, Lookup, etc.).
- Open several tables in tabs/panes; remember layout between sessions.
- Load malformed files in a diagnostic mode that reports the problem without rewriting the file.

### Localization

- Show base English values with translated overlay values; mark untranslated fields and records.
- Edit translations in place; write only to the overlay file.
- Compare base and translation side by side.
- Report missing translations, orphan overlay keys, duplicate keys, and empty values.
- Create a new language overlay containing keys and translatable fields only.
- Localize the editor UI itself (English/Russian initially), with a language selector in settings.

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
- Preserve XML structure and encoding; round-trip unknown elements, sparse records, BOM, and line endings.
- Load → save without edits must reproduce the original file; this is verified by automated tests.
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
| `References/FreeImage` | Native FreeImage 3.18 fork for PNG and other raster formats; optional dependency if WPF decoding is not enough. |
| `References/Ja 2 1.13 source` | Last-resort reference for how the engine interprets each field. Explore only with explicit user permission. |
| `References/JA 2 1.13 v4` | Primary data set: 1,776 items, Russian localization overlays, mixed BOM/line endings. |
| `References/JA 2 1.13 v5` | Second data set: 1,775 items, English base with no overlays, CRLF, one malformed lookup file. |

See `REFERENCE-PROJECTS-STRUCTURE.md` for detailed structures and file-level notes.

## 9. Conventions

- **Language:** source code, identifiers, and XML documentation in English. UI strings may be localized later; Russian is the primary communication language for the team.
- **Encoding:** UTF-8 without BOM for all source and documentation files.
- **Documentation:** public APIs carry XML documentation; complex algorithms (STI, ETRLE, metadata mapping) carry explanatory comments.
- **Build:** `dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx` must succeed before a task is considered complete.
- **Git:** `References/`, `bin/`, `obj/`, and IDE caches are ignored. Commits and pushes only on explicit request.

## 10. Roadmap

1. Scaffold the solution, shared build props, and empty projects; verify the build.
2. Implement `Core` metadata abstractions and the standard result type.
3. Implement `Data` SQLite metadata repository; import the prototype schema.
4. Implement `Imaging` STI parser and ETRLE decoder with golden-file tests.
5. Implement `Xml` table loading and round-trip saving with metadata validation; add byte-level round-trip tests against copies of v4/v5 `TableData`.
6. Build the WPF shell: settings, data directory selection, table list.
7. Add grid editing (sorting, filtering, row operations).
8. Add item detail form with images and flag editors.
9. Add localization support: overlay discovery, merged display, translation editing, validation reports, and RESX-based UI localization.
10. Add merc gear editor and lookup management.
11. Add save/validation workflows, cache, error reporting, and a cross-version field report.
