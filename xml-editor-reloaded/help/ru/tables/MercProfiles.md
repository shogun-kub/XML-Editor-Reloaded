---
schema: 1
lang: ru
family: mercs-profiles
title: "MercProfiles — профили наёмников и NPC"
tables:
  - MercProfiles
table_ids:
  - MercProfiles
files:
  - MercProfiles.xml
root: MERCPROFILES
record: PROFILE
key: uiIndex
sources:
  - Merc profile table data
  - MercProfiles schema comments
status: reviewed
updated: 2026-11-12
---

# MercProfiles — профили наёмников и NPC

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MERCPROFILES/PROFILE/uiIndex`](#mercprofilesprofileuiindex)
  - [`MERCPROFILES/PROFILE/Type`](#mercprofilesprofiletype)
  - [`MERCPROFILES/PROFILE/zName`](#mercprofilesprofilezname)
  - [`MERCPROFILES/PROFILE/zNickname`](#mercprofilesprofileznickname)
  - [`MERCPROFILES/PROFILE/ubFaceIndex`](#mercprofilesprofileubfaceindex)
  - [`MERCPROFILES/PROFILE/usVoiceIndex`](#mercprofilesprofileusvoiceindex)
  - [`MERCPROFILES/PROFILE/usEyesX`](#mercprofilesprofileuseyesx)
  - [`MERCPROFILES/PROFILE/usEyesY`](#mercprofilesprofileuseyesy)
  - [`MERCPROFILES/PROFILE/usMouthX`](#mercprofilesprofileusmouthx)
  - [`MERCPROFILES/PROFILE/usMouthY`](#mercprofilesprofileusmouthy)
  - [`MERCPROFILES/PROFILE/uiEyeDelay`](#mercprofilesprofileuieyedelay)
  - [`MERCPROFILES/PROFILE/uiMouthDelay`](#mercprofilesprofileuimouthdelay)
  - [`MERCPROFILES/PROFILE/uiBlinkFrequency`](#mercprofilesprofileuiblinkfrequency)
  - [`MERCPROFILES/PROFILE/uiExpressionFrequency`](#mercprofilesprofileuiexpressionfrequency)
  - [`MERCPROFILES/PROFILE/PANTS`](#mercprofilesprofilepants)
  - [`MERCPROFILES/PROFILE/VEST`](#mercprofilesprofilevest)
  - [`MERCPROFILES/PROFILE/SKIN`](#mercprofilesprofileskin)
  - [`MERCPROFILES/PROFILE/HAIR`](#mercprofilesprofilehair)
  - [`MERCPROFILES/PROFILE/bSex`](#mercprofilesprofilebsex)
  - [`MERCPROFILES/PROFILE/ubBodyType`](#mercprofilesprofileubbodytype)
  - [`MERCPROFILES/PROFILE/uiBodyTypeSubFlags`](#mercprofilesprofileuibodytypesubflags)
  - [`MERCPROFILES/PROFILE/bAttitude`](#mercprofilesprofilebattitude)
  - [`MERCPROFILES/PROFILE/bCharacterTrait`](#mercprofilesprofilebcharactertrait)
  - [`MERCPROFILES/PROFILE/bDisability`](#mercprofilesprofilebdisability)
  - [`MERCPROFILES/PROFILE/ubNeedForSleep`](#mercprofilesprofileubneedforsleep)
  - [`MERCPROFILES/PROFILE/bReputationTolerance`](#mercprofilesprofilebreputationtolerance)
  - [`MERCPROFILES/PROFILE/bDeathRate`](#mercprofilesprofilebdeathrate)
  - [`MERCPROFILES/PROFILE/bLifeMax`](#mercprofilesprofileblifemax)
  - [`MERCPROFILES/PROFILE/bLife`](#mercprofilesprofileblife)
  - [`MERCPROFILES/PROFILE/bStrength`](#mercprofilesprofilebstrength)
  - [`MERCPROFILES/PROFILE/bAgility`](#mercprofilesprofilebagility)
  - [`MERCPROFILES/PROFILE/bDexterity`](#mercprofilesprofilebdexterity)
  - [`MERCPROFILES/PROFILE/bWisdom`](#mercprofilesprofilebwisdom)
  - [`MERCPROFILES/PROFILE/bMarksmanship`](#mercprofilesprofilebmarksmanship)
  - [`MERCPROFILES/PROFILE/bExplosive`](#mercprofilesprofilebexplosive)
  - [`MERCPROFILES/PROFILE/bLeadership`](#mercprofilesprofilebleadership)
  - [`MERCPROFILES/PROFILE/bMedical`](#mercprofilesprofilebmedical)
  - [`MERCPROFILES/PROFILE/bMechanical`](#mercprofilesprofilebmechanical)
  - [`MERCPROFILES/PROFILE/bExpLevel`](#mercprofilesprofilebexplevel)
  - [`MERCPROFILES/PROFILE/fRegresses`](#mercprofilesprofilefregresses)
  - [`MERCPROFILES/PROFILE/bOldSkillTrait`](#mercprofilesprofileboldskilltrait)
  - [`MERCPROFILES/PROFILE/bOldSkillTrait2`](#mercprofilesprofileboldskilltrait2)
  - [`MERCPROFILES/PROFILE/bNewSkillTrait1`](#mercprofilesprofilebnewskilltrait1)
  - [`MERCPROFILES/PROFILE/bNewSkillTrait2`](#mercprofilesprofilebnewskilltrait2)
  - [`MERCPROFILES/PROFILE/bNewSkillTrait3`](#mercprofilesprofilebnewskilltrait3)
  - [`MERCPROFILES/PROFILE/bNewSkillTrait4`](#mercprofilesprofilebnewskilltrait4)
  - [`MERCPROFILES/PROFILE/bBuddy1`](#mercprofilesprofilebbuddy1)
  - [`MERCPROFILES/PROFILE/bBuddy2`](#mercprofilesprofilebbuddy2)
  - [`MERCPROFILES/PROFILE/bBuddy3`](#mercprofilesprofilebbuddy3)
  - [`MERCPROFILES/PROFILE/bBuddy4`](#mercprofilesprofilebbuddy4)
  - [`MERCPROFILES/PROFILE/bBuddy5`](#mercprofilesprofilebbuddy5)
  - [`MERCPROFILES/PROFILE/bLearnToLike`](#mercprofilesprofileblearntolike)
  - [`MERCPROFILES/PROFILE/bLearnToLikeTime`](#mercprofilesprofileblearntoliketime)
  - [`MERCPROFILES/PROFILE/bHated1`](#mercprofilesprofilebhated1)
  - [`MERCPROFILES/PROFILE/bHatedTime1`](#mercprofilesprofilebhatedtime1)
  - [`MERCPROFILES/PROFILE/bHated2`](#mercprofilesprofilebhated2)
  - [`MERCPROFILES/PROFILE/bHatedTime2`](#mercprofilesprofilebhatedtime2)
  - [`MERCPROFILES/PROFILE/bHated3`](#mercprofilesprofilebhated3)
  - [`MERCPROFILES/PROFILE/bHatedTime3`](#mercprofilesprofilebhatedtime3)
  - [`MERCPROFILES/PROFILE/bHated4`](#mercprofilesprofilebhated4)
  - [`MERCPROFILES/PROFILE/bHatedTime4`](#mercprofilesprofilebhatedtime4)
  - [`MERCPROFILES/PROFILE/bHated5`](#mercprofilesprofilebhated5)
  - [`MERCPROFILES/PROFILE/bHatedTime5`](#mercprofilesprofilebhatedtime5)
  - [`MERCPROFILES/PROFILE/bLearnToHate`](#mercprofilesprofileblearntohate)
  - [`MERCPROFILES/PROFILE/bLearnToHateTime`](#mercprofilesprofileblearntohatetime)
  - [`MERCPROFILES/PROFILE/bRace`](#mercprofilesprofilebrace)
  - [`MERCPROFILES/PROFILE/bNationality`](#mercprofilesprofilebnationality)
  - [`MERCPROFILES/PROFILE/bAppearance`](#mercprofilesprofilebappearance)
  - [`MERCPROFILES/PROFILE/bAppearanceCareLevel`](#mercprofilesprofilebappearancecarelevel)
  - [`MERCPROFILES/PROFILE/bRefinement`](#mercprofilesprofilebrefinement)
  - [`MERCPROFILES/PROFILE/bRefinementCareLevel`](#mercprofilesprofilebrefinementcarelevel)
  - [`MERCPROFILES/PROFILE/bHatedNationality`](#mercprofilesprofilebhatednationality)
  - [`MERCPROFILES/PROFILE/bHatedNationalityCareLevel`](#mercprofilesprofilebhatednationalitycarelevel)
  - [`MERCPROFILES/PROFILE/bRacist`](#mercprofilesprofilebracist)
  - [`MERCPROFILES/PROFILE/bSexist`](#mercprofilesprofilebsexist)
  - [`MERCPROFILES/PROFILE/sSalary`](#mercprofilesprofilessalary)
  - [`MERCPROFILES/PROFILE/uiWeeklySalary`](#mercprofilesprofileuiweeklysalary)
  - [`MERCPROFILES/PROFILE/uiBiWeeklySalary`](#mercprofilesprofileuibiweeklysalary)
  - [`MERCPROFILES/PROFILE/bMedicalDeposit`](#mercprofilesprofilebmedicaldeposit)
  - [`MERCPROFILES/PROFILE/sMedicalDepositAmount`](#mercprofilesprofilesmedicaldepositamount)
  - [`MERCPROFILES/PROFILE/usOptionalGearCost`](#mercprofilesprofileusoptionalgearcost)
  - [`MERCPROFILES/PROFILE/bArmourAttractiveness`](#mercprofilesprofilebarmourattractiveness)
  - [`MERCPROFILES/PROFILE/bMainGunAttractiveness`](#mercprofilesprofilebmaingunattractiveness)
  - [`MERCPROFILES/PROFILE/fGoodGuy`](#mercprofilesprofilefgoodguy)
  - [`MERCPROFILES/PROFILE/usApproachFactorFriendly`](#mercprofilesprofileusapproachfactorfriendly)
  - [`MERCPROFILES/PROFILE/usApproachFactorDirect`](#mercprofilesprofileusapproachfactordirect)
  - [`MERCPROFILES/PROFILE/usApproachFactorThreaten`](#mercprofilesprofileusapproachfactorthreaten)
  - [`MERCPROFILES/PROFILE/usApproachFactorRecruit`](#mercprofilesprofileusapproachfactorrecruit)
  - [`MERCPROFILES/PROFILE/sSectorX`](#mercprofilesprofilessectorx)
  - [`MERCPROFILES/PROFILE/sSectorY`](#mercprofilesprofilessectory)
  - [`MERCPROFILES/PROFILE/sSectorZ`](#mercprofilesprofilessectorz)
  - [`MERCPROFILES/PROFILE/ubCivilianGroup`](#mercprofilesprofileubciviliangroup)
  - [`MERCPROFILES/PROFILE/bTown`](#mercprofilesprofilebtown)
  - [`MERCPROFILES/PROFILE/bTownAttachment`](#mercprofilesprofilebtownattachment)
  - [`MERCPROFILES/PROFILE/usBackground`](#mercprofilesprofileusbackground)
  - [`MERCPROFILES/PROFILE/GrowthModifierLife`](#mercprofilesprofilegrowthmodifierlife)
  - [`MERCPROFILES/PROFILE/GrowthModifierStrength`](#mercprofilesprofilegrowthmodifierstrength)
  - [`MERCPROFILES/PROFILE/GrowthModifierAgility`](#mercprofilesprofilegrowthmodifieragility)
  - [`MERCPROFILES/PROFILE/GrowthModifierDexterity`](#mercprofilesprofilegrowthmodifierdexterity)
  - [`MERCPROFILES/PROFILE/GrowthModifierWisdom`](#mercprofilesprofilegrowthmodifierwisdom)
  - [`MERCPROFILES/PROFILE/GrowthModifierMarksmanship`](#mercprofilesprofilegrowthmodifiermarksmanship)
  - [`MERCPROFILES/PROFILE/GrowthModifierExplosive`](#mercprofilesprofilegrowthmodifierexplosive)
  - [`MERCPROFILES/PROFILE/GrowthModifierLeadership`](#mercprofilesprofilegrowthmodifierleadership)
  - [`MERCPROFILES/PROFILE/GrowthModifierMedical`](#mercprofilesprofilegrowthmodifiermedical)
  - [`MERCPROFILES/PROFILE/GrowthModifierMechanical`](#mercprofilesprofilegrowthmodifiermechanical)
  - [`MERCPROFILES/PROFILE/GrowthModifierExpLevel`](#mercprofilesprofilegrowthmodifierexplevel)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MercProfiles.xml` задаёт полный лист персонажа каждого наёмника, RPC и NPC: характеристики, навыки, черты, внешность, зарплаты, стартовый сектор, координаты портрета и взаимоотношения.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `MercProfiles` | `MercProfiles.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
MERCPROFILES
└── PROFILE
    ├── uiIndex
    ├── Type
    ├── zName
    ├── zNickname
    ├── ubFaceIndex
    ├── usVoiceIndex
    ├── usEyesX
    ├── usEyesY
    ├── usMouthX
    ├── usMouthY
    ├── uiEyeDelay
    ├── uiMouthDelay
    ├── uiBlinkFrequency
    ├── uiExpressionFrequency
    ├── PANTS
    ├── VEST
    ├── SKIN
    ├── HAIR
    ├── bSex
    ├── ubBodyType
    ├── uiBodyTypeSubFlags
    ├── bAttitude
    ├── bCharacterTrait
    ├── bDisability
    ├── ubNeedForSleep
    ├── bReputationTolerance
    ├── bDeathRate
    ├── bLifeMax
    ├── bLife
    ├── bStrength
    ├── bAgility
    ├── bDexterity
    ├── bWisdom
    ├── bMarksmanship
    ├── bExplosive
    ├── bLeadership
    ├── bMedical
    ├── bMechanical
    ├── bExpLevel
    ├── fRegresses
    ├── bOldSkillTrait
    ├── bOldSkillTrait2
    ├── bNewSkillTrait1
    ├── bNewSkillTrait2
    ├── bNewSkillTrait3
    ├── bNewSkillTrait4
    ├── bBuddy1
    ├── bBuddy2
    ├── bBuddy3
    ├── bBuddy4
    ├── bBuddy5
    ├── bLearnToLike
    ├── bLearnToLikeTime
    ├── bHated1
    ├── bHatedTime1
    ├── bHated2
    ├── bHatedTime2
    ├── bHated3
    ├── bHatedTime3
    ├── bHated4
    ├── bHatedTime4
    ├── bHated5
    ├── bHatedTime5
    ├── bLearnToHate
    ├── bLearnToHateTime
    ├── bRace
    ├── bNationality
    ├── bAppearance
    ├── bAppearanceCareLevel
    ├── bRefinement
    ├── bRefinementCareLevel
    ├── bHatedNationality
    ├── bHatedNationalityCareLevel
    ├── bRacist
    ├── bSexist
    ├── sSalary
    ├── uiWeeklySalary
    ├── uiBiWeeklySalary
    ├── bMedicalDeposit
    ├── sMedicalDepositAmount
    ├── usOptionalGearCost
    ├── bArmourAttractiveness
    ├── bMainGunAttractiveness
    ├── fGoodGuy
    ├── usApproachFactorFriendly
    ├── usApproachFactorDirect
    ├── usApproachFactorThreaten
    ├── usApproachFactorRecruit
    ├── sSectorX
    ├── sSectorY
    ├── sSectorZ
    ├── ubCivilianGroup
    ├── bTown
    ├── bTownAttachment
    ├── usBackground
    ├── GrowthModifierLife
    ├── GrowthModifierStrength
    ├── GrowthModifierAgility
    ├── GrowthModifierDexterity
    ├── GrowthModifierWisdom
    ├── GrowthModifierMarksmanship
    ├── GrowthModifierExplosive
    ├── GrowthModifierLeadership
    ├── GrowthModifierMedical
    ├── GrowthModifierMechanical
    └── GrowthModifierExpLevel
```

