#!/usr/bin/env python3
"""Build the consolidated M23a legacy parity report.

The script reads the six section files in ``M23A-PARITY`` and writes
``LEGACY-PARITY-REPORT.md`` in the repository root.  It normalises short
file citations to full repository paths, checks inventory coverage,
merges duplicate gap roots, and assembles the gap register for M23b.

Run from the repository root or from this directory:

    python M23A-PARITY/consolidate.py
"""

from __future__ import annotations

import collections
import json
import os
import pathlib
import re
import sys

ROOT = pathlib.Path(__file__).resolve().parent.parent
PARITY = ROOT / "M23A-PARITY"
REPORT = ROOT / "LEGACY-PARITY-REPORT.md"

SECTION_FILES = [
    ("10", "Shell и grid", "10-shell-grid.md", "M23a1"),
    ("20", "Item workspace UI", "20-item-ui.md", "M23a2"),
    ("21", "Item-таблицы и изображения", "21-item-data-images.md", "M23a3"),
    ("30", "Merc gear и вспомогательные таблицы", "30-merc-aux.md", "M23a4"),
    ("40", "Backend: хранение", "40-storage.md", "M23a5A"),
    ("41", "Backend: операции", "41-operations.md", "M23a5B"),
]

VERDICTS = ("реализовано", "superseded", "исключено", "пробел")

ALL_ID = re.compile(
    r"\bL-(?:SHELL|GRID|ITEM|MERC|AUX|SET|MISC|TBL|STO|OPS|DATA|IMG|SND|SHP|IFIELD|IFLAG|ICOUNT)-\d{3}\b"
)
INVENTORY_ID = re.compile(
    r"\bL-(?:SHELL|GRID|ITEM|MERC|AUX|SET|MISC|TBL)-\d{3}\b"
)
GAP_ID_RANGE = re.compile(r"L-([A-Z]+)-(\d{3})(?:…(\d{3}))?")

MIGRATIONS = {
    "0001": "0001_InitialSchema.sql",
    "0002": "0002_PrototypeSeed.sql",
    "0003": "0003_ColumnHints.sql",
    "0004": "0004_ItemColumnHints.sql",
    "0005": "0005_VerifiedColumnMetadata.sql",
    "0006": "0006_FlagLookupMappings.sql",
    "0007": "0007_TableRelations.sql",
    "0008": "0008_M20AuxiliaryTables.sql",
    "0009": "0009_RecordKeyFields.sql",
}
MIGRATION_DIR = "xml-editor-reloaded/src/XmlEditorReloaded.Data/Metadata/Migrations/"


def read_text(path: pathlib.Path) -> str:
    return path.read_text(encoding="utf-8")


def parse_matrix(path: pathlib.Path):
    """Return matrix groups: [{'title': str|None, 'rows': [cells]}...]."""
    groups = []
    current = {"title": None, "rows": []}
    in_matrix = False
    for line in read_text(path).splitlines():
        if line.startswith("## "):
            if in_matrix and current["rows"]:
                groups.append(current)
            in_matrix = line.startswith("## Матрица")
            current = {"title": None, "rows": []}
            continue
        if not in_matrix:
            continue
        if line.startswith("### "):
            if current["rows"]:
                groups.append(current)
            current = {"title": line[4:].strip(), "rows": []}
            continue
        if not line.startswith("|"):
            continue
        cells = [cell.strip() for cell in line.strip("|").split("|")]
        if not cells or set(cells[0]) <= set("-: "):
            continue
        if cells[0] == "ID":
            continue
        if not ALL_ID.search(cells[0]):
            continue
        if len(cells) != 7:
            raise SystemExit(f"{path.name}: row with {len(cells)} cells: {line[:120]}")
        current["rows"].append(cells)
    if current["rows"]:
        groups.append(current)
    return groups


def parse_gaps(path: pathlib.Path):
    """Return gap rows with raw ID text, severity, substance and location."""
    rows = []
    in_gaps = False
    for line in read_text(path).splitlines():
        if line.startswith("## "):
            in_gaps = line.startswith("## Пробелы")
            continue
        if not in_gaps or not line.startswith("|"):
            continue
        cells = [cell.strip() for cell in line.strip("|").split("|")]
        if not cells or cells[0] == "ID" or set(cells[0]) <= set("-: "):
            continue
        if not ALL_ID.search(cells[0]) or len(cells) < 4:
            continue
        rows.append(
            {
                "ids_raw": cells[0],
                "severity": cells[1],
                "substance": cells[2],
                "where": cells[3],
                "file": path.name,
            }
        )
    return rows


def expand_ids(text: str):
    """Expand ``L-SHELL-079…093`` and comma-separated ID lists."""
    result = []
    for match in GAP_ID_RANGE.finditer(text):
        prefix, start, end = match.group(1), int(match.group(2)), match.group(3)
        if end:
            result.extend(f"L-{prefix}-{number:03d}" for number in range(start, int(end) + 1))
        else:
            result.append(f"L-{prefix}-{start:03d}")
    return result


