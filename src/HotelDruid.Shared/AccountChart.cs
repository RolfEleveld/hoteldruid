namespace HotelDruid.Shared;

/// <summary>
/// Ledger buckets. A numbering standard assigns a code to each bucket.
/// Posting talks about buckets. The chart chosen in setup decides the numbers.
/// </summary>
public static class AccountRoles
{
    public const string Cash = "cash";
    public const string GuestLedger = "guest-ledger";
    public const string GuestDeposits = "guest-deposits";
    public const string RoomRevenue = "room-revenue";
    public const string OtherIncome = "other-income";
    public const string OperatingExpense = "operating-expense";

    public static readonly IReadOnlyList<string> All =
    [
        Cash,
        GuestLedger,
        GuestDeposits,
        RoomRevenue,
        OtherIncome,
        OperatingExpense
    ];
}

/// <summary>
/// A named chart. Fixed standards carry their bucket numbers.
/// Custom leaves the numbers for the hotel to register.
/// Add a later standard by appending a fixed entry to <see cref="AccountChart.Standards"/>.
/// </summary>
public sealed record AccountStandard(
    string Id,
    string Name,
    bool CustomNumbers,
    IReadOnlyDictionary<string, string>? Codes);

/// <summary>
/// The six bucket codes for one installation.
/// </summary>
public sealed class AccountChart
{
    public const string UsaliId = "usali";
    public const string SwissKmuId = "ch-kmu";
    public const string MexicoSatId = "mx-sat";
    public const string SpainPgcId = "es-pgc";
    public const string CustomId = "custom";

    private static readonly IReadOnlyDictionary<string, (string Name, string Group, string NormalBalance, string Description)> Buckets =
        new Dictionary<string, (string, string, string, string)>
        {
            [AccountRoles.Cash] = ("Cash", "asset", "debit", "Cash registers and hotel cash on hand."),
            [AccountRoles.GuestLedger] = ("Guest ledger", "asset", "debit", "Amounts owed by staying guests. A payment reduces this balance; it does not create revenue."),
            [AccountRoles.GuestDeposits] = ("Guest deposits", "liability", "credit", "Deposits held until earned or settled. Taking a deposit does not reduce room revenue."),
            [AccountRoles.RoomRevenue] = ("Room revenue", "revenue", "credit", "Earned room charges. Recognized when the stay is charged, not when cash is received."),
            [AccountRoles.OtherIncome] = ("Other income", "revenue", "credit", "Walk-up sales, extras, and petty-cash income that is not room revenue."),
            [AccountRoles.OperatingExpense] = ("Operating expense", "expense", "debit", "Hotel purchases and petty-cash outflows. The booking reference stays empty.")
        };

    private readonly Dictionary<string, string> _codes;

    private AccountChart(string standardId, IReadOnlyDictionary<string, string> codes)
    {
        StandardId = standardId;
        _codes = new Dictionary<string, string>(codes, StringComparer.OrdinalIgnoreCase);
        Accounts = BuildAccounts(_codes);
    }

    public string StandardId { get; }

    public string Cash => _codes[AccountRoles.Cash];
    public string GuestLedger => _codes[AccountRoles.GuestLedger];
    public string GuestDeposits => _codes[AccountRoles.GuestDeposits];
    public string RoomRevenue => _codes[AccountRoles.RoomRevenue];
    public string OtherIncome => _codes[AccountRoles.OtherIncome];
    public string OperatingExpense => _codes[AccountRoles.OperatingExpense];

    public IReadOnlyDictionary<string, string> Codes => _codes;

    public IReadOnlyList<ChartAccountDto> Accounts { get; }

    public static AccountChart Usali { get; } = new(UsaliId, new Dictionary<string, string>
    {
        [AccountRoles.Cash] = "1000",
        [AccountRoles.GuestLedger] = "1200",
        [AccountRoles.GuestDeposits] = "2400",
        [AccountRoles.RoomRevenue] = "4100",
        [AccountRoles.OtherIncome] = "4900",
        [AccountRoles.OperatingExpense] = "6100"
    });

    /// <summary>
    /// Schweizer Kontenrahmen KMU (school chart, kmu.admin.ch): 1000 Kasse,
    /// 1100 Debitoren, 2030 Anzahlungen von Dritten, 3400 Dienstleistungserlöse,
    /// 3600 Übrige Erlöse, 6700 Sonstiger betrieblicher Aufwand.
    /// </summary>
    public static AccountChart SwissKmu { get; } = new(SwissKmuId, new Dictionary<string, string>
    {
        [AccountRoles.Cash] = "1000",
        [AccountRoles.GuestLedger] = "1100",
        [AccountRoles.GuestDeposits] = "2030",
        [AccountRoles.RoomRevenue] = "3400",
        [AccountRoles.OtherIncome] = "3600",
        [AccountRoles.OperatingExpense] = "6700"
    });

