# Sources and evidence

The help documents describe the **shipped game data** and the **game code that reads it**.
They do not cite source files or line numbers: those drift, and a modder needs the behaviour,
not the location. Each field says what kind of evidence supports it and how confident the
answer is.

## Source types

| Source | What it is used for |
|---|---|
| Game code | parsing, defaults, clamping, effective type and runtime use |
| XML comments and readmes | author intent, units and enumerations |
| Editor metadata | types, lookup names, limits and table relations |
| Legacy editors and DTOs | naming and grouping only; not semantics |
| Table data | observed values, ranges and sparse fields |

`References/` is read-only. The help tooling reads v4, v5, the game source and the legacy
editors, but never writes to them.

## Roots

| Root | Contents | Use |
|---|---|---|
| `References/JA 2 1.13 v5/Data-1.13/TableData` | English base data, 239 tables | primary data set |
| `References/JA 2 1.13 v4/Data-1.13/TableData` | Russian-styled data, 238 tables + 25 `Russian.*` overlays | cross-check and fallback |
| `References/Ja 2 1.13 source` | C++ game source | behaviour and field semantics |
| `References/xml-editor` | legacy VB editor | historical names and grouping |
| `References/JA_2_1.13_TableData_Editor` | legacy C# DTO editor | structure hints only |

## How a field is verified

1. Find the table's root and record elements in the XML.
2. Find the loader for those elements in the game code.
3. Read both the start-element and end-element handlers. The save/write path is often the
   best complete field list, but it may write defaults that the loader never reads.
4. Cross-check the runtime record or struct that receives the value. Comments there often
   give units and ranges.
5. Compare the v4 and v5 data when values differ; structure differences are described in the
   table's `Versions` section.
6. Prefer a direct source observation over a plausible name. If the meaning is still unclear,
   mark the field `unknown` and write the open question.

The table tag and the runtime record are the stable anchors. File names and line numbers are
not used in the help.

## Evidence order

1. **Game code** - authoritative for parsing, defaults, clamping and effective type.
2. **XML comments and readme files** - author intent; verify against code behaviour.
3. **Verified editor metadata** - types, lookup names, limits and relations. It is curated but
   incomplete: 66 tables have no metadata row.
4. **Legacy editors and DTOs** - naming and grouping only; do not copy semantics.
5. **Data statistics** - useful to detect outliers and sparse fields, never proof of meaning.

## What a field entry says

Each field entry answers:

- What is stored (unit, scale, index or bit mask)?
- Who reads it (loader, AI, UI, another table)?
- What are the valid values and the default?
- What happens for a missing, empty or out-of-range value?
- Which version(s) does the answer apply to?
- How confident is the answer, and why?

The `Source` line uses a general label such as `game code (item loader)`, `table data` or
`editor metadata`. It never contains a source file name or a line number.

If a field is opaque, record `unknown` and the exact question in `## Open questions`. A
plausible guess is worse than a documented gap.
