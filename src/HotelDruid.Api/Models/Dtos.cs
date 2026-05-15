using HotelDruid.Shared;

namespace HotelDruid.Api.Models;

// DTOs are now defined in HotelDruid.Shared.Dtos.cs for shared use between Client and API

/// <summary>
/// Internal storage model for rooms.
/// Explicit field definitions to avoid JSON deserialization defaults.
/// </summary>
public class RoomStorageModel
{
    public string? Name { get; set; }
    public int? Capacity { get; set; }
    public string? FloorNumber { get; set; }
    public string? HouseNumber { get; set; }
    public int? Priority { get; set; }
    public int? SecondaryPriority { get; set; }
    public string? HasBeds { get; set; }
    public string? NeighboringRooms { get; set; }
    public string? Comments { get; set; }
}

public class AssetStorageModel
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class WarehouseStorageModel
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? FloorNumber { get; set; }
    public string? HouseNumber { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class InventoryStorageModel
{
    // Reference to Asset (id in assets collection)
    public string? AssetId { get; set; }

    // Either RoomId (appartamento) or WarehouseId (magazzino)
    public string? RoomId { get; set; }
    public string? WarehouseId { get; set; }

    public int Quantity { get; set; }
    public int? MinQuantityDefault { get; set; }
    public bool? RequiredOnCheckin { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public record AssetDto(string? Id, string? Name, string? Code, string? Description, DateTime? CreatedAt);
public record WarehouseDto(string? Id, string? Name, string? Code, string? Description, string? FloorNumber, string? HouseNumber, DateTime? CreatedAt);
public record InventoryDto(string? Id, string? AssetId, string? RoomId, string? WarehouseId, int Quantity, int? MinQuantityDefault, bool? RequiredOnCheckin, DateTime? CreatedAt);

public class NationStorageModel
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Code2 { get; set; }
    public string? Code3 { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class RegionStorageModel
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Code2 { get; set; }
    public string? Code3 { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class CityStorageModel
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Code2 { get; set; }
    public string? Code3 { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class IdentityDocumentTypeStorageModel
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Code2 { get; set; }
    public string? Code3 { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class FamilyRelationshipStorageModel
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Code2 { get; set; }
    public string? Code3 { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public record NationDto(string? Id, string? Name, string? Code, string? Code2, string? Code3, DateTime? CreatedAt);
public record RegionDto(string? Id, string? Name, string? Code, string? Code2, string? Code3, DateTime? CreatedAt);
public record CityDto(string? Id, string? Name, string? Code, string? Code2, string? Code3, DateTime? CreatedAt);
public record IdentityDocumentTypeDto(string? Id, string? Name, string? Code, string? Code2, string? Code3, DateTime? CreatedAt);
public record FamilyRelationshipDto(string? Id, string? Name, string? Code, string? Code2, string? Code3, DateTime? CreatedAt);

// --- Layer 1: Configuration entities ---

public class CashRegisterStorageModel
{
    public string? Name { get; set; }
    public string? Status { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class UserGroupStorageModel
{
    public string? Name { get; set; }
}

/// <summary>
/// Internal storage model for users.
/// PasswordHash and Salt are stored but NEVER returned in API responses.
/// The default admin user (id=1, username='admin') is seeded by PHP HotelDruid on install.
/// </summary>
public class UserStorageModel
{
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? Salt { get; set; }
    public string? PasswordType { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
}

public class SettingStorageModel
{
    public string? Key { get; set; }
    public int? UserId { get; set; }
    public string? StringValue { get; set; }
    public int? NumericValue { get; set; }
}

public record CashRegisterDto(string? Id, string? Name, string? Status, string? Code, string? Description, DateTime? CreatedAt);
public record UserGroupDto(string? Id, string? Name);

/// <summary>Read-only user DTO — never contains password or salt.</summary>
public record UserDto(string? Id, string? Username, string? PasswordType, DateTime? CreatedAt);

/// <summary>Write DTO for user create/update — accepts password in plaintext for hashing server-side.</summary>
public record UserWriteDto(string? Username, string? Password, string? PasswordType);

public record SettingDto(string? Key, int? UserId, string? StringValue, int? NumericValue);

// --- Layer 2: Guest and User management ---

public class ClientStorageModel
{
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? Nickname { get; set; }
    public string? Gender { get; set; }
    public string? Title { get; set; }
    public string? Language { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? BirthCity { get; set; }
    public string? BirthRegion { get; set; }
    public string? BirthNation { get; set; }
    public string? DocumentNumber { get; set; }
    public DateTime? DocumentExpiry { get; set; }
    public string? DocumentType { get; set; }
    public string? DocumentCity { get; set; }
    public string? DocumentRegion { get; set; }
    public string? DocumentNation { get; set; }
    public string? Nationality { get; set; }
    public string? Nation { get; set; }
    public string? Region { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? StreetNumber { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Phone2 { get; set; }
    public string? Phone3 { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Email2 { get; set; }
    public string? Email3 { get; set; }
    public string? TaxCode { get; set; }
    public string? VatNumber { get; set; }
    public string? Notes { get; set; }
    public int? MaxOrderNumber { get; set; }
    public string? CompanionIds { get; set; }
    public string? DocumentsSent { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class ClientDataStorageModel
{
    public string? ClientId { get; set; }
    public int? Number { get; set; }
    public string? Type { get; set; }
    public string? Text1 { get; set; }
    public string? Text2 { get; set; }
    public string? Text3 { get; set; }
    public string? Text4 { get; set; }
    public string? Text5 { get; set; }
    public string? Text6 { get; set; }
    public string? Text7 { get; set; }
    public string? Text8 { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class UserPrivilegeStorageModel
{
    public int? UserId { get; set; }
    public int? Year { get; set; }
    public string? AllowedRules { get; set; }
    public string? AllowedTariffs { get; set; }
    public string? AllowedExtraCosts { get; set; }
    public string? AllowedContracts { get; set; }
    public string? AllowedCashRegisters { get; set; }
    public string? PaymentCashRegister { get; set; }
    public string? BookingInsertPriv { get; set; }
    public string? BookingModifyPriv { get; set; }
    public string? PersonalSettingsPriv { get; set; }
    public string? ClientInsertPriv { get; set; }
    public string? ClientPrefix { get; set; }
    public string? CostInsertPriv { get; set; }
    public string? ViewTablePriv { get; set; }
    public string? TariffInsertPriv { get; set; }
    public string? RuleInsertPriv { get; set; }
    public string? MessagePriv { get; set; }
    public string? InventoryPriv { get; set; }
}

public class UserRelationStorageModel
{
    public int? UserId { get; set; }
    public int? NationId { get; set; }
    public int? RegionId { get; set; }
    public int? CityId { get; set; }
    public int? DocumentTypeId { get; set; }
    public int? FamilyRelationshipId { get; set; }
    public int? SuperiorId { get; set; }
    public int? IsDefault { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class GroupMembershipStorageModel
{
    public int? UserId { get; set; }
    public int? GroupId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public record ClientFullDto(string? Id, string? LastName, string? FirstName, string? Nickname, string? Gender, string? Title, string? Language, DateTime? DateOfBirth, string? BirthCity, string? BirthRegion, string? BirthNation, string? DocumentNumber, DateTime? DocumentExpiry, string? DocumentType, string? DocumentCity, string? DocumentRegion, string? DocumentNation, string? Nationality, string? Nation, string? Region, string? City, string? Street, string? StreetNumber, string? PostalCode, string? Phone, string? Phone2, string? Phone3, string? Fax, string? Email, string? Email2, string? Email3, string? TaxCode, string? VatNumber, string? Notes, int? MaxOrderNumber, string? CompanionIds, string? DocumentsSent, DateTime? CreatedAt, string? HostCreated, int? CreatedBy);

public record ClientDataDto(string? Id, string? ClientId, int? Number, string? Type, string? Text1, string? Text2, string? Text3, string? Text4, string? Text5, string? Text6, string? Text7, string? Text8, DateTime? CreatedAt, string? HostCreated, int? CreatedBy);

public record UserPrivilegeDto(string? Id, int? UserId, int? Year, string? AllowedRules, string? AllowedTariffs, string? AllowedExtraCosts, string? AllowedContracts, string? AllowedCashRegisters, string? PaymentCashRegister, string? BookingInsertPriv, string? BookingModifyPriv, string? PersonalSettingsPriv, string? ClientInsertPriv, string? ClientPrefix, string? CostInsertPriv, string? ViewTablePriv, string? TariffInsertPriv, string? RuleInsertPriv, string? MessagePriv, string? InventoryPriv);

public record UserRelationDto(string? Id, int? UserId, int? NationId, int? RegionId, int? CityId, int? DocumentTypeId, int? FamilyRelationshipId, int? SuperiorId, int? IsDefault, DateTime? CreatedAt, string? HostCreated, int? CreatedBy);

public record GroupMembershipDto(string? Id, int? UserId, int? GroupId, DateTime? CreatedAt, string? HostCreated, int? CreatedBy);

// --- Layer 3: Year-scoped Entities ---

public class YearStorageModel
{
    public string? PeriodType { get; set; }
}

public class PeriodStorageModel
{
    public int Year { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public double? Tariff1 { get; set; }
    public double? Tariff1PerPerson { get; set; }
    public double? Tariff2 { get; set; }
    public double? Tariff2PerPerson { get; set; }
    public double? Tariff3 { get; set; }
    public double? Tariff3PerPerson { get; set; }
    public double? Tariff4 { get; set; }
    public double? Tariff4PerPerson { get; set; }
    public double? Tariff5 { get; set; }
    public double? Tariff5PerPerson { get; set; }
    public double? Tariff6 { get; set; }
    public double? Tariff6PerPerson { get; set; }
    public double? Tariff7 { get; set; }
    public double? Tariff7PerPerson { get; set; }
    public double? Tariff8 { get; set; }
    public double? Tariff8PerPerson { get; set; }
    public double? Tariff9 { get; set; }
    public double? Tariff9PerPerson { get; set; }
    public double? Tariff10 { get; set; }
    public double? Tariff10PerPerson { get; set; }
    public double? Tariff11 { get; set; }
    public double? Tariff11PerPerson { get; set; }
    public double? Tariff12 { get; set; }
    public double? Tariff12PerPerson { get; set; }
}

public class TariffStorageModel
{
    public int Year { get; set; }
    public string? ExtraCostName { get; set; }
    public string? CostType { get; set; }
    public double? BaseValue { get; set; }
    public double? PercentageValue { get; set; }
    public double? TaxPercentage { get; set; }
    public string? Category { get; set; }
    public int? NumberLimit { get; set; }
    public string? Tariff1 { get; set; }
    public string? Tariff2 { get; set; }
    public string? Tariff3 { get; set; }
    public string? Tariff4 { get; set; }
    public string? Tariff5 { get; set; }
    public string? Tariff6 { get; set; }
    public string? Tariff7 { get; set; }
    public string? Tariff8 { get; set; }
    public string? Tariff9 { get; set; }
    public string? Tariff10 { get; set; }
    public string? Tariff11 { get; set; }
    public string? Tariff12 { get; set; }
    public string? AssignmentRules { get; set; }
}

public class AssignmentRuleStorageModel
{
    public int Year { get; set; }
    public string? RoomOrAgency { get; set; }
    public string? ClosedTariff { get; set; }
    public string? TariffPerRoom { get; set; }
    public int? StartPeriodId { get; set; }
    public int? EndPeriodId { get; set; }
    public string? Reason1 { get; set; }
    public string? Reason2 { get; set; }
    public string? Reason3 { get; set; }
    public string? Reason4 { get; set; }
    public string? Reason5 { get; set; }
}

public record YearDto(int Year, string? PeriodType);

public record PeriodDto(string? Id, int Year, DateOnly? StartDate, DateOnly? EndDate,
    double? Tariff1, double? Tariff1PerPerson,
    double? Tariff2, double? Tariff2PerPerson,
    double? Tariff3, double? Tariff3PerPerson,
    double? Tariff4, double? Tariff4PerPerson,
    double? Tariff5, double? Tariff5PerPerson,
    double? Tariff6, double? Tariff6PerPerson,
    double? Tariff7, double? Tariff7PerPerson,
    double? Tariff8, double? Tariff8PerPerson,
    double? Tariff9, double? Tariff9PerPerson,
    double? Tariff10, double? Tariff10PerPerson,
    double? Tariff11, double? Tariff11PerPerson,
    double? Tariff12, double? Tariff12PerPerson);

public record TariffDto(string? Id, int Year, string? ExtraCostName, string? CostType, double? BaseValue, double? PercentageValue, double? TaxPercentage, string? Category, int? NumberLimit);

public record AssignmentRuleDto(string? Id, int Year, string? RoomOrAgency, string? ClosedTariff, string? TariffPerRoom, int? StartPeriodId, int? EndPeriodId, string? Reason1);

// --- Layer 4: Transactional entity storage models ---

public class BookingStorageModel
{
    public int Year { get; set; }
    public string? ClientId { get; set; }
    public string? RoomId { get; set; }
    public DateOnly? ArrivalDate { get; set; }
    public DateOnly? DepartureDate { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

public class BookingCostStorageModel
{
    public int Year { get; set; }
    public string? BookingId { get; set; }
    public string? TariffId { get; set; }
    public double? Amount { get; set; }
    public string? Description { get; set; }
}

public class BookingGuestStorageModel
{
    public int Year { get; set; }
    public string? BookingId { get; set; }
    public string? ClientId { get; set; }
    public int? GuestNumber { get; set; }
}

public class CancelledBookingStorageModel
{
    public int Year { get; set; }
    public string? ClientId { get; set; }
    public string? RoomId { get; set; }
    public DateOnly? ArrivalDate { get; set; }
    public DateOnly? DepartureDate { get; set; }
    public string? Status { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? Notes { get; set; }
}

public class ExpenseStorageModel
{
    public int Year { get; set; }
    public string? CashRegisterId { get; set; }
    public double? Amount { get; set; }
    public string? Description { get; set; }
    public DateOnly? Date { get; set; }
}

public class MoneyHistoryStorageModel
{
    public int Year { get; set; }
    public string? CashRegisterId { get; set; }
    public double? Amount { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public DateOnly? Date { get; set; }
}

// --- Layer 5: Communication/Integration entities ---

public class MessageStorageModel
{
    public string? MessageType { get; set; }
    public string? Status { get; set; }
    public string? RecipientUserIds { get; set; }
    public string? SeenByUserIds { get; set; }
    public DateTime? SeenAt { get; set; }
    public string? Sender { get; set; }
    public string? Body { get; set; }
    public Dictionary<string, string?>? ExtraData { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class ContractTemplateStorageModel
{
    public int Number { get; set; }
    public string? Type { get; set; }
    public string? Content { get; set; }
}

public class ExternalIntegrationStorageModel
{
    public int? LocalId { get; set; }
    public string? RemoteId1 { get; set; }
    public string? RemoteId2 { get; set; }
    public string? IdType { get; set; }
    public string? IntegrationName { get; set; }
    public int? Year { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class SessionStorageModel
{
    public string? ClientToken { get; set; }
    public int? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? ConnectionType { get; set; }
    public string? UserAgent { get; set; }
    public DateTime? LastAccess { get; set; }
}

