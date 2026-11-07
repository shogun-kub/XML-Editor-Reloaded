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
status: draft
updated: 2026-11-07
---

# Items - item master table

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
  - [Item classes and `ubClassIndex`](#item-classes-and-ubclassindex)
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
  - [`ITEMLIST/ITEM/CROUCH_MODIFIERS`](#itemlistitemcrouch_modifiers)
  - [`ITEMLIST/ITEM/PRONE_MODIFIERS`](#itemlistitemprone_modifiers)
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

The table is flat: one `ITEM` record per item, keyed by `uiIndex`. The v5 file
contains 1775 records; the v4 file contains 1776.

## Tables

| File | Root / record | Records | Notes |
|---|---|---:|---|
| `Items/Items.xml` | `ITEMLIST` / `ITEM` | 1775 (v5), 1776 (v4) | Base item table. |

The localized overlays (`Russian.Items.xml` and the other language files) are
not separate tables. They override the five text fields by `uiIndex`; see
[Localization](../../Localization.md).

## Structure

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

Confidence `inferred` means the field exists in the shipped data and its role
is reconstructed from its name, the legacy item-editor caption, and the
project metadata; it was not checked against a dedicated code review in this
block. Fields marked `source` are directly observed in data or lookup tables.

**Identity and descriptions.** IDs, names and player-facing text.

### `ITEMLIST/ITEM/uiIndex`

- **Type:** `Integer`
- **Description:** Primary item ID. Cross-table references and localization overlays use this value; it is unique inside one file.
- **Values:** `0..1774` in v5; non-negative and unique inside the file.
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

- **Type:** `Flags` (bit mask)
- **Description:** Bit mask of item-class flags (`IC_*`). It selects the class-specific subtable reached through `ubClassIndex` and controls which item behaviours apply. See `ItemClass`.
- **Values:** Bit mask; `0` = none. Common observed values: `1`, `2`, `1024`, `268435456`. See [ItemClass](../Lookup.md#itemclass).
- **Default:** None; every item declares a class.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/AttachmentClass`

- **Type:** `Flags` (bit mask)
- **Description:** Built-in attachment-class bit mask used by the legacy attachment system. Bits are defined by the [AttachmentClass](../Lookup.md#attachmentclass) lookup.
- **Values:** Bit mask; `0` = none; bits are defined by [AttachmentClass](../Lookup.md#attachmentclass).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/nasAttachmentClass`

- **Type:** `Flags` (bit mask)
- **Description:** Custom NAS attachment-class bit mask used with [AttachmentSystem](../Lookup.md#attachmentsystem) and NAS attachment points.
- **Values:** Bit mask; `0` = none; bits are defined by `NasAttachmentClass`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/nasLayoutClass`

- **Type:** `Flags` (bit mask)
- **Description:** NAS layout-class bit mask. It tells the NAS layout which attachment slots the item exposes; bits are defined by [NasLayoutClass](../Lookup.md#naslayoutclass).
- **Values:** Bit mask; `0` = none; bits are defined by `NasLayoutClass`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AvailableAttachmentPoint`

- **Type:** `Flags` (bit mask)
- **Description:** Bit mask of attachment points the item can accept. Used for receivers and mounts; bits are defined by the [AttachmentPoint](../Lookup.md#attachmentpoint) lookup.
- **Values:** Bit mask; `0` = none; bits are defined by [AttachmentPoint](../Lookup.md#attachmentpoint).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AttachmentPoint`

- **Type:** `Flags` (bit mask)
- **Description:** Attachment point occupied by this item when installed. Zero means the item is not installed at a point.
- **Values:** Bit mask; `0` = none; bits are defined by [AttachmentPoint](../Lookup.md#attachmentpoint).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AttachToPointAPCost`

- **Type:** `Integer`
- **Description:** AP cost to attach or detach the item at its attachment point.
- **Values:** Observed `0..15`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ubClassIndex`

- **Type:** `Integer`
- **Description:** Index of the class-specific record selected by `usItemClass`: an armour row, magazine row, weapon row, explosive row, and so on.
- **Values:** Observed `0..1762`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ubCursor`

- **Type:** `Lookup` (integer ID)
- **Description:** Cursor lookup ID shown when the item is selected in the inventory. See [Cursor](../Lookup.md#cursor).
- **Values:** Lookup ID; see [Cursor](../Lookup.md#cursor).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/bSoundType`

- **Type:** `Integer`
- **Description:** Sound family used when the item is handled or fired. Only `0` appears in the shipped v5 file.
- **Values:** `0` = default sound family; only `0` is used in v5.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Inventory, graphics and economy.** Graphics, weight, size, price and Bobby Ray stock.

### `ITEMLIST/ITEM/ubGraphicType`

- **Type:** `Integer`
- **Description:** Graphic family for the inventory picture: `0` none, `1` big item, `2` interface, `3` alternate interface.
- **Values:** `0` none, `1` big item, `2` interface, `3` alternate interface.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ubGraphicNum`

- **Type:** `Integer`
- **Description:** Graphic index inside the family selected by `ubGraphicType`.
- **Values:** Observed `0..927`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ubWeight`

- **Type:** `Integer`
- **Description:** Item weight in the game's weight units; used by carry strength and backpack capacity.
- **Values:** Observed `0..10000`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ubPerPocket`

- **Type:** `Integer`
- **Description:** How many identical items fit into one inventory pocket. Zero means the item does not stack in a pocket.
- **Values:** Observed `0..8`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ItemSize`

- **Type:** `Integer`
- **Description:** Inventory size class (`0..99`). It drives pocket packing and item-size limits.
- **Values:** `0..99`; `99` means the item does not fit a normal pocket.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ItemSizeBonus`

- **Type:** `Integer`
- **Description:** Adjustment added to the item's effective size when pocket capacity is checked.
- **Values:** Observed `-1..1`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usPrice`

- **Type:** `Integer`
- **Description:** Base purchase price. Bobby Ray prices and some trade calculations derive from it.
- **Values:** Observed `0..70000`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ubCoolness`

- **Type:** `Integer`
- **Description:** Coolness/progress level at which the item becomes available in the campaign.
- **Values:** Observed `0..10`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/bReliability`

- **Type:** `Integer`
- **Description:** Reliability modifier: higher values make the item less likely to jam or fail.
- **Values:** Observed `-6..5`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/bRepairEase`

- **Type:** `Integer`
- **Description:** Repair-difficulty modifier: higher values make repair easier.
- **Values:** Observed `-8..5`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/Inseparable`

- **Type:** `Lookup` (integer ID)
- **Description:** Separability lookup ID that controls whether the item can be split, merged, or removed from a kit. See [Separability](../Lookup.md#separability).
- **Values:** `0` separable, `1` inseparable, `2` replaceable. See [Separability](../Lookup.md#separability).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/BR_NewInventory`

- **Type:** `Integer`
- **Description:** Bobby Ray's new-inventory stock level or quantity for this item.
- **Values:** Observed `0..40`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BR_UsedInventory`

- **Type:** `Integer`
- **Description:** Bobby Ray's used-inventory stock level or quantity.
- **Values:** Observed `0..25`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BR_ROF`

- **Type:** `Integer`
- **Description:** Bobby Ray's rate-of-fire/restock value for this item.
- **Values:** Observed `0..2500`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Food, drugs and action items.** Consumable classes and special-use flags.

### `ITEMLIST/ITEM/DrugType`

- **Type:** `Flags` (bit mask)
- **Description:** Drug/effect class. Editor metadata marks it as a flag mask, but the shipped values look like a sequence; see [DrugType](../Lookup.md#drugtype) and Open questions.
- **Values:** `0` = none; values are lookup IDs in [DrugType](../Lookup.md#drugtype); shipped values run from `0` to `18`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/FoodType`

- **Type:** `Integer`
- **Description:** Food or drink class used by the food system. The target food table is not embedded in v5.
- **Values:** `0` = none; values index the food definitions. No separate v5 lookup file exists.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usActionItemFlag`

- **Type:** `Flags` (bit mask)
- **Description:** Bit mask of action-item flags. It controls special use actions such as NET, radio, or remote detonator.
- **Values:** Bit mask; observed `0..32768`; `0` = none.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/clothestype`

- **Type:** `Integer`
- **Description:** Clothing type index used by the clothes system; see the future `Clothes` document.
- **Values:** `0` = none; index into the clothes definitions (future `Clothes` document).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/spreadPattern`

- **Type:** `String`
- **Description:** Named shotgun spread pattern: `Full Choke`, `Imp Mod Choke`, `Rifled Choke`, `Duckbill`, `Sawed off`, or `Caws`. Empty means the default.
- **Values:** `Full Choke`, `Imp Mod Choke`, `Rifled Choke`, `Duckbill`, `Sawed off`, `Caws`; empty = default.
- **Default:** Empty; default spread.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/Alcohol`

- **Type:** `Decimal`
- **Description:** Alcohol strength/effect value for drinks; feeds the drunk state and related penalties.
- **Values:** Observed `0..192`; decimal.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Weapon modifiers and overheating.** Optics, recoil, accuracy, heat and dirt.

### `ITEMLIST/ITEM/ScopeMagFactor`

- **Type:** `Decimal`
- **Description:** Scope magnification factor used by aiming and vision calculations.
- **Values:** Observed `0..10`; decimal.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ProjectionFactor`

- **Type:** `Decimal`
- **Description:** Laser projection factor; controls how far the laser dot remains useful.
- **Values:** Observed `0..3.5`; decimal.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RecoilModifierX`

- **Type:** `Decimal`
- **Description:** Horizontal recoil modifier added to the weapon's base recoil.
- **Values:** Observed `0..0`; decimal.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RecoilModifierY`

- **Type:** `Decimal`
- **Description:** Vertical recoil modifier added to the weapon's base recoil.
- **Values:** Observed `0..0`; decimal.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentRecoilModifier`

- **Type:** `Integer`
- **Description:** Percentage recoil modifier; negative values reduce recoil.
- **Values:** Observed `-40..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentAccuracyModifier`

- **Type:** `Integer`
- **Description:** Percentage accuracy modifier applied by the attachment.
- **Values:** Observed `-10..10`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usOverheatingCooldownFactor`

- **Type:** `Decimal`
- **Description:** Overheating cooldown factor. `100` is neutral; higher values cool faster.
- **Values:** `100` = neutral; higher values cool faster.
- **Default:** `100`.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/overheatTemperatureModificator`

- **Type:** `Decimal`
- **Description:** Adjustment to the weapon's overheat temperature threshold.
- **Values:** Observed `0..0`; decimal.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/overheatCooldownModificator`

- **Type:** `Decimal`
- **Description:** Adjustment to the overheat cooldown rate.
- **Values:** Observed `-0.2..0`; decimal.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/overheatJamThresholdModificator`

- **Type:** `Decimal`
- **Description:** Adjustment to the heat level at which the weapon starts jamming.
- **Values:** Observed `0..0`; decimal.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/overheatDamageThresholdModificator`

- **Type:** `Decimal`
- **Description:** Adjustment to the heat level at which the weapon takes damage.
- **Values:** Observed `0..0`; decimal.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DirtIncreaseFactor`

- **Type:** `Decimal`
- **Description:** How quickly the item accumulates dirt; higher values increase dirt gain.
- **Values:** Observed `0..120`; decimal.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Robot, random loot and shield links.** Robot damage, random-item links and riot-shield data.

### `ITEMLIST/ITEM/RobotDamageReduction`

- **Type:** `Decimal`
- **Description:** Damage-reduction multiplier for robot items; `1.0` means no reduction.
- **Values:** Decimal multiplier; `1.0` = no reduction.
- **Default:** `1`.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DiscardedLauncherItem`

- **Type:** `Integer`
- **Description:** Item ID left behind after a disposable launcher is fired.
- **Values:** Observed `0..919`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/randomitem`

- **Type:** `Integer`
- **Description:** Random-item table ID used when this item is generated as random loot.
- **Values:** Observed `0..16`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/buddyitem`

- **Type:** `Integer`
- **Description:** Item ID of the buddy/companion item linked to this item.
- **Values:** Observed `0..1724`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usRiotShieldStrength`

- **Type:** `Integer`
- **Description:** Riot-shield protection strength.
- **Values:** Observed `0..52`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usRiotShieldGraphic`

- **Type:** `Integer`
- **Description:** Graphic index for the riot-shield overlay.
- **Values:** Observed `0..2`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentNoiseReduction`

- **Type:** `Integer`
- **Description:** Percentage reduction of the noise produced by the item.
- **Values:** Observed `0..90`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Combat modifiers.** To-hit, range, damage, AP, burst and autofire modifiers.

### `ITEMLIST/ITEM/Bipod`

- **Type:** `Integer`
- **Description:** Bipod bonus value; adds stability when the weapon is deployed.
- **Values:** Observed `0..10`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ToHitBonus`

- **Type:** `Integer`
- **Description:** Flat bonus to hit probability.
- **Values:** Observed `-8..25`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BestLaserRange`

- **Type:** `Integer`
- **Description:** Range at which the laser sight gives its best accuracy bonus.
- **Values:** Observed `0..400`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RangeBonus`

- **Type:** `Integer`
- **Description:** Flat range bonus in tiles.
- **Values:** Observed `-80..120`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentRangeBonus`

- **Type:** `Integer`
- **Description:** Percentage range bonus.
- **Values:** Observed `0..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AimBonus`

- **Type:** `Integer`
- **Description:** Flat aim bonus.
- **Values:** Observed `-11..20`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/MinRangeForAimBonus`

- **Type:** `Integer`
- **Description:** Minimum range at which `AimBonus` applies.
- **Values:** Observed `0..70`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentAPReduction`

- **Type:** `Integer`
- **Description:** Percentage reduction of AP costs for actions with this item.
- **Values:** Observed `-9..20`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentStatusDrainReduction`

- **Type:** `Integer`
- **Description:** Percentage reduction of status/condition drain.
- **Values:** Observed `-300..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BloodiedItem`

- **Type:** `Integer`
- **Description:** Item ID of the bloodied variant produced when this item is bloodied.
- **Values:** Observed `0..597`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/HearingRangeBonus`

- **Type:** `Integer`
- **Description:** Bonus to hearing range.
- **Values:** Observed `0..5`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/VisionRangeBonus`

- **Type:** `Integer`
- **Description:** Bonus to general vision range.
- **Values:** Observed `0..50`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/NightVisionRangeBonus`

- **Type:** `Integer`
- **Description:** Bonus to night vision range.
- **Values:** Observed `-10..45`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DayVisionRangeBonus`

- **Type:** `Integer`
- **Description:** Bonus to day vision range.
- **Values:** Observed `-20..100`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CaveVisionRangeBonus`

- **Type:** `Integer`
- **Description:** Bonus to cave vision range.
- **Values:** Observed `-20..30`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BrightLightVisionRangeBonus`

- **Type:** `Integer`
- **Description:** Bonus to vision range in bright light.
- **Values:** Observed `-40..90`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DamageBonus`

- **Type:** `Integer`
- **Description:** Flat damage bonus.
- **Values:** Observed `-1..3`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/MeleeDamageBonus`

- **Type:** `Integer`
- **Description:** Flat melee damage bonus.
- **Values:** Observed `0..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/MagSizeBonus`

- **Type:** `Integer`
- **Description:** Magazine-size bonus.
- **Values:** Observed `0..70`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentBurstFireAPReduction`

- **Type:** `Integer`
- **Description:** Percentage AP reduction for burst fire.
- **Values:** Observed `0..80`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentAutofireAPReduction`

- **Type:** `Integer`
- **Description:** Percentage AP reduction for autofire.
- **Values:** Observed `0..50`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentReadyTimeAPReduction`

- **Type:** `Integer`
- **Description:** Percentage AP reduction for readying the item.
- **Values:** Observed `0..33`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentReloadTimeAPReduction`

- **Type:** `Integer`
- **Description:** Percentage AP reduction for reloading.
- **Values:** Observed `-30..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/AutoFireToHitBonus`

- **Type:** `Integer`
- **Description:** To-hit bonus applied to autofire.
- **Values:** Observed `0..6`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/APBonus`

- **Type:** `Integer`
- **Description:** Bonus action points while the item is equipped.
- **Values:** Observed `-11..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RateOfFireBonus`

- **Type:** `Integer`
- **Description:** Rate-of-fire bonus.
- **Values:** Observed `0..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BurstSizeBonus`

- **Type:** `Integer`
- **Description:** Extra shots in a burst.
- **Values:** Observed `0..3`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BurstToHitBonus`

- **Type:** `Integer`
- **Description:** To-hit bonus applied to burst fire.
- **Values:** Observed `0..5`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/BulletSpeedBonus`

- **Type:** `Integer`
- **Description:** Bullet-speed bonus.
- **Values:** Observed `-10..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Camouflage, vision and body modifiers.** Camo, stealth, spotting, vision and backpack/fire modifiers.

### `ITEMLIST/ITEM/CamoBonus`

- **Type:** `Integer`
- **Description:** General camouflage bonus.
- **Values:** Observed `-20..100`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/UrbanCamoBonus`

- **Type:** `Integer`
- **Description:** Additional camouflage bonus in urban sectors.
- **Values:** Observed `-20..100`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DesertCamoBonus`

- **Type:** `Integer`
- **Description:** Additional camouflage bonus in desert sectors.
- **Values:** Observed `-20..100`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/SnowCamoBonus`

- **Type:** `Integer`
- **Description:** Additional camouflage bonus in snow sectors.
- **Values:** Observed `-20..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/StealthBonus`

- **Type:** `Integer`
- **Description:** Stealth bonus; improves the chance to avoid detection.
- **Values:** Observed `-33..25`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usSpotting`

- **Type:** `Integer`
- **Description:** Spotting modifier; positive values improve detection of hidden targets.
- **Values:** Observed `0..70`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/sBackpackWeightModifier`

- **Type:** `Integer`
- **Description:** Carry-strength modifier applied when the item is in the backpack.
- **Values:** Observed `-1..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/sFireResistance`

- **Type:** `Integer`
- **Description:** Fire-resistance modifier.
- **Values:** Observed `0..25`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PercentTunnelVision`

- **Type:** `Integer`
- **Description:** Percentage of tunnel vision imposed by the item.
- **Values:** Observed `0..90`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

**Attachment system, utility and transport.** Attachment mode, kits, skills, transport progress and default attachments.

### `ITEMLIST/ITEM/AttachmentSystem`

- **Type:** `Lookup` (integer ID)
- **Description:** Attachment system selector: `0` legacy, `1` NAS, `2` NAS with layout classes. See [AttachmentSystem](../Lookup.md#attachmentsystem).
- **Values:** `0` legacy, `1` NAS, `2` NAS with layout classes. See [AttachmentSystem](../Lookup.md#attachmentsystem).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/CrowbarModifier`

- **Type:** `Integer`
- **Description:** Bonus applied when the item is used as a crowbar.
- **Values:** Observed `0..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DisarmModifier`

- **Type:** `Integer`
- **Description:** Bonus applied when disarming traps or bombs.
- **Values:** Observed `0..15`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RepairModifier`

- **Type:** `Integer`
- **Description:** Repair-kit effectiveness modifier.
- **Values:** Observed `0..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usHackingModifier`

- **Type:** `Integer`
- **Description:** Hacking bonus.
- **Values:** Observed `0..50`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usBurialModifier`

- **Type:** `Integer`
- **Description:** Burial modifier.
- **Values:** Observed `0..50`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DamageChance`

- **Type:** `Integer`
- **Description:** Chance that the item takes dirt or damage when used.
- **Values:** Observed `0..40`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/FlashLightRange`

- **Type:** `Integer`
- **Description:** Flashlight range in tiles.
- **Values:** Observed `0..18`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/ItemChoiceTimeSetting`

- **Type:** `Integer`
- **Description:** Time setting used when the item is selected in an item-choice list.
- **Values:** Observed `0..2`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/SleepModifier`

- **Type:** `Integer`
- **Description:** Sleep modifier for food, drink, or resting items.
- **Values:** Observed `0..20`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usPortionSize`

- **Type:** `Integer`
- **Description:** Portion size for food or drink items.
- **Values:** Observed `0..100`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/usAdministrationModifier`

- **Type:** `Integer`
- **Description:** Administration modifier for strategic actions.
- **Values:** Observed `0..20`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/LockPickModifier`

- **Type:** `Integer`
- **Description:** Lockpicking bonus.
- **Values:** Observed `-10..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/randomitemcoolnessmodificator`

- **Type:** `Integer`
- **Description:** Coolness adjustment applied when this item is chosen as random loot.
- **Values:** Observed `-1..1`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotStrBonus`

- **Type:** `Integer`
- **Description:** Robot strength bonus.
- **Values:** Observed `0..10`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotAgiBonus`

- **Type:** `Integer`
- **Description:** Robot agility bonus.
- **Values:** Observed `0..10`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotDexBonus`

- **Type:** `Integer`
- **Description:** Robot dexterity bonus.
- **Values:** Observed `0..10`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotTargetingSkillGrant`

- **Type:** `Integer`
- **Description:** Targeting skill granted by the robot chassis or module.
- **Values:** Observed `0..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotUtilitySkillGrant`

- **Type:** `Integer`
- **Description:** Utility skill granted by the robot chassis or module.
- **Values:** Observed `0..21`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/RobotChassisSkillGrant`

- **Type:** `Integer`
- **Description:** Chassis skill granted by the robot item.
- **Values:** Observed `0..0`; integer.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/TransportGroupMinProgress`

- **Type:** `Integer`
- **Description:** Minimum transport progress at which this item or group can appear.
- **Values:** `0..100` percent.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/TransportGroupMaxProgress`

- **Type:** `Integer`
- **Description:** Maximum transport progress at which this item or group can appear.
- **Values:** `0..100` percent.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item table data; item property metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DefaultAttachment`

- **Type:** `Integer`
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
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/NewInv`

- **Type:** `Boolean`
- **Description:** The item is available only with the new inventory.
- **Values:** `0` (no) or `1` (yes).
- **Default:** `0` when omitted.
- **Source:** item table data; item flag metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/DiseaseSystemExclusive`

- **Type:** `Boolean`
- **Description:** The item belongs to the disease system.
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

### `ITEMLIST/ITEM/CROUCH_MODIFIERS`

- **Type:** `Container` (11 integers)
- **Description:** Structural group for crouching aim and handling modifiers. It has no value of its own; the child fields carry the modifiers.
- **Values:** One child element per modifier; an absent group means all children are neutral.
- **Default:** Absent; children default to `0` when omitted.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `source`

### `ITEMLIST/ITEM/PRONE_MODIFIERS`

- **Type:** `Container` (11 integers)
- **Description:** Structural group for prone aim and handling modifiers. It has no value of its own; the child fields carry the modifiers.
- **Values:** One child element per modifier; an absent group means all children are neutral.
- **Default:** Absent; children default to `0` when omitted.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `source`

**Stance modifier fields.** Each stance group has the same eleven fields.
Empty means neutral; positive and negative percentages are allowed.

### `ITEMLIST/ITEM/STAND_MODIFIERS/FlatBase`

- **Type:** `Integer`
- **Description:** Flat base modifier while standing.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentBase`

- **Type:** `Integer`
- **Description:** Percent base modifier while standing.
- **Values:** Observed `-46..0`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/FlatAim`

- **Type:** `Integer`
- **Description:** Flat aim modifier while standing.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentAim`

- **Type:** `Integer`
- **Description:** Percent aim modifier while standing.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentCap`

- **Type:** `Integer`
- **Description:** Aim-cap modifier while standing.
- **Values:** Observed `-17..0`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentHandling`

- **Type:** `Integer`
- **Description:** Handling modifier while standing.
- **Values:** Observed `-10..20`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentTargetTrackingSpeed`

- **Type:** `Integer`
- **Description:** Target-tracking speed modifier while standing.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentDropCompensation`

- **Type:** `Integer`
- **Description:** Drop-compensation modifier while standing.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentMaxCounterForce`

- **Type:** `Integer`
- **Description:** Maximum counter-force modifier while standing.
- **Values:** Observed `-30..70`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/PercentCounterForceAccuracy`

- **Type:** `Integer`
- **Description:** Counter-force accuracy modifier while standing.
- **Values:** Observed `-46..30`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/STAND_MODIFIERS/AimLevels`

- **Type:** `Integer`
- **Description:** Number of aim levels added or removed while standing.
- **Values:** Observed `-1..3`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/FlatBase`

- **Type:** `Integer`
- **Description:** Flat base modifier while crouching.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentBase`

- **Type:** `Integer`
- **Description:** Percent base modifier while crouching.
- **Values:** Observed `-88..0`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/FlatAim`

- **Type:** `Integer`
- **Description:** Flat aim modifier while crouching.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentAim`

- **Type:** `Integer`
- **Description:** Percent aim modifier while crouching.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentCap`

- **Type:** `Integer`
- **Description:** Aim-cap modifier while crouching.
- **Values:** Observed `-33..0`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentHandling`

- **Type:** `Integer`
- **Description:** Handling modifier while crouching.
- **Values:** Observed `-5..0`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentTargetTrackingSpeed`

- **Type:** `Integer`
- **Description:** Target-tracking speed modifier while crouching.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentDropCompensation`

- **Type:** `Integer`
- **Description:** Drop-compensation modifier while crouching.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentMaxCounterForce`

- **Type:** `Integer`
- **Description:** Maximum counter-force modifier while crouching.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/PercentCounterForceAccuracy`

- **Type:** `Integer`
- **Description:** Counter-force accuracy modifier while crouching.
- **Values:** Observed `-88..0`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/CROUCH_MODIFIERS/AimLevels`

- **Type:** `Integer`
- **Description:** Number of aim levels added or removed while crouching.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/FlatBase`

- **Type:** `Integer`
- **Description:** Flat base modifier while prone.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentBase`

- **Type:** `Integer`
- **Description:** Percent base modifier while prone.
- **Values:** Observed `-56..0`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/FlatAim`

- **Type:** `Integer`
- **Description:** Flat aim modifier while prone.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentAim`

- **Type:** `Integer`
- **Description:** Percent aim modifier while prone.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentCap`

- **Type:** `Integer`
- **Description:** Aim-cap modifier while prone.
- **Values:** Observed `-17..0`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentHandling`

- **Type:** `Integer`
- **Description:** Handling modifier while prone.
- **Values:** Observed `-75..0`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentTargetTrackingSpeed`

- **Type:** `Integer`
- **Description:** Target-tracking speed modifier while prone.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentDropCompensation`

- **Type:** `Integer`
- **Description:** Drop-compensation modifier while prone.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentMaxCounterForce`

- **Type:** `Integer`
- **Description:** Maximum counter-force modifier while prone.
- **Values:** Observed `-5..100`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/PercentCounterForceAccuracy`

- **Type:** `Integer`
- **Description:** Counter-force accuracy modifier while prone.
- **Values:** Observed `-56..35`; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

### `ITEMLIST/ITEM/PRONE_MODIFIERS/AimLevels`

- **Type:** `Integer`
- **Description:** Number of aim levels added or removed while prone.
- **Values:** `-100..100` percent or flat modifier; empty means neutral.
- **Default:** `0` when omitted; an empty element is neutral.
- **Source:** item table data; stance modifier metadata.
- **Confidence:** `inferred`

## Relations

- `Items.uiIndex` is referenced by many tables: starting gear, random items,
  attachments, trader inventories, map extras and quest data.
- `Items.ubClassIndex` selects a row in the table chosen by `usItemClass`;
  see [Armours](Armours.md), [Magazines](Magazines.md) and
  [Launchables](Launchables.md).
- `Items.ubCursor` -> `Cursor`; `Items.Inseparable` -> `Separability`.
- `Items.AttachmentClass` -> `AttachmentClass`; `Items.nasAttachmentClass` ->
  `NasAttachmentClass`; `Items.nasLayoutClass` -> `NasLayoutClass`.
- `Items.AttachmentPoint` and `Items.AvailableAttachmentPoint` ->
  `AttachmentPoint`; `Items.AttachmentSystem` -> `AttachmentSystem`.
- `Items.DrugType` -> `DrugType`; `Items.FoodType` -> the food definitions
  (future `Food` document).
- `Items.clothestype` -> the clothes definitions (future `Clothes` document).
- `Items.spreadPattern` -> the spread-pattern definitions (future
  `SpreadPatterns` document).
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

- v5: 1775 records, 233 top-level elements including three stance
  groups; 257 unique leaf paths (254 scalar fields plus the three group
  containers). This is the primary source for the document.
- v4: 1776 records. The set of unique leaf paths is identical;
  text and a few values differ because of localization and content edits.
- No record or field was removed from the documentation to make the counts
  match; `DefaultAttachment` is one documented path with repeated elements.

## Sources

- Item table data: v5 primary, v4 comparison.
- Item property and flag metadata from the editor database.
- Lookup tables for cursors, attachment classes, separability, drugs and
  attachment systems.
- Localization overlays for the five text fields.

No C++ source review was performed for this block. Statements that depend on
the meaning rather than the observed value are marked `inferred`; the
remaining uncertainties are listed below.

## Open questions

- `DrugType`: the editor metadata types it as a flag mask, while the shipped
  values look like a sequence. Both readings are documented; the lookup
  remains the authority for named values.
- `FoodType`: the target food table is not present in the v5 TableData set, so
  the description stays at the level of a food-class index.
- `bSoundType`: only `0` occurs in v5; the full enum is not reconstructed.
- `RobotTargetingSkillGrant`, `RobotUtilitySkillGrant` and
  `RobotChassisSkillGrant`: the unit (skill points or skill level) is not
  verified.
- `TransportGroupMinProgress` and `TransportGroupMaxProgress`: percent is
  inferred from the value range and the transport context, not from a named
  schema.
- `AttachmentClass`, `nasAttachmentClass` and `nasLayoutClass` are documented
  as bit masks; individual bit names live in the corresponding lookup pages.
