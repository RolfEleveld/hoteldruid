namespace HotelDruid.Shared;

/// <summary>Full client DTO. Maps to HotelDruid 'clienti' table.</summary>
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
    string? Notes = null,
    DateOnly? ActualArrivalDate = null,
    int? ActualGuestCount = null,
    double? PrepaidAmount = null,
    string? PrepaidMethod = null,
    string? PrepaidReference = null,
    DateOnly? ActualDepartureDate = null,
    int? ExpectedGuestCount = null,
    string? TariffId = null,
    string? PreAuthReference = null,
    string? PreAuthMethod = null,
    DateTime? SettlementDate = null
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

/// <summary>One immutable financial movement on a booking.</summary>
public record BookingTransactionDto(
    string? BookingId,
    int Sequence,
    DateTime Timestamp,
    string? Type,
    decimal Amount,
    string? Description,
    string? AppliedTo,
    string? CreatedBy
);

/// <summary>Request used to append a payment, deposit, refund, or adjustment.</summary>
public record BookingTransactionWriteDto(
    string? Type,
    decimal Amount,
    string? Description = null,
    string? AppliedTo = null,
    string? CreatedBy = null,
    DateTime? Timestamp = null
);

/// <summary>One account in the small USALI chart used by the hotel ledger.</summary>
public record ChartAccountDto(
    string Code,
    string Name,
    string Group,
    string NormalBalance,
    string Description
);

/// <summary>One balanced line from the hotel general ledger.</summary>
public record LedgerLineDto(
    string? Id,
    string? JournalId,
    DateTime Timestamp,
    string? EntryType,
    string? BookingId,
    string? AccountCode,
    string? AccountGroup,
    decimal Debit,
    decimal Credit,
    string? CashRegisterId,
    string? PayerName,
    string? Description,
    string? SourceType,
    string? SourceId
);

/// <summary>Read-only financial position for one booking.</summary>
public record BookingFinanceSummaryDto(
    string BookingId,
    int Year,
    decimal StayTotal,
    decimal ExtraTotal,
    decimal ChargesTotal,
    decimal PrepaidAmount,
    decimal PaymentsTotal,
    decimal RefundsTotal,
    decimal DepositsHeld,
    decimal TotalDue,
    string Currency,
    string? TariffId,
    string? TariffName,
    int Nights
);

/// <summary>
/// Room/Apartment Data Transfer Object.
/// Maps to HotelDruid 'appartamenti' table.
/// 
/// Property Mapping (HotelDruid database → API):
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

// --- Layer 5: Communication/Integration DTOs ---

public record MessageDto(string? Id, string? MessageType, string? Status, string? Sender, string? Body, string? RecipientUserIds, DateTime? CreatedAt, DateTime? SeenAt);

public record ContractTemplateDto(string? Id, string? Type, int Number, string? Content);

public record ExternalIntegrationDto(string? Id, string? IntegrationName, string? IdType, int? LocalId, string? RemoteId1, string? RemoteId2, int? Year, DateTime? CreatedAt);

public record SessionDto(string? SessionId, int? UserId, string? IpAddress, string? ConnectionType, DateTime? LastAccess);

