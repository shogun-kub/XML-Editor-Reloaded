---
name: m26a-lookup-ids
description: M26a — lookup value management (id allocation by lookup kind, free-form 64-bit id, undo, metadata flag).
metadata:
  node_type: memory
  type: project
---

# M26a — Идентификаторы значений справочников

**Вид справочника.** Миграция `0018_LookupKinds.sql` добавляет `Tables.IsFlagLookup` и помечает
`AmmoFlag`, `AttachmentClass`, `AttachmentPoint`, `DrugType`, `ItemClass`, `ItemFlag`,
`NasAttachmentClass`, `NasLayoutClass`. `TableMetadata.IsFlagLookup` (только при `IsLookup`) читает
`SqliteMetadataProvider`; ни определение по колонкам `Flags`, ни эвристика по значениям не
используются — у `Lookup.ItemFlag` нет ссылающейся колонки, а `AmmoFlag` хранит 0–4, хотя данные уже
используют биты 8 и 16.

**Распределение.** `LookupIdAllocator` (`Xml/Lookups`) — единственное место выбора ID:
`SuggestNextId` для битового справочника возвращает **младший свободный бит** объединения всех ID
(комбинации вроде 3 и 7 учитываются), для последовательного — `max + 1` (дырка не переиспользуется:
старое значение не должно молча указывать на новое имя); пустой последовательный — 0, пустой битовый —
1; исчерпание — `null`. `TryParseId` принимает только `ulong` в `NumberStyles.None` (без знака, пробелов
и hex), `IsSingleBit` отличает бит от комбинации. `TableWorkspace.SuggestLookupId()` отдаёт строку для
тулбара и диалога.

**Backend.** `TableDocument.AddRecord(primaryKey)` пишет точный текст ключа; `TableRecord.
SetFieldValueUnchecked` меняет прямой child без проверки 32-битного типа колонки (id справочника —
`ulong`, `Integer` в метаданных — только подсказка). `TableWorkspace.AddLookupRecord` и
`ChangeLookupId` проверяют вид, диапазон и **числовой** дубликат (`key.conflict`; `08` и `8` — один ID),
пишут одну undo-транзакцию. `GetLookupIds`/`LookupKeyField` — только для справочников.

**UI.** В тулбаре `TableView` для справочника видны `LookupKindText` («битовая маска» /
«последовательные ID») и кнопка «Изменить ID…». `AddRow` справочника сам подставляет выделенный ID и
пишет статус `LookupValueAddedFormat` (S80 обновлён под него); обычные таблицы не изменились.
`LookupIdWindow` + `LookupIdViewModel` показывают текущий и свободный ID, блокируют синтаксис и
числовой дубликат, предупреждают о комбинации бит, но принимают её; `IDialogService.EditLookupId`
имеет реализацию по умолчанию (фейки не ломаются). Смена ID перестраивает строки по новому ключу,
статус `LookupIdChangedFormat`.

**Проверка.** Xml: аллокатор (последовательный/битовый, дырки, 64 бита, исчерпание) и workspace
(точный ID, `> int.MaxValue`, дубликат, undo, не-lookup). Data: миграция 18, `ItemFlag` — битовый,
`Cursor` — последовательный. App: `LookupIdViewModel` (синтаксис, дубликат, текущий ID, комбинация) и
`TableEditorViewModel` (бит 32 для `AttachmentPoint`, 4 для `Cursor`, диалог, дубликат, отмена,
обычная таблица без инструментов). UI-smoke S96 — PASS на v4 (свободный бит 32 → свободный ID 64 →
удаление, файл вернулся к 8 записям); S80 — PASS.

**Грабли.** `TableDocument.GetNextPrimaryKey` остаётся 32-битным и используется только обычными
таблицами; для справочников ключ задаётся явно. `Lookup.AmmoChoices` хранит шаг 19…79 — аллокатор
даст 80, некрасивый, но свободный ID; нестандартный шаг вводится вручную. Создание и удаление самих
lookup-файлов не делается: имена справочников фиксированы кодом игры и метаданными, новый файл не был
бы связан ни с одной колонкой; M26a управляет значениями.
