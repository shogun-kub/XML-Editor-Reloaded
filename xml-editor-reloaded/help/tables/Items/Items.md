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
status: reviewed
updated: 2026-11-07
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
  `CROUCH_MODIFIERS` and `PRONE_MODIFIERS`. Each group contains eleven
  integer modifiers and is absent when the item has no stance-specific
  values.
- `DefaultAttachment` is a repeated element. A record may contain up to ten
  occurrences; each occurrence is one item ID. The documentation describes
  the path once and notes the repetition.
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

`Type` names the editor field type. `Values` lists a code, metadata-validation or
field-type limit when one is known; otherwise the document does not assert a range.
Confidence `inferred` means the field exists in the table and its role is
reconstructed from its name, the legacy item-editor caption, and the project
metadata; it was not checked against a dedicated code review in this block.
Fields marked `source` are verified against the table and lookup data.

**Identity and descriptions.** IDs, names and player-facing text.

### `ITEMLIST/ITEM/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Primary item ID. Cross-table references and localization overlays use this value; it is unique inside one file.
- **Values:** Non-negative integer; stored as `UINT32`; unique inside the file.
- **Default:** None; required and unique inside the file.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szItemName`

- **Type:** `String`
- **Description:** Short item name used in lists and compact cells.
- **Values:** Free text; the editor limit is 80 characters.
- **Default:** Empty when omitted.
- **Source:** item table data; item localization overlay.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szLongItemName`

- **Type:** `String`
- **Description:** Full item name used in detail panels and tooltips.
- **Values:** Free text; the editor limit is 80 characters.
- **Default:** Empty when omitted.
- **Source:** item table data; item localization overlay.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szItemDesc`

- **Type:** `String`
- **Description:** Item description shown in the detail panel.
- **Values:** Free text; the editor limit is 400 characters.
- **Default:** Empty when omitted.
- **Source:** item table data; item localization overlay.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szBRName`

- **Type:** `String`
- **Description:** Bobby Ray's catalogue name.
- **Values:** Free text; the editor limit is 80 characters.
- **Default:** Empty when omitted.
- **Source:** item table data; item localization overlay.
- **Confidence:** `source`

### `ITEMLIST/ITEM/szBRDesc`

- **Type:** `String`
- **Description:** Bobby Ray's catalogue description.
- **Values:** Free text; the editor limit is 400 characters.
- **Default:** Empty when omitted.
- **Source:** item table data; item localization overlay.
- **Confidence:** `source`

**Classification and attachment points.** Item class, attachment systems and class dispatch.

