# M22c — Compound record keys: план реализации

**Статус:** выполнен 2026-10-24; следующий шаг — повторный M23a.
**Дата:** 2026-10-24.
**Основание:** аудит M23a на v4/v5 выявил, что у восьми таблиц естественный ключ состоит из нескольких полей, а текущая метабаза объявляет одно поле.

## 1. Проблема и доказательства

`OverlayMerger` и `TableWorkspace.GetRows` группируют записи по одному ключу и берут первую запись группы (`GroupBy(...).First()`). Для таблиц с составным ключом это приводит к подстановке значений одной строки другим строкам:

- `Items.Attachments` в v5: 6023 записи, 154 различных `attachmentIndex`; при выборе предмета 12 секция «Вложения» показывает 967 строк вместо 27.
- Валидация item workspace на v4/v5 даёт 217/218 предупреждений `validation.duplicate_key`: `Items.Attachments` (127/128), `Items.IncompatibleAttachments` (62), `Items.Launchables` (18), `Items.CompatibleFaceItems` (9), `Items.Merges` (49), `NPCInventory.AdditionalDealer_43_Inventory` (1).
- `Items.Merges` имеет 607 дублирующихся `firstItemIndex`; `Items.Attachments` — 5869 дублирующихся `attachmentIndex`.
- Реальные точные дубликаты полного ключа: v5 `Items.Attachments` — 2 (`1011/1076`, `1750/1332`); `AdditionalDealer_43_Inventory` — 1 (`uiIndex=26`) в обоих корпусах. Это дефекты данных, их нельзя исправлять автоматически.

Legacy `DatabaseCode.vb` задавал составные первичные ключи:
- `Items.Attachments`: `attachmentIndex, itemIndex`
- `Items.Launchables`: `launchableIndex, itemIndex`
- `Items.IncompatibleAttachments`: `itemIndex, incompatibleattachmentIndex`
- `Items.CompatibleFaceItems`: `compatiblefaceitemIndex, itemIndex`
- `Items.Merges`: `firstItemIndex, secondItemIndex, firstResultingItemIndex, secondResultingItemIndex`
- `Items.Item_Transformations`: `usItem, usResult1…usResult10`
- `Items.StructureConstruct` / `Items.StructureDeconstruct`: в legacy отсутствуют; текущие одиночные ключи не уникальны, выбраны `usCreationItem, usItemStatusLoss` и `usDeconstructItem, usItemToCreate, usCreatedItemStatus`.

Принятые решения:
- все восемь таблиц получают составные ключи;
- `Duplicate Row` для таблиц с естественным составным ключом отключается;
- key-колонки составных таблиц редактируемы (иначе новую строку невозможно заполнить); одиночные ключи остаются read-only;
- реальные дефекты данных (`Attachments` v5, `AdditionalDealer_43`) остаются предупреждениями, без перезаписи.

## 2. Метаданные и модель

### 2.1 `Core/Metadata/TableMetadata.cs`

Добавить:
- `IReadOnlyList<string> RecordKeyFields` — упорядоченный список XML-полей записи; пусто = текущее поведение.
- `bool HasCompoundRecordKey => RecordKeyFields.Count > 1`.
- `bool HasRecordKey => RecordKeyFields.Count > 0 || RecordKey is not null`.
- Новый необязательный параметр конструктора в конце, чтобы не ломать фикстуры.

`RecordKeySpec`/`RecordKeySegment` **не переиспользовать** для составного ключа: сегменты 1..N уже означают вложенные коллекции (`GEARKIT`, `CITYLIST/CITY`). Составной ключ — отдельное понятие.

### 2.2 `Core/Metadata/RecordKeyCodec.cs`

Каноническое представление только для compound-ключей:
- компонент `null` → `-`;
- непустой компонент → `{length}:{value}` (length-prefixed; разделители и культура не влияют);
- пустой компонент → `0:`;
- `Compose(IReadOnlyList<string?>)`;
- `Decompose(string)`;
- `Display(IReadOnlyList<string?>)` → компоненты через `" | "`.

Для одиночных ключей `GetKey()` остаётся прежней сырой строкой. Это сохраняет overlay-, copy- и тестовые контракты.

