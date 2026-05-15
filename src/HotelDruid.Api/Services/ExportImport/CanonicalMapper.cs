using System.Text.Json;
using HotelDruid.Api.Models;
using HotelDruid.Shared;

namespace HotelDruid.Api.Services.ExportImport;

/// <summary>
/// Maps between .NET objects and HotelDruid canonical JSON format
/// Implements: Inward Liberal, Outward Conservative
/// - Accepts any reasonable table/field names on input
/// - Normalizes to canonical form internally
/// - Exports with industry-standard refined names
/// </summary>
public class CanonicalMapper : ICanonicalMapper
{
    private readonly NamingConfiguration _naming;
    private readonly ILogger<CanonicalMapper> _logger;

    // Default naming configuration
    private static readonly NamingConfiguration DefaultNaming = new(
        TableAliases: new()
        {
            // Rooms
            ["appartamenti"] = "rooms",
            ["apartments"] = "rooms",
            ["rooms"] = "rooms",
            ["appartment"] = "rooms",
            ["apartment"] = "rooms",

            // Assets / Inventory (italian → canonical)
            ["beniinventario"] = "assets",
            ["beniinventario"] = "assets",
            ["magazzini"] = "warehouses",
            ["relinventario"] = "inventory",

            // English forms
            ["assets"] = "assets",
            ["warehouses"] = "warehouses",
            ["inventory"] = "inventory",

            // Reference data (italian → canonical)
            ["nazioni"] = "nations",
            ["regioni"] = "regions",
            ["citta"] = "cities",
            ["documentiid"] = "identity-document-types",
            ["parentele"] = "family-relationships",

            // Reference data (english → canonical)
            ["nations"] = "nations",
            ["regions"] = "regions",
            ["cities"] = "cities",
            ["identity-document-types"] = "identity-document-types",
            ["family-relationships"] = "family-relationships",

            // Layer 1: Configuration entities (italian → canonical)
            ["casse"] = "cash_registers",
            ["cash_registers"] = "cash_registers",
            ["gruppi"] = "user_groups",
            ["user_groups"] = "user_groups",
            ["utenti"] = "users",
            ["users"] = "users",
            ["personalizza"] = "settings",
            ["settings"] = "settings",

            // Layer 2: Guest and User management (italian → canonical)
            ["clienti"] = "clients",
            ["clients"] = "clients",
            ["relclienti"] = "client_data",
            ["client_data"] = "client_data",
            ["privilegi"] = "user_privileges",
            ["user_privileges"] = "user_privileges",
            ["relutenti"] = "user_relations",
            ["user_relations"] = "user_relations",
            ["relgruppi"] = "group_memberships",
            ["group_memberships"] = "group_memberships",
            ["anni"] = "years",
            ["years"] = "years",
            ["periodi"] = "periods",
            ["periods"] = "periods",
            ["ntariffe"] = "tariffs",
            ["tariffs"] = "tariffs",
            ["regole"] = "assignment_rules",
            ["assignment_rules"] = "assignment_rules",

            // Layer 4: Transactional entities (italian → canonical)
            ["prenotazioni"] = "bookings",
            ["bookings"] = "bookings",
            ["costiprenotazione"] = "booking_costs",
            ["booking_costs"] = "booking_costs",
            ["ospiti"] = "booking_guests",
            ["booking_guests"] = "booking_guests",
            ["prenotazioniannullate"] = "cancelled_bookings",
            ["cancelled_bookings"] = "cancelled_bookings",
            ["spese"] = "expenses",
            ["expenses"] = "expenses",
            ["movimenticassa"] = "money_history",
            ["money_history"] = "money_history",

            // Layer 5: Communication/Integration entities (italian → canonical)
            ["messaggi"] = "messages",
            ["messages"] = "messages",
            ["contratti"] = "contract_templates",
            ["contract_templates"] = "contract_templates",
            ["interconnessioni"] = "external_integrations",
            ["external_integrations"] = "external_integrations",
            ["sessioni"] = "sessions",
            ["sessions"] = "sessions"
        },
        FieldAliases: new()
        {
            // HotelDruid Italian → Canonical (rooms)
            ["idappartamenti"] = "id",
            ["numpiano"] = "floor_number",
            ["maxoccupanti"] = "capacity",
            ["numcasa"] = "house_number",
            ["app_vicini"] = "neighboring_rooms",
            ["priorita"] = "priority",
            ["priorita2"] = "secondary_priority",
            ["letto"] = "has_beds",
            ["commento"] = "comments",

            // Rooms - English variants
            ["room_id"] = "id",
            ["floor"] = "floor_number",
            ["max_occupancy"] = "capacity",
            ["house_id"] = "house_number",
            ["neighbors"] = "neighboring_rooms",
            ["notes"] = "comments",
            ["beds"] = "has_beds",

            // API names (pass-through)
            ["id"] = "id",
            ["floor_number"] = "floor_number",
            ["capacity"] = "capacity",
            ["house_number"] = "house_number",
            ["neighboring_rooms"] = "neighboring_rooms",
            ["priority"] = "priority",
            ["secondary_priority"] = "secondary_priority",
            ["has_beds"] = "has_beds",
            ["comments"] = "comments",
            ["name"] = "name",

            // Assets (italian)
            ["idbeniinventario"] = "id",
            ["nome_bene"] = "name",
            ["codice_bene"] = "code",
            ["descrizione_bene"] = "description",
            ["datainserimento"] = "created_at",
            ["hostinserimento"] = "host_created",
            ["utente_inserimento"] = "created_by",

            // Warehouses (italian)
            ["idmagazzini"] = "id",
            ["nome_magazzino"] = "name",
            ["codice_magazzino"] = "code",
            ["descrizione_magazzino"] = "description",
            ["numpiano"] = "floor_number",
            ["numcasa"] = "house_number",

            // Inventory (italian)
            ["idbeneinventario"] = "asset_id",
            ["idappartamento"] = "room_id",
            ["idmagazzino"] = "warehouse_id",
            ["quantita"] = "quantity",
            ["quantita_min_predef"] = "min_quantity_default",
            ["richiesto_checkin"] = "required_on_checkin",

            // Nations (italian → canonical)
            ["idnazioni"] = "id",
            ["nome_nazione"] = "name",
            ["codice_nazione"] = "code",
            ["codice2_nazione"] = "code2",
            ["codice3_nazione"] = "code3",

            // Regions (italian → canonical)
            ["idregioni"] = "id",
            ["nome_regione"] = "name",
            ["codice_regione"] = "code",
            ["codice2_regione"] = "code2",
            ["codice3_regione"] = "code3",

            // Cities (italian → canonical)
            ["idcitta"] = "id",
            ["nome_citta"] = "name",
            ["codice_citta"] = "code",
            ["codice2_citta"] = "code2",
            ["codice3_citta"] = "code3",

            // Identity document types (italian → canonical)
            ["iddocumentiid"] = "id",
            ["nome_documentoid"] = "name",
            ["codice_documentoid"] = "code",
            ["codice2_documentoid"] = "code2",
            ["codice3_documentoid"] = "code3",

            // Family relationships (italian → canonical)
            ["idparentele"] = "id",
            ["nome_parentela"] = "name",
            ["codice_parentela"] = "code",
            ["codice2_parentela"] = "code2",
            ["codice3_parentela"] = "code3",

            // Cash Registers (italian → canonical)
            ["idcasse"] = "id",
            ["nome_cassa"] = "name",
            ["stato"] = "status",
            ["codice_cassa"] = "code",
            ["descrizione_cassa"] = "description",

            // User Groups (italian → canonical)
            ["idgruppi"] = "id",
            ["nome_gruppo"] = "name",

            // Users (italian → canonical)
            ["idutenti"] = "id",
            ["nome_utente"] = "username",
            ["tipo_pass"] = "password_type",

            // Settings (italian → canonical)
            ["idpersonalizza"] = "key",
            ["idutente"] = "user_id",
            ["valpersonalizza"] = "string_value",
            ["valpersonalizza_num"] = "numeric_value",

            // Clients (italian → canonical)
            ["idclienti"] = "id",
            ["cognome"] = "last_name",
            ["nome"] = "first_name",
            ["soprannome"] = "nickname",
            ["sesso"] = "gender",
            ["titolo"] = "title",
            ["lingua"] = "language",
            ["datanascita"] = "date_of_birth",
            ["cittanascita"] = "birth_city",
            ["regionenascita"] = "birth_region",
            ["nazionenascita"] = "birth_nation",
            ["documento"] = "document_number",
            ["scadenzadoc"] = "document_expiry",
            ["tipodoc"] = "document_type",
            ["nazione"] = "nation",
            ["regione"] = "region",
            ["citta_res"] = "city",
            ["via"] = "street",
            ["numcivico"] = "street_number",
            ["cap"] = "postal_code",
            ["telefono"] = "phone",
            ["email"] = "email",
            ["cod_fiscale"] = "tax_code",
            ["partita_iva"] = "vat_number",

            // Client pass-throughs
            ["last_name"] = "last_name",
            ["first_name"] = "first_name",
            ["date_of_birth"] = "date_of_birth",
            ["birth_city"] = "birth_city",
            ["birth_region"] = "birth_region",
            ["birth_nation"] = "birth_nation",
            ["document_number"] = "document_number",
            ["document_expiry"] = "document_expiry",
            ["document_type"] = "document_type",
            ["document_nation"] = "document_nation",
            ["nationality"] = "nationality",
            ["street"] = "street",
            ["street_number"] = "street_number",
            ["postal_code"] = "postal_code",
            ["phone"] = "phone",
            ["tax_code"] = "tax_code",
            ["vat_number"] = "vat_number",

            // ClientData (italian → canonical)
            ["relclienti_id"] = "client_id",
            ["numero"] = "number",
            ["tipo"] = "type",
            ["testo1"] = "text1",
            ["testo2"] = "text2",
            ["testo3"] = "text3",
            ["testo4"] = "text4",
            ["testo5"] = "text5",
            ["testo6"] = "text6",
            ["testo7"] = "text7",
            ["testo8"] = "text8",

            // ClientData pass-throughs
            ["client_id"] = "client_id",
            ["number"] = "number",
            ["type"] = "type",
            ["text1"] = "text1",
            ["text2"] = "text2",
            ["text3"] = "text3",
            ["text4"] = "text4",
            ["text5"] = "text5",
            ["text6"] = "text6",
            ["text7"] = "text7",
            ["text8"] = "text8",

            // UserPrivileges (italian → canonical)
            ["anno"] = "year",
            ["regole1_consentite"] = "allowed_rules",
            ["tariffe_consentite"] = "allowed_tariffs",
            ["casse_consentite"] = "allowed_cash_registers",

            // UserPrivileges pass-throughs
            ["user_id"] = "user_id",
            ["year"] = "year",
            ["allowed_rules"] = "allowed_rules",
            ["allowed_tariffs"] = "allowed_tariffs",
            ["allowed_cash_registers"] = "allowed_cash_registers",

            // UserRelations (italian → canonical)
            ["idnazione"] = "nation_id",
            ["idregione"] = "region_id",
            ["idcitta"] = "city_id",
            ["iddocumentoid"] = "document_type_id",
            ["idparentela"] = "family_relationship_id",
            ["idsup"] = "superior_id",
            ["predef"] = "is_default",

            // UserRelations pass-throughs
            ["nation_id"] = "nation_id",
            ["region_id"] = "region_id",
            ["city_id"] = "city_id",
            ["document_type_id"] = "document_type_id",
            ["family_relationship_id"] = "family_relationship_id",
            ["superior_id"] = "superior_id",
            ["is_default"] = "is_default",

            // GroupMemberships (italian → canonical)
            ["idgruppo"] = "group_id",

            // GroupMemberships pass-throughs
            ["group_id"] = "group_id",

            // Years (italian → canonical)
            ["tipoper"] = "period_type",

            // Years pass-throughs
            ["period_type"] = "period_type",

            // Periods (italian → canonical)
            ["inizioper"] = "start_date",
            ["fineper"] = "end_date",
            ["tper1"] = "tariff_1",
            ["tper1pp"] = "tariff_1_per_person",

            // Periods pass-throughs
            ["start_date"] = "start_date",
            ["end_date"] = "end_date",
            ["tariff_1"] = "tariff_1",
            ["tariff_1_per_person"] = "tariff_1_per_person",

            // Tariffs (italian → canonical)
            ["nomecosto_ca"] = "extra_cost_name",
            ["tipocosto_ca"] = "cost_type",
            ["valoreb_ca"] = "base_value",
            ["valorep_ca"] = "percentage_value",
            ["aliq_ca"] = "tax_percentage",
            ["categoria_ca"] = "category",
            ["numlimite_ca"] = "number_limit",

            // Tariffs pass-throughs
            ["extra_cost_name"] = "extra_cost_name",
            ["cost_type"] = "cost_type",
            ["base_value"] = "base_value",
            ["percentage_value"] = "percentage_value",
            ["tax_percentage"] = "tax_percentage",
            ["category"] = "category",
            ["number_limit"] = "number_limit",

            // AssignmentRules (italian → canonical)
            ["camoeag_ra"] = "room_or_agency",
            ["tariccl_ra"] = "closed_tariff",
            ["taricam_ra"] = "tariff_per_room",
            ["inizioper_ra"] = "start_period_id",
            ["fineper_ra"] = "end_period_id",
            ["motivo1_ra"] = "reason_1",

            // AssignmentRules pass-throughs
            ["room_or_agency"] = "room_or_agency",
            ["closed_tariff"] = "closed_tariff",
            ["tariff_per_room"] = "tariff_per_room",
            ["start_period_id"] = "start_period_id",
            ["end_period_id"] = "end_period_id",
            ["reason_1"] = "reason_1",

            // Messages (italian → canonical)
            ["idmessaggi"] = "message_id",
            ["tipo_messaggio"] = "message_type",
            ["idutenti_visto"] = "seen_by_user_ids",
            ["datavisione"] = "seen_at",
            ["mittente"] = "sender",
            ["testo"] = "body",

            // Messages pass-throughs
            ["message_type"] = "message_type",
            ["seen_by_user_ids"] = "seen_by_user_ids",
            ["seen_at"] = "seen_at",
            ["sender"] = "sender",
            ["body"] = "body",
            ["recipient_user_ids"] = "recipient_user_ids",

            // ContractTemplates pass-throughs
            ["content"] = "content",
            ["numero"] = "number",

            // ExternalIntegrations (italian → canonical)
            ["idlocale"] = "local_id",
            ["idremoto1"] = "remote_id1",
            ["idremoto2"] = "remote_id2",
            ["tipoid"] = "id_type",
            ["nome_ic"] = "integration_name",

            // ExternalIntegrations pass-throughs
            ["local_id"] = "local_id",
            ["remote_id1"] = "remote_id1",
            ["remote_id2"] = "remote_id2",
            ["id_type"] = "id_type",
            ["integration_name"] = "integration_name",

            // Sessions (italian → canonical)
            ["idsessioni"] = "session_id",
            ["idcliente"] = "client_token",
            ["indirizzo_ip"] = "ip_address",
            ["tipo_conn"] = "connection_type",
            ["user_agent"] = "user_agent",
            ["ultimo_accesso"] = "last_access",

            // Sessions pass-throughs
            ["session_id"] = "session_id",
            ["client_token"] = "client_token",
            ["ip_address"] = "ip_address",
            ["connection_type"] = "connection_type",
            ["last_access"] = "last_access"
        },
        ExportNames: new ExportNamingConfig(
            TableName: "rooms",  // Standard export name
            Fields: new()
            {
                ["id"] = "id",
                ["floor_number"] = "floor_number",
                ["capacity"] = "capacity",
                ["house_number"] = "house_number",
                ["neighboring_rooms"] = "neighboring_rooms",
                ["priority"] = "priority",
                ["secondary_priority"] = "secondary_priority",
                ["has_beds"] = "has_beds",
                ["comments"] = "comments",
                ["name"] = "name"
            }
        )
    );