## Fields

### `MERCPROFILES/PROFILE/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Идентификатор профиля и его индекс в массиве профилей.
- **Values:** `0..4294967295`; загрузчик разбирает это поле как `UINT32` и использует его как индекс массива.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/Type`

- **Type:** `Integer` (`UINT32`)
- **Description:** Тип профиля: отсутствует, AIM, MERC, RPC, NPC, транспорт или IMP.
- **Values:** `0` нет, `1` AIM, `2` MERC, `3` RPC, `4` NPC, `5` транспорт, `6` IMP.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/zName`

- **Type:** `String` (`CHAR16`)
- **Description:** Полное имя наёмника или NPC.
- **Values:** Свободный текст.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/zNickname`

- **Type:** `String` (`CHAR16`)
- **Description:** Короткое имя, используемое в диалогах и списке наёмников.
- **Values:** Свободный текст.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/ubFaceIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс графики лица, используемой для портрета профиля.
- **Values:** `0..254`; если профили не читаются из XML, загрузчик подставляет `uiIndex` профиля.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usVoiceIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Номер голосового набора, из которого строятся имена файлов диалогов и речи профиля.
- **Values:** `0..254`; если профиль не читается из XML, по умолчанию используется ID профиля.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usEyesX`

- **Type:** `Integer` (`UINT16`)
- **Description:** Координата X глаз на портрете.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз по карте или портрету, отрицательные координаты выходят за допустимый диапазон.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usEyesY`

- **Type:** `Integer` (`UINT16`)
- **Description:** Координата Y глаз на портрете.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз по карте или портрету, отрицательные координаты выходят за допустимый диапазон.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usMouthX`

