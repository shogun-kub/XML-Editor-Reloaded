# XML Editor Reloaded — Project Structure

> **Status:** M0–M19 complete. `XmlEditorReloaded.slnx` contains the 5 source projects and 5 test projects; the backend is fully implemented and wired through DI. The WPF shell can select a data directory, list tables, show mapped/unmapped and overlay languages, inspect table details, persist settings, open a standalone STI image viewer, and open tables in editable tabs. M10 added performance gates, cancellation contracts, culture-independent numerics, and shell robustness. M11 added schema discovery, the `TableWorkspace` (base + overlay, merged rows, add/duplicate/delete, unified save, validation, translation reports), runtime data-set sessions, and lossless structural patches. M12 added the virtualized schema-driven grid: tabs, sparse cell editing, row operations, search, sorting, a column chooser, dirty tracking, save/save-all, reload, and a validation panel. M13 added a per-row detail panel with curated non-declaring column hints, field grouping and filtering, three STI preview panels, and a 32-bit flag editor. M14 added per-tab language selection, merged translation display with untranslated markers, a translation report window, and creation of new overlay files. M14.5 verified the legacy schema against v4 and v5, seeded 502 non-declaring hints (224 for `Items.Items`) and 46 declaring lookup columns, enabled hint-driven validation, and added 64-bit lookup identifiers. Lookup/Boolean/flag editors (M15) added lookup combos, Boolean checkboxes, and named bit labels with graceful fallback when a lookup file is malformed. M16 added bulk set/add/multiply with a preview-and-confirm dialog and a bounded undo/redo history covering cell, row, bulk, and translation edits (Ctrl+Z/Ctrl+Y). M17 added a read-only cross-version comparison of two `TableData` directories with table/field/type/lookup/sparse-profile differences and Markdown/CSV export; it never writes to either data set. M18 added the `TableRelations` metadata model with 84 verified links, the cached `TableRelationResolver` (display fields, structured filters, language overlays, name-based remapping), searchable relation pickers in the grid and detail panel, target-record navigation, and relation-aware validation. M19 added the item master-detail workspace: `IEditorTab` for heterogeneous tabs, `TableSetWorkspace` with one shared undo history across Items and its 1:1/1:N tables, nested and repeated-field writes, the item workflow (new, duplicate with dependents, relation-aware delete, ID rename/swap), class-aware sub-table sections, localized edits that stay in overlays, item images with a graphic-type picker and lazy list thumbnails, and the shared `FieldEditor`. This document defines the intended layout and must be updated when the structure changes.

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
├── PLAN.md                          # Implementation plan and milestone history
├── PROJECT-STRUCTURE.md             # This document
├── M14.5-METADATA-REPORT.md         # Verified legacy-schema seed report
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
    ├── XmlEditorReloaded.Imaging.Tests/
    └── XmlEditorReloaded.App.Tests/
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
- Overlay creation: a new file contains only the root element declared by metadata (UTF-8 without BOM); records and translated fields are added lazily by the writer. Existing files are never overwritten and the base table is never copied.
- Schema discovery (`TableSchemaBuilder`): the union of sparse record fields in first-appearance order, inferred types, presence counts, key/list/dict/translatable flags, and curated column overrides.
- Non-declaring column hints (`ColumnHints`): type, editor, display width, lookup, tooltip, flags, and detail-form group/order refine fields that are present in the document; hints never add absent fields. Nested elements are detected from child nodes and marked read-only with a dedicated subtype.
- `TableWorkspace`: a loaded base document plus optional overlay, merged rows, routed edits, add/duplicate/delete, bulk preview/apply, a bounded undo/redo history, unified saving, validation, and translation reports.
- `TableSetWorkspace`: the primary table plus every `SubTable` and `ChildRows` relation target loaded with one shared `TableEditHistory`; multi-file duplicate/delete/rename/swap as single undoable transactions, dependent-row reporting for confident deletion, missing-row creation for class-keyed sub-tables, and per-file atomic saving that reports every failed file. Nested field paths are created on demand, and repeated list fields are replaced without disturbing the sparse record shape.
- Lossless structural patching: scalar edits, element insertions, and deletions are applied to the original bytes with neighbor indentation; ambiguous blocks fall back to the serializer.
- Validation against metadata and referral integrity rules.
- Read-only cross-version comparison (`Comparison/`): table presence, record counts, field sets, inferred types, cardinality, lookup-reference availability, and sparse profiles for two `TableData` directories, plus Markdown/CSV report formatting. The comparer loads files only and never writes to either directory.
- Depends on `Core`.

