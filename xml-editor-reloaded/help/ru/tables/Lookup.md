---
schema: 1
lang: ru
family: lookup
title: "Справочники"
tables:
  - AmmoChoices
  - AmmoFlag
  - ArmourClass
  - AttachmentClass
  - AttachmentPoint
  - AttachmentSystem
  - Cursor
  - DrugType
  - ExplosionSize
  - ExplosionType
  - ItemClass
  - ItemFlag
  - LBEClass
  - MagazineType
  - MergeType
  - NasAttachmentClass
  - NasLayoutClass
  - PocketSize
  - Separability
  - Silhouette
  - SkillCheckType
  - WeaponClass
  - WeaponType
table_ids:
  - Lookup.AmmoChoices
  - Lookup.AmmoFlag
  - Lookup.ArmourClass
  - Lookup.AttachmentClass
  - Lookup.AttachmentPoint
  - Lookup.AttachmentSystem
  - Lookup.Cursor
  - Lookup.DrugType
  - Lookup.ExplosionSize
  - Lookup.ExplosionType
  - Lookup.ItemClass
  - Lookup.ItemFlag
  - Lookup.LBEClass
  - Lookup.MagazineType
  - Lookup.MergeType
  - Lookup.NasAttachmentClass
  - Lookup.NasLayoutClass
  - Lookup.PocketSize
  - Lookup.Separability
  - Lookup.Silhouette
  - Lookup.SkillCheckType
  - Lookup.WeaponClass
  - Lookup.WeaponType
files:
  - Lookup/AmmoChoices.xml
  - Lookup/AmmoFlag.xml
  - Lookup/ArmourClass.xml
  - Lookup/AttachmentClass.xml
  - Lookup/AttachmentPoint.xml
  - Lookup/AttachmentSystem.xml
  - Lookup/Cursor.xml
  - Lookup/DrugType.xml
  - Lookup/ExplosionSize.xml
  - Lookup/ExplosionType.xml
  - Lookup/ItemClass.xml
  - Lookup/ItemFlag.xml
  - Lookup/LBEClass.xml
  - Lookup/MagazineType.xml
  - Lookup/MergeType.xml
  - Lookup/NasAttachmentClass.xml
  - Lookup/NasLayoutClass.xml
  - Lookup/PocketSize.xml
  - Lookup/Separability.xml
  - Lookup/Silhouette.xml
  - Lookup/SkillCheckType.xml
  - Lookup/WeaponClass.xml
  - Lookup/WeaponType.xml
root: JA2Data
record: "<lookup element>"
key: id
path_mode: relative
sources:
  - Lookup XML data
  - Editor metadata for flag/sequence kind
status: reviewed
updated: 2026-11-12
---

