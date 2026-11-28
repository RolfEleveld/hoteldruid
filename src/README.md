# Source

HotelDruid is three projects.

| Project | Role |
| --- | --- |
| [HotelDruid.Client](HotelDruid.Client) | Blazor WebAssembly desk. The browser downloads this and calls the API. |
| [HotelDruid.Api](HotelDruid.Api) | ASP.NET Core API. It stores the hotel as files and serves the desk. |
| [HotelDruid.Shared](HotelDruid.Shared) | Contracts used by both. |

The API is the program that a desk install and a container both run. The client is copied into the API `wwwroot` by the build. A PHP MySQL or SQLite database is loaded by [tools/HotelDruid.Migration](../tools/HotelDruid.Migration/README.md). The steps are in [Import a PHP hotel](../docs/import-from-php.md).

## Configuration

System configuration is stored in the same file store as the other hotel records.

- `GET /api/system/configuration`
- `PUT /api/system/configuration`
- `DELETE /api/system/configuration`
- `GET /api/system/setup/status`

`/setup` opens when configuration is missing for an administrator, or when the address includes `?configure=true`. The page asks for the baseline values, including the default currency and year. Export and import include `system_configuration` when it is present.

A request for a missing year of periods or tariffs returns an empty list, then fills that year from the configured source year. An unknown room id still returns 404.

Tests cover configuration persistence, setup detection, export of the configuration, the year fill, and that 404.
