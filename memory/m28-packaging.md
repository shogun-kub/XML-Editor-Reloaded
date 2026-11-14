---
name: m28-packaging
description: M28 portable release — version 1.0.0, icon, self-contained and framework-dependent win-x64 publishes, ZIPs with SHA-256, GLWTPL license, release script and CI/release workflows.
metadata:
  node_type: memory
  type: project
---

**M28 (2026-11-05).** Версия `1.0.0` в `Directory.Build.props`; AssemblyVersion/FileVersion `1.0.0.0`,
`IncludeSourceRevisionInInformationalVersion=false`. Брендинг (`Product`, `Company`, `Copyright`,
`Description`) — только в `XmlEditorReloaded.App.csproj`, иначе он протекает в тестовые сборки.

**Иконка.** `src/XmlEditorReloaded.App/Assets/app.ico`, 6 размеров (16–256), тёмная таблица с
оранжевой шапкой. `ApplicationIcon` задаёт иконку exe; окна используют
`pack://application:,,,/XmlEditorReloaded.App;component/Assets/app.ico` — assembly-qualified URI
нужен, потому что в тестах entry assembly не App.

**Релизный скрипт.** `util-scripts/release.ps1` читает версию из `Directory.Build.props`, при
`-Version` требует совпадения, прогоняет `test.ps1 -Configuration Release`, публикует
self-contained и framework-dependent `win-x64` (не single-file, не trimmed), копирует
README/USER-GUIDE/CHANGELOG/LICENSE/THIRD-PARTY-NOTICES/licenses/start-portable.cmd в каждый пакет
и пишет ZIP + `.sha256` в `out/release/`. Пакеты 1.0.0: self-contained 425 файлов, 64,3 МБ,
SHA-256 `0daa2075cb18a31ab68a7b54e2d6ce4c8cdf0fb16631fd2026b9b74ed3c736e1`; framework-dependent
30 файлов, 2,2 МБ, SHA-256 `8b9493e32a4c25843247c8b797e8c8f5851b6eb4d414d8384a0ac300a553d836`.
Framework-dependent требует .NET 10 Desktop Runtime x64; функция проверяет отсутствие
`coreclr.dll`/`hostfxr.dll`/`hostpolicy.dll` в его publish. `-SkipTests` — только для уже
проверенной сборки.

**Проверка распакованных ZIP (2026-11-06).** Оба архива распакованы после сверки checksums;
S01 + обязательный S50 — 2/2 для self-contained и framework-dependent. `start-portable.cmd` из
framework-dependent пакета создал `data` и `metadata.sqlite` с путём рядом с приложением
(`XER_APP_DATA`), запись в `References/` не было.

**Документы.** `README.md`, `USER-GUIDE.md` (English), `USER-GUIDE-RU.md` (Russian),
`CHANGELOG.md` (Russian), `CHANGELOG.en.md` (English), `LICENSE` (GLWTPL),
`THIRD-PARTY-NOTICES.md`, `licenses/MIT.txt`, `licenses/Apache-2.0.txt`. `start-portable.cmd`
задаёт `XER_APP_DATA=%~dp0data`, чтобы настройки/метабаза/лог оставались рядом с программой.

**CI.** `.github/workflows/ci.yml` — restore, Release build `-warnaserror`, тесты через `test.ps1
-NoBuild` для `main`, `dev` и `release/**`; артефакты не публикует. `.github/workflows/release.yml`:
`workflow_dispatch` из `release/<version>` собирает оба пакета и по флагу публикует очередной
pre-release `v<version>-pN`; пуш финального тега `v<version>` собирает и публикует GitHub Release.
Версия ветки/тега должна совпасть с базой в props. Процесс — в `RELEASING.md`.

**Проверка.** Build 0/0; release-прогон: 726 тестов (App 265, Core 62, Data 97, Imaging 24, Xml 278);
UI-smoke против `out/publish/win-x64/XmlEditorReloaded.App.exe` — PASS для S66/S68/S81/S101–S104;
framework-dependent exe — S01 + S50 PASS 2026-11-06
(About `1.0.0`, RU/EN; S68: переход из валидации; S66: editable lookup-combo; S50: 504 файла
`References/` не изменены). S81 иногда теряет синтетический Enter при повторном открытии About;
сценарий повторяет клавишу и проверяет исчезновение окна. S66 читал editable lookup-combo через
`SelectionPattern` и получал пустую строку до первого выбора; добавлен `Ui.ReadComboText`.

**Полный UI-smoke (2026-11-05):** 99/99 PASS (98 автоматических + S50), 0 failed. Финальный
automated gate 2026-11-05: corpus v4/v5 726/726, compare 1/1, copy 1/1, perf 9/9 на каждом корпусе.
Ручной v5-gate 2026-11-06: S04/S22/S53 PASS, S74 N/A (отдельной сетки нет), S13 error branch PASS;
baseline `out/manual-v5` unchanged. Кодовая подпись и установщик не входят в M28.
