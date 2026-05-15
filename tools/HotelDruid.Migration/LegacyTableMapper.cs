namespace HotelDruid.Migration;

/// <summary>
/// Maps HotelDruid Italian table/column names to canonical English names.
/// Handles older versions that may be missing columns by returning null defaults.
/// </summary>
public static class LegacyTableMapper
{
    // All known HotelDruid tables: Italian name -> canonical name
    public static readonly Dictionary<string, string> TableNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["appartamenti"]          = "rooms",
        ["nazioni"]               = "nations",
        ["regioni"]               = "regions",
        ["citta"]                 = "cities",
        ["documentiid"]           = "identity-document-types",
        ["parentele"]             = "family-relationships",
        ["beniinventario"]        = "assets",
        ["magazzini"]             = "warehouses",
        ["relinventario"]         = "inventory",
        ["casse"]                 = "cash_registers",
        ["gruppi"]                = "user_groups",
        ["utenti"]                = "users",
        ["personalizza"]          = "settings",
        ["clienti"]               = "clients",
        ["relclienti"]            = "client_data",
        ["privilegi"]             = "user_privileges",
        ["relutenti"]             = "user_relations",
        ["relgruppi"]             = "group_memberships",
        ["anni"]                  = "years",
        ["periodi"]               = "periods",
        ["ntariffe"]              = "tariffs",
        ["regole"]                = "assignment_rules",
        ["prenotazioni"]          = "bookings",
        ["costiprenotazione"]     = "booking_costs",
        ["ospiti"]                = "booking_guests",
        ["prenotazioniannullate"] = "cancelled_bookings",
        ["spese"]                 = "expenses",
        ["movimenticassa"]        = "money_history",
        ["messaggi"]              = "messages",
        ["contratti"]             = "contract_templates",
        ["interconnessioni"]      = "external_integrations",
    };

    // Common Italian column name -> canonical column name
    public static readonly Dictionary<string, string> ColumnNames = new(StringComparer.OrdinalIgnoreCase)
    {
        // Rooms (appartamenti)
        ["idappartamento"]        = "room_id",
        ["idappartamenti"]        = "id",
        ["nomerappartamento"]     = "room_name",
        ["numpiano"]              = "floor_number",
        ["maxoccupanti"]          = "capacity",
        ["postiletto"]            = "beds",
        ["numcasa"]               = "house_number",
        ["app_vicini"]            = "neighboring_rooms",
        ["priorita"]              = "priority",
        ["priorita2"]             = "secondary_priority",
        ["letto"]                 = "has_beds",
        ["commento"]              = "comments",
        // Clients (clienti)
        ["idcliente"]             = "client_id",
        ["idclienti"]             = "id",
        ["nomecliente"]           = "client_name",
        ["cognomecliente"]        = "client_surname",
        ["cognome"]               = "last_name",
        ["nome"]                  = "first_name",
        ["email"]                 = "email",
        ["telefono"]              = "phone",
        ["cellulare"]             = "mobile",
        ["indirizzo"]             = "address",
        ["cap"]                   = "postal_code",
        ["nazione"]               = "nation",
        // Nations / Regions
        ["idnazione"]             = "nation_id",
        ["idnazioni"]             = "id",
        ["nomenazione"]           = "nation_name",
        ["idregione"]             = "region_id",
        ["idregioni"]             = "id",
        ["nomeregione"]           = "region_name",
        // Bookings
        ["idprenotazione"]        = "booking_id",
        ["idprenotazioni"]        = "id",
        ["datainizio"]            = "start_date",
        ["datafine"]              = "end_date",
        ["orainizio"]             = "start_time",
        ["orafine"]               = "end_time",
        ["numadulti"]             = "adults",
        ["numbambini"]            = "children",
        ["prezzo"]                = "price",
        ["stato"]                 = "status",
        ["nota"]                  = "notes",
        // Users
        ["idutente"]              = "user_id",
        ["idutenti"]              = "id",
        ["nomeutente"]            = "username",
        ["password"]              = "password_hash",
        ["livello"]               = "role",
        ["abilitato"]             = "enabled",
        // Assets / Inventory / Warehouses
        ["idbene"]                = "asset_id",
        ["idmagazzino"]           = "warehouse_id",
        ["quantita"]              = "quantity",
        ["descrizione"]           = "description",
        ["categoria"]             = "category",
    };

    public static string MapTable(string italianName, string prefix)
    {
        var stripped = prefix.Length > 0 && italianName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? italianName[prefix.Length..]
            : italianName;
        return TableNames.TryGetValue(stripped, out var canonical) ? canonical : stripped;
    }

    public static string MapColumn(string columnName)
        => ColumnNames.TryGetValue(columnName, out var canonical) ? canonical : columnName;

    // Returns list of known table source names (with optional prefix)
    public static IEnumerable<string> KnownSourceTables(string prefix)
        => TableNames.Keys.Select(t => prefix + t);
}