### 2.3 Миграция `0009_RecordKeyFields.sql`

```sql
CREATE TABLE RecordKeyFields (
    TableId INTEGER NOT NULL,
    Ordinal INTEGER NOT NULL,
    XmlFieldName TEXT NOT NULL,
    PRIMARY KEY (TableId, Ordinal),
    UNIQUE (TableId, XmlFieldName),
    FOREIGN KEY (TableId) REFERENCES Tables (Id) ON DELETE CASCADE
);

DELETE FROM TableKeys
WHERE TableId IN (SELECT Id FROM Tables WHERE XmlTable IN (...восемь таблиц...));

INSERT INTO RecordKeyFields (TableId, Ordinal, XmlFieldName)
SELECT Id, 0, 'attachmentIndex' FROM Tables WHERE XmlTable = 'Items.Attachments';
INSERT INTO RecordKeyFields ...
```

Порядок полей — legacy. Для `Items.Item_Transformations` 11 строк: `usItem`, `usResult1`…`usResult10`.

`Items.StructureConstruct` и `Items.StructureDeconstruct` в legacy отсутствуют, но текущие одиночные ключи не уникальны; ключи выбраны по реальным данным (уникальны в v4/v5).

`SqliteMetadataProvider` читает таблицу в `TableMetadata.RecordKeyFields`. `MetadataTests` обновляет ожидаемое число миграций с 8 до 9 и проверяет отсутствие старых `TableKeys` у восьми таблиц.

### 2.4 Инвариант overlay

Таблица с `RecordKeyFields.Count > 1` не должна иметь `TranslatableFields`. Сейчас это так; добавить metadata-тест. `OverlayWriter` не расширяется на compound-ключи в этом milestone.

## 3. Документ, workspace и строки

### 3.1 `TableRecord`

- `GetKeyComponents()` — значения `RecordKeyFields` (compound) либо текущая логика.
- `GetKey()` — каноническая строка для compound, иначе прежняя.
- `KeyDisplay` — `Display(components)` для UI.
- `IsKeyComplete` — все компоненты присутствуют и непусты (числовой `0` — валиден).

### 3.2 `TableDocument`

- `FindRecord` — по канонической строке.
- `GetNextPrimaryKey`/`AddRecord`: для compound ключ не генерируется; создаётся запись без key-полей, пользователь заполняет их в grid/detail. Journal пишет `RecordAdded`.
- `DuplicateRecord`:
  - `AssignNextKey` — текущее поведение для одиночных ключей; для compound возвращает `key.duplicate_not_supported`;
  - `PreserveKey` — clone с теми же ключами для зависимых строк и copy.
- `DeleteRecord(TableRecord)` — удаление именно выбранной записи (важно для точных дубликатов); строковый overload сохраняется.

### 3.3 `TableWorkspace`

- `GetRows()`: если overlay нет и `BaseDocument.Records.Count > 0`, возвращать строки напрямую (`Merged = null`), без `OverlayMerger`. Это устраняет утечку значений. Для nested-primary (`Cities`) без overlay сохранить merged-путь.
- `SetValue(WorkspaceRow, field, value)` и `DeleteRow(WorkspaceRow)` — операции по ссылке на запись.
- Для compound-таблиц journal/undo — через `ElementSnapshotState` до/после элемента; для одиночных ключей — текущий key-based путь.
- `GetRow(WorkspaceRow)`/refresh без поиска по старому ключу.

### 3.4 `TableSetWorkspace`

- Duplicate item: зависимые ChildRows копировать через `PreserveKey`, затем переписывать parent-поле; single-key SubTable — текущее выделение нового ключа.
- Delete item: удалять зависимые записи по ссылкам/точному элементу, не по первому совпадению ключа.
- Rename/swap item не меняются (primary ITEM — одиночный ключ).
- `FindDependingKeys` дополнить вариантом, возвращающим записи, чтобы точные дубликаты не терялись.

### 3.5 Bulk

Key-based bulk остаётся; для точных дубликатов compound-ключей применяется первая строка. Ограничение зафиксировать в отчёте M23a; отдельный row-identity bulk — при необходимости позже.

## 4. Overlay