    private const string StandardTableName = "rooms";
    private const string HotelDruidTableName = "appartamenti";
    private const string AssetsTableName = "assets";
    private const string AssetsHotelDruidName = "beniinventario";
    private const string WarehousesTableName = "warehouses";
    private const string WarehousesHotelDruidName = "magazzini";
    private const string InventoryTableName = "inventory";
    private const string InventoryHotelDruidName = "relinventario";

    public CanonicalMapper(ILogger<CanonicalMapper> logger, NamingConfiguration? naming = null)
    {
        _logger = logger;
        _naming = naming ?? DefaultNaming;
    }

    public string NormalizeTableName(string inputTableName)
    {
        var lower = inputTableName?.ToLowerInvariant().Trim() ?? "";
        
        if (_naming.TableAliases.TryGetValue(lower, out var canonical))
        {
            _logger.LogDebug("Normalized table '{Input}' to '{Canonical}'", inputTableName, canonical);
            return canonical;
        }

        _logger.LogWarning("Unknown table name '{Input}', passing through", inputTableName);
        return lower;
    }

    public string GetExportTableName(string normalizedTableName)
    {
        var table = NormalizeTableName(normalizedTableName);
        return table switch
        {
            "rooms" => StandardTableName,
            "assets" => AssetsTableName,
            "warehouses" => WarehousesTableName,
            "inventory" => InventoryTableName,
            _ => table
        };
    }

