# XML Editor Reloaded

Редактор XML-таблиц `TableData` мода **Jagged Alliance 2 1.13** и просмотрщик STI-изображений
предметов. Работает с наборами данных v4 и v5, сохраняет структуру и лексическое представление
файлов, поддерживает русские и английские оверлеи переводов.

## Что умеет

- Просмотр и редактирование всех таблиц `TableData` в schema-driven grid.
- Дерево таблиц с папками, фильтром и поиском; отдельный список таблиц.
- Item master-detail, merc gear, shopkeeper и вспомогательные таблицы.
- Вложенные редакторы `Map.Cities` и `Map.FacilityTypes`.
- Lookup- и flag-редакторы, связи, pickers, массовые правки, undo/redo.
- Локализационные оверлеи: правка перевода пишется только в оверлей, база не меняется.
- Сравнение v4↔v5 по структуре и значениям, копирование строк между датасетами с remap.
- Проверка всего датасета одним отчётом и глобальный поиск по таблицам и оверлеям.
- Просмотр STI-изображений Big/Medium/Small с выбором кадра.
- Глобальный режим **Grid / XML**: read-only снимок базовой таблицы, поиск по ID/ключу и переход
  из замечания валидации к записи.
- Тёмная, светлая и системная темы; русский и английский интерфейс.

## Скачать и запустить

Два варианта, оба с файлом `.sha256` рядом:

| Архив | Требования |
|---|---|
| `XmlEditorReloaded-<версия>-win-x64.zip` | Self-contained: .NET устанавливать не нужно |
| `XmlEditorReloaded-<версия>-win-x64-framework-dependent.zip` | Нужен .NET 10 Desktop Runtime x64 |

1. Распакуйте архив в любую папку.
2. Запустите `XmlEditorReloaded.App.exe`.
3. Нажмите **Обзор** и выберите каталог игры, каталог `Data-1.13` или сразу `Data-1.13\TableData`.
4. Откройте таблицу двойным щелчком в дереве или списке.

Оба варианта требуют Windows 10/11 x64 и установленную игру с `Data-1.13\TableData`.
Framework-dependent сборка меньше, но запускается только при установленном
[.NET 10 Desktop Runtime x64](https://dotnet.microsoft.com/download/dotnet/10.0).

Для полностью переносимого режима запускайте `start-portable.cmd`: настройки, layout, метабаза и
лог будут лежать в подпапке `data` рядом с программой. Обычный запуск хранит их в
`%AppData%\XmlEditorReloaded`.

Подробная инструкция: [USER-GUIDE-RU.md](USER-GUIDE-RU.md) (русский) или
[USER-GUIDE.md](USER-GUIDE.md) (English).

## Безопасность данных

- Редактор меняет только те файлы, которые вы явно открыли и сохранили.
- Загрузка → сохранение без правок не перезаписывает файл.
- Неизвестные элементы и атрибуты сохраняются; порядок элементов и лексика значений не
  нормализуются без необходимости.
- Ошибки XML показываются пользователю; редактор не исправляет и не перезаписывает повреждённый
  файл автоматически.
- Валидация предупреждает, но ничего не удаляет и не нормализует.
- Перед первым сохранением сделайте резервную копию `TableData`: автоматического backup XML нет.

Настройки, layout, `metadata.sqlite` и `XmlEditorReloaded.log` находятся в
`%AppData%\XmlEditorReloaded` (или в `XER_APP_DATA`, если переменная задана).

## Сборка из исходников

Требуется .NET 10 SDK.

```powershell
dotnet restore xml-editor-reloaded/XmlEditorReloaded.slnx
dotnet build xml-editor-reloaded/XmlEditorReloaded.slnx -m:1 --no-restore -warnaserror
pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/test.ps1
```

Приложение запускается из `out/bin/XmlEditorReloaded.App/<Configuration>/XmlEditorReloaded.App.exe`.
Все `bin/obj` складываются в `out/`.

Полный набор тестов — 726 тестов в пяти xUnit-проектах. Корпусные, сравнительные, copy- и
perf-гейты описаны в `PLAN.md`; они требуют `XER_CORPUS_DIR` и временных копий данных.

## Сборка релиза

```powershell
pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/release.ps1
```

Скрипт проверяет версию из `Directory.Build.props`, запускает тесты, публикует self-contained и
framework-dependent `win-x64`, складывает `README`, `USER-GUIDE`, `USER-GUIDE-RU`, `CHANGELOG`,
лицензию и
third-party notices в каждый ZIP и пишет отдельный SHA-256. Результат: `out/release/`.

## Структура

| Каталог | Назначение |
|---|---|
| `src/XmlEditorReloaded.Core` | контракты и доменные примитивы, без I/O |
| `src/XmlEditorReloaded.Xml` | загрузка, lossless-сохранение, оверлеи, сравнение |
| `src/XmlEditorReloaded.Data` | SQLite-метабаза, каталог, настройки, лог |
| `src/XmlEditorReloaded.Imaging` | STI/ETRLE → RGBA |
| `src/XmlEditorReloaded.App` | WPF-оболочка и MVVM |
| `tests/` | xUnit-тесты и UI-smoke runner |

Архитектура и правила — в `PROJECT-STRUCTURE.md` и `AGENTS.md`, план — в `PLAN.md` исходного
репозитория. История версий — в [CHANGELOG.md](CHANGELOG.md).

## Лицензия

Проект распространяется по [GLWT(Good Luck With That) Public License](LICENSE). Сторонние
библиотеки сохраняют свои лицензии; полный список и тексты — в
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) и `licenses/`.
