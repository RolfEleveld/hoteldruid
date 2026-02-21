# Room API Specification

## Overview

The Room API endpoints provide full CRUD operations for managing hotel rooms/apartments, aligned with the hoteldruid `appartamenti` database table structure.

## Property Mapping: HotelDruid Database → API

This table documents how hoteldruid database columns map to API properties:

| API Property | hoteldruid Column | Type | Required | Description |
|---|---|---|---|---|
| **Id** | idappartamenti (PK) | string | Generated | Unique room identifier, assigned by system (base32-encoded GUID) |
| **Name** | idappartamenti | string | ✓ Yes | Room/apartment identifier (e.g., "101", "Suite-A", "Room-West-1") |
| **Capacity** | maxoccupanti | integer | ✓ Yes | Maximum number of occupants. Must be > 0. |
| **FloorNumber** | numpiano | string | Optional | Physical floor designation (e.g., "1", "2", "Ground", "Basement") |
| **HouseNumber** | numcasa | string | Optional | House/building section identifier (e.g., "A", "B1", "West Wing") |
| **Priority** | priorita | integer | Optional | Booking assignment priority. Lower values indicate higher priority. Used to select preferred rooms during automated assignment. |
| **SecondaryPriority** | priorita2 | integer | Optional | Bed selection priority. Used for determining bed assignments when assigning multiple occupants to a room. |
| **HasBeds** | letto | string | Optional | Room has beds flag. Expected values: "S" (yes/sì), "N" (no). Indicates room is suitable/configured for sleeping arrangements. |
| **NeighboringRooms** | app_vicini | string | Optional | Comma-separated list of adjacent/neighboring room IDs. Used for identifying proximate rooms and managing room relationships. Format: "101,102,103" |
| **Comments** | commento | string | Optional | Arbitrary notes or metadata about the room (e.g., "Needs maintenance", "Premium balcony", "Ground floor accessible") |

## Field Validation Rules

### Required Fields
- **Name** (mapped from idappartamenti)
  - Must not be null or whitespace
  - Uniqueness constraint: no two rooms can have the same name
  - Typical format: numeric ("101") or alphanumeric ("Suite-A")

- **Capacity** (mapped from maxoccupanti)
  - Must be explicitly provided
  - Must be greater than 0
  - Typical range: 1-6 for hotel rooms

### Optional Fields
- **FloorNumber**: No format restrictions; can be numeric, text, or mixed
- **HouseNumber**: No format restrictions; can be numeric, text, or mixed
- **Priority**: Integer; no specific range enforced, but typically 1-10
- **SecondaryPriority**: Integer; no specific range enforced
- **HasBeds**: Expected values "S" or "N", but not validated (any string accepted)
- **NeighboringRooms**: Comma-separated string; referential integrity NOT enforced (references are not validated)
- **Comments**: Any string including empty

## API Endpoints

### Create Room
```
POST /api/rooms
Content-Type: application/json

Request Body (RoomDto):
{
  "name": "101",
  "capacity": 2,
  "floorNumber": "1",
  "houseNumber": "A",
  "priority": 1,
  "secondaryPriority": 2,
  "hasBeds": "S",
  "neighboringRooms": "102,103",
  "comments": "Standard room with balcony"
}

Response: 201 Created
{
  "id": "base32-encoded-guid",
  "name": "101",
  "capacity": 2,
  "floorNumber": "1",
  "houseNumber": "A",
  "priority": 1,
  "secondaryPriority": 2,
  "hasBeds": "S",
  "neighboringRooms": "102,103",
  "comments": "Standard room with balcony"
}

Error Responses:
- 400 Bad Request: Missing name or invalid capacity (≤ 0)
- 409 Conflict: Name already exists
```

### Get Room by ID
```
GET /api/rooms/{id}

Response: 200 OK
{
  "id": "base32-encoded-guid",
  "name": "101",
  "capacity": 2,
  ...
}

Error Responses:
- 404 Not Found: Room with specified ID does not exist
```

