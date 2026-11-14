# XML Editor Reloaded

**English** | [Русский](README.ru.md)

A desktop editor for the `TableData` XML tables of the **Jagged Alliance 2 1.13** mod, with
previews of STI item images. It works with v4 and v5 data sets, preserves unknown elements and the
original lexical form of values, and supports Russian and English translation overlays.

> This is a community tool. It does not install or modify the mod itself; it edits an existing
> game installation.

## Features

- Schema-driven grid for viewing and editing every `TableData` table.
- Table tree and flat list with filtering, search, tabs, and several data sets open at once.
- Item master-detail editor, merc gear, shopkeeper, auxiliary tables, and nested editors for
  `Map.Cities` and `Map.FacilityTypes`.
- Lookup and flag editors, relation navigation, pickers, bulk edits, filters, and undo/redo.
- Translation overlays: translated text is written only to the overlay file; the base table is
  never changed.
- Dataset validation with a single report, global search across base tables and overlays, and
  Markdown/CSV export.
- v4 ↔ v5 comparison by structure and values, plus row copying between data sets with remapping.
- STI image previews for Big/Medium/Small item images, with frame selection and live thumbnails.
- Grid/XML view for a read-only snapshot of the base table, record search, and navigation from a
  validation finding to the affected record.
- Dark, light, and system themes; Russian and English UI.

## Requirements

- Windows 10 or 11, x64.
- An installed Jagged Alliance 2 1.13 with `Data-1.13\TableData`.
- For the framework-dependent package: [.NET 10 Desktop Runtime x64](https://dotnet.microsoft.com/download/dotnet/10.0).
  The self-contained package does not need a separate runtime.

## Download and run

Download the latest release from the
[Releases page](https://github.com/shogun-kub/XML-Editor-Reloaded/releases/latest). Each archive
has a matching `.sha256` file.

| Archive | What it contains |
|---|---|
| `XmlEditorReloaded-<version>-win-x64.zip` | Self-contained build; the .NET runtime is included |
| `XmlEditorReloaded-<version>-win-x64-framework-dependent.zip` | Smaller build; requires .NET 10 Desktop Runtime x64 |

1. Extract the archive to any folder.
2. Run `XmlEditorReloaded.App.exe`.
3. Click **Browse** and select the game folder, the `Data-1.13` folder, or `Data-1.13\TableData`.
4. Double-click a table in the tree or list to open it.

Windows may show a SmartScreen warning because the build is not signed with a commercial
certificate. Verify the downloaded archive against its `.sha256` file if needed.

For a fully portable installation, run `start-portable.cmd`. Settings, layouts, the metadata
database, and the log are then stored in a `data` folder next to the application instead of
`%AppData%\XmlEditorReloaded`.

## Typical usage

### First run and navigation

Choose the data directory once; the editor finds `TableData` automatically. The left panel has two
views: **Tree**, grouped by table folders, and **List**, a flat table index. Each opened table gets
its own tab, and tabs from different data sets can stay open together.

### Editing a table

Select a cell and enter a new value. Lookup fields offer a list of display names while storing the
original identifier. Press **Enter** to apply the value, or **Esc** to cancel the current edit.
Use **Ctrl+Z / Ctrl+Y** for undo/redo before saving.

Save the active table with **Ctrl+S**, or all open tables with **Ctrl+Shift+S**. Missing elements
stay missing; unknown elements and attributes are preserved when the file is written.

### Translations

Choose a language in the editor. Editing a translation writes only to the corresponding overlay
file, such as `Russian.Items.xml`; the base table is not modified. Untranslated fields are shown
from the base table with a marker. Use the translation report to find missing, orphaned, or empty
translations.

### Validation, search, and comparison

- **F8** validates the current table; **Ctrl+Shift+P** validates the whole data set.
- **Ctrl+Shift+F** searches values across base tables and overlays.
- The **Compare** command matches v4 and v5 records by their declared key, or by order when no key
  is available, and reports structural and value differences.
- Validation reports and comparison results can be exported to Markdown or CSV.

### Images and special editors

Item images can be viewed in Big/Medium/Small sizes with frame selection. The item editor shows
the item and its subtables, merc gear shows five equipment kits, and `Map.Cities` /
`Map.FacilityTypes` have dedicated nested editors.

## Data safety

- Only game files that you explicitly open and save are written.
- Loading and saving without changes does not overwrite a file.
- Unknown elements and attributes are preserved; element order and lexical values are not
  normalized without a reason.
- Saves are atomic, and external file changes are detected before overwriting.
- Invalid XML is reported and is not silently repaired or overwritten.
- Validation warns about problems but never deletes or normalizes data.
- There is no automatic XML backup: make a copy of `TableData` before your first editing session.

Settings, layouts, `metadata.sqlite`, and `XmlEditorReloaded.log` live in
`%AppData%\XmlEditorReloaded`, or in `XER_APP_DATA` when that variable is set (the portable launcher
uses a `data` folder next to the application).

## Reporting bugs

Please use the [GitHub issue tracker](https://github.com/shogun-kub/XML-Editor-Reloaded/issues).
Before opening a new issue, check the latest release and search existing issues — the problem may
already be known.

A useful report includes:

- the application version from **About**;
- Windows version and the release variant used (self-contained or framework-dependent);
- the data set version (v4 or v5) and the affected table;
- exact steps to reproduce, expected behavior, and actual behavior;
- the full error message and, when relevant, `XmlEditorReloaded.log`;
- a screenshot if it helps explain the problem.

Do not attach the game's original `TableData` files or other copyrighted game content. If a
specific XML fragment is required, include only the smallest anonymized excerpt.

If you suspect data loss or file corruption, stop editing, keep a copy of the affected files, and
mention this explicitly in the issue. Feature requests are also welcome in the issue tracker.

## Documentation

- [User guide](xml-editor-reloaded/USER-GUIDE.md) — detailed end-user instructions.
- [User guide in Russian](xml-editor-reloaded/USER-GUIDE-RU.md) — Russian translation.
- [Changelog](xml-editor-reloaded/CHANGELOG.en.md) — release history.
- [Project structure](PROJECT-STRUCTURE.md) — architecture, projects, and repository layout.
- [Third-party notices](xml-editor-reloaded/THIRD-PARTY-NOTICES.md) — dependencies and licenses.

## Building from source

Requires the .NET 10 SDK.

```powershell
dotnet restore xml-editor-reloaded/XmlEditorReloaded.slnx
dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx -m:1 --no-restore -warnaserror
pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/test.ps1
```

The application is built to
`out/bin/XmlEditorReloaded.App/<Configuration>/XmlEditorReloaded.App.exe`. All `bin`/`obj` output
is kept under `out/`.

## License

The project is distributed under the
[GLWT(Good Luck With That) Public License](xml-editor-reloaded/LICENSE). Third-party libraries keep
their own licenses; see [THIRD-PARTY-NOTICES.md](xml-editor-reloaded/THIRD-PARTY-NOTICES.md) and
`xml-editor-reloaded/licenses/`.