    public string GetHotelDruidTableName(string normalizedTableName)
    {
        var table = NormalizeTableName(normalizedTableName);
        return table switch
        {
            "rooms" => HotelDruidTableName,
            "assets" => AssetsHotelDruidName,
            "warehouses" => WarehousesHotelDruidName,
            "inventory" => InventoryHotelDruidName,
            _ => table
        };
    }

    // --- Assets / Warehouses / Inventory canonicalization ---

    public CanonicalData ToCanonicalAssets(AssetDto[] assets)
    {
        var columns = new List<ColumnDefinition>
        {
            new("id", "string", false, "Asset identifier"),
            new("name", "string", false, "Asset name"),
            new("code", "string", true, "Asset code"),
            new("description", "string", true, "Description"),
            new("created_at", "string", true, "Created timestamp")
        };

        var rows = assets.Select(a => new Dictionary<string, object?>
        {
            ["id"] = a.Id,
            ["name"] = a.Name,
            ["code"] = a.Code,
            ["description"] = a.Description,
            ["created_at"] = a.CreatedAt?.ToString("O")
        }).ToList();

        return new CanonicalData(TableName: AssetsTableName, RowCount: assets.Length, Rows: rows, Columns: columns);
    }

    public AssetDto[] FromCanonicalAssets(CanonicalData data)
    {
        var normalized = NormalizeTableName(data.TableName);
        if (normalized != "assets") throw new InvalidOperationException($"Cannot map table '{data.TableName}' to assets.");

        var results = new List<AssetDto>();
        foreach (var row in data.Rows)
        {
            var id = GetNormalizedValue(row, "id")?.ToString();
            var name = GetNormalizedValue(row, "name")?.ToString() ?? "";
            var code = GetNormalizedValue(row, "code")?.ToString();
            var description = GetNormalizedValue(row, "description")?.ToString();
            var created = DateTime.TryParse(GetNormalizedValue(row, "created_at")?.ToString(), out var dt) ? dt : (DateTime?)null;

            results.Add(new AssetDto(id, name, code, description, created));
        }

        return results.ToArray();
    }

