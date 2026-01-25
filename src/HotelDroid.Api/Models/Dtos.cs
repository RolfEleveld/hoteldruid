namespace HotelDroid.Api.Models;

public record ClientDto(int Id, string Name, string? Email, string? Phone);
public record BookingDto(int Id, int ClientId, string ArrivalDate, string DepartureDate, string Status);
