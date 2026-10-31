---
name: ja2-editor-ui-smoke
description: Автоматический UI-smoke-раннер XmlEditorReloaded.UiSmoke — покрытые сценарии, запуск, изоляция %AppData% и обходные приёмы UIAutomation.
metadata:
  node_type: memory
  type: project
---

- **Что это:** консольный проект `xml-editor-reloaded/tests/XmlEditorReloaded.UiSmoke` (`net10.0-windows`, UI Automation + `SendInput`). Запускает настоящий `XmlEditorReloaded.App.exe`, проходит сценарии из `M23A-PARITY/MANUAL-SMOKE-CHECKLIST.md`, сверяет тексты с RESX и пишет `report.md`/`report.json`. В solution добавлен 2026-10-31; тестовым проектом не считается (`test.ps1` берёт только `*.Tests.csproj`).
- **Покрыто (2026-10-31, зелёные):** S01 холодный старт, S02 добавление v4, S03 переключение датасета с открытыми вкладками и несохранённой правкой, S05 детали/диагностика, S06 generic-таблица и переключение датасета, S07 поиск, S12 сохранение грязной вкладки, S13 закрытие вкладки (три ветки диалога), S14 «Сохранить всё» и закрытие приложения (Cancel/Save, чистая вкладка не спрашивается), S48 повреждённый v5-файл, S50 целостность `References/` (SHA-256 до/после). Ручными остаются S22, S71–S75, S82, S83 (зрение/рабочий стол); не автоматизированы S04 (диалог выбора папки), S08 фильтры, S09 массовая правка, S10 строки и S11 колонки/layout.
- **Запуск:** сначала сборка, затем `out/bin/XmlEditorReloaded.UiSmoke/debug/XmlEditorReloaded.UiSmoke.exe [--only S01,S12] [--work .tmp/ui-smoke/run] [--lang ru] [--list] [--keep]`. Нужен интерактивный рабочий стол: раннер переводит окно приложения на передний план и шлёт настоящие клики/клавиши. Сценарии с записью копируют `TableData`, `BigItems`, `Interface`, `tilesets` во временный каталог сценария; оригиналы в `References/` не трогаются.
- **Изоляция:** `XER_APP_DATA` (см. `AppPaths` в `Data/Settings`) перенаправляет `settings.json`, `layouts.json`, `metadata.sqlite` и лог в каталог сценария; `UiApp.Start` пишет туда стартовые настройки. `UiStrings` читает `Strings.resx` + `Strings.<lang>.resx` напрямую, чтобы проверять те же тексты, что показывает UI.
- **Грабли, уже учтённые в коде:**
  - модальный диалог: `InvokePattern.Invoke` не возвращается, пока диалог открыт → кнопку закрытия вкладки вызывают через `Task.Run`, а диалог обрабатывают в основном потоке;
  - синтетические клики попадают в окно, которое оказалось сверху (терминал агента) → перед кликом `AttachThreadInput` + `SetForegroundWindow` для top-level окна элемента;
  - редактирование ячейки надёжнее клавиатурой (`cell.SetFocus()`, F2, Ctrl+A, ввод, Enter), чем двойным щелчком;
  - дамп UIA надо снимать до закрытия приложения (иначе окон уже нет) — закрытие перенесено в `finally` после `DumpUi`;
  - `ValidationList` живёт в свёрнутом Expander (для повреждённого файла пуст) → ошибку разбора читают из `DiagnosticsList` в «Деталях»;
  - шаблон `name.*` в `Directory.GetFiles` матчит и сам файл → `ScenarioUi.BackupCopies`;
  - `StatusError` — формат `«Ошибка: {0}»` → сравнивать префикс через `ScenarioUi.FormatPrefix`;
  - AutomationId задаётся через `AutomationProperties.AutomationId` или `x:Name` (диалоги `UnsavedChangesWindow`/`ExternalChangeWindow`).
- **Проверка:** `pwsh -File xml-editor-reloaded/test.ps1` — сборка одним узлом MSBuild и пять xUnit-проектов (598 тестов). `dotnet test` на всём решении в песочнице DSH зависает (воркеры MSBuild/testhost), поэтому только `test.ps1` или `dotnet test <один project>.csproj`.