    public CanonicalData ToCanonicalWarehouses(WarehouseDto[] warehouses)
    {
        var columns = new List<ColumnDefinition>
        {
            new("id", "string", false, "Warehouse identifier"),
            new("name", "string", false, "Warehouse name"),
            new("code", "string", true, "Code"),
            new("description", "string", true, "Description"),
            new("floor_number", "string", true, "Floor"),
            new("house_number", "string", true, "House number")
        };

        var rows = warehouses.Select(w => new Dictionary<string, object?>
        {
            ["id"] = w.Id,
            ["name"] = w.Name,
            ["code"] = w.Code,
            ["description"] = w.Description,
            ["floor_number"] = w.FloorNumber,
            ["house_number"] = w.HouseNumber
        }).ToList();

        return new CanonicalData(TableName: WarehousesTableName, RowCount: warehouses.Length, Rows: rows, Columns: columns);
    }

    public WarehouseDto[] FromCanonicalWarehouses(CanonicalData data)
    {
        var normalized = NormalizeTableName(data.TableName);
        if (normalized != "warehouses") throw new InvalidOperationException($"Cannot map table '{data.TableName}' to warehouses.");

        var results = new List<WarehouseDto>();
        foreach (var row in data.Rows)
        {
            var id = GetNormalizedValue(row, "id")?.ToString();
            var name = GetNormalizedValue(row, "name")?.ToString() ?? "";
            var code = GetNormalizedValue(row, "code")?.ToString();
            var desc = GetNormalizedValue(row, "description")?.ToString();
            var floor = GetNormalizedValue(row, "floor_number")?.ToString();
            var house = GetNormalizedValue(row, "house_number")?.ToString();

            results.Add(new WarehouseDto(id, name, code, desc, floor, house, null));
        }

        return results.ToArray();
    }

