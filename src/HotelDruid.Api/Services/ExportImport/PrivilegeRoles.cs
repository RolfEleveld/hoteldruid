using HotelDruid.Api.Models;

namespace HotelDruid.Api.Services.ExportImport;

public sealed record RoleConfirmation(int? UserId, int Year, string Role, string Reason);

/// <summary>
/// Turns one PHP allow-list into one of the four desk roles.
/// The messages role is never proposed. It belongs to the service client.
/// </summary>
public static class PrivilegeRoles
{
    public static string Propose(UserPrivilegeStorageModel row, out string reason)
    {
        var cash = row.AllowedCashRegisters;
        var pay = row.PaymentCashRegister;
        var tariffs = row.AllowedTariffs;
        var extras = row.AllowedExtraCosts;
        var rules = row.AllowedRules;
        var contracts = row.AllowedContracts;

        if (IsUnrestricted(cash) || IsUnrestricted(tariffs) || IsUnrestricted(rules) || ContainsAdmin(cash) || ContainsAdmin(rules))
        {
            reason = "The PHP allow-list was unrestricted.";
            return "administrator";
        }

        if (Has(cash) || Has(pay) || Has(tariffs) || Has(extras))
        {
            reason = "The PHP allow-list included cash or tariffs.";
            return "clerk";
        }

        if (Has(rules) || Has(contracts))
        {
            reason = "The PHP allow-list included rooms and not cash.";
            return "room-service";
        }

        reason = "The PHP allow-list did not include a change to the hotel.";
        return "reports";
    }

    public static List<RoleConfirmation> Latest(IEnumerable<RoleConfirmation> rows)
    {
        var chosen = new Dictionary<int, RoleConfirmation>();
        foreach (var row in rows)
        {
            if (row.UserId is null)
                continue;
            if (!chosen.TryGetValue(row.UserId.Value, out var current) || row.Year >= current.Year)
                chosen[row.UserId.Value] = row;
        }
        return chosen.Values.OrderBy(row => row.UserId).ToList();
    }

    private static bool Has(string? value) => !string.IsNullOrWhiteSpace(value) && !IsNone(value);

    private static bool IsNone(string value)
    {
        var text = value.Trim();
        return text.Equals("n", StringComparison.OrdinalIgnoreCase)
            || text.Equals("none", StringComparison.OrdinalIgnoreCase)
            || text.Equals("nessuno", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsUnrestricted(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        var text = value.Trim();
        return text.Equals("tutte", StringComparison.OrdinalIgnoreCase)
            || text.Equals("tutti", StringComparison.OrdinalIgnoreCase)
            || text.Equals("all", StringComparison.OrdinalIgnoreCase)
            || text == "*"
            || text.Equals("s", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ContainsAdmin(string? value)
        => value is not null && value.Contains("admin", StringComparison.OrdinalIgnoreCase);
}
