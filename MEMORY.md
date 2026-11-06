# Память проекта — XML Editor Reloaded

Рабочие заметки предыдущего агента (ZCode), импортированные 2026-10-25 из
`~/.zcode/cli/memories/projects/ja2_1.13-tools-…/memory`: 26 файлов, ~105 КБ.
Тексты перенесены без изменений (включая frontmatter и исходный индекс
`memory/MEMORY.md`).

**Как пользоваться**

- Это индекс: читайте его в начале работы, тематический файл — только по необходимости.
- Источник истины — код, тесты и git-история; правила — `AGENTS.md`, план — `PLAN.md`.
  При расхождении правы они, а замеченное расхождение стоит поправить в файле памяти.
- Заметки, помеченные «инструментальное», описывают настройку ZCode и к текущему
  окружению относятся только как история.
- После milestone обновляйте затронутый файл и держите его коротким
  (`memory/documentation-stays-compact.md`, `memory/context-stays-small.md`).

## Правила и процесс

| Тема | Файл | О чём |
|---|---|---|
| Целостность XML | [memory/xml-integrity-is-mandatory.md](memory/xml-integrity-is-mandatory.md) | Никогда не ломать таблицы: без удаления полей, sparse-записи, лексические типы |
| `References/` | [memory/references-folder-is-user-managed.md](memory/references-folder-is-user-managed.md) | Только чтение, обновляет пользователь, >12 ГБ, две установки игры |
| Структура `References/` | [memory/reference-projects-structure-doc.md](memory/reference-projects-structure-doc.md) | Как расширять `REFERENCE-PROJECTS-STRUCTURE.md`, семь справочников |
| Доступ к исходникам | [memory/ja2-source-needs-permission.md](memory/ja2-source-needs-permission.md) | C++-исходники 5,4 ГБ — спрашивать разрешение каждый раз |
| Коммиты | [memory/commit-after-each-milestone.md](memory/commit-after-each-milestone.md) | Локальный `Mx: …` после milestone, push только по просьбе |
| Компактная документация | [memory/documentation-stays-compact.md](memory/documentation-stays-compact.md) | `PLAN.md` и `PROJECT-STRUCTURE.md` — сводки, детали в коде/тестах/git |
| Малый контекст | [memory/context-stays-small.md](memory/context-stays-small.md) | Контекст сессии — бюджет; короткие файлы вместо подробных |
| Чтение перед правкой | [memory/read-before-edit.md](memory/read-before-edit.md) | Правило только для `Edit`, не для `Write`; держать в одну фразу |
| Бюджет контекста ZCode | [memory/zcode-context-budget.md](memory/zcode-context-budget.md) | Инструментальное: замеры 29,4k→12,8k и рычаги настройки ZCode |
| UI-отступы | [UI-RULES.md](UI-RULES.md) | Минимум 3 px, стандарт 6 px; равные промежутки кнопок и строк; подпись и поле не расцепляются |

## Данные и эталон

| Тема | Файл | О чём |
|---|---|---|
| Факты о данных v4/v5 | [memory/ja2-v4-v5-data-facts.md](memory/ja2-v4-v5-data-facts.md) | Одинаковые наборы тегов, разные данные, один битый lookup, аномалия burst-звука |
| Модель оверлеев | [memory/ja2-localization-overlay-model.md](memory/ja2-localization-overlay-model.md) | v4 русский / v5 английский по замыслу; `Russian.*` сливаются по ключу |
| Находки в legacy | [memory/legacy-reference-findings.md](memory/legacy-reference-findings.md) | Непригодный StiLib, захардкоженная схема VB, битый парсер прототипа |
| Проверенные метаданные M14.5 | [memory/m14.5-verified-metadata.md](memory/m14.5-verified-metadata.md) | 502 hints, 46 lookup-колонок, 64-битные ID, ноль ложных предупреждений |

## Редактор

