# Quick Reference - New Additions

## Architecture Refinements Session 2

**Added to EXPORT_IMPORT_DESIGN.md:** Two critical enhancements

---

## Change 1: Hosting System Field

### In Manifest

```json
"source_machine": {
  "hosting_system": "PHP 7.4.3"    // Generic, not PHP-specific
}
```

**Why:** Enables any implementation to document its hosting environment  
**Examples:**

- `"PHP 7.4.3"` - HotelDroid PHP
- `"Blazor .NET 8.0"` - Blazor/C# implementation
- `"Node.js 20.0"` - Future Node.js implementation

---

## Change 2: Entity Mapping

### New File in Export Package

```text
metadata/entity_mapping.json
```

### Mapping Levels

**1. Table Mapping:**

```json
"table_translations": {
  "clienti": {
    "international_name": "guests",
    "description": "Client/Guest records",
    "primary_key": "idclienti",
    "record_count": 892
  }
}
```

**2. Field Mapping:**

```json
"field_translations": {
  "clienti": {
    "idclienti": "id",
    "cognome": "last_name",
    "nome": "first_name",
    "email": "email_address"
  }
}
```

**3. Relationship Mapping:**

```json
"relationship_translations": {
  "contratti_idclienti": {
    "source_table": "contratti",
    "source_key": "idclienti",
    "source_table_international": "contracts",
    "source_key_international": "guest_id"
  }
}
```

---

## Import Logic with Mappings

```text
1. Extract ZIP
2. Read manifest (know source: PHP 7.4.3)
3. Read entity_mapping.json (know the meanings)
4. Check target system:
   ├─ PHP? → Skip transformation (native format)
   ├─ Blazor? → Apply mapping: guests→Customer, cognome→LastName
   └─ Node.js? → Apply mapping: guests→Guest, cognome→surname
5. Import with transformed data
```

---

## Tables Mapped (Partial List)

| Italian | International | Meaning |
| - | - | - |
| clienti | guests | Guest/Client records |
| contratti | contracts | Client agreements |
| appartamenti | properties | Rooms/Apartments |
| prenota* | bookings_* | Reservations |
| regole | rules | Booking rules |
| ntariffe | rates | Pricing |

---

## Design Decisions Updated

**Decision 9:** Entity Mapping for Implementation Agnostic Imports  
**Decision 10:** Implementation-Agnostic Hosting System Field

---

## Impact Summary

| Aspect | Impact |
| - | - |
| PHP Import | No change - native format |
| Blazor Import | Now possible with entity mapping |
| Node.js Import | Future-proofed with mapping |
| Data Preservation | Guaranteed via mapping |
| Platform Agnosticism | Complete |

---

## Files Updated

- ✅ EXPORT_IMPORT_DESIGN.md (150+ lines added)
- ✅ ARCHITECTURE_REFINEMENT_2.md (new document)

---

**Status:** ✅ Ready for implementation phase
