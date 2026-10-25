# M23b5 — изображения: журнал реализации

**Дата:** 2026-10-25. **Статус:** выполнен. **Закрытые пробелы:** `L-ITEM-014`,
`L-IMG-005`, `L-IMG-020` — 3 записи реестра.
**Основание:** `M23A-PARITY/20-item-ui.md` (L-ITEM-014), `M23A-PARITY/21-item-data-images.md`
(L-IMG-005/020), `LEGACY-PARITY-REPORT.md` (реестр и подшаг M23b5); план — `PLAN.md` (M23b5).

Этот файл фиксирует состояние, решения и проверки M23b5. Источник истины — код, тесты и git;
здесь только то, что из них не восстанавливается (мотивы выбора поведения и сверка с legacy).

## 1. Состояние на входе

- Незакоммиченный каркас: `ItemImageCatalog.GetFrameCountAsync`/`GetMaxIndexAsync`,
  `IItemImageService.GetFrameCountAsync`/`WpfItemImageService`, `ItemImageOption`, chooser
  `GraphicImages`/`SelectedGraphicImage`/`GraphicIndexMaximum` в `ItemWorkspaceViewModel`,
  `InvalidateThumbnail` в `TableRowViewModel`/`ItemListEntry`, разметка вкладки Images и строки
  `Strings.resx`/`Strings.ru.resx`. Тесты: 2 в Imaging (frame count/max и missing/big).
- Сборка уже была зелёной, базовый прогон — 472/472 (App 150, Imaging 21). Не хватало тестов
  App-слоя, исправлений гонки миниатюр, окна валидации индекса на время загрузки, проверки
  отрицательного индекса и всей документации.

## 2. Сверка с legacy

- Максимум `ubGraphicNum` = `SmallItems(type).Length - 1`, пересчитывается при открытии формы и
  смене типа (`References/xml-editor/GUI/Data Forms/ItemDataForm.vb:74`, `:807`;
  `References/xml-editor/BackEnd/Images/ItemImageManager.vb:136-140`). WinForms `NumericUpDown`
  при смене `Maximum` молча зажимает значение вниз и сохраняет зажатое при Apply; при открытии
  формы значение выше максимума, наоборот, бросает исключение.
- `ImageListBox` перечисляет **большие** изображения текущего типа (`:809`), но длину списка
  legacy брала из **medium**-листа (`ItemImageManager.vb:89`), а максимум — из **small**. Поэтому в
  legacy список и максимум могли не совпадать; клик по лишнему элементу бросал исключение.
  Выбор писал только индекс (`:821-823`).
- После Commit форма обновляла синтетическую `ItemImage` (big-кадр) и писала `ubGraphicNum`/
  `ubGraphicType` (`:953-955`); `ItemImage` в XML не сохраняется (`ItemTable.vb:38-48`, `:71-75`).
  Строки с отсутствующим big-изображением legacy молча переписывала в `0/0` (`ItemTable.vb:56-68`).
- Для big-кадров legacy делала прозрачным цвет угла (`ItemImageManager.vb:105-128`), а green key
  применяла только к medium/small (`:60`, `:78`). Отсутствие любого STI при старте было фатальным.

## 3. Принятые решения

1. **Динамический максимум, без seed.** `ubGraphicNum` не получает статический `Maximum`:
   в v4 `smguns.sti` 376 кадров, в v5 — 375, у типа 3 — 12; единого статического предела нет,
   а `ColumnHints`/`TableValidator` не имеют доступа к data-dir и STI. Максимум считает
   `ItemImageCatalog` (`GetFrameCountAsync(Small) - 1`), форма получает его после загрузки листа.
2. **Chooser = ровно `max + 1` кадр small-листа, превью — из big-файла.** Это осознанно
   исправляет расхождение legacy (список medium против максимума small): в rewrite список и
   допустимый диапазон всегда совпадают, клик не может выйти за максимум.
3. **Никакого clamp и перезаписи.** Отрицательный индекс отклоняется всегда (у legacy
   `Minimum = 0`); нецелый — метаданными; при известном максимуме верхняя граница отклоняется с
   сообщением; пока лист грузится, запись индекса отклоняется с `image.not_ready` (старый список
   уже очищен синхронно, поэтому клик по чужому кадру невозможен). При смене типа сохранённый
   индекс **не зажимается**: форма показывает «Index N is outside the valid range 0…M», а после
   выбора валидного кадра сообщение сменяется количеством изображений. Это соответствует
   правилам XML-целостности: редактор меняет только явно отредактированное поле.
4. **Инвалидация миниатюр без гонки.** `ubGraphicType`/`ubGraphicNum` сбрасывают кэш строки
   грида и записи списка предметов; generation-guard отбрасывает завершившийся позже старый
   декод, поэтому он не перезаписывает новый кадр. Если миниатюра ещё не запрашивалась, загрузка
   произойдёт при появлении элемента (виртуализация сохранена).
5. **Безопасные отличия от legacy.** Нет переписывания строк в `0/0` при отсутствии big-файла;
   отсутствие STI не фатально; тип можно сохранить, даже если файла листа нет (превью покажет
   `not available`, редактирование индекса остаётся возможным, кроме отрицательного).
6. **33 вложенных контрола стоек не входят в M23b5.** В `50-numeric-ranges.md`/`52` они
   упомянуты как возможное расширение, но в реестре M23b5 только три image-ID; вложенные пути
   требуют отдельной модели границ и остаются за пределами этого подшага.

