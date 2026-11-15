using HotelDruid.Api.Models;
using HotelDruid.Api.Services.ExportImport;
using Xunit;

namespace HotelDruid.Api.Tests.Unit.ExportImport;

public class CanonicalReferencesTests
{
    [Fact]
    public void Apply_RewritesStoreIdsToNamesAndQualifiesARepeatedRoom()
    {
        var tables = new Dictionary<string, CanonicalData>
        {
            ["rooms"] = new("rooms", 2, new()
            {
                new() { ["id"] = "ek6nv6lggweubnew24bkpq7oci", ["name"] = "101", ["house_number"] = "A", ["neighboring_rooms"] = "wrmtrezt72zujou2r6avuahsge" },
                new() { ["id"] = "wrmtrezt72zujou2r6avuahsge", ["name"] = "101", ["house_number"] = "B" }
            }, new()),
            ["clients"] = new("clients", 1, new()
            {
                new() { ["blazorId"] = "11111111-1111-1111-1111-111111111111", ["indexName"] = "Bauer_1", ["lastName"] = "Bauer", ["firstName"] = "Felix" }
            }, new()),
            ["tariffs"] = new("tariffs", 1, new()
            {
                new() { ["blazorId"] = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", ["name"] = "Rack" }
            }, new()),
            ["bookings"] = new("bookings", 1, new()
            {
                new()
                {
                    ["id"] = "bookingbookingbookingbookingb",
                    ["blazorId"] = "bookingbookingbookingbookingb",
                    ["year"] = 2026,
                    ["client_id"] = "11111111-1111-1111-1111-111111111111",
                    ["room_id"] = "ek6nv6lggweubnew24bkpq7oci",
                    ["tariff_id"] = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                    ["arrival_date"] = "2026-11-10",
                    ["departure_date"] = "2026-11-11"
                }
            }, new()),
            ["booking_costs"] = new("booking_costs", 1, new()
            {
                new() { ["blazorId"] = "22222222-2222-2222-2222-222222222222", ["bookingId"] = "bookingbookingbookingbookingb", ["tariffId"] = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" }
            }, new())
        };

        CanonicalReferences.Apply(tables);

        Assert.Equal("A/101", tables["rooms"].Rows[0]["id"]);
        Assert.Equal("B/101", tables["rooms"].Rows[0]["neighboring_rooms"]);
        Assert.Equal("Bauer, Felix", tables["clients"].Rows[0]["id"]);
        Assert.Equal("Bauer, Felix", tables["clients"].Rows[0]["blazorId"]);
        Assert.Equal("Rack", tables["tariffs"].Rows[0]["id"]);
        Assert.Equal("Bauer, Felix|A/101|2026-11-10|2026-11-11", tables["bookings"].Rows[0]["id"]);
        Assert.Equal("A/101", tables["bookings"].Rows[0]["room_id"]);
        Assert.Equal("Bauer, Felix", tables["bookings"].Rows[0]["client_id"]);
        Assert.Equal("Rack", tables["bookings"].Rows[0]["tariff_id"]);
        Assert.Equal("Bauer, Felix|A/101|2026-11-10|2026-11-11", tables["booking_costs"].Rows[0]["bookingId"]);
        Assert.Equal("Rack", tables["booking_costs"].Rows[0]["tariffId"]);
        Assert.Equal("cost|Bauer, Felix|A/101|2026-11-10|2026-11-11", tables["booking_costs"].Rows[0]["id"]);
        Assert.Equal("cost|Bauer, Felix|A/101|2026-11-10|2026-11-11", tables["booking_costs"].Rows[0]["blazorId"]);
    }
}
