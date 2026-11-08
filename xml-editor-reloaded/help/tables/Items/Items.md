---
schema: 1
lang: en
family: items-core
title: "Items - item master table"
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
updated: 2026-11-08
---

# Items - item master table

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

`Items.xml` is the master list of every item in the game. Each `ITEM` record
defines one item ID, its name and description, its class, inventory data,
combat modifiers, flags and links to class-specific tables. Other tables use
the item `uiIndex` as a foreign key, so this table is the hub of most item
relationships.

The table is flat: one `ITEM` record per item, keyed by `uiIndex`.

## Tables

| File | Root / record | Notes |
|---|---|---|
| `Items/Items.xml` | `ITEMLIST` / `ITEM` | Base item table. |

The localized overlays (`Russian.Items.xml` and the other language files) are
not separate tables. They override the five text fields by `uiIndex`; see
[Localization](../../Localization.md).

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

- Root: `ITEMLIST`; record: `ITEM`; key: `uiIndex`.
- Flat fields plus three nested stance groups: `STAND_MODIFIERS`,
  `CROUCH_MODIFIERS` and `PRONE_MODIFIERS`. Each group holds eleven `INT16`
  modifiers. A missing crouch value inherits the standing value and a missing
  prone value inherits the crouch value; a missing standing value stays `0`.
- `DefaultAttachment` is a repeated element. A record may contain up to twenty
  occurrences (`MAX_DEFAULT_ATTACHMENTS`); each occurrence is one item ID.
  Values fill the first free slot, so a `0` entry is skipped and later values
  still load; occurrences beyond twenty are ignored. The documentation
  describes the path once and notes the repetition.
- Records are sparse in practice: missing fields take the game default, and
  empty elements are not the same as a numeric zero for text fields.
- Numeric fields keep their lexical form. A decimal such as `1.0` is not
  rewritten as `1` by the editor.

### Item classes and `ubClassIndex`

`usItemClass` is a bit mask. It selects the class-specific subtable used by
`ubClassIndex`:

- armour class -> [Armours](Armours.md);
- magazine/ammo class -> [Magazines](Magazines.md);
- launcher/ammo pairs -> [Launchables](Launchables.md);
- weapon, explosive and other class rows are documented in their own
  family documents.

Do not read `ubClassIndex` without `usItemClass`: the same number means a
different row in a different class table.

## Game options

This table is shared by several optional systems. The option names below are
the ones that change how a field is read; a field without a note is used by
both variants of the relevant system. The general option map is
[OPTIONS.md](../../OPTIONS.md).

| Option | Values | Effect on this table |
|---|---|---|
| New Chance to Hit (NCTH) / old Chance to Hit (OCTH) | `NCTH = TRUE` or `FALSE` in the game option file. `TRUE` selects NCTH; `FALSE` selects OCTH. | NCTH reads `ScopeMagFactor`, `ProjectionFactor`, `RecoilModifierX`, `RecoilModifierY`, `PercentRecoilModifier`, `PercentAccuracyModifier` and the three stance groups. OCTH ignores those fields. |
| New inventory (NIV) / old inventory (OIV) | Selected when a new campaign starts; the choice is stored in the save. The 1.13 data set is NIV, and legacy OIV data remains supported. `OLD_INVENTORY_ITEM_NUMBER` (default `99`) marks the OIV-only item size. | `ubPerPocket` controls OIV pocket capacity only; in NIV the `Pockets` and `LoadBearingEquipment` tables and `ItemSize` control capacity. `NewInv` marks an item as NIV-only. In NIV, an item with `ItemSize` equal to `OLD_INVENTORY_ITEM_NUMBER` is excluded. |
| Sci-Fi mode | `SciFi = TRUE` or `FALSE`. | `SciFi` marks the item as part of the Sci-Fi item set. |
| Tons of Guns | `BigGunList = TRUE` or `FALSE`. | `BigGunList` includes the item in the expanded gun list. |
| Disease system | disease system enabled or disabled. | `DiseaseSystemExclusive` marks an item that belongs to the disease system. |

## Field map

| Group | Fields | Covers |
|---|---:|---|
| Identity and descriptions | 6 | IDs, names and player-facing text. |
| Classification and attachment points | 10 | Item class, attachment systems and class dispatch. |
| Inventory, graphics and economy | 14 | Graphics, weight, size, price and Bobby Ray stock. |
| Food, drugs and action items | 6 | Consumable classes and special-use flags. |
| Weapon modifiers and overheating | 12 | Optics, recoil, accuracy, heat and dirt. |
| Robot, random loot and shield links | 7 | Robot damage, random-item links and riot-shield data. |
| Combat modifiers | 29 | To-hit, range, damage, AP, burst and autofire modifiers. |
| Camouflage, vision and body modifiers | 9 | Camo, stealth, spotting, vision and backpack/fire modifiers. |
| Attachment system, utility and transport | 23 | Attachment mode, kits, skills, transport progress and default attachments. |
| Item flags | 105 | Boolean item roles used across the game systems. |
| Stance modifier groups | 3 | Structural `STAND_MODIFIERS`, `CROUCH_MODIFIERS` and `PRONE_MODIFIERS` containers. |
| Stance modifier fields | 33 | Standing, crouching and prone aim modifiers. |

## Fields

`Type` names the editor field type; the width in parentheses is the C storage
type in the item struct. `Values` gives the range of that storage type and any
clamp the loader applies. The loader casts without validating, so integer text
outside the range is truncated by the cast rather than rejected, and an omitted
element leaves the zero-initialised value (only `usPortionSize` has a non-zero
code default). Confidence `source` means the type, storage width, parse call and any
clamp or inheritance rule were checked against the game's item loader and item
struct. Role descriptions follow the member names in that struct; where a unit,
enum or effect is not established by the code, it is listed under Open
questions. Confidence `inferred` means that check was not possible; no field is
left at that level in this revision.

**Identity and descriptions.** IDs, names and player-facing text.

### `ITEMLIST/ITEM/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Primary item ID. Cross-table references and localization overlays use this value; it is unique inside one file. A record whose `usItemClass` is `0` is skipped by the loader (only its text is copied in a localized overlay pass).
- **Values:** `0..4294967295`; stored as `UINT32`. The array has `MAXITEMS` (`16001`) entries; a larger index is ignored.
- **Default:** None; required and unique inside the file.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szItemName`

- **Type:** `String`
- **Description:** Short item name used in lists and compact cells.
- **Values:** Up to 79 characters plus the terminator (`CHAR16[80]`); longer text is truncated by the loader.
- **Default:** Empty when omitted.
- **Source:** game code (item loader); item table data; item localization overlay.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szLongItemName`