- **Type:** `Integer` (`UINT16`)
- **Description:** Координата X рта на портрете.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз по карте или портрету, отрицательные координаты выходят за допустимый диапазон.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usMouthY`

- **Type:** `Integer` (`UINT16`)
- **Description:** Координата Y рта на портрете.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз по карте или портрету, отрицательные координаты выходят за допустимый диапазон.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiEyeDelay`

- **Type:** `Integer` (`UINT32`)
- **Description:** Задержка анимации глаз, хранимая в профиле. Текущий отрисовщик лица игнорирует это значение и использует фиксированную задержку `50 + Random(30)` мс.
- **Numeric semantics:** Миллисекунды; большие значения означали бы более медленную анимацию глаз.
- **Values:** Миллисекунды; более узкий диапазон не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiMouthDelay`

- **Type:** `Integer` (`UINT32`)
- **Description:** Задержка анимации рта, хранимая в профиле. Текущий отрисовщик лица игнорирует это значение и использует фиксированную задержку 120 мс.
- **Numeric semantics:** Миллисекунды; большие значения означали бы более медленную анимацию рта.
- **Values:** Миллисекунды; более узкий диапазон не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiBlinkFrequency`

- **Type:** `Integer` (`UINT32`)
- **Description:** Интервал между морганиями в анимации портрета.
- **Numeric semantics:** Миллисекунды; большие значения означают больший интервал между морганиями (моргание реже). В профилях IMP по умолчанию `3000`.
- **Values:** Миллисекунды; более узкий диапазон не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiExpressionFrequency`

- **Type:** `Integer` (`UINT32`)
- **Description:** Интервал между сменами выражения лица в анимации портрета.
- **Numeric semantics:** Миллисекунды; большие значения означают больший интервал между сменами выражения (смены реже). В профилях IMP по умолчанию `2000`.
- **Values:** Миллисекунды; более узкий диапазон не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/PANTS`

- **Type:** `String` (`CHAR8`)
- **Description:** Идентификатор замены палитры штанов (например, `BLUEPANTS` или `GREENPANTS`).
- **Values:** Имя замены палитры, до 29 символов.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/VEST`

- **Type:** `String` (`CHAR8`)
- **Description:** Идентификатор замены палитры жилета (например, `BLUEPANTS` или `GREENPANTS`).
- **Values:** Имя замены палитры, до 29 символов.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/SKIN`

