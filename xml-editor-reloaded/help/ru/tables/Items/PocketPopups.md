---
schema: 1
lang: ru
family: items-pockets
title: "PocketPopups — всплывающие меню карманов NIV"
tables:
  - PocketPopups
table_ids:
  - Items.PocketPopups
files:
  - Items/PocketPopups.xml
root: POCKETPOPUPS
record: POCKET
key: pIndex
sources:
  - Pocket popup table data
  - Game code (pocket-popup loader and popup definition)
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# PocketPopups — всплывающие меню карманов NIV

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`POCKETPOPUPS/POCKET/pIndex`](#pocketpopupspocketpindex)
  - [`POCKETPOPUPS/POCKET/pName`](#pocketpopupspocketpname)
  - [`POCKETPOPUPS/POCKET/popup/generator/id`](#pocketpopupspocketpopupgeneratorid)
  - [`POCKETPOPUPS/POCKET/popup/subMenu/name`](#pocketpopupspocketpopupsubmenuname)
  - [`POCKETPOPUPS/POCKET/popup/subMenu/generator/id`](#pocketpopupspocketpopupsubmenugeneratorid)
  - [`POCKETPOPUPS/POCKET/popup/subMenu/subMenu/name`](#pocketpopupspocketpopupsubmenusubmenuname)
  - [`POCKETPOPUPS/POCKET/popup/subMenu/subMenu/generator/id`](#pocketpopupspocketpopupsubmenusubmenugeneratorid)
  - [`POCKETPOPUPS/POCKET/generator/id`](#pocketpopupspocketgeneratorid)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`PocketPopups.xml` определяет всплывающее меню и вложенные подменю, показываемые для кармана в
новой системе инвентаря. Каждое всплывающее меню строится именованным генератором, а не
фиксированным списком предметов.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `PocketPopups` | `Items/PocketPopups.xml` | В v4 и v5 совпадают пути полей и, в эталонных данных, значения. |

## Structure

```
POCKETPOPUPS
└── POCKET
    ├── pIndex
    ├── pName
    ├── popup
    │   ├── generator
    │   │   └── id
    │   └── subMenu
    │       ├── name
    │       ├── generator
    │       │   └── id
    │       └── subMenu
    │           ├── name
    │           └── generator
    │               └── id
    └── generator
        └── id
```

## Fields

### `POCKETPOPUPS/POCKET/pIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Тип кармана, которому принадлежит это определение всплывающего меню. Это ключ карты `LBEPocketPopup` времени выполнения, в которой интерфейс ищет по типу кармана, возвращённому для слота; это же значение является `pIndex` соответствующей строки `Pockets`.
- **Values:** `0..255`; загрузчик приводит разобранное значение к `UINT8`. В поставляемых данных используются ID типов карманов `1..22`.
- **Default:** `0` в первой записи; последующая запись сохраняет предыдущий ID кармана, если элемент отсутствует, потому что разборщик не сбрасывает текущий ID для каждого `POCKET`.
- **Source:** game code (pocket-popup loader; pocket-type lookup); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/pName`

- **Type:** `String` (`String`)
- **Description:** Имя кармана, присутствующее в данных. Загрузчик всплывающих меню карманов его не читает, поэтому на меню оно не влияет; там, где имя кармана показывается в другом месте (например, в описаниях разгрузки у Бобби Рея), оно берётся из `Pockets`.
- **Values:** Свободный текст.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (pocket-popup loader); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/popup/generator/id`

- **Type:** `String` (`String`)
- **Description:** Генератор, который строит содержимое всплывающего меню верхнего уровня. Элемент может повторяться, и каждое вхождение добавляет генератор в меню. Неизвестные имена молча не дают генератора.
- **Values:** Имя генератора, принимаемое загрузчиком: `dummy`, `addArmor`, `addLBE`, `addWeapons`, `addWeaponGroups`, `addGrenades`, `addBombs`, `addFaceGear`, `addAmmo`, `addRifleGrenades`, `addRocketAmmo`, `addMisc` или `addKits`; любое другое значение (включая `none`) ничего не добавляет.
- **Default:** пустая строка, если элемент отсутствует; генератор не добавляется.
- **Source:** game code (pocket-popup loader and generator registry); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/popup/subMenu/name`

- **Type:** `String` (`String`)
- **Description:** Подпись подменю во всплывающем меню. Загрузчик сохраняет её в буфере на 128 широких символов (более длинный текст обрезается) и применяет к пункту подменю.
- **Values:** Свободный текст; подпись отображается во всплывающем меню.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (pocket-popup loader; submenu renaming); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/popup/subMenu/generator/id`

- **Type:** `String` (`String`)
- **Description:** Генератор, который строит содержимое подменю. Он использует тот же реестр имён, что и генератор верхнего уровня, а неизвестные имена ничего не добавляют.
- **Values:** Те же имена генераторов, что и в `popup/generator/id`.
- **Default:** пустая строка, если элемент отсутствует; генератор не добавляется.
- **Source:** game code (pocket-popup loader and generator registry); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/popup/subMenu/subMenu/name`

- **Type:** `String` (`String`)
- **Description:** Подпись вложенного подменю. Подменю могут вкладываться друг в друга; загрузчик отслеживает до `POPUP_MAX_SUB_POPUPS` (`24`) уровней.
- **Values:** Свободный текст; подпись отображается во вложенном меню.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (pocket-popup loader; nested submenu renaming); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/popup/subMenu/subMenu/generator/id`

- **Type:** `String` (`String`)
- **Description:** Генератор, который строит содержимое вложенного подменю. Он использует тот же реестр имён, что и генератор верхнего уровня, а неизвестные имена ничего не добавляют.
- **Values:** Те же имена генераторов, что и в `popup/generator/id`.
- **Default:** пустая строка, если элемент отсутствует; генератор не добавляется.
- **Source:** game code (pocket-popup loader and generator registry); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/generator/id`

- **Type:** `String` (`String`)
- **Description:** Генератор, размещённый прямо под `POCKET`, а не внутри обёртки `popup`. Загрузчик читает `generator` только внутри `popup` или `subMenu`, поэтому этот элемент игнорируется и меню не создаёт; тем не менее поставляемые данные содержат такие элементы (например, для `pIndex` `13`, `14`, `20` и `21`).
- **Values:** Любое имя генератора; в этой позиции загрузчик его не использует.
- **Default:** пустая строка, если элемент отсутствует; ни на что не влияет.
- **Source:** game code (pocket-popup loader); pocket popup table data.
- **Confidence:** `source`

## Relations

- `pIndex` — тип кармана, используемый как ключ `LBEPocketPopup` и соответствующий `pIndex` строки [Pockets](Pockets.md).
- ID генераторов — ключи в системе всплывающих меню инвентаря, а не ID предметов.

## Localization

`pName` встречается в оверлеях локализации, но загрузчик его не читает, поэтому он не влияет на
отображаемый карман; имя кармана берётся из локализованной таблицы `Pockets`. Подписи `name`
подменю отображаются из таблицы дословно, а ID генераторов — это ключи кода, которые не
переводятся.

## Versions

В v4 и v5 совпадают пути полей, и эталонные файлы содержат одинаковые значения.

## Sources

- Данные таблицы всплывающих меню карманов.
- Код игры (загрузчик всплывающих меню карманов, реестр генераторов и определение меню).
- Метаданные предметов.
- Типы и пределы полей: метаданные предметов.
- Описания опираются на код игры и имена XML-элементов; записи, помеченные как `inferred`, не
  утверждают проверенную кодом формулу.

## Open questions

Поставляемые данные содержат прямые элементы `POCKET/generator`, которые загрузчик игнорирует. В
исходниках не сказано, ошибка ли это в данных или незавершённая возможность; таблица не может
заставить эти карманы использовать генератор без обёртки `popup`.
