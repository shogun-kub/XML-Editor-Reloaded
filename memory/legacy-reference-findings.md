---
name: legacy-reference-findings
description: Key non-obvious findings about the legacy reference projects
  (StiLib canvas/frame confusion, hardcoded schema, broken prototype parser) plus the real STI header layout
metadata:
  node_type: memory
  type: project
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

Findings from analyzing the three references on 2026-10-23 that are not obvious from the trees:

- **StiLib.dll** (`JA 2 1.13 Image viewer`, 2008, x86, .NET Framework) cannot be referenced from .NET 10 x64. The portable STI/ETRLE algorithm is in `xml-editor/BackEnd/Images/STI.vb`.
- **xml-editor** hardcodes its ~70-table `DataSet` schema in `BackEnd/DatabaseCode.vb` (2,504 lines); there is no external metadata DB. Designer files are huge (`ItemDataForm.Designer.vb` ~14,000 lines) and loading uses `Application.DoEvents()`.
  - When seeding our `Columns`, this file is the best available schema source: 710 `MakeColumn` calls across ~57 `Make…Table` functions (`ITEM` alone has 257 columns) carrying type (`Integer`/`Boolean`/`Decimal`/`ULong`/`Byte`/`String`), lookup table/ID/text columns, caption, hidden flag, max length and `ReferenceRequired`. `MakeLookupTable` (id/name pairs) covers all 23 `Lookup/*.xml` files.
  - Lookup arguments do not always point at `Lookup/*.xml`: many columns look up into main tables (`"ITEM","uiIndex","szLongItemName"`, `"AMMO","uiIndex","AmmoCaliber"`) and add SQL-ish filters (`"usItemClass <> 1 OR uiIndex=0"`); only ~20 `MakeLookupTable` calls map to the 23 `Lookup/*.xml` files.
  - It is version-stale, so it must be filtered against real v4/v5 data, never imported blindly: it declares `AvailableAttachmentPoint0..9` while the real data uses a single `AvailableAttachmentPoint`; `BarrelConfiguration` is commented out; shopkeeper ControlTable/InventoryTable are deliberately unsupported; localized tables (`MakeLocalizedItemsTable`) are overlay-shaped; `ULong` masks such as `usItemClass` reach `1073741824`.
- **JA_2_1.13_TableData_Editor** `TableDataParser.ParseSimpleTable<T>` is broken: it never calls `result.Add`, and the list/dict/unknown-table branches are empty. Its value is the SQLite metadata schema (`TableMappings` 53 rows, `ListFields` 12, `DictFields` 3). It is **not a functional editor and must not be used as a parity benchmark** — the only working legacy benchmark is `xml-editor` (agreed with the user 2026-10-24 when defining "functional parity").
- **STI specifics:** palette bytes in the file are **RGB** order (`palette[i*3..i*3+3]` matched a decoded `#8C7A7A` at index 50 in v4 `BigItems/P2ITEM35.STI`); the legacy `STI.vb` reads them reversed because its output is GDI+ BGR, which is why older notes said "BGR". Palette index 0 is transparent; named item images live under `BigItems`, `Interface` (md*), and `tilesets\0` (sm*).
- **Legacy compound primary keys (`DatabaseCode.vb`, found 2026-10-24 during the M23a audit):** legacy declared multi-field unique constraints that our seed lost — `MERGE` `(firstItemIndex, secondItemIndex, firstResultingItemIndex, secondResultingItemIndex)`, `LAUNCHABLE` `(launchableIndex, itemIndex)`, `INCOMPATIBLEATTACHMENT` `(itemIndex, incompatibleattachmentIndex)`, `COMPATIBLEFACEITEM` `(compatiblefaceitemIndex, itemIndex)`, `ATTACHMENT` `(attachmentIndex, itemIndex)`, `TRANSFORM` `(usItem, usResult1..usResult10)`. Legacy shopkeeper inventories are keyed by `uiIndex` only (and legacy maps just the 20 named dealers, not `AdditionalDealer_*`); it has no `StructureConstruct`/`StructureDeconstruct` builders. Legacy also mirrors reverse pairs with `DuplicateEntryTable` on save and never uses the STCI `transparentValue`, instead making a corner color (`SetTransparentColours`) plus bright green transparent at load time.
- **StiLib header quirk:** `StciData.Height/Width` and `UiAppDataSize` are wrong — they read 480/640 for every file (the canvas size) and 0 even when the app-data byte is non-zero. Probe with the raw bytes instead: `@16` u32 flags (40 = indexed|ETRLE), `@20/@22` u16 canvas height/width, `@24` u32 color count, `@28` u16 sub-image count, `@30..33` depth bytes (8,8,8,0), `@48` u32 app-data size (always 0 across all 1,584 v4 item STIs). Real frame sizes are only in the 16-byte directory entries.

**Why:** These were discovered by reading the sources and would otherwise be re-derived on every task touching the references.

**How to apply:** Warn about them before proposing to reuse legacy code; see [[reference-projects-structure-doc]] for full details.