### XmlEditorReloaded.Data (`net10.0`)

- SQLite metadata repository. The prototype schema from `References/JA_2_1.13_TableData_Editor` is the starting point:
  - `TableMappings` — XML table path, DTO/model name, lookup flag, XML root, list item name.
  - `ListFields` — XML field name ↔ model property for repeated elements.
  - `DictFields` — XML field mask (e.g. `ItemCapacityPerSize%Index%`) ↔ model property.
- Embedded, versioned migrations (`SchemaMigrations`) create the production schema: `Tables`, `TableKeys`, `Columns`, `ListFields`, `DictFields`, `Languages`, `TranslatableFields`, and `ColumnHints`. `Columns` is declaring metadata (an absent field becomes visible); `ColumnHints` is non-declaring and only refines fields discovered in the document. M14.5 populated 46 declaring columns for the 23 lookup tables and 502 verified hints for 25 tables; `Items.Items` remains hints-only so sparse records are never expanded.
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
- Translation workflow: per-tab language selector, merged base/translation display, untranslated cell and detail markers, a translation report window, and creation of a missing overlay. Edits to translatable fields go to the overlay only; edits to other fields go to the base table with a status warning.
- Conversion of backend pixel buffers to `BitmapSource` and image display.
- Dependency injection composition root and application configuration.
- Runtime data-set session (`IDataSetSessionFactory`/`DataSetSession`) that binds lookup and translation services to the selected data directory and opens tables through `TableWorkspace`.
- Grid editor (`TableEditorViewModel`, `TableView`): table tabs, schema-driven columns, sparse cell editing, row operations, search/sorting, column chooser, validation panel, and save/save-all. Rows can carry a lazily loaded small item thumbnail; the grid can be embedded without its standalone chrome for the item workspace.
- Tab contract (`IEditorTab`): `TableName`, title/display name, dirty and busy state, initialize/save/select-record, and language/navigation events. `MainViewModel` keeps a homogeneous tab list while the content template selects `TableView` or `ItemWorkspaceView`.
- Item workspace (`ItemWorkspaceViewModel`, `ItemWorkspaceView`, item section view models): searchable item list with thumbnails, editable ID (rename/swap), field groups from hints with a filter, class-aware 1:1 sub-table sections, embedded filtered child grids with add/delete, nested stance modifiers, repeated attachment lists, Big/Medium/Small previews with a graphic-type picker, whole-set validation, and a language selector that can create a missing overlay. New items use `ItemCreationWindow`; shared `FieldEditor` renders one field in both the detail panel and the item form. `FieldSourceSet` loads lookup and relation sources once for any workspace.
- Detail panel for the selected row: grouped and filterable fields, sparse edits routed through the workspace, translation markers, three STI preview panels, and a 32-bit unsigned flag editor for curated flag masks.
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

