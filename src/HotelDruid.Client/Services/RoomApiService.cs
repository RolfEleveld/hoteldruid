using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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
        bool IncludeConfigs = true,
        bool IncludeDocs = true,
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
        int? ProgressPercent = null,
        DateTime? CreatedAt = null,
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
        int RoomsToUpdate = 0,
        string? PackageId = null
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
        string? ErrorMessage = null,
        string? StatusUrl = null
    );

    /// <summary>
    /// Room API Service Implementation
    /// </summary>
    public class RoomApiService : IRoomApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        private static readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };

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
                var apiRooms = JsonSerializer.Deserialize<List<ApiRoomDto>>(content, _jsonOpts);
                var rooms = apiRooms?.Select(MapToClientDto).ToList();
                
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
                var apiRoom = JsonSerializer.Deserialize<ApiRoomDto>(content, _jsonOpts);
                return apiRoom is null ? null : MapToClientDto(apiRoom);
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
                    JsonSerializer.Serialize(MapToApiDto(room)),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/rooms", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiRoomDto>(responseContent, _jsonOpts);
                
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
                var request = new ExportRequest(IncludeConfigs: true, IncludeDocs: true, ExportType: exportType);
                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/export/create", content);
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
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/export/{exportId}/status");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ExportStatusResponse>(content, _jsonOpts)
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
                using var fileContent = new StreamContent(zipFileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");
                using var form = new MultipartFormDataContent();
                form.Add(fileContent, "file", "rooms-import.zip");

                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/import/validate", form);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var api = JsonSerializer.Deserialize<ApiImportValidationResponse>(responseContent, _jsonOpts);
                var roomTable = api?.Tables?.FirstOrDefault(t => string.Equals(t.Name, "rooms", StringComparison.OrdinalIgnoreCase));
                var rowsToImport = roomTable?.RowCount ?? 0;
                return new ImportValidationResponse(
                    IsValid: api?.Valid ?? false,
                    Errors: api?.Errors ?? new List<string>(),
                    Warnings: new List<string>(),
                    RoomsToImport: rowsToImport,
                    RoomsToUpdate: 0,
                    PackageId: api?.PackageId
                );
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
                var validation = await ValidateImportAsync(zipFileStream);
                if (!validation.IsValid || string.IsNullOrWhiteSpace(validation.PackageId))
                {
                    return new ImportExecuteResponse(
                        ImportId: string.Empty,
                        Status: "failed",
                        RoomsImported: 0,
                        RoomsUpdated: 0,
                        Errors: validation.Errors,
                        ErrorMessage: "Import package validation failed",
                        StatusUrl: null
                    );
                }

                var payload = new
                {
                    PackageId = validation.PackageId,
                    DryRun = false,
                    MappingOverrides = (Dictionary<string, object>?)null
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{ApiBaseUrl}/import/{validation.PackageId}/execute",
                    payload,
                    _jsonOpts);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ImportExecuteResponse>(responseContent, _jsonOpts)
                    ?? new ImportExecuteResponse("", "unknown");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error executing import: {ex.Message}");
                throw;
            }
        }

        private record ApiImportValidationResponse(
            bool Valid,
            string PackageId,
            List<ApiTablePreview>? Tables,
            List<string>? Errors
        );

        private record ApiTablePreview(string Name, int RowCount);

        private record ApiRoomDto(
            string? Id,
            string? Name,
            int Capacity,
            string? FloorNumber,
            string? HouseNumber,
            int? Priority,
            int? SecondaryPriority,
            string? HasBeds,
            string? NeighboringRooms,
            string? Comments
        );

        private static RoomDto MapToClientDto(ApiRoomDto api)
        {
            int? floorNumber = int.TryParse(api.FloorNumber, out var parsedFloor) ? parsedFloor : null;
            var hasBeds = string.Equals(api.HasBeds, "S", StringComparison.OrdinalIgnoreCase);
            var neighboringRooms = string.IsNullOrWhiteSpace(api.NeighboringRooms)
                ? null
                : api.NeighboringRooms
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();

            return new RoomDto(
                Id: api.Id ?? string.Empty,
                Name: api.Name ?? string.Empty,
                Capacity: api.Capacity,
                FloorNumber: floorNumber,
                HouseNumber: api.HouseNumber,
                Priority: api.Priority ?? 0,
                SecondaryPriority: api.SecondaryPriority ?? 0,
                HasBeds: hasBeds,
                NeighboringRooms: neighboringRooms,
                Comments: api.Comments
            );
        }

        private static ApiRoomDto MapToApiDto(RoomDto room)
        {
            var hasBeds = room.HasBeds ? "S" : "N";
            var neighboringRooms = room.NeighboringRooms is { Count: > 0 }
                ? string.Join(",", room.NeighboringRooms)
                : null;

            return new ApiRoomDto(
                Id: string.IsNullOrWhiteSpace(room.Id) ? null : room.Id,
                Name: room.Name,
                Capacity: room.Capacity,
                FloorNumber: room.FloorNumber?.ToString(),
                HouseNumber: room.HouseNumber,
                Priority: room.Priority,
                SecondaryPriority: room.SecondaryPriority,
                HasBeds: hasBeds,
                NeighboringRooms: neighboringRooms,
                Comments: room.Comments
            );
        }
    }
}

