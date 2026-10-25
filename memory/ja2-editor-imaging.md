---
name: ja2-editor-imaging
description: STI decoding/catalog and the standalone image viewer in XML Editor Reloaded (M7 plus the STI viewer quick win).
metadata:
  node_type: memory
  type: project
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

- **M7 imaging:** `Sti.StciParser` (validates magic, flags must be exactly `Indexed|Etrle`, rejects RGB, bounds-checks every directory entry → `sti.bad_magic` / `sti.truncated` / `sti.unsupported_flags`), `Etrle.EtrleDecoder` (`0` = end of row, high bit set = transparent run, `1..127` = literal run; overrun → `sti.etrle_overrun`), `Catalog.ItemImageCatalog` (big = `BigItems/gun{NN}.sti` or `p{type}item{NN}.sti` frame 0, medium = `Interface/mdguns.sti` / `mdp{type}items.sti` frame = index, small = `tilesets/0/...` frame = index; index padded to 2 digits only below 10; directory scan is case-insensitive because the files mix `GUN00.STI` and `Gun375.sti`), `ImageTransparency.ApplyGreenKey` for medium/small only, `Cache.DecodedFrameCache` (LRU by bytes, invalidated by mtime). Fixtures are the real `P2ITEM35.STI`, `MDP3ITEMS.STI`, `SMP3ITEMS.STI` copied into the test project; `.gitattributes` marks those fixtures `-text -diff`.
- **STI viewer quick win (commit `7b770fc`):** standalone read-only window opened from the shell. `DataSetPaths` resolves TableData/Data-1.13 from the selected path; `WpfItemImageService` caches one `ItemImageCatalog` per data directory and converts RGBA→BGRA32 frozen `BitmapSource` (WPF has no Rgba32 pixel format, so the App layer swizzles); `ItemGraphicsOption.FromRecord` projects `uiIndex`/`szItemName`/`ubGraphicType`/`ubGraphicNum` without metadata (`ubGraphicType` absent → gun sheet 0; 379 v5 items omit it); the viewer auto-selects the first item with `ubGraphicNum`, shows big/medium/small side by side with a checkerboard, zoom 1–8× and nearest-neighbor scaling, and per-panel size or error code. Composition moved to public `App.ConfigureServices`, covered by a DI test plus STA XAML smoke tests for both windows. An env-gated real-data test asserts >1000 projected items and decodes the real `P2ITEM35.STI` (type 2 / index 35). Total 149 tests.
- Related: [[ja2-editor-item-merc]] (Big/Medium/Small previews), [[ja2-editor-shell-grid]], [[ja2-editor-parity]] (image gaps in M23a3).
