---
schema: 1
lang: en
family: vehicles
title: "Vehicles - vehicle definitions and seats"
tables:
  - Vehicles
table_ids:
  - Vehicles
files:
  - Vehicles.xml
root: VEHICLES
record: VEHICLE
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# Vehicles - vehicle definitions and seats

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`VEHICLES/VEHICLE/uiIndex`](#vehiclesvehicleuiindex)
  - [`VEHICLES/VEHICLE/Name`](#vehiclesvehiclename)
  - [`VEHICLES/VEHICLE/LongName`](#vehiclesvehiclelongname)
  - [`VEHICLES/VEHICLE/ShortName`](#vehiclesvehicleshortname)
  - [`VEHICLES/VEHICLE/MvtTypes`](#vehiclesvehiclemvttypes)
  - [`VEHICLES/VEHICLE/StiFaceIcon`](#vehiclesvehiclestifaceicon)
  - [`VEHICLES/VEHICLE/SeatingCapacities`](#vehiclesvehicleseatingcapacities)
  - [`VEHICLES/VEHICLE/EnterVehicleSndID`](#vehiclesvehicleentervehiclesndid)
  - [`VEHICLES/VEHICLE/MoveVehicleSndID`](#vehiclesvehiclemovevehiclesndid)
  - [`VEHICLES/VEHICLE/VehicleArmourType`](#vehiclesvehiclevehiclearmourtype)
  - [`VEHICLES/VEHICLE/Neutral`](#vehiclesvehicleneutral)
  - [`VEHICLES/VEHICLE/VehicleEnabled`](#vehiclesvehiclevehicleenabled)
  - [`VEHICLES/VEHICLE/Pilot`](#vehiclesvehiclepilot)
  - [`VEHICLES/VEHICLE/SEAT/SeatIndex`](#vehiclesvehicleseatseatindex)
  - [`VEHICLES/VEHICLE/SEAT/SeatName`](#vehiclesvehicleseatseatname)
  - [`VEHICLES/VEHICLE/SEAT/Driver`](#vehiclesvehicleseatdriver)
  - [`VEHICLES/VEHICLE/SEAT/Rotation`](#vehiclesvehicleseatrotation)
  - [`VEHICLES/VEHICLE/SEAT/OffsetX`](#vehiclesvehicleseatoffsetx)
  - [`VEHICLES/VEHICLE/SEAT/OffsetY`](#vehiclesvehicleseatoffsety)
  - [`VEHICLES/VEHICLE/SEAT/Compartment`](#vehiclesvehicleseatcompartment)
  - [`VEHICLES/VEHICLE/SEAT/BlockedShots`](#vehiclesvehicleseatblockedshots)
  - [`VEHICLES/VEHICLE/SEAT/BlockedView`](#vehiclesvehicleseatblockedview)
  - [`VEHICLES/VEHICLE/SEAT/Hidden`](#vehiclesvehicleseathidden)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Vehicles.xml` defines vehicle names, movement/armour/sound references and the seat layout used when mercenaries ride. Each vehicle can have repeated `SEAT` entries; this page documents the shared seat field set.

## Tables

| Table | File | Notes |
|---|---|---|
| `Vehicles` | `Vehicles.xml` | — |

## Structure

```
VEHICLES
└── VEHICLE
    ├── uiIndex
    ├── Name
    ├── LongName
    ├── ShortName
    ├── MvtTypes
    ├── StiFaceIcon
    ├── SeatingCapacities
    ├── EnterVehicleSndID
    ├── MoveVehicleSndID
    ├── VehicleArmourType
    ├── Neutral
    ├── VehicleEnabled
    ├── Pilot
    └── SEAT
        ├── SeatIndex
        ├── SeatName
        ├── Driver
        ├── Rotation
        ├── OffsetX
        ├── OffsetY
        ├── Compartment
        ├── BlockedShots
        ├── BlockedView
        └── Hidden
```

## Fields

### `VEHICLES/VEHICLE/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Index of the vehicle. It is an array key.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** Required; identifies the record.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/Name`

- **Type:** `String` (`CHAR16`)
- **Description:** Internal/display name of the vehicle.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/LongName`

- **Type:** `String` (`CHAR16`)
- **Description:** Long display name shown in detailed UI; may be empty.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/ShortName`

- **Type:** `String` (`CHAR16`)
- **Description:** Short display name for the vehicle; may be empty.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/MvtTypes`

- **Type:** `Integer` (`INT32`)
- **Description:** Movement-type code for the vehicle. It is an enum/bit field.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/StiFaceIcon`

- **Type:** `String` (`CHAR8`)
- **Description:** STI face icon path for the vehicle. It is a file reference.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SeatingCapacities`

- **Type:** `Integer` (`INT32`)
- **Description:** Number of seats in the vehicle.
- **Numeric semantics:** higher = more seats; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/EnterVehicleSndID`

- **Type:** `Integer` (`INT32`)
- **Description:** Sound ID played when entering the vehicle. It is a sound reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/MoveVehicleSndID`

- **Type:** `Integer` (`INT32`)
- **Description:** Sound ID played when the vehicle moves. It is a sound reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/VehicleArmourType`

- **Type:** `Integer` (`INT16`)
- **Description:** Armour type/item ID used by the vehicle. It is a reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/Neutral`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the vehicle is neutral rather than owned by a faction. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/VehicleEnabled`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the vehicle is enabled in the game. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/Pilot`

- **Type:** `Integer` (`INT8`)
- **Description:** Profile ID of the default pilot, or `-1` when there is no pilot. It is a reference.
- **Values:** `-128..127`; `-1` means no pilot. The loader stores this field as `INT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/SeatIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Index of the seat within the vehicle. It is a seat key.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/SeatName`

- **Type:** `String` (`CHAR16`)
- **Description:** Display name of the seat, such as `Driver` or `Passenger`.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/Driver`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, this seat is the driver’s seat. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/Rotation`

- **Type:** `Integer` (`UINT8`)
- **Description:** Facing/rotation code for the seat occupant. It is an orientation code.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/OffsetX`

- **Type:** `Integer` (`INT8`)
- **Description:** Horizontal offset of the seat position in pixels.
- **Numeric semantics:** increasing = right, decreasing = left; negative values are allowed.
- **Values:** `-128..127`; the loader stores this field as `INT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/OffsetY`

- **Type:** `Integer` (`INT8`)
- **Description:** Vertical offset of the seat position in pixels.
- **Numeric semantics:** increasing = down, decreasing = up; negative values are allowed.
- **Values:** `-128..127`; the loader stores this field as `INT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/Compartment`

- **Type:** `Integer` (`UINT8`)
- **Description:** Compartment/area code for the seat. It is an enum/reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/BlockedShots`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, shots from this seat are blocked. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/BlockedView`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the occupant’s view from this seat is blocked. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/Hidden`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the seat/occupant is hidden in the vehicle UI. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

## Relations

- Sound IDs reference [Sounds](Sounds/Sounds.md); armour and icon fields reference [Items](Items/Items.md) and graphic data; `Pilot` references [MercProfiles](MercProfiles.md).

## Localization

Display names and labels are localized through language overlays; numeric IDs and codes are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Table data.
- Field names, XML comments and observed values.
- Descriptions are inferred from names and observed values; no runtime formula is claimed.

## Open questions

- Which UI surface uses `ShortName` instead of `Name`/`LongName` is not confirmed; this needs a code/UI check.
- The exact formula or enum mapping for inferred fields is not stated by the table data.
