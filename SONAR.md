# Локальный анализ SonarQube

Анализ выполняется **только на этой машине**: SonarQube находится в локальной сети
(`http://alpine-box.lan:9000`), поэтому GitHub Actions его не использует. В репозитории нет
workflow для SonarQube и нет токена.

| Параметр | Значение |
|---|---|
| Сервер | `http://alpine-box.lan:9000` |
| Project key | `XML-Editor-Reloaded` |
| Редакция | Community, без анализа веток |
| Что анализируется | текущая локальная сборка |

## 1. Токен

Создайте в корне репозитория файл `sonartoken.txt` и положите в него **только токен**:

```text
<ваш SonarQube token>
```

Файл добавлен в `.gitignore`; проверка:

```sh
git check-ignore sonartoken.txt
```

Токен не нужно присылать в чат. Скрипты читают его из файла, а если файла нет — из
переменной окружения `SONAR_TOKEN`. Для API-выгрузки issues лучше использовать **user token**
(а не project analysis token), иначе у токена может не быть прав на чтение issues.

## 2. Первый анализ

Убедитесь, что установлен сканер:

```sh
dotnet tool list --global
```

Если `dotnet-sonarscanner` отсутствует:

```sh
dotnet tool install --global dotnet-sonarscanner
```

Запустите полный локальный цикл:

```sh
pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/sonar-analyze.ps1
```

Скрипт выполняет:

```text
dotnet restore
dotnet sonarscanner begin
dotnet build
pwsh util-scripts/test.ps1 -NoBuild
dotnet sonarscanner end
```

Варианты:

```sh
# Release-сборка
pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/sonar-analyze.ps1 -Configuration Release

# Быстро: без xUnit-тестов
pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/sonar-analyze.ps1 -SkipTests

# Другой сервер/проект
pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/sonar-analyze.ps1 `
  -HostUrl http://alpine-box.lan:9000 `
  -ProjectKey XML-Editor-Reloaded
```

`sonarscanner begin/end` нельзя просто挂到 обычному `dotnet build`: сканер должен видеть
компиляцию между `begin` и `end`, поэтому для локального анализа используется wrapper-скрипт.

Community Edition не поддерживает анализ веток: анализируется текущий checkout как сборка
проекта, без `sonar.branch.name`.

## 3. Выгрузка результатов

После завершения анализа:

```sh
pwsh -NoProfile -ExecutionPolicy Bypass -File util-scripts/sonar-issues.ps1
```

Скрипт обращается к `/api/issues/search` и создаёт:

```text
.tmp/sonar/issues.json   # полный машинный список
.tmp/sonar/issues.md     # таблица для чтения и для ассистента
```

Открытые issues — по умолчанию; `-IncludeResolved` добавляет закрытые. Если нужно передать
список ассистенту, достаточно указать файл:

```text
@.tmp/sonar/issues.md
```

Токен в файлы не записывается.

## 4. Ограничения

- Сервер должен быть доступен с машины, где запускается `sonar-analyze.ps1`.
- Если на машине включён HTTP-прокси для интернета, скрипт выгрузки использует прямой
  запрос (`-NoProxy`), а анализ добавляет `alpine-box.lan` в `NO_PROXY`. Для API это важно:
  корпоративный прокси иначе отвечает `501` на LAN-адрес.
- Для приватного LAN-сервера не подходит GitHub-hosted runner; при необходимости используйте
  self-hosted runner, но это отдельное решение и в текущей конфигурации не настроено.
- Скрипты не меняют `References/` и не сохраняют XML-таблицы; они только собирают решение и
  запускают тесты.