- **Type:** `String` (`CHAR8`)
- **Description:** Идентификатор замены палитры кожи (например, `BLUEPANTS` или `GREENPANTS`).
- **Values:** Имя замены палитры, до 29 символов.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/HAIR`

- **Type:** `String` (`CHAR8`)
- **Description:** Идентификатор замены палитры волос (например, `BLUEPANTS` или `GREENPANTS`).
- **Values:** Имя замены палитры, до 29 символов.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bSex`

- **Type:** `Integer` (`INT8`)
- **Description:** Пол профиля.
- **Values:** `0` мужской, `1` женский.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/ubBodyType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Тип телосложения, используемый для анимаций и портретов.
- **Values:** `0` обычный мужской, `1` крупный мужской, `2` коренастый мужской, `3` обычный женский; остальные значения — типы телосложения существ, гражданских и транспорта.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiBodyTypeSubFlags`

- **Type:** `Integer` (`UINT32`)
- **Description:** Подфлаги анимации для типа телосложения (подстановки типа телосложения).
- **Numeric semantics:** Битовая маска или перечисляемый код; смысл несут отдельные биты/варианты, поэтому большее число не означает просто «больше». Отрицательные значения не имеют смысла.
- **Values:** Битовая маска; отдельные биты в коде профилей не документированы.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bAttitude`

- **Type:** `Integer` (`INT8`)
- **Description:** Исходное отношение (характер). Применяется только при отключённой новой системе черт (STOMP); при новой системе вместо него используется `bCharacterTrait`.
- **Values:** `0` обычное, `1` дружелюбное, `2` одиночка, `3` оптимист, `4` пессимист, `5` агрессивное, `6` высокомерное, `7` важная персона, `8` хам, `9` трус.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bCharacterTrait`

- **Type:** `Integer` (`INT8`)
- **Description:** Черта характера STOMP. Применяется при включённой новой системе черт (STOMP); иначе используется `bAttitude`.
- **Values:** Черта характера STOMP: `0` нейтральный, `1` общительный, `2` одиночка, `3` оптимист, `4` напористый, `5` интеллектуал, `6` примитивный, `7` агрессивный, `8` флегматичный, `9` бесстрашный, `10` пацифист, `11` злобный, `12` позёр, `13` трус.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bDisability`

- **Type:** `Integer` (`INT8`)
- **Description:** Ограничение STOMP или его отсутствие.
- **Values:** Ограничение: `0` нет, `1` непереносимость жары, `2` нервный, `3` клаустрофобия, `4` не умеет плавать, `5` боязнь насекомых, `6` забывчивый, `7` психопат, `8` глухой, `9` близорукий, `10` гемофилик, `11` боязнь высоты, `12` самоповреждение.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/ubNeedForSleep`

- **Type:** `Integer` (`UINT8`)
- **Description:** Базовое количество часов сна, необходимое профилю в сутки.
- **Numeric semantics:** Часы в сутки; игра ограничивает базовое значение диапазоном `3..12` до применения модификаторов травм, предыстории и болезней (итоговый максимум `18`). Большие значения заставляют персонажа уставать быстрее.
- **Values:** Часы в сутки, обычно `3..12`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bReputationTolerance`

- **Type:** `Integer` (`INT8`)
- **Description:** Терпимость к плохой репутации игрока: если показатель плохой репутации превышает это значение, наёмник отказывается или жалуется. `101` означает, что наёмнику всё равно.
- **Numeric semantics:** Большие значения допускают худшую репутацию; `101` отключает проверку.
- **Values:** Обычно `0..100`; `101` = игнорировать.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bDeathRate`

- **Type:** `Integer` (`INT8`)
- **Description:** Терпимость к расчётному уровню смертности: если расчётное значение превышает это, наёмник отказывается или жалуется. `101` означает, что наёмнику всё равно.
- **Numeric semantics:** Большие значения допускают более высокий уровень смертности; `101` отключает проверку.
- **Values:** Обычно `0..100`; `101` = игнорировать.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLifeMax`

- **Type:** `Integer` (`INT8`)
- **Description:** Максимальный запас здоровья (очки здоровья) профиля.
- **Numeric semantics:** Большие значения означают больше очков здоровья; отрицательные выходят за обычный диапазон здоровья.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLife`

- **Type:** `Integer` (`INT8`)
- **Description:** Начальный текущий запас здоровья при создании бойца.
- **Numeric semantics:** Большие значения означают больше очков здоровья; отрицательные выходят за обычный диапазон здоровья.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bStrength`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое значение характеристики «Сила» при создании профиля.
- **Numeric semantics:** Большие значения означают лучшую характеристику; отрицательные выходят за обычный диапазон и не имеют определённого смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bAgility`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое значение характеристики «Ловкость» при создании профиля.
- **Numeric semantics:** Большие значения означают лучшую характеристику; отрицательные выходят за обычный диапазон и не имеют определённого смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bDexterity`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое значение характеристики «Проворство» при создании профиля.
- **Numeric semantics:** Большие значения означают лучшую характеристику; отрицательные выходят за обычный диапазон и не имеют определённого смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bWisdom`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое значение характеристики «Мудрость» при создании профиля.
- **Numeric semantics:** Большие значения означают лучшую характеристику; отрицательные выходят за обычный диапазон и не имеют определённого смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bMarksmanship`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое значение характеристики «Меткость» при создании профиля.
- **Numeric semantics:** Большие значения означают лучшую характеристику; отрицательные выходят за обычный диапазон и не имеют определённого смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bExplosive`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое значение характеристики «Взрывное дело» при создании профиля.
- **Numeric semantics:** Большие значения означают лучшую характеристику; отрицательные выходят за обычный диапазон и не имеют определённого смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLeadership`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое значение характеристики «Лидерство» при создании профиля.
- **Numeric semantics:** Большие значения означают лучшую характеристику; отрицательные выходят за обычный диапазон и не имеют определённого смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bMedical`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое значение характеристики «Медицина» при создании профиля.
- **Numeric semantics:** Большие значения означают лучшую характеристику; отрицательные выходят за обычный диапазон и не имеют определённого смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bMechanical`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое значение характеристики «Техника» при создании профиля.
- **Numeric semantics:** Большие значения означают лучшую характеристику; отрицательные выходят за обычный диапазон и не имеют определённого смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bExpLevel`

- **Type:** `Integer` (`INT8`)
- **Description:** Начальный уровень опыта.
- **Numeric semantics:** Большие значения означают более опытный профиль; отрицательные выходят за обычный диапазон уровней.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/fRegresses`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если `TRUE`, профиль не развивается: броски на улучшение характеристик пропускаются в `ProcessStatChange`.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bOldSkillTrait`

- **Type:** `Integer` (`INT8`)
- **Description:** Первый исходный (до STOMP) навык-черта. Используется только при отключённой новой системе черт.
- **Values:** ID исходного навыка: `0` нет, `1` взлом замков, `2` рукопашный бой, `3` электроника, `4` ночные операции, `5` метание, `6` обучение, `7` тяжёлое оружие, `8` автоматическое оружие, `9` скрытность, `10` амбидекстрия, `11` воровство, `12` боевые искусства, `13` ножевой бой, `14` снайпинг с крыш, `15` маскировка.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bOldSkillTrait2`

