using HotelDruid.Shared;

namespace HotelDruid.Api.Tests.Unit;

public class StayTaxTests
{
    [Fact]
    public void Inclusive_KeepsTheGuestAmount()
    {
        var (guest, net, tax) = StayTax.Split(108.10m, 8.1m, inclusive: true);
        Assert.Equal(108.10m, guest);
        Assert.Equal(100.00m, net);
        Assert.Equal(8.10m, tax);
    }

    [Fact]
    public void Exclusive_AddsTheTax()
    {
        var (guest, net, tax) = StayTax.Split(100m, 8.1m, inclusive: false);
        Assert.Equal(100m, net);
        Assert.Equal(8.10m, tax);
        Assert.Equal(108.10m, guest);
    }

    [Fact]
    public void CityTax_IsPerPersonNight()
    {
        Assert.Equal(9.00m, StayTax.CityTax(2, 2, "PersonNight", 2.25m));
        Assert.Equal(0m, StayTax.CityTax(2, 2, "None", 2.25m));
    }
}