| Тема | Файл | О чём |
|---|---|---|
| Статус и roadmap | [memory/ja2-editor-status.md](memory/ja2-editor-status.md) | M0–M28 готовы, автогейты M23b–M27c пройдены, B1–B2 закрыты, 726 тестов, полный UI-smoke 99/99, ручной smoke принят; M28 portable release; roadmap M18–M28 закрыт |
| Упаковка и релиз | [memory/m28-packaging.md](memory/m28-packaging.md) | M28: версия 1.0.0, иконка, self-contained + framework-dependent win-x64, два portable ZIP/SHA-256, GLWTPL, release-скрипт, CI/release workflow |
| Архитектура и сборка | [memory/ja2-editor-architecture.md](memory/ja2-editor-architecture.md) | Решение, пакеты, DI, corpus/perf-гейты, гигиена репозитория |
| Round-trip XML | [memory/ja2-editor-xml-roundtrip.md](memory/ja2-editor-xml-roundtrip.md) | Байтовый патчинг, нормализация `XmlWriter`, структурный патчинг, editing API |
| Метаданные | [memory/ja2-editor-metadata.md](memory/ja2-editor-metadata.md) | Схема/seed SQLite, валидация, `ColumnHints` vs `Columns`, границы min/max, флаги |
| Локализация | [memory/ja2-editor-localization.md](memory/ja2-editor-localization.md) | Загрузка/слияние/запись оверлеев, вложенные ключи, отчёт по переводам |
| Shell и grid | [memory/ja2-editor-shell-grid.md](memory/ja2-editor-shell-grid.md) | M9–M13: shell, workspace/schema, grid-редактор, панель деталей |
| Item и merc | [memory/ja2-editor-item-merc.md](memory/ja2-editor-item-merc.md) | M19 item master-detail, M20 merc gear и вспомогательные таблицы, M23b2 |
| Массовые правки и сравнение | [memory/ja2-editor-editing-operations.md](memory/ja2-editor-editing-operations.md) | M16 bulk/undo, M17 сравнение версий и экспорт |
| Связи и копирование | [memory/ja2-editor-relations-copy.md](memory/ja2-editor-relations-copy.md) | M18 pickers связей, M21 multi-dataset copy/remap |
| Полировка grid | [memory/ja2-editor-grid-polish.md](memory/ja2-editor-grid-polish.md) | M22a фильтры/layout, M22b recovery, M22c составные ключи, F1 вложенные таблицы/контекстное меню |
| Вложенные редакторы | [memory/m24a-nested-editors.md](memory/m24a-nested-editors.md) | M24a: `Map.Cities`/`Map.FacilityTypes`, nested primary path, root singleton, `ASSIGNMENT`, overlay tooltip, S93 |
| Структурные вложенные записи | [memory/m24b-nested-collections.md](memory/m24b-nested-collections.md) | M24b: add/delete `ASSIGNMENT`, блоки `PRODUCTIONLINE`, списки `drugitem`, nested-перевод `townName`, S94 |
| Сравнение v2 | [memory/m25a-comparison-v2.md](memory/m25a-comparison-v2.md) | M25a: значения записей по ключу/порядку, ячейки оверлеев, root singleton, лимиты деталей, вкладки и экспорт |
| Датасет: отчёт и поиск | [memory/m25b-dataset-report.md](memory/m25b-dataset-report.md) | M25b: одна проверка всех таблиц, повреждённые файлы/оверлеи, экспорт, глобальный поиск по базе и оверлеям, переход к записи |
| Справочники: ID значений | [memory/m26a-lookup-ids.md](memory/m26a-lookup-ids.md) | M26a: свободный бит для битовых lookup, максимум + 1 для последовательных, свободный 64-битный ID, undo, вид справочника в метаданных |
| Справочники: защита значений | [memory/m26b-lookup-protection.md](memory/m26b-lookup-protection.md) | M26b: «где используется», запрет удаления/смены ID используемого значения, биты маски без имени, переход к записи, S97 |
| Дерево таблиц | [memory/m27b-table-tree.md](memory/m27b-table-tree.md) | M27b: вкладки «Дерево»/«Список», папки по `RelativePath`, фильтр, Fold/Unfold all, Find in tree, S100 |
| XML-просмотр | [memory/m27c-xml-view.md](memory/m27c-xml-view.md) | M27c: глобальный Grid/XML для всех таблиц, read-only снимок базовой таблицы, поиск по ID/ключу, переход из валидации, S101–S104 |
| Паритет с legacy | [memory/ja2-editor-parity.md](memory/ja2-editor-parity.md) | Аудиты M23a0–M23a7, реестр пробелов, исправления M23b1–M23b8 |
| STI-изображения | [memory/ja2-editor-imaging.md](memory/ja2-editor-imaging.md) | Парсер STI/ETRLE, каталоги, просмотр и chooser кадров |
| UI-smoke | [memory/ja2-editor-ui-smoke.md](memory/ja2-editor-ui-smoke.md) | Автоматический прогон чеклиста (части A–G): запуск, изоляция `%AppData%`, грабли UIA |