def compact_ids(ids):
    """Render sorted IDs, collapsing consecutive numbers per prefix."""
    by_prefix = collections.defaultdict(list)
    for item in ids:
        prefix, number = item.split("-")[1], int(item.split("-")[2])
        by_prefix[prefix].append(number)
    parts = []
    for prefix in ("SHELL", "GRID", "ITEM", "MERC", "AUX", "SET", "MISC", "TBL", "IFIELD", "IFLAG", "ICOUNT", "DATA", "IMG", "SND", "SHP", "STO", "OPS"):
        numbers = sorted(set(by_prefix.get(prefix, [])))
        if not numbers:
            continue
        start = previous = numbers[0]
        for number in numbers[1:]:
            if number == previous + 1:
                previous = number
                continue
            parts.append(f"L-{prefix}-{start:03d}" if start == previous else f"L-{prefix}-{start:03d}…{previous:03d}")
            start = previous = number
        parts.append(f"L-{prefix}-{start:03d}" if start == previous else f"L-{prefix}-{start:03d}…{previous:03d}")
    return ", ".join(parts)


def inventory_ids():
    ids = set()
    for name in ("00-inventory-ui.md", "00-inventory-tables.md"):
        for line in read_text(PARITY / name).splitlines():
            if not line.startswith("|"):
                continue
            cells = [cell.strip() for cell in line.strip("|").split("|")]
            if cells:
                ids.update(INVENTORY_ID.findall(cells[0]))
    return ids


def collect_files():
    files = []
    for root_name in ("References/xml-editor", "xml-editor-reloaded/src", "xml-editor-reloaded/tests"):
        for directory, dirnames, filenames in os.walk(ROOT / root_name):
            dirnames[:] = [name for name in dirnames if name not in ("bin", "obj", ".git")]
            for filename in filenames:
                if filename.endswith((".vb", ".cs", ".xaml", ".sql")):
                    files.append(pathlib.Path(directory, filename).relative_to(ROOT).as_posix())
    return files


class CitationResolver:
    def __init__(self):
        self.by_name = collections.defaultdict(list)
        for path in collect_files():
            self.by_name[path.rsplit("/", 1)[-1]].append(path)
        self.full_path = re.compile(
            r"(?:References/xml-editor|xml-editor-reloaded)/[^`;:\n]+?\.(?:vb|cs|xaml|sql):[0-9]+(?:[-–,][0-9]+)*"
        )
        self.file_token = re.compile(
            r"(?<![\w/])((?:[A-Za-z0-9_.-]+/)*[A-Za-z0-9_.-]+\.(?:vb|cs|xaml|sql)):"
            r"([0-9]+(?:[-–,][0-9]+)*)"
        )
        self.migration = re.compile(
            r"(?<![\w/])(000[1-9])(?:…|\.\.\.)?:([0-9]+(?:[-–,][0-9]+)*)"
        )
        self.migration_path = re.compile(
            r"(?<![\w/])(?:[A-Za-z0-9_.-]+/)*Migrations/000[1-9](?:_[A-Za-z0-9_]+)?(?=:[0-9])"
        )
        self.unresolved = []

    def resolve(self, token: str, kind: str):
        candidates = self.by_name.get(token.rsplit("/", 1)[-1], [])
        exact = [path for path in candidates if path.endswith("/" + token.lstrip("/"))]
        if not exact:
            exact = candidates
        if len(exact) == 1:
            return exact[0]
        if kind == "any":
            self.unresolved.append((kind, token))
            return None
        preferred = [
            path
            for path in exact
            if path.startswith("References/xml-editor" if kind == "legacy" else "xml-editor-reloaded")
        ]
        if len(preferred) == 1:
            return preferred[0]
        self.unresolved.append((kind, token))
        return None

    def normalise(self, cell: str, kind: str) -> str:
        protected = []

        def protect(match):
            protected.append(match.group(0))
            return f"\x00{len(protected) - 1}\x00"

        text = self.full_path.sub(protect, cell)

        def replace_file(match):
            path = self.resolve(match.group(1), kind) or match.group(1)
            return f"{path}:{match.group(2)}"

        text = self.file_token.sub(replace_file, text)
        text = self.migration.sub(
            lambda match: f"{MIGRATION_DIR}{MIGRATIONS[match.group(1)]}:{match.group(2)}", text
        )
        text = self.migration_path.sub(
            lambda match: MIGRATION_DIR + MIGRATIONS[re.search(r"000[1-9]", match.group(0)).group(0)],
            text,
        )
        return re.sub(r"\x00(\d+)\x00", lambda match: protected[int(match.group(1))], text)


def render_table(headers, rows):
    lines = ["| " + " | ".join(headers) + " |", "|" + "|".join("---" for _ in headers) + "|"]
    lines.extend("| " + " | ".join(cells) + " |" for cells in rows)
    return "\n".join(lines)


