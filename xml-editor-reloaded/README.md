# XML Editor Reloaded

Desktop editor for Jagged Alliance 2 1.13 `TableData` XML files and STI item images.

## Build

```
dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx
```

All projects build into the repository-level `out/` folder (`out/bin/<Project>/<Configuration>`,
`out/obj/...`). Run the application from
`out/bin/XmlEditorReloaded.App/<Configuration>/XmlEditorReloaded.App.exe`.

## Test

```
dotnet test xml-editor-reloaded/XmlEditorReloaded.slnx
```

Optional full-corpus round-trip tests (runs against a temporary copy, never writes to the source):

```
XER_CORPUS_DIR="<GameDir>\Data-1.13\TableData" dotnet test
```

Optional performance gates (read-only; each operation must stay under 2 seconds):

```
XER_CORPUS_DIR="<GameDir>\Data-1.13\TableData" dotnet test --filter "Category=Performance"
```

## Layout

- `src/XmlEditorReloaded.Core` — domain primitives and abstractions (no I/O).
- `src/XmlEditorReloaded.Xml` — XML table loading, round-trip saving, overlays.
- `src/XmlEditorReloaded.Data` — SQLite metadata, table catalog, settings.
- `src/XmlEditorReloaded.Imaging` — STI/ETRLE decoding to RGBA buffers.
- `src/XmlEditorReloaded.App` — WPF application shell.
- `tests/` — xUnit test projects.

See `PLAN.md` in the repository root for the implementation plan and `PROJECT-STRUCTURE.md` for the architecture.