- **Type:** `Integer` (`INT8`)
- **Description:** Второй исходный (до STOMP) навык-черта.
- **Values:** ID второго исходного навыка; тот же список значений, что у `bOldSkillTrait`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bNewSkillTrait1`

- **Type:** `Integer` (`INT8`)
- **Description:** Навык или черта STOMP в слоте 1. Используется при включённой новой системе черт.
- **Values:** ID навыка/черты STOMP. Основные: `1` автоматическое оружие, `2` тяжёлое оружие, `3` стрелок, `4` охотник, `5` ганслингер, `6` рукопашный бой, `7` заместитель, `8` техник, `9` фельдшер, `20` тайные операции. Второстепенные: `10` амбидекстрия, `11` ближний бой, `12` метание, `13` ночные операции, `14` скрытность, `15` атлетика, `16` бодибилдинг, `17` подрывное дело, `18` обучение, `19` разведка, `21` радист, `22` осведомитель, `23` выживание.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bNewSkillTrait2`

- **Type:** `Integer` (`INT8`)
- **Description:** Навык или черта STOMP в слоте 2. Используется при включённой новой системе черт.
- **Values:** ID навыка/черты STOMP; тот же список значений, что у `bNewSkillTrait1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bNewSkillTrait3`

- **Type:** `Integer` (`INT8`)
- **Description:** Навык или черта STOMP в слоте 3. Используется при включённой новой системе черт.
- **Values:** ID навыка/черты STOMP; тот же список значений, что у `bNewSkillTrait1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bNewSkillTrait4`

- **Type:** `Integer` (`INT8`)
- **Description:** Навык или черта STOMP в слоте 4. Используется при включённой новой системе черт.
- **Values:** ID навыка/черты STOMP; тот же список значений, что у `bNewSkillTrait1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bBuddy1`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID профиля-друга в слоте 1. `255` (записывается как `-1` в таблице) означает, что слот пуст.
- **Values:** ID профиля; `255`/`-1` означает пустой слот.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bBuddy2`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID профиля-друга в слоте 2. `255` (записывается как `-1` в таблице) означает, что слот пуст.
- **Values:** ID профиля; `255`/`-1` означает пустой слот.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bBuddy3`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID профиля-друга в слоте 3. `255` (записывается как `-1` в таблице) означает, что слот пуст.
- **Values:** ID профиля; `255`/`-1` означает пустой слот.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bBuddy4`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID профиля-друга в слоте 4. `255` (записывается как `-1` в таблице) означает, что слот пуст.
- **Values:** ID профиля; `255`/`-1` означает пустой слот.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bBuddy5`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID профиля-друга в слоте 5. `255` (записывается как `-1` в таблице) означает, что слот пуст.
- **Values:** ID профиля; `255`/`-1` означает пустой слот.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLearnToLike`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID профиля, к которому персонаж постепенно привыкает во время совместной службы; `255` означает отсутствие.
- **Values:** ID профиля; `255`/`-1` означает отсутствие.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLearnToLikeTime`

- **Type:** `Integer` (`INT8`)
- **Description:** Начальное количество часов, необходимое, чтобы привыкнуть к `bLearnToLike` во время совместной службы; уменьшается каждый час.
- **Numeric semantics:** Часы; `0` применяет состояние «привык» сразу, большие значения откладывают его. Отрицательные значения не имеют смысла.
- **Values:** Часы, обычно `0..100`; `255` (хранится как `-1`) используется, когда `bLearnToLike` пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHated1`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID ненавидимого профиля в слоте 1; `255` означает, что слот пуст.
- **Values:** ID профиля `0..255`; `255` означает пустой слот. Загрузчик хранит это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedTime1`

- **Type:** `Integer` (`INT8`)
- **Description:** Длительность отношения ненависти в слоте 1, уменьшается каждый час.
- **Numeric semantics:** Часы; большие значения означают, что ненависть длится дольше. `0` означает отсутствие терпимости: ветка жалобы/ухода срабатывает, как только ненавидимый профиль оказывается в отряде. Пустые слоты тоже хранят `0`.
- **Values:** Часы; загрузчик хранит это поле как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHated2`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID ненавидимого профиля в слоте 2; `255` означает, что слот пуст.
- **Values:** ID профиля `0..255`; `255` означает пустой слот. Загрузчик хранит это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedTime2`

- **Type:** `Integer` (`INT8`)
- **Description:** Длительность отношения ненависти в слоте 2, уменьшается каждый час.
- **Numeric semantics:** Часы; большие значения означают, что ненависть длится дольше. `0` означает отсутствие терпимости: ветка жалобы/ухода срабатывает, как только ненавидимый профиль оказывается в отряде. Пустые слоты тоже хранят `0`.
- **Values:** Часы; загрузчик хранит это поле как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHated3`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID ненавидимого профиля в слоте 3; `255` означает, что слот пуст.
- **Values:** ID профиля `0..255`; `255` означает пустой слот. Загрузчик хранит это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedTime3`

- **Type:** `Integer` (`INT8`)
- **Description:** Длительность отношения ненависти в слоте 3, уменьшается каждый час.
- **Numeric semantics:** Часы; большие значения означают, что ненависть длится дольше. `0` означает отсутствие терпимости: ветка жалобы/ухода срабатывает, как только ненавидимый профиль оказывается в отряде. Пустые слоты тоже хранят `0`.
- **Values:** Часы; загрузчик хранит это поле как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHated4`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID ненавидимого профиля в слоте 4; `255` означает, что слот пуст.
- **Values:** ID профиля `0..255`; `255` означает пустой слот. Загрузчик хранит это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedTime4`

- **Type:** `Integer` (`INT8`)
- **Description:** Длительность отношения ненависти в слоте 4, уменьшается каждый час.
- **Numeric semantics:** Часы; большие значения означают, что ненависть длится дольше. `0` означает отсутствие терпимости: ветка жалобы/ухода срабатывает, как только ненавидимый профиль оказывается в отряде. Пустые слоты тоже хранят `0`.
- **Values:** Часы; загрузчик хранит это поле как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHated5`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID ненавидимого профиля в слоте 5; `255` означает, что слот пуст.
- **Values:** ID профиля `0..255`; `255` означает пустой слот. Загрузчик хранит это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedTime5`

