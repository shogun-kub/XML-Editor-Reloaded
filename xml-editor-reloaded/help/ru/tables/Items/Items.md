---
schema: 1
lang: ru
family: items-core
title: "Items — главная таблица предметов"
tables:
  - Items
table_ids:
  - Items.Items
files:
  - Items/Items.xml
root: ITEMLIST
record: ITEM
key: uiIndex
sources:
  - Item table data
  - Item property and flag metadata
  - Item lookup tables
  - Game C++ source (item loader)
status: reviewed
updated: 2026-11-12
---

# Items — главная таблица предметов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
  - [Item classes and `ubClassIndex`](#item-classes-and-ubclassindex)
- [Game options](#game-options)
- [Field map](#field-map)
- [Fields](#fields)
  - [`ITEMLIST/ITEM/uiIndex`](#itemlistitemuiindex)
  - [`ITEMLIST/ITEM/szItemName`](#itemlistitemszitemname)
  - [`ITEMLIST/ITEM/szLongItemName`](#itemlistitemszlongitemname)
  - [`ITEMLIST/ITEM/szItemDesc`](#itemlistitemszitemdesc)
  - [`ITEMLIST/ITEM/szBRName`](#itemlistitemszbrname)
  - [`ITEMLIST/ITEM/szBRDesc`](#itemlistitemszbrdesc)
  - [`ITEMLIST/ITEM/usItemClass`](#itemlistitemusitemclass)
  - [`ITEMLIST/ITEM/AttachmentClass`](#itemlistitemattachmentclass)
  - [`ITEMLIST/ITEM/nasAttachmentClass`](#itemlistitemnasattachmentclass)
  - [`ITEMLIST/ITEM/nasLayoutClass`](#itemlistitemnaslayoutclass)
  - [`ITEMLIST/ITEM/AvailableAttachmentPoint`](#itemlistitemavailableattachmentpoint)
  - [`ITEMLIST/ITEM/AttachmentPoint`](#itemlistitemattachmentpoint)
  - [`ITEMLIST/ITEM/AttachToPointAPCost`](#itemlistitemattachtopointapcost)
  - [`ITEMLIST/ITEM/ubClassIndex`](#itemlistitemubclassindex)
  - [`ITEMLIST/ITEM/ubCursor`](#itemlistitemubcursor)
  - [`ITEMLIST/ITEM/bSoundType`](#itemlistitembsoundtype)
  - [`ITEMLIST/ITEM/ubGraphicType`](#itemlistitemubgraphictype)
  - [`ITEMLIST/ITEM/ubGraphicNum`](#itemlistitemubgraphicnum)
  - [`ITEMLIST/ITEM/ubWeight`](#itemlistitemubweight)
  - [`ITEMLIST/ITEM/ubPerPocket`](#itemlistitemubperpocket)
  - [`ITEMLIST/ITEM/ItemSize`](#itemlistitemitemsize)
  - [`ITEMLIST/ITEM/ItemSizeBonus`](#itemlistitemitemsizebonus)
  - [`ITEMLIST/ITEM/usPrice`](#itemlistitemusprice)
  - [`ITEMLIST/ITEM/ubCoolness`](#itemlistitemubcoolness)
  - [`ITEMLIST/ITEM/bReliability`](#itemlistitembreliability)
  - [`ITEMLIST/ITEM/bRepairEase`](#itemlistitembrepairease)
  - [`ITEMLIST/ITEM/Inseparable`](#itemlistiteminseparable)
  - [`ITEMLIST/ITEM/BR_NewInventory`](#itemlistitembr_newinventory)
  - [`ITEMLIST/ITEM/BR_UsedInventory`](#itemlistitembr_usedinventory)
  - [`ITEMLIST/ITEM/BR_ROF`](#itemlistitembr_rof)
  - [`ITEMLIST/ITEM/DrugType`](#itemlistitemdrugtype)
  - [`ITEMLIST/ITEM/FoodType`](#itemlistitemfoodtype)
  - [`ITEMLIST/ITEM/usActionItemFlag`](#itemlistitemusactionitemflag)
  - [`ITEMLIST/ITEM/clothestype`](#itemlistitemclothestype)
  - [`ITEMLIST/ITEM/spreadPattern`](#itemlistitemspreadpattern)
  - [`ITEMLIST/ITEM/Alcohol`](#itemlistitemalcohol)
  - [`ITEMLIST/ITEM/ScopeMagFactor`](#itemlistitemscopemagfactor)
  - [`ITEMLIST/ITEM/ProjectionFactor`](#itemlistitemprojectionfactor)
  - [`ITEMLIST/ITEM/RecoilModifierX`](#itemlistitemrecoilmodifierx)
  - [`ITEMLIST/ITEM/RecoilModifierY`](#itemlistitemrecoilmodifiery)
  - [`ITEMLIST/ITEM/PercentRecoilModifier`](#itemlistitempercentrecoilmodifier)
  - [`ITEMLIST/ITEM/PercentAccuracyModifier`](#itemlistitempercentaccuracymodifier)
  - [`ITEMLIST/ITEM/usOverheatingCooldownFactor`](#itemlistitemusoverheatingcooldownfactor)
  - [`ITEMLIST/ITEM/overheatTemperatureModificator`](#itemlistitemoverheattemperaturemodificator)
  - [`ITEMLIST/ITEM/overheatCooldownModificator`](#itemlistitemoverheatcooldownmodificator)
  - [`ITEMLIST/ITEM/overheatJamThresholdModificator`](#itemlistitemoverheatjamthresholdmodificator)
  - [`ITEMLIST/ITEM/overheatDamageThresholdModificator`](#itemlistitemoverheatdamagethresholdmodificator)
  - [`ITEMLIST/ITEM/DirtIncreaseFactor`](#itemlistitemdirtincreasefactor)
  - [`ITEMLIST/ITEM/RobotDamageReduction`](#itemlistitemrobotdamagereduction)
  - [`ITEMLIST/ITEM/DiscardedLauncherItem`](#itemlistitemdiscardedlauncheritem)
  - [`ITEMLIST/ITEM/randomitem`](#itemlistitemrandomitem)
  - [`ITEMLIST/ITEM/buddyitem`](#itemlistitembuddyitem)
  - [`ITEMLIST/ITEM/usRiotShieldStrength`](#itemlistitemusriotshieldstrength)
  - [`ITEMLIST/ITEM/usRiotShieldGraphic`](#itemlistitemusriotshieldgraphic)
  - [`ITEMLIST/ITEM/PercentNoiseReduction`](#itemlistitempercentnoisereduction)
  - [`ITEMLIST/ITEM/Bipod`](#itemlistitembipod)
  - [`ITEMLIST/ITEM/ToHitBonus`](#itemlistitemtohitbonus)
  - [`ITEMLIST/ITEM/BestLaserRange`](#itemlistitembestlaserrange)
  - [`ITEMLIST/ITEM/RangeBonus`](#itemlistitemrangebonus)
  - [`ITEMLIST/ITEM/PercentRangeBonus`](#itemlistitempercentrangebonus)
  - [`ITEMLIST/ITEM/AimBonus`](#itemlistitemaimbonus)
  - [`ITEMLIST/ITEM/MinRangeForAimBonus`](#itemlistitemminrangeforaimbonus)
  - [`ITEMLIST/ITEM/PercentAPReduction`](#itemlistitempercentapreduction)
  - [`ITEMLIST/ITEM/PercentStatusDrainReduction`](#itemlistitempercentstatusdrainreduction)
  - [`ITEMLIST/ITEM/BloodiedItem`](#itemlistitembloodieditem)
  - [`ITEMLIST/ITEM/HearingRangeBonus`](#itemlistitemhearingrangebonus)
  - [`ITEMLIST/ITEM/VisionRangeBonus`](#itemlistitemvisionrangebonus)
  - [`ITEMLIST/ITEM/NightVisionRangeBonus`](#itemlistitemnightvisionrangebonus)
  - [`ITEMLIST/ITEM/DayVisionRangeBonus`](#itemlistitemdayvisionrangebonus)
  - [`ITEMLIST/ITEM/CaveVisionRangeBonus`](#itemlistitemcavevisionrangebonus)
  - [`ITEMLIST/ITEM/BrightLightVisionRangeBonus`](#itemlistitembrightlightvisionrangebonus)
  - [`ITEMLIST/ITEM/DamageBonus`](#itemlistitemdamagebonus)
  - [`ITEMLIST/ITEM/MeleeDamageBonus`](#itemlistitemmeleedamagebonus)
  - [`ITEMLIST/ITEM/MagSizeBonus`](#itemlistitemmagsizebonus)
  - [`ITEMLIST/ITEM/PercentBurstFireAPReduction`](#itemlistitempercentburstfireapreduction)
  - [`ITEMLIST/ITEM/PercentAutofireAPReduction`](#itemlistitempercentautofireapreduction)
  - [`ITEMLIST/ITEM/PercentReadyTimeAPReduction`](#itemlistitempercentreadytimeapreduction)
  - [`ITEMLIST/ITEM/PercentReloadTimeAPReduction`](#itemlistitempercentreloadtimeapreduction)
  - [`ITEMLIST/ITEM/AutoFireToHitBonus`](#itemlistitemautofiretohitbonus)
  - [`ITEMLIST/ITEM/APBonus`](#itemlistitemapbonus)
  - [`ITEMLIST/ITEM/RateOfFireBonus`](#itemlistitemrateoffirebonus)
  - [`ITEMLIST/ITEM/BurstSizeBonus`](#itemlistitemburstsizebonus)
  - [`ITEMLIST/ITEM/BurstToHitBonus`](#itemlistitembursttohitbonus)
  - [`ITEMLIST/ITEM/BulletSpeedBonus`](#itemlistitembulletspeedbonus)
  - [`ITEMLIST/ITEM/CamoBonus`](#itemlistitemcamobonus)
  - [`ITEMLIST/ITEM/UrbanCamoBonus`](#itemlistitemurbancamobonus)
  - [`ITEMLIST/ITEM/DesertCamoBonus`](#itemlistitemdesertcamobonus)
  - [`ITEMLIST/ITEM/SnowCamoBonus`](#itemlistitemsnowcamobonus)
  - [`ITEMLIST/ITEM/StealthBonus`](#itemlistitemstealthbonus)
  - [`ITEMLIST/ITEM/usSpotting`](#itemlistitemusspotting)
  - [`ITEMLIST/ITEM/sBackpackWeightModifier`](#itemlistitemsbackpackweightmodifier)
  - [`ITEMLIST/ITEM/sFireResistance`](#itemlistitemsfireresistance)
  - [`ITEMLIST/ITEM/PercentTunnelVision`](#itemlistitempercenttunnelvision)
  - [`ITEMLIST/ITEM/AttachmentSystem`](#itemlistitemattachmentsystem)
  - [`ITEMLIST/ITEM/CrowbarModifier`](#itemlistitemcrowbarmodifier)
  - [`ITEMLIST/ITEM/DisarmModifier`](#itemlistitemdisarmmodifier)
  - [`ITEMLIST/ITEM/RepairModifier`](#itemlistitemrepairmodifier)
  - [`ITEMLIST/ITEM/usHackingModifier`](#itemlistitemushackingmodifier)
  - [`ITEMLIST/ITEM/usBurialModifier`](#itemlistitemusburialmodifier)
  - [`ITEMLIST/ITEM/DamageChance`](#itemlistitemdamagechance)
  - [`ITEMLIST/ITEM/FlashLightRange`](#itemlistitemflashlightrange)
  - [`ITEMLIST/ITEM/ItemChoiceTimeSetting`](#itemlistitemitemchoicetimesetting)
  - [`ITEMLIST/ITEM/SleepModifier`](#itemlistitemsleepmodifier)
  - [`ITEMLIST/ITEM/usPortionSize`](#itemlistitemusportionsize)
  - [`ITEMLIST/ITEM/usAdministrationModifier`](#itemlistitemusadministrationmodifier)
  - [`ITEMLIST/ITEM/LockPickModifier`](#itemlistitemlockpickmodifier)
  - [`ITEMLIST/ITEM/randomitemcoolnessmodificator`](#itemlistitemrandomitemcoolnessmodificator)
  - [`ITEMLIST/ITEM/RobotStrBonus`](#itemlistitemrobotstrbonus)
  - [`ITEMLIST/ITEM/RobotAgiBonus`](#itemlistitemrobotagibonus)
  - [`ITEMLIST/ITEM/RobotDexBonus`](#itemlistitemrobotdexbonus)
  - [`ITEMLIST/ITEM/RobotTargetingSkillGrant`](#itemlistitemrobottargetingskillgrant)
  - [`ITEMLIST/ITEM/RobotUtilitySkillGrant`](#itemlistitemrobotutilityskillgrant)
  - [`ITEMLIST/ITEM/RobotChassisSkillGrant`](#itemlistitemrobotchassisskillgrant)
  - [`ITEMLIST/ITEM/TransportGroupMinProgress`](#itemlistitemtransportgroupminprogress)
  - [`ITEMLIST/ITEM/TransportGroupMaxProgress`](#itemlistitemtransportgroupmaxprogress)
  - [`ITEMLIST/ITEM/DefaultAttachment`](#itemlistitemdefaultattachment)
  - [`ITEMLIST/ITEM/Bloodbag`](#itemlistitembloodbag)
  - [`ITEMLIST/ITEM/Manpad`](#itemlistitemmanpad)
  - [`ITEMLIST/ITEM/Beartrap`](#itemlistitembeartrap)
  - [`ITEMLIST/ITEM/Camera`](#itemlistitemcamera)
  - [`ITEMLIST/ITEM/Waterdrum`](#itemlistitemwaterdrum)
  - [`ITEMLIST/ITEM/BloodcatMeat`](#itemlistitembloodcatmeat)
  - [`ITEMLIST/ITEM/CowMeat`](#itemlistitemcowmeat)
  - [`ITEMLIST/ITEM/Beltfed`](#itemlistitembeltfed)
  - [`ITEMLIST/ITEM/Ammobelt`](#itemlistitemammobelt)
  - [`ITEMLIST/ITEM/AmmobeltVest`](#itemlistitemammobeltvest)
  - [`ITEMLIST/ITEM/CamoRemoval`](#itemlistitemcamoremoval)
  - [`ITEMLIST/ITEM/Cleaningkit`](#itemlistitemcleaningkit)
  - [`ITEMLIST/ITEM/AttentionItem`](#itemlistitemattentionitem)
  - [`ITEMLIST/ITEM/Garotte`](#itemlistitemgarotte)
  - [`ITEMLIST/ITEM/Covert`](#itemlistitemcovert)
  - [`ITEMLIST/ITEM/Corpse`](#itemlistitemcorpse)
  - [`ITEMLIST/ITEM/BloodcatSkin`](#itemlistitembloodcatskin)
  - [`ITEMLIST/ITEM/NoMetalDetection`](#itemlistitemnometaldetection)
  - [`ITEMLIST/ITEM/JumpGrenade`](#itemlistitemjumpgrenade)
  - [`ITEMLIST/ITEM/Handcuffs`](#itemlistitemhandcuffs)
  - [`ITEMLIST/ITEM/Taser`](#itemlistitemtaser)
  - [`ITEMLIST/ITEM/ScubaBottle`](#itemlistitemscubabottle)
  - [`ITEMLIST/ITEM/ScubaMask`](#itemlistitemscubamask)
  - [`ITEMLIST/ITEM/ScubaFins`](#itemlistitemscubafins)
  - [`ITEMLIST/ITEM/TripwireRoll`](#itemlistitemtripwireroll)
  - [`ITEMLIST/ITEM/Radioset`](#itemlistitemradioset)
  - [`ITEMLIST/ITEM/SignalShell`](#itemlistitemsignalshell)
  - [`ITEMLIST/ITEM/Soda`](#itemlistitemsoda)
  - [`ITEMLIST/ITEM/RoofcollapseItem`](#itemlistitemroofcollapseitem)
  - [`ITEMLIST/ITEM/DiseaseprotectionFace`](#itemlistitemdiseaseprotectionface)
  - [`ITEMLIST/ITEM/DiseaseprotectionHand`](#itemlistitemdiseaseprotectionhand)
  - [`ITEMLIST/ITEM/LBEexplosionproof`](#itemlistitemlbeexplosionproof)
  - [`ITEMLIST/ITEM/EmptyBloodbag`](#itemlistitememptybloodbag)
  - [`ITEMLIST/ITEM/MedicalSplint`](#itemlistitemmedicalsplint)
  - [`ITEMLIST/ITEM/Damageable`](#itemlistitemdamageable)
  - [`ITEMLIST/ITEM/Repairable`](#itemlistitemrepairable)
  - [`ITEMLIST/ITEM/WaterDamages`](#itemlistitemwaterdamages)
  - [`ITEMLIST/ITEM/Metal`](#itemlistitemmetal)
  - [`ITEMLIST/ITEM/Sinks`](#itemlistitemsinks)
  - [`ITEMLIST/ITEM/ShowStatus`](#itemlistitemshowstatus)
  - [`ITEMLIST/ITEM/HiddenAddon`](#itemlistitemhiddenaddon)
  - [`ITEMLIST/ITEM/TwoHanded`](#itemlistitemtwohanded)
  - [`ITEMLIST/ITEM/NotBuyable`](#itemlistitemnotbuyable)
  - [`ITEMLIST/ITEM/Attachment`](#itemlistitemattachment)
  - [`ITEMLIST/ITEM/HiddenAttachment`](#itemlistitemhiddenattachment)
  - [`ITEMLIST/ITEM/BigGunList`](#itemlistitembiggunlist)
  - [`ITEMLIST/ITEM/NotInEditor`](#itemlistitemnotineditor)
  - [`ITEMLIST/ITEM/DefaultUndroppable`](#itemlistitemdefaultundroppable)
  - [`ITEMLIST/ITEM/Unaerodynamic`](#itemlistitemunaerodynamic)
  - [`ITEMLIST/ITEM/Electronic`](#itemlistitemelectronic)
  - [`ITEMLIST/ITEM/Cannon`](#itemlistitemcannon)
  - [`ITEMLIST/ITEM/RocketRifle`](#itemlistitemrocketrifle)
  - [`ITEMLIST/ITEM/FingerPrintID`](#itemlistitemfingerprintid)
  - [`ITEMLIST/ITEM/MetalDetector`](#itemlistitemmetaldetector)
  - [`ITEMLIST/ITEM/GasMask`](#itemlistitemgasmask)
  - [`ITEMLIST/ITEM/LockBomb`](#itemlistitemlockbomb)
  - [`ITEMLIST/ITEM/Flare`](#itemlistitemflare)
  - [`ITEMLIST/ITEM/GrenadeLauncher`](#itemlistitemgrenadelauncher)
  - [`ITEMLIST/ITEM/Mortar`](#itemlistitemmortar)
  - [`ITEMLIST/ITEM/Duckbill`](#itemlistitemduckbill)
  - [`ITEMLIST/ITEM/HideMuzzleFlash`](#itemlistitemhidemuzzleflash)
  - [`ITEMLIST/ITEM/RocketLauncher`](#itemlistitemrocketlauncher)
  - [`ITEMLIST/ITEM/SingleShotRocketLauncher`](#itemlistitemsingleshotrocketlauncher)
  - [`ITEMLIST/ITEM/BrassKnuckles`](#itemlistitembrassknuckles)
  - [`ITEMLIST/ITEM/Crowbar`](#itemlistitemcrowbar)
  - [`ITEMLIST/ITEM/GLGrenade`](#itemlistitemglgrenade)
  - [`ITEMLIST/ITEM/FlakJacket`](#itemlistitemflakjacket)
  - [`ITEMLIST/ITEM/LeatherJacket`](#itemlistitemleatherjacket)
  - [`ITEMLIST/ITEM/Batteries`](#itemlistitembatteries)
  - [`ITEMLIST/ITEM/NeedsBatteries`](#itemlistitemneedsbatteries)
  - [`ITEMLIST/ITEM/XRay`](#itemlistitemxray)
  - [`ITEMLIST/ITEM/WireCutters`](#itemlistitemwirecutters)
  - [`ITEMLIST/ITEM/Toolkit`](#itemlistitemtoolkit)
  - [`ITEMLIST/ITEM/FirstAidKit`](#itemlistitemfirstaidkit)
  - [`ITEMLIST/ITEM/MedicalKit`](#itemlistitemmedicalkit)
  - [`ITEMLIST/ITEM/Canteen`](#itemlistitemcanteen)
  - [`ITEMLIST/ITEM/Jar`](#itemlistitemjar)
  - [`ITEMLIST/ITEM/CanAndString`](#itemlistitemcanandstring)
  - [`ITEMLIST/ITEM/Marbles`](#itemlistitemmarbles)
  - [`ITEMLIST/ITEM/Walkman`](#itemlistitemwalkman)
  - [`ITEMLIST/ITEM/RemoteTrigger`](#itemlistitemremotetrigger)
  - [`ITEMLIST/ITEM/RobotRemoteControl`](#itemlistitemrobotremotecontrol)
  - [`ITEMLIST/ITEM/CamouflageKit`](#itemlistitemcamouflagekit)
  - [`ITEMLIST/ITEM/LocksmithKit`](#itemlistitemlocksmithkit)
  - [`ITEMLIST/ITEM/Mine`](#itemlistitemmine)
  - [`ITEMLIST/ITEM/AntitankMine`](#itemlistitemantitankmine)
  - [`ITEMLIST/ITEM/Hardware`](#itemlistitemhardware)
  - [`ITEMLIST/ITEM/Medical`](#itemlistitemmedical)
  - [`ITEMLIST/ITEM/GasCan`](#itemlistitemgascan)
  - [`ITEMLIST/ITEM/ContainsLiquid`](#itemlistitemcontainsliquid)
  - [`ITEMLIST/ITEM/Rock`](#itemlistitemrock)
  - [`ITEMLIST/ITEM/ThermalOptics`](#itemlistitemthermaloptics)
  - [`ITEMLIST/ITEM/SciFi`](#itemlistitemscifi)
  - [`ITEMLIST/ITEM/NewInv`](#itemlistitemnewinv)
  - [`ITEMLIST/ITEM/DiseaseSystemExclusive`](#itemlistitemdiseasesystemexclusive)
  - [`ITEMLIST/ITEM/Barrel`](#itemlistitembarrel)
  - [`ITEMLIST/ITEM/TripWireActivation`](#itemlistitemtripwireactivation)
  - [`ITEMLIST/ITEM/TripWire`](#itemlistitemtripwire)
  - [`ITEMLIST/ITEM/Directional`](#itemlistitemdirectional)
  - [`ITEMLIST/ITEM/BlockIronSight`](#itemlistitemblockironsight)
  - [`ITEMLIST/ITEM/AllowClimbing`](#itemlistitemallowclimbing)
  - [`ITEMLIST/ITEM/Cigarette`](#itemlistitemcigarette)
  - [`ITEMLIST/ITEM/ProvidesRobotCamo`](#itemlistitemprovidesrobotcamo)
  - [`ITEMLIST/ITEM/ProvidesRobotNightVision`](#itemlistitemprovidesrobotnightvision)
  - [`ITEMLIST/ITEM/ProvidesRobotLaserBonus`](#itemlistitemprovidesrobotlaserbonus)
  - [`ITEMLIST/ITEM/STAND_MODIFIERS`](#itemlistitemstand_modifiers)
  - [`ITEMLIST/ITEM/STAND_MODIFIERS/FlatBase`](#itemlistitemstand_modifiersflatbase)
  - [`ITEMLIST/ITEM/STAND_MODIFIERS/PercentBase`](#itemlistitemstand_modifierspercentbase)
  - [`ITEMLIST/ITEM/STAND_MODIFIERS/FlatAim`](#itemlistitemstand_modifiersflataim)
  - [`ITEMLIST/ITEM/STAND_MODIFIERS/PercentAim`](#itemlistitemstand_modifierspercentaim)
  - [`ITEMLIST/ITEM/STAND_MODIFIERS/PercentCap`](#itemlistitemstand_modifierspercentcap)
  - [`ITEMLIST/ITEM/STAND_MODIFIERS/PercentHandling`](#itemlistitemstand_modifierspercenthandling)
  - [`ITEMLIST/ITEM/STAND_MODIFIERS/PercentTargetTrackingSpeed`](#itemlistitemstand_modifierspercenttargettrackingspeed)
  - [`ITEMLIST/ITEM/STAND_MODIFIERS/PercentDropCompensation`](#itemlistitemstand_modifierspercentdropcompensation)
  - [`ITEMLIST/ITEM/STAND_MODIFIERS/PercentMaxCounterForce`](#itemlistitemstand_modifierspercentmaxcounterforce)
  - [`ITEMLIST/ITEM/STAND_MODIFIERS/PercentCounterForceAccuracy`](#itemlistitemstand_modifierspercentcounterforceaccuracy)
  - [`ITEMLIST/ITEM/STAND_MODIFIERS/AimLevels`](#itemlistitemstand_modifiersaimlevels)
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS`](#itemlistitemcrouch_modifiers)
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS/FlatBase`](#itemlistitemcrouch_modifiersflatbase)
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentBase`](#itemlistitemcrouch_modifierspercentbase)
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS/FlatAim`](#itemlistitemcrouch_modifiersflataim)
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentAim`](#itemlistitemcrouch_modifierspercentaim)
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentCap`](#itemlistitemcrouch_modifierspercentcap)
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentHandling`](#itemlistitemcrouch_modifierspercenthandling)
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentTargetTrackingSpeed`](#itemlistitemcrouch_modifierspercenttargettrackingspeed)
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentDropCompensation`](#itemlistitemcrouch_modifierspercentdropcompensation)
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentMaxCounterForce`](#itemlistitemcrouch_modifierspercentmaxcounterforce)
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentCounterForceAccuracy`](#itemlistitemcrouch_modifierspercentcounterforceaccuracy)
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS/AimLevels`](#itemlistitemcrouch_modifiersaimlevels)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS`](#itemlistitemprone_modifiers)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS/FlatBase`](#itemlistitemprone_modifiersflatbase)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS/PercentBase`](#itemlistitemprone_modifierspercentbase)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS/FlatAim`](#itemlistitemprone_modifiersflataim)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS/PercentAim`](#itemlistitemprone_modifierspercentaim)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS/PercentCap`](#itemlistitemprone_modifierspercentcap)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS/PercentHandling`](#itemlistitemprone_modifierspercenthandling)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS/PercentTargetTrackingSpeed`](#itemlistitemprone_modifierspercenttargettrackingspeed)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS/PercentDropCompensation`](#itemlistitemprone_modifierspercentdropcompensation)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS/PercentMaxCounterForce`](#itemlistitemprone_modifierspercentmaxcounterforce)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS/PercentCounterForceAccuracy`](#itemlistitemprone_modifierspercentcounterforceaccuracy)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS/AimLevels`](#itemlistitemprone_modifiersaimlevels)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Items.xml` — главный список всех предметов в игре. Каждая запись `ITEM`
определяет один ID предмета, его название и описание, его класс, данные
инвентаря, боевые модификаторы, флаги и ссылки на таблицы конкретных классов.
Другие таблицы используют `uiIndex` предмета как внешний ключ, поэтому эта
таблица — центр большинства связей предметов.

Таблица плоская: одна запись `ITEM` на предмет, ключ — `uiIndex`.

## Tables

| Файл | Корень / запись | Примечания |
|---|---|---|
| `Items/Items.xml` | `ITEMLIST` / `ITEM` | Базовая таблица предметов. |

Локализованные оверлеи (`Russian.Items.xml` и другие языковые файлы) не
являются отдельными таблицами. Они переопределяют пять текстовых полей по
`uiIndex`; см. [Localization](../../Localization.md).

## Structure

```
ITEMLIST
└── ITEM
    ├── uiIndex
    ├── szItemName
    ├── szLongItemName
    ├── szItemDesc
    ├── szBRName
    ├── szBRDesc
    ├── usItemClass
    ├── AttachmentClass
    ├── nasAttachmentClass
    ├── nasLayoutClass
    ├── AvailableAttachmentPoint
    ├── AttachmentPoint
    ├── AttachToPointAPCost
    ├── ubClassIndex
    ├── ubCursor
    ├── bSoundType
    ├── ubGraphicType
    ├── ubGraphicNum
    ├── ubWeight
    ├── ubPerPocket
    ├── ItemSize
    ├── ItemSizeBonus
    ├── usPrice
    ├── ubCoolness
    ├── bReliability
    ├── bRepairEase
    ├── Inseparable
    ├── BR_NewInventory
    ├── BR_UsedInventory
    ├── BR_ROF
    ├── DrugType
    ├── FoodType
    ├── usActionItemFlag
    ├── clothestype
    ├── spreadPattern
    ├── Alcohol
    ├── ScopeMagFactor
    ├── ProjectionFactor
    ├── RecoilModifierX
    ├── RecoilModifierY
    ├── PercentRecoilModifier
    ├── PercentAccuracyModifier
    ├── usOverheatingCooldownFactor
    ├── overheatTemperatureModificator
    ├── overheatCooldownModificator
    ├── overheatJamThresholdModificator
    ├── overheatDamageThresholdModificator
    ├── DirtIncreaseFactor
    ├── RobotDamageReduction
    ├── DiscardedLauncherItem
    ├── randomitem
    ├── buddyitem
    ├── usRiotShieldStrength
    ├── usRiotShieldGraphic
    ├── PercentNoiseReduction
    ├── Bipod
    ├── ToHitBonus
    ├── BestLaserRange
    ├── RangeBonus
    ├── PercentRangeBonus
    ├── AimBonus
    ├── MinRangeForAimBonus
    ├── PercentAPReduction
    ├── PercentStatusDrainReduction
    ├── BloodiedItem
    ├── HearingRangeBonus
    ├── VisionRangeBonus
    ├── NightVisionRangeBonus
    ├── DayVisionRangeBonus
    ├── CaveVisionRangeBonus
    ├── BrightLightVisionRangeBonus
    ├── DamageBonus
    ├── MeleeDamageBonus
    ├── MagSizeBonus
    ├── PercentBurstFireAPReduction
    ├── PercentAutofireAPReduction
    ├── PercentReadyTimeAPReduction
    ├── PercentReloadTimeAPReduction
    ├── AutoFireToHitBonus
    ├── APBonus
    ├── RateOfFireBonus
    ├── BurstSizeBonus
    ├── BurstToHitBonus
    ├── BulletSpeedBonus
    ├── CamoBonus
    ├── UrbanCamoBonus
    ├── DesertCamoBonus
    ├── SnowCamoBonus
    ├── StealthBonus
    ├── usSpotting
    ├── sBackpackWeightModifier
    ├── sFireResistance
    ├── PercentTunnelVision
    ├── AttachmentSystem
    ├── CrowbarModifier
    ├── DisarmModifier
    ├── RepairModifier
    ├── usHackingModifier
    ├── usBurialModifier
    ├── DamageChance
    ├── FlashLightRange
    ├── ItemChoiceTimeSetting
    ├── SleepModifier
    ├── usPortionSize
    ├── usAdministrationModifier
    ├── LockPickModifier
    ├── randomitemcoolnessmodificator
    ├── RobotStrBonus
    ├── RobotAgiBonus
    ├── RobotDexBonus
    ├── RobotTargetingSkillGrant
    ├── RobotUtilitySkillGrant
    ├── RobotChassisSkillGrant
    ├── TransportGroupMinProgress
    ├── TransportGroupMaxProgress
    ├── DefaultAttachment
    ├── Bloodbag
    ├── Manpad
    ├── Beartrap
    ├── Camera
    ├── Waterdrum
    ├── BloodcatMeat
    ├── CowMeat
    ├── Beltfed
    ├── Ammobelt
    ├── AmmobeltVest
    ├── CamoRemoval
    ├── Cleaningkit
    ├── AttentionItem
    ├── Garotte
    ├── Covert
    ├── Corpse
    ├── BloodcatSkin
    ├── NoMetalDetection
    ├── JumpGrenade
    ├── Handcuffs
    ├── Taser
    ├── ScubaBottle
    ├── ScubaMask
    ├── ScubaFins
    ├── TripwireRoll
    ├── Radioset
    ├── SignalShell
    ├── Soda
    ├── RoofcollapseItem
    ├── DiseaseprotectionFace
    ├── DiseaseprotectionHand
    ├── LBEexplosionproof
    ├── EmptyBloodbag
    ├── MedicalSplint
    ├── Damageable
    ├── Repairable
    ├── WaterDamages
    ├── Metal
    ├── Sinks
    ├── ShowStatus
    ├── HiddenAddon
    ├── TwoHanded
    ├── NotBuyable
    ├── Attachment
    ├── HiddenAttachment
    ├── BigGunList
    ├── NotInEditor
    ├── DefaultUndroppable
    ├── Unaerodynamic
    ├── Electronic
    ├── Cannon
    ├── RocketRifle
    ├── FingerPrintID
    ├── MetalDetector
    ├── GasMask
    ├── LockBomb
    ├── Flare
    ├── GrenadeLauncher
    ├── Mortar
    ├── Duckbill
    ├── HideMuzzleFlash
    ├── RocketLauncher
    ├── SingleShotRocketLauncher
    ├── BrassKnuckles
    ├── Crowbar
    ├── GLGrenade
    ├── FlakJacket
    ├── LeatherJacket
    ├── Batteries
    ├── NeedsBatteries
    ├── XRay
    ├── WireCutters
    ├── Toolkit
    ├── FirstAidKit
    ├── MedicalKit
    ├── Canteen
    ├── Jar
    ├── CanAndString
    ├── Marbles
    ├── Walkman
    ├── RemoteTrigger
    ├── RobotRemoteControl
    ├── CamouflageKit
    ├── LocksmithKit
    ├── Mine
    ├── AntitankMine
    ├── Hardware
    ├── Medical
    ├── GasCan
    ├── ContainsLiquid
    ├── Rock
    ├── ThermalOptics
    ├── SciFi
    ├── NewInv
    ├── DiseaseSystemExclusive
    ├── Barrel
    ├── TripWireActivation
    ├── TripWire
    ├── Directional
    ├── BlockIronSight
    ├── AllowClimbing
    ├── Cigarette
    ├── ProvidesRobotCamo
    ├── ProvidesRobotNightVision
    ├── ProvidesRobotLaserBonus
    ├── STAND_MODIFIERS
    │   ├── FlatBase
    │   ├── PercentBase
    │   ├── FlatAim
    │   ├── PercentAim
    │   ├── PercentCap
    │   ├── PercentHandling
    │   ├── PercentTargetTrackingSpeed
    │   ├── PercentDropCompensation
    │   ├── PercentMaxCounterForce
    │   ├── PercentCounterForceAccuracy
    │   └── AimLevels
    ├── CROUCH_MODIFIERS
    │   ├── FlatBase
    │   ├── PercentBase
    │   ├── FlatAim
    │   ├── PercentAim
    │   ├── PercentCap
    │   ├── PercentHandling
    │   ├── PercentTargetTrackingSpeed
    │   ├── PercentDropCompensation
    │   ├── PercentMaxCounterForce
    │   ├── PercentCounterForceAccuracy
    │   └── AimLevels
    └── PRONE_MODIFIERS
        ├── FlatBase
        ├── PercentBase
        ├── FlatAim
        ├── PercentAim
        ├── PercentCap
        ├── PercentHandling
        ├── PercentTargetTrackingSpeed
        ├── PercentDropCompensation
        ├── PercentMaxCounterForce
        ├── PercentCounterForceAccuracy
        └── AimLevels
```

- Корень: `ITEMLIST`; запись: `ITEM`; ключ: `uiIndex`.
- Плоские поля плюс три вложенные группы стоек: `STAND_MODIFIERS`,
  `CROUCH_MODIFIERS` и `PRONE_MODIFIERS`. В каждой группе одиннадцать
  модификаторов `INT16`. Отсутствующее значение для приседа наследует значение
  для стояния, а отсутствующее значение для лежачего положения наследует
  значение для приседа; отсутствующее значение для стояния остаётся `0`.
- `DefaultAttachment` — повторяющийся элемент. Запись может содержать до двадцати
  вхождений (`MAX_DEFAULT_ATTACHMENTS`); каждое вхождение — один ID предмета.
  Значения заполняют первый свободный слот, поэтому запись `0` пропускается, а
  последующие значения всё равно загружаются; вхождения сверх двадцати
  игнорируются. Документация описывает этот путь один раз и отмечает
  повторяемость.
- На практике записи разреженные: отсутствующие поля принимают игровое значение
  по умолчанию, а пустые элементы для текстовых полей — не то же самое, что
  числовой ноль.
- Числовые поля сохраняют свою лексическую форму. Десятичная запись вроде `1.0`
  не переписывается редактором в `1`.

### Item classes and `ubClassIndex`

`usItemClass` — битовая маска. Она выбирает подтаблицу конкретного класса,
используемую через `ubClassIndex`:

- класс брони -> [Armours](Armours.md);
- класс магазина/боеприпаса -> [Magazines](Magazines.md);
- пары пусковая установка/боеприпас -> [Launchables](Launchables.md);
- строки оружия, взрывчатки и других классов описаны в документах своих
  семейств.

Не читайте `ubClassIndex` без `usItemClass`: одно и то же число означает
разные строки в разных таблицах классов.

## Game options

Эта таблица используется несколькими необязательными системами. Перечисленные
ниже опции — те, что меняют чтение поля; поле без примечания используется обоими
вариантами соответствующей системы. Общая карта опций —
[OPTIONS.md](../../OPTIONS.md).

| Опция | Значения | Влияние на эту таблицу |
|---|---|---|
| Новый шанс попадания (NCTH) / старый шанс попадания (OCTH) | `NCTH = TRUE` или `FALSE` в файле игровых опций. `TRUE` выбирает NCTH; `FALSE` выбирает OCTH. | NCTH читает `ScopeMagFactor`, `ProjectionFactor`, `RecoilModifierX`, `RecoilModifierY`, `PercentRecoilModifier`, `PercentAccuracyModifier` и три группы стоек. OCTH игнорирует эти поля. |
| Новый инвентарь (NIV) / старый инвентарь (OIV) | Выбирается при старте новой кампании; выбор сохраняется в сейве. Набор данных 1.13 — это NIV, а устаревшие данные OIV по-прежнему поддерживаются. `OLD_INVENTORY_ITEM_NUMBER` (по умолчанию `99`) помечает размер предмета, существующий только в OIV. | `ubPerPocket` управляет ёмкостью кармана только в OIV; в NIV ёмкостью управляют таблицы `Pockets` и `LoadBearingEquipment` и `ItemSize`. `NewInv` помечает предмет как существующий только в NIV. В NIV предмет с `ItemSize`, равным `OLD_INVENTORY_ITEM_NUMBER`, исключается. |
| Режим Sci-Fi | `SciFi = TRUE` или `FALSE`. | `SciFi` помечает предмет как часть набора предметов Sci-Fi. |
| Tons of Guns | `BigGunList = TRUE` или `FALSE`. | `BigGunList` включает предмет в расширенный список оружия. |
| Система болезней | система болезней включена или отключена. | `DiseaseSystemExclusive` помечает предмет, относящийся к системе болезней. |

## Field map

| Группа | Поля | Охватывает |
|---|---:|---|
| Идентификация и описания | 6 | ID, названия и текст для игрока. |
| Классификация и точки вложения | 10 | Класс предмета, системы вложений и диспетчеризация классов. |
| Инвентарь, графика и экономика | 14 | Графика, вес, размер, цена и запас Bobby Ray. |
| Еда, наркотики и предметы действия | 6 | Классы расходуемых предметов и флаги особого применения. |
| Модификаторы оружия и перегрев | 12 | Оптика, отдача, точность, нагрев и загрязнение. |
| Робот, случайная добыча и связи щитов | 7 | Урон роботу, связи случайных предметов и данные полицейского щита. |
| Боевые модификаторы | 29 | Модификаторы попадания, дальности, урона, ОД, очереди и автоогня. |
| Маскировка, обзор и модификаторы тела | 9 | Маскировка, скрытность, наблюдательность, обзор и модификаторы рюкзака/огня. |
| Система вложений, утилиты и транспорт | 23 | Режим вложений, наборы, навыки, прогресс транспорта и вложения по умолчанию. |
| Флаги предметов | 105 | Булевы роли предметов, используемые в игровых системах. |
| Группы модификаторов стоек | 3 | Структурные контейнеры `STAND_MODIFIERS`, `CROUCH_MODIFIERS` и `PRONE_MODIFIERS`. |
| Поля модификаторов стоек | 33 | Модификаторы прицеливания для стояния, приседа и лежачего положения. |

## Fields

`Type` называет тип поля в редакторе; ширина в скобках — это тип хранения в C
в структуре предмета. `Values` даёт диапазон этого типа хранения и любое
ограничение, которое применяет загрузчик. Загрузчик приводит тип без проверки,
поэтому целочисленный текст вне диапазона усекается приведением, а не
отвергается, а отсутствующий элемент оставляет значение, инициализированное
нулём (только у `usPortionSize` есть ненулевое значение по умолчанию в коде).
Достоверность `source` означает, что тип, ширина хранения, вызов разбора и
любое правило ограничения или наследования были сверены с игровым загрузчиком
предметов и структурой предмета. Описания ролей следуют именам членов этой
структуры; там, где единица измерения, перечисление или эффект не установлены
кодом, это перечислено в разделе Open questions. Достоверность `inferred`
означает, что такая проверка была невозможна; в этой редакции ни одно поле не
остаётся на этом уровне.

**Идентификация и описания.** ID, названия и текст для игрока.

### `ITEMLIST/ITEM/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Первичный ID предмета. Ссылки между таблицами и оверлеи локализации используют это значение; внутри одного файла он уникален. Запись, у которой `usItemClass` равен `0`, пропускается загрузчиком (при проходе локализованного оверлея копируется только её текст).
- **Values:** `0..4294967295`; хранится как `UINT32`. Массив содержит `MAXITEMS` (`16001`) записей; больший индекс игнорируется.
- **Default:** Нет; обязателен и уникален внутри файла.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szItemName`

- **Type:** `String`
- **Description:** Короткое название предмета, используемое в списках и компактных ячейках.
- **Values:** До 79 символов плюс терминатор (`CHAR16[80]`); более длинный текст усекается загрузчиком.
- **Default:** Пусто, если элемент отсутствует.
- **Source:** game code (item loader); item table data; item localization overlay.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szLongItemName`

- **Type:** `String`
- **Description:** Полное название предмета, используемое на панелях подробностей и во всплывающих подсказках.
- **Values:** До 79 символов плюс терминатор (`CHAR16[80]`); более длинный текст усекается загрузчиком.
- **Default:** Пусто, если элемент отсутствует.
- **Source:** game code (item loader); item table data; item localization overlay.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szItemDesc`

- **Type:** `String`
- **Description:** Описание предмета, показываемое на панели подробностей.
- **Values:** До 399 символов плюс терминатор (`CHAR16[400]`); более длинный текст усекается загрузчиком.
- **Default:** Пусто, если элемент отсутствует.
- **Source:** game code (item loader); item table data; item localization overlay.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szBRName`

- **Type:** `String`
- **Description:** Название предмета в каталоге Bobby Ray.
- **Values:** До 79 символов плюс терминатор (`CHAR16[80]`); более длинный текст усекается загрузчиком.
- **Default:** Пусто, если элемент отсутствует.
- **Source:** game code (item loader); item table data; item localization overlay.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szBRDesc`

- **Type:** `String`
- **Description:** Описание предмета в каталоге Bobby Ray.
- **Values:** До 399 символов плюс терминатор (`CHAR16[400]`); более длинный текст усекается загрузчиком.
- **Default:** Пусто, если элемент отсутствует.
- **Source:** game code (item loader); item table data; item localization overlay.
- **Confidence:** `source`

**Классификация и точки вложения.** Класс предмета, системы вложений и диспетчеризация классов.

### `ITEMLIST/ITEM/usItemClass`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Битовая маска флагов класса предмета (`IC_*`). Она выбирает подтаблицу конкретного класса, доступную через `ubClassIndex`, и определяет, какие поведения предмета применяются. Запись с `0` пропускается загрузчиком. См. [Lookup.ItemClass](../Lookup.md#itemclass).
- **Numeric semantics:** Битовая маска или перечислимый код: смысл несут отдельные биты/варианты, поэтому большее число не означает просто «больше». Отрицательные значения не имеют смысла.
- **Values:** `0` = нет; `0..4294967295`; разбирается через `strtoul` с основанием `0` (шестнадцатеричная запись `0x...` допускается); хранится как `UINT32`.
- **Default:** Нет; каждый предмет объявляет класс.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AttachmentClass`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Битовая маска устаревших классов вложений (`AC_*`), используемых старой системой вложений. Биты определены справочником [AttachmentClass](../Lookup.md#attachmentclass).
- **Numeric semantics:** Битовая маска или перечислимый код: смысл несут отдельные биты/варианты, поэтому большее число не означает просто «больше». Отрицательные значения не имеют смысла.
- **Values:** `0` = нет; `0..4294967295`; разбирается через `strtoul` с основанием `0` (шестнадцатеричная запись `0x...` допускается); хранится как `UINT32`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/nasAttachmentClass`

- **Type:** `Flags` (bit mask, `UINT64`)
- **Description:** Пользовательская битовая маска классов вложений NAS, используемая вместе с [AttachmentSystem](../Lookup.md#attachmentsystem) и точками вложений NAS.
- **Numeric semantics:** Битовая маска или перечислимый код: смысл несут отдельные биты/варианты, поэтому большее число не означает просто «больше». Отрицательные значения не имеют смысла.
- **Values:** `0` = нет; разбирается через `atof` и усекается до `UINT64`, поэтому читается только десятичная запись; хранится как `UINT64`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/nasLayoutClass`

- **Type:** `Flags` (bit mask, `UINT64`)
- **Description:** Битовая маска классов компоновки NAS. Она сообщает компоновке NAS, какие слоты вложений предоставляет предмет; биты определены справочником [NasLayoutClass](../Lookup.md#naslayoutclass).
- **Numeric semantics:** Битовая маска или перечислимый код: смысл несут отдельные биты/варианты, поэтому большее число не означает просто «больше». Отрицательные значения не имеют смысла.
- **Values:** `0` = нет; разбирается через `atof` и усекается до `UINT64`, поэтому читается только десятичная запись; хранится как `UINT64`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AvailableAttachmentPoint`

- **Type:** `Flags` (bit mask, `UINT64`)
- **Description:** Битовая маска точек вложения, которые предмет может принимать. Повторяющиеся элементы объединяются побитовым ИЛИ, поэтому для одного предмета можно перечислить несколько масок; биты определены справочником [AttachmentPoint](../Lookup.md#attachmentpoint).
- **Numeric semantics:** Битовая маска или перечислимый код: смысл несут отдельные биты/варианты, поэтому большее число не означает просто «больше». Отрицательные значения не имеют смысла.
- **Values:** `0` = нет; разбирается через `atof` и используется как маска `UINT64`; повторяющиеся элементы объединяются побитовым ИЛИ.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AttachmentPoint`

- **Type:** `Flags` (bit mask, `UINT64`)
- **Description:** Точка вложения, которую занимает этот предмет при установке. Ноль означает, что предмет не установлен в точку; повторяющийся элемент заменяет предыдущее значение.
- **Numeric semantics:** Битовая маска или перечислимый код: смысл несут отдельные биты/варианты, поэтому большее число не означает просто «больше». Отрицательные значения не имеют смысла.
- **Values:** `0` = нет; разбирается через `atof` и используется как маска `UINT64`; повторяющийся элемент заменяет предыдущее значение.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AttachToPointAPCost`

- **Type:** `Integer` (`UINT8`)
- **Description:** Стоимость в ОД на присоединение или отсоединение предмета в его точке вложения.
- **Numeric semantics:** Стоимость в очках действия: большие значения делают присоединение и отсоединение дороже в ОД, отрицательные значения не имеют смысла.
- **Values:** `0..255`; хранится как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubClassIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс записи конкретного класса, выбранной по `usItemClass`: строка брони, строка магазина, строка оружия, строка взрывчатки и так далее.
- **Values:** `0..65535`; хранится как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubCursor`

- **Type:** `Lookup` (integer ID, `UINT8`)
- **Description:** ID справочника курсоров, показываемый при выборе предмета в инвентаре. См. [Cursor](../Lookup.md#cursor).
- **Values:** ID справочника (`0..255`, хранится как `UINT8`); см. [Cursor](../Lookup.md#cursor).
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/bSoundType`

- **Type:** `Integer` (`INT8`)
- **Description:** Семейство звуков, используемое при обращении с предметом или при выстреле из него.
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Инвентарь, графика и экономика.** Графика, вес, размер, цена и запас Bobby Ray.

### `ITEMLIST/ITEM/ubGraphicType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Библиотека графики для изображения предмета в инвентаре: `0` графика оружия, `1` графика больших предметов (`P1ITEMS`), `2` графика интерфейса (`P2ITEMS`), `3` альтернативная графика интерфейса (`P3ITEMS`), `4` и выше — дальнейшие библиотеки тайлов предметов (начиная с `P4ITEMS`).
- **Values:** `0..255`; хранится как `UINT8`. `0` выбирает библиотеки графики оружия, `1`..`3` выбирают `P1ITEMS`..`P3ITEMS`, а `4` и выше выбирают `P4ITEMS` и более поздние библиотеки тайлов предметов.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubGraphicNum`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс графики внутри библиотеки, выбранной `ubGraphicType`.
- **Values:** `0..65535`; хранится как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubWeight`

- **Type:** `Integer` (`UINT16`)
- **Description:** Вес предмета в игровых единицах веса; используется грузоподъёмностью и ёмкостью рюкзака. Две единицы — примерно один килограмм (грубо одна единица на фунт).
- **Numeric semantics:** Большие значения означают более тяжёлый предмет; отрицательные значения не имеют смысла.
- **Values:** `0..65535`; хранится как `UINT16`; значение — целое число единиц веса.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubPerPocket`

- **Type:** `Integer` (`UINT8`)
- **Description:** Ёмкость кармана в старом инвентаре (OIV): сколько одинаковых предметов помещается в один карман. Ноль означает отсутствие укладки в стопку. В NIV это поле не управляет ёмкостью кармана (это делают LBE/Pockets и `ItemSize`), но код описания предмета всё равно читает его для свойства «маленький»: в NIV иначе каждый предмет сообщается как помещающийся в маленький карман.
- **Numeric semantics:** Большие значения означают большую ёмкость кармана, отрицательные значения не имеют смысла.
- **Values:** `0..255`; хранится как `UINT8`. Ёмкость слота OIV, получаемая из него, ограничивается значением `MAX_OBJECTS_PER_SLOT` (`255`).
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ItemSize`

- **Type:** `Integer` (`UINT16`)
- **Description:** Класс размера в инвентаре. Он определяет укладку по карманам и ограничения размера предметов. В NIV предмет, размер которого равен `OLD_INVENTORY_ITEM_NUMBER` (настройка INI, по умолчанию `99`), исключается.
- **Values:** `0..65535`; хранится как `UINT16`. `99` — это только поставляемое по умолчанию значение `OLD_INVENTORY_ITEM_NUMBER`, а не ограничение кода.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ItemSizeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Поправка, добавляемая к действующему размеру предмета при проверке ёмкости кармана (`iSize += ItemSizeBonus`).
- **Numeric semantics:** Знаковая поправка размера: отрицательные значения уменьшают действующий размер.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usPrice`

- **Type:** `Integer` (`UINT16`)
- **Description:** Базовая цена покупки. От неё происходят цены Bobby Ray и некоторые расчёты торговли.
- **Numeric semantics:** Сумма денег или стоимость: большие значения означают большую сумму, отрицательные значения не имеют смысла.
- **Values:** `0..65535`; хранится как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubCoolness`

- **Type:** `Integer` (`UINT8`)
- **Description:** Уровень крутости/прогресса, на котором предмет становится доступным в кампании. Код снаряжения случайных наёмников пропускает предметы с `ubCoolness < 1`.
- **Numeric semantics:** Большие значения означают более поздний и более редкий уровень крутости; отрицательные значения вне обычного диапазона.
- **Values:** `0..255`; хранится как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/bReliability`

- **Type:** `Integer` (`INT8`)
- **Description:** Модификатор надёжности предмета.
- **Numeric semantics:** Большие значения означают лучшую надёжность, меньшие — худшую; отрицательные значения вне обычного диапазона.
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/bRepairEase`

- **Type:** `Integer` (`INT8`)
- **Description:** Модификатор сложности ремонта предмета.
- **Numeric semantics:** Большие значения облегчают ремонт, меньшие — усложняют; отрицательные значения вне обычного диапазона.
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Inseparable`

- **Type:** `Lookup` (integer ID, `UINT8`)
- **Description:** Режим отделимости вложения: `0` отделяется, `1` неотделимо, `2` неотделимо, но заменяемо.
- **Values:** `0`, `1` или `2`, хранится как `UINT8`. См. [Separability](../Lookup.md#separability).
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BR_NewInventory`

- **Type:** `Integer` (`UINT8`)
- **Description:** Уровень или количество запаса нового инвентаря Bobby Ray для этого предмета.
- **Numeric semantics:** Большие значения означают больший уровень запаса; отрицательные значения не имеют смысла.
- **Values:** `0..255`; хранится в массиве `UINT8` вне структуры предмета.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BR_UsedInventory`

- **Type:** `Integer` (`UINT8`)
- **Description:** Уровень или количество запаса подержанного инвентаря Bobby Ray.
- **Numeric semantics:** Большие значения означают больший уровень запаса; отрицательные значения не имеют смысла.
- **Values:** `0..255`; хранится в массиве `UINT8` вне структуры предмета.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BR_ROF`

- **Type:** `Integer` (`INT16`)
- **Description:** Значение скорострельности/пополнения запаса Bobby Ray для этого предмета.
- **Numeric semantics:** Большие значения означают большее значение скорострельности/пополнения; отрицательные значения не имеют смысла.
- **Values:** `-32768..32767`; хранится в массиве `INT16` вне структуры предмета.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Еда, наркотики и предметы действия.** Классы расходуемых предметов и флаги особого применения.

### `ITEMLIST/ITEM/DrugType`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Битовая маска флагов наркотика/эффекта. Структура предмета определяет её как маску флагов компонентов, используемых в наркотике; именованные биты — в [DrugType](../Lookup.md#drugtype).
- **Values:** `0` = нет; `0..4294967295`; разбирается через `strtoul` с основанием `0` (шестнадцатеричная запись `0x...` допускается); хранится как `UINT32`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/FoodType`

- **Type:** `Integer` (`UINT32`)
- **Description:** Класс еды или напитка, используемый системой питания; строки определены в [Food](Food.md).
- **Values:** `0..4294967295`; разбирается через `strtoul` с основанием `0` (шестнадцатеричная запись `0x...` допускается); хранится как `UINT32`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usActionItemFlag`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Битовая маска флагов предметов действия. Она управляет особыми действиями использования, такими как NET, радио или дистанционный детонатор.
- **Values:** `0` = нет; `0..4294967295`; разбирается через `strtoul` с основанием `0` (шестнадцатеричная запись `0x...` допускается); хранится как `UINT32`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/clothestype`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс типа одежды, используемый системой одежды; строки определены в [Clothes](Clothes.md).
- **Values:** `0..4294967295`; разбирается через `strtoul` с основанием `0` (шестнадцатеричная запись `0x...` допускается); хранится как `UINT32`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/spreadPattern`

- **Type:** `String`
- **Description:** Именованный шаблон рассеивания дроби дробовика из [SpreadPatterns](../SpreadPatterns.md). Загрузчик принимает либо имя шаблона, либо числовой индекс; неизвестное имя или индекс вне диапазона приводятся к `0` — шаблону по умолчанию.
- **Numeric semantics:** Хранимое значение — индекс шаблона в SpreadPatterns.xml; `0` — шаблон по умолчанию.
- **Values:** Имя шаблона из SpreadPatterns.xml или числовой индекс в этой таблице; пустое значение, неизвестное имя или индекс вне диапазона приводятся к `0` (по умолчанию).
- **Default:** Пусто; загрузчик приводит его к шаблону по умолчанию `0`.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Alcohol`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Значение крепости/эффекта алкоголя для напитков; питает состояние опьянения и связанные штрафы.
- **Numeric semantics:** Большие значения означают более сильный эффект алкоголя; загрузчик принудительно приводит отрицательные значения к `0`.
- **Values:** Десятичное значение; загрузчик принудительно приводит отрицательные значения к `0`; хранится как `FLOAT`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Модификаторы оружия и перегрев.** Оптика, отдача, точность, нагрев и загрязнение.

### `ITEMLIST/ITEM/ScopeMagFactor`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Коэффициент увеличения прицела в NCTH.
- **Only used with:** NCTH.
- **Numeric semantics:** Большие значения означают более сильное увеличение; `0` означает отсутствие увеличения, отрицательные значения не имеют смысла.
- **Values:** Десятичное значение; хранится как `FLOAT`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ProjectionFactor`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Коэффициент лазерной проекции в NCTH.
- **Only used with:** NCTH.
- **Numeric semantics:** Большие значения означают более сильную проекцию; `0` означает отсутствие проекции, отрицательные значения не имеют смысла.
- **Values:** Десятичное значение; хранится как `FLOAT`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RecoilModifierX`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Модификатор горизонтальной отдачи, добавляемый к базовой отдаче оружия.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый горизонтальный модификатор: положительные значения смещают отдачу вправо, отрицательные — влево; большая величина хуже контролируется.
- **Values:** Десятичное значение; хранится как `FLOAT`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RecoilModifierY`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Модификатор вертикальной отдачи, добавляемый к базовой отдаче оружия.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый вертикальный модификатор: положительные значения смещают отдачу вверх, отрицательные — вниз; большая величина хуже контролируется.
- **Values:** Десятичное значение; хранится как `FLOAT`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentRecoilModifier`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентный модификатор отдачи в NCTH.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый процент: отдача умножается на `(100 + value)/100`, поэтому отрицательные значения уменьшают отдачу, а положительные увеличивают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentAccuracyModifier`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентный модификатор точности в NCTH.
- **Only used with:** NCTH.
- **Numeric semantics:** Процент: большие значения дают более сильный эффект, отрицательные снижают его ниже базового уровня.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usOverheatingCooldownFactor`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Единицы температуры, снимаемые за такт охлаждения (примерно каждый ход или пять секунд), до модификаторов вложений и глобальных модификаторов.
- **Numeric semantics:** Большие значения охлаждают оружие быстрее; `0` означает отсутствие охлаждения, отрицательные значения не имеют смысла.
- **Values:** Десятичное значение; хранится как `FLOAT`.
- **Default:** `0`, если элемент отсутствует; загрузчик начинает с нуля. `100` — это значение данных, а не значение по умолчанию в коде.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/overheatTemperatureModificator`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Процентный модификатор нагрева, который создаёт один выстрел (обычно читается из вложений).
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный.
- **Values:** Десятичное значение; хранится как `FLOAT`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/overheatCooldownModificator`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Процентный модификатор величины охлаждения; применяется к оружию и стволам.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный.
- **Values:** Десятичное значение; хранится как `FLOAT`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/overheatJamThresholdModificator`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Процентный модификатор порога заклинивания оружия.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный.
- **Values:** Десятичное значение; хранится как `FLOAT`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/overheatDamageThresholdModificator`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Процентный модификатор порога повреждения оружия.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный.
- **Values:** Десятичное значение; хранится как `FLOAT`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DirtIncreaseFactor`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Загрязнение, которое один выстрел добавляет оружию (расширенная система ремонта/загрязнения).
- **Numeric semantics:** Большие значения накапливают загрязнение быстрее; `0` означает отсутствие прироста загрязнения, отрицательные значения не имеют смысла.
- **Values:** Десятичное значение; хранится как `FLOAT`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Робот, случайная добыча и связи щитов.** Урон роботу, связи случайных предметов и данные полицейского щита.

### `ITEMLIST/ITEM/RobotDamageReduction`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Модификатор снижения урона роботу, читаемый из вложений робота.
- **Numeric semantics:** Десятичный модификатор; загрузчик не применяет ограничений, а шкала, используемая кодом робота, здесь не восстановлена.
- **Values:** Десятичное значение; хранится как `FLOAT`.
- **Default:** `0`, если элемент отсутствует; загрузчик инициализирует запись нулями, поэтому `1.0` — это соглашение данных, а не значение по умолчанию в коде.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DiscardedLauncherItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, остающегося после выстрела из одноразовой пусковой установки.
- **Numeric semantics:** ID предмета; `0` означает, что ничего не остаётся.
- **Values:** `0..65535`; хранится как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/randomitem`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID таблицы случайных предметов, используемый, когда этот предмет создаётся как случайная добыча.
- **Numeric semantics:** ID таблицы случайных предметов; `0` означает отсутствие.
- **Values:** `0..65535`; хранится как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/buddyitem`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета-компаньона (buddy), связанного с этим предметом.
- **Numeric semantics:** ID предмета; `0` означает отсутствие связанного предмета.
- **Values:** `0..65535`; хранится как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usRiotShieldStrength`

- **Type:** `Integer` (`UINT16`)
- **Description:** Прочность защиты полицейского щита.
- **Numeric semantics:** Большие значения означают более прочный щит; загрузчик ограничивает значение на `100`.
- **Values:** `0..100`; загрузчик ограничивает значение на `100`; хранится как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usRiotShieldGraphic`

- **Type:** `Integer` (`UINT16`)
- **Description:** Графика полицейского щита, когда щит развёрнут в тактическом режиме; берётся из `Tilecache/riotshield.sti`.
- **Values:** `0..65535`; хранится как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentNoiseReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентное снижение шума, создаваемого предметом.
- **Numeric semantics:** Процент: большие значения дают более сильный эффект, отрицательные снижают его ниже базового уровня.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Боевые модификаторы.** Модификаторы попадания, дальности, урона, ОД, очереди и автоогня.

### `ITEMLIST/ITEM/Bipod`

- **Type:** `Integer` (`INT16`)
- **Description:** Плоский модификатор сошек. Переработка NCTH перечисляет это значение среди переменных стрельбы, которые она удалила.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ToHitBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Плоский бонус к вероятности попадания.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BestLaserRange`

- **Type:** `Integer` (`INT16`)
- **Description:** Дальность, используемая для значения лазерной проекции; описание предмета масштабирует её модификатором настройки предмета и размером тайла.
- **Numeric semantics:** Большие значения дают более длинную проекцию; загрузчик сохраняет значение как обычное целое.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Плоский бонус к дальности.
- **Numeric semantics:** Знаковое число единиц дальности; отрицательные значения сохраняются как есть.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентный бонус к дальности.
- **Numeric semantics:** Процент: большие значения дают более сильный эффект, отрицательные снижают его ниже базового уровня.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AimBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Плоский бонус к прицеливанию.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/MinRangeForAimBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Минимальная дальность, на которой применяется `AimBonus`.
- **Numeric semantics:** Порог дальности в тех же единицах, что и дальность оружия; отрицательные значения сохраняются как есть, но не имеют смысла для минимальной дальности.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentAPReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентное снижение затрат ОД на действия с этим предметом.
- **Numeric semantics:** Процент: затраты ОД умножаются на `(100 - value)/100`, поэтому `0` — без снижения, а отрицательное значение увеличило бы затраты.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentStatusDrainReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентное снижение расхода состояния/прочности.
- **Numeric semantics:** Процент: потеря состояния умножается на `(100 - value)/100`, поэтому `0` — без снижения, а отрицательное значение увеличило бы расход.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BloodiedItem`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета окровавленного варианта, который получается, когда этот предмет окровавлен.
- **Numeric semantics:** ID предмета; `0` означает отсутствие окровавленного варианта.
- **Values:** ID предмета; загрузчик разбирает его через приведение к `UINT16` и сохраняет в член `INT16` (`0` = нет).
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/HearingRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к дальности слуха.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/VisionRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к общей дальности обзора.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/NightVisionRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к дальности ночного видения.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DayVisionRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к дальности дневного видения.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CaveVisionRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к дальности видения в пещерах.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BrightLightVisionRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к дальности обзора при ярком свете.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DamageBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Плоский бонус к урону, добавляемый к вычисленному урону оружия (в том числе от заряженных боеприпасов и вложений).
- **Numeric semantics:** Целое, добавляемое к урону после вычисления базового урона; отрицательные значения вычитают.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/MeleeDamageBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Плоский бонус к урону в ближнем бою, добавляемый к вычисленному урону ближнего боя.
- **Numeric semantics:** Целое, добавляемое к урону ближнего боя после вычисления базового урона; отрицательные значения вычитают.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/MagSizeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к размеру магазина.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentBurstFireAPReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентное снижение ОД для стрельбы очередью.
- **Numeric semantics:** Процент: затраты ОД на очередь умножаются на `(100 - value)/100`.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentAutofireAPReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентное снижение ОД для автоогня.
- **Numeric semantics:** Процент: затраты ОД на автоогонь умножаются на `(100 - value)/100`.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentReadyTimeAPReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентное снижение ОД на подготовку предмета.
- **Numeric semantics:** Процент: сложность подготовки (обращения) умножается на `(100 - value)/100`.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentReloadTimeAPReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентное снижение ОД на перезарядку.
- **Numeric semantics:** Процент: затраты ОД на перезарядку умножаются на `(100 - value)/100`.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AutoFireToHitBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к попаданию, применяемый к автоогню.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/APBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонусные очки действия, пока предмет экипирован.
- **Numeric semantics:** Загрузчик пересчитывает значение из шкалы со 100 ОД в настроенную систему ОД (`value * AP_MAXIMUM / 100`), поэтому хранимое значение отличается от значения в файле, когда `AP_MAXIMUM` не равен `100`.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RateOfFireBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к скорострельности.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BurstSizeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Дополнительные выстрелы в очереди.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BurstToHitBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к попаданию, применяемый к стрельбе очередью.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BulletSpeedBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к скорости пули.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Маскировка, обзор и модификаторы тела.** Маскировка, скрытность, наблюдательность, обзор и модификаторы рюкзака/огня.

### `ITEMLIST/ITEM/CamoBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Общий бонус к маскировке.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/UrbanCamoBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Дополнительный бонус к маскировке в городских секторах.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DesertCamoBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Дополнительный бонус к маскировке в пустынных секторах.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/SnowCamoBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Дополнительный бонус к маскировке в снежных секторах.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/StealthBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к скрытности; повышает шанс избежать обнаружения.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usSpotting`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор наблюдательности.
- **Numeric semantics:** Большие значения улучшают наблюдательность; `0` — нейтрально. Загрузчик ограничивает хранимое значение диапазоном `0..100`, поэтому отрицательный ввод становится `0`, а не штрафом.
- **Values:** `0..100`; загрузчик ограничивает значение этим диапазоном (`min(100, max(0, ...))`); хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/sBackpackWeightModifier`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор грузоподъёмности, применяемый, когда предмет находится в рюкзаке.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/sFireResistance`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор огнестойкости.
- **Numeric semantics:** Знаковая стойкость: большие значения означают большую огнестойкость; загрузчик ограничивает значение на `100`.
- **Values:** `-32768..32767`; загрузчик ограничивает значение на `100`, но не ограничивает нижнюю границу; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentTunnelVision`

- **Type:** `Integer` (`UINT8`)
- **Description:** Процент туннельного зрения, навязываемого предметом.
- **Numeric semantics:** Процент туннельного зрения; загрузчик сохраняет `UINT8` и не ограничивает значение, поэтому значения выше `100` сохраняются как есть.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Система вложений, утилиты и транспорт.** Режим вложений, наборы, навыки, прогресс транспорта и вложения по умолчанию.

### `ITEMLIST/ITEM/AttachmentSystem`

- **Type:** `Lookup` (integer ID, `UINT8`)
- **Description:** Доступность системы вложений: `0` = обе, `1` = только старая система вложений, `2` = только NAS. Предмет недоступен, пока активна другая система.
- **Values:** `0`, `1` или `2`, хранится как `UINT8`. См. [AttachmentSystem](../Lookup.md#attachmentsystem).
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CrowbarModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Модификатор, применяемый, когда предмет используется как лом.
- **Numeric semantics:** Беззнаковое значение: отрицательный ввод заворачивается при приведении к `UINT8` вместо того, чтобы действовать как штраф.
- **Values:** `0..255`; хранится как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DisarmModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Модификатор, применяемый при обезвреживании ловушек или бомб.
- **Numeric semantics:** Беззнаковое значение: отрицательный ввод заворачивается при приведении к `UINT8` вместо того, чтобы действовать как штраф.
- **Values:** `0..255`; хранится как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RepairModifier`

- **Type:** `Integer` (`INT8`)
- **Description:** Модификатор эффективности ремнабора.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usHackingModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Модификатор взлома.
- **Numeric semantics:** Беззнаковый модификатор; загрузчик сохраняет не более `100`.
- **Values:** Фактически `0..100`: загрузчик приводит к `UINT8` и сохраняет не более `100`, поэтому значения выше `100` и отрицательные значения сохраняются как `100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usBurialModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Модификатор эффективности захоронения.
- **Numeric semantics:** Беззнаковый модификатор; загрузчик сохраняет не более `100`.
- **Values:** Фактически `0..100`: загрузчик приводит к `UINT8` и сохраняет не более `100`, поэтому значения выше `100` и отрицательные значения сохраняются как `100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DamageChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что повреждение состояния также повреждает порог ремонта (расширенная система ремонта/загрязнения).
- **Numeric semantics:** Большие значения означают больший шанс; отрицательные значения вне допустимого диапазона шанса.
- **Values:** `0..255`; хранится как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/FlashLightRange`

- **Type:** `Integer` (`UINT8`)
- **Description:** Дальность фонаря; любой предмет со значением выше `0` считается фонарём.
- **Numeric semantics:** Большие значения означают большую дальность/радиус, отрицательные значения не имеют смысла.
- **Values:** `0..255`; хранится как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ItemChoiceTimeSetting`

- **Type:** `Integer` (`UINT8`)
- **Description:** Настройка времени, определяющая, когда ИИ может выбрать этот предмет для своих решений.
- **Numeric semantics:** `0`, `1` и `2` — единственные сохраняемые значения; любой другой ввод ограничивается этим диапазоном.
- **Values:** `0..2`; загрузчик ограничивает значение этим диапазоном; хранится как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/SleepModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Бонус к восстановлению дыхания, который предмет даёт во время отдыха.
- **Numeric semantics:** Беззнаковое значение: загрузчик сохраняет `UINT8` и не ограничивает его, поэтому отрицательный ввод заворачивается при приведении.
- **Values:** `0..255`; хранится как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usPortionSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Размер порции для предметов еды или напитков.
- **Numeric semantics:** Большие значения означают большую порцию; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `100`; загрузчик задаёт это значение до разбора записи.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usAdministrationModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Модификатор эффективности управления для стратегических действий.
- **Numeric semantics:** Большие значения означают более сильный эффект управления; `0` означает отсутствие модификатора, отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/LockPickModifier`

- **Type:** `Integer` (`INT8`)
- **Description:** Бонус к взлому замков.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/randomitemcoolnessmodificator`

- **Type:** `Integer` (`INT8`)
- **Description:** Поправка крутости, применяемая, когда этот предмет выбран как случайная добыча.
- **Numeric semantics:** Знаковая поправка крутости, ограничиваемая загрузчиком диапазоном `-20..20`.
- **Values:** `-20..20`; загрузчик ограничивает значение этим диапазоном; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotStrBonus`

- **Type:** `Integer` (`INT8`)
- **Description:** Бонус к силе робота.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotAgiBonus`

- **Type:** `Integer` (`INT8`)
- **Description:** Бонус к ловкости робота.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotDexBonus`

- **Type:** `Integer` (`INT8`)
- **Description:** Бонус к проворству робота.
- **Numeric semantics:** Знаковый модификатор: большие значения дают более сильный положительный эффект, отрицательные — противоположный (штраф или снижение).
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotTargetingSkillGrant`

- **Type:** `Integer` (`INT8`)
- **Description:** Навык стрельбы (targeting), даруемый шасси или модулем робота.
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotUtilitySkillGrant`

- **Type:** `Integer` (`INT8`)
- **Description:** Навык утилиты (utility), даруемый шасси или модулем робота.
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotChassisSkillGrant`

- **Type:** `Integer` (`INT8`)
- **Description:** Навык шасси (chassis), даруемый предметом робота.
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/TransportGroupMinProgress`

- **Type:** `Integer` (`INT8`)
- **Description:** Нижняя граница прогресса, при которой транспортная группа может выдать этот предмет.
- **Numeric semantics:** Знаковая граница прогресса; загрузчик сохраняет `INT8` и не ограничивает его.
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/TransportGroupMaxProgress`

- **Type:** `Integer` (`INT8`)
- **Description:** Верхняя граница прогресса, при которой транспортная группа может выдать этот предмет.
- **Numeric semantics:** Знаковая граница прогресса; загрузчик сохраняет `INT8` и не ограничивает его.
- **Values:** `-128..127`; хранится как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DefaultAttachment`

- **Type:** `Integer` (`UINT16`)
- **Description:** Повторяющийся элемент, задающий ID предмета, устанавливаемого по умолчанию. До двадцати вхождений (`MAX_DEFAULT_ATTACHMENTS`); пусто означает отсутствие вложения по умолчанию.
- **Numeric semantics:** Каждое вхождение — один ID предмета; `0` означает отсутствие вложения в этом слоте.
- **Values:** ID предмета (`0..65535`), повторяется; значения заполняют первый свободный слот, поэтому запись `0` пропускается, а вхождения сверх двадцати игнорируются.
- **Default:** Пусто; вложения по умолчанию нет.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Флаги предметов.** Булевы роли предметов, используемые в игровых системах.

### `ITEMLIST/ITEM/Bloodbag`

- **Type:** `Boolean`
- **Description:** Помечает предмет как пакет крови, пригодный для переливания.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Manpad`

- **Type:** `Boolean`
- **Description:** Помечает предмет как пусковую установку ПЗРК (MANPADS).
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Beartrap`

- **Type:** `Boolean`
- **Description:** Помечает предмет как медвежий капкан.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Camera`

- **Type:** `Boolean`
- **Description:** Помечает предмет как фотоаппарат.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Waterdrum`

- **Type:** `Boolean`
- **Description:** Помечает предмет как бочку для воды.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BloodcatMeat`

- **Type:** `Boolean`
- **Description:** Помечает предмет как мясо кровокота.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CowMeat`

- **Type:** `Boolean`
- **Description:** Помечает предмет как мясо коровы.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Beltfed`

- **Type:** `Boolean`
- **Description:** Помечает оружие как имеющее ленточное питание.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Ammobelt`

- **Type:** `Boolean`
- **Description:** Помечает предмет как патронную ленту.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AmmobeltVest`

- **Type:** `Boolean`
- **Description:** Помечает предмет как разгрузку под патронные ленты.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CamoRemoval`

- **Type:** `Boolean`
- **Description:** Помечает предмет как средство удаления маскировки.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Cleaningkit`

- **Type:** `Boolean`
- **Description:** Помечает предмет как набор для чистки оружия.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AttentionItem`

- **Type:** `Boolean`
- **Description:** Помечает предмет как привлекающий внимание; противники могут его заметить.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Garotte`

- **Type:** `Boolean`
- **Description:** Помечает предмет как удавку.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Covert`

- **Type:** `Boolean`
- **Description:** Помечает предмет как оборудование для скрытной работы.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Corpse`

- **Type:** `Boolean`
- **Description:** Помечает предмет как труп.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BloodcatSkin`

- **Type:** `Boolean`
- **Description:** Помечает предмет как шкуру кровокота.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/NoMetalDetection`

- **Type:** `Boolean`
- **Description:** Предмет не обнаруживается металлодетектором.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/JumpGrenade`

- **Type:** `Boolean`
- **Description:** Помечает гранату как прыгающую.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Handcuffs`

- **Type:** `Boolean`
- **Description:** Помечает предмет как наручники.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Taser`

- **Type:** `Boolean`
- **Description:** Помечает предмет как тазер.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ScubaBottle`

- **Type:** `Boolean`
- **Description:** Помечает предмет как баллон для подводного плавания.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ScubaMask`

- **Type:** `Boolean`
- **Description:** Помечает предмет как маску для подводного плавания.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ScubaFins`

- **Type:** `Boolean`
- **Description:** Помечает предмет как ласты.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/TripwireRoll`

- **Type:** `Boolean`
- **Description:** Помечает предмет как моток растяжки.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Radioset`

- **Type:** `Boolean`
- **Description:** Помечает предмет как радиостанцию.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/SignalShell`

- **Type:** `Boolean`
- **Description:** Помечает предмет как сигнальный снаряд.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Soda`

- **Type:** `Boolean`
- **Description:** Помечает предмет как газированный напиток.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RoofcollapseItem`

- **Type:** `Boolean`
- **Description:** Помечает предмет как предмет обрушения крыши.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DiseaseprotectionFace`

- **Type:** `Boolean`
- **Description:** Защищает лицо от болезней.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DiseaseprotectionHand`

- **Type:** `Boolean`
- **Description:** Защищает руки от болезней.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/LBEexplosionproof`

- **Type:** `Boolean`
- **Description:** Помечает разгрузку как стойкую к взрыву.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/EmptyBloodbag`

- **Type:** `Boolean`
- **Description:** Помечает предмет как пустой пакет крови.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/MedicalSplint`

- **Type:** `Boolean`
- **Description:** Помечает предмет как медицинскую шину.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Damageable`

- **Type:** `Boolean`
- **Description:** Предмет может получать повреждения состояния.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Repairable`

- **Type:** `Boolean`
- **Description:** Предмет можно отремонтировать.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/WaterDamages`

- **Type:** `Boolean`
- **Description:** Вода повреждает предмет.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Metal`

- **Type:** `Boolean`
- **Description:** Предмет сделан из металла.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Sinks`

- **Type:** `Boolean`
- **Description:** Предмет тонет в воде.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ShowStatus`

- **Type:** `Boolean`
- **Description:** Показывать состояние или статус предмета в интерфейсе.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/HiddenAddon`

- **Type:** `Boolean`
- **Description:** Аддон скрыт из обычных списков вложений.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/TwoHanded`

- **Type:** `Boolean`
- **Description:** Для предмета требуются две руки.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/NotBuyable`

- **Type:** `Boolean`
- **Description:** Предмет не продаётся Bobby Ray.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Attachment`

- **Type:** `Boolean`
- **Description:** Помечает предмет как вложение.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/HiddenAttachment`

- **Type:** `Boolean`
- **Description:** Вложение скрыто из обычных списков вложений.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BigGunList`

- **Type:** `Boolean`
- **Description:** Включает предмет в режим Tons of Guns.
- **Only used with:** Tons of Guns.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/NotInEditor`

- **Type:** `Boolean`
- **Description:** Скрывает предмет из обычных списков редактора.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DefaultUndroppable`

- **Type:** `Boolean`
- **Description:** Предмет нельзя выбросить по умолчанию.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Unaerodynamic`

- **Type:** `Boolean`
- **Description:** Предмет обладает плохой аэродинамикой.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Electronic`

- **Type:** `Boolean`
- **Description:** Предмет электронный.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Cannon`

- **Type:** `Boolean`
- **Description:** Помечает оружие как пушку.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RocketRifle`

- **Type:** `Boolean`
- **Description:** Помечает оружие как реактивную винтовку.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/FingerPrintID`

- **Type:** `Boolean`
- **Description:** Помечает предмет как устройство идентификации по отпечатку пальца.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/MetalDetector`

- **Type:** `Boolean`
- **Description:** Помечает предмет как металлодетектор.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/GasMask`

- **Type:** `Boolean`
- **Description:** Помечает предмет как противогаз.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/LockBomb`

- **Type:** `Boolean`
- **Description:** Помечает предмет как бомбу-замок.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Flare`

- **Type:** `Boolean`
- **Description:** Помечает предмет как сигнальную ракету.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/GrenadeLauncher`

- **Type:** `Boolean`
- **Description:** Помечает оружие как гранатомёт.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Mortar`

- **Type:** `Boolean`
- **Description:** Помечает оружие как миномёт.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Duckbill`

- **Type:** `Boolean`
- **Description:** Помечает дробовик как имеющий дульное сужение «утиный клюв».
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/HideMuzzleFlash`

- **Type:** `Boolean`
- **Description:** Скрывает дульную вспышку при выстреле из оружия.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RocketLauncher`

- **Type:** `Boolean`
- **Description:** Помечает оружие как реактивную пусковую установку.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/SingleShotRocketLauncher`

- **Type:** `Boolean`
- **Description:** Помечает пусковую установку как одноразовую.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BrassKnuckles`

- **Type:** `Boolean`
- **Description:** Помечает предмет как кастет.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Crowbar`

- **Type:** `Boolean`
- **Description:** Помечает предмет как лом.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/GLGrenade`

- **Type:** `Boolean`
- **Description:** Помечает предмет как гранату для гранатомёта.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/FlakJacket`

- **Type:** `Boolean`
- **Description:** Помечает броню как бронежилет.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/LeatherJacket`

- **Type:** `Boolean`
- **Description:** Помечает броню как кожаную куртку.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Batteries`

- **Type:** `Boolean`
- **Description:** Помечает предмет как батарейки.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/NeedsBatteries`

- **Type:** `Boolean`
- **Description:** Для работы предмету нужны батарейки.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/XRay`

- **Type:** `Boolean`
- **Description:** Помечает предмет как рентгеновское устройство.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/WireCutters`

- **Type:** `Boolean`
- **Description:** Помечает предмет как кусачки.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Toolkit`

- **Type:** `Boolean`
- **Description:** Помечает предмет как набор инструментов.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/FirstAidKit`

- **Type:** `Boolean`
- **Description:** Помечает предмет как аптечку первой помощи.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/MedicalKit`

- **Type:** `Boolean`
- **Description:** Помечает предмет как медицинский набор.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Canteen`

- **Type:** `Boolean`
- **Description:** Помечает предмет как флягу.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Jar`

- **Type:** `Boolean`
- **Description:** Помечает предмет как банку.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CanAndString`

- **Type:** `Boolean`
- **Description:** Помечает предмет как сигнализацию из банки с верёвкой.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Marbles`

- **Type:** `Boolean`
- **Description:** Помечает предмет как шарики.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Walkman`

- **Type:** `Boolean`
- **Description:** Помечает предмет как плеер Walkman.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RemoteTrigger`

- **Type:** `Boolean`
- **Description:** Помечает предмет как дистанционный взрыватель.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotRemoteControl`

- **Type:** `Boolean`
- **Description:** Помечает предмет как пульт управления роботом.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CamouflageKit`

- **Type:** `Boolean`
- **Description:** Помечает предмет как набор маскировки.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/LocksmithKit`

- **Type:** `Boolean`
- **Description:** Помечает предмет как набор взломщика.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Mine`

- **Type:** `Boolean`
- **Description:** Помечает предмет как мину.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AntitankMine`

- **Type:** `Boolean`
- **Description:** Помечает предмет как противотанковую мину.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Hardware`

- **Type:** `Boolean`
- **Description:** Помечает предмет как хозтовары.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Medical`

- **Type:** `Boolean`
- **Description:** Помечает предмет как медицинские припасы.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/GasCan`

- **Type:** `Boolean`
- **Description:** Помечает предмет как канистру с бензином.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ContainsLiquid`

- **Type:** `Boolean`
- **Description:** Предмет содержит жидкость.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Rock`

- **Type:** `Boolean`
- **Description:** Помечает предмет как камень.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ThermalOptics`

- **Type:** `Boolean`
- **Description:** Помечает предмет как тепловизионную оптику.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/SciFi`

- **Type:** `Boolean`
- **Description:** Включает предмет в режим Sci-Fi.
- **Only used with:** Sci-Fi.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/NewInv`

- **Type:** `Boolean`
- **Description:** Помечает предмет как существующий только в новом инвентаре (NIV). Старый инвентарь (OIV) его не показывает.
- **Only used with:** NIV.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DiseaseSystemExclusive`

- **Type:** `Boolean`
- **Description:** Предмет относится к системе болезней.
- **Only used with:** Система болезней.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Barrel`

- **Type:** `Boolean`
- **Description:** Помечает предмет как ствол.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/TripWireActivation`

- **Type:** `Boolean`
- **Description:** Помечает предмет как активатор растяжки.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/TripWire`

- **Type:** `Boolean`
- **Description:** Помечает предмет как растяжку.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Directional`

- **Type:** `Boolean`
- **Description:** Предмет направленный.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BlockIronSight`

- **Type:** `Boolean`
- **Description:** Предмет блокирует механический прицел.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AllowClimbing`

- **Type:** `Boolean`
- **Description:** Предмет позволяет лазать.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Cigarette`

- **Type:** `Boolean`
- **Description:** Помечает предмет как сигарету. Загрузчик сопоставляет имя элемента в нижнем регистре — `cigarette`.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ProvidesRobotCamo`

- **Type:** `Boolean`
- **Description:** Даёт роботу маскировку.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ProvidesRobotNightVision`

- **Type:** `Boolean`
- **Description:** Даёт роботу ночное видение.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ProvidesRobotLaserBonus`

- **Type:** `Boolean`
- **Description:** Даёт роботу лазерный бонус.
- **Values:** `0` (нет) или `1` (да); загрузчик устанавливает флаг, если разобранное значение не равно нулю.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Группы модификаторов стоек.** Три контейнера структурны; каждый
содержит одни и те же одиннадцать полей, перечисленных после блоков групп.

### `ITEMLIST/ITEM/STAND_MODIFIERS`

- **Type:** `Container` (11 integers)
- **Description:** Структурная группа модификаторов прицеливания и обращения для стояния. Собственного значения у неё нет; модификаторы несут дочерние поля.
- **Values:** Один дочерний элемент на модификатор; отсутствующий дочерний элемент остаётся `0`.
- **Default:** Отсутствует; отсутствующий дочерний элемент остаётся `0`.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/FlatBase`

- **Type:** `Integer` (`INT16`)
- **Description:** Плоский модификатор базы в NCTH при стоянии.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый плоский модификатор: отрицательные значения уменьшают базовое значение.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение нейтрально (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentBase`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентный модификатор базы в NCTH при стоянии.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор: отрицательные значения снижают эффект ниже базового уровня.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение нейтрально (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/FlatAim`

- **Type:** `Integer` (`INT16`)
- **Description:** Плоский модификатор прицеливания в NCTH при стоянии.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый плоский модификатор: отрицательные значения уменьшают значение прицеливания.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение нейтрально (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentAim`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентный модификатор прицеливания в NCTH при стоянии.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор: отрицательные значения снижают эффект ниже базового уровня.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение нейтрально (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentCap`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор предела прицеливания в NCTH при стоянии.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор предела прицеливания; отрицательные значения уменьшают его.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение нейтрально (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentHandling`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор обращения в NCTH при стоянии.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор обращения; отрицательные значения уменьшают эффект.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение нейтрально (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentTargetTrackingSpeed`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор скорости сопровождения цели в NCTH при стоянии.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор скорости сопровождения цели; отрицательные значения уменьшают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение нейтрально (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentDropCompensation`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор компенсации падения пули в NCTH при стоянии.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор компенсации падения; отрицательные значения уменьшают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение нейтрально (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentMaxCounterForce`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор максимальной противодействующей силы в NCTH при стоянии.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор максимальной противодействующей силы; отрицательные значения уменьшают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение нейтрально (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentCounterForceAccuracy`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор точности противодействующей силы в NCTH при стоянии.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор точности противодействующей силы; отрицательные значения уменьшают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение нейтрально (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/AimLevels`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор количества уровней прицеливания в NCTH при стоянии.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый модификатор количества уровней прицеливания для этой стойки.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение нейтрально (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS`

- **Type:** `Container` (11 integers)
- **Description:** Структурная группа модификаторов прицеливания и обращения для приседа. Собственного значения у неё нет; модификаторы несут дочерние поля.
- **Values:** Один дочерний элемент на модификатор; отсутствующий дочерний элемент наследует следующее заданное значение стойки.
- **Default:** Отсутствует; отсутствующий дочерний элемент наследует следующее заданное значение стойки.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/FlatBase`

- **Type:** `Integer` (`INT16`)
- **Description:** Плоский модификатор базы в NCTH при приседе.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый плоский модификатор: отрицательные значения уменьшают базовое значение.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentBase`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентный модификатор базы в NCTH при приседе.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор: отрицательные значения снижают эффект ниже базового уровня.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/FlatAim`

- **Type:** `Integer` (`INT16`)
- **Description:** Плоский модификатор прицеливания в NCTH при приседе.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый плоский модификатор: отрицательные значения уменьшают значение прицеливания.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentAim`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентный модификатор прицеливания в NCTH при приседе.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор: отрицательные значения снижают эффект ниже базового уровня.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentCap`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор предела прицеливания в NCTH при приседе.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор предела прицеливания; отрицательные значения уменьшают его.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentHandling`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор обращения в NCTH при приседе.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор обращения; отрицательные значения уменьшают эффект.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentTargetTrackingSpeed`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор скорости сопровождения цели в NCTH при приседе.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор скорости сопровождения цели; отрицательные значения уменьшают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentDropCompensation`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор компенсации падения пули в NCTH при приседе.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор компенсации падения; отрицательные значения уменьшают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentMaxCounterForce`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор максимальной противодействующей силы в NCTH при приседе.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор максимальной противодействующей силы; отрицательные значения уменьшают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentCounterForceAccuracy`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор точности противодействующей силы в NCTH при приседе.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор точности противодействующей силы; отрицательные значения уменьшают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/AimLevels`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор количества уровней прицеливания в NCTH при приседе.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый модификатор количества уровней прицеливания для этой стойки.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS`

- **Type:** `Container` (11 integers)
- **Description:** Структурная группа модификаторов прицеливания и обращения для лежачего положения. Собственного значения у неё нет; модификаторы несут дочерние поля.
- **Values:** Один дочерний элемент на модификатор; отсутствующий дочерний элемент наследует следующее заданное значение стойки.
- **Default:** Отсутствует; отсутствующий дочерний элемент наследует следующее заданное значение стойки.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Поля модификаторов стоек (NCTH).** В каждой группе стоек одни и те же одиннадцать полей; их читает система New Chance to Hit.
Пустое значение означает нейтральность; положительные и отрицательные проценты допускаются.

### `ITEMLIST/ITEM/PRONE_MODIFIERS/FlatBase`

- **Type:** `Integer` (`INT16`)
- **Description:** Плоский модификатор базы в NCTH в лежачем положении.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый плоский модификатор: отрицательные значения уменьшают базовое значение.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentBase`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентный модификатор базы в NCTH в лежачем положении.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор: отрицательные значения снижают эффект ниже базового уровня.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/FlatAim`

- **Type:** `Integer` (`INT16`)
- **Description:** Плоский модификатор прицеливания в NCTH в лежачем положении.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый плоский модификатор: отрицательные значения уменьшают значение прицеливания.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentAim`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентный модификатор прицеливания в NCTH в лежачем положении.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор: отрицательные значения снижают эффект ниже базового уровня.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentCap`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор предела прицеливания в NCTH в лежачем положении.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор предела прицеливания; отрицательные значения уменьшают его.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentHandling`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор обращения в NCTH в лежачем положении.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор обращения; отрицательные значения уменьшают эффект.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentTargetTrackingSpeed`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор скорости сопровождения цели в NCTH в лежачем положении.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор скорости сопровождения цели; отрицательные значения уменьшают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentDropCompensation`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор компенсации падения пули в NCTH в лежачем положении.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор компенсации падения; отрицательные значения уменьшают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentMaxCounterForce`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор максимальной противодействующей силы в NCTH в лежачем положении.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор максимальной противодействующей силы; отрицательные значения уменьшают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentCounterForceAccuracy`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор точности противодействующей силы в NCTH в лежачем положении.
- **Only used with:** NCTH.
- **Numeric semantics:** Процентный модификатор точности противодействующей силы; отрицательные значения уменьшают её.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/AimLevels`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор количества уровней прицеливания в NCTH в лежачем положении.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый модификатор количества уровней прицеливания для этой стойки.
- **Values:** `-32768..32767`; хранится как `INT16`.
- **Default:** `0`, если элемент отсутствует; отсутствующее значение наследует следующее заданное значение стойки (`-10000` — внутренний маркер «не задано»).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

## Relations

- `Items.uiIndex` используется многими таблицами: стартовое снаряжение, случайные
  предметы, вложения, ассортимент торговцев, дополнительные объекты карт и данные квестов.
- `Items.ubClassIndex` выбирает строку в таблице, задаваемой `usItemClass`;
  см. [Armours](Armours.md), [Magazines](Magazines.md) и
  [Launchables](Launchables.md).
- `Items.ubCursor` -> [Lookup.Cursor](../Lookup.md#cursor); `Items.Inseparable` -> [Lookup.Separability](../Lookup.md#separability).
- `Items.AttachmentClass` -> [Lookup.AttachmentClass](../Lookup.md#attachmentclass); `Items.nasAttachmentClass` ->
  [Lookup.NasAttachmentClass](../Lookup.md#nasattachmentclass); `Items.nasLayoutClass` -> [Lookup.NasLayoutClass](../Lookup.md#naslayoutclass).
- `Items.AttachmentPoint` и `Items.AvailableAttachmentPoint` ->
  [Lookup.AttachmentPoint](../Lookup.md#attachmentpoint); `Items.AttachmentSystem` -> [Lookup.AttachmentSystem](../Lookup.md#attachmentsystem).
- `Items.DrugType` -> [Lookup.DrugType](../Lookup.md#drugtype); `Items.FoodType` -> определения [Food](Food.md).
- `Items.clothestype` -> определения [Clothes](Clothes.md).
- `Items.spreadPattern` -> определения [SpreadPatterns](../SpreadPatterns.md).
- `Items.DefaultAttachment`, `Items.DiscardedLauncherItem`,
  `Items.BloodiedItem`, `Items.buddyitem` и `Items.randomitem` — это ID предметов
  или случайных предметов.
- `Items.usActionItemFlag` — битовая маска, а не ID предмета.

## Related formulas

Формулы урона и брони используют несколько значений предметов и боеприпасов. Они
описаны в [FORMULAS.md](../../FORMULAS.md):

- [Armour protection](../../FORMULAS.md#armour-protection)
- [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt)

## Localization

Переводимые поля — `szItemName`, `szLongItemName`, `szItemDesc`,
`szBRName` и `szBRDesc`. Они находятся в `Russian.Items.xml` и других
языковых оверлеях с ключом `uiIndex`. Базовый `Items.xml` остаётся схемой
и запасным вариантом для непереведённых полей. См.
[Localization](../../Localization.md).

Имена полей, ID предметов, биты классов и числа никогда не переводятся.

## Versions

- Основной источник для документа — v5.
- В v4 тот же набор листовых путей; текст и несколько значений отличаются из-за
  локализации и правок содержимого.
- `DefaultAttachment` — один документированный путь с повторяющимися элементами;
  повторяемость — структурное свойство, а не отдельное поле.

## Sources

- Данные таблицы предметов: основной источник v5, сравнение с v4.
- Метаданные свойств и флагов предметов из базы данных редактора.
- Справочники курсоров, классов вложений, отделимости, наркотиков и
  систем вложений.
- Оверлеи локализации для пяти текстовых полей.
- Исходный код игры на C++: загрузчик `Items.xml` и структура предмета в
  заголовке типов предметов.

Этот блок сверен с игровым загрузчиком `Items.xml` и структурой предмета.
Загрузчик устанавливает тип, ширину хранения, вызов разбора и любое
ограничение или наследование для каждого поля; именованные значения битов
по-прежнему берутся из справочников. Оставшиеся неопределённости перечислены
ниже.

## Open questions

- `DrugType`: структура предмета определяет поле как маску флагов; то, какие
  биты соответствуют каким компонентам наркотика, по-прежнему берётся из
  справочника.
- `FoodType`: строки определяет таблица [Food](Food.md); файл предметов хранит
  только индекс класса.
- `bSoundType`: полное перечисление семейств звуков не восстановлено.
- `RobotTargetingSkillGrant`, `RobotUtilitySkillGrant` и
  `RobotChassisSkillGrant`: единица измерения (очки навыка или уровень навыка)
  не проверена.
- `TransportGroupMinProgress` и `TransportGroupMaxProgress`: единица измерения и
  шкала не подтверждены; документ трактует их как значения прогресса транспорта.
- `BR_NewInventory`, `BR_UsedInventory` и `BR_ROF`: загрузчик хранит их в
  массивах запаса Bobby Ray, но формула запаса и пополнения здесь
  не восстановлена.
- `RobotDamageReduction`: загрузчик только сохраняет десятичное значение;
  трактует ли код робота его как множитель или как процент, не восстановлено.
- `AttachmentClass`, `nasAttachmentClass` и `nasLayoutClass` документированы
  как битовые маски; имена отдельных битов находятся на соответствующих
  страницах справочников.