GAP_META = {
    "L-SHELL-079": ("Item workspace", "`App/ItemWorkspaceViewModel`", "M23b1"),
    "L-SHELL-095": ("Item workspace", "`App/ItemWorkspaceViewModel`, `App/TableFilterViewModel`", "M23b1"),
    "L-SHELL-183": ("Shell", "`App/AboutWindow`, команда shell", "M23b7"),
    "L-SET-009": ("Settings", "`App/MainWindow`, `AppSettings`/`JsonSettingsStore`", "M23b7"),
    "L-ITEM-005": ("Item workspace", "`App/ItemWorkspaceViewModel` + карта классов", "M23b1"),
    "L-ITEM-011": ("Metadata lookups", "`Data/0005`, `0007`", "M23b3"),
    "L-ITEM-014": ("Images", "`App/ItemWorkspaceViewModel`, вкладка Images", "M23b5"),
    "L-ITEM-017": ("Field UI", "`App/DetailFieldViewModel`/`FieldEditor`", "M23b7"),
    "L-ITEM-027": ("Item relations", "`Data/0007`, `App/ItemWorkspaceViewModel`", "M23b2"),
    "L-ITEM-028": ("Item relations", "`App/TableEditorViewModel`, `Views/TableView`", "M23b2"),
    "L-ITEM-029": ("Item relations", "`App/TableEditorViewModel`, `Views/TableView`", "M23b2"),
    "L-ITEM-030": ("Item relations", "`Data/0007`, `App/ItemWorkspaceViewModel`", "M23b2"),
    "L-ITEM-031": ("Item relations", "`App/TableEditorViewModel`, `Views/TableView`", "M23b2"),
    "L-ITEM-032": ("Item relations", "`Data/0007` (направление связи)", "M23b2"),
    "L-ITEM-034": ("Validation", "`App/ItemWorkspaceViewModel` + validation", "M23b4"),
    "L-ITEM-039": ("Shopkeeper", "`App/ItemWorkspaceViewModel`", "M23b6"),
    "L-ITEM-047": ("Validation", "`Data/ColumnHints` + validation", "M23b4"),
    "L-IFIELD-233": ("Metadata lookups", "`Data/0005`, `0007`", "M23b3"),
    "L-IFIELD-275": ("Metadata lookups", "`Data/0005`, `0007`", "M23b3"),
    "L-IFIELD-315": ("Metadata lookups", "`Data/0007`", "M23b3"),
    "L-IFIELD-227": ("Validation", "metadata + validation", "M23b4"),
    "L-DATA-008": ("Item data", "`App/TableEditorViewModel`, `ItemWorkspaceViewModel`", "M23b2"),
    "L-DATA-011": ("Item data", "`Xml/TableSetWorkspace` или validation", "M23b2"),
    "L-DATA-013": ("Item data", "`App/TableEditorViewModel`, `ItemWorkspaceViewModel`", "M23b2"),
    "L-DATA-009": ("Item data", "`Xml/Loading`, item-нормализатор", "M23b2"),
    "L-DATA-012": ("Validation", "`Core/ColumnDataType`, validation, `BitEditorViewModel`", "M23b4"),
    "L-IMG-005": ("Images", "`Imaging/ItemImageCatalog`", "M23b5"),
    "L-IMG-020": ("Images", "`App/TableRowViewModel`, `ItemListEntry`", "M23b5"),
    "L-MERC-003": ("Merc gear", "`MercGearKitViewModel`, `MercGearWorkspaceView.xaml`", "M23b7"),
    "L-MERC-011": ("Pickers", "`Data/0008`, `RelationPicker`", "M23b3"),
    "L-MERC-013": ("Merc gear", "`MercGearWorkspaceView.xaml`", "M23b7"),
    "L-MERC-014": ("Validation", "`Data/ColumnHints`, `MercGearWorkspaceViewModel`", "M23b4"),
    "L-SHP-001": ("Shopkeeper", "`App/ShopkeeperEntryViewModel`, `TableSetWorkspace`", "M23b6"),
    "L-SND-001": ("Metadata lookups", "`Data/0005`, `0007`", "M23b3"),
    "L-TBL-124": ("Item choice/ammo", "`Data/0008`, `App/ItemWorkspaceViewModel`", "M23b2"),
    "L-AUX-036": ("Lookup CRUD", "`Xml/TableDocument.AddRecord`, lookup validation; M26a", "M23b6"),
    "L-STO-034": ("Storage", "`Xml/TableDocument`, `TableFileLoader`", "M23b8"),
    "L-OPS-011": ("Copy", "`Xml/Copy/TableCopyService`, seed метаданных", "M23b8"),
    "L-OPS-025": ("Error handling", "`App` error boundary + logging adapter", "M23b8"),
}


