# Ветки и релизы

Модель: `main` — только релизы, `dev` — разработка, `release/<version>` — стабилизация.
CI (`ci.yml`) для всех трёх веток делает только build и tests и не публикует артефакты.
Пакеты собирает и публикует только `release.yml`.

## 1. Обычная разработка

```bash
git switch dev
git pull --ff-only
```

Работа и коммиты идут в `dev`. Прямые коммиты в `main` запрещены.

## 2. Создание release-ветки

Когда релиз запланирован:

```bash
git switch dev
git pull --ff-only
git switch -c release/1.1.0
```

В `xml-editor-reloaded/Directory.Build.props` выставь ту же версию:

```xml
<Version>1.1.0</Version>
```

Обнови `xml-editor-reloaded/CHANGELOG.md`. Затем:

```bash
git add xml-editor-reloaded/Directory.Build.props xml-editor-reloaded/CHANGELOG.md
git commit -m "Release 1.1.0"
git push -u origin release/1.1.0
```

Разрешено cherry-pick изменений из `dev`:

```bash
git cherry-pick -x <commit>
```

или точечные правки прямо в `release/1.1.0`.

## 3. Pre-release

GitHub → **Actions → Release → Run workflow**:

- **Use workflow from**: `release/1.1.0`
- **publish_prerelease**: `true`, если нужно опубликовать pre-release;
  `false`, если нужен только artifact сборки.

Workflow соберёт оба ZIP и два `.sha256`, загрузит artifact и при
`publish_prerelease=true` создаст pre-release с тегом:

```text
pre-v1.1.0-<run_number>
```

Из `dev` и `main` артефакты не публикуются.

## 4. Финальный релиз

После проверки pre-release слей ветку в `dev` и `main`, затем удали её:

```bash
git switch main
git pull --ff-only
git merge --no-ff release/1.1.0
git push origin main

git switch dev
git pull --ff-only
git merge --no-ff release/1.1.0
git push origin dev

git branch -d release/1.1.0
git push origin --delete release/1.1.0
```

Создай annotated-тег на `main` и отправь его:

```bash
git switch main
git pull --ff-only
git tag -a v1.1.0 -m "XML Editor Reloaded 1.1.0"
git push origin v1.1.0
```

Пуш тега запускает финальную сборку и публикацию GitHub Release.

## 5. Правила тегов

- Финальный тег: `v1.1.0`; pre-release: `pre-v1.1.0-<run>`.
- Тег должен указывать на протестированный merge-коммит в `main`.
- Переставлять тег можно только пока релиз не опубликован.
- `git push --force` для `main` и `dev` запрещён; для тега — только при исправлении
  неудавшегося релиза до публикации.