- **Type:** `Integer` (`INT8`)
- **Description:** Длительность отношения ненависти в слоте 5, уменьшается каждый час.
- **Numeric semantics:** Часы; большие значения означают, что ненависть длится дольше. `0` означает отсутствие терпимости: ветка жалобы/ухода срабатывает, как только ненавидимый профиль оказывается в отряде. Пустые слоты тоже хранят `0`.
- **Values:** Часы; загрузчик хранит это поле как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLearnToHate`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID профиля, к которому персонаж постепенно начинает испытывать ненависть во время совместной службы; `255` означает отсутствие.
- **Values:** ID профиля; `255`/`-1` означает отсутствие.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLearnToHateTime`

- **Type:** `Integer` (`INT8`)
- **Description:** Начальное количество часов, необходимое, чтобы возненавидеть `bLearnToHate` во время совместной службы; уменьшается каждый час.
- **Numeric semantics:** Часы; `0` применяет состояние «возненавидел» сразу, большие значения откладывают его. Отрицательные значения не имеют смысла.
- **Values:** Часы; `0..120` в поставляемой таблице.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bRace`

- **Type:** `Integer` (`INT8`)
- **Description:** Категория расы, используемая проверками расизма.
- **Values:** `0` белый, `1` чёрный, `2` азиат, `3` эскимос, `4` латиноамериканец.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bNationality`

- **Type:** `Integer` (`INT8`)
- **Description:** ID национальности, используемый проверками национальности и предрассудков.
- **Values:** ID национальности `0..112` (список в комментарии таблицы); `-1` допустим только для `bHatedNationality`. При загрузке значения ограничиваются диапазоном `0..112`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bAppearance`

- **Type:** `Integer` (`INT8`)
- **Description:** Категория внешности, используемая профилем и диалогами.
- **Values:** `0` средняя, `1` уродливая, `2` простая, `3` привлекательная, `4` красотка.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bAppearanceCareLevel`

- **Type:** `Integer` (`INT8`)
- **Description:** Насколько профиль заботится о внешности: `0` нет, `1` немного, `2` крайне.
- **Values:** `0` нет, `1` немного, `2` крайне; ограничивается загрузчиком.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bRefinement`

- **Type:** `Integer` (`INT8`)
- **Description:** Категория утончённости: обычная, неряха или сноб.
- **Values:** `0` обычная, `1` неряха, `2` сноб.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bRefinementCareLevel`

- **Type:** `Integer` (`INT8`)
- **Description:** Насколько профиль заботится об утончённости: `0` нет, `1` немного, `2` крайне.
- **Values:** `0` нет, `1` немного, `2` крайне; ограничивается загрузчиком.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedNationality`

- **Type:** `Integer` (`INT8`)
- **Description:** Национальность, которую профиль ненавидит, или `-1`, если такой нет.
- **Values:** ID национальности `0..112`; `-1` означает отсутствие ненавидимой нации.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedNationalityCareLevel`

- **Type:** `Integer` (`INT8`)
- **Description:** Насколько сильно профиль реагирует на ненавидимую национальность: `0` нет, `1` немного, `2` крайне.
- **Values:** `0` нет, `1` немного, `2` крайне; ограничивается загрузчиком.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bRacist`

- **Type:** `Integer` (`INT8`)
- **Description:** Уровень расизма.
- **Values:** `0` нет, `1` немного, `2` очень.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bSexist`

- **Type:** `Integer` (`UINT8`)
- **Description:** Уровень сексизма.
- **Values:** `0` нет, `1` слегка, `2` очень, `3` джентльмен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/sSalary`

- **Type:** `Integer` (`INT16`)
- **Description:** Дневная зарплата наёмника.
- **Numeric semantics:** Деньги в сутки; используется как плата по дневным контрактам и как основа страховой премии. Отрицательные значения не являются осмысленными суммами.
- **Values:** Денежная сумма.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiWeeklySalary`

- **Type:** `Integer` (`UINT32`)
- **Description:** Недельная зарплата.
- **Numeric semantics:** Деньги в неделю; дневной эквивалент — `uiWeeklySalary / 7`.
- **Values:** Денежная сумма.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiBiWeeklySalary`

- **Type:** `Integer` (`UINT32`)
- **Description:** Зарплата за две недели.
- **Numeric semantics:** Деньги за две недели; дневной эквивалент — `uiBiWeeklySalary / 14`.
- **Values:** Денежная сумма.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bMedicalDeposit`

- **Type:** `Integer` (`INT8`)
- **Description:** Требуется ли наёмнику медицинский залог (`0` = нет, ненулевое значение = да).
- **Values:** `0` залог не нужен, `1` залог требуется.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/sMedicalDepositAmount`

- **Type:** `Integer` (`UINT16`)
- **Description:** Размер медицинского залога. Если профили не читаются из XML, он вычисляется по компетентности наёмника; если XML-данные читаются, используется значение из таблицы.
- **Numeric semantics:** Денежная сумма; большие значения означают больший залог.
- **Values:** Денежная сумма, хранится как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usOptionalGearCost`

- **Type:** `Integer` (`UINT16`)
- **Description:** Стоимость дополнительного стартового комплекта снаряжения. Вычисляется по предметам выбранного комплекта и их полям цены (`mAbsolutePrice` или `mPriceMod`) и добавляется к стоимости контракта.
- **Numeric semantics:** Денежная сумма; при загрузке значение из XML перезаписывается вычисленной ценой комплекта.
- **Values:** Денежная сумма, хранится как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bArmourAttractiveness`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое качество брони. Наёмник жалуется, если защита лучшей брони (с поправкой на боевой дух) опускается ниже половины этого значения; `-1` означает, что в комплекте нет брони.
- **Numeric semantics:** Значение защиты брони; большие значения требуют лучшей брони. `-1` означает отсутствие базового уровня брони.
- **Values:** `-1`, если брони нет; иначе вычисленное значение защиты.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bMainGunAttractiveness`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое качество основного оружия. Наёмник жалуется, если убойная сила лучшего оружия (с поправкой на боевой дух) опускается ниже половины этого значения; `-1` означает, что в комплекте нет оружия.
- **Numeric semantics:** Значение убойной силы оружия; большие значения требуют лучшего оружия. `-1` означает отсутствие базового уровня оружия.
- **Values:** `-1`, если оружия нет; иначе вычисленное значение убойной силы оружия.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/fGoodGuy`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Помечает профиль как «хорошего парня»: устанавливает `PROFILE_MISC_FLAG3_GOODGUY` в `ubMiscFlags3`.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usApproachFactorFriendly`