- `OverlayKeyResolver`: при `RecordKeyFields` собирать канонический ключ из direct child-полей; иначе прежняя логика.
- `MergedRecord.Key` — канонический.
- `TableWorkspace.GetRows` — no-overlay fast path; с overlay composite-ключи больше не сворачиваются.
- `TranslationAnalyzer`/`OverlayWriter` не расширяются; действует инвариант из 2.4.
- `MercStartingGear`, `Cities`, `FacilityTypes` (nested/positional) должны остаться без регрессий.

## 5. Схема и UI

- `TableSchema`: добавить `KeyColumns`; `KeyColumn` остаётся первым для совместимости.
- `TableSchemaBuilder`: `IsKey` для всех `RecordKeyFields`; группа `identity` для всех; иначе текущее поведение.
- `TableColumnViewModel.IsReadOnly`: key-колонки read-only только для одиночных ключей; compound — редактируемы.
- `DetailFieldViewModel`: та же логика; bulk по-прежнему исключает все `IsKey`.
- `TableEditorViewModel`:
  - `ApplyDefaultColumns` показывает все key-колонки;
  - `SetCellValue` для compound идёт через `SetValue(WorkspaceRow, …)`;
  - `AddRow` выбирает строку по ссылке на запись;
  - `DeleteRow` удаляет точную строку;
  - `DuplicateRow` disable для compound;
  - `KeyDisplay` в статусах/подтверждениях/поиске;
  - `ApplyRowView` — выбор по `TableRecord`/строке для add и key-edit.
- `TableView.xaml.cs`: жирным все key-заголовки.
- Item workspace: дочерний фильтр станет корректным после composite keys и no-overlay fast path; `NewRowDefaults` задаёт parent-link, остальные key-поля вводятся в дочерней сетке.

## 6. Валидация, сохранение, copy, compare

- `TableValidator`: duplicate/missing по полному compound-ключу; missing перечисляет отсутствующие поля; unknown-field включает все key-поля.
- `LosslessDocumentPatcher.BuildSignatures`: принимать список key-полей; compound-сигнатура `R|name|` + каноническая строка; при неполном ключе — positional fallback.
- `TableCopyService`:
  - `TableLoad.KeyFields`;
  - `Preserve` сохраняет все компоненты, конфликт по полному ключу;
  - `Allocate` для compound отклоняется (`copy.allocate_unsupported`), UI отключает режим;
  - related-строки: clone сохраняет остальные key-компоненты, parent-link переписывается;
  - `InsertRecord` принимает готовый clone без перезаписи единственного key-поля.
- `TableCopyViewModel`: display-ключи `a | b`, парсинг по `|`, ошибки отклоняются; каноническая строка внутренняя.
- `TableSnapshotAnalyzer`: все key-поля помечать `isKey`.

## 7. Тесты

### Unit
- `RecordKeyCodec`: отсутствующие/пустые компоненты, `:`, `|`, Unicode, round-trip.
- `TableDocument`: composite GetKey/Find/Add без генерации/Delete точной строки/Duplicate modes.
- `TableWorkspace`: key edit + undo, удаление одной из строк с общим первым компонентом, no-overlay `GetRows` без утечки.
- `OverlayTests`: fixture с двумя записями, совпадающий первый компонент и разный второй; значения не смешиваются.
- `TableValidatorTests`: composite duplicate/missing.
- `LosslessPatchTests`: add/delete в compound-таблице, unchanged bytes.
- `TableCopyServiceTests`: preserve/related copy compound, allocate rejected.
- `TableSchemaBuilderTests`: несколько key-колонок.

### Corpus (v4/v5)
- `RealRecordKeyTests`:
  - duplicate-key baseline: v4 — только `AdditionalDealer_43` (1); v5 — `Items.Attachments` (2) + `AdditionalDealer_43` (1); остальные 0;
  - missing-key на реальных данных — 0;
  - item 12 attachments: число строк равно сырому XML (27 в v5; в v4 считать из файла), все строки имеют `itemIndex=12`;
  - удаление одной строки с общим `attachmentIndex` на временной копии меняет только её; undo/сохранение не трогает остальные байты.
- `RealDataValidationTests`/`RealAuxiliaryTablesTests` обновить под новые ключи и baseline.
- `CorpusRoundTrip`, `CorpusStructural`, `CorpusCopy`, `RealDataComparison`, perf — зелёные.