The production schema replaces the prototype mapping tables with `Tables`, `TableKeys`, `Columns`, `ListFields`, `DictFields`, `Languages`, `TranslatableFields`, and `ColumnHints`, plus `SchemaMigrations` for versioning. The seed covers 73 tables, 76 key segments, 12 list fields, 3 dict fields, 61 translatable paths, language `ru → Russian`, 46 declaring `Columns` rows for the 23 lookup tables, and 502 verified hints for 25 tables. `Items.Items` has 224 hints, including 8 flag masks; its fields are 105 Boolean, 87 Integer, 12 Decimal, 8 Flags, 3 Lookup, 6 String, and 3 nested. `ColumnHints` refines only fields that are present in a document, so sparse-record semantics are preserved. The hints were verified against both v4 and v5; stale legacy fields are excluded (see `M14.5-METADATA-REPORT.md`).

**Implemented (M18): `TableRelations`.** One row per source-field-to-target-table link: `SourceTableId`, `SourceField`, `TargetTableId`, `TargetValueField`, `TargetDisplayField`, `Kind` (`Reference`, `SubTable`, `ChildRows`, `Reverse`), structured filter columns (`FilterField`, `FilterOperator`, `FilterValue`), `LabelKey`, `RemapByText`, and `Ordinal`. The seed contains 84 relations: 68 `Reference` rows for the main-table columns of the seeded tables (`ITEM`, `AMMO`, `AMMOTYPE`, plus `Food`/`Clothes`/`RandomItem`), 5 item 1:1 sub-tables (`Weapons` matched by `uiIndex`; `Magazines`/`Armours`/`Explosives`/`LoadBearingEquipment` by `ubClassIndex`), 7 `ChildRows`, and 4 `Reverse` rows. Legacy references owned by shopkeeper/enemy/militia/IMP tables are added in M20 together with those tables because of the foreign keys to `Tables`. Filters are evaluated in memory with a closed operator set (`Equals`, `NotEquals`, `Contains`, `HasFlag`); no SQL or arbitrary expressions are stored. Filters only narrow picker options; existing values stay resolvable and validation checks the complete target. `ITEMTOEXPLOSIVE` is a `HasFlag 768` filter over `Items` and is never written as a separate table.

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
- Lookup identifiers are 64-bit values: `Lookup/ItemFlag.xml` contains IDs up to `8589934592`, which exceeds both `int` and `uint`. `LookupEntry.Id` is a `long`, and lookup files are parsed with 64-bit integer semantics.
- Bit-flag columns (item flags, ammo flags, attachment classes) are edited with a dedicated checkbox dialog rather than raw numbers. Curated flag masks use an unsigned 32-bit editor, so values above `int.MaxValue` are supported; other integer fields remain numeric.
- Lookups can be language-specific, similar to regular tables.
- Lookup metadata is seeded for real `Lookup/*.xml` files. Legacy references to main tables (`ITEM`, `AMMO`, `ITEMTOEXPLOSIVE`) are not lookup files because they use non-`id`/`name` value and text columns; since M18 they are `TableRelations` rows resolved by `TableRelationResolver` (value field, display field, optional structured filter).

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

### 6.8 Cross-version comparison

`TableDataComparer` compares two discovered `TableData` directories without writing to either of them:

- Tables are matched by dotted table name. Left-only and right-only tables are reported with their side; common tables are compared field by field.
- Records are identified by the metadata record element. Nested-key tables such as `Map.Cities` use descendants (`CITYLIST/CITY`); other tables use direct children of the root. Document-driven tables fall back to the root's children.
- Per side the report records the root and record element names, record count, distinct field names in first-appearance order, distinct record shapes, minimum/maximum/average fields per record, empty-record count, and overlay languages.
- Field types are inferred from values (`Boolean`, `Integer`, `Decimal`, `String`, `Nested`) with the same rules as schema discovery; the key field is never inferred as Boolean. Fields are compared by presence, inferred type, and cardinality (scalar versus repeated element).
- When one side is malformed or unreadable, the other side is still analyzed, but field-level differences are suppressed so that the readable side's fields are not misreported as removed.
- Lookup references come from `Columns` and `ColumnHints`; each reference reports whether the lookup table is available, missing, or unreadable on each side.
- The report can be rendered as Markdown (summary, lookup references, table list, details only for changed tables) or as long-format CSV with `Table`, `Field`, `Lookup`, and `Warning` rows. Both formats use invariant numbers and UTF-8 without BOM when exported.
- The comparison is strictly read-only: files are loaded through the normal loader, no document is saved, and unchanged byte content is verified by tests and the env-gated v4/v5 comparison.

