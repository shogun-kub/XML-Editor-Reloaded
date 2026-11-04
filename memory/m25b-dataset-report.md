---
name: m25b-dataset-report
description: M25b — dataset-wide validation report and global search (backend services, window, limits, navigation).
metadata:
  node_type: memory
  type: project
---

# M25b — Проверка датасета и глобальный поиск

**Backend (Xml).** `DatasetValidator` (`Xml/Validation`) обходит таблицы каталога по порядку:
метаданные (если есть), `TableFileLoader`, затем `TableValidator` с общим на весь прогон кэшем lookup
(внутренняя перегрузка `ValidateAsync`, публичный API не менялся). Повреждённый файл → ошибка
`validation.malformed_file`, нечитаемый → `validation.unreadable_file`, битый/нечитаемый оверлей →
`validation.malformed_overlay` с именем файла, диагностики загрузки → предупреждения
`validation.load_diagnostic`. `TableValidationSummary` хранит точные счётчики по важности и первые
`MaxIssuesPerTable = 1000` замечаний; `DatasetValidationReport` суммирует их и знает число
повреждённых/нечитаемых/неметаданных таблиц. `DatasetValidationReportFormatter` пишет Markdown и CSV
(строка таблицы + строки замечаний), числа инвариантные.

**Backend (поиск).** `DatasetSearchService` (`Xml/Search`) ищет подстроку в **листьях XML** базы и
каждого оверлея. Записи берутся из `OverlayKeyResolver.ResolvePrimary` (понимает вложенные контейнеры,
позиционные и singleton-ключи), для неметаданных — из `TableDocument.Records` (ключ = первое поле).
Путь поля считается от записи, корневые singleton-поля — от корня документа с пустым ключом. Значение
сравнивается целиком, в отчёт попадает обрезанным до 200 символов; лимиты `MaxHits = 5000`,
`MaxHitsPerTable = 500`, при этом `MatchCount` и список таблиц с совпадениями точные.

**UI.** `DatasetViewModel` + `DatasetWindow` (`Views`): вкладки «Валидация» и «Поиск», общий статус с
прогрессом по таблицам, отдельные `CancellationTokenSource` на прогон. Валидация автостартует при
открытии на своей вкладке; фильтр важности и текстовый фильтр пересобирают список без повторного
чтения. Экспорт — через `IDialogService.PickSaveFile` (`.md`/`.csv`). `NavigationRequested` уходит в
`MainViewModel`, который открывает вкладку через `OpenEditorByNameAsync` и вызывает `SelectRecord`
(запись выбирается, если у замечания/совпадения есть ключ). Окно немодальное; `IDialogService.ShowDataset`
имеет пустую реализацию по умолчанию, чтобы не менять все тестовые фейки.

**Проверка.** Xml-тесты: валидатор (счётчики, повреждённый/нечитаемый файл, битый оверлей, лимиты,
прогресс, форматтеры) и поиск (база, оверлей, регистр, вложенные/корневые пути, лимиты, повреждённая
таблица). App-тесты: проекция отчёта, фильтры, поиск, экспорт, навигация из замечания. Корпус
(`XER_CORPUS_DIR`, `RealDatasetReportTests`): v4 — 238 таблиц, 25918 записей, 2 предупреждения, 65
информации; v5 — 239 таблиц, 1 повреждённый lookup, 3 дубликата ключа. UI-smoke S95 проходит на v4;
попутно `TabNames`/`WaitForEditorTabCount` ограничены прямыми детьми `EditorsTabs` (вложенные вкладки
item/merc workspace больше не считаются вкладками редактора), а S14 переведён на `EditCellUntilDirty`.

**Грабли.** `Progress<T>` создаётся в UI-потоке (иначе `StatusText` обновляется из чужого потока).
Для поиска язык не важен: ищутся все оверлеи, язык показывается в результате. Валидация relations не
проверяет (в отличие от одиночной таблицы в редакторе) — для этого нужен полный граф целей, а отчёт
задуман как быстрый обзор.