- **Type:** `String`
- **Description:** Full item name used in detail panels and tooltips.
- **Values:** Up to 79 characters plus the terminator (`CHAR16[80]`); longer text is truncated by the loader.
- **Default:** Empty when omitted.
- **Source:** game code (item loader); item table data; item localization overlay.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szItemDesc`

- **Type:** `String`
- **Description:** Item description shown in the detail panel.
- **Values:** Up to 399 characters plus the terminator (`CHAR16[400]`); longer text is truncated by the loader.
- **Default:** Empty when omitted.
- **Source:** game code (item loader); item table data; item localization overlay.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szBRName`

- **Type:** `String`
- **Description:** Bobby Ray's catalogue name.
- **Values:** Up to 79 characters plus the terminator (`CHAR16[80]`); longer text is truncated by the loader.
- **Default:** Empty when omitted.
- **Source:** game code (item loader); item table data; item localization overlay.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szBRDesc`

- **Type:** `String`
- **Description:** Bobby Ray's catalogue description.
- **Values:** Up to 399 characters plus the terminator (`CHAR16[400]`); longer text is truncated by the loader.
- **Default:** Empty when omitted.
- **Source:** game code (item loader); item table data; item localization overlay.
- **Confidence:** `source`

**Classification and attachment points.** Item class, attachment systems and class dispatch.

### `ITEMLIST/ITEM/usItemClass`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Bit mask of item-class flags (`IC_*`). It selects the class-specific subtable reached through `ubClassIndex` and controls which item behaviours apply. A record with `0` is skipped by the loader. See [Lookup.ItemClass](../Lookup.md#itemclass).
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** `0` = none; `0..4294967295`; parsed with `strtoul` base `0` (hexadecimal `0x...` is accepted); stored as `UINT32`.
- **Default:** None; every item declares a class.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AttachmentClass`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Bit mask of the legacy attachment classes (`AC_*`) used by the old attachment system. Bits are defined by the [AttachmentClass](../Lookup.md#attachmentclass) lookup.
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** `0` = none; `0..4294967295`; parsed with `strtoul` base `0` (hexadecimal `0x...` is accepted); stored as `UINT32`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/nasAttachmentClass`

- **Type:** `Flags` (bit mask, `UINT64`)
- **Description:** Custom NAS attachment-class bit mask used with [AttachmentSystem](../Lookup.md#attachmentsystem) and NAS attachment points.
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** `0` = none; parsed with `atof` and truncated to `UINT64`, so only decimal text is read; stored as `UINT64`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/nasLayoutClass`

- **Type:** `Flags` (bit mask, `UINT64`)
- **Description:** NAS layout-class bit mask. It tells the NAS layout which attachment slots the item exposes; bits are defined by [NasLayoutClass](../Lookup.md#naslayoutclass).
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** `0` = none; parsed with `atof` and truncated to `UINT64`, so only decimal text is read; stored as `UINT64`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AvailableAttachmentPoint`

- **Type:** `Flags` (bit mask, `UINT64`)
- **Description:** Bit mask of attachment points the item can accept. Repeated elements are combined with a bitwise OR, so several masks can be listed for one item; bits are defined by the [AttachmentPoint](../Lookup.md#attachmentpoint) lookup.
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** `0` = none; parsed with `atof` and used as a `UINT64` mask; repeated elements are combined with a bitwise OR.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AttachmentPoint`

- **Type:** `Flags` (bit mask, `UINT64`)
- **Description:** Attachment point occupied by this item when installed. Zero means the item is not installed at a point; a repeated element replaces the earlier value.
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** `0` = none; parsed with `atof` and used as a `UINT64` mask; a repeated element replaces the previous value.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AttachToPointAPCost`

- **Type:** `Integer` (`UINT8`)
- **Description:** AP cost to attach or detach the item at its attachment point.
- **Numeric semantics:** Action-point cost; higher values make attaching/detaching more expensive in AP, and negative values are not meaningful.
- **Values:** `0..255`; stored as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubClassIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Index of the class-specific record selected by `usItemClass`: an armour row, magazine row, weapon row, explosive row, and so on.
- **Values:** `0..65535`; stored as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubCursor`

- **Type:** `Lookup` (integer ID, `UINT8`)
- **Description:** Cursor lookup ID shown when the item is selected in the inventory. See [Cursor](../Lookup.md#cursor).
- **Values:** Lookup ID (`0..255`, stored as `UINT8`); see [Cursor](../Lookup.md#cursor).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/bSoundType`

- **Type:** `Integer` (`INT8`)
- **Description:** Sound family used when the item is handled or fired.
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Inventory, graphics and economy.** Graphics, weight, size, price and Bobby Ray stock.

### `ITEMLIST/ITEM/ubGraphicType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Graphic library for the inventory picture: `0` gun/weapon graphics, `1` big-item graphics (`P1ITEMS`), `2` interface graphics (`P2ITEMS`), `3` alternate interface graphics (`P3ITEMS`), `4` and above further item tile libraries (`P4ITEMS` onward).
- **Values:** `0..255`; stored as `UINT8`. `0` selects the gun graphic libraries, `1`..`3` select `P1ITEMS`..`P3ITEMS`, and `4` and above select `P4ITEMS` and later item tile libraries.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubGraphicNum`

- **Type:** `Integer` (`UINT16`)
- **Description:** Graphic index inside the library selected by `ubGraphicType`.
- **Values:** `0..65535`; stored as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubWeight`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item weight in the game's weight units; used by carry strength and backpack capacity. Two units are approximately one kilogram (roughly one unit per pound).
- **Numeric semantics:** Higher values mean a heavier item; negative values are not meaningful.
- **Values:** `0..65535`; stored as `UINT16`; the value is a whole number of weight units.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubPerPocket`

- **Type:** `Integer` (`UINT8`)
- **Description:** Old-inventory (OIV) pocket capacity: how many identical items fit into one pocket. Zero means no stacking. In NIV the field does not control pocket capacity (LBE/Pockets and ItemSize do), but the item-description code still reads it for the "small" property: under NIV every item otherwise reports as fitting a small pocket.
- **Numeric semantics:** Higher values mean a larger pocket capacity, and negative values are not meaningful.
- **Values:** `0..255`; stored as `UINT8`. The OIV slot capacity taken from it is clamped to `MAX_OBJECTS_PER_SLOT` (`255`).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ItemSize`

- **Type:** `Integer` (`UINT16`)
- **Description:** Inventory size class. It drives pocket packing and item-size limits. In NIV, an item whose size equals `OLD_INVENTORY_ITEM_NUMBER` (INI setting, default `99`) is excluded.
- **Values:** `0..65535`; stored as `UINT16`. `99` is only the shipped default of `OLD_INVENTORY_ITEM_NUMBER`, not a code limit.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ItemSizeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Adjustment added to the item's effective size when pocket capacity is checked (`iSize += ItemSizeBonus`).
- **Numeric semantics:** Signed size adjustment: negative values shrink the effective size.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usPrice`

- **Type:** `Integer` (`UINT16`)
- **Description:** Base purchase price. Bobby Ray prices and some trade calculations derive from it.
- **Numeric semantics:** Amount of money or cost; higher values mean a larger amount, and negative values are not meaningful.
- **Values:** `0..65535`; stored as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubCoolness`

- **Type:** `Integer` (`UINT8`)
- **Description:** Coolness/progress level at which the item becomes available in the campaign. The random-merc gear code skips items with `ubCoolness < 1`.
- **Numeric semantics:** Higher values mean a later/rarer coolness tier; negative values are outside the normal range.
- **Values:** `0..255`; stored as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/bReliability`

- **Type:** `Integer` (`INT8`)
- **Description:** Reliability modifier for the item.
- **Numeric semantics:** Higher values mean better reliability; lower values mean worse reliability, and negative values are outside the normal range.
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/bRepairEase`

- **Type:** `Integer` (`INT8`)
- **Description:** Repair-difficulty modifier for the item.
- **Numeric semantics:** Higher values make repair easier; lower values make it harder, and negative values are outside the normal range.
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Inseparable`

- **Type:** `Lookup` (integer ID, `UINT8`)
- **Description:** Attachment separability mode: `0` removable, `1` inseparable, `2` inseparable but replaceable.
- **Values:** `0`, `1` or `2`, stored as `UINT8`. See [Separability](../Lookup.md#separability).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BR_NewInventory`

- **Type:** `Integer` (`UINT8`)
- **Description:** Bobby Ray's new-inventory stock level or quantity for this item.
- **Numeric semantics:** Higher values mean a larger stock level; negative values are not meaningful.
- **Values:** `0..255`; stored in a `UINT8` array outside the item struct.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BR_UsedInventory`

- **Type:** `Integer` (`UINT8`)
- **Description:** Bobby Ray's used-inventory stock level or quantity.
- **Numeric semantics:** Higher values mean a larger stock level; negative values are not meaningful.
- **Values:** `0..255`; stored in a `UINT8` array outside the item struct.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BR_ROF`

- **Type:** `Integer` (`INT16`)
- **Description:** Bobby Ray's rate-of-fire/restock value for this item.
- **Numeric semantics:** Higher values mean a higher rate-of-fire/restock value; negative values are not meaningful.
- **Values:** `-32768..32767`; stored in an `INT16` array outside the item struct.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Food, drugs and action items.** Consumable classes and special-use flags.

### `ITEMLIST/ITEM/DrugType`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Drug/effect flag mask. The item struct defines it as a flag mask of the components used in a drug; the named bits are in [DrugType](../Lookup.md#drugtype).
- **Values:** `0` = none; `0..4294967295`; parsed with `strtoul` base `0` (hexadecimal `0x...` is accepted); stored as `UINT32`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/FoodType`

- **Type:** `Integer` (`UINT32`)
- **Description:** Food or drink class used by the food system; rows are defined in [Food](Food.md).
- **Values:** `0..4294967295`; parsed with `strtoul` base `0` (hexadecimal `0x...` is accepted); stored as `UINT32`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usActionItemFlag`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Bit mask of action-item flags. It controls special use actions such as NET, radio, or remote detonator.
- **Values:** `0` = none; `0..4294967295`; parsed with `strtoul` base `0` (hexadecimal `0x...` is accepted); stored as `UINT32`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/clothestype`

- **Type:** `Integer` (`UINT32`)
- **Description:** Clothing type index used by the clothes system; rows are defined in [Clothes](Clothes.md).
- **Values:** `0..4294967295`; parsed with `strtoul` base `0` (hexadecimal `0x...` is accepted); stored as `UINT32`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/spreadPattern`

- **Type:** `String`
- **Description:** Named shotgun pellet spread pattern from [SpreadPatterns](../SpreadPatterns.md). The loader accepts either a pattern name or a numeric index; an unknown name or an out-of-range index resolves to `0`, the default pattern.
- **Numeric semantics:** The stored value is the index of the pattern in SpreadPatterns.xml; `0` is the default pattern.
- **Values:** A pattern name from SpreadPatterns.xml or a numeric index into that table; empty, an unknown name or an out-of-range index resolves to `0` (default).
- **Default:** Empty; the loader resolves it to the default pattern `0`.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Alcohol`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Alcohol strength/effect value for drinks; feeds the drunk state and related penalties.
- **Numeric semantics:** Higher values mean a stronger alcohol effect; the loader forces negative input to `0`.
- **Values:** Decimal value; the loader forces negative input to `0`; stored as `FLOAT`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Weapon modifiers and overheating.** Optics, recoil, accuracy, heat and dirt.

### `ITEMLIST/ITEM/ScopeMagFactor`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH scope magnification factor.
- **Only used with:** NCTH.
- **Numeric semantics:** Higher values mean stronger magnification; `0` means no magnification, and negative values are not meaningful.
- **Values:** Decimal value; stored as `FLOAT`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ProjectionFactor`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH laser projection factor.
- **Only used with:** NCTH.
- **Numeric semantics:** Higher values mean stronger projection; `0` means no projection, and negative values are not meaningful.
- **Values:** Decimal value; stored as `FLOAT`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RecoilModifierX`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Horizontal recoil modifier added to the weapon's base recoil.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed horizontal modifier: positive values shift the kick right, negative values shift it left; a larger magnitude is harder to control.
- **Values:** Decimal value; stored as `FLOAT`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RecoilModifierY`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Vertical recoil modifier added to the weapon's base recoil.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed vertical modifier: positive values shift the kick up, negative values shift it down; a larger magnitude is harder to control.
- **Values:** Decimal value; stored as `FLOAT`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentRecoilModifier`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH percentage recoil modifier.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed percentage: the recoil is multiplied by `(100 + value)/100`, so negative values reduce recoil and positive values increase it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentAccuracyModifier`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH percentage accuracy modifier.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usOverheatingCooldownFactor`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Temperature units removed per cooldown tick (roughly every turn or five seconds), before attachment and global modifiers.
- **Numeric semantics:** Higher values cool the weapon faster; `0` means no cooling, and negative values are not meaningful.
- **Values:** Decimal value; stored as `FLOAT`.
- **Default:** `0` when omitted; the loader starts from zero. `100` is a data value, not a code default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/overheatTemperatureModificator`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Percentage modifier of the heat a shot generates (normally read from attachments).
- **Numeric semantics:** Signed modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** Decimal value; stored as `FLOAT`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/overheatCooldownModificator`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Percentage modifier of the cooldown amount; applies to guns and barrels.
- **Numeric semantics:** Signed modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** Decimal value; stored as `FLOAT`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/overheatJamThresholdModificator`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Percentage modifier of the gun's jam threshold.
- **Numeric semantics:** Signed modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** Decimal value; stored as `FLOAT`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/overheatDamageThresholdModificator`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Percentage modifier of the gun's damage threshold.
- **Numeric semantics:** Signed modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** Decimal value; stored as `FLOAT`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DirtIncreaseFactor`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Dirt a single shot adds to the gun (advanced repair/dirt system).
- **Numeric semantics:** Higher values accumulate dirt faster; `0` means no dirt gain, and negative values are not meaningful.
- **Values:** Decimal value; stored as `FLOAT`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Robot, random loot and shield links.** Robot damage, random-item links and riot-shield data.

### `ITEMLIST/ITEM/RobotDamageReduction`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Robot damage-reduction modifier read from robot attachments.
- **Numeric semantics:** Decimal modifier; the loader applies no clamp and the scale used by the robot code is not reconstructed here.
- **Values:** Decimal value; stored as `FLOAT`.
- **Default:** `0` when omitted; the loader zero-initialises the record, so `1.0` is a data convention, not a code default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DiscardedLauncherItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID left behind after a disposable launcher is fired.
- **Numeric semantics:** Item ID; `0` means nothing is left behind.
- **Values:** `0..65535`; stored as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/randomitem`

- **Type:** `Integer` (`UINT16`)
- **Description:** Random-item table ID used when this item is generated as random loot.
- **Numeric semantics:** Random-item table ID; `0` means none.
- **Values:** `0..65535`; stored as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/buddyitem`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the buddy/companion item linked to this item.
- **Numeric semantics:** Item ID; `0` means no linked item.
- **Values:** `0..65535`; stored as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usRiotShieldStrength`

- **Type:** `Integer` (`UINT16`)
- **Description:** Riot-shield protection strength.
- **Numeric semantics:** Higher values mean a stronger shield; the loader caps the value at `100`.
- **Values:** `0..100`; the loader caps the value at `100`; stored as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usRiotShieldGraphic`

- **Type:** `Integer` (`UINT16`)
- **Description:** Riot-shield graphic when the shield is deployed in tactical, taken from `Tilecache/riotshield.sti`.
- **Values:** `0..65535`; stored as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentNoiseReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Percentage reduction of the noise produced by the item.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Combat modifiers.** To-hit, range, damage, AP, burst and autofire modifiers.

### `ITEMLIST/ITEM/Bipod`

- **Type:** `Integer` (`INT16`)
- **Description:** Flat bipod modifier. The NCTH rework lists this value among the shooting variables it removed.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ToHitBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Flat bonus to hit probability.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BestLaserRange`

- **Type:** `Integer` (`INT16`)
- **Description:** Range used for the laser projection value; the item description scales it by the item-setting modifier and by the tile size.
- **Numeric semantics:** Higher values give a longer projection; the loader stores the value as a plain integer.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Flat range bonus.
- **Numeric semantics:** Signed number of range units; negative values are stored as given.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Percentage range bonus.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AimBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Flat aim bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/MinRangeForAimBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Minimum range at which `AimBonus` applies.
- **Numeric semantics:** Range threshold in the same units as weapon range; negative values are stored as given but are not meaningful for a minimum range.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentAPReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Percentage reduction of AP costs for actions with this item.
- **Numeric semantics:** Percentage: the AP cost is multiplied by `(100 - value)/100`, so `0` is no reduction and a negative value would increase the cost.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentStatusDrainReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Percentage reduction of status/condition drain.
- **Numeric semantics:** Percentage: status loss is multiplied by `(100 - value)/100`, so `0` is no reduction and a negative value would increase the drain.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BloodiedItem`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID of the bloodied variant produced when this item is bloodied.
- **Numeric semantics:** Item ID; `0` means no bloodied variant.
- **Values:** Item ID; the loader parses it through a `UINT16` cast and stores it in an `INT16` member (`0` = none).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/HearingRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Bonus to hearing range.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/VisionRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Bonus to general vision range.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/NightVisionRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Bonus to night vision range.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DayVisionRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Bonus to day vision range.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CaveVisionRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Bonus to cave vision range.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BrightLightVisionRangeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Bonus to vision range in bright light.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DamageBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Flat damage bonus added to the computed weapon damage (also from loaded ammo and attachments).
- **Numeric semantics:** Integer added to damage after the base damage is computed; negative values subtract.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/MeleeDamageBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Flat melee damage bonus added to the computed melee damage.
- **Numeric semantics:** Integer added to melee damage after the base damage is computed; negative values subtract.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/MagSizeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Magazine-size bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentBurstFireAPReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Percentage AP reduction for burst fire.
- **Numeric semantics:** Percentage: the burst AP cost is multiplied by `(100 - value)/100`.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentAutofireAPReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Percentage AP reduction for autofire.
- **Numeric semantics:** Percentage: the autofire AP cost is multiplied by `(100 - value)/100`.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentReadyTimeAPReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Percentage AP reduction for readying the item.
- **Numeric semantics:** Percentage: the ready (handling) difficulty is multiplied by `(100 - value)/100`.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentReloadTimeAPReduction`

- **Type:** `Integer` (`INT16`)
- **Description:** Percentage AP reduction for reloading.
- **Numeric semantics:** Percentage: the reload AP cost is multiplied by `(100 - value)/100`.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AutoFireToHitBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** To-hit bonus applied to autofire.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/APBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Bonus action points while the item is equipped.
- **Numeric semantics:** The loader rescales the value from the 100-AP scale to the configured AP system (`value * AP_MAXIMUM / 100`), so the stored value differs from the file value when `AP_MAXIMUM` is not `100`.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RateOfFireBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Rate-of-fire bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BurstSizeBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Extra shots in a burst.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BurstToHitBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** To-hit bonus applied to burst fire.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BulletSpeedBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Bullet-speed bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Camouflage, vision and body modifiers.** Camo, stealth, spotting, vision and backpack/fire modifiers.

### `ITEMLIST/ITEM/CamoBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** General camouflage bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/UrbanCamoBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Additional camouflage bonus in urban sectors.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DesertCamoBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Additional camouflage bonus in desert sectors.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/SnowCamoBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Additional camouflage bonus in snow sectors.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/StealthBonus`

- **Type:** `Integer` (`INT16`)
- **Description:** Stealth bonus; improves the chance to avoid detection.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usSpotting`

- **Type:** `Integer` (`INT16`)
- **Description:** Spotting modifier.
- **Numeric semantics:** Higher values improve spotting; `0` is neutral. The loader clamps the stored value to `0..100`, so negative input becomes `0` rather than a penalty.
- **Values:** `0..100`; the loader clamps the value to this range (`min(100, max(0, ...))`); stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/sBackpackWeightModifier`

- **Type:** `Integer` (`INT16`)
- **Description:** Carry-strength modifier applied when the item is in the backpack.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/sFireResistance`

- **Type:** `Integer` (`INT16`)
- **Description:** Fire-resistance modifier.
- **Numeric semantics:** Signed resistance: higher values mean more fire resistance; the loader caps the value at `100`.
- **Values:** `-32768..32767`; the loader caps the value at `100` but does not clamp the lower bound; stored as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PercentTunnelVision`

- **Type:** `Integer` (`UINT8`)
- **Description:** Percentage of tunnel vision imposed by the item.
- **Numeric semantics:** Percentage of tunnel vision; the loader stores `UINT8` and does not clamp, so values above `100` are stored as given.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Attachment system, utility and transport.** Attachment mode, kits, skills, transport progress and default attachments.

### `ITEMLIST/ITEM/AttachmentSystem`

- **Type:** `Lookup` (integer ID, `UINT8`)
- **Description:** Attachment system availability: `0` = both, `1` = old attachment system only, `2` = NAS only. An item is unavailable while the other system is active.
- **Values:** `0`, `1` or `2`, stored as `UINT8`. See [AttachmentSystem](../Lookup.md#attachmentsystem).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CrowbarModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Modifier applied when the item is used as a crowbar.
- **Numeric semantics:** Unsigned value: negative input wraps through the `UINT8` cast instead of acting as a penalty.
- **Values:** `0..255`; stored as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DisarmModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Modifier applied when disarming traps or bombs.
- **Numeric semantics:** Unsigned value: negative input wraps through the `UINT8` cast instead of acting as a penalty.
- **Values:** `0..255`; stored as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RepairModifier`

- **Type:** `Integer` (`INT8`)
- **Description:** Repair-kit effectiveness modifier.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usHackingModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Hacking modifier.
- **Numeric semantics:** Unsigned modifier; the loader keeps at most `100`.
- **Values:** `0..100` effectively: the loader casts to `UINT8` and keeps at most `100`, so values above `100` and negative values both store as `100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usBurialModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Burial-effectiveness modifier.
- **Numeric semantics:** Unsigned modifier; the loader keeps at most `100`.
- **Values:** `0..100` effectively: the loader casts to `UINT8` and keeps at most `100`, so values above `100` and negative values both store as `100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DamageChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that damage to the status also damages the repair threshold (advanced repair/dirt system).
- **Numeric semantics:** Higher values mean a greater chance; negative values are outside the valid chance range.
- **Values:** `0..255`; stored as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/FlashLightRange`

- **Type:** `Integer` (`UINT8`)
- **Description:** Flashlight range; any item with a value above `0` is treated as a flashlight.
- **Numeric semantics:** Higher values mean a longer range/radius, and negative values are not meaningful.
- **Values:** `0..255`; stored as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ItemChoiceTimeSetting`

- **Type:** `Integer` (`UINT8`)
- **Description:** Time setting that decides when the AI may pick this item for its choices.
- **Numeric semantics:** `0`, `1` and `2` are the only storable values; every other input is clamped into that range.
- **Values:** `0..2`; the loader clamps the value to this range; stored as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/SleepModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Breath-regeneration bonus the item provides while resting.
- **Numeric semantics:** Unsigned value: the loader stores `UINT8` and does not clamp it, so negative input wraps through the cast.
- **Values:** `0..255`; stored as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usPortionSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Portion size for food or drink items.
- **Numeric semantics:** Higher values mean a larger portion; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `100`; the loader sets this before parsing the record.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usAdministrationModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Administration-effectiveness modifier for strategic actions.
- **Numeric semantics:** Higher values mean a stronger administration effect; `0` means no modifier, and negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/LockPickModifier`

- **Type:** `Integer` (`INT8`)
- **Description:** Lockpicking bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/randomitemcoolnessmodificator`

- **Type:** `Integer` (`INT8`)
- **Description:** Coolness adjustment applied when this item is chosen as random loot.
- **Numeric semantics:** Signed coolness adjustment, clamped by the loader to `-20..20`.
- **Values:** `-20..20`; the loader clamps the value to this range; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotStrBonus`

- **Type:** `Integer` (`INT8`)
- **Description:** Robot strength bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotAgiBonus`

- **Type:** `Integer` (`INT8`)
- **Description:** Robot agility bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotDexBonus`

- **Type:** `Integer` (`INT8`)
- **Description:** Robot dexterity bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotTargetingSkillGrant`

- **Type:** `Integer` (`INT8`)
- **Description:** Targeting skill granted by the robot chassis or module.
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotUtilitySkillGrant`

- **Type:** `Integer` (`INT8`)
- **Description:** Utility skill granted by the robot chassis or module.
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotChassisSkillGrant`

- **Type:** `Integer` (`INT8`)
- **Description:** Chassis skill granted by the robot item.
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/TransportGroupMinProgress`

- **Type:** `Integer` (`INT8`)
- **Description:** Lower progress bound that allows a transport group to drop this item.
- **Numeric semantics:** Signed progress bound; the loader stores `INT8` and does not clamp it.
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/TransportGroupMaxProgress`

- **Type:** `Integer` (`INT8`)
- **Description:** Upper progress bound that allows a transport group to drop this item.
- **Numeric semantics:** Signed progress bound; the loader stores `INT8` and does not clamp it.
- **Values:** `-128..127`; stored as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DefaultAttachment`

- **Type:** `Integer` (`UINT16`)
- **Description:** Repeated element listing an item ID installed by default. Up to twenty occurrences (`MAX_DEFAULT_ATTACHMENTS`); empty means no default attachment.
- **Numeric semantics:** Each occurrence is one item ID; `0` means no attachment in that slot.
- **Values:** Item ID (`0..65535`), repeated; values fill the first free slot, so a `0` entry is skipped, and occurrences beyond twenty are ignored.
- **Default:** Empty; no default attachment.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Item flags.** Boolean item roles used across the game systems.

### `ITEMLIST/ITEM/Bloodbag`

- **Type:** `Boolean`
- **Description:** Marks the item as a blood bag usable for transfusions.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Manpad`

- **Type:** `Boolean`
- **Description:** Marks the item as a MANPADS launcher.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Beartrap`

- **Type:** `Boolean`
- **Description:** Marks the item as a bear trap.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Camera`

- **Type:** `Boolean`
- **Description:** Marks the item as a camera.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Waterdrum`

- **Type:** `Boolean`
- **Description:** Marks the item as a water drum.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BloodcatMeat`

- **Type:** `Boolean`
- **Description:** Marks the item as bloodcat meat.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CowMeat`

- **Type:** `Boolean`
- **Description:** Marks the item as cow meat.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Beltfed`

- **Type:** `Boolean`
- **Description:** Marks the weapon as belt-fed.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Ammobelt`

- **Type:** `Boolean`
- **Description:** Marks the item as an ammunition belt.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AmmobeltVest`

- **Type:** `Boolean`
- **Description:** Marks the item as an ammunition-belt vest.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CamoRemoval`

- **Type:** `Boolean`
- **Description:** Marks the item as a camouflage remover.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Cleaningkit`

- **Type:** `Boolean`
- **Description:** Marks the item as a weapon-cleaning kit.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AttentionItem`

- **Type:** `Boolean`
- **Description:** Marks the item as attention-grabbing; enemies may notice it.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Garotte`

- **Type:** `Boolean`
- **Description:** Marks the item as a garrote.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Covert`

- **Type:** `Boolean`
- **Description:** Marks the item as covert equipment.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Corpse`

- **Type:** `Boolean`
- **Description:** Marks the item as a corpse.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BloodcatSkin`

- **Type:** `Boolean`
- **Description:** Marks the item as a bloodcat skin.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/NoMetalDetection`

- **Type:** `Boolean`
- **Description:** The item cannot be detected by a metal detector.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/JumpGrenade`

- **Type:** `Boolean`
- **Description:** Marks the grenade as a jump grenade.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Handcuffs`

- **Type:** `Boolean`
- **Description:** Marks the item as handcuffs.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Taser`

- **Type:** `Boolean`
- **Description:** Marks the item as a taser.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ScubaBottle`

- **Type:** `Boolean`
- **Description:** Marks the item as a scuba bottle.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ScubaMask`

- **Type:** `Boolean`
- **Description:** Marks the item as a scuba mask.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ScubaFins`

- **Type:** `Boolean`
- **Description:** Marks the item as scuba fins.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/TripwireRoll`

- **Type:** `Boolean`
- **Description:** Marks the item as a roll of tripwire.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Radioset`

- **Type:** `Boolean`
- **Description:** Marks the item as a radio set.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/SignalShell`

- **Type:** `Boolean`
- **Description:** Marks the item as a signal shell.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Soda`

- **Type:** `Boolean`
- **Description:** Marks the item as a soda drink.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RoofcollapseItem`

- **Type:** `Boolean`
- **Description:** Marks the item as a roof-collapse item.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DiseaseprotectionFace`

- **Type:** `Boolean`
- **Description:** Protects the face from disease.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DiseaseprotectionHand`

- **Type:** `Boolean`
- **Description:** Protects the hands from disease.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/LBEexplosionproof`

- **Type:** `Boolean`
- **Description:** Marks the LBE item as explosion-proof.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/EmptyBloodbag`

- **Type:** `Boolean`
- **Description:** Marks the item as an empty blood bag.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/MedicalSplint`

- **Type:** `Boolean`
- **Description:** Marks the item as a medical splint.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Damageable`

- **Type:** `Boolean`
- **Description:** The item can take condition damage.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Repairable`

- **Type:** `Boolean`
- **Description:** The item can be repaired.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/WaterDamages`

- **Type:** `Boolean`
- **Description:** Water damages the item.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Metal`

- **Type:** `Boolean`
- **Description:** The item is made of metal.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Sinks`

- **Type:** `Boolean`
- **Description:** The item sinks in water.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ShowStatus`

- **Type:** `Boolean`
- **Description:** Show the item's condition or status in the interface.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/HiddenAddon`

- **Type:** `Boolean`
- **Description:** The add-on is hidden from normal attachment lists.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/TwoHanded`

- **Type:** `Boolean`
- **Description:** The item requires two hands.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/NotBuyable`

- **Type:** `Boolean`
- **Description:** The item is not sold by Bobby Ray.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Attachment`

- **Type:** `Boolean`
- **Description:** Marks the item as an attachment.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/HiddenAttachment`

- **Type:** `Boolean`
- **Description:** The attachment is hidden from normal attachment lists.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BigGunList`

- **Type:** `Boolean`
- **Description:** Include the item in the Tons of Guns mode.
- **Only used with:** Tons of Guns.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/NotInEditor`

- **Type:** `Boolean`
- **Description:** Hide the item from the editor's normal lists.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DefaultUndroppable`

- **Type:** `Boolean`
- **Description:** The item cannot be dropped by default.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Unaerodynamic`

- **Type:** `Boolean`
- **Description:** The item has poor aerodynamic behaviour.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Electronic`

- **Type:** `Boolean`
- **Description:** The item is electronic.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Cannon`

- **Type:** `Boolean`
- **Description:** Marks the weapon as a cannon.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RocketRifle`

- **Type:** `Boolean`
- **Description:** Marks the weapon as a rocket rifle.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/FingerPrintID`

- **Type:** `Boolean`
- **Description:** Marks the item as a fingerprint ID device.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/MetalDetector`

- **Type:** `Boolean`
- **Description:** Marks the item as a metal detector.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/GasMask`

- **Type:** `Boolean`
- **Description:** Marks the item as a gas mask.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/LockBomb`

- **Type:** `Boolean`
- **Description:** Marks the item as a lock bomb.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Flare`

- **Type:** `Boolean`
- **Description:** Marks the item as a flare.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/GrenadeLauncher`

- **Type:** `Boolean`
- **Description:** Marks the weapon as a grenade launcher.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Mortar`

- **Type:** `Boolean`
- **Description:** Marks the weapon as a mortar.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Duckbill`

- **Type:** `Boolean`
- **Description:** Marks the shotgun as having a duckbill choke.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/HideMuzzleFlash`

- **Type:** `Boolean`
- **Description:** Hides the muzzle flash when the weapon fires.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RocketLauncher`

- **Type:** `Boolean`
- **Description:** Marks the weapon as a rocket launcher.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/SingleShotRocketLauncher`

- **Type:** `Boolean`
- **Description:** Marks the launcher as single-shot.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BrassKnuckles`

- **Type:** `Boolean`
- **Description:** Marks the item as brass knuckles.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Crowbar`

- **Type:** `Boolean`
- **Description:** Marks the item as a crowbar.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/GLGrenade`

- **Type:** `Boolean`
- **Description:** Marks the item as a grenade-launcher grenade.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/FlakJacket`

- **Type:** `Boolean`
- **Description:** Marks the armour as a flak jacket.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/LeatherJacket`

- **Type:** `Boolean`
- **Description:** Marks the armour as a leather jacket.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Batteries`

- **Type:** `Boolean`
- **Description:** Marks the item as batteries.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/NeedsBatteries`

- **Type:** `Boolean`
- **Description:** The item needs batteries to operate.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/XRay`

- **Type:** `Boolean`
- **Description:** Marks the item as an X-ray device.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/WireCutters`

- **Type:** `Boolean`
- **Description:** Marks the item as wire cutters.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Toolkit`

- **Type:** `Boolean`
- **Description:** Marks the item as a toolkit.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/FirstAidKit`

- **Type:** `Boolean`
- **Description:** Marks the item as a first-aid kit.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/MedicalKit`

- **Type:** `Boolean`
- **Description:** Marks the item as a medical kit.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Canteen`

- **Type:** `Boolean`
- **Description:** Marks the item as a canteen.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Jar`

- **Type:** `Boolean`
- **Description:** Marks the item as a jar.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CanAndString`

- **Type:** `Boolean`
- **Description:** Marks the item as a can-and-string alarm.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Marbles`

- **Type:** `Boolean`
- **Description:** Marks the item as marbles.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Walkman`

- **Type:** `Boolean`
- **Description:** Marks the item as a Walkman.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RemoteTrigger`

- **Type:** `Boolean`
- **Description:** Marks the item as a remote trigger.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/RobotRemoteControl`

- **Type:** `Boolean`
- **Description:** Marks the item as a robot remote control.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CamouflageKit`

- **Type:** `Boolean`
- **Description:** Marks the item as a camouflage kit.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/LocksmithKit`

- **Type:** `Boolean`
- **Description:** Marks the item as a locksmith kit.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Mine`

- **Type:** `Boolean`
- **Description:** Marks the item as a mine.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AntitankMine`

- **Type:** `Boolean`
- **Description:** Marks the item as an antitank mine.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Hardware`

- **Type:** `Boolean`
- **Description:** Marks the item as hardware.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Medical`

- **Type:** `Boolean`
- **Description:** Marks the item as medical supplies.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/GasCan`

- **Type:** `Boolean`
- **Description:** Marks the item as a gas can.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ContainsLiquid`

- **Type:** `Boolean`
- **Description:** The item contains liquid.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Rock`

- **Type:** `Boolean`
- **Description:** Marks the item as a rock.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ThermalOptics`

- **Type:** `Boolean`
- **Description:** Marks the item as thermal optics.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/SciFi`

- **Type:** `Boolean`
- **Description:** Include the item in Sci-Fi mode.
- **Only used with:** Sci-Fi.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/NewInv`

- **Type:** `Boolean`
- **Description:** Marks the item as New Inventory (NIV) only. Old inventory (OIV) does not expose it.
- **Only used with:** NIV.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DiseaseSystemExclusive`

- **Type:** `Boolean`
- **Description:** The item belongs to the disease system.
- **Only used with:** Disease system.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Barrel`

- **Type:** `Boolean`
- **Description:** Marks the item as a barrel.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/TripWireActivation`

- **Type:** `Boolean`
- **Description:** Marks the item as a tripwire activator.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/TripWire`

- **Type:** `Boolean`
- **Description:** Marks the item as tripwire.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Directional`

- **Type:** `Boolean`
- **Description:** The item is directional.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BlockIronSight`

- **Type:** `Boolean`
- **Description:** The item blocks iron sights.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AllowClimbing`

- **Type:** `Boolean`
- **Description:** The item allows climbing.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Cigarette`

- **Type:** `Boolean`
- **Description:** Marks the item as a cigarette. The loader matches the lowercase element name `cigarette`.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ProvidesRobotCamo`

- **Type:** `Boolean`
- **Description:** Grants camouflage to a robot.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ProvidesRobotNightVision`

- **Type:** `Boolean`
- **Description:** Grants night vision to a robot.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ProvidesRobotLaserBonus`

- **Type:** `Boolean`
- **Description:** Grants a laser bonus to a robot.
- **Values:** `0` (no) or `1` (yes); the loader sets the flag when the parsed value is non-zero.
- **Default:** `0` when omitted.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Stance modifier groups.** The three containers are structural; each
contains the same eleven fields listed after the group blocks.

### `ITEMLIST/ITEM/STAND_MODIFIERS`

- **Type:** `Container` (11 integers)
- **Description:** Structural group for standing aim and handling modifiers. It has no value of its own; the child fields carry the modifiers.
- **Values:** One child element per modifier; an omitted child stays `0`.
- **Default:** Absent; an omitted child stays `0`.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/FlatBase`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH flat base modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed flat modifier: negative values reduce the base value.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value is neutral (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentBase`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH percent base modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier: negative values reduce the effect below the baseline.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value is neutral (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/FlatAim`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH flat aim modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed flat modifier: negative values reduce the aim value.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value is neutral (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentAim`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH percent aim modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier: negative values reduce the effect below the baseline.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value is neutral (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentCap`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH aim-cap modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of the aim cap; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value is neutral (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentHandling`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH handling modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of handling; negative values reduce the effect.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value is neutral (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentTargetTrackingSpeed`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH target-tracking speed modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of target-tracking speed; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value is neutral (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentDropCompensation`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH drop-compensation modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of drop compensation; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value is neutral (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentMaxCounterForce`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH maximum counter-force modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of the maximum counter force; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value is neutral (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentCounterForceAccuracy`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH counter-force accuracy modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of counter-force accuracy; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value is neutral (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/AimLevels`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH aim-level modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed modifier of the aim-level count for the stance.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value is neutral (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS`

- **Type:** `Container` (11 integers)
- **Description:** Structural group for crouching aim and handling modifiers. It has no value of its own; the child fields carry the modifiers.
- **Values:** One child element per modifier; an omitted child inherits the next defined stance value.
- **Default:** Absent; an omitted child inherits the next defined stance value.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/FlatBase`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH flat base modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed flat modifier: negative values reduce the base value.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentBase`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH percent base modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier: negative values reduce the effect below the baseline.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/FlatAim`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH flat aim modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed flat modifier: negative values reduce the aim value.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentAim`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH percent aim modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier: negative values reduce the effect below the baseline.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentCap`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH aim-cap modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of the aim cap; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentHandling`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH handling modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of handling; negative values reduce the effect.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentTargetTrackingSpeed`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH target-tracking speed modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of target-tracking speed; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentDropCompensation`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH drop-compensation modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of drop compensation; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentMaxCounterForce`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH maximum counter-force modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of the maximum counter force; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentCounterForceAccuracy`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH counter-force accuracy modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of counter-force accuracy; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/AimLevels`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH aim-level modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed modifier of the aim-level count for the stance.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS`

- **Type:** `Container` (11 integers)
- **Description:** Structural group for prone aim and handling modifiers. It has no value of its own; the child fields carry the modifiers.
- **Values:** One child element per modifier; an omitted child inherits the next defined stance value.
- **Default:** Absent; an omitted child inherits the next defined stance value.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Stance modifier fields (NCTH).** Each stance group has the same eleven fields; they are read by the New Chance to Hit system.
Empty means neutral; positive and negative percentages are allowed.

### `ITEMLIST/ITEM/PRONE_MODIFIERS/FlatBase`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH flat base modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed flat modifier: negative values reduce the base value.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentBase`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH percent base modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier: negative values reduce the effect below the baseline.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/FlatAim`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH flat aim modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed flat modifier: negative values reduce the aim value.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentAim`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH percent aim modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier: negative values reduce the effect below the baseline.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentCap`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH aim-cap modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of the aim cap; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentHandling`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH handling modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of handling; negative values reduce the effect.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentTargetTrackingSpeed`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH target-tracking speed modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of target-tracking speed; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentDropCompensation`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH drop-compensation modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of drop compensation; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentMaxCounterForce`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH maximum counter-force modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of the maximum counter force; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentCounterForceAccuracy`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH counter-force accuracy modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage modifier of counter-force accuracy; negative values reduce it.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/AimLevels`

- **Type:** `Integer` (`INT16`)
- **Description:** NCTH aim-level modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed modifier of the aim-level count for the stance.
- **Values:** `-32768..32767`; stored as `INT16`.
- **Default:** `0` when omitted; an omitted value inherits the next defined stance value (`-10000` is the internal unset marker).
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

## Relations

- `Items.uiIndex` is referenced by many tables: starting gear, random items,
  attachments, trader inventories, map extras and quest data.
- `Items.ubClassIndex` selects a row in the table chosen by `usItemClass`;
  see [Armours](Armours.md), [Magazines](Magazines.md) and
  [Launchables](Launchables.md).
- `Items.ubCursor` -> [Lookup.Cursor](../Lookup.md#cursor); `Items.Inseparable` -> [Lookup.Separability](../Lookup.md#separability).
- `Items.AttachmentClass` -> [Lookup.AttachmentClass](../Lookup.md#attachmentclass); `Items.nasAttachmentClass` ->
  [Lookup.NasAttachmentClass](../Lookup.md#nasattachmentclass); `Items.nasLayoutClass` -> [Lookup.NasLayoutClass](../Lookup.md#naslayoutclass).
- `Items.AttachmentPoint` and `Items.AvailableAttachmentPoint` ->
  [Lookup.AttachmentPoint](../Lookup.md#attachmentpoint); `Items.AttachmentSystem` -> [Lookup.AttachmentSystem](../Lookup.md#attachmentsystem).
- `Items.DrugType` -> [Lookup.DrugType](../Lookup.md#drugtype); `Items.FoodType` -> the [Food](Food.md) definitions.
- `Items.clothestype` -> the [Clothes](Clothes.md) definitions.
- `Items.spreadPattern` -> the [SpreadPatterns](../SpreadPatterns.md) definitions.
- `Items.DefaultAttachment`, `Items.DiscardedLauncherItem`,
  `Items.BloodiedItem`, `Items.buddyitem` and `Items.randomitem` are item or
  random-item IDs.
- `Items.usActionItemFlag` is a bit mask, not an item ID.

## Related formulas

Damage and armour formulas consume several item and ammo values. They are
described in [FORMULAS.md](../../FORMULAS.md):

- [Armour protection](../../FORMULAS.md#armour-protection)
- [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt)

## Localization

The translatable fields are `szItemName`, `szLongItemName`, `szItemDesc`,
`szBRName` and `szBRDesc`. They live in `Russian.Items.xml` and the other
language overlays, keyed by `uiIndex`. The base `Items.xml` remains the
schema and the fallback for untranslated fields. See
[Localization](../../Localization.md).

Field names, item IDs, class bits and numbers are never translated.

## Versions

- v5 is the primary source for the document.
- v4 uses the same set of leaf paths; text and a few values differ because
  of localization and content edits.
- `DefaultAttachment` is one documented path with repeated elements; the
  repetition is a structural property, not a separate field.

## Sources

- Item table data: v5 primary, v4 comparison.
- Item property and flag metadata from the editor database.
- Lookup tables for cursors, attachment classes, separability, drugs and
  attachment systems.
- Localization overlays for the five text fields.
- Game C++ source: the `Items.xml` loader and the item struct in the item
  types header.

This block was checked against the game's `Items.xml` loader and the item
struct. The loader establishes the type, storage width, parse call and any
clamp or inheritance for every field; named bit meanings still come from the
lookup tables. The uncertainties that remain are listed below.

## Open questions

- `DrugType`: the item struct defines the field as a flag mask; which bits map
  to which drug components still comes from the lookup.
- `FoodType`: the [Food](Food.md) table defines the rows; the item file stores
  only the class index.
- `bSoundType`: the full sound-family enum is not reconstructed.
- `RobotTargetingSkillGrant`, `RobotUtilitySkillGrant` and
  `RobotChassisSkillGrant`: the unit (skill points or skill level) is not
  verified.
- `TransportGroupMinProgress` and `TransportGroupMaxProgress`: the unit and scale
  are not confirmed; the document treats them as transport progress values.
- `BR_NewInventory`, `BR_UsedInventory` and `BR_ROF`: the loader keeps these in
  the Bobby Ray stock arrays, but the stock and restock formula is not
  reconstructed here.
- `RobotDamageReduction`: the loader only stores the decimal; whether the robot
  code treats it as a multiplier or a percentage is not reconstructed.
- `AttachmentClass`, `nasAttachmentClass` and `nasLayoutClass` are documented
  as bit masks; individual bit names live in the corresponding lookup pages.
