using System;
using System.Collections.Generic;
using HotelDruid.Shared;

namespace HotelDruid.Shared.Configuration
{
    public class SystemConfiguration
    {
        public string Id { get; set; } = "system";
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
        public string? DefaultCurrency { get; set; }
        public int? DefaultYear { get; set; }
        public string? AccountingStandard { get; set; }
        public Dictionary<string, string>? AccountCodes { get; set; }
        public string? RegistrationStandard { get; set; }
        public RegistrationProfile? RegistrationCustom { get; set; }
        public bool MessageAutomation { get; set; }
        public Dictionary<string, int>? InvoiceSequence { get; set; }
        public List<string>? AdminEmails { get; set; }
        public Dictionary<string, string>? Settings { get; set; }
        // Add more configuration fields as needed
    }
}
