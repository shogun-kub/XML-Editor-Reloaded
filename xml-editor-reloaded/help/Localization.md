# Localization overlays

Language overlays are `<Language>.<BaseFile>.xml` files in `TableData` (for example
`Map/Russian.Cities.xml`). They are not separate tables and do not appear in `INDEX.md`.

## Rules

- An overlay contains a key (`uiIndex`, `mIndex`, `pIndex`, `ubIndex`, `uiSlotIndex`,
  `SectorGrid`) and only the fields it overrides.
- The base table supplies the schema and the default values; a missing field is shown from the
  base with a "not translated" marker.
- A translation edit writes only the affected field to the overlay; the base file is never
  changed.
- Structural and non-translatable fields (`Enabled`, `Sector`, `popup`, `generator`, numeric
  stats) stay in the base.
- An overlay may be incomplete; validation reports missing, orphaned, duplicate and empty
  translations but never fills them automatically.
- Nested structures need their own keys: `CITY_INFO/CITYLIST/CITY` by `uiIndex`,
  `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT` by `ubAssignmentType`,
  `MERCGEARLIST/MERCGEAR/GEARKIT` by kit ordinal.

## v4 and v5

- v4 is Russian-localized; v5 is the English base. The absence of `Russian.*` in v5 is normal.
- The editor works with both data sets; the language of the UI is separate from the language
  of the data.

## In table documents

A table page lists only the fields that can appear in an overlay, under `## Localization`.
The overlay files themselves are not documented one by one; this page is the shared reference.

For the current pilot, `Map/Cities` is the only translated table: `Map/Russian.Cities.xml`
overrides `countryName`, `countryNoun` and `townName`, keyed by `uiIndex`; the `CITY_TABLE`
matrix and the numeric city fields stay in the base.