    public CanonicalData ToCanonicalInventory(InventoryDto[] inventory)
    {
        var columns = new List<ColumnDefinition>
        {
            new("id", "string", false, "Inventory identifier"),
            new("asset_id", "string", false, "Asset reference id"),
            new("room_id", "string", true, "Room id (optional)"),
            new("warehouse_id", "string", true, "Warehouse id (optional)"),
            new("quantity", "integer", false, "Quantity"),
            new("min_quantity_default", "integer", true, "Min default quantity"),
            new("required_on_checkin", "boolean", true, "Required on check-in")
        };

        var rows = inventory.Select(i => new Dictionary<string, object?>
        {
            ["id"] = i.Id,
            ["asset_id"] = i.AssetId,
            ["room_id"] = i.RoomId,
            ["warehouse_id"] = i.WarehouseId,
            ["quantity"] = i.Quantity,
            ["min_quantity_default"] = i.MinQuantityDefault,
            ["required_on_checkin"] = i.RequiredOnCheckin
        }).ToList();

        return new CanonicalData(TableName: InventoryTableName, RowCount: inventory.Length, Rows: rows, Columns: columns);
    }

    public InventoryDto[] FromCanonicalInventory(CanonicalData data)
    {
        var normalized = NormalizeTableName(data.TableName);
        if (normalized != "inventory") throw new InvalidOperationException($"Cannot map table '{data.TableName}' to inventory.");

        var results = new List<InventoryDto>();
        foreach (var row in data.Rows)
        {
            var id = GetNormalizedValue(row, "id")?.ToString();
            var aid = GetNormalizedValue(row, "asset_id")?.ToString();
            var rid = GetNormalizedValue(row, "room_id")?.ToString();
            var wid = GetNormalizedValue(row, "warehouse_id")?.ToString();
            var qty = int.TryParse(GetNormalizedValue(row, "quantity")?.ToString(), out var q) ? q : 0;
            var minq = int.TryParse(GetNormalizedValue(row, "min_quantity_default")?.ToString(), out var mq) ? mq : (int?)null;
            var req = bool.TryParse(GetNormalizedValue(row, "required_on_checkin")?.ToString(), out var b) ? b : (bool?)null;

            results.Add(new InventoryDto(id, aid, rid, wid, qty, minq, req, null));
        }

        return results.ToArray();
    }

