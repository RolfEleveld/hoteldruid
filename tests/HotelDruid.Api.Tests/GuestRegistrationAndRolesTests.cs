using HotelDruid.Api.Models;
using HotelDruid.Api.Services.ExportImport;
using HotelDruid.Shared;

namespace HotelDruid.Api.Tests;

public class GuestRegistrationAndRolesTests
{
    [Fact]
    public void Two_arriving_guests_need_two_names()
    {
        var profile = GuestRegistration.BuiltIn(GuestRegistration.ItalyId);
        var gaps = GuestRegistration.Gaps(profile, new StayFacts
        {
            Label = "Room 101",
            Arrived = true,
            ActualGuests = 2,
            Lead = new GuestFacts { LastName = "Rossi", FirstName = "Anna", Gender = "female", DateOfBirth = new DateTime(1990, 1, 1), BirthCity = "Rome", Nationality = "IT", DocumentType = "passport", DocumentNumber = "A1", DocumentCity = "Rome" }
        });

        Assert.Contains(gaps, gap => gap.Contains("1 names for 2 guests"));
    }

    [Fact]
    public void Dutch_register_keeps_one_name_on_the_room()
    {
        var profile = GuestRegistration.BuiltIn(GuestRegistration.NetherlandsId);
        var gaps = GuestRegistration.Gaps(profile, new StayFacts
        {
            Label = "Room 102",
            Arrived = true,
            ActualGuests = 4,
            Lead = new GuestFacts { LastName = "Jansen", City = "Amsterdam", DocumentType = "passport" }
        });

        Assert.DoesNotContain(gaps, gap => gap.Contains("names for"));
    }

    [Fact]
    public void Latest_year_wins_and_messages_is_not_a_desk_role()
    {
        var early = new UserPrivilegeStorageModel { UserId = 4, Year = 2024, AllowedRules = "1" };
        var later = new UserPrivilegeStorageModel { UserId = 4, Year = 2026, AllowedCashRegisters = "1" };
        var rows = new[]
        {
            new RoleConfirmation(4, 2024, PrivilegeRoles.Propose(early, out _), "rooms"),
            new RoleConfirmation(4, 2026, PrivilegeRoles.Propose(later, out _), "cash")
        };

        var chosen = PrivilegeRoles.Latest(rows);

        Assert.Equal("clerk", Assert.Single(chosen).Role);
    }
}