# Справочники

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Lookup details](#lookup-details)
  - [AmmoChoices](#ammochoices)
  - [AmmoFlag](#ammoflag)
  - [ArmourClass](#armourclass)
  - [AttachmentClass](#attachmentclass)
  - [AttachmentPoint](#attachmentpoint)
  - [AttachmentSystem](#attachmentsystem)
  - [Cursor](#cursor)
  - [DrugType](#drugtype)
  - [ExplosionSize](#explosionsize)
  - [ExplosionType](#explosiontype)
  - [ItemClass](#itemclass)
  - [ItemFlag](#itemflag)
  - [LBEClass](#lbeclass)
  - [MagazineType](#magazinetype)
  - [MergeType](#mergetype)
  - [NasAttachmentClass](#nasattachmentclass)
  - [NasLayoutClass](#naslayoutclass)
  - [PocketSize](#pocketsize)
  - [Separability](#separability)
  - [Silhouette](#silhouette)
  - [SkillCheckType](#skillchecktype)
  - [WeaponClass](#weaponclass)
  - [WeaponType](#weapontype)
- [Structure](#structure)
- [Fields](#fields)
  - [`id`](#id)
  - [`name`](#name)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

Каталог `Lookup` содержит небольшие словари, превращающие числовые значения в метки для редактора и
для моддеров. Это **данные стороны редактора**: код игры не читает эти файлы, он использует числовые
константы, которые отражают значения. Редактор использует их для раскрывающихся списков, редакторов
битов и представлений «где используется».

Все справочники имеют общую структуру. Каждый из них — либо словарь флагов/битовых масок
(`IsFlagLookup = 1`), либо последовательный список значений. Файлы разбираются и в v4, и в v5;
`Lookup/AttachmentPoint.xml` был исправлен 2026-11-07.

## Tables

Все файлы справочников используют `JA2Data` как корневой элемент документа и имя справочника как
элемент записи (`AmmoChoices`, `ItemClass`, ...).

| Справочник | Файл | Вид | Примечания |
|---|---|---|---|
| `AmmoChoices` | `Lookup/AmmoChoices.xml` | последовательный | разреженные ID для вариантов боеприпасов врагов/ополчения |
| `AmmoFlag` | `Lookup/AmmoFlag.xml` | маска | биты свойств боеприпасов |
| `ArmourClass` | `Lookup/ArmourClass.xml` | последовательный | соответствует `ARMOURCLASS_*` |
| `AttachmentClass` | `Lookup/AttachmentClass.xml` | маска | классы вложений OAS |
| `AttachmentPoint` | `Lookup/AttachmentPoint.xml` | маска | исправленный файл v5 |
| `AttachmentSystem` | `Lookup/AttachmentSystem.xml` | последовательный | любая / OAS / NAS |
| `Cursor` | `Lookup/Cursor.xml` | последовательный | ID курсоров |
| `DrugType` | `Lookup/DrugType.xml` | маска | биты эффектов препаратов |
| `ExplosionSize` | `Lookup/ExplosionSize.xml` | последовательный | размер взрыва |
| `ExplosionType` | `Lookup/ExplosionType.xml` | последовательный | вид взрыва |
| `ItemClass` | `Lookup/ItemClass.xml` | маска | битовые флаги `IC_*` |
| `ItemFlag` | `Lookup/ItemFlag.xml` | маска | флаги предметов, 64-битные |
| `LBEClass` | `Lookup/LBEClass.xml` | последовательный | класс разгрузки |
| `MagazineType` | `Lookup/MagazineType.xml` | последовательный | магазин / пуля / коробка / ящик |
| `MergeType` | `Lookup/MergeType.xml` | последовательный | вид преобразования слиянием |
| `NasAttachmentClass` | `Lookup/NasAttachmentClass.xml` | маска | классы вложений NAS |
| `NasLayoutClass` | `Lookup/NasLayoutClass.xml` | маска | классы компоновки NAS |
| `PocketSize` | `Lookup/PocketSize.xml` | последовательный | размер кармана |
| `Separability` | `Lookup/Separability.xml` | последовательный | съёмный / неотделимый / заменяемый |
| `Silhouette` | `Lookup/Silhouette.xml` | последовательный | силуэт предмета |
| `SkillCheckType` | `Lookup/SkillCheckType.xml` | последовательный | вид проверки навыка |
| `WeaponClass` | `Lookup/WeaponClass.xml` | последовательный | `WEAPONCLASS_*` |
| `WeaponType` | `Lookup/WeaponType.xml` | последовательный | `WEAPONTYPE_*` |

## Lookup details

Каждая запись повторяет вид справочника и источник его значений. Полные списки значений остаются в
XML-файлах; редактор и будущее окно справки читают их оттуда.

### AmmoChoices

Последовательный справочник с разреженными ID. Используется таблицами вариантов боеприпасов
врагов/ополчения, а не напрямую данными предметов.

### AmmoFlag

Справочник-маска. Биты свойств боеприпасов, используемые `AmmoTypes.ammoflag`.

### ArmourClass

Последовательный справочник. Соответствует `ARMOURCLASS_*`; используется
[Armours](Items/Armours.md).

### AttachmentClass

Справочник-маска. Биты классов вложений OAS.

### AttachmentPoint

Справочник-маска. Используется полями слотов вложений и NAS.

### AttachmentSystem

Последовательный справочник: любая, только OAS, только NAS.

### Cursor

Последовательный справочник. ID курсоров, используемые интерфейсом.

### DrugType

Справочник-маска. Биты эффектов препаратов, используемые `Drugs`.

### ExplosionSize

Последовательный справочник, от отсутствия до большого.

### ExplosionType

Последовательный справочник, например обычный, оглушающий и слезоточивый газ.

### ItemClass

Справочник-маска. Биты классов предметов `IC_*`.

### ItemFlag

Справочник-маска. Флаги предметов, включая 64-битные значения.

### LBEClass

Последовательный справочник. Класс разгрузки.

### MagazineType

Последовательный справочник: магазин, пуля(и), коробка, ящик. Соответствует `AMMO_MAGAZINE`,
`AMMO_BULLET`, `AMMO_BOX`, `AMMO_CRATE`; используется [Magazines](Items/Magazines.md).

### MergeType

Последовательный справочник. Вид слияния/преобразования.

### NasAttachmentClass

Справочник-маска. Биты классов вложений NAS.

### NasLayoutClass

Справочник-маска. Биты классов компоновки NAS.

### PocketSize

Последовательный справочник: нет, малый, средний, большой.

### Separability

Последовательный справочник: съёмный, неотделимый, заменяемый.

### Silhouette

Последовательный справочник. Силуэт предмета, используемый отображением инвентаря.

### SkillCheckType

Последовательный справочник. Вид проверки навыка.

### WeaponClass

Последовательный справочник. `WEAPONCLASS_*`, используемые `Weapons.ubWeaponClass`.

### WeaponType

Последовательный справочник. `WEAPONTYPE_*`, используемые `Weapons.ubWeaponType`.

## Structure

```
JA2Data
├── id
└── name
```

- `id` может быть разреженным (в `AmmoChoices` используются 19, 29, ... 79).
- `name` — метка, а не ключ локализации. Имена английские и не переводятся оверлеями `Russian.*`.
- Справочники-маски могут содержать в `id` составные значения; строка может описывать как группу
  битов, так и один бит.
- Порядок записей не важен; редактор сортирует по `id`.

## Fields

### `id`

- **Type:** `Integer` (up to 64-bit for `ItemFlag`)
- **Description:** Числовой идентификатор значения. Для справочников-масок это бит или комбинация
  битов; для последовательных справочников — порядковый номер или явный ID.
- **Values:** `0..max`; разреженные ID допускаются (`AmmoChoices`).
- **Default:** нет; значение должно существовать в файле.
- **Source:** lookup XML data; editor metadata for flag/sequence kind.
- **Confidence:** `source`

### `name`

- **Type:** `String`
- **Description:** Отображаемая метка значения, показываемая в раскрывающихся списках и списках битов
  редактора. Для составного значения-маски она описывает группу битов.
- **Values:** свободный текст; на практике уникален в пределах одного справочника.
- **Default:** нет.
- **Source:** lookup XML data; editor metadata for flag/sequence kind.
- **Confidence:** `source`

## Relations

Справочники отражают числовые константы, используемые полями предметов и оружия. Примеры:

- `Items.usItemClass` и `Items.<flag fields>` используют `ItemClass` и `ItemFlag`.
- `Weapons.ubWeaponClass` / `ubWeaponType` используют `WeaponClass` и `WeaponType`.
- `Armours.ubArmourClass` использует `ArmourClass`.
- `Magazines.ubMagType` использует `MagazineType`.

Документы таблиц — источник истины о том, какое поле использует какой справочник; эта страница только
определяет словари.

## Localization

Имена справочников не переводятся механизмом оверлеев игры. Русский интерфейс может переводить
метку, показываемую рядом со справочником, но значения `name` в этих файлах остаются английскими.

## Versions

- В v4 и v5 те же файлы справочников и та же схема `id`/`name`.
- `Lookup/AttachmentPoint.xml` в v5 был исправлен 2026-11-07; теперь он разбирается и содержит
  значения точек вложения:

| id | name |
|---:|---|
| 0 | None |
| 1 | Small Molle |
| 2 | Medium Molle |
| 3 | All Molle |
| 4 | Hydration Carrier |
| 7 | All Molle and Carrier |
| 8 | Surgical Mask and Gloves |
| 16 | Batteries |

Исправление — это правка данных, а не изменение кода. `help-inventory.ps1` по-прежнему откатывается
на v4, если будущий файл v5 не разберётся; в текущем дереве v5 откат не используется.

## Sources

- `TableData/Lookup/*.xml` (v4 и v5) — значения и имена.
- `metadata: Tables.IsFlagLookup` — классификация «маска или последовательность».
- Константы игры, которые отражают значения: классы и флаги предметов, классы брони, классы и типы
  оружия, флаги боеприпасов и виды проверок навыков.
- Ни один загрузчик игры не читает `TableData/Lookup/*.xml`; это данные редактора и моддинга.

## Open questions

- Совпадают ли все наборы ID справочников с текущими константами кода? Проход B0 проверил
  количества, виды и диапазоны, но не каждую константу с её заголовком.
- Следует ли выводить составные строки-маски как отдельные значения или хранить их только в файле
  справочника? Редактор показывает их как значения; документы не дублируют полный список значений.
