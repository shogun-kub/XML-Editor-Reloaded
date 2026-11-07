# Sources and citations

The help documents describe the **shipped game data** and the **game code that reads it**.
This file fixes the source roots and the citation format so every page can be checked.

## Roots

| Root | Contents | Use |
|---|---|---|
| `References/JA 2 1.13 v5/Data-1.13/TableData` | English base data, 239 tables | primary data set |
| `References/JA 2 1.13 v4/Data-1.13/TableData` | Russian-styled data, 238 tables + 25 `Russian.*` overlays | cross-check and fallback |
| `References/Ja 2 1.13 source` | C++ source, 1,327 `.cpp/.h/.hpp` files | behaviour and field semantics |
| `References/xml-editor` | legacy VB editor | names, grouping, historical relations |
| `References/JA_2_1.13_TableData_Editor` | legacy C# DTO editor | names and structure hints |

`References/` is read-only and is never modified by the help tooling.

## Citation format

Code references are written as text, not links:

```
Tactical/Weapons.cpp:390
Tactical/Weapons.h:139-144
Utils/XML_Items.cpp:577-580
```

- Paths are relative to `References/Ja 2 1.13 source`.
- Line numbers are an orientation; the function name and the XML tag are the stable parts.
  When code moves, update the line, not the meaning.
- Data references use `TableData/<relative path>` from the selected version, for example
  `TableData/Map/Cities.xml`.
- Metadata references name the table and column, for example
  `metadata: Tables.IsFlagLookup` or `metadata: TableKeys`.

## How to find a loader

1. Take the table's root and record element from the XML or from the inventory.
2. Search the source for the string literal:

   ```powershell
   Select-String -Path 'References/Ja 2 1.13 source/**/*.cpp' -Pattern '"WEAPONLIST"'
   ```

3. Most loaders live in `Tactical/XML_*.cpp`, `Strategic/XML_*.cpp`, `Laptop/XML_*.cpp` or
   `Utils/XML_*.cpp`. Some older loaders live in gameplay files:
   - `Tactical/Weapons.cpp` - `Weapons.xml`;
   - `Strategic/strategicmap.cpp` - `Map/Cities.xml`;
   - `Arms Dealer Init.cpp` / `XML_TonyInventory.cpp` - dealer inventories.
4. Read both the start-element and end-element handlers. The save/write function is often the
   best complete field list, but it may write defaults that the loader never reads.
5. Cross-check the struct in the matching header (`Weapons.h`, `Item Types.h`, ...). Header
   comments often give units and ranges.

## Evidence order

1. **Loader/save code** - authoritative for parsing, defaults, clamping and effective type.
2. **XML comments and readme files** - author intent; verify code behaviour.
3. **Verified editor metadata** - types, lookup names, limits and relations. It is curated but
   incomplete: 66 tables have no metadata row.
4. **Legacy editors and DTOs** - naming and grouping only; do not copy semantics.
5. **Data statistics** - useful to detect outliers and sparse fields, never proof of meaning.

## Reporting a field

Each field entry should answer:

- What is stored (unit, scale, index or bit mask)?
- Who reads it (loader, AI, UI, another table)?
- What are the valid values and the default?
- What happens for a missing, empty or out-of-range value?
- Which version(s) does the answer apply to?
- How confident is the answer, and why?

If a field is opaque, record `unknown` and the exact question in `## Open questions`. A
plausible guess is worse than a documented gap.