### 6.9 Localization

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

Legacy parity is tracked in `PLAN.md`. The benchmark is `References/xml-editor`; `References/JA_2_1.13_TableData_Editor` is an unfinished prototype and is not a parity benchmark. **Item-workflow parity is reached at M19; full legacy parity at M23.** M24–M27 are post-parity improvements.

### Data sources and navigation

- Configure one or more game data directories; detect `TableData` automatically. **Multi-dataset management, switching, and recent directories: M21.**
- Optional language-specific directories for localized table variants. **M20.**
- Discover language overlays (`<Language>.<BaseFile>.xml`) per table; choose a display/edit language. **Done (M14); additional legacy language codes: M20.**
- Open v4, v5, and future versions side by side; report files and fields that differ between data sets. **Implemented read-only (M17):** the comparison window reports table presence, record counts, field sets, inferred types, cardinality, lookup references, and sparse profiles, and exports Markdown or CSV. **Value-level diff: M25.**
- Table list grouped by category (Items, Weapons, Inventory, Lookup, etc.). **Future backlog; not required for legacy parity.**
- Open several tables in tabs/panes; remember layout between sessions. **Tabs are done; layout persistence: M22.**
- Load malformed files in a diagnostic mode that reports the problem without rewriting the file. **Done (M3/M9).**

### Localization

- Show base English values with translated overlay values; mark untranslated fields and records. **Done (M14).**
- Edit translations in place; write only to the overlay file. **Done (M14).**
- Compare base and translation side by side. **M25.**
- Report missing translations, orphan overlay keys, duplicate keys, and empty values. **Done (M14).**
- Create a new language overlay containing keys and translatable fields only. **Done (M14).**
- Localize the editor UI itself (English/Russian initially), with a language selector in settings. **RESX en/ru done; selector and runtime application: M22.**

### Grid editing

- Virtualized grid with sorting, filtering (by value, excluding value, structured expression), and column selection. **Sort and column chooser are done (M12); structured filters and filter-by-value: M22.**
- Add, duplicate, and delete rows; automatic primary-key allocation. **Done (M11/M12).**
- Bulk value operations: add, multiply, set for a selected column. **Done (M16); selection-scoped bulk and empty Set: M22.**
- Copy rows between data sets with overwrite rules. **M21.**
- Item grid with inline image previews. **M19.**

### Item and gear editors

- Item master-detail with fields grouped by category (general, weapon, armour, flags, modifiers, food/drug, etc.). **Generic detail panel done (M13); full item workspace with related sub-tables: M19.**
- Item image preview (big, medium, small) with transparency. **Done (M13); graphic type/index picker and grid thumbnail: M19.**
- Attachment slots, default attachments, and compatible/incompatible attachment lists. **M19.**
- Merc starting gear editor with several gear kits. **M20.**
- Bit-flag editor, item class changer, new-item dialog, item ID lookup. **Flag editor done (M15); class changer, new-item dialog, and ID lookup: M19.**

### Lookup tables

- Editable lookup grids with ID/name validation. **Done for `Lookup.*` via the generic grid; auxiliary table metadata: M20.**
- Main-table item pickers (`ITEM`, `AMMO`, `ITEMTOEXPLOSIVE`). **Done (M18): searchable relation pickers in the grid and detail panel, target-record navigation, unknown values kept raw, relation validation.**
- Protection against deleting values referenced by other tables. **M26 (post-parity).**

### Persistence

