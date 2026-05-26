using System;
using System.Collections.Generic;

namespace HotelDruid.Shared.Configuration
{
    public class TokenIdentityConfiguration
    {
        public string TokenId { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public List<string> AllowedMethods { get; set; } = new();
        public List<string> AllowedPathPrefixes { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
    }

    public class CurrentTokenIdentity
    {
        public string TokenId { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public bool IsAssigned { get; set; }
    }
}