### `ITEMLIST/ITEM/usItemClass`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Bit mask of item-class flags (`IC_*`). It selects the class-specific subtable reached through `ubClassIndex` and controls which item behaviours apply. See [Lookup.ItemClass](../Lookup.md#itemclass).
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** Bit mask; `0` = none. Bits are defined by [ItemClass](../Lookup.md#itemclass).
- **Default:** None; every item declares a class.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AttachmentClass`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Built-in attachment-class bit mask used by the legacy attachment system. Bits are defined by the [AttachmentClass](../Lookup.md#attachmentclass) lookup.
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** Bit mask; `0` = none; bits are defined by [AttachmentClass](../Lookup.md#attachmentclass).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/nasAttachmentClass`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Custom NAS attachment-class bit mask used with [AttachmentSystem](../Lookup.md#attachmentsystem) and NAS attachment points.
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** Bit mask; `0` = none; bits are defined by `NasAttachmentClass`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/nasLayoutClass`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** NAS layout-class bit mask. It tells the NAS layout which attachment slots the item exposes; bits are defined by [NasLayoutClass](../Lookup.md#naslayoutclass).
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** Bit mask; `0` = none; bits are defined by `NasLayoutClass`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AvailableAttachmentPoint`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Bit mask of attachment points the item can accept. Used for receivers and mounts; bits are defined by the [AttachmentPoint](../Lookup.md#attachmentpoint) lookup.
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** Bit mask; `0` = none; bits are defined by [AttachmentPoint](../Lookup.md#attachmentpoint).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AttachmentPoint`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Attachment point occupied by this item when installed. Zero means the item is not installed at a point.
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** Bit mask; `0` = none; bits are defined by [AttachmentPoint](../Lookup.md#attachmentpoint).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AttachToPointAPCost`

- **Type:** `Integer`
- **Description:** AP cost to attach or detach the item at its attachment point.
- **Numeric semantics:** Amount of money or cost; higher values mean a larger amount, and negative values are not meaningful.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ubClassIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Index of the class-specific record selected by `usItemClass`: an armour row, magazine row, weapon row, explosive row, and so on.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubCursor`

- **Type:** `Lookup` (integer ID, `UINT8`)
- **Description:** Cursor lookup ID shown when the item is selected in the inventory. See [Cursor](../Lookup.md#cursor).
- **Values:** Lookup ID; see [Cursor](../Lookup.md#cursor).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/bSoundType`

- **Type:** `Integer` (`INT8`)
- **Description:** Sound family used when the item is handled or fired.
- **Values:** `0` = default sound family; the full enum is the source constant set.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Inventory, graphics and economy.** Graphics, weight, size, price and Bobby Ray stock.

### `ITEMLIST/ITEM/ubGraphicType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Graphic family for the inventory picture: `0` none, `1` big item, `2` interface, `3` alternate interface.
- **Values:** `0` none, `1` big item, `2` interface, `3` alternate interface.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ubGraphicNum`

- **Type:** `Integer` (`UINT16`)
- **Description:** Graphic index inside the family selected by `ubGraphicType`.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ubWeight`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item weight in the game's weight units; used by carry strength and backpack capacity. Two units are approximately one kilogram (roughly one unit per pound).
- **Numeric semantics:** Higher values mean a heavier item; negative values are not meaningful.
- **Values:** `0..32000`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ubPerPocket`

- **Type:** `Integer` (`UINT8`)
- **Description:** Old-inventory (OIV) pocket capacity: how many identical items fit into one pocket. Zero means no stacking. In NIV the field does not control pocket capacity (LBE/Pockets and ItemSize do), but the item-description code still reads it for the "small" property: under NIV every item otherwise reports as fitting a small pocket.
- **Numeric semantics:** Higher values mean a larger pocket capacity, and negative values are not meaningful.
- **Values:** `0..8` metadata validation limit; stored as `UINT8`; the OIV slot capacity is clamped to `MAX_OBJECTS_PER_SLOT` (`255`).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ItemSize`

- **Type:** `Integer` (`UINT16`)
- **Description:** Inventory size class (`0..99`). It drives pocket packing and item-size limits. In NIV, an item whose size equals `OLD_INVENTORY_ITEM_NUMBER` (default `99`) is treated as an OIV-only item and is not available.
- **Values:** `0..99`; `99` means the item does not fit a normal pocket.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ItemSizeBonus`

- **Type:** `Integer`
- **Description:** Adjustment added to the item's effective size when pocket capacity is checked.
- **Values:** `-34..34`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usPrice`

- **Type:** `Integer` (`UINT16`)
- **Description:** Base purchase price. Bobby Ray prices and some trade calculations derive from it.
- **Numeric semantics:** Amount of money or cost; higher values mean a larger amount, and negative values are not meaningful.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/ubCoolness`

- **Type:** `Integer` (`UINT8`)
- **Description:** Coolness/progress level at which the item becomes available in the campaign.
- **Numeric semantics:** Higher values mean a later/rarer coolness tier; negative values are outside the normal range.
- **Values:** `0..10`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/bReliability`

- **Type:** `Integer` (`INT8`)
- **Description:** Reliability modifier for the item.
- **Numeric semantics:** Higher values mean better reliability; lower values mean worse reliability, and negative values are outside the normal range.
- **Values:** `-10..10`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/bRepairEase`

- **Type:** `Integer` (`INT8`)
- **Description:** Repair-difficulty modifier for the item.
- **Numeric semantics:** Higher values make repair easier; lower values make it harder, and negative values are outside the normal range.
- **Values:** `-10..10`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Inseparable`

- **Type:** `Lookup` (integer ID, `UINT8`)
- **Description:** Separability lookup ID that controls whether the item can be split, merged, or removed from a kit. See [Separability](../Lookup.md#separability).
- **Values:** `0` separable, `1` inseparable, `2` replaceable. See [Separability](../Lookup.md#separability).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BR_NewInventory`

- **Type:** `Integer`
- **Description:** Bobby Ray's new-inventory stock level or quantity for this item.
- **Numeric semantics:** Higher values mean a larger stock level; negative values are not meaningful.
- **Values:** `-100..250`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BR_UsedInventory`

- **Type:** `Integer`
- **Description:** Bobby Ray's used-inventory stock level or quantity.
- **Numeric semantics:** Higher values mean a larger stock level; negative values are not meaningful.
- **Values:** `-100..250`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BR_ROF`

- **Type:** `Integer`
- **Description:** Bobby Ray's rate-of-fire/restock value for this item.
- **Numeric semantics:** Higher values mean a higher rate-of-fire/restock value; negative values are not meaningful.
- **Values:** `0..10000`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Food, drugs and action items.** Consumable classes and special-use flags.

### `ITEMLIST/ITEM/DrugType`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Drug/effect class. Editor metadata marks it as a flag mask, while the lookup is also presented as a sequence; see [DrugType](../Lookup.md#drugtype) and Open questions.
- **Values:** `0` = none; values are lookup IDs in [DrugType](../Lookup.md#drugtype).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/FoodType`

- **Type:** `Integer`
- **Description:** Food or drink class used by the food system; rows are defined in [Food](Food.md).
- **Values:** `0` = none; values index the [Food](Food.md) rows. The v5 item set does not embed a separate food lookup.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usActionItemFlag`

- **Type:** `Flags` (bit mask, `UINT32`)
- **Description:** Bit mask of action-item flags. It controls special use actions such as NET, radio, or remote detonator.
- **Values:** Bit mask; `0` = none. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/clothestype`

- **Type:** `Integer`
- **Description:** Clothing type index used by the clothes system; rows are defined in [Clothes](Clothes.md).
- **Values:** `0` = none; index into the [Clothes](Clothes.md) rows.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/spreadPattern`

- **Type:** `String`
- **Description:** Named shotgun spread pattern from [SpreadPatterns](../SpreadPatterns.md): `Full Choke`, `Imp Mod Choke`, `Rifled Choke`, `Duckbill`, `Sawed off`, or `Caws`. Empty means the default.
- **Values:** `Full Choke`, `Imp Mod Choke`, `Rifled Choke`, `Duckbill`, `Sawed off`, `Caws`; empty = default.
- **Default:** Empty; default spread.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Alcohol`

- **Type:** `Decimal`
- **Description:** Alcohol strength/effect value for drinks; feeds the drunk state and related penalties.
- **Numeric semantics:** Higher values mean a stronger alcohol effect; negative values are not meaningful.
- **Values:** Decimal value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Weapon modifiers and overheating.** Optics, recoil, accuracy, heat and dirt.

### `ITEMLIST/ITEM/ScopeMagFactor`

- **Type:** `Decimal`
- **Description:** NCTH scope magnification factor.
- **Only used with:** NCTH.
- **Numeric semantics:** Higher values mean stronger magnification; `0` means no magnification, and negative values are not meaningful.
- **Values:** `0..20`; code validation limit (decimal).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ProjectionFactor`

- **Type:** `Decimal`
- **Description:** NCTH laser projection factor.
- **Only used with:** NCTH.
- **Numeric semantics:** Higher values mean stronger projection; `0` means no projection, and negative values are not meaningful.
- **Values:** `0..20`; code validation limit (decimal).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RecoilModifierX`

- **Type:** `Decimal`
- **Description:** Horizontal recoil modifier added to the weapon's base recoil.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed horizontal modifier: positive values shift the kick right, negative values shift it left; a larger magnitude is harder to control.
- **Values:** `-20..20`; code validation limit (decimal).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RecoilModifierY`

- **Type:** `Decimal`
- **Description:** Vertical recoil modifier added to the weapon's base recoil.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed vertical modifier: positive values shift the kick up, negative values shift it down; a larger magnitude is harder to control.
- **Values:** `-20..20`; code validation limit (decimal).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentRecoilModifier`

- **Type:** `Integer`
- **Description:** NCTH percentage recoil modifier.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentAccuracyModifier`

- **Type:** `Integer`
- **Description:** NCTH percentage accuracy modifier.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usOverheatingCooldownFactor`

- **Type:** `Decimal`
- **Description:** Temperature units removed per cooldown tick (roughly every turn or five seconds), before attachment and global modifiers.
- **Numeric semantics:** Higher values cool the weapon faster; `0` means no cooling, and negative values are not meaningful.
- **Values:** Decimal amount; `100` is the common value in shipped data.
- **Default:** `0` when omitted; the loader starts from zero. `100` is a data value, not a code default.
- **Source:** game code (cooldown calculation); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/overheatTemperatureModificator`

- **Type:** `Decimal`
- **Description:** Adjustment to the weapon's overheat temperature threshold.
- **Numeric semantics:** Signed modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** `-1..10`; code validation limit (decimal).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/overheatCooldownModificator`

- **Type:** `Decimal`
- **Description:** Adjustment to the overheat cooldown rate.
- **Numeric semantics:** Signed modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** `-1..10`; code validation limit (decimal).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/overheatJamThresholdModificator`

- **Type:** `Decimal`
- **Description:** Adjustment to the heat level at which the weapon starts jamming.
- **Numeric semantics:** Signed modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** `-1..10`; code validation limit (decimal).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/overheatDamageThresholdModificator`

- **Type:** `Decimal`
- **Description:** Adjustment to the heat level at which the weapon takes damage.
- **Numeric semantics:** Signed modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** `-1..10`; code validation limit (decimal).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DirtIncreaseFactor`

- **Type:** `Decimal`
- **Description:** How quickly the item accumulates dirt.
- **Numeric semantics:** Higher values accumulate dirt faster; `0` means no dirt gain, and negative values are not meaningful.
- **Values:** Decimal value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Robot, random loot and shield links.** Robot damage, random-item links and riot-shield data.

### `ITEMLIST/ITEM/RobotDamageReduction`

- **Type:** `Decimal`
- **Description:** Damage-reduction multiplier for robot items; `1.0` means no reduction.
- **Numeric semantics:** Higher values mean a stronger reduction/resistance; negative values reduce that protection and are not meaningful unless the field explicitly defines a penalty.
- **Values:** Decimal multiplier; `1.0` = no reduction.
- **Default:** `1`.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DiscardedLauncherItem`

- **Type:** `Integer`
- **Description:** Item ID left behind after a disposable launcher is fired.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/randomitem`

- **Type:** `Integer`
- **Description:** Random-item table ID used when this item is generated as random loot.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/buddyitem`

- **Type:** `Integer`
- **Description:** Item ID of the buddy/companion item linked to this item.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usRiotShieldStrength`

- **Type:** `Integer` (`UINT16`)
- **Description:** Riot-shield protection strength.
- **Numeric semantics:** Higher values mean a larger value/amount, and negative values are not meaningful.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usRiotShieldGraphic`

- **Type:** `Integer` (`UINT16`)
- **Description:** Graphic index for the riot-shield overlay.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentNoiseReduction`

- **Type:** `Integer`
- **Description:** Percentage reduction of the noise produced by the item.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Combat modifiers.** To-hit, range, damage, AP, burst and autofire modifiers.

### `ITEMLIST/ITEM/Bipod`

- **Type:** `Integer`
- **Description:** Bipod bonus value; adds stability when the weapon is deployed.
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ToHitBonus`

- **Type:** `Integer`
- **Description:** Flat bonus to hit probability.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BestLaserRange`

- **Type:** `Integer`
- **Description:** Range at which the laser sight gives its best accuracy bonus.
- **Numeric semantics:** Higher values mean a longer range/radius, and negative values are not meaningful.
- **Values:** `0..10000`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RangeBonus`

- **Type:** `Integer`
- **Description:** Flat range bonus in tiles.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..10000`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentRangeBonus`

- **Type:** `Integer`
- **Description:** Percentage range bonus.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AimBonus`

- **Type:** `Integer`
- **Description:** Flat aim bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/MinRangeForAimBonus`

- **Type:** `Integer`
- **Description:** Minimum range at which `AimBonus` applies.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `0..1000`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentAPReduction`

- **Type:** `Integer`
- **Description:** Percentage reduction of AP costs for actions with this item.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentStatusDrainReduction`

- **Type:** `Integer`
- **Description:** Percentage reduction of status/condition drain.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BloodiedItem`

- **Type:** `Integer`
- **Description:** Item ID of the bloodied variant produced when this item is bloodied.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/HearingRangeBonus`

- **Type:** `Integer`
- **Description:** Bonus to hearing range.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/VisionRangeBonus`

- **Type:** `Integer`
- **Description:** Bonus to general vision range.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-1000..1000`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/NightVisionRangeBonus`

- **Type:** `Integer`
- **Description:** Bonus to night vision range.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-1000..1000`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DayVisionRangeBonus`

- **Type:** `Integer`
- **Description:** Bonus to day vision range.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-1000..1000`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CaveVisionRangeBonus`

- **Type:** `Integer`
- **Description:** Bonus to cave vision range.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-1000..1000`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BrightLightVisionRangeBonus`

- **Type:** `Integer`
- **Description:** Bonus to vision range in bright light.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-1000..1000`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DamageBonus`

- **Type:** `Integer`
- **Description:** Flat damage bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/MeleeDamageBonus`

- **Type:** `Integer`
- **Description:** Flat melee damage bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/MagSizeBonus`

- **Type:** `Integer`
- **Description:** Magazine-size bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-250..1000`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentBurstFireAPReduction`

- **Type:** `Integer`
- **Description:** Percentage AP reduction for burst fire.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentAutofireAPReduction`

- **Type:** `Integer`
- **Description:** Percentage AP reduction for autofire.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentReadyTimeAPReduction`

- **Type:** `Integer`
- **Description:** Percentage AP reduction for readying the item.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentReloadTimeAPReduction`

- **Type:** `Integer`
- **Description:** Percentage AP reduction for reloading.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AutoFireToHitBonus`

- **Type:** `Integer`
- **Description:** To-hit bonus applied to autofire.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/APBonus`

- **Type:** `Integer`
- **Description:** Bonus action points while the item is equipped.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RateOfFireBonus`

- **Type:** `Integer`
- **Description:** Rate-of-fire bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BurstSizeBonus`

- **Type:** `Integer`
- **Description:** Extra shots in a burst.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BurstToHitBonus`

- **Type:** `Integer`
- **Description:** To-hit bonus applied to burst fire.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BulletSpeedBonus`

- **Type:** `Integer`
- **Description:** Bullet-speed bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Camouflage, vision and body modifiers.** Camo, stealth, spotting, vision and backpack/fire modifiers.

### `ITEMLIST/ITEM/CamoBonus`

- **Type:** `Integer`
- **Description:** General camouflage bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/UrbanCamoBonus`

- **Type:** `Integer`
- **Description:** Additional camouflage bonus in urban sectors.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DesertCamoBonus`

- **Type:** `Integer`
- **Description:** Additional camouflage bonus in desert sectors.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/SnowCamoBonus`

- **Type:** `Integer`
- **Description:** Additional camouflage bonus in snow sectors.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/StealthBonus`

- **Type:** `Integer`
- **Description:** Stealth bonus; improves the chance to avoid detection.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usSpotting`

- **Type:** `Integer` (`INT16`)
- **Description:** Spotting modifier.
- **Numeric semantics:** Higher values improve spotting; `0` is neutral. The loader clamps the stored value to `0..100`, so negative input becomes `0` rather than a penalty.
- **Values:** `0..100`; the loader clamps to this range.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/sBackpackWeightModifier`

- **Type:** `Integer`
- **Description:** Carry-strength modifier applied when the item is in the backpack.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-10000..10000`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/sFireResistance`

- **Type:** `Integer`
- **Description:** Fire-resistance modifier.
- **Numeric semantics:** Higher values mean a stronger reduction/resistance; negative values reduce that protection and are not meaningful unless the field explicitly defines a penalty.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentTunnelVision`

- **Type:** `Integer` (`UINT8`)
- **Description:** Percentage of tunnel vision imposed by the item.
- **Numeric semantics:** Higher values mean a stronger tunnel-vision effect; `0` means no effect, and negative values are not meaningful.
- **Values:** `0..100`; a percentage. The loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

**Attachment system, utility and transport.** Attachment mode, kits, skills, transport progress and default attachments.

### `ITEMLIST/ITEM/AttachmentSystem`

- **Type:** `Lookup` (integer ID, `UINT8`)
- **Description:** Attachment system selector: `0` legacy, `1` NAS, `2` NAS with layout classes. See [AttachmentSystem](../Lookup.md#attachmentsystem).
- **Values:** `0` legacy, `1` NAS, `2` NAS with layout classes. See [AttachmentSystem](../Lookup.md#attachmentsystem).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CrowbarModifier`

- **Type:** `Integer`
- **Description:** Bonus applied when the item is used as a crowbar.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DisarmModifier`

- **Type:** `Integer`
- **Description:** Bonus applied when disarming traps or bombs.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RepairModifier`

- **Type:** `Integer`
- **Description:** Repair-kit effectiveness modifier.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usHackingModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Hacking bonus.
- **Numeric semantics:** Higher values mean a stronger bonus; `0` means no bonus. The loader clamps the stored value to `0..100`, so negative input is not meaningful.
- **Values:** `0..100`; the loader clamps to this range.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usBurialModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Burial modifier.
- **Numeric semantics:** Higher values mean a stronger burial effect; `0` means no modifier. The loader clamps the stored value to `0..100`, so negative input is not meaningful.
- **Values:** `0..100`; the loader clamps to this range.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/DamageChance`

- **Type:** `Integer`
- **Description:** Chance that the item takes dirt or damage when used.
- **Numeric semantics:** Higher values mean a greater chance; negative values are outside the valid chance range.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/FlashLightRange`

- **Type:** `Integer`
- **Description:** Flashlight range in tiles.
- **Numeric semantics:** Higher values mean a longer range/radius, and negative values are not meaningful.
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ItemChoiceTimeSetting`

- **Type:** `Integer`
- **Description:** Time setting used when the item is selected in an item-choice list.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/SleepModifier`

- **Type:** `Integer`
- **Description:** Sleep modifier for food, drink, or resting items.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usPortionSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Portion size for food or drink items.
- **Numeric semantics:** Higher values mean a larger portion; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `100` when the element is omitted; the loader sets this before parsing.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/usAdministrationModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Administration modifier for strategic actions.
- **Numeric semantics:** Higher values mean a stronger administration effect; `0` means no modifier, and negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (item loader); item table data.
- **Confidence:** `source`

### `ITEMLIST/ITEM/LockPickModifier`

- **Type:** `Integer`
- **Description:** Lockpicking bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** `-100..100`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/randomitemcoolnessmodificator`

- **Type:** `Integer`
- **Description:** Coolness adjustment applied when this item is chosen as random loot.
- **Values:** `-20..20`; code validation limit (integer).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotStrBonus`

- **Type:** `Integer`
- **Description:** Robot strength bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotAgiBonus`

- **Type:** `Integer`
- **Description:** Robot agility bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotDexBonus`

- **Type:** `Integer`
- **Description:** Robot dexterity bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotTargetingSkillGrant`

- **Type:** `Integer`
- **Description:** Targeting skill granted by the robot chassis or module.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotUtilitySkillGrant`

- **Type:** `Integer`
- **Description:** Utility skill granted by the robot chassis or module.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotChassisSkillGrant`

- **Type:** `Integer`
- **Description:** Chassis skill granted by the robot item.
- **Values:** Integer value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/TransportGroupMinProgress`

- **Type:** `Integer`
- **Description:** Minimum transport progress at which this item or group can appear.
- **Numeric semantics:** Higher values mean a larger value/amount, and negative values are not meaningful.
- **Values:** Integer progress value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/TransportGroupMaxProgress`

- **Type:** `Integer`
- **Description:** Maximum transport progress at which this item or group can appear.
- **Numeric semantics:** Higher values mean a larger value/amount, and negative values are not meaningful.
- **Values:** Integer progress value; no narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DefaultAttachment`

- **Type:** `Integer` (`UINT32`)
- **Description:** Repeated element listing an item ID installed by default. Up to ten occurrences per item; empty means no default attachment.
- **Values:** Item ID; repeated up to ten times; empty = none.
- **Default:** Empty; no default attachment.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Item flags.** Boolean item roles used across the game systems.

### `ITEMLIST/ITEM/Bloodbag`

- **Type:** `Boolean`
- **Description:** Marks the item as a blood bag usable for transfusions.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Manpad`

- **Type:** `Boolean`
- **Description:** Marks the item as a MANPADS launcher.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Beartrap`

- **Type:** `Boolean`
- **Description:** Marks the item as a bear trap.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Camera`

- **Type:** `Boolean`
- **Description:** Marks the item as a camera.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Waterdrum`

- **Type:** `Boolean`
- **Description:** Marks the item as a water drum.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BloodcatMeat`

- **Type:** `Boolean`
- **Description:** Marks the item as bloodcat meat.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CowMeat`

- **Type:** `Boolean`
- **Description:** Marks the item as cow meat.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Beltfed`

- **Type:** `Boolean`
- **Description:** Marks the weapon as belt-fed.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Ammobelt`

- **Type:** `Boolean`
- **Description:** Marks the item as an ammunition belt.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AmmobeltVest`

- **Type:** `Boolean`
- **Description:** Marks the item as an ammunition-belt vest.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CamoRemoval`

- **Type:** `Boolean`
- **Description:** Marks the item as a camouflage remover.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Cleaningkit`

- **Type:** `Boolean`
- **Description:** Marks the item as a weapon-cleaning kit.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AttentionItem`

- **Type:** `Boolean`
- **Description:** Marks the item as attention-grabbing; enemies may notice it.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Garotte`

- **Type:** `Boolean`
- **Description:** Marks the item as a garrote.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Covert`

- **Type:** `Boolean`
- **Description:** Marks the item as covert equipment.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Corpse`

- **Type:** `Boolean`
- **Description:** Marks the item as a corpse.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BloodcatSkin`

- **Type:** `Boolean`
- **Description:** Marks the item as a bloodcat skin.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/NoMetalDetection`

- **Type:** `Boolean`
- **Description:** The item cannot be detected by a metal detector.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/JumpGrenade`

- **Type:** `Boolean`
- **Description:** Marks the grenade as a jump grenade.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Handcuffs`

- **Type:** `Boolean`
- **Description:** Marks the item as handcuffs.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Taser`

- **Type:** `Boolean`
- **Description:** Marks the item as a taser.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ScubaBottle`

- **Type:** `Boolean`
- **Description:** Marks the item as a scuba bottle.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ScubaMask`

- **Type:** `Boolean`
- **Description:** Marks the item as a scuba mask.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ScubaFins`

- **Type:** `Boolean`
- **Description:** Marks the item as scuba fins.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/TripwireRoll`

- **Type:** `Boolean`
- **Description:** Marks the item as a roll of tripwire.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Radioset`

- **Type:** `Boolean`
- **Description:** Marks the item as a radio set.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/SignalShell`

- **Type:** `Boolean`
- **Description:** Marks the item as a signal shell.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Soda`

- **Type:** `Boolean`
- **Description:** Marks the item as a soda drink.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RoofcollapseItem`

- **Type:** `Boolean`
- **Description:** Marks the item as a roof-collapse item.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DiseaseprotectionFace`

- **Type:** `Boolean`
- **Description:** Protects the face from disease.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DiseaseprotectionHand`

- **Type:** `Boolean`
- **Description:** Protects the hands from disease.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/LBEexplosionproof`

- **Type:** `Boolean`
- **Description:** Marks the LBE item as explosion-proof.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/EmptyBloodbag`

- **Type:** `Boolean`
- **Description:** Marks the item as an empty blood bag.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/MedicalSplint`

- **Type:** `Boolean`
- **Description:** Marks the item as a medical splint.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Damageable`

- **Type:** `Boolean`
- **Description:** The item can take condition damage.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Repairable`

- **Type:** `Boolean`
- **Description:** The item can be repaired.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/WaterDamages`

- **Type:** `Boolean`
- **Description:** Water damages the item.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Metal`

- **Type:** `Boolean`
- **Description:** The item is made of metal.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Sinks`

- **Type:** `Boolean`
- **Description:** The item sinks in water.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ShowStatus`

- **Type:** `Boolean`
- **Description:** Show the item's condition or status in the interface.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/HiddenAddon`

- **Type:** `Boolean`
- **Description:** The add-on is hidden from normal attachment lists.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/TwoHanded`

- **Type:** `Boolean`
- **Description:** The item requires two hands.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/NotBuyable`

- **Type:** `Boolean`
- **Description:** The item is not sold by Bobby Ray.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Attachment`

- **Type:** `Boolean`
- **Description:** Marks the item as an attachment.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/HiddenAttachment`

- **Type:** `Boolean`
- **Description:** The attachment is hidden from normal attachment lists.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BigGunList`

- **Type:** `Boolean`
- **Description:** Include the item in the Tons of Guns mode.
- **Only used with:** Tons of Guns.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/NotInEditor`

- **Type:** `Boolean`
- **Description:** Hide the item from the editor's normal lists.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DefaultUndroppable`

- **Type:** `Boolean`
- **Description:** The item cannot be dropped by default.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Unaerodynamic`

- **Type:** `Boolean`
- **Description:** The item has poor aerodynamic behaviour.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Electronic`

- **Type:** `Boolean`
- **Description:** The item is electronic.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Cannon`

- **Type:** `Boolean`
- **Description:** Marks the weapon as a cannon.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RocketRifle`

- **Type:** `Boolean`
- **Description:** Marks the weapon as a rocket rifle.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/FingerPrintID`

- **Type:** `Boolean`
- **Description:** Marks the item as a fingerprint ID device.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/MetalDetector`

- **Type:** `Boolean`
- **Description:** Marks the item as a metal detector.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/GasMask`

- **Type:** `Boolean`
- **Description:** Marks the item as a gas mask.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/LockBomb`

- **Type:** `Boolean`
- **Description:** Marks the item as a lock bomb.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Flare`

- **Type:** `Boolean`
- **Description:** Marks the item as a flare.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/GrenadeLauncher`

- **Type:** `Boolean`
- **Description:** Marks the weapon as a grenade launcher.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Mortar`

- **Type:** `Boolean`
- **Description:** Marks the weapon as a mortar.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Duckbill`

- **Type:** `Boolean`
- **Description:** Marks the shotgun as having a duckbill choke.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/HideMuzzleFlash`

- **Type:** `Boolean`
- **Description:** Hides the muzzle flash when the weapon fires.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RocketLauncher`

- **Type:** `Boolean`
- **Description:** Marks the weapon as a rocket launcher.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/SingleShotRocketLauncher`

- **Type:** `Boolean`
- **Description:** Marks the launcher as single-shot.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BrassKnuckles`

- **Type:** `Boolean`
- **Description:** Marks the item as brass knuckles.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Crowbar`

- **Type:** `Boolean`
- **Description:** Marks the item as a crowbar.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/GLGrenade`

- **Type:** `Boolean`
- **Description:** Marks the item as a grenade-launcher grenade.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/FlakJacket`

- **Type:** `Boolean`
- **Description:** Marks the armour as a flak jacket.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/LeatherJacket`

- **Type:** `Boolean`
- **Description:** Marks the armour as a leather jacket.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Batteries`

- **Type:** `Boolean`
- **Description:** Marks the item as batteries.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/NeedsBatteries`

- **Type:** `Boolean`
- **Description:** The item needs batteries to operate.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/XRay`

- **Type:** `Boolean`
- **Description:** Marks the item as an X-ray device.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/WireCutters`

- **Type:** `Boolean`
- **Description:** Marks the item as wire cutters.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Toolkit`

- **Type:** `Boolean`
- **Description:** Marks the item as a toolkit.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/FirstAidKit`

- **Type:** `Boolean`
- **Description:** Marks the item as a first-aid kit.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/MedicalKit`

- **Type:** `Boolean`
- **Description:** Marks the item as a medical kit.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Canteen`

- **Type:** `Boolean`
- **Description:** Marks the item as a canteen.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Jar`

- **Type:** `Boolean`
- **Description:** Marks the item as a jar.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CanAndString`

- **Type:** `Boolean`
- **Description:** Marks the item as a can-and-string alarm.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Marbles`

- **Type:** `Boolean`
- **Description:** Marks the item as marbles.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Walkman`

- **Type:** `Boolean`
- **Description:** Marks the item as a Walkman.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RemoteTrigger`

- **Type:** `Boolean`
- **Description:** Marks the item as a remote trigger.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotRemoteControl`

- **Type:** `Boolean`
- **Description:** Marks the item as a robot remote control.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CamouflageKit`

- **Type:** `Boolean`
- **Description:** Marks the item as a camouflage kit.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/LocksmithKit`

- **Type:** `Boolean`
- **Description:** Marks the item as a locksmith kit.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Mine`

- **Type:** `Boolean`
- **Description:** Marks the item as a mine.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AntitankMine`

- **Type:** `Boolean`
- **Description:** Marks the item as an antitank mine.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Hardware`

- **Type:** `Boolean`
- **Description:** Marks the item as hardware.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Medical`

- **Type:** `Boolean`
- **Description:** Marks the item as medical supplies.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/GasCan`

- **Type:** `Boolean`
- **Description:** Marks the item as a gas can.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ContainsLiquid`

- **Type:** `Boolean`
- **Description:** The item contains liquid.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Rock`

- **Type:** `Boolean`
- **Description:** Marks the item as a rock.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ThermalOptics`

- **Type:** `Boolean`
- **Description:** Marks the item as thermal optics.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/SciFi`

- **Type:** `Boolean`
- **Description:** Include the item in Sci-Fi mode.
- **Only used with:** Sci-Fi.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/NewInv`

- **Type:** `Boolean`
- **Description:** Marks the item as New Inventory (NIV) only. Old inventory (OIV) does not expose it.
- **Only used with:** NIV.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DiseaseSystemExclusive`

- **Type:** `Boolean`
- **Description:** The item belongs to the disease system.
- **Only used with:** Disease system.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Barrel`

- **Type:** `Boolean`
- **Description:** Marks the item as a barrel.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/TripWireActivation`

- **Type:** `Boolean`
- **Description:** Marks the item as a tripwire activator.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/TripWire`

- **Type:** `Boolean`
- **Description:** Marks the item as tripwire.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Directional`

- **Type:** `Boolean`
- **Description:** The item is directional.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BlockIronSight`

- **Type:** `Boolean`
- **Description:** The item blocks iron sights.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AllowClimbing`

- **Type:** `Boolean`
- **Description:** The item allows climbing.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Cigarette`

- **Type:** `Boolean`
- **Description:** Marks the item as a cigarette.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ProvidesRobotCamo`

- **Type:** `Boolean`
- **Description:** Grants camouflage to a robot.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ProvidesRobotNightVision`

- **Type:** `Boolean`
- **Description:** Grants night vision to a robot.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ProvidesRobotLaserBonus`

- **Type:** `Boolean`
- **Description:** Grants a laser bonus to a robot.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

**Stance modifier groups.** The three containers are structural; each
contains the same eleven fields listed after the group blocks.

### `ITEMLIST/ITEM/STAND_MODIFIERS`

- **Type:** `Container` (11 integers)
- **Description:** Structural group for standing aim and handling modifiers. It has no value of its own; the child fields carry the modifiers.
- **Values:** One child element per modifier; an absent group means all children are neutral.
- **Default:** Absent; children default to `0` when omitted.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/STAND_MODIFIERS/FlatBase`

- **Type:** `Integer`
- **Description:** NCTH flat base modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed stance modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentBase`

- **Type:** `Integer`
- **Description:** NCTH percent base modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/FlatAim`

- **Type:** `Integer`
- **Description:** NCTH flat aim modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed stance modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentAim`

- **Type:** `Integer`
- **Description:** NCTH percent aim modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentCap`

- **Type:** `Integer`
- **Description:** NCTH aim-cap modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentHandling`

- **Type:** `Integer`
- **Description:** NCTH handling modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentTargetTrackingSpeed`

- **Type:** `Integer`
- **Description:** NCTH target-tracking speed modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentDropCompensation`

- **Type:** `Integer`
- **Description:** NCTH drop-compensation modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentMaxCounterForce`

- **Type:** `Integer`
- **Description:** NCTH maximum counter-force modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentCounterForceAccuracy`

- **Type:** `Integer`
- **Description:** NCTH counter-force accuracy modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/AimLevels`

- **Type:** `Integer`
- **Description:** NCTH aim-level modifier while standing.
- **Only used with:** NCTH.
- **Numeric semantics:** Higher values mean a stronger associated combat value; negative values reduce it and are not meaningful unless the field is explicitly signed.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS`

- **Type:** `Container` (11 integers)
- **Description:** Structural group for crouching aim and handling modifiers. It has no value of its own; the child fields carry the modifiers.
- **Values:** One child element per modifier; an absent group means all children are neutral.
- **Default:** Absent; children default to `0` when omitted.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/FlatBase`

- **Type:** `Integer`
- **Description:** NCTH flat base modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed stance modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentBase`

- **Type:** `Integer`
- **Description:** NCTH percent base modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/FlatAim`

- **Type:** `Integer`
- **Description:** NCTH flat aim modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed stance modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentAim`

- **Type:** `Integer`
- **Description:** NCTH percent aim modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentCap`

- **Type:** `Integer`
- **Description:** NCTH aim-cap modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentHandling`

- **Type:** `Integer`
- **Description:** NCTH handling modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentTargetTrackingSpeed`

- **Type:** `Integer`
- **Description:** NCTH target-tracking speed modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentDropCompensation`

- **Type:** `Integer`
- **Description:** NCTH drop-compensation modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentMaxCounterForce`

- **Type:** `Integer`
- **Description:** NCTH maximum counter-force modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentCounterForceAccuracy`

- **Type:** `Integer`
- **Description:** NCTH counter-force accuracy modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/AimLevels`

- **Type:** `Integer`
- **Description:** NCTH aim-level modifier while crouching.
- **Only used with:** NCTH.
- **Numeric semantics:** Higher values mean a stronger associated combat value; negative values reduce it and are not meaningful unless the field is explicitly signed.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS`

- **Type:** `Container` (11 integers)
- **Description:** Structural group for prone aim and handling modifiers. It has no value of its own; the child fields carry the modifiers.
- **Values:** One child element per modifier; an absent group means all children are neutral.
- **Default:** Absent; children default to `0` when omitted.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `source`

**Stance modifier fields (NCTH).** Each stance group has the same eleven fields; they are read by the New Chance to Hit system.
Empty means neutral; positive and negative percentages are allowed.

### `ITEMLIST/ITEM/PRONE_MODIFIERS/FlatBase`

- **Type:** `Integer`
- **Description:** NCTH flat base modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed stance modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentBase`

- **Type:** `Integer`
- **Description:** NCTH percent base modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/FlatAim`

- **Type:** `Integer`
- **Description:** NCTH flat aim modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed stance modifier: higher values mean a stronger positive effect, and negative values apply the opposite effect.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentAim`

- **Type:** `Integer`
- **Description:** NCTH percent aim modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentCap`

- **Type:** `Integer`
- **Description:** NCTH aim-cap modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentHandling`

- **Type:** `Integer`
- **Description:** NCTH handling modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentTargetTrackingSpeed`

- **Type:** `Integer`
- **Description:** NCTH target-tracking speed modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentDropCompensation`

- **Type:** `Integer`
- **Description:** NCTH drop-compensation modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentMaxCounterForce`

- **Type:** `Integer`
- **Description:** NCTH maximum counter-force modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentCounterForceAccuracy`

- **Type:** `Integer`
- **Description:** NCTH counter-force accuracy modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/AimLevels`

- **Type:** `Integer`
- **Description:** NCTH aim-level modifier while prone.
- **Only used with:** NCTH.
- **Numeric semantics:** Higher values mean a stronger associated combat value; negative values reduce it and are not meaningful unless the field is explicitly signed.
- **Values:** Integer modifier; empty means neutral. No narrower validation limit is recorded.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

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

No C++ source review was performed for this block. Statements that depend on
the meaning rather than the value are marked `inferred`; the
remaining uncertainties are listed below.

## Open questions

- `DrugType`: the editor metadata types it as a flag mask, while the field is
  also described as a sequence in the lookup. Both readings are documented;
  the lookup remains the authority for named values.
- `FoodType`: the [Food](Food.md) table defines the rows; the item file stores
  only the class index.
- `bSoundType`: the full sound-family enum is not reconstructed.
- `RobotTargetingSkillGrant`, `RobotUtilitySkillGrant` and
  `RobotChassisSkillGrant`: the unit (skill points or skill level) is not
  verified.
- `TransportGroupMinProgress` and `TransportGroupMaxProgress`: the unit and scale
  are not confirmed; the document treats them as transport progress values.
- `AttachmentClass`, `nasAttachmentClass` and `nasLayoutClass` are documented
  as bit masks; individual bit names live in the corresponding lookup pages.
