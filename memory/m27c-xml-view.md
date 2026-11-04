---
name: m27c-xml-view
description: M27c stage 1 — global Grid/XML switch for every table tab, read-only base-table snapshot, copy/refresh, S101–S104.
metadata:
  node_type: memory
  type: project
---

# M27c, этап 1 — глобальный read-only XML

**Дата:** 2026-11-04. **Статус:** этап 1 завершён, S101–S104 PASS на v4.

- Переключатель **Grid/XML** находится справа от вкладок редакторов и действует на все таблицы,
  включая `Items`, `MercGear` и `Map` (специализированные workspace-редакторы). Состояние хранится
  в `MainViewModel.IsXmlMode` и переключается `RadioButton`-сегментами; это не отдельная вкладка
  внутри таблицы.
- `IEditorTab.CreateXmlSnapshot()` реализован всеми четырьмя типами вкладок и возвращает XML
  **базовой** таблицы: `TableEditorViewModel`, `ItemWorkspaceViewModel`, `MercGearWorkspaceViewModel`,
  `NestedTableWorkspaceViewModel`. В снимок не входят оверлеи и дочерние таблицы workspace.
- XML строится из **текущего in-memory документа** `TableDocument.CreateXmlSnapshot()`, поэтому
  отражает несохранённые правки. Это снимок для чтения, а не байтовое представление файла:
  форматирование сериализатора может отличаться, сохранение по-прежнему идёт через lossless patcher.
- В XML-режиме общий `XmlTextView` заменяет всё содержимое выбранной вкладки: скрыты тулбар,
  поиск, фильтр, выбор колонок, сетка, валидация и специализированные панели. Остаются только
  действия, видимые в текстовом редакторе: копирование и обновление снимка.
- Снимок создаётся в фоне при включении XML и при смене выбранной вкладки; кнопка «Обновить XML»
  повторяет сборку. Подпись «Только чтение · базовая таблица» и статус
  «Сохранено/Есть несохранённые изменения» объясняют источник.
- Проверки: `TableDocument.CreateXmlSnapshot()` покрыт Xml-тестом на несохранённую правку; UI-smoke
  **S101** проверяет глобальный переключатель на `Items.Weapons`, скрытие грид-хрома, объявление XML,
  записи, `IsReadOnly`, доступность кнопки копирования и возврат к `RowsGrid`; **S102** проверяет
  item-workspace `Items.Items`, **S103** — mercenary-gear `Inventory.MercStartingGear`, **S104** — nested
  map `Map.Cities`. Всего 726 unit-тестов, 98 автоматизированных UI-сценариев.
- Дальше по M27c: поиск по ID/ключу и переход из замечания валидации к строке/фрагменту XML.
