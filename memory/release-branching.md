---
name: release-branching
description: Branching and release model after 1.0.0 — main stable, dev active, release/version stabilization, manual pre-release and tag-driven final release.
metadata:
  node_type: memory
  type: project
---

**С 2026-11-06, после релиза 1.0.0.** Ветки:

- `main` — только стабильные релизы; прямые коммиты запрещены, только merge из
  `release/<version>`.
- `dev` — активная разработка; `.github/workflows/ci.yml` делает build/tests и **не** загружает
  артефакты.
- `release/<version>` — создаётся из `dev`, версия в `Directory.Build.props` совпадает с именем;
  разрешены cherry-pick из `dev` и правки на месте; CI делает build/tests.

**Pre-release.** `workflow_dispatch` из release-ветки (`release.yml`), вход
`publish_prerelease`; собираются оба ZIP, artifact загружается всегда, при `true` создаётся
pre-release с тегом `pre-v<version>-<run>`. Из `dev`/`main` артефакты не публикуются.

**Финальный релиз.** Release-ветка вливается в `dev` и `main`, удаляется; на `main` создаётся
annotated-тег `v<version>`. Пуш тега запускает `release.yml`, который собирает и публикует
GitHub Release с двумя ZIP и двумя `.sha256`.

**Теги.** Тег только на протестированный merge-коммит; переставлять можно только до публикации
релиза. `push --force` для `main`/`dev` запрещён. Команды — в `RELEASING.md`; правила —
в `AGENTS.md`, раздел «Ветки и релизы».