### Get Room by Name
```
GET /api/rooms?name=101

Response: 200 OK
{
  "id": "base32-encoded-guid",
  "name": "101",
  "capacity": 2,
  ...
}

Error Responses:
- 404 Not Found: Room with specified name does not exist
```

### List All Rooms
```
GET /api/rooms

Response: 200 OK
[
  {
    "id": "guid1",
    "name": "101",
    "capacity": 2,
    ...
  },
  {
    "id": "guid2",
    "name": "102",
    "capacity": 3,
    ...
  }
]
```

### Update Room
```
PUT /api/rooms/{id}
Content-Type: application/json

Request Body (RoomDto):
{
  "name": "101-Updated",
  "capacity": 3,
  "floorNumber": "2",
  ...
}

Response: 200 OK
{
  "id": "base32-encoded-guid",
  "name": "101-Updated",
  "capacity": 3,
  ...
}

Error Responses:
- 400 Bad Request: Invalid name or capacity
- 404 Not Found: Room with specified ID does not exist
```

### Delete Room
```
DELETE /api/rooms/{id}

Response: 204 No Content

Error Responses:
- 404 Not Found: Room with specified ID does not exist
```

## Usage Examples

### Create Minimal Room
```json
{
  "name": "101",
  "capacity": 2
}
```

### Create Room with All Fields
```json
{
  "name": "Suite-A",
  "capacity": 4,
  "floorNumber": "2",
  "houseNumber": "B",
  "priority": 1,
  "secondaryPriority": 2,
  "hasBeds": "S",
  "neighboringRooms": "Suite-B,Suite-C",
  "comments": "Premium suite with balcony and sea view"
}
```

### Partial Update
Only changed fields need to be provided:
```json
{
  "name": "101",
  "capacity": 3,
  "floorNumber": "2"
}
```
(Other fields will be preserved from existing record)

## Notes

### Data Integrity
- All room data is persisted and retrieved with field integrity maintained
- Updates preserve any fields not explicitly provided in the request
- Null/missing optional fields are stored and returned as null

### Neighboring Rooms
- NeighboringRooms is a comma-separated string of room IDs
- No validation is performed on neighbor IDs (they don't need to exist)
- Typical usage: "101,102,103" for adjacent rooms
- Used for proximity analysis and room relationship queries

### Priority System
- **Priority**: Room selection precedence during automated booking assignment
  - Lower values = higher priority for selection
  - Typical scale: 1-10 (1 = highest priority)
- **SecondaryPriority**: Bed/occupant assignment within room
  - Used when assigning multiple people to a room
  - Helps determine preferred bed allocation

### HasBeds Flag
- "S" (Italian 'sì' = yes): Room has beds configured
- "N" (Italian 'no' = no): Room does not have beds or is configured without beds
- Can be null (no bed information)
- Primarily metadata for room capability description

## Removed Properties

The following properties were NOT included in this API design because they do not exist in the hoteldruid `appartamenti` table:
- ~~RoomType~~ (custom addition, not in hoteldruid)
- ~~PricePerNight~~ (custom addition, pricing is managed in tariffs table)

These concepts can be implemented through hoteldruid's tariff system (`ntariffe` table) for pricing and custom attributes if needed.

## Test Coverage

The API includes comprehensive test coverage (27 tests) including:
- **CRUD Operations**: Create, Read (by ID/name), List, Update, Delete
- **Field Coverage**: All 9 properties tested individually
- **Partial Updates**: Tests verify behavior when updating with missing fields
- **Validation**: Required field validation, capacity validation
- **Error Handling**: Duplicate names, non-existent IDs, invalid data
- **Integration Tests**: Neighboring rooms relationships, priority logic
- **Data Integrity**: Full round-trip create-read-update-delete cycles

See [RoomsApiTests.cs](tests/HotelDroid.Api.Tests/Integration/RoomsApiTests.cs) for detailed test implementations.
