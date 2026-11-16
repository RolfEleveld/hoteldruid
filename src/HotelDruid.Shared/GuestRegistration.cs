namespace HotelDruid.Shared;

/// <summary>
/// How many named guests a room must carry, and which fields each name must hold.
/// A standard is chosen the way a chart of accounts is chosen. Custom is the same
/// lists with the boxes left for the hotel.
/// </summary>
public sealed class RegistrationProfile
{
    public string Id { get; set; } = "none";
    public string Name { get; set; } = "None";
    /// <summary>One named guest for the room. Otherwise one named guest for each person in scope.</summary>
    public bool OneNamePerRoom { get; set; } = true;
    /// <summary>all, foreign, or booker.</summary>
    public string Scope { get; set; } = "booker";
    /// <summary>Nationality code treated as local when scope is foreign. Blank means every named guest is in scope.</summary>
    public string? HomeCode { get; set; }
    public bool PrintCard { get; set; }
    public bool DigitalReport { get; set; }
    public bool ShowOnNightAudit { get; set; }
    public bool AllowCoveredByLead { get; set; }
    public List<string> LeadFields { get; set; } = [];
    public List<string> AttachedFields { get; set; } = [];
}

public sealed class GuestFacts
{
    public string? LastName { get; init; }
    public string? FirstName { get; init; }
    public string? Gender { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? BirthCity { get; init; }
    public string? BirthNation { get; init; }
    public string? Nationality { get; init; }
    public string? SecondNationality { get; init; }
    public string? Street { get; init; }
    public string? City { get; init; }
    public string? Nation { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? DocumentType { get; init; }
    public string? DocumentNumber { get; init; }
    public string? DocumentCity { get; init; }
    public string? DocumentNation { get; init; }
    public bool CoveredByLead { get; init; }

    public string DisplayName
    {
        get
        {
            var name = string.Join(' ', new[] { FirstName, LastName }.Where(part => !string.IsNullOrWhiteSpace(part)));
            return string.IsNullOrWhiteSpace(name) ? "guest" : name;
        }
    }
}

public sealed class StayFacts
{
    public string Label { get; init; } = "";
    public bool Arrived { get; init; }
    public int? ExpectedGuests { get; init; }
    public int? ActualGuests { get; init; }
    public DateTime? EntryTime { get; init; }
    public GuestFacts? Lead { get; init; }
    public IReadOnlyList<GuestFacts> Attached { get; init; } = [];
}

public static class GuestRegistration
{
    public const string NoneId = "none";
    public const string SwitzerlandForeignId = "ch-foreign";
    public const string SwitzerlandLocalId = "ch-local";
    public const string ItalyId = "it-alloggiati";
    public const string FranceId = "fr";
    public const string GermanyId = "de";
    public const string SpainId = "es";
    public const string NetherlandsId = "nl";
    public const string MexicoCityId = "mx-cdmx";
    public const string CustomId = "custom";

    public static IReadOnlyList<(string Id, string Name)> Standards { get; } =
    [
        (NoneId, "None"),
        (SwitzerlandForeignId, "Switzerland, foreign guests"),
        (SwitzerlandLocalId, "Switzerland, local"),
        (ItalyId, "Italy, Alloggiati"),
        (FranceId, "France"),
        (GermanyId, "Germany"),
        (SpainId, "Spain"),
        (NetherlandsId, "Netherlands"),
        (MexicoCityId, "Mexico City"),
        (CustomId, "Custom")
    ];

    public static RegistrationProfile Resolve(string? id, RegistrationProfile? custom)
    {
        if (string.Equals(id, CustomId, StringComparison.OrdinalIgnoreCase))
            return custom ?? BlankCustom();
        return BuiltIn(id);
    }

    public static RegistrationProfile BuiltIn(string? id) => (id ?? "").Trim().ToLowerInvariant() switch
    {
        SwitzerlandForeignId => new RegistrationProfile
        {
            Id = SwitzerlandForeignId,
            Name = "Switzerland, foreign guests",
            OneNamePerRoom = false,
            Scope = "foreign",
            HomeCode = "CH",
            PrintCard = true,
            ShowOnNightAudit = true,
            LeadFields = ["name", "birthDate", "documentType", "documentNumber"],
            AttachedFields = ["name", "birthDate", "documentType", "documentNumber"]
        },
        SwitzerlandLocalId => new RegistrationProfile
        {
            Id = SwitzerlandLocalId,
            Name = "Switzerland, local",
            OneNamePerRoom = false,
            Scope = "all",
            PrintCard = true,
            ShowOnNightAudit = true,
            LeadFields = ["name", "birthDate"],
            AttachedFields = ["name", "birthDate"]
        },
        ItalyId => new RegistrationProfile
        {
            Id = ItalyId,
            Name = "Italy, Alloggiati",
            OneNamePerRoom = false,
            Scope = "all",
            DigitalReport = true,
            ShowOnNightAudit = true,
            AllowCoveredByLead = true,
            LeadFields = ["name", "gender", "birthDate", "birthPlace", "nationality", "documentType", "documentNumber", "documentIssue"],
            AttachedFields = ["name", "gender", "birthDate", "birthPlace", "nationality"]
        },
        FranceId => new RegistrationProfile
        {
            Id = FranceId,
            Name = "France",
            OneNamePerRoom = false,
            Scope = "foreign",
            HomeCode = "FR",
            PrintCard = true,
            ShowOnNightAudit = true,
            AllowCoveredByLead = true,
            LeadFields = ["name", "birthDate", "birthPlace", "nationality", "address", "phone", "email"],
            AttachedFields = ["name", "birthDate", "birthPlace", "nationality", "address"]
        },
        GermanyId => new RegistrationProfile
        {
            Id = GermanyId,
            Name = "Germany",
            OneNamePerRoom = false,
            Scope = "foreign",
            HomeCode = "DE",
            PrintCard = true,
            ShowOnNightAudit = true,
            LeadFields = ["name", "birthDate", "nationality", "address", "documentNumber"],
            AttachedFields = ["name", "birthDate", "nationality"]
        },
        SpainId => new RegistrationProfile
        {
            Id = SpainId,
            Name = "Spain",
            OneNamePerRoom = false,
            Scope = "all",
            DigitalReport = true,
            ShowOnNightAudit = true,
            LeadFields = ["name", "birthDate", "nationality", "documentType", "documentNumber"],
            AttachedFields = ["name", "birthDate", "nationality", "documentType", "documentNumber"]
        },
        NetherlandsId => new RegistrationProfile
        {
            Id = NetherlandsId,
            Name = "Netherlands",
            OneNamePerRoom = true,
            Scope = "booker",
            ShowOnNightAudit = true,
            LeadFields = ["name", "address", "documentType"]
        },
        MexicoCityId => new RegistrationProfile
        {
            Id = MexicoCityId,
            Name = "Mexico City",
            OneNamePerRoom = false,
            Scope = "all",
            ShowOnNightAudit = true,
            LeadFields = ["name", "address", "entryTime"],
            AttachedFields = ["name", "address"]
        },
        _ => new RegistrationProfile()
    };

    public static RegistrationProfile BlankCustom() => new()
    {
        Id = CustomId,
        Name = "Custom",
        OneNamePerRoom = true,
        Scope = "booker",
        ShowOnNightAudit = true
    };

    public static IReadOnlyList<string> Gaps(RegistrationProfile profile, StayFacts stay)
    {
        if (!profile.ShowOnNightAudit && !profile.DigitalReport)
            return [];

        var gaps = new List<string>();
        var names = Named(stay);
        var required = RequiredNames(profile, stay, names);
        if (names.Count < required)
            gaps.Add($"{stay.Label}: {names.Count} names for {required} guests");

        var leadFields = profile.LeadFields;
        if (stay.Lead is not null && InScope(profile, stay.Lead))
            gaps.AddRange(Missing(profile, stay.Label, stay.Lead, leadFields, isLead: true));

        foreach (var guest in stay.Attached)
        {
            if (!InScope(profile, guest))
                continue;
            if (guest.CoveredByLead && profile.AllowCoveredByLead)
            {
                if (string.IsNullOrWhiteSpace(guest.LastName))
                    gaps.Add($"{stay.Label}, guest: name is missing");
                continue;
            }
            gaps.AddRange(Missing(profile, stay.Label, guest, profile.AttachedFields.Count > 0 ? profile.AttachedFields : leadFields, isLead: false));
        }

        if (profile.LeadFields.Contains("entryTime") && stay.EntryTime is null && names.Count > 0)
            gaps.Add($"{stay.Label}: time of entry is missing");

        return gaps;
    }

    public static int RequiredNames(RegistrationProfile profile, StayFacts stay, IReadOnlyList<GuestFacts>? names = null)
    {
        if (profile.OneNamePerRoom || string.Equals(profile.Scope, "booker", StringComparison.OrdinalIgnoreCase))
            return 1;
        var count = stay.Arrived
            ? stay.ActualGuests ?? stay.ExpectedGuests ?? 1
            : stay.ExpectedGuests ?? stay.ActualGuests ?? 1;
        return Math.Max(1, count);
    }

    private static List<GuestFacts> Named(StayFacts stay)
    {
        var names = new List<GuestFacts>();
        if (stay.Lead is not null && !string.IsNullOrWhiteSpace(stay.Lead.LastName))
            names.Add(stay.Lead);
        names.AddRange(stay.Attached.Where(guest => !string.IsNullOrWhiteSpace(guest.LastName)));
        return names;
    }

    private static bool InScope(RegistrationProfile profile, GuestFacts guest)
    {
        if (!string.Equals(profile.Scope, "foreign", StringComparison.OrdinalIgnoreCase))
            return true;
        if (string.IsNullOrWhiteSpace(profile.HomeCode) || string.IsNullOrWhiteSpace(guest.Nationality))
            return true;
        return !guest.Nationality.Trim().Equals(profile.HomeCode, StringComparison.OrdinalIgnoreCase);
    }

    private static IEnumerable<string> Missing(RegistrationProfile profile, string label, GuestFacts guest, IReadOnlyList<string> fields, bool isLead)
    {
        foreach (var field in fields)
        {
            if (field == "entryTime")
                continue;
            if (!Has(guest, field))
                yield return $"{label}, {guest.DisplayName}: {FieldLabel(field)} is missing";
        }
    }

    public static bool Has(GuestFacts guest, string field) => field switch
    {
        "name" => !string.IsNullOrWhiteSpace(guest.LastName),
        "gender" => !string.IsNullOrWhiteSpace(guest.Gender),
        "birthDate" => guest.DateOfBirth.HasValue,
        "birthPlace" => !string.IsNullOrWhiteSpace(guest.BirthCity) || !string.IsNullOrWhiteSpace(guest.BirthNation),
        "nationality" => !string.IsNullOrWhiteSpace(guest.Nationality),
        "secondNationality" => !string.IsNullOrWhiteSpace(guest.SecondNationality),
        "address" => !string.IsNullOrWhiteSpace(guest.City) || !string.IsNullOrWhiteSpace(guest.Street),
        "phone" => !string.IsNullOrWhiteSpace(guest.Phone),
        "email" => !string.IsNullOrWhiteSpace(guest.Email),
        "documentType" => !string.IsNullOrWhiteSpace(guest.DocumentType),
        "documentNumber" => !string.IsNullOrWhiteSpace(guest.DocumentNumber),
        "documentIssue" => !string.IsNullOrWhiteSpace(guest.DocumentCity) || !string.IsNullOrWhiteSpace(guest.DocumentNation),
        _ => true
    };

    public static string FieldLabel(string field) => field switch
    {
        "birthDate" => "date of birth",
        "birthPlace" => "place of birth",
        "secondNationality" => "second nationality",
        "documentType" => "document type",
        "documentNumber" => "document number",
        "documentIssue" => "place of issue",
        "entryTime" => "time of entry",
        _ => field
    };
}
