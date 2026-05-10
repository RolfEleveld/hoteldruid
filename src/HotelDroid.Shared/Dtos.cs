namespace HotelDroid.Shared;

/// <summary>Full client DTO. Maps to hoteldruid 'clienti' table.</summary>
public record ClientDto(
    string? Id,
    string LastName,
    string? FirstName = null,
    string? Nickname = null,
    string? Gender = null,
    string? Title = null,
    string? Language = null,
    DateTime? DateOfBirth = null,
    string? BirthCity = null,
    string? BirthRegion = null,
    string? BirthNation = null,
    string? DocumentNumber = null,
    DateTime? DocumentExpiry = null,
    string? DocumentType = null,
    string? DocumentCity = null,
    string? DocumentRegion = null,
    string? DocumentNation = null,
    string? Nationality = null,
    string? Nation = null,
    string? Region = null,
    string? City = null,
    string? Street = null,
    string? StreetNumber = null,
    string? PostalCode = null,
    string? Phone = null,
    string? Phone2 = null,
    string? Phone3 = null,
    string? Fax = null,
    string? Email = null,
    string? Email2 = null,
    string? Email3 = null,
    string? TaxCode = null,
    string? VatNumber = null,
    string? Notes = null,
    int? MaxOrderNumber = null,
    string? CompanionIds = null,
    string? DocumentsSent = null,
    DateTime? CreatedAt = null,
    string? HostCreated = null,
    int? CreatedBy = null
);
public record BookingDto(
    string? Id = null,
    int Year = 0,
    string? ClientId = null,
    string? RoomId = null,
    DateOnly? ArrivalDate = null,
    DateOnly? DepartureDate = null,
    string? Status = null,
    string? Notes = null
);

public record BookingCostDto(
    string? Id = null,
    int Year = 0,
    string? BookingId = null,
    string? TariffId = null,
    double? Amount = null,
    string? Description = null
);

public record BookingGuestDto(
    string? Id = null,
    int Year = 0,
    string? BookingId = null,
    string? ClientId = null,
    int? GuestNumber = null
);

public record CancelledBookingDto(
    string? Id = null,
    int Year = 0,
    string? ClientId = null,
    string? RoomId = null,
    DateOnly? ArrivalDate = null,
    DateOnly? DepartureDate = null,
    string? Status = null,
    DateTime? CancelledAt = null,
    string? Notes = null
);

public record ExpenseDto(
    string? Id = null,
    int Year = 0,
    string? CashRegisterId = null,
    double? Amount = null,
    string? Description = null,
    DateOnly? Date = null
);

public record MoneyHistoryDto(
    string? Id = null,
    int Year = 0,
    string? CashRegisterId = null,
    double? Amount = null,
    string? Type = null,
    string? Description = null,
    DateOnly? Date = null
);

/// <summary>
/// Room/Apartment Data Transfer Object.
/// Maps to hoteldruid 'appartamenti' table.
/// 
/// Property Mapping (hoteldruid database → API):
/// - Name ← idappartamenti (PRIMARY KEY: Room identifier, e.g., "101", "Suite-A")
/// - FloorNumber ← numpiano (Physical floor, e.g., "1", "2", "Ground")
/// - Capacity ← maxoccupanti (Max occupants as INTEGER, e.g., 2, 4)
/// - HouseNumber ← numcasa (House/building section, e.g., "A", "B1")
/// - Priority ← priorita (Booking priority INTEGER, lower=higher priority)
/// - SecondaryPriority ← priorita2 (Bed assignment priority)
/// - HasBeds ← letto (VARCHAR(1), "S"=yes, "N"=no)
/// - NeighboringRooms ← app_vicini (Comma-separated IDs of adjacent rooms)
/// - Comments ← commento (Interior/metadata notes)
/// </summary>
public record RoomDto(
    string? Id, 
    string Name, 
    int Capacity, 
    string? FloorNumber = null,
    string? HouseNumber = null,
    int? Priority = null,
    int? SecondaryPriority = null,
    string? HasBeds = null,
    string? NeighboringRooms = null,
    string? Comments = null
);