### App
- key-колонки редактируемы у compound;
- duplicate disabled;
- add/delete точной строки;
- item child filter на реальных данных;
- copy display-ключи.

Обновить тесты, ожидающие `schema.KeyColumn`, `Primary.KeyField`, число миграций и bare-key (`"10"`) для `Items.Attachments`.

## 8. Проверка и критерии готовности

1. `dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx -warnaserror` — 0/0.
2. `dotnet test …` — все тесты зелёные.
3. `XER_CORPUS_DIR` для v4 и v5 — полный прогон; round-trip, validation, relations, copy, compare, perf — зелёные.
4. Приёмка:
   - нет 217/218 ложных duplicate-key;
   - item 12 показывает реальное число вложений;
   - удаление строки с неуникальным первым компонентом удаляет именно выбранную;
   - неизменённые файлы не перезаписываются; `References/` не изменён.
5. Коммит `M22c: compound record keys`.
6. После этого — новая сессия M23a: `LEGACY-PARITY-REPORT.md` создаётся заново; composite-key дефект фиксируется как исправленный в M22c, оставшиеся пробелы (sound pickers, LBE pockets, explosion animation, robot skills, image chooser, shopkeeper add/remove, reverse item grids/launchables label, duplicate-entry validation, description limits, About) идут в M23b.

## 9. Вне scope M22c

- Автоисправление реальных дефектов данных (`Attachments` v5, `AdditionalDealer_43`): только предупреждения.
- Compound overlay-ключи: сейчас compound-таблицы не translatable; инвариант-тест.
- Bulk по точным дубликатам compound-ключей: первая строка, ограничение документируется.
- Семантика/подпись Launchables и остальные parity-пробелы — M23a/M23b.

## 10. Основные риски

- Правка key-полей меняет identity строки: операции должны идти по ссылке на запись, undo — по snapshot элемента.
- Нельзя менять формат одиночных ключей: сломает overlays, copy и существующие тесты.
- `LosslessDocumentPatcher` должен сохранять byte-identity и корректно патчить compound-записи.
- Вложенные ключи (`MercStartingGear`, `Cities`, `FacilityTypes`) не должны регрессировать.

## 11. Затрагиваемые файлы

- `src/XmlEditorReloaded.Core/Metadata/TableMetadata.cs`
- `src/XmlEditorReloaded.Core/Metadata/RecordKeyCodec.cs` (новый)
- `src/XmlEditorReloaded.Data/Metadata/Migrations/0009_RecordKeyFields.sql` (новый)
- `src/XmlEditorReloaded.Data/Metadata/SqliteMetadataProvider.cs`
- `src/XmlEditorReloaded.Xml/Documents/TableDocument.cs`, `TableRecord.cs`, `DocumentChangeTracker.cs`
- `src/XmlEditorReloaded.Xml/Workspace/TableWorkspace.cs`, `TableWorkspace.Bulk.cs`, `TableSetWorkspace.cs`, `TableEditHistory.cs`
- `src/XmlEditorReloaded.Xml/Overlays/OverlayKeyResolver.cs`, `OverlayMerger.cs`
- `src/XmlEditorReloaded.Xml/Schema/TableSchemaBuilder.cs`, `TableSchema.cs`
- `src/XmlEditorReloaded.Xml/Saving/LosslessDocumentPatcher.cs`
- `src/XmlEditorReloaded.Xml/Validation/TableValidator.cs`
- `src/XmlEditorReloaded.Xml/Copy/TableCopyService.cs`, `TableCopyModels.cs`
- `src/XmlEditorReloaded.Xml/Comparison/TableSnapshotAnalyzer.cs`
- `src/XmlEditorReloaded.App/ViewModels/TableEditorViewModel.cs`, `TableRowViewModel.cs`, `TableColumnViewModel.cs`, `DetailFieldViewModel.cs`, `ItemWorkspaceViewModel.cs`, `TableCopyViewModel.cs`
- `src/XmlEditorReloaded.App/Views/TableView.xaml.cs`
- тесты: Core/Xml/Data/App, перечисленные в разделе 7.
