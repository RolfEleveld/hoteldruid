using System;
using System.Collections.Generic;

namespace HotelDruid.Shared.Configuration
{
    public class SystemConfiguration
    {
        public string Id { get; set; } = "system";
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
        public string? DefaultCurrency { get; set; }
        public int? DefaultYear { get; set; }
        public List<string>? AdminEmails { get; set; }
        public Dictionary<string, string>? Settings { get; set; }
        // Add more configuration fields as needed
    }
}
