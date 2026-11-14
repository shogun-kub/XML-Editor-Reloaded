# Справка по TableData (русская версия)

Этот каталог — русский перевод английской справки из `help/`. Английские документы остаются
каноном: при расхождении смысла ориентируйтесь на них и на `help/GLOSSARY.md`.

- `tables/` повторяет структуру `help/tables/`; имена файлов, таблиц и полей не переводятся.
- `GLOSSARY.md` содержит утверждённые русские термины.
- `INDEX.md` и `COVERAGE.md` генерируются скриптом `util-scripts/help-check.ps1 -Write -Lang ru`.
- `TRANSLATION.md` показывает статус перевода по документам.
- Исходные имена XML-элементов, значения перечислений и пути полей остаются английскими.

Проверка структуры и полноты:

```sh
pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/help-check.ps1 -Lang ru
```

Генерация русского `help.ru.json`:

```sh
dotnet run --project tools/XmlEditorReloaded.HelpTool -- \
  --source help/ru/tables --language ru
```