- Save individual tables or all modified tables. **Done (M11/M12); item workspace Save All: M19.**
- Detect external file changes and warn before overwriting. **Detection is done; reload/discard/overwrite prompt: M22.**
- Preserve XML structure and encoding; round-trip unknown elements, sparse records, BOM, and line endings. **Done (M3/M10/M11).**
- Load → save without edits must reproduce the original file; this is verified by automated tests. **Done.**
- No working-data cache. Performance gates cover load time; relation and copy caches are in-memory, per-session, and invalidated on reload (**M18/M21**).

### UX and infrastructure

- Asynchronous loading with progress and cancellation. **Done (M9/M10).**
- Undo/redo for row-level edits where feasible. **Done (M16); composite item-workspace undo: M19.**
- Clear validation messages tied to table, row, and column. **Done per table; dataset-wide report: M25.**
- Settings dialog for game paths, language directories, and display options. **M22.**
- Crash-safe error reporting that does not discard unsaved changes silently. **Exit prompt and external-change handling: M22.**

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

**Legacy parity:** item-workflow parity at M19, full legacy parity at M23; M24–M27 are post-parity. The benchmark is `References/xml-editor`.

1. Scaffold the solution, shared build props, and empty projects; verify the build. **Done (M0).**
2. Implement `Core` metadata abstractions and the standard result type. **Done (M1).**
3. Implement `Data` SQLite metadata repository; import the prototype schema. **Done (M2).**
4. Implement `Imaging` STI parser and ETRLE decoder with golden-file tests. **Done (M7).**
5. Implement `Xml` table loading and round-trip saving with metadata validation; add byte-level round-trip tests against copies of v4/v5 `TableData`. **Done (M3–M6).**
6. Build the WPF shell: settings, data directory selection, table list. **Done (M9).**
7. Harden the backend: performance gates, cancellation contract, culture-independent numerics, documentation audit. **Done (M10).**
8. Add schema discovery, the table workspace (base + overlay, unified save), runtime data-set binding, and the lossless structural patcher. **Done (M11).**
9. Add grid editing (sorting, filtering, row operations, column selection). **Done (M12).**
10. Add the item detail form with STI images and flag editors. **Done (M13).**
11. Add localization support: overlay discovery, merged display, translation editing, validation reports, and RESX-based UI localization. **Done (M14).**
12. Seed column metadata from verified legacy definitions and activate type/lookup validation. **Done (M14.5).**
13. Add lookup, Boolean, and flag editors. **Done (M15).**
14. Add bulk column operations with preview and undo. **Done (M16).**
15. Add a read-only cross-version field report. **Done (M17).**
16. Add the relation model and main-table pickers (`ITEM`, `AMMO`, `ITEMTOEXPLOSIVE`). **Done (M18).**
17. Add the item master-detail workspace with related sub-tables, nested/list editing, and item workflows. **Planned (M19); item-workflow parity.**
18. Add the merc gear editor, legacy auxiliary table metadata (shopkeeper/enemy/militia/IMP/sounds), and the legacy language catalog. **Planned (M20).**
19. Add multi-dataset management and cross-dataset copy/merge with lookup and relation remapping. **Planned (M21).**
20. Add grid/workflow parity polish: structured filters, multi-select, selection-scoped bulk, layout persistence, settings dialog, and exit/external-change prompts. **Planned (M22).**
21. Audit working legacy features and record parity in `LEGACY-PARITY-REPORT.md`. **Planned (M23); full legacy parity.**
22. Add a universal nested/list/dict editor for Cities, facility assignments, root singleton cells, and overlay nested translations. **Planned (M24).**
23. Add comparison v2 (value-level diffs, overlay contents, root singletons), dataset-wide validation, and global search. **Planned (M25).**
24. Add lookup management and reference protection (CRUD, bitmask ID allocation, usage reports, delete protection). **Planned (M26).**
25. Add packaging and release: self-contained publish, icon/version/changelog, user guide, portable zip, CI gates. **Planned (M27).**