## 4. Что изменено

- `Imaging/Catalog/ItemImageCatalog.cs`: `GetFrameCountAsync` (Big → `image.no_sheet`), кэш
  `(path, lastWriteTime)` → count, `GetMaxIndexAsync`; тесты на medium, повторное чтение после
  изменения файла, negative type, cancellation, missing sheet.
- `Core/Api/ErrorCodes.cs`: `image.not_ready` для попытки правки во время загрузки списка кадров.
- `App/Services/IItemImageService.cs`, `WpfItemImageService.cs`: frame count через каталог данных.
- `App/ViewModels/ItemWorkspaceViewModel.cs`: `GraphicImages`/`SelectedGraphicImage`/
  `GraphicIndexMaximum`/`GraphicImageListInfo`; фильтр индекса (negative/loading/upper bound);
  очистка списка до await; out-of-range сообщение; перезагрузка списка при смене типа, предмета,
  undo/redo; инвалидация миниатюры записи списка; `SyncGraphicImageSelection` возвращает
  количество кадров после исправления индекса.
- `App/ViewModels/ItemImageOption.cs`: ленивая одноразовая загрузка big-превью кадра.
- `App/ViewModels/TableRowViewModel.cs`, `ItemListEntry.cs`: `InvalidateThumbnail` +
  generation-guard; `TableEditorViewModel.SetCellValue` и `ItemWorkspaceViewModel.SetCellValue`
  вызывают инвалидацию для двух graphic-полей.
- `App/Views/ItemWorkspaceView.xaml(.cs)`: левая колонка со списком кадров (виртуализация,
  big-миниатюры, loaded-handler), правая — прежние три превью и combo типа.
- `Strings.resx`/`Strings.ru.resx`: `ItemImageListLabel`, `ItemImageListInfoFormat`,
  `ItemImageOptionFormat`, `ItemImageIndexOutOfRangeFormat`, `ItemImageIndexNegative`,
  `ItemImageListLoading`.
- Тесты: `ItemWorkspaceViewModelTests` (chooser, выбор/запись индекса, границы, loading-window,
  negative, смена типа, очистка выбора, undo, инвалидация и гонка list-миниатюры, corpus item 12),
  `TableEditorViewModelTests` (инвалидация и гонка grid-миниатюры),
  `ItemImageCatalogTests` (5 новых), `TestItemImageService` (задержки, фабрика битмапов).

## 5. Проверка M23b5

| Проверка | Результат |
|---|---|
| Сборка `-warnaserror` | 0 ошибок / 0 предупреждений |
| Базовый прогон | 488/488: Core 37, Data 67, Imaging 24, Xml 197, App 163 |
| Corpus v4 / v5 | 488/488 на каждом; `Corpus_Item12ImageChooserMatchesGunSheet` проверяет item 12 (тип 0, индекс 110) на реальном листе |
| Compare / copy / perf | 1/1, 1/1, 9/9 (v5) |
| `References/` | не изменён; тесты работают на temp-копиях и читают корпус только на чтение |

Ключевые тесты: `SelectItem_BuildsGraphicImageChooserFromSheetFrames`,
`SelectGraphicImage_WritesGraphicIndexAndReloadsPreview`,
`SetItemValue_GraphicIndexAboveSheet_IsRejected`,
`ChangeGraphicType_ReloadsImageChooserForNewSheet`,
`ChangeGraphicType_WhileFrameCountPending_RejectsStaleIndex`,
`SetItemValue_NegativeGraphicIndex_IsRejectedWithoutSheet`,
`ClearSelection_ClearsGraphicImageChooser`, `Undo_GraphicIndexChange_RestoresChooserSelection`,
`GraphicEdit_InvalidatesItemListThumbnail`,
`GraphicEdit_WhileListThumbnailLoadPending_KeepsNewestImage`,
`GraphicEdit_InvalidatesGridThumbnail`,
`GraphicEdit_WhileGridThumbnailLoadPending_KeepsNewestImage`,
`Corpus_Item12ImageChooserMatchesGunSheet`,
`Catalog_FrameCount_MatchesSmallSheetAndMaxIndex`,
`Catalog_FrameCount_MissingSheetAndBigSizeAreRejected`,
`Catalog_FrameCount_ReReadsWhenSheetChanges`,
`Catalog_FrameCount_MediumSheetAndArgumentGuards`,
`Catalog_MaxIndex_MissingSheet_ReturnsImageNotFound`.

## 6. Остаточные риски и задел

- Прозрачность цвета угла у big-кадров legacy не воспроизведена: rewrite применяет green key
  только к medium/small. Это визуальное отличие существовало до M23b5 и в реестр пробелов не
  входило; при необходимости — отдельная задача с тестом на цвет угла.
- Chooser-превью декодируются с `CancellationToken.None`; при быстрой смене типа старый декод
  досчитывается, но его результат остаётся у выброшенной опции.
- Frame count читает STI целиком (через `ParseAsync`), а не только заголовок; результат кэширован
  по пути и mtime, поэтому повторные открытия формы не парсят файл заново.
- Статическая валидация `ubGraphicNum` остаётся динамической только в App; `TableValidator` не
  знает data-dir. Вне item workspace (standalone grid) верхняя граница не проверяется — там
  неверный индекс даёт `image.not_found`, как и раньше.