    public string NormalizeFieldName(string inputFieldName)
    {
        var lower = inputFieldName?.ToLowerInvariant().Trim() ?? "";
        
        if (_naming.FieldAliases.TryGetValue(lower, out var canonical))
        {
            return canonical;
        }

        return lower;
    }

    public CanonicalData ToCanonical(RoomDto[] rooms)
    {
        // Build column list
        var columns = new List<ColumnDefinition>
        {
            new("id", "string", false, "Room identifier"),
            new("name", "string", false, "Room name"),
            new("capacity", "integer", false, "Maximum occupants"),
            new("floor_number", "string", true, "Floor number"),
            new("house_number", "string", true, "House/apartment number"),
            new("priority", "integer", true, "Priority ordering (primary)"),
            new("secondary_priority", "integer", true, "Priority ordering (secondary)"),
            new("has_beds", "string", true, "Has beds (S/N)"),
            new("neighboring_rooms", "string", true, "Comma-separated neighbor room IDs"),
            new("comments", "string", true, "Additional comments")
        };

        // Convert rows to canonical format
        var rows = rooms.Select(r => new Dictionary<string, object?>
        {
            ["id"] = r.Id,
            ["name"] = r.Name,
            ["capacity"] = r.Capacity,
            ["floor_number"] = r.FloorNumber,
            ["house_number"] = r.HouseNumber,
            ["priority"] = r.Priority,
            ["secondary_priority"] = r.SecondaryPriority,
            ["has_beds"] = r.HasBeds,
            ["neighboring_rooms"] = r.NeighboringRooms,
            ["comments"] = r.Comments
        }).ToList();

        return new CanonicalData(
            TableName: StandardTableName,
            RowCount: rooms.Length,
            Rows: rows,
            Columns: columns
        );
    }