- **Type:** `Integer` (`UINT16`)
- **Description:** Процент коэффициента подхода, используемый для дружелюбных диалогов и проверок найма.
- **Numeric semantics:** Проценты: `100` = ×1.0 (норма); значение делится на 100 и умножается на лидерство наёмника (или эффективность угроз). Большие значения улучшают подход, меньшие — ухудшают.
- **Values:** Проценты, `0..300` в поставляемой таблице.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usApproachFactorDirect`

- **Type:** `Integer` (`UINT16`)
- **Description:** Процент коэффициента подхода, используемый для прямых диалогов и проверок найма.
- **Numeric semantics:** Проценты: `100` = ×1.0 (норма); значение делится на 100 и умножается на лидерство наёмника (или эффективность угроз). Большие значения улучшают подход, меньшие — ухудшают.
- **Values:** Проценты, `0..300` в поставляемой таблице.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usApproachFactorThreaten`

- **Type:** `Integer` (`UINT16`)
- **Description:** Процент коэффициента подхода, используемый для диалогов с угрозами.
- **Numeric semantics:** Проценты: `100` = ×1.0 (норма); значение делится на 100 и умножается на эффективность угроз наёмника. Большие значения улучшают подход, меньшие — ухудшают.
- **Values:** Проценты, `0..300` в поставляемой таблице.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usApproachFactorRecruit`

- **Type:** `Integer` (`UINT16`)
- **Description:** Процент коэффициента подхода, используемый для диалогов найма.
- **Numeric semantics:** Проценты: `100` = ×1.0 (норма); значение делится на 100 и умножается на лидерство наёмника. Большие значения улучшают подход, меньшие — ухудшают.
- **Values:** Проценты, `0..300` в поставляемой таблице.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/sSectorX`

- **Type:** `Integer` (`UINT16`)
- **Description:** Сектор X, в котором начинает NPC или RPC. Сбрасывается в `0` для профилей AIM/MERC/IMP, когда профили читаются из XML.
- **Numeric semantics:** Координата X на карте; отрицательные значения выходят за допустимый диапазон.
- **Values:** Сектор X `1..16`; `0` означает, что не размещён.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/sSectorY`

- **Type:** `Integer` (`UINT16`)
- **Description:** Сектор Y, в котором начинает NPC или RPC. Сбрасывается в `0` для профилей AIM/MERC/IMP, когда профили читаются из XML.
- **Numeric semantics:** Координата Y на карте; отрицательные значения выходят за допустимый диапазон.
- **Values:** Сектор Y `1..16` (ряды A..P); `0` означает, что не размещён.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/sSectorZ`

- **Type:** `Integer` (`INT8`)
- **Description:** Сектор Z (подземный уровень), в котором начинает NPC или RPC. Сбрасывается в `0` для профилей AIM/MERC/IMP, когда профили читаются из XML.
- **Numeric semantics:** `0` = поверхность; большие значения выбирают более глубокие подземные уровни. Отрицательные значения выходят за допустимый диапазон.
- **Values:** `0` или `1` в поставляемой таблице; более глубокие подземные уровни используют большие значения.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/ubCivilianGroup`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID группы гражданских, используемый ИИ гражданских и обработкой фракций.
- **Values:** ID группы; `0` = без группы гражданских (`NON_CIV_GROUP`).
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bTown`

