# 59 — ручной smoke: дефекты S74 и автоматизация S83

**Дата:** 2026-11-03. **Статус:** S74 исправлен и перепроверен (✅); S83 автоматизирован.
**Связанные документы:** `MANUAL-SMOKE-CHECKLIST.md`, `PLAN.md`, `LEGACY-PARITY-REPORT.md`.

## S74 — найденные дефекты и исправления

Ручная проверка v4 выявила два независимых дефекта в item workspace:

1. **«Перейти к ID» не прокручивал список.** Команда `GoToItemId` очищала фильтры и выбирала
   `ItemListEntry`, но `ListBox.SelectedItem` не вызывает `ScrollIntoView`; выбранный предмет
   оставался за пределами видимой области. Добавлен обработчик `OnItemListSelectionChanged`
   в `Views/ItemWorkspaceView.xaml.cs`: при появлении выбранного элемента список прокручивается
   к нему (`ItemWorkspaceView.xaml`, `SelectionChanged="OnItemListSelectionChanged"`).
2. **Часть миниатюр не загружалась после прокрутки.** Список использует
   `VirtualizingStackPanel` с `VirtualizationMode="Recycling"`, а запрос миниатюры был привязан
   только к `Image.Loaded`. Переиспользованный контейнер получает новый `DataContext`, но
   повторного `Loaded` не происходит, поэтому новые строки оставались с пустой картинкой.
   Добавлены обработчики `DataContextChanged` для миниатюр списка предметов и списка кадров
   (`OnItemThumbnailDataContextChanged`, `OnGraphicImageThumbnailDataContextChanged`); запрос
   идемпотентен благодаря `ItemListEntry.RequestThumbnail` / `ItemImageOption.RequestThumbnail`.

Отдельной сетки `Items.Items` в rewrite нет — таблица всегда открывается как item workspace,
поэтому часть сценария «в гриде» помечена N/A. Пункт «item без big-файла» для v4 не
воспроизводится: big-файл есть у всех 1776 предметов; пустая ячейка покрыта unit-тестами.

## S83 — автоматизация со скриншотами

Добавлен `Scenarios/PlacementRecoveryScenario.cs` (id S83, v4). Сценарий перезапускает
приложение с точным содержимым `settings.json` через новый
`ScenarioContext.RestartAppWithJson` / `UiApp.Start(..., rawSettingsJson)` и проверяет:

- off-screen placement `left/top = -32000` — окно поджимается в виртуальный экран;
- `width = 0`, `height = 0` — placement отбрасывается, окно обычного размера;
- неполный объект (`windowPlacement` без `height`) — placement отбрасывается;
- `"width": "NaN"` — невалидный JSON не мешает запуску, окно открывается с настройками по умолчанию;
- read-only `settings.json` — ошибка записи не мешает закрытию, файл не перезаписывается.

Координаты окна читаются нативно (`GetWindowRect`), границы виртуального экрана — через
`GetSystemMetrics`, поэтому проверка не зависит от DPI и WPF-координат. После каждого
перезапуска снимается PNG (`Screenshots.Capture`): 6 файлов в
`.tmp/ui-smoke/<run>/scenarios/S83/`.

Физическое отключение монитора не автоматизируется; его поведение покрывает проверка
восстановления off-screen placement (тот же `WindowPlacement.FitToScreen`).

## S53 — название секции

В UI секция `Launchables` называется **«Боеприпасы»** (`ItemSectionReverse.Items_Launchables`),
а не «Запускаемые предметы». Это семантически верное название; поправлена документация
чек-листа и инструкция, код не менялся.

## Проверка

- `dotnet build … -warnaserror` — 0/0.
- `util-scripts/test.ps1` — 601/601.
- `--only S83` — 2/2 (S83 + обязательный S50), 6 скриншотов.
- `--only S16` — 2/2; `--only S71` — 2/2 (регрессия списка и кадров после правок XAML).
- Ручная перепроверка S74 на v4 после `faefca8` — ✅.

## Итог

Ручной smoke принят 2026-11-03: S04, S22, S53 и S74 пройдены на v4, S83 автоматизирован.
Колонки v5 для S22/S53/S74 не перепроверялись по решению пользователя (v4 содержит те же
данные плюс оверлеи). Поставлен маркер `M23: legacy functional parity gate`. Дополнение
2026-11-06: S04/S22/S53 на v5 — PASS, S74 — N/A, ветка ошибки S13 — PASS; baseline unchanged.
