using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    /// <summary>
    /// Room API client service
    /// </summary>
    public interface IRoomApiService
    {
        Task<List<RoomDto>> GetRoomsAsync();
        Task<RoomDto?> GetRoomAsync(string id);
        Task<string> CreateRoomAsync(RoomDto room);
        Task ExportRoomsAsync(List<string> roomIds, string exportType = "full");
        Task<ExportStatusResponse> GetExportStatusAsync(string exportId);
        Task<ImportValidationResponse> ValidateImportAsync(Stream zipFileStream);
        Task<ImportExecuteResponse> ExecuteImportAsync(Stream zipFileStream, bool overwrite = false);
    }

    /// <summary>
    /// Room Data Transfer Object
    /// </summary>
    public record RoomDto(
        string Id,
        string Name,
        int Capacity,
        int? FloorNumber,
        string? HouseNumber,
        int Priority = 0,
        int SecondaryPriority = 0,
        bool HasBeds = true,
        List<string>? NeighboringRooms = null,
        string? Comments = null
    );

    /// <summary>
    /// Export Request
    /// </summary>
    public record ExportRequest(
        List<string> RoomIds,
        string ExportType = "full"
    );

    /// <summary>
    /// Export Response
    /// </summary>
    public record ExportResponse(
        string ExportId,
        string Status,
        DateTime? CompletedAt = null,
        string? DownloadUrl = null
    );

    /// <summary>
    /// Export Status Response
    /// </summary>
    public record ExportStatusResponse(
        string ExportId,
        string Status,
        int? Progress = null,
        DateTime? CompletedAt = null,
        string? DownloadUrl = null,
        string? ErrorMessage = null
    );

    /// <summary>
    /// Import Validation Response
    /// </summary>
    public record ImportValidationResponse(
        bool IsValid,
        List<string> Errors = null!,
        List<string> Warnings = null!,
        int RoomsToImport = 0,
        int RoomsToUpdate = 0
    );

    /// <summary>
    /// Import Execute Response
    /// </summary>
    public record ImportExecuteResponse(
        string ImportId,
        string Status,
        int RoomsImported = 0,
        int RoomsUpdated = 0,
        List<string> Errors = null!,
        string? ErrorMessage = null
    );

    /// <summary>
    /// Room API Service Implementation
    /// </summary>
    public class RoomApiService : IRoomApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        private static readonly System.Text.Json.JsonSerializerOptions _jsonOpts = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public RoomApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get all rooms
        /// </summary>
        public async Task<List<RoomDto>> GetRoomsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/rooms");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var rooms = System.Text.Json.JsonSerializer.Deserialize<List<RoomDto>>(content);
                
                return rooms ?? new List<RoomDto>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting rooms: {ex.Message}");
                return new List<RoomDto>();
            }
        }

        /// <summary>
        /// Get a specific room by ID
        /// </summary>
        public async Task<RoomDto?> GetRoomAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/rooms/{id}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<RoomDto>(content, _jsonOpts);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting room {id}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create or update a room
        /// </summary>
        public async Task<string> CreateRoomAsync(RoomDto room)
        {
            try
            {
                var content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(room),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/rooms", content);
                response.EnsureSuccessStatusCode();
                
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<RoomDto>(responseContent, _jsonOpts);
                
                return result?.Id ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error creating room: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Export rooms to ZIP file
        /// </summary>
        public async Task ExportRoomsAsync(List<string> roomIds, string exportType = "full")
        {
            try
            {
                var request = new ExportRequest(roomIds, exportType);
                var content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/export/rooms", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error exporting rooms: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get export status
        /// </summary>
        public async Task<ExportStatusResponse> GetExportStatusAsync(string exportId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/export/status/{exportId}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<ExportStatusResponse>(content, _jsonOpts)
                    ?? new ExportStatusResponse(exportId, "unknown");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting export status: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Validate import ZIP file
        /// </summary>
        public async Task<ImportValidationResponse> ValidateImportAsync(Stream zipFileStream)
        {
            try
            {
                using var content = new StreamContent(zipFileStream);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");

                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/import/validate", content);
                response.EnsureSuccessStatusCode();
                
                var responseContent = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<ImportValidationResponse>(responseContent, _jsonOpts)
                    ?? new ImportValidationResponse(false, new(), new());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error validating import: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Execute import from ZIP file
        /// </summary>
        public async Task<ImportExecuteResponse> ExecuteImportAsync(Stream zipFileStream, bool overwrite = false)
        {
            try
            {
                using var content = new StreamContent(zipFileStream);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");

                var url = $"{ApiBaseUrl}/import/execute?overwrite={overwrite}";
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                
                var responseContent = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<ImportExecuteResponse>(responseContent, _jsonOpts)
                    ?? new ImportExecuteResponse("", "unknown");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error executing import: {ex.Message}");
                throw;
            }
        }
    }
}

