# M32 translation guide

This file is the working contract for translating the English TableData help into
`xml-editor-reloaded/help/ru/`. Read `help/GLOSSARY.md` before translating; its approved Russian
terms are mandatory.

## Scope

- Translate every file under `xml-editor-reloaded/help/tables/` to the same relative path under
  `xml-editor-reloaded/help/ru/tables/`.
- Translate the top-level pages listed in `help/README.md` when assigned.
- Do not edit the English documents.
- Do not add or remove fields, tables, links, sections, or files.

## Front matter

Keep these keys byte-for-byte compatible with the English source:

- `schema`, `family`, `tables`, `table_ids`, `files`, `root`, `record`, `key`, `path_mode`;
- all table/file IDs and dotted names stay English.

Change only:

```yaml
lang: ru
title: "Russian title"
status: reviewed
updated: 2026-11-12
```

The `title` value is translated. `sources` entries may stay as technical English names; do not
change their meaning.

## Markdown structure

- Keep the level-one `#` heading aligned with the translated `title`; it may be Russian.
- Keep every level-two and level-three heading text exactly as in English (`## Purpose`,
  `## Tables`, `## Structure`, `## Fields`, `## Relations`, `## Localization`, `## Versions`,
  `## Sources`, `## Open questions`, and every `### \`FIELD/PATH\``). These headings define
  the anchors and the generated contents.
- Keep the generated `## Contents` block and its links unchanged unless you regenerate it.
- Keep field paths, XML element names, table names, file names, enum values, code identifiers,
  numbers, ranges, and Markdown link targets unchanged.
- Keep the ASCII `## Structure` tree unchanged, except that comments inside it may be translated.

## Field bullets

The generator reads the English bullet labels. Keep these labels exactly:

```md
- **Type:** ...
- **Description:** Russian text.
- **Numeric semantics:** Russian text.
- **Only used with:** Russian text.
- **Values:** Russian text, identifiers unchanged.
- **Default:** Russian text.
- **Source:** English technical source text, unchanged.
- **Confidence:** `source` / `comment` / `metadata` / `inferred` / `unknown`
```

Translate the values of `Description`, `Numeric semantics`, `Only used with`, `Values`, and
`Default`. Keep the values of `Type`, `Source`, and `Confidence` exactly equal to English.
Do not translate `Type`, `Source`, or `Confidence`.

## Prose and tables

- Translate prose, notes, warnings, examples, and table headers.
- Keep inline code, field names, dotted table names, XML tags, file names, and link targets.
- Preserve blank lines between field blocks and around headings.
- Do not introduce source-file or line references; use the existing general source wording.

## Validation

After a batch:

```sh
pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/help-check.ps1 -Lang ru
```

The structural checker verifies that every English document has a Russian counterpart, that
field paths and order match, that `Type`/`Source`/`Confidence` match, that headings/anchors and
links match, and that Russian documents have no extra fields. Fix structural errors before
finishing.