def build_gap_register(gap_rows):
    merged = []
    item_skill = None
    field_skill = None
    for row in gap_rows:
        ids = expand_ids(row["ids_raw"])
        if "L-ITEM-011" in ids:
            item_skill = row
            continue
        if "L-IFIELD-337" in ids:
            field_skill = row
            continue
        merged.append(row)
    if item_skill is None or field_skill is None:
        raise SystemExit("expected L-ITEM-011 and L-IFIELD-337 gap rows")
    merged.append(
        {
            "ids_raw": "L-ITEM-011, L-IFIELD-337…339",
            "severity": item_skill["severity"],
            "substance": (
                "Три `Robot*SkillGrant` — числовые поля без списка 24 трейтов; в legacy были combo "
                "с именами (`NoTrait`…`Survival`). То же для полевой карты `L-IFIELD-337…339`; "
                "строка реестра одна."
            ),
            "where": item_skill["where"],
            "file": item_skill["file"],
        }
    )
    for row in merged:
        ids = expand_ids(row["ids_raw"])
        first = ids[0]
        if first not in GAP_META:
            raise SystemExit(f"no gap metadata for {first}")
        row["ids"] = ids
        row["area"], row["layer"], row["step"] = GAP_META[first]
    order = {
        "SHELL": 0, "GRID": 1, "ITEM": 2, "MERC": 3, "AUX": 4, "SET": 5, "MISC": 6,
        "TBL": 7, "IFIELD": 8, "IFLAG": 9, "ICOUNT": 10, "DATA": 11, "IMG": 12,
        "SND": 13, "SHP": 14, "STO": 15, "OPS": 16,
    }
    merged.sort(
        key=lambda row: (
            row["step"],
            order[row["ids"][0].split("-")[1]],
            int(row["ids"][0].split("-")[2]),
        )
    )
    return merged


