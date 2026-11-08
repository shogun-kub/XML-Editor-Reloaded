# Glossary

Canonical English terms and the approved Russian translations used by the help set and, later,
by the Russian UI. Add a row before using a new term in a document or a `Strings.ru.resx`
entry. The English column is the source of truth; the Russian column is a controlled label.

| English | Russian (approved) | Forbidden / notes |
|---|---|---|
| table | таблица | — |
| record | запись | — |
| field | поле | — |
| key | ключ | not "ID" unless the field is literally an ID |
| root element | корневой элемент | — |
| record element | элемент записи | — |
| lookup | справочник | — |
| flag lookup | справочник-маска | not "флаговый справочник" |
| sequence lookup | последовательный справочник | — |
| bit mask | битовая маска | — |
| sparse | разреженный | about fields that are absent in some records |
| overlay | оверлей | localization overlay |
| confidence | достоверность | — |
| source | источник | — |
| inferred | выведено | — |
| unknown | неизвестно | — |
| item | предмет | — |
| action item | предмет действия | map interactable; `ACTION_ITEM_*` codes in `Action Items.h` |
| item class | класс предмета | — |
| attachment | вложение | — |
| LBE (load-bearing equipment) | разгрузка | not "носимое снаряжение" |
| NAS (new attachment system) | новая система вложений (NAS) | keep the abbreviation in code contexts |
| OAS (old attachment system) | старая система вложений (OAS) | opposed to NAS; `AttachmentSystem` 1 selects OAS only |
| pocket | карман | inventory/LBE pocket; not "отделение" |
| fortification points | очки укрепления | `dCreationCost` progress for structures; not money |
| armour | броня | not "доспех" |
| armour class | класс брони | — |
| protection | защита | the raw armour value |
| coverage | покрытие | armour coverage, percent |
| degrade | износ | `ubDegradePercent` |
| magazine | магазин | the item, not a shop |
| calibre | калибр | — |
| ammo type | тип боеприпаса | — |
| magazine type | тип магазина | also covers bullet/box/crate |
| launchable | боеприпас | **approved 2026-11-07**. Not "Запускаемые предметы", not "запускаемое" |
| AmmoTypes | Типы боеприпасов | table `AmmoTypes` |
| AmmoStrings | Названия калибров | table `AmmoStrings` |
| trader | торговец | covers merchants and dealers |
| dealer | дилер | shopkeeper inventory files |
| merchant | торговец | `Merchants` |
| town | город | — |
| sector | сектор | map sector, e.g. `B13` |
| facility | объект | map facility instance |
| facility type | тип объекта | `FacilityTypes` |
| loyalty | лояльность | — |
| rebel sentiment | склонность к мятежу | `townRebelSentiment` |
| militia | ополчение | — |
| hidden town | скрытый город | `hiddenTown` |
| town icon | значок города | `townIcon` |
| base sector | базовый сектор | top-left sector of the town militia map |
| town point | точка города | map label position |
| country noun | название жителя страны | `countryNoun` |
| AP (action points) | очки действия (ОД) | — |
| CTH (chance to hit) | шанс попадания | — |
| recoil | отдача | signed muzzle deviation; X is horizontal, Y is vertical |
| aim level | уровень прицеливания | `0` = automatic level count |
| burst penalty | штраф за очередь | accuracy penalty per shot after the first |

## Notes

- `Launchables` is the user-approved exception: the Russian UI uses **«Боеприпасы»**.
  `Strings.ru.resx` currently contains both «Запускаемые предметы» and «Боеприпасы»; M32
  must align every occurrence with this row.
- Table names are never translated: `Armours`, `Cities`, `ItemClass`.
- Field names are never translated: `ubArmourClass`, `townRebelSentiment`.
- When a term has no approved Russian row yet, keep the English term in the Russian text and
  add the row before release.