    /// <summary>
    /// SAT código agrupador (Anexo 24): 101.01 Caja, 105.01 Clientes nacionales,
    /// 206.01 Anticipo de cliente nacional, 401.01 Ventas y/o servicios gravados a la tasa general,
    /// 403.01 Otros ingresos, 601.84 Otros gastos generales.
    /// </summary>
    public static AccountChart MexicoSat { get; } = new(MexicoSatId, new Dictionary<string, string>
    {
        [AccountRoles.Cash] = "101.01",
        [AccountRoles.GuestLedger] = "105.01",
        [AccountRoles.GuestDeposits] = "206.01",
        [AccountRoles.RoomRevenue] = "401.01",
        [AccountRoles.OtherIncome] = "403.01",
        [AccountRoles.OperatingExpense] = "601.84"
    });

    /// <summary>
    /// Plan General de Contabilidad (RD 1514/2007): 570 Caja, 430 Clientes,
    /// 438 Anticipos de clientes, 705 Prestaciones de servicios,
    /// 759 Ingresos por servicios diversos, 629 Otros servicios.
    /// </summary>
    public static AccountChart SpainPgc { get; } = new(SpainPgcId, new Dictionary<string, string>
    {
        [AccountRoles.Cash] = "570",
        [AccountRoles.GuestLedger] = "430",
        [AccountRoles.GuestDeposits] = "438",
        [AccountRoles.RoomRevenue] = "705",
        [AccountRoles.OtherIncome] = "759",
        [AccountRoles.OperatingExpense] = "629"
    });

    public static IReadOnlyList<AccountStandard> Standards { get; } =
    [
        new(UsaliId, "USALI", false, Usali.Codes),
        new(SwissKmuId, "Switzerland (Kontenrahmen KMU)", false, SwissKmu.Codes),
        new(MexicoSatId, "Mexico (SAT código agrupador)", false, MexicoSat.Codes),
        new(SpainPgcId, "Spain (PGC)", false, SpainPgc.Codes),
        new(CustomId, "Custom", true, null)
    ];

    public static bool TryResolve(
        string? standardId,
        IReadOnlyDictionary<string, string>? customCodes,
        out AccountChart chart,
        out string? errorCode)
    {
        var id = string.IsNullOrWhiteSpace(standardId) ? UsaliId : standardId.Trim();
        var standard = Standards.FirstOrDefault(item => string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase));
        if (standard is null)
        {
            chart = Usali;
            errorCode = "unknown-standard";
            return false;
        }

        if (!standard.CustomNumbers)
        {
            chart = new AccountChart(standard.Id, standard.Codes!);
            errorCode = null;
            return true;
        }

        return TryCustom(customCodes, out chart, out errorCode);
    }

    public static AccountChart ResolveOrUsali(string? standardId, IReadOnlyDictionary<string, string>? customCodes)
        => TryResolve(standardId, customCodes, out var chart, out _) ? chart : Usali;

    private static bool TryCustom(
        IReadOnlyDictionary<string, string>? customCodes,
        out AccountChart chart,
        out string? errorCode)
    {
        chart = Usali;
        if (customCodes is null)
        {
            errorCode = "missing-code";
            return false;
        }

        var codes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var role in AccountRoles.All)
        {
            if (!customCodes.TryGetValue(role, out var raw) || string.IsNullOrWhiteSpace(raw))
            {
                errorCode = "missing-code";
                return false;
            }

            var code = raw.Trim();
            if (code.Length > 16 || !IsAccountCode(code))
            {
                errorCode = "invalid-code";
                return false;
            }

            if (!seen.Add(code))
            {
                errorCode = "duplicate-code";
                return false;
            }

            codes[role] = code;
        }

        chart = new AccountChart(CustomId, codes);
        errorCode = null;
        return true;
    }

    private static bool IsAccountCode(string code)
    {
        if (code.Length == 0 || !char.IsLetterOrDigit(code[0]))
            return false;
        for (var i = 1; i < code.Length; i++)
        {
            var character = code[i];
            if (!char.IsLetterOrDigit(character) && character is not '.' and not '/' and not '_' and not '-')
                return false;
        }

        return true;
    }

    private static IReadOnlyList<ChartAccountDto> BuildAccounts(IReadOnlyDictionary<string, string> codes)
    {
        var accounts = new List<ChartAccountDto>(AccountRoles.All.Count);
        foreach (var role in AccountRoles.All)
        {
            var bucket = Buckets[role];
            accounts.Add(new ChartAccountDto(codes[role], bucket.Name, bucket.Group, bucket.NormalBalance, bucket.Description, role));
        }

        return accounts;
    }
}
