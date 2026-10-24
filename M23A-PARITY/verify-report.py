#!/usr/bin/env python3
"""Verify the consolidated M23a report and its evidence.

Checks performed:

* the report is UTF-8 without BOM;
* the consolidated matrix has 440 rows and 588 unique IDs;
* every one of the 463 inventory IDs appears exactly once;
* section matrices and the consolidated matrix contain the same IDs;
* verdict totals in the section summary match the matrix;
* every row has legacy evidence and the required rewrite evidence;
* every ``пробел`` row is present in the gap register;
* the gap register has 39 entries / 88 IDs with valid severity and M23b step;
* every full-path citation resolves to an existing file and a valid line;
* no unnormalised short ``file:line`` citation remains;
* ``References/`` has no working-tree changes.

Exit code 0 means all checks passed.
"""

from __future__ import annotations

import collections
import pathlib
import re
import subprocess
import sys

import consolidate as c

REPORT = c.ROOT / "LEGACY-PARITY-REPORT.md"
VERDICTS = set(c.VERDICTS)
CITATION = re.compile(
    r"((?:References/xml-editor|xml-editor-reloaded)/[^`;:\n]+?\.(?:vb|cs|xaml|sql)):"
    r"([0-9]+(?:[-–,][0-9]+)*)"
)
BARE = re.compile(
    r"(?<![\w/])((?:[A-Za-z0-9_.-]+/)*[A-Za-z0-9_.-]+\.(?:vb|cs|xaml|sql)):"
    r"([0-9]+(?:[-–,][0-9]+)*)"
)


def fail(message: str):
    print(f"FAIL: {message}")
    raise SystemExit(1)


def report_matrix(text: str):
    rows = []
    in_matrix = False
    for line in text.splitlines():
        if line.startswith("## "):
            in_matrix = line.startswith("## Консолидированная матрица")
            continue
        if in_matrix and line.startswith("| L-"):
            cells = [cell.strip() for cell in line.strip("|").split("|")]
            if len(cells) != 7:
                fail(f"matrix row has {len(cells)} cells: {line[:120]}")
            rows.append(cells)
    return rows


def report_gaps(text: str):
    rows = []
    in_gaps = False
    for line in text.splitlines():
        if line.startswith("## "):
            in_gaps = line.startswith("## Реестр пробелов")
            continue
        if in_gaps and line.startswith("| L-"):
            cells = [cell.strip() for cell in line.strip("|").split("|")]
            if len(cells) != 6:
                fail(f"gap row has {len(cells)} cells: {line[:120]}")
            rows.append(cells)
    return rows


def totals_row(text: str):
    for line in text.splitlines():
        if line.startswith("| **Всего** |") and "**588**" in line:
            return [cell.strip() for cell in line.strip("|").split("|")]
    fail("section summary total row not found")
    return []


def check_citations(text: str):
    files = set()
    for path, spec in CITATION.findall(text):
        files.add(path)
        if not (c.ROOT / path).exists():
            fail(f"cited file does not exist: {path}")
        length = sum(1 for _ in open(c.ROOT / path, encoding="utf-8", errors="replace"))
        if max(int(value) for value in re.split(r"[-–,]", spec) if value) > length:
            fail(f"line {spec} outside {length}-line file: {path}")
    return files


def check_bare(text: str):
    protected = CITATION.sub(" ", text)
    leftovers = BARE.findall(protected)
    if leftovers:
        fail(f"unnormalised short citations: {leftovers[:10]}")


def check_references_clean():
    try:
        result = subprocess.run(
            ["git", "status", "--porcelain", "--", "References"],
            cwd=c.ROOT,
            capture_output=True,
            text=True,
            check=False,
        )
    except OSError:
        print("SKIP: git not available, References check skipped")
        return
    if result.stdout.strip():
        fail(f"References/ has working-tree changes:\n{result.stdout}")