    public RoomDto[] FromCanonical(CanonicalData data)
    {
        // Validate table name
        var normalizedTable = NormalizeTableName(data.TableName);
        if (normalizedTable != "rooms")
        {
            throw new InvalidOperationException(
                $"Cannot map table '{data.TableName}' to rooms API. Expected 'rooms', 'apartments', or 'appartamenti'.");
        }

        var results = new List<RoomDto>();

        foreach (var row in data.Rows)
        {
            try
            {
                var roomDto = new RoomDto(
                    Id: GetNormalizedValue(row, "id")?.ToString(),
                    Name: GetNormalizedValue(row, "name")?.ToString() ?? "",
                    Capacity: int.TryParse(GetNormalizedValue(row, "capacity")?.ToString(), out var cap) ? cap : 0,
                    FloorNumber: GetNormalizedValue(row, "floor_number")?.ToString(),
                    HouseNumber: GetNormalizedValue(row, "house_number")?.ToString(),
                    Priority: int.TryParse(GetNormalizedValue(row, "priority")?.ToString(), out var p) ? p : null,
                    SecondaryPriority: int.TryParse(GetNormalizedValue(row, "secondary_priority")?.ToString(), out var p2) ? p2 : null,
                    HasBeds: GetNormalizedValue(row, "has_beds")?.ToString(),
                    NeighboringRooms: GetNormalizedValue(row, "neighboring_rooms")?.ToString(),
                    Comments: GetNormalizedValue(row, "comments")?.ToString()
                );
                
                results.Add(roomDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping row to RoomDto");
                throw;
            }
        }

        return results.ToArray();
    }

    public object? GetNormalizedValue(Dictionary<string, object?> row, string canonicalFieldName)
    {
        if (row == null)
            return null;

        // Try exact canonical name first (case-insensitive)
        var lowerCanonical = canonicalFieldName?.ToLowerInvariant() ?? "";
        
        foreach (var (key, value) in row)
        {
            if (key.Equals(lowerCanonical, StringComparison.OrdinalIgnoreCase))
                return value;
        }

        // Try to find via alias mapping (reverse lookup)
        foreach (var (alias, canonical) in _naming.FieldAliases)
        {
            if (canonical == lowerCanonical)
            {
                // Check for this alias in the row (case-insensitive)
                foreach (var (key, value) in row)
                {
                    if (key.Equals(alias, StringComparison.OrdinalIgnoreCase))
                        return value;
                }
            }
        }

        _logger.LogDebug("Field '{Field}' not found in row", canonicalFieldName);
        return null;
    }

    /// <summary>
    /// Build metadata info showing source names vs. export names
    /// </summary>
    public Dictionary<string, string> GetNamingInfo(string sourceTableName)
    {
        var normalized = NormalizeTableName(sourceTableName);
        
        return new()
        {
            ["source_name"] = sourceTableName,
            ["normalized_to"] = normalized,
            ["export_name"] = GetExportTableName(normalized),
            ["HotelDruid_compat"] = GetHotelDruidTableName(normalized)
        };
    }
}

