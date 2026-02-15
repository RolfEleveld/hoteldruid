namespace HotelDroid.Api.Models;

public record ClientDto(int Id, string Name, string? Email, string? Phone);
public record BookingDto(int Id, int ClientId, string ArrivalDate, string DepartureDate, string Status);

public record RoomDto(string? Id, string Name, int Capacity, string RoomType, decimal PricePerNight);

// Internal storage model (explicit fields to avoid defaults)
public class RoomStorageModel
{
    public string? Name { get; set; }
    public int? Capacity { get; set; }
    public string? RoomType { get; set; }
    public decimal? PricePerNight { get; set; }
}
