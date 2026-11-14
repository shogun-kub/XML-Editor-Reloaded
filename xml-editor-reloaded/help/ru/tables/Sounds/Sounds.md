---
schema: 1
lang: ru
family: sounds
title: "Sounds — списки файлов звуков и звуков очереди"
tables:
  - Sounds
  - BurstSounds
table_ids:
  - Sounds.Sounds
  - Sounds.BurstSounds
files:
  - Sounds/Sounds.xml
  - Sounds/BurstSounds.xml
root: SOUNDLIST
record: SOUND
key: none
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# Sounds — списки файлов звуков и звуков очереди

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SOUNDLIST/SOUND`](#soundlistsound)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Sounds.xml` и `BurstSounds.xml` сопоставляют числовые ID звуков с путями к WAV-файлам. Позиция
элемента `SOUND` — это его ID; `BurstSounds` использует ту же раскладку для звуков огня очередями.
`NOSOUND` — допустимый заполнитель.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Sounds` | `Sounds/Sounds.xml` | — |
| `BurstSounds` | `Sounds/BurstSounds.xml` | — |

## Structure

```
SOUNDLIST
└── SOUND
```

## Fields

### `SOUNDLIST/SOUND`

- **Type:** `String` (`CHAR8`)
- **Description:** Путь к звуковому файлу относительно каталога игровых данных. Отсчитываемая от
  нуля позиция в списке — это ID звука; элемент может повторяться, и порядок важен. Это ссылка на
  файл.
- **Values:** Путь, например `SOUNDS\RICOCHET 01.wav`; значения усекаются до 255 символов.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (`XML_Sounds.cpp`); table data.
- **Confidence:** `source`

## Relations

- На ID звуков ссылаются [Weapons](../Items/Weapons.md), [Vehicles](../Vehicles.md) и поля
  интерфейса в других местах TableData.

## Localization

Отображаемые имена и метки локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не заявляется.

## Open questions

- Точная формула или сопоставление перечисления для выведенных полей в данных таблицы не указаны.
