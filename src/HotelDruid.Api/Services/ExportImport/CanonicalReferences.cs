using System.Text.Json;
using HotelDruid.Api.Models;

namespace HotelDruid.Api.Services.ExportImport;

/// <summary>
/// Export speaks in canonical names. Store ids stay inside the hotel.
/// A file may still contain an old id; import uses it only to reconnect rows, then mints a new id.
/// </summary>
public static class CanonicalReferences
{
    private static readonly string[] IdentityKeys = ["id", "blazorId", "indexName"];
    private static readonly string[] ReferenceKeys =
    [
        "room_id", "client_id", "tariff_id", "asset_id", "warehouse_id",
        "cash_register_id", "booking_id", "target_id", "source_id", "journal_id",
        "neighboring_rooms", "companion_ids", "app_vicini"
    ];

    public static bool IsStoreId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var text = value.Trim();
        if (Guid.TryParse(text, out _))
            return true;
        if (text.Length == 32 && text.All(Uri.IsHexDigit))
            return true;
        return text.Length == 26 && text.All(c => (c is >= 'a' and <= 'z') || (c is >= '2' and <= '7'));
    }

    public static void Apply(IDictionary<string, CanonicalData> tables)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        LabelRooms(Find(tables, "rooms"), map);
        LabelByName(Find(tables, "assets"), map);
        LabelByName(Find(tables, "warehouses"), map);
        LabelByName(Find(tables, "tariffs"), map);
        LabelByName(Find(tables, "cash_registers"), map);
        LabelClients(Find(tables, "clients"), map);
        LabelBookings(Find(tables, "bookings"), map);
        LabelOwned(Find(tables, "booking_costs"), map, "cost", row => Join(Resolve(Text(row, "bookingId", "booking_id"), map), Text(row, "description"), Text(row, "amount")));
        LabelOwned(Find(tables, "booking_guests"), map, "guest", row => Join(Resolve(Text(row, "bookingId", "booking_id"), map), Resolve(Text(row, "clientId", "client_id"), map), Text(row, "guestNumber", "guest_number")));
        LabelOwned(Find(tables, "expenses"), map, "expense", row => Join(DateText(Text(row, "date")), Text(row, "description"), Text(row, "amount")));
        LabelOwned(Find(tables, "cancelled_bookings"), map, "cancelled", row => Join(Resolve(Text(row, "bookingId", "booking_id"), map), Resolve(Text(row, "clientId", "client_id"), map), DateText(Text(row, "arrivalDate", "arrival_date")), DateText(Text(row, "departureDate", "departure_date"))));
        LabelJournals(Find(tables, "ledger_entries"), map);

        foreach (var table in tables.Values)
            Rewrite(table, map);
    }

    private static void LabelRooms(CanonicalData? table, Dictionary<string, string> map)
    {
        if (table is null)
            return;

        var counts = table.Rows
            .GroupBy(row => Text(row, "name") ?? "", StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);

        foreach (var row in table.Rows)
        {
            var name = Text(row, "name") ?? "";
            if (name.Length == 0)
                continue;
            var house = Text(row, "house_number", "houseNumber") ?? "";
            var label = counts.GetValueOrDefault(name) > 1 && house.Length > 0 ? $"{house}/{name}" : name;
            RememberIdentity(row, label, map);
        }
    }

    private static void LabelByName(CanonicalData? table, Dictionary<string, string> map)
    {
        if (table is null)
            return;

        foreach (var row in table.Rows)
        {
            var name = Text(row, "name");
            if (string.IsNullOrWhiteSpace(name))
                continue;
            RememberIdentity(row, name.Trim(), map);
        }
    }

    private static void LabelClients(CanonicalData? table, Dictionary<string, string> map)
    {
        if (table is null)
            return;

        var labels = table.Rows.Select(ClientLabel).ToList();
        var counts = labels.GroupBy(label => label, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);
        var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < table.Rows.Count; i++)
        {
            var label = labels[i];
            if (counts.GetValueOrDefault(label) > 1)
            {
                var email = Text(table.Rows[i], "email") ?? "";
                var next = seen.GetValueOrDefault(label) + 1;
                seen[label] = next;
                label = email.Length > 0 && next == 1 ? $"{label} <{email}>" : $"{label} #{next}";
            }

            if (label.Length == 0)
                continue;
            RememberIdentity(table.Rows[i], label, map);
        }
    }

    private static void LabelBookings(CanonicalData? table, Dictionary<string, string> map)
    {
        if (table is null)
            return;

        var labels = new List<string>();
        foreach (var row in table.Rows)
        {
            var client = Resolve(Text(row, "client_id", "clientId"), map);
            var room = Resolve(Text(row, "room_id", "roomId"), map);
            var arrival = DateText(Text(row, "arrival_date", "arrivalDate"));
            var departure = DateText(Text(row, "departure_date", "departureDate"));
            labels.Add($"{client}|{room}|{arrival}|{departure}");
        }

        var counts = labels.GroupBy(label => label, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);
        var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < table.Rows.Count; i++)
        {
            var label = labels[i];
            if (counts.GetValueOrDefault(label) > 1)
            {
                var next = seen.GetValueOrDefault(label) + 1;
                seen[label] = next;
                label = $"{label} #{next}";
            }

            RememberIdentity(table.Rows[i], label, map);
            Set(table.Rows[i], "client_id", Resolve(Text(table.Rows[i], "client_id", "clientId"), map));
            Set(table.Rows[i], "room_id", Resolve(Text(table.Rows[i], "room_id", "roomId"), map));
            Set(table.Rows[i], "tariff_id", Resolve(Text(table.Rows[i], "tariff_id", "tariffId"), map));
        }
    }

    private static void LabelOwned(
        CanonicalData? table,
        Dictionary<string, string> map,
        string prefix,
        Func<Dictionary<string, object?>, string> body)
    {
        if (table is null)
            return;

        var labels = table.Rows.Select(row =>
        {
            var text = body(row);
            return string.IsNullOrWhiteSpace(text) ? prefix : $"{prefix}|{text}";
        }).ToList();
        var counts = labels.GroupBy(label => label, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);
        var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < table.Rows.Count; i++)
        {
            var label = labels[i];
            if (counts.GetValueOrDefault(label) > 1)
            {
                var next = seen.GetValueOrDefault(label) + 1;
                seen[label] = next;
                label = $"{label} #{next}";
            }

            RememberIdentity(table.Rows[i], label, map);
        }
    }

    private static void LabelJournals(CanonicalData? table, Dictionary<string, string> map)
    {
        if (table is null)
            return;

        var seen = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var number = 0;
        foreach (var row in table.Rows)
        {
            var current = Text(row, "journalId", "journal_id");
            if (string.IsNullOrWhiteSpace(current) || !IsStoreId(current))
                continue;
            if (!seen.TryGetValue(current, out var label))
            {
                number++;
                var when = DateText(Text(row, "timestamp"));
                label = string.IsNullOrWhiteSpace(when) ? $"journal|{number}" : $"journal|{when}|{number}";
                seen[current] = label;
            }

            map[current] = label;
        }
    }

    private static string Join(params string?[] parts)
        => string.Join("|", parts.Where(part => !string.IsNullOrWhiteSpace(part)).Select(part => part!.Trim()));

    private static void Rewrite(CanonicalData table, Dictionary<string, string> map)
    {
        foreach (var row in table.Rows)
        {
            foreach (var key in row.Keys.ToList())
            {
                if (!IsIdentity(key) && !IsReference(key))
                    continue;

                var text = AsText(row[key]);
                if (text is null)
                    continue;

                var list = key.Contains("neighbor", StringComparison.OrdinalIgnoreCase)
                    || key.Contains("companion", StringComparison.OrdinalIgnoreCase)
                    || key.Contains("vicini", StringComparison.OrdinalIgnoreCase);
                row[key] = list
                    ? RewriteList(text, map, IsIdentity(key))
                    : RewriteToken(text, map, IsIdentity(key));
            }
        }
    }

    private static string? RewriteToken(string token, Dictionary<string, string> map, bool identity)
    {
        var resolved = Resolve(token, map);
        if (identity && IsStoreId(resolved))
            return null;
        return string.IsNullOrWhiteSpace(resolved) ? null : resolved;
    }

    private static string? RewriteList(string raw, Dictionary<string, string> map, bool identity)
    {
        var parts = raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(part => RewriteToken(part, map, identity))
            .Where(part => !string.IsNullOrWhiteSpace(part));
        var joined = string.Join(",", parts);
        return string.IsNullOrWhiteSpace(joined) ? null : joined;
    }

    private static void RememberIdentity(Dictionary<string, object?> row, string label, Dictionary<string, string> map)
    {
        foreach (var key in IdentityKeys)
        {
            var current = Text(row, key);
            if (!string.IsNullOrWhiteSpace(current))
                map[current] = label;
        }

        var name = Text(row, "name");
        if (!string.IsNullOrWhiteSpace(name) && !map.ContainsKey(name))
            map[name] = label;

        Set(row, "id", label);
        if (Text(row, "blazorId") is not null)
            Set(row, "blazorId", label);
        if (Text(row, "indexName") is not null)
            Set(row, "indexName", label);
    }

    private static string ClientLabel(Dictionary<string, object?> row)
    {
        var last = Text(row, "lastName", "last_name") ?? "";
        var first = Text(row, "firstName", "first_name") ?? "";
        if (last.Length > 0 && first.Length > 0)
            return $"{last}, {first}";
        if (last.Length > 0)
            return last;
        return Text(row, "email") ?? "";
    }

    private static string Resolve(string? token, Dictionary<string, string> map)
    {
        if (string.IsNullOrWhiteSpace(token))
            return "";
        var text = token.Trim();
        return map.TryGetValue(text, out var label) ? label : text;
    }

    private static string DateText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";
        return DateTime.TryParse(value, out var parsed)
            ? parsed.ToString("yyyy-MM-dd")
            : value.Trim();
    }

    private static CanonicalData? Find(IDictionary<string, CanonicalData> tables, string name)
    {
        if (tables.TryGetValue(name, out var direct))
            return direct;
        return tables.Values.FirstOrDefault(table =>
            string.Equals(table.TableName, name, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsIdentity(string key)
        => IdentityKeys.Any(name => key.Equals(name, StringComparison.OrdinalIgnoreCase));

    private static bool IsReference(string key)
        => ReferenceKeys.Any(name => key.Equals(name, StringComparison.OrdinalIgnoreCase))
            || key.Equals("RoomId", StringComparison.OrdinalIgnoreCase)
            || key.Equals("ClientId", StringComparison.OrdinalIgnoreCase)
            || key.Equals("TariffId", StringComparison.OrdinalIgnoreCase)
            || key.Equals("AssetId", StringComparison.OrdinalIgnoreCase)
            || key.Equals("WarehouseId", StringComparison.OrdinalIgnoreCase)
            || key.Equals("CashRegisterId", StringComparison.OrdinalIgnoreCase)
            || key.Equals("BookingId", StringComparison.OrdinalIgnoreCase)
            || key.Equals("TargetId", StringComparison.OrdinalIgnoreCase)
            || key.Equals("SourceId", StringComparison.OrdinalIgnoreCase)
            || key.Equals("JournalId", StringComparison.OrdinalIgnoreCase)
            || key.Equals("NeighboringRooms", StringComparison.OrdinalIgnoreCase)
            || key.Equals("CompanionIds", StringComparison.OrdinalIgnoreCase);

    private static string? Text(Dictionary<string, object?> row, params string[] names)
    {
        foreach (var name in names)
        {
            foreach (var (key, value) in row)
            {
                if (key.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return AsText(value);
            }
        }

        return null;
    }

    private static void Set(Dictionary<string, object?> row, string name, string? value)
    {
        var key = row.Keys.FirstOrDefault(existing => existing.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? name;
        row[key] = value;
    }

    private static string? AsText(object? value) => value switch
    {
        null => null,
        string text => string.IsNullOrWhiteSpace(text) ? null : text.Trim(),
        JsonElement element when element.ValueKind == JsonValueKind.String => element.GetString()?.Trim(),
        JsonElement element when element.ValueKind == JsonValueKind.Null => null,
        _ => value.ToString()?.Trim()
    };
}