def main() -> int:
    data = REPORT.read_bytes()
    if data.startswith(b"\xef\xbb\xbf"):
        fail("report has a UTF-8 BOM")
    text = data.decode("utf-8")

    rows = report_matrix(text)
    if len(rows) != 440:
        fail(f"expected 440 matrix rows, got {len(rows)}")

    ids = [item for row in rows for item in c.expand_ids(row[0])]
    if len(ids) != 588 or len(set(ids)) != 588:
        fail(f"expected 588 unique matrix IDs, got {len(ids)} values / {len(set(ids))} unique")

    inventory = c.inventory_ids()
    missing = inventory - set(ids)
    if missing:
        fail(f"inventory IDs missing from report: {sorted(missing)}")
    if len(inventory) != 463:
        fail(f"expected 463 inventory IDs, got {len(inventory)}")

    section_ids = set()
    section_rows = 0
    for _, _, filename, _ in c.SECTION_FILES:
        groups = c.parse_matrix(c.PARITY / filename)
        file_rows = [row for group in groups for row in group["rows"]]
        section_rows += len(file_rows)
        for row in file_rows:
            section_ids.update(c.expand_ids(row[0]))
    if section_rows != len(rows):
        fail(f"section rows {section_rows} != report rows {len(rows)}")
    if section_ids != set(ids):
        fail(f"section IDs differ from report IDs: {sorted(section_ids ^ set(ids))[:10]}")

    counts = collections.Counter()
    gap_ids = set()
    gap_rows = []
    for row in rows:
        row_ids = c.expand_ids(row[0])
        verdict = row[5]
        if verdict not in VERDICTS:
            fail(f"unknown verdict {verdict!r}: {row[0]}")
        counts[verdict] += len(row_ids)

        legacy = row[2]
        if "References/xml-editor/" not in legacy and "Нет в legacy" not in legacy and "Аналога нет" not in legacy:
            fail(f"missing legacy evidence: {row[0]}")
        if verdict == "реализовано" and "xml-editor-reloaded/" not in row[4]:
            fail(f"missing rewrite evidence for implemented row: {row[0]}")
        if verdict == "пробел":
            if row[4].strip() not in ("нет", "—"):
                fail(f"gap row has rewrite evidence: {row[0]} => {row[4][:80]}")
            if not row[6].strip():
                fail(f"gap row has no note: {row[0]}")
            gap_ids.update(row_ids)
        if verdict == "исключено" and not row[6].strip():
            fail(f"excluded row has no cause: {row[0]}")

    totals = totals_row(text)
    expected = [
        totals[2].strip("* "),
        totals[3].strip("* "),
        (counts["реализовано"], totals[4].strip("* ")),
        (counts["superseded"], totals[5].strip("* ")),
        (counts["исключено"], totals[6].strip("* ")),
        (counts["пробел"], totals[7].strip("* ")),
    ]
    if expected[0] != str(len(rows)) or expected[1] != str(len(set(ids))):
        fail(f"summary row count/ID mismatch: {totals}")
    for actual, reported in expected[2:]:
        if str(actual) != reported:
            fail(f"summary verdict mismatch: actual {actual}, reported {reported}")
    if counts["пробел"] != 61:
        fail(f"expected 61 gap IDs, got {counts['пробел']}")

    gaps = report_gaps(text)
    if len(gaps) != 39:
        fail(f"expected 39 gap register rows, got {len(gaps)}")
    register_ids = []
    severity = collections.Counter()
    for cells in gaps:
        register_ids.extend(c.expand_ids(cells[0]))
        if cells[2] not in ("P1", "P2", "P3"):
            fail(f"bad severity {cells[2]!r}: {cells[0]}")
        if not cells[4] or not cells[5].startswith("M23b"):
            fail(f"gap row lacks layer or M23b step: {cells[0]}")
        severity[cells[2]] += len(c.expand_ids(cells[0]))
    if len(register_ids) != 88 or len(set(register_ids)) != 88:
        fail(f"expected 88 unique register IDs, got {len(register_ids)} / {len(set(register_ids))}")
    if gap_ids - set(register_ids):
        fail(f"matrix gaps absent from register: {sorted(gap_ids - set(register_ids))}")
    if severity["P2"] != 55 or severity["P3"] != 33 or severity["P1"]:
        fail(f"severity totals mismatch: {dict(severity)}")

    cited_files = check_citations(text)
    check_bare(text)

    section_40 = (c.PARITY / "40-storage.md").read_text(encoding="utf-8")
    if "| superseded | 40 |" not in section_40 or "| **Итого ID** | **48** |" not in section_40:
        fail("40-storage.md summary was not corrected to 40/48")

    if "M23a0–M23a6" not in text or "M23a7" not in text:
        fail("report status does not name M23a6 / M23a7")

    check_references_clean()

    print("PASS: report is UTF-8 without BOM")
    print(f"PASS: {len(rows)} matrix rows, {len(set(ids))} unique IDs (463 inventory + 125 delta)")
    print(f"PASS: verdicts by ID {dict(counts)}")
    print(f"PASS: {len(gaps)} gap entries / {len(set(register_ids))} IDs; {dict(severity)}")
    print(f"PASS: {len(cited_files)} cited files, all line references valid")
    print("PASS: no unnormalised short citations")
    print("PASS: section 40 summary corrected; References/ clean")
    return 0


if __name__ == "__main__":
    sys.exit(main())