- **Type:** `Integer` (`INT8`)
- **Description:** ID родного города профиля, используемый для лояльности города и готовности NPC. Сбрасывается в `0` для профилей AIM/MERC/IMP, когда профили читаются из XML.
- **Values:** ID города; `0` = нет.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bTownAttachment`

- **Type:** `Integer` (`INT8`)
- **Description:** Весовой процент, используемый при расчёте вклада местного NPC в лояльность города. `100` — обычное значение; меньшие значения уменьшают местную надбавку.
- **Numeric semantics:** Проценты; `100` — норма, значения ниже `100` уменьшают вклад в лояльность города, отрицательные значения не имеют смысла.
- **Values:** Проценты, обычно `0..100`; загрузчик хранит это поле как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usBackground`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс строки в таблице предысторий (`Backgrounds.xml`).
- **Values:** Индекс строки предыстории; `0` означает отсутствие предыстории.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierLife`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор роста для «Здоровья» при развитии персонажа. Отрицательные значения ускоряют рост, `0` нейтрально, положительные замедляют. Игнорируется, если `fRegresses` равно `TRUE` или модификаторы роста отключены. Примечание: загрузчик в этой версии исходников сопоставляет теги `bGrowthModifier*`, а в таблице используются `GrowthModifier*`, поэтому значение может не применяться.
- **Numeric semantics:** Субпункты, добавляемые к порогу роста за очко: отрицательные значения снижают его (рост быстрее), `0` нейтрально, положительные повышают (рост медленнее). `fRegresses = TRUE` отключает модификаторы роста.
- **Values:** `-32768..32767`; в поставляемой таблице используется `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierStrength`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор роста для «Силы» при развитии персонажа. Отрицательные значения ускоряют рост, `0` нейтрально, положительные замедляют. Игнорируется, если `fRegresses` равно `TRUE` или модификаторы роста отключены. Примечание: загрузчик в этой версии исходников сопоставляет теги `bGrowthModifier*`, а в таблице используются `GrowthModifier*`, поэтому значение может не применяться.
- **Numeric semantics:** Субпункты, добавляемые к порогу роста за очко: отрицательные значения снижают его (рост быстрее), `0` нейтрально, положительные повышают (рост медленнее). `fRegresses = TRUE` отключает модификаторы роста.
- **Values:** `-32768..32767`; в поставляемой таблице используется `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierAgility`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор роста для «Ловкости» при развитии персонажа. Отрицательные значения ускоряют рост, `0` нейтрально, положительные замедляют. Игнорируется, если `fRegresses` равно `TRUE` или модификаторы роста отключены. Примечание: загрузчик в этой версии исходников сопоставляет теги `bGrowthModifier*`, а в таблице используются `GrowthModifier*`, поэтому значение может не применяться.
- **Numeric semantics:** Субпункты, добавляемые к порогу роста за очко: отрицательные значения снижают его (рост быстрее), `0` нейтрально, положительные повышают (рост медленнее). `fRegresses = TRUE` отключает модификаторы роста.
- **Values:** `-32768..32767`; в поставляемой таблице используется `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierDexterity`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор роста для «Проворства» при развитии персонажа. Отрицательные значения ускоряют рост, `0` нейтрально, положительные замедляют. Игнорируется, если `fRegresses` равно `TRUE` или модификаторы роста отключены. Примечание: загрузчик в этой версии исходников сопоставляет теги `bGrowthModifier*`, а в таблице используются `GrowthModifier*`, поэтому значение может не применяться.
- **Numeric semantics:** Субпункты, добавляемые к порогу роста за очко: отрицательные значения снижают его (рост быстрее), `0` нейтрально, положительные повышают (рост медленнее). `fRegresses = TRUE` отключает модификаторы роста.
- **Values:** `-32768..32767`; в поставляемой таблице используется `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierWisdom`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор роста для «Мудрости» при развитии персонажа. Отрицательные значения ускоряют рост, `0` нейтрально, положительные замедляют. Игнорируется, если `fRegresses` равно `TRUE` или модификаторы роста отключены. Примечание: загрузчик в этой версии исходников сопоставляет теги `bGrowthModifier*`, а в таблице используются `GrowthModifier*`, поэтому значение может не применяться.
- **Numeric semantics:** Субпункты, добавляемые к порогу роста за очко: отрицательные значения снижают его (рост быстрее), `0` нейтрально, положительные повышают (рост медленнее). `fRegresses = TRUE` отключает модификаторы роста.
- **Values:** `-32768..32767`; в поставляемой таблице используется `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierMarksmanship`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор роста для «Меткости» при развитии персонажа. Отрицательные значения ускоряют рост, `0` нейтрально, положительные замедляют. Игнорируется, если `fRegresses` равно `TRUE` или модификаторы роста отключены. Примечание: загрузчик в этой версии исходников сопоставляет теги `bGrowthModifier*`, а в таблице используются `GrowthModifier*`, поэтому значение может не применяться.
- **Numeric semantics:** Субпункты, добавляемые к порогу роста за очко: отрицательные значения снижают его (рост быстрее), `0` нейтрально, положительные повышают (рост медленнее). `fRegresses = TRUE` отключает модификаторы роста.
- **Values:** `-32768..32767`; в поставляемой таблице используется `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierExplosive`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор роста для «Взрывного дела» при развитии персонажа. Отрицательные значения ускоряют рост, `0` нейтрально, положительные замедляют. Игнорируется, если `fRegresses` равно `TRUE` или модификаторы роста отключены. Примечание: загрузчик в этой версии исходников сопоставляет теги `bGrowthModifier*`, а в таблице используются `GrowthModifier*`, поэтому значение может не применяться.
- **Numeric semantics:** Субпункты, добавляемые к порогу роста за очко: отрицательные значения снижают его (рост быстрее), `0` нейтрально, положительные повышают (рост медленнее). `fRegresses = TRUE` отключает модификаторы роста.
- **Values:** `-32768..32767`; в поставляемой таблице используется `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierLeadership`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор роста для «Лидерства» при развитии персонажа. Отрицательные значения ускоряют рост, `0` нейтрально, положительные замедляют. Игнорируется, если `fRegresses` равно `TRUE` или модификаторы роста отключены. Примечание: загрузчик в этой версии исходников сопоставляет теги `bGrowthModifier*`, а в таблице используются `GrowthModifier*`, поэтому значение может не применяться.
- **Numeric semantics:** Субпункты, добавляемые к порогу роста за очко: отрицательные значения снижают его (рост быстрее), `0` нейтрально, положительные повышают (рост медленнее). `fRegresses = TRUE` отключает модификаторы роста.
- **Values:** `-32768..32767`; в поставляемой таблице используется `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierMedical`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор роста для «Медицины» при развитии персонажа. Отрицательные значения ускоряют рост, `0` нейтрально, положительные замедляют. Игнорируется, если `fRegresses` равно `TRUE` или модификаторы роста отключены. Примечание: загрузчик в этой версии исходников сопоставляет теги `bGrowthModifier*`, а в таблице используются `GrowthModifier*`, поэтому значение может не применяться.
- **Numeric semantics:** Субпункты, добавляемые к порогу роста за очко: отрицательные значения снижают его (рост быстрее), `0` нейтрально, положительные повышают (рост медленнее). `fRegresses = TRUE` отключает модификаторы роста.
- **Values:** `-32768..32767`; в поставляемой таблице используется `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierMechanical`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор роста для «Техники» при развитии персонажа. Отрицательные значения ускоряют рост, `0` нейтрально, положительные замедляют. Игнорируется, если `fRegresses` равно `TRUE` или модификаторы роста отключены. Примечание: загрузчик в этой версии исходников сопоставляет теги `bGrowthModifier*`, а в таблице используются `GrowthModifier*`, поэтому значение может не применяться.
- **Numeric semantics:** Субпункты, добавляемые к порогу роста за очко: отрицательные значения снижают его (рост быстрее), `0` нейтрально, положительные повышают (рост медленнее). `fRegresses = TRUE` отключает модификаторы роста.
- **Values:** `-32768..32767`; в поставляемой таблице используется `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierExpLevel`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор роста для уровня опыта при развитии персонажа. Отрицательные значения ускоряют рост, `0` нейтрально, положительные замедляют. Игнорируется, если `fRegresses` равно `TRUE` или модификаторы роста отключены. Примечание: загрузчик в этой версии исходников сопоставляет теги `bGrowthModifier*`, а в таблице используются `GrowthModifier*`, поэтому значение может не применяться.
- **Numeric semantics:** Субпункты, добавляемые к порогу роста за очко: отрицательные значения снижают его (рост быстрее), `0` нейтрально, положительные повышают (рост медленнее). `fRegresses = TRUE` отключает модификаторы роста.
- **Values:** `-32768..32767`; в поставляемой таблице используется `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

## Relations

- На `uiIndex` ссылаются `Inventory.MercStartingGear.mIndex`, `MercOpinions.uiIndex`, `MercQuote.uiIndex`, `MercAvailability.ProfilId` и `AIMAvailability.ProfilId`.
- `usBackground` указывает на [Backgrounds](Backgrounds.md); `ubCivilianGroup` — на [CivGroupNames](CivGroupNames.md).
- `ubFaceIndex` указывает на [RPCFacesSmall](RPCFacesSmall.md); `usVoiceIndex` — на [IMPVoices](IMPVoices.md).
- `bTown` и связанные с городом поля используют индексы городов из [Cities](Map/Cities.md).

## Localization

`zName`, `zNickname` и ключи палитр — отображаемые данные. Числовые характеристики, черты и ID не локализуются.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы профилей наёмников.
- Комментарии схемы MercProfiles.
- Игровой код: XML-загрузчик профилей, структура и загрузчик профиля, создание бойца, рост в кампании, отрисовщик лица, стратегический обработчик наёмников, стратегический статус.

## Open questions

Полный список национальностей ведётся в комментарии схемы таблицы и здесь не повторяется. Теги модификаторов роста в таблице записаны как `GrowthModifier*`, тогда как загрузчик в проанализированной версии исходников сопоставляет `bGrowthModifier*`, поэтому вопрос, читает ли поставляемая сборка эти значения, остаётся открытым.