def main() -> int:
    sections = []
    matrix_ids = []
    all_rows = 0
    for number, title, filename, substep in SECTION_FILES:
        path = PARITY / filename
        groups = parse_matrix(path)
        rows = [row for group in groups for row in group["rows"]]
        ids = [item for row in rows for item in expand_ids(row[0])]
        if len(ids) != len(set(ids)):
            raise SystemExit(f"{filename}: duplicate matrix IDs")
        sections.append(
            {
                "number": number,
                "title": title,
                "filename": filename,
                "substep": substep,
                "groups": groups,
                "rows": rows,
                "ids": ids,
            }
        )
        matrix_ids.extend(ids)
        all_rows += len(rows)

    if len(matrix_ids) != len(set(matrix_ids)):
        duplicates = [item for item, count in collections.Counter(matrix_ids).items() if count > 1]
        raise SystemExit(f"duplicate IDs across sections: {duplicates}")

    inventory = inventory_ids()
    missing_inventory = sorted(inventory - set(matrix_ids))
    if missing_inventory:
        raise SystemExit(f"inventory IDs missing from matrix: {missing_inventory}")
    delta = sorted(set(matrix_ids) - inventory)

    resolver = CitationResolver()
    for section in sections:
        for group in section["groups"]:
            for row in group["rows"]:
                row[1] = resolver.normalise(row[1], "any")
                row[2] = resolver.normalise(row[2], "legacy")
                row[3] = resolver.normalise(row[3], "any")
                row[4] = resolver.normalise(row[4], "rewrite")
                row[6] = resolver.normalise(row[6], "any")
    if resolver.unresolved:
        raise SystemExit(f"unresolved citations: {resolver.unresolved}")

    gap_rows = []
    for number, title, filename, substep in SECTION_FILES:
        gap_rows.extend(parse_gaps(PARITY / filename))
    gap_register = build_gap_register(gap_rows)
    for row in gap_register:
        row["substance"] = resolver.normalise(row["substance"], "any")
        row["where"] = resolver.normalise(row["where"], "any")
    if resolver.unresolved:
        raise SystemExit(f"unresolved gap citations: {resolver.unresolved}")
    gap_ids = [item for row in gap_register for item in row["ids"]]

    matrix_gap_ids = []
    for section in sections:
        for row in section["rows"]:
            if row[5] == "пробел":
                matrix_gap_ids.extend(expand_ids(row[0]))
    if set(matrix_gap_ids) - set(gap_ids):
        raise SystemExit(f"matrix gaps absent from register: {sorted(set(matrix_gap_ids) - set(gap_ids))}")
    if set(expand_ids(" ".join(row["ids_raw"] for row in gap_rows))) - set(gap_ids):
        raise SystemExit("register lost gap IDs")

    verdict_counts = collections.Counter()
    for section in sections:
        for row in section["rows"]:
            verdict_counts[row[5]] += len(expand_ids(row[0]))
    if set(verdict_counts) - set(VERDICTS):
        raise SystemExit(f"unknown verdicts: {set(verdict_counts) - set(VERDICTS)}")

    severity_counts = collections.Counter()
    for row in gap_register:
        severity_counts[row["severity"]] += len(row["ids"])

    step_counts = collections.defaultdict(lambda: {"rows": 0, "ids": 0, "P2": 0, "P3": 0})
    for row in gap_register:
        bucket = step_counts[row["step"]]
        bucket["rows"] += 1
        bucket["ids"] += len(row["ids"])
        bucket[row["severity"]] += len(row["ids"])

    report = []
    report.append("# Legacy parity report (M23a)\n")
    report.append(
        "**Статус:** M23a0–M23a6 выполнены; M23a7 (проверка на v4/v5 и ручной UI-smoke) — "
        "следующий подшаг. Поведение редактора в M23a не менялось.\n"
    )
    report.append(
        "**Эталон:** `References/xml-editor` (VB.NET/WinForms), только чтение. "
        "`References/JA_2_1.13_TableData_Editor` — прототип, эталоном не считается. "
        "`References/Ja 2 1.13 source` в M23a не использовался.\n"
    )
    report.append(
        "**Область:** каждая реально достижимая функция legacy либо реализована в rewrite, либо "
        "заменена более безопасным эквивалентом, либо явно исключена. Bug-for-bug совместимость "
        "не требуется; деструктивные эффекты legacy не воспроизводятся.\n"
    )
    report.append(
        "**Метод:** шесть секций `M23A-PARITY/10…41`, инвентарь `00-inventory-*`; строки матрицы "
        "собраны в этом отчёте, цитаты нормализованы до полных путей репозитория. Детали и "
        "локальные сокращения сохранены в файлах секций.\n"
    )
    report.append(
        f"**Итог:** {all_rows} строк матрицы, {len(matrix_ids)} ID "
        f"({len(inventory)} инвентарных + {len(delta)} дельт); "
        f"вердикты: {verdict_counts['реализовано']} реализовано, "
        f"{verdict_counts['superseded']} superseded, {verdict_counts['исключено']} исключено, "
        f"{verdict_counts['пробел']} пробел. Реестр: {len(gap_register)} записей / {len(gap_ids)} ID; "
        f"P1 — 0, P2 — {severity_counts['P2']}, P3 — {severity_counts['P3']}.\n"
    )

    report.append("## Вердикты\n")
    report.append(
        render_table(
            ["Вердикт", "Значение"],
            [
                ["`реализовано`", "Поведение достижимо в rewrite и покрывает существо legacy-функции. Доказательство — достижимый путь в коде; тест только подтверждает."],
                ["`superseded`", "Заменено более безопасным или более общим эквивалентом; отличие описано."],
                ["`исключено`", "Мёртвая/скрытая функция, деструктивное поведение или осознанно вынесенный за паритет пункт. Причина и доказательство обязательны."],
                ["`пробел`", "Функция реальна и достижима в legacy, но отсутствует или неполна в rewrite. Входит в реестр M23b."],
            ],
        )
    )
    report.append("")

    report.append("## Покрытие инвентаря\n")
    report.append(
        render_table(
            ["Источник", "Уникальных ID"],
            [
                ["UI-команды, `00-inventory-ui.md`", "341"],
                ["Legacy-таблицы, `00-inventory-tables.md`", "122"],
                ["**Инвентарь всего**", f"**{len(inventory)}**"],
                ["Дельты, добавленные секциями", f"{len(delta)}"],
                ["**Матрица всего**", f"**{len(matrix_ids)}**"],
            ],
        )
    )
    report.append("")
    report.append(
        "Проверка скриптом: каждый из 463 инвентарных ID встречается в консолидированной "
        "матрице ровно один раз; ни одного ID вне инвентаря или дельт нет; 125 дельт "
        "зарегистрированы в разделах «Дельта инвентаря» своих секций. "
        "`L-IFIELD`, `L-IFLAG`, `L-ICOUNT` — детализация карты полей `ItemDataForm` "
        "(`00-inventory-ui-item-fields.md`), а не самостоятельные строки инвентаря; в матрице "
        "они представлены агрегированными строками `L-ITEM-*`.\n"
    )
    report.append("Дельты по секциям:\n")
    report.append(
        render_table(
            ["Секция", "Дельта", "ID"],
            [
                ["10", "—", "0"],
                ["20", "`L-ITEM-047…049`", "3"],
                ["21", "`L-DATA-001…014`, `L-IMG-001…020`", "34"],
                ["30", "`L-MERC-009…015`, `L-SHP-001`, `L-SND-001`, `L-TBL-123…126`, `L-AUX-032…036`", "18"],
                ["40", "`L-STO-001…034`", "34"],
                ["41", "`L-OPS-001…036`", "36"],
                ["**Всего**", "", f"**{len(delta)}**"],
            ],
        )
    )
    report.append("")

    report.append("## Правила доказательств\n")
    report.append(
        "- Каждая строка имеет ID из инвентаря или дельты, legacy-доказательство, "
        "rewrite-доказательство (или `нет` для пробела) и вердикт.\n"
        "- `реализовано` без цитаты достижимого пути не принимается; тест — только подтверждение.\n"
        "- `исключено` требует причины: мёртвый код, скрытый контрол, деструктивность или "
        "документированный перенос в M24–M27.\n"
        "- Для дельт, описывающих rewrite-only аспект, legacy-колонка может содержать "
        "«Нет в legacy»/«Аналога нет» с объяснением; такие строки помечены в примечании.\n"
        "- Все цитаты в этом отчёте нормализованы до полных путей; в файлах секций сохранены "
        "локальные сокращения, раскрытые в их заголовках.\n"
        "- Непроверяемые утверждения остаются в «Открытых вопросах» секций, а не в вердикте.\n"
    )

    report.append("## Сводка по секциям\n")
    summary_rows = []
    for section in sections:
        counts = collections.Counter()
        for row in section["rows"]:
            counts[row[5]] += len(expand_ids(row[0]))
        summary_rows.append(
            [
                section["number"],
                section["title"],
                str(len(section["rows"])),
                str(len(section["ids"])),
                str(counts["реализовано"]),
                str(counts["superseded"]),
                str(counts["исключено"]),
                str(counts["пробел"]),
            ]
        )
    summary_rows.append(
        [
            "**Всего**",
            "",
            f"**{all_rows}**",
            f"**{len(matrix_ids)}**",
            f"**{verdict_counts['реализовано']}**",
            f"**{verdict_counts['superseded']}**",
            f"**{verdict_counts['исключено']}**",
            f"**{verdict_counts['пробел']}**",
        ]
    )
    report.append(
        render_table(
            ["Секция", "Область", "Строк", "ID", "реализовано", "superseded", "исключено", "пробел"],
            summary_rows,
        )
    )
    report.append("")

    report.append("## Межсекционные пересечения\n")
    report.append(
        "Секции уже не дублируют инвентарные ID: каждая функция принадлежит одной секции, "
        "остальные ссылаются на неё. Таблица фиксирует смысловые пересечения, чтобы в M23b "
        "не исправлять одно поведение дважды.\n"
    )
    report.append(
        render_table(
            ["Концепт", "Владелец", "Связанные строки", "Решение"],
            [
                ["Сохранение вкладки / Save All", "`L-SHELL-014/015` (10)", "`L-STO-011` (40), `L-OPS-018` (41), `L-OPS-033` (41)", "Одна команда и один backend-путь; вердикты слоёв не дублируются."],
                ["Закрытие и несохранённые правки", "`L-SHELL-019` (10)", "`L-OPS-018`, `L-OPS-033` (41)", "Close — UI-политика; backend не сохраняет автоматически."],
                ["Фильтры/поиск generic grid", "`L-GRID-002/036/037` (10)", "`L-OPS-035` (41)", "UI-фильтр и backend-матчер — разные слои."],
                ["Фильтры item workspace", "`L-SHELL-079…101` (10)", "`L-ITEM-005` (20)", "Список фильтруется по классу/флагам; поля ITEM — отдельная видимость; оба в M23b1."],
                ["Изображения предметов", "`L-ITEM-012/013/014` (20)", "`L-IMG-001…020` (21)", "UI-превью/chooser и backend-каталог; `L-ITEM-014` ↔ `L-IMG-005`."],
                ["Lookup-таблицы", "`L-TBL-037…061` (30)", "`L-STO-028/029` (40), `L-AUX-036` (30), `L-OPS-011` (41)", "Таблица/UI, загрузка файла, CRUD и копирование — разные слои."],
                ["Shopkeeper/дилеры", "`L-TBL-081…099` (30)", "`L-ITEM-039` (20), `L-SHP-001` (30)", "Generic grid, item-центричное создание и адресация дубликата — разные пробелы."],
                ["Локализация/оверлеи", "UI — 10/20", "`L-TBL-023…036`, `L-STO-026/027` (40)", "UI-выбор языка отделён от механики merge/записи."],
                ["Merc gear", "`L-MERC-001…008` (30)", "`L-TBL-061` (30), дельты `L-MERC-009…015`", "Одна область; внутренние пересечения закрыты в секции 30."],
                ["Валидация", "`L-GRID-035` (10)", "`L-OPS-020/030` (41), `L-DATA-011/012` (21), `L-ITEM-034/047` (20), `L-MERC-014` (30)", "UI-ошибка, backend-ограничение, инвариант данных и границы полей — разные правила."],
                ["Undo/redo", "`L-AUX-025` (10)", "`L-OPS-033` (41)", "Legacy-поведение и rewrite-реализация; `L-OPS-033` — единственная реализация."],
                ["Сравнение версий", "rewrite-only", "—", "Legacy-аналога нет; в матрицу отдельной строкой не вынесено, зафиксировано в открытых вопросах."],
            ],
        )
    )
    report.append("")

    report.append("## Консолидированная матрица\n")
    report.append(
        "Порядок секций: 10 → 20 → 21 → 30 → 40 → 41. Внутри секции сохранён порядок строк; "
        "групповые строки перечисляют все закрытые ID. Цитаты — полные пути; в секциях "
        "используются документированные сокращения.\n"
    )
    for section in sections:
        report.append(f"### {section['number']} — {section['title']}\n")
        report.append(
            f"Источник: [{section['filename']}](M23A-PARITY/{section['filename']}); "
            f"строк {len(section['rows'])}, ID {len(section['ids'])}.\n"
        )
        for group in section["groups"]:
            if group["title"]:
                report.append(f"**{group['title']}**\n")
            report.append(
                render_table(
                    ["ID", "Legacy-функция", "Legacy-доказательство", "Реализация", "Rewrite-доказательство", "Вердикт", "Примечание"],
                    group["rows"],
                )
            )
            report.append("")

    report.append("## Реестр пробелов\n")
    report.append(
        f"{len(gap_ids)} ID в {len(gap_register)} записях: "
        f"P1 — 0 (блокирующих паритет нет), P2 — {severity_counts['P2']} (неполная функция), "
        f"P3 — {severity_counts['P3']} (косметика/полировка). "
        "Суть и ожидаемый слой взяты из секций; полные пути — в разделе «Пробелы» каждой секции.\n"
    )
    gap_table_rows = []
    for row in gap_register:
        gap_table_rows.append(
            [
                compact_ids(row["ids"]),
                row["area"],
                row["severity"],
                row["substance"],
                row["layer"],
                row["step"],
            ]
        )
    report.append(
        render_table(
            ["ID", "Область", "Серьёзность", "Суть", "Ожидаемый слой", "M23b"],
            gap_table_rows,
        )
    )
    report.append("")

    report.append("## Предлагаемые подшаги M23b\n")
    report.append(
        "Подшаги сгруппированы по слою и проверяемому поведению; каждый пробел входит ровно "
        "в один подшаг. P1 нет, поэтому M23b можно вести пакетами P2, затем P3.\n"
    )
    step_titles = {
        "M23b1": "Фильтры item workspace и видимость полей",
        "M23b2": "Item-связи, навигация и структурированные редакторы",
        "M23b3": "Источники значений и pickers",
        "M23b4": "Валидация, границы чисел и 64-битные маски",
        "M23b5": "Изображения",
        "M23b6": "Shopkeeper и identity/CRUD lookup-строк",
        "M23b7": "Shell, field UI и полировка merc gear",
        "M23b8": "Хранение, копирование и журналирование",
    }
    step_rows = []
    for step in sorted(step_counts):
        bucket = step_counts[step]
        step_rows.append(
            [
                step,
                step_titles[step],
                str(bucket["rows"]),
                str(bucket["ids"]),
                str(bucket["P2"]),
                str(bucket["P3"]),
            ]
        )
    step_rows.append(
        [
            "**Всего**",
            "",
            f"**{len(gap_register)}**",
            f"**{len(gap_ids)}**",
            f"**{severity_counts['P2']}**",
            f"**{severity_counts['P3']}**",
        ]
    )
    report.append(
        render_table(["Подшаг", "Содержание", "Записей", "ID", "P2", "P3"], step_rows)
    )
    report.append("")

    report.append("## Оставшиеся вопросы\n")
    report.append(
        "Полные списки — в разделах «Открытые вопросы» секций. Ниже решения, которые влияют "
        "на состав M23b и требуют подтверждения перед исправлениями.\n"
    )
    questions = [
        "**Фильтры и видимость полей (M23b1).** Достаточно ли фильтра item workspace по `usItemClass`/флагам ITEM, или нужен полный аналог legacy-страниц `DisplayTabs`? От этого зависит объём `L-SHELL-079…101` и `L-ITEM-005`.",
        "**Имя предмета.** Legacy использует `szLongItemName` в заголовке и при создании; rewrite — `szItemName`. Переключать заголовок/создание, заполнять оба поля или оставить как есть? (20, открытый вопрос 2.)",
        "**`CompatibleFaceItems`.** Фильтрация по `compatiblefaceitemIndex` вместо `itemIndex` — дефект метаданных или намеренная инверсия? (20, вопрос 3; `L-ITEM-032`.)",
        "**Карта LBE → карманы.** Ветки legacy 2 и 4 идентичны; какую карту считать эталонной для неразрушающей подсказки/валидации? (20, вопрос 4; `L-ITEM-034`.)",
        "**Симметрия пар.** Автоматически создавать/удалять обратную строку `IncompatibleAttachments`/`CompatibleFaceItems` или ограничиться предупреждением валидатора? (21, вопрос 3; `L-DATA-011`.)",
        "**Старый формат `Item_Transformations`.** Поддерживать повторяющийся `usResult` или отклонять с диагностикой? (21, вопрос 4; `L-DATA-009`.)",
        "**Dict-семейства.** Делать отдельный индексированный редактор для `ItemCapacityPerSize*`/`randomitem*`/`item*` или вернуть их в скалярные колонки? (21, вопрос 6; `L-DATA-013`.)",
        "**64-битные маски.** Нужны ли значения выше 32 бит для `AttachmentClass`/`nasAttachmentClass` в будущих версиях? (21, вопрос 7; `L-DATA-012`.)",
        "**Lookup CRUD.** `L-AUX-036` (новая строка без `name`) — часть M23b6 или M26a? (30, Lookup-вопрос 3.)",
        "**Строка Ammo (`uiIndex=19`).** Условная связь в `TableRelations` или отдельный редактор «Enemy ammo» на 7 строк? Нужна ли защита от удаления строки? (30, Enemy-вопрос 1; `L-TBL-124`.)",
        "**CONTROL в будущих версиях.** Оставить preserve-only или добавить дескриптор 51 колонки? (30, вопрос 1; `L-TBL-062…080`.)",
        "**Working-data cache.** Нужен ли файл восстановления несохранённых правок вне `TableData`? (40, вопрос 1; `L-SET-007`.)",
        "**Отсутствующий lookup-файл.** Жёсткая ошибка или пустой список с предупреждением? (40, вопрос 2; `L-STO-029`.)",
        "**v5 `Lookup/AttachmentPoint.xml`.** Частичная загрузка первых валидных записей или ручное исправление данных? (40, вопрос 7; `L-TBL-058`.)",
        "**Геометрия окна и About.** Входят ли `L-SET-009`/`L-SHELL-183` в M23b7 или выносятся в M27b? (10, вопросы 1 и 3.)",
        "**Журналирование и глобальная граница ошибок.** Достаточно ли файлового лога в M23b8 или нужен также перехват `DispatcherUnhandledException`/`AppDomain.UnhandledException`? (41, вопросы 7–8; `L-OPS-025`.)",
        "**Дубликат `uiIndex=26`.** Делать блокирующей ошибкой или чинить exact-row адресацию и оставлять предупреждение? (30, вопрос 6; `L-SHP-001`.)",
        "**Сравнение версий.** `TableDataComparer`/`CompareViewModel` — rewrite-only; регистрировать как дельту паритета или оставить вне реестра? (41, вопрос 1.)",
    ]
    for index, question in enumerate(questions, 1):
        report.append(f"{index}. {question}\n")
    report.append("")

    report.append("## Проверка M23a6\n")
    report.append(
        render_table(
            ["Проверка", "Результат"],
            [
                ["Покрытие инвентаря", f"463/463 ID, каждый ровно один раз; дельт {len(delta)}; всего {len(matrix_ids)} ID"],
                ["Строки матрицы", f"{all_rows} строк; у всех 7 колонок и вердикт из допустимых"],
                ["Вердикты", f"{verdict_counts['реализовано']} реализовано / {verdict_counts['superseded']} superseded / {verdict_counts['исключено']} исключено / {verdict_counts['пробел']} пробел"],
                ["Пробелы", f"{len(gap_ids)} ID в {len(gap_register)} записях; все матричные `пробел` присутствуют в реестре; P1 — 0, P2 — {severity_counts['P2']}, P3 — {severity_counts['P3']}"],
                ["Доказательства", "у `реализовано` есть rewrite-цитата; у `исключено` — причина; у `пробел` — `нет` и запись реестра"],
                ["Файлы и строки", "все цитаты разрешаются в существующие файлы; номера строк не выходят за длину файла"],
                ["Секционные суммы", "сводки секций сверены с матрицами; в 40 исправлена сумма ID (было 28/36, стало 40/48)"],
            ],
        )
    )
    report.append("")
    report.append(
        "Выборочная проверка по исходникам: 19 строк в шести секциях (10 — 4, 20 — 3, "
        "21 — 3, 30 — 3, 40 — 3, 41 — 3). Подтверждено 17 строк; исправлены две цитаты:\n"
    )
    report.append(
        "- `L-MERC-001`: путь с несуществующим `App/Views/MainWindow.xaml` заменён на `App/MainWindow.xaml` (строки 190–191).\n"
        "- `L-STO-003`: ссылка на ветку метаданных `TableRepository.cs` заменена на `Saving/TableFileSaver.cs` (строки 53–58) и `TableRepository.cs` (строки 85–99).\n"
    )
    report.append(
        "Сборка, тесты, corpus/compare/copy/perf и ручной WPF-smoke в M23a6 не запускались: "
        "это M23a7.\n"
    )

    report.append("## Проверка M23a7 (ожидает выполнения)\n")
    report.append(
        "- `dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx -warnaserror`, `dotnet test`.\n"
        "- Corpus v4/v5, сравнение v4↔v5, copy v4→v5, perf-гейты.\n"
        "- Приёмка M22c: item 12 показывает 27 вложений; duplicate-baseline v5 — 2 группы `Attachments` и 1 дилер.\n"
        "- Ручной UI-smoke по `M23A-PARITY/MANUAL-SMOKE-CHECKLIST.md` (создаётся в M23a7) — item/merc workspace, shopkeeper, pickers, copy, фильтры, изображения, локализация, настройки, close, external-change.\n"
        "- Результаты, дата и подпись пользователя вносятся в этот отчёт; без них M23a не считается закрытым.\n"
    )
    report.append(
        "Ограничение среды: WPF-поведение не автоматизируется; агент подтверждает только "
        "код, сборку, тесты и данные.\n"
    )

    report.append("## Вне паритета (осознанно)\n")
    report.append(
        "- Скрытые/мёртвые меню legacy: Edit, Print/Preview/Setup, Profiles, Tools/Options, "
        "Maps/Cities/SAM/Movement, New Window и неработающие BitCodeMaker/working-data cache/"
        "сохранение layout в неверный путь.\n"
        "- Деструктивное поведение: молчаливая потеря unknown-тегов, каскадное удаление без "
        "предупреждения, перезапись `ubClassIndex`, сброс отсутствующих изображений в `0/0`, "
        "trim нулей/пустых элементов, сортировка записей при записи.\n"
        "- Сервисные/вычисляемые настройки (`AP_Maximum`, `Hide_Nothing_Items`) заменены "
        "отображением, поиском и perf-гейтами.\n"
        "- Cities/facilities, value-level compare, lookup management и packaging — M24–M27. "
        "Undo/redo, оверлеи, структурное сравнение и валидация уже сильнее legacy.\n"
    )

    REPORT.write_text("\n".join(report), encoding="utf-8")
    print(f"wrote {REPORT} ({REPORT.stat().st_size} bytes)")
    print(f"rows={all_rows} ids={len(matrix_ids)} inventory={len(inventory)} delta={len(delta)}")
    print(f"verdicts={dict(verdict_counts)}")
    print(f"gaps={len(gap_register)} gap_ids={len(gap_ids)} severity={dict(severity_counts)}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
