# Email text, placeholders and formatting

This page explains how text in the e-mail tables is interpreted. It covers
`EmailMercAvailable`, `EmailMercLevelUp` and `EmailSenderNameList` in `Data-1.13`, and the
Base table `Emails.xml` where a data set provides one.

## Literal text tables

`EmailMercAvailable` and `EmailMercLevelUp` store finished subject and message strings. The
game copies them into the e-mail without placeholder substitution. A `$MERCNAME$`, `$AMOUN$`,
`$DESTINATIONNAME$` or `%s` typed into these tables is displayed as literal text.

The sender display name is not stored in either table. It is read from `MercProfiles` for the
profile selected by the record's `uiIndex`, which is the merc profile ID. `EmailSenderNameList`
is separate: its `uiIndex` selects a sender name for generic `Emails.xml` mail, not a
mercenary profile, and its `Name` value is also used verbatim.

Use `±` in `Message` for a line break. It is the plus-minus sign (U+00B1, decimal 177), not a
placeholder. Shipped `EmailMercAvailable` and `EmailMercLevelUp` messages use only `±`; they do
not contain `$...$` tokens.

## `Emails.xml` placeholders

`Emails.xml` is a Base table (`Base/TableData/Email`), not part of
`Data-1.13/TableData/Email`. Its comment header defines three placeholders that may appear in
`Subject` and `Message`:

| Placeholder | Replaced with | Data required by the call |
|---|---|---|
| `$MERCNAME$` | The name of a mercenary. | `AddEmailFromXML` with the merc ID (`specialdata2`; the Lua helper calls it `MercID`). |
| `$AMOUN$` | A dollar amount. | `AddEmailFromXML` with the amount (`specialdata1`). The token is `$AMOUN$`, not `$AMOUNT$`. |
| `$DESTINATIONNAME$` | A Bobby Ray shipping destination name. | `iCurrentShipmentDestinationID` for the shipment. |

The header states that the matching call data is required: the placeholders are not resolved
from the text itself. Use the exact uppercase spelling, including the trailing dollar sign;
spaces inside the token make it ordinary text.

These placeholders belong to `Emails.xml`. Do not assume that `$MERCNAME$`, `$AMOUN$` or
`$DESTINATIONNAME$` work in `EmailMercAvailable`, `EmailMercLevelUp` or
`EmailSenderNameList`: those tables are loaded and displayed without that substitution.

## Formatting markers

The `Emails.xml` header documents these markers for message text:

| Marker | Effect |
|---|---|
| `² Text ²` | Renders the enclosed text in bold. |
| `³ Text ³` | Centers the enclosed text on its own line. |
| `±` | Starts a new line. |
| `&amp;`, `&lt;`, `&gt;` | Produce `&`, `<` and `>` in the displayed text; XML requires the escaped form. |

These are formatting conventions of `Emails.xml`. The three `Data-1.13` e-mail tables are not
written with `²` or `³` in the reference data; only `±` appears there.

## `%s` is not an e-mail placeholder

`History.xml` uses printf-style `%s` placeholders that are filled with mercenary, town or mine
names by the history-log code. That mechanism is specific to the history log. It does not apply
to any e-mail table, and `%s` written into an e-mail message is not replaced.

## Sources

- `EmailMercAvailable.xml`, `EmailMercLevelUp.xml` and `EmailSenderNameList.xml` table data.
- Game code that loads and displays e-mail text.
- The comment header of `Base/TableData/Email/Emails.xml`, which defines the placeholders and
  formatting markers above.
