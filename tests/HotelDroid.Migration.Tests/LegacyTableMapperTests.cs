using HotelDroid.Migration;
using Xunit;

namespace HotelDroid.Migration.Tests;

public class LegacyTableMapperTests
{
    [Theory]
    [InlineData("appartamenti", "rooms")]
    [InlineData("clienti", "clients")]
    [InlineData("magazzini", "warehouses")]
    [InlineData("beniinventario", "assets")]
    [InlineData("relinventario", "inventory")]
    [InlineData("casse", "cash_registers")]
    [InlineData("utenti", "users")]
    [InlineData("gruppi", "user_groups")]
    [InlineData("nazioni", "nations")]
    [InlineData("regioni", "regions")]
    public void MapTable_KnownItalianName_ReturnsCanonical(string italian, string expected)
    {
        var result = LegacyTableMapper.MapTable(italian, "");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void MapTable_WithPrefix_StripsPrefix()
    {
        // "hd_appartamenti" with prefix "hd_" -> maps "appartamenti" -> "rooms"
        var result = LegacyTableMapper.MapTable("hd_appartamenti", "hd_");
        Assert.Equal("rooms", result);
    }

    [Fact]
    public void MapTable_UnknownTable_ReturnsOriginalName()
    {
        var result = LegacyTableMapper.MapTable("some_unknown_table", "");
        Assert.Equal("some_unknown_table", result);
    }

    [Theory]
    [InlineData("id", "id")]
    [InlineData("idappartamento", "room_id")]
    [InlineData("idcliente", "client_id")]
    [InlineData("nomecliente", "client_name")]
    [InlineData("cognomecliente", "client_surname")]
    [InlineData("idnazione", "nation_id")]
    public void MapColumn_KnownItalianColumn_ReturnsCanonical(string italian, string expected)
    {
        var result = LegacyTableMapper.MapColumn(italian);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void MapColumn_UnknownColumn_ReturnsOriginalName()
    {
        var result = LegacyTableMapper.MapColumn("some_custom_col");
        Assert.Equal("some_custom_col", result);
    }

    [Fact]
    public void KnownSourceTables_ReturnsAtLeast20Tables()
    {
        var tables = LegacyTableMapper.KnownSourceTables("").ToList();
        Assert.True(tables.Count >= 20, $"Expected >= 20 known tables, got {tables.Count}");
    }

    [Fact]
    public void KnownSourceTables_WithPrefix_PrefixesAllNames()
    {
        var tables = LegacyTableMapper.KnownSourceTables("hd_").ToList();
        Assert.All(tables, t => Assert.StartsWith("hd_", t));
    }
}
