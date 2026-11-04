# UI spacing rules

These rules apply to every WPF form in `xml-editor-reloaded/src/XmlEditorReloaded.App/Views` and to
`MainWindow.xaml`. Review a form against this checklist before changing its layout.

## Rules

1. **Minimum gap: 3 px.** Two adjacent controls never touch. Use the standard gap of **6 px** for
   form-level rows, button rows, and label/field pairs; 3 px is allowed only inside dense list rows.
2. **One gap per button row.** Every gap between adjacent buttons in a row is equal. Use
   `Margin="0,0,6,0"` for every button except the last, which has `Margin="0,0,0,0"`; a wrapping
   toolbar uses `Margin="0,0,6,6"` on every button so the next wrapped line also starts 6 px below.
3. **One row gap per form.** Vertical gaps between rows of the same form are equal. Use `6` on the
   bottom margin of the row container, not a mix of 4/8/10/12 on individual children.
4. **Label and field stay together.** A label and the control it marks sit in one `Grid` row with
   `Auto` + `*` columns (or one `DockPanel` with the label docked left). Do not put the label and the
   field as independent children of a `WrapPanel`: a narrow window can wrap one without the other.
   The gap between the label and its field is 6 px.
5. **Outer padding is not a gap.** Window/panel padding may be 8, 12, or 16 px; it does not replace
   the 6 px gaps between rows inside the panel.
6. **Control padding is separate.** `Padding` inside a button or text box is chosen for its content
   and does not count as inter-element spacing.

## Checklist

| Form | Reviewed | Notes |
| --- | --- | --- |
| `MainWindow.xaml` | ✅ | Directory label/combo share one Grid row; Browse/Remove/Refresh/Save have equal 6 px gaps; no wrapping of a label away from its field. |
| `TableView.xaml` | ✅ | Toolbar buttons 6 px apart and 6 px line gap; the lookup kind marker and «Изменить ID…» use the same 6 px toolbar gaps; filter, columns, detail and status rows use 6 px; label/field pairs are Grid columns. |
| `ItemWorkspaceView.xaml` | ✅ | Toolbar wraps with 6 px horizontal/vertical gaps; language label and combo are one StackPanel; item, sub-table, merchant and image rows use 6 px. |
| `MercGearWorkspaceView.xaml` | ✅ | Toolbar wraps with equal 6 px gaps; language pair is one StackPanel; column and gear rows use 6 px. |
| `NestedTableWorkspaceView.xaml` | ✅ | Toolbar wraps with equal 6 px gaps; add-assignment label/field pair is one StackPanel; sections and lists use 6 px. |
| `FieldEditor.xaml` | ✅ | Label and editor are adjacent Grid columns with 6 px gaps; buttons use 6 px. |
| `BulkEditWindow.xaml` | ✅ | Header label/field pairs are Grid columns with 6 px; checkboxes and action buttons use 6 px. |
| `CompareWindow.xaml` | ✅ | Directory labels/fields and action buttons use 6 px; progress/status follow the same gap; the Records/Overlays tabs keep 6 px between summary, grids, splitter and root-field group. |
| `DatasetWindow.xaml` | ✅ | Validation toolbar, summary/note/filter rows and search row use equal 6 px gaps; label/field pairs are Grid columns; grids, status row and close button keep 6 px. |
| `TableCopyWindow.xaml` | ✅ | Source/target and option labels/fields use 6 px; action row uses 6 px. |
| `SettingsWindow.xaml` | ✅ | Directory row, column gap, reset/status rows and footer buttons use 6 px. |
| `ImageViewerWindow.xaml` | ✅ | Item, type/index, zoom and preview header rows use 6 px; three preview columns are separated by 6 px. |
| `TranslationReportWindow.xaml` | ✅ | Summary, group chips, issue grid and close button use 6 px. |
| `BitEditorWindow.xaml` | ✅ | Summary, 3 px bit grid and footer buttons meet the minimum. |
| `LookupIdWindow.xaml` | ✅ | Summary, kind hint, ID label/field Grid row, error/warning lines and footer buttons use 6 px. |
| `ItemCreationWindow.xaml` | ✅ | Three label/field Grid rows and footer buttons use 6 px. |
| `AboutWindow.xaml` | ✅ | Label/value rows use 6 px; close button is separated by 6 px. |
| `EnemyAmmoWindow.xaml` | ✅ | Ammo cells use 3 px side gaps and a 3 px label/field gap; action buttons use 6 px. |
| `ExternalChangeWindow.xaml` | ✅ | Message and three action buttons use 6 px. |
| `UnsavedChangesWindow.xaml` | ✅ | Message and three action buttons use 6 px. |
| `RelationPicker.xaml` | ✅ | The text box and dropdown arrow are separated by 3 px. |
