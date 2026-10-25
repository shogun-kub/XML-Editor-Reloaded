---
name: ja2-v4-v5-data-facts
description: v4/v5 TableData comparison — same tag sets, different data, sparse
  records, mixed BOM/EOL, one malformed lookup file
metadata:
  node_type: memory
  type: project
  originSessionId: sess_9123e0d2-50a3-44a1-ac9f-49aedbc18523
---

Measured on 2026-10-24 across `References/JA 2 1.13 v4` and `v5`, `Data-1.13/TableData`:

- v4: 264 files (263 XML + 1 readme), 25 `Russian.*` localized files. v5: 240 files (239 XML), no Russian files, adds `Email/EmailSenderNameList.xml`.
- **v4 is the Russian build, v5 the English base — intentional.** The `Russian.*` files are overlays, not tables; see [[ja2-localization-overlay-model]].
- Of 239 shared XML files: 71 byte-identical, 168 differ; **all share identical XML tag sets** (same columns).
- Record counts differ only in `Items/Items.xml` (1,776 → 1,775), `Items/Weapons.xml` (389 → 388), `Items/Attachments.xml` (6,079 → 6,023).
- **Records are sparse:** `Items.xml` has 1,776 records but 640 distinct field sets (233 elements in the first record, 224 distinct tags overall). `Weapons.xml` has 48 union tags but 70 field sets.
- Values: floats stored as text (`1.3`, `17.5`, `-0.2`, `16.56`); many leaf elements are empty.
- Encoding: v4 has 162 BOM files, mixed 91 CRLF / 172 LF; v5 has 159 BOM, all 239 CRLF.
- **v5 `Lookup/AttachmentPoint.xml` is malformed** — extra `</AttachmentPoint>` at line 31; v4's copy is valid.
- **Fire Extinguisher sound anomaly (found 2026-10-25):** item 1761 in both corpora stores `sBurstSound=505` and `sSilencedBurstSound=505`, but `Sounds/BurstSounds.xml` has only 128 records (0–127). 505 is valid in `Sounds/Sounds.xml` (506 rows, fireextinguisher.wav), so the burst fields reuse a SOUND index. Legacy BURSTSOUND combos showed no match; the rewrite displays `505 (unknown)` and `RealRelationTests` allows exactly these 2 cases, so a data fix forces review.
- **Lookup IDs are 64-bit:** `Lookup/ItemFlag.xml` reaches `8589934592` (also `4294967296`), `Lookup/ItemClass.xml` reaches `1073741824`; `LookupEntry.Id` must be `long`/`ulong`, not `int`. Item field flag values themselves fit `uint` (max `usItemClass` 1073741824, `AttachmentClass` 33554432, `usActionItemFlag` 32768).
- Engine data (`gamedir/Base/TableData`, 277 XML) differs from v5 in 17 shared files' tag sets, e.g. v5 has `szNWSSSound`, `lbeCombo`, `GrowthModifier*`; engine data has `Detonator`/`RemoteDetonator`, `bEvolution`.
- Images: `BigItems` 1,568 STI (v4) / 1,567 (v5, lacks `GUN375.sti`), names mix case and 1–3 digit indices (`GUN00.STI`, `Gun375.sti`, `P1ITEM100.STI`) → look up case-insensitively. `Interface` 25 files (13 PNG), `tilesets/0` 4 STI.

**Why:** Any parser/saver assumption about column sets, types, or formatting is wrong for at least one data set; this is the evidence behind [[xml-integrity-is-mandatory]].

**How to apply:** Drive loading/saving from metadata plus the document itself, never from hardcoded v4/v5 field lists; test round-trip against both versions and expect a deliberate parse error on the v5 file. Details in [[reference-projects-structure-doc]].
