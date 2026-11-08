# XML Editor Reloaded — user guide

**English** | [Русский](USER-GUIDE-RU.md)

The editor is designed for viewing and editing the `TableData` XML tables of the **Jagged Alliance 2
1.13** mod. It is not a mod installer and does not create new tables from scratch: it works with an
existing game installation and its `Data-1.13` directory.

## 1. Requirements and installation

- Windows 10 or 11, x64.
- An installed Jagged Alliance 2 1.13 with `Data-1.13\TableData`.
- For images: `Data-1.13\BigItems`, `Data-1.13\Interface`, and `Data-1.13\tilesets\0`.
- For the framework-dependent package: [.NET 10 Desktop Runtime x64](https://dotnet.microsoft.com/download/dotnet/10.0).
  The self-contained package does not need a separate runtime.

The release is available in two variants. Extract the archive to any folder and run
`XmlEditorReloaded.App.exe`:

| Archive | Contents |
|---|---|
| `XmlEditorReloaded-<version>-win-x64.zip` | Self-contained: the .NET runtime is included |
| `XmlEditorReloaded-<version>-win-x64-framework-dependent.zip` | Application only; requires .NET 10 Desktop Runtime x64 |

Windows may show a SmartScreen warning because the build is not signed with a commercial
certificate. Verify the archive's SHA-256 against the matching `.sha256` file:

```powershell
Get-FileHash .\XmlEditorReloaded-1.0.0-win-x64.zip -Algorithm SHA256
Get-FileHash .\XmlEditorReloaded-1.0.0-win-x64-framework-dependent.zip -Algorithm SHA256
```

### Portable mode

`start-portable.cmd` sets `XER_APP_DATA=%~dp0data` and starts the editor. In this mode, settings,
layouts, the metadata database, and the log are stored in a `data` folder next to the application
instead of the Windows user profile.

A normal launch stores these files in `%AppData%\XmlEditorReloaded`:

| File | Purpose |
|---|---|
| `settings.json` | data directories, theme, interface language |
| `layouts.json` | column widths and order |
| `metadata.sqlite` | table, relation, lookup, and translation metadata |
| `XmlEditorReloaded.log` | operation and error log |

Deleting these files resets settings and layouts; it does not affect the game's XML files.

## 2. First run

1. Click **Browse** next to the data directory field.
2. Select the game folder, the `Data-1.13` folder, or `Data-1.13\TableData` — the editor finds
   `TableData` automatically.
3. The left panel shows the **Tree** and **List** tabs. The tree groups tables by folder; the list
   shows a flat index.
4. Double-click a table to open it in a tab.

The data directory can be changed at any time. Open tabs keep their data set, and several data sets
(for example v4 and v5) can be open at the same time.

## 3. Main window

- **Tree/List** — table navigation, with filtering, **Collapse all**, **Expand all**, and
  **Find in tree**.
- **Editor tabs** — each open table has its own tab. Closing a tab with unsaved changes requires
  confirmation.
- **Grid** — the record table. **Details** on the right shows the fields of the selected record.
- **Validation panel** — findings for the current table.
- **Status bar** — path, record count, operation result, and errors.

The **Grid / XML** switch is on the table tab (Ctrl+E). The XML view is read-only; editing is done in
the grid.

## 4. Editing a table

- Select a cell and change the value. Lookup fields offer a list of display names while the original
  identifier is stored in the file.
- **Enter** applies a value; **Esc** cancels the current edit (behavior depends on the editor type).
- **Ctrl+Z / Ctrl+Y** undo and redo edits made before saving.
- **Ctrl+S** saves the active table; **Ctrl+Shift+S** saves all open tables.
- **Insert** adds a record, **Ctrl+D** duplicates one, **Delete** deletes one.
- Filters: **Ctrl+Shift+A** adds a condition, **Ctrl+Shift+X** clears filters, **Ctrl+G** jumps to a
  filter value.
- Bulk edit: select several rows and click **Bulk edit**. For lookup columns, a display name can be
  selected from the list.
- **Reset layout** in settings restores the default column widths and order.

Records are sparse: missing elements are not added automatically. Unknown elements and attributes
are preserved when the file is written.

## 5. Details, relations, and images

- The **Details** panel shows the selected record's fields, including lookup names, flags, and
  related tables.
- The relation button opens the target table and selects the referenced record.
- Lookup values support:
  - **Ctrl+L** — change the ID (a free bit for a bit mask, the next ID for a sequence);
  - **Ctrl+U** — **Where used**, with navigation to the first record;
  - deletion and ID changes are blocked while the value is in use.
- **Images** opens the Big/Medium/Small preview. You can choose the type, browse frames, and see
  live thumbnails in the grid.
- **Item editor** shows an item and its subtables; **Merc gear** shows five equipment kits;
  `Map.Cities` and `Map.FacilityTypes` have dedicated nested editors.

## 6. Translations and overlays

- In v4, the base localization is Russian; in v5, it is English. Missing `Russian.*` files in v5 are
  normal, not lost data.
- An overlay is a `<Language>.<BaseFile>.xml` file in `TableData`. It contains a key and only the
  overridden fields.
- Choose a language in the editor. Translation edits are written **only to the overlay**; the base
  file is not changed.
- An untranslated field is shown from the base table with a marker. A new overlay is created lazily
  and contains only the key and translated fields.
- The **Translation report** shows missing, orphaned, and empty translations. There is no automatic
  filling.
- Nested structures use separate keys: `uiIndex` for cities, `ubAssignmentType` for assignments,
  and the kit number for merc gear.

## 7. Validation and data-set search

- **Ctrl+Shift+P** validates all tables. The report shows errors, warnings, and information; an
  unreadable or malformed file becomes a report error instead of aborting the scan.
- Filter by severity and text; export to Markdown or CSV.
- Double-click a finding to open the table and select the affected record.
- **Ctrl+Shift+F** searches values across base tables and overlays. Results include the table, key,
  field path, value, and language. Selecting a result opens the record.

Validation only reads files and never fixes anything.

## 8. Comparing v4 and v5

The **Compare** button opens a dialog for choosing two directories. The comparison:

- matches records by their declared key, or by order when no key is available;
- compares values lexically (`1.0` and `1` are different values);
- compares overlays by the cells they contain;
- compares root singleton fields (`COUNTRY`) separately;
- limits the amount of detail but keeps exact counters;
- exports the report to Markdown or CSV.

## 9. XML mode

- **Ctrl+E** switches between Grid and XML.
- XML is a read-only snapshot of the **base** table; overlays and child tables are not included.
- **Copy** copies the text; **Refresh** reloads the snapshot.
- Search accepts a record ID/key and highlights the opening tag, for example `<WEAPON>`.
- In the grid, selecting a validation finding moves to the affected row. For a finding in a child
  table, the item workspace reports that the table must be opened separately.
- XML text cannot be edited.

## 10. Keyboard shortcuts

| Keys | Action |
|---|---|
| Ctrl+O | open a table |
| Ctrl+S | save the active table |
| Ctrl+Shift+S | save all |
| Ctrl+Z / Ctrl+Y | undo / redo |
| Ctrl+F | search in the table |
| Ctrl+Shift+T | find the selected record in the tree |
| Ctrl+E | Grid / XML |
| Ctrl+Shift+P | validate the data set |
| Ctrl+Shift+F | search the data set |
| Ctrl+, | settings |
| Ctrl+W | close the tab |
| F5 | reload the table |
| F8 | validate the current table |
| F1 | context help for the active table and field |
| Insert | add a record |
| Ctrl+D | duplicate a record |
| Delete | delete a record |
| Ctrl+L | change a lookup value's ID |
| Ctrl+U | where a lookup value is used |
| Ctrl+Shift+A | add a filter |
| Ctrl+Shift+X | clear filters |
| Ctrl+G | go to a filter value |
| Ctrl+N | new item (item workspace) |

`F1` and **Help > Context help** open the field reference for the active table and cell. The
window searches all table help and follows related-document links; **Open table** opens the
related table in the main window. **Help > Glossary** and **Help > Index** open the term list
and the generated table index. **Help > About** remains available without a shortcut.

## 11. Integrity and recovery

- Make a copy of `TableData` before your first editing session: there is no automatic XML backup.
- Saves are atomic; a file is overwritten only when it has real changes.
- External file changes are checked by length and modification time. On a conflict, the editor
  offers to reload the file or explicitly overwrite it.
- Malformed XML is reported by diagnostics; such a file is not saved or overwritten.
- Save and preflight errors are written to `XmlEditorReloaded.log`.
- If the application cannot write to `%AppData%`, run `start-portable.cmd` or set `XER_APP_DATA` to
  a writable directory.
- `References/` and the original `TableData` are not modified without an explicit save. Use a copy
  of the data set for experiments.

## 12. Known limitations

- The XML view shows only the base table and is not a text editor.
- There is no automatic backup and no three-way merge.
- Some tables without seed metadata are shown through a document-driven fallback; new fields are
  preserved even when they have no description yet.
- The interface and help are localized in Russian and English; the language is switched in settings.

## Reporting problems

Use the [GitHub issue tracker](https://github.com/shogun-kub/XML-Editor-Reloaded/issues). Include the
application version, Windows version, data-set version (v4 or v5), affected table, exact steps,
expected and actual behavior, and the relevant part of `XmlEditorReloaded.log`. Do not attach the
game's original `TableData` files; if needed, include only a minimal anonymized XML fragment.
