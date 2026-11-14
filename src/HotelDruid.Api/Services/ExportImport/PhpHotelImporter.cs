using HotelDruid.Api.Models;
using HotelDruid.Shared;

namespace HotelDruid.Api.Services.ExportImport;

/// <summary>
/// Loads a PHP export into the Blazor books.
/// Booking money becomes ledger journals. The cash-register copy of a booking
/// payment is kept as a register post and is not posted a second time.
/// Extra costs stay on the booking and are not posted.
/// </summary>
public sealed class PhpHotelImporter
{
    private readonly IKeyValueStore _store;
    private readonly IFinancialPostingService? _ledger;
    private readonly ILogger _logger;
    private readonly AccountChart _chart;
    private readonly Maps _maps = new();

    public PhpHotelImporter(IKeyValueStore store, IFinancialPostingService? ledger, ILogger logger, AccountChart? chart = null)
    {
        _store = store;
        _ledger = ledger;
        _logger = logger;
        _chart = chart ?? AccountChart.Usali;
    }

    public static bool Handles(string normalized, int? year, CanonicalData data)
    {
        if (Has(data, "blazorId") || Has(data, "blazor_id"))
            return false;

        return normalized switch
        {
            "cash_registers" => true,
            "clients" => true,
            "booking_payments" => true,
            "cash_journal" => true,
            "rooms" => Has(data, "unit_id"),
            "bookings" => year != null || Has(data, "start_date_id"),
            "cancelled_bookings" => year != null || Has(data, "start_date_id"),
            "booking_costs" => year != null || Has(data, "booking_cost_id") || Has(data, "cost_type"),
            "booking_guests" => year != null || Has(data, "idprenota") || Has(data, "parentela"),
            "user_privileges" => Has(data, "idutente") || Has(data, "user_id") || Has(data, "cash_registers_allowed") || Has(data, "booking_payment_register"),
            _ => false
        };
    }

    public async Task<DistributionResult> ImportAsync(IReadOnlyList<PhpTable> tables)
    {
        var results = new Dictionary<string, TableImportResult>();
        var errors = new List<ImportError>();
        var total = 0;
        foreach (var table in tables.OrderBy(Rank).ThenBy(item => item.Year ?? 0).ThenBy(item => item.SourceName, StringComparer.Ordinal))
        {
            try
            {
                var count = table.Normalized switch
                {
                    "cash_registers" => await ImportRegistersAsync(table.Data),
                    "clients" => await ImportClientsAsync(table.Data),
                    "rooms" => await ImportRoomsAsync(table.Data),
                    "user_privileges" => await ImportPrivilegesAsync(table.Data),
                    "bookings" => await ImportBookingsAsync(table, cancelled: false),
                    "cancelled_bookings" => await ImportBookingsAsync(table, cancelled: true),
                    "booking_costs" => await ImportCostsAsync(table),
                    "booking_guests" => await ImportGuestsOnBookingsAsync(table),
                    "booking_payments" => await ImportPaymentsAsync(table),
                    "cash_journal" => await ImportCashJournalAsync(table),
                    _ => 0
                };
                results[table.SourceName] = new TableImportResult(table.SourceName, count, true);
                total += count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PHP import failed for {Table}", table.SourceName);
                errors.Add(new ImportError(table.SourceName, ex.Message));
                results[table.SourceName] = new TableImportResult(table.SourceName, 0, false, ex.Message);
            }
        }

        return new DistributionResult(total, results, errors);
    }

    private async Task<int> ImportRegistersAsync(CanonicalData data)
    {
        var count = 0;
        foreach (var row in data.Rows)
        {
            var name = PhpExportJson.Text(row, "name", "nome_cassa");
            if (name is null)
                continue;
            var phpId = PhpExportJson.Text(row, "cash_register_id", "idcasse");
            var storage = new CashRegisterStorageModel
            {
                Name = name,
                Status = PhpExportJson.Text(row, "status", "stato"),
                Code = PhpExportJson.Text(row, "code", "codice_cassa"),
                Description = PhpExportJson.Text(row, "description", "descrizione_cassa"),
                CreatedAt = PhpExportJson.Timestamp(row, "created_at", "datainserimento"),
                HostCreated = PhpExportJson.Text(row, "created_host", "hostinserimento"),
                CreatedBy = PhpExportJson.Whole(row, "created_by", "utente_inserimento")
            };
            var id = await SaveAsync("cash_registers", name, storage);
            _maps.RegistersByName[name] = id;
            if (phpId is not null)
                _maps.RegistersByPhpId[phpId] = id;
            if (phpId == "1" || name.Contains("principale", StringComparison.OrdinalIgnoreCase))
                _maps.MainRegisterId = id;
            _maps.MainRegisterId ??= id;
            count++;
        }
        return count;
    }

    private async Task<int> ImportClientsAsync(CanonicalData data)
    {
        var count = 0;
        foreach (var row in data.Rows)
        {
            var phpId = PhpExportJson.Text(row, "id", "idclienti");
            var lastName = PhpExportJson.Text(row, "last_name", "cognome") ?? (phpId is null ? null : $"Guest {phpId}");
            if (phpId is null || lastName is null)
                continue;
            var storage = new ClientStorageModel
            {
                LastName = lastName,
                FirstName = PhpExportJson.Text(row, "first_name", "nome"),
                Nickname = PhpExportJson.Text(row, "nickname", "soprannome"),
                Gender = PhpExportJson.Text(row, "gender", "sesso"),
                Title = PhpExportJson.Text(row, "title", "titolo"),
                Language = PhpExportJson.Text(row, "language", "lingua"),
                DateOfBirth = PhpExportJson.Timestamp(row, "birth_date", "datanascita"),
                BirthCity = PhpExportJson.Text(row, "birth_city", "cittanascita"),
                BirthNation = PhpExportJson.Text(row, "birth_country", "nazionenascita"),
                DocumentNumber = PhpExportJson.Text(row, "document_number", "documento"),
                DocumentExpiry = PhpExportJson.Timestamp(row, "document_expiry", "scadenzadoc"),
                DocumentType = PhpExportJson.Text(row, "document_type", "tipodoc"),
                Nation = PhpExportJson.Text(row, "nazione", "nation"),
                City = PhpExportJson.Text(row, "citta", "city"),
                Street = PhpExportJson.Text(row, "via", "street"),
                StreetNumber = PhpExportJson.Text(row, "numcivico"),
                PostalCode = PhpExportJson.Text(row, "cap"),
                Phone = PhpExportJson.Text(row, "phone", "telefono"),
                Email = PhpExportJson.Text(row, "email"),
                TaxCode = PhpExportJson.Text(row, "tax_id", "cod_fiscale"),
                VatNumber = PhpExportJson.Text(row, "vat_id", "partita_iva"),
                Notes = PhpExportJson.Text(row, "notes", "commento"),
                CreatedAt = PhpExportJson.Timestamp(row, "created_at", "datainserimento"),
                HostCreated = PhpExportJson.Text(row, "created_host", "hostinserimento"),
                CreatedBy = PhpExportJson.Whole(row, "created_by", "utente_inserimento")
            };
            _maps.Clients[phpId] = await SaveAsync("clients", $"guest-{phpId}", storage);
            count++;
        }
        return count;
    }

    private async Task<int> ImportRoomsAsync(CanonicalData data)
    {
        var count = 0;
        foreach (var row in data.Rows)
        {
            var unitId = PhpExportJson.Text(row, "unit_id", "idappartamenti");
            if (unitId is null)
                continue;
            var room = new RoomDto(
                Id: null,
                Name: unitId,
                Capacity: PhpExportJson.Whole(row, "max_occupancy", "maxoccupanti") ?? 1,
                FloorNumber: PhpExportJson.Text(row, "floor", "numpiano"),
                HouseNumber: PhpExportJson.Text(row, "unit_number", "numcasa"),
                Priority: PhpExportJson.Whole(row, "priorita", "priority"),
                SecondaryPriority: PhpExportJson.Whole(row, "priorita2"),
                HasBeds: PhpExportJson.Text(row, "letto", "has_beds"),
                NeighboringRooms: PhpExportJson.Text(row, "app_vicini"),
                Comments: PhpExportJson.Text(row, "notes", "commento"));
            _maps.Rooms[unitId] = await SaveAsync("rooms", unitId, room);
            count++;
        }
        return count;
    }

    private async Task<int> ImportPrivilegesAsync(CanonicalData data)
    {
        var count = 0;
        foreach (var row in data.Rows)
        {
            var userId = PhpExportJson.Whole(row, "idutente", "user_id");
            var year = PhpExportJson.Whole(row, "anno", "year");
            if (userId is null || year is null)
                continue;
            var storage = new UserPrivilegeStorageModel
            {
                UserId = userId,
                Year = year,
                AllowedCashRegisters = PhpExportJson.Text(row, "cash_registers_allowed", "casse_consentite"),
                PaymentCashRegister = PhpExportJson.Text(row, "booking_payment_register", "cassa_pagamenti"),
                AllowedRules = PhpExportJson.Text(row, "regole1_consentite"),
                AllowedTariffs = PhpExportJson.Text(row, "tariffe_consentite"),
                AllowedExtraCosts = PhpExportJson.Text(row, "costi_agg_consentiti"),
                AllowedContracts = PhpExportJson.Text(row, "contratti_consentiti")
            };
            await SaveAsync("user_privileges", $"{userId}_{year}", storage);
            count++;
        }
        return count;
    }

    private async Task<int> ImportBookingsAsync(PhpTable table, bool cancelled)
    {
        var year = table.Year ?? 0;
        if (year == 0)
            return 0;
        var count = 0;
        foreach (var row in table.Data.Rows)
        {
            var phpId = PhpExportJson.Text(row, "booking_id", "idprenota");
            var guestPhp = PhpExportJson.Text(row, "guest_id", "idclienti");
            var unitId = PhpExportJson.Text(row, "unit_id", "idappartamenti");
            var startId = PhpExportJson.Whole(row, "start_date_id", "iddatainizio");
            var endId = PhpExportJson.Whole(row, "end_date_id", "iddatafine");
            if (phpId is null || guestPhp is null || startId is null || endId is null)
                continue;
            if (!_maps.Clients.TryGetValue(guestPhp, out var clientId))
                continue;
            _maps.Rooms.TryGetValue(unitId ?? "", out var roomId);
            var arrival = PhpExportJson.DayOfYear(year, startId.Value);
            var departure = PhpExportJson.DayOfYear(year, endId.Value);
            var notes = PhpExportJson.Text(row, "commento", "notes");
            var noShow = notes != null && notes.Contains("No show", StringComparison.OrdinalIgnoreCase);
            var checkIn = PhpExportJson.Timestamp(row, "checkin");
            var checkOut = PhpExportJson.Timestamp(row, "checkout");
            var status = cancelled ? "Cancelled" : noShow ? "NoShow" : checkOut is not null ? "CheckedOut" : checkIn is not null ? "CheckedIn" : "Confirmed";
            var total = PhpExportJson.Number(row, "total_rate", "tariffa_tot") ?? 0;
            var deposit = PhpExportJson.Number(row, "deposit", "caparra") ?? 0;
            var storage = new BookingStorageModel
            {
                Year = year,
                ClientId = clientId,
                RoomId = roomId,
                ArrivalDate = arrival,
                DepartureDate = departure,
                Status = status,
                Notes = notes,
                ActualArrivalDate = checkIn.HasValue ? DateOnly.FromDateTime(checkIn.Value) : null,
                ActualDepartureDate = checkOut.HasValue ? DateOnly.FromDateTime(checkOut.Value) : null,
                ActualGuestCount = PhpExportJson.Whole(row, "guest_count", "num_persone"),
                ExpectedGuestCount = PhpExportJson.Whole(row, "guest_count", "num_persone"),
                PrepaidAmount = deposit,
                PrepaidMethod = PhpExportJson.Text(row, "payment_method", "metodo_pagamento"),
                TariffId = PhpExportJson.Text(row, "tariffa"),
                TariffTotalPrice = total,
                DeletedAt = cancelled ? DateTime.UtcNow : null
            };
            var id = await SaveAsync("bookings", $"{year}_{phpId}", storage);
            _maps.Bookings[$"{year}:{phpId}"] = id;
            if (cancelled)
            {
                await SaveAsync("cancelled_bookings", $"{year}_{phpId}", new CancelledBookingStorageModel
                {
                    Year = year,
                    ClientId = clientId,
                    RoomId = roomId,
                    ArrivalDate = arrival,
                    DepartureDate = departure,
                    Status = "Cancelled",
                    CancelledAt = DateTime.UtcNow,
                    Notes = notes,
                    BookingId = id
                });
            }
            else if (_ledger is not null && total > 0)
            {
                await _ledger.PostBalancedAsync(arrival.ToDateTime(new TimeOnly(12, 0)), "booking_stay_charge", "Stay charge imported from PHP", (decimal)total, _chart.GuestLedger, "asset", _chart.RoomRevenue, "revenue", bookingId: id, sourceType: "booking", sourceId: id, createdBy: PhpExportJson.Text(row, "created_by", "utente_inserimento"));
            }
            count++;
        }
        return count;
    }

    private async Task<int> ImportCostsAsync(PhpTable table)
    {
        var year = table.Year ?? 0;
        if (year == 0)
            return 0;
        var count = 0;
        foreach (var row in table.Data.Rows)
        {
            var costId = PhpExportJson.Text(row, "booking_cost_id", "idcostiprenota") ?? $"{count + 1}";
            var phpBooking = PhpExportJson.Text(row, "booking_id", "idprenota");
            if (phpBooking is null || !_maps.Bookings.TryGetValue($"{year}:{phpBooking}", out var bookingId))
                continue;
            await SaveAsync("booking_costs", $"{year}_{costId}", new BookingCostStorageModel
            {
                Year = year,
                BookingId = bookingId,
                TariffId = PhpExportJson.Text(row, "idntariffe"),
                Amount = PhpExportJson.Number(row, "amount", "valore"),
                Description = PhpExportJson.Text(row, "name", "nome")
            });
            count++;
        }
        return count;
    }

    private async Task<int> ImportGuestsOnBookingsAsync(PhpTable table)
    {
        var year = table.Year ?? 0;
        if (year == 0)
            return 0;
        var count = 0;
        foreach (var row in table.Data.Rows)
        {
            var phpBooking = PhpExportJson.Text(row, "idprenota", "booking_id");
            var phpGuest = PhpExportJson.Text(row, "idclienti", "guest_id");
            if (phpBooking is null || phpGuest is null)
                continue;
            if (!_maps.Bookings.TryGetValue($"{year}:{phpBooking}", out var bookingId) || !_maps.Clients.TryGetValue(phpGuest, out var clientId))
                continue;
            var number = PhpExportJson.Whole(row, "num_ordine") ?? count + 1;
            await SaveAsync("booking_guests", $"{year}_{phpBooking}_{number}", new BookingGuestStorageModel
            {
                Year = year,
                BookingId = bookingId,
                ClientId = clientId,
                GuestNumber = number
            });
            count++;
        }
        return count;
    }

    private async Task<int> ImportPaymentsAsync(PhpTable table)
    {
        var year = table.Year ?? 0;
        if (year == 0)
            return 0;
        var count = 0;
        foreach (var row in table.Data.Rows)
        {
            var amount = PhpExportJson.Number(row, "booking_balance", "saldo_prenota") ?? 0;
            if (amount == 0)
                continue;
            var reason = PhpExportJson.Text(row, "reason", "motivazione");
            var phpBooking = reason?.Split(';').LastOrDefault();
            if (string.IsNullOrWhiteSpace(phpBooking) || !_maps.Bookings.TryGetValue($"{year}:{phpBooking}", out var bookingId))
                continue;
            var when = PhpExportJson.Timestamp(row, "transaction_date", "data_transazione") ?? DateTime.UtcNow;
            var notes = PhpExportJson.Text(row, "notes", "note") ?? "";
            var method = PhpExportJson.Text(row, "payment_method", "metodo_pagamento");
            var registerId = string.Equals(method, "Bonifico", StringComparison.OrdinalIgnoreCase) ? null : _maps.MainRegisterId;
            var createdBy = PhpExportJson.Text(row, "created_by", "utente_inserimento");
            _maps.RegisterCopies.Add(new RegisterCopy(year, Math.Abs(amount), method, when, PhpExportJson.Number(row, "running_cash_total", "saldo_cassa")));
            if (_ledger is not null)
            {
                var magnitude = (decimal)Math.Abs(amount);
                var refund = amount < 0 || notes.Contains("Rimborso", StringComparison.OrdinalIgnoreCase);
                var deposit = !refund && notes.Contains("Caparra", StringComparison.OrdinalIgnoreCase);
                if (refund && notes.Contains("Caparra", StringComparison.OrdinalIgnoreCase) || refund && notes.Contains("cancellazione", StringComparison.OrdinalIgnoreCase))
                {
                    await _ledger.PostBalancedAsync(when, "booking_refund", notes, magnitude, _chart.GuestDeposits, "liability", _chart.Cash, "asset", bookingId: bookingId, cashRegisterId: registerId, sourceType: "booking-payment", sourceId: PhpExportJson.Text(row, "payment_id", "idsoldi"), createdBy: createdBy);
                }
                else if (refund)
                {
                    await _ledger.PostBalancedAsync(when, "booking_refund", notes, magnitude, _chart.GuestLedger, "asset", _chart.Cash, "asset", bookingId: bookingId, cashRegisterId: registerId, sourceType: "booking-payment", sourceId: PhpExportJson.Text(row, "payment_id", "idsoldi"), createdBy: createdBy);
                }
                else if (deposit)
                {
                    await _ledger.PostBalancedAsync(when, "booking_deposit", notes, magnitude, _chart.Cash, "asset", _chart.GuestDeposits, "liability", bookingId: bookingId, cashRegisterId: registerId, sourceType: "booking-payment", sourceId: PhpExportJson.Text(row, "payment_id", "idsoldi"), createdBy: createdBy);
                }
                else
                {
                    await _ledger.PostBalancedAsync(when, "booking_payment", string.IsNullOrWhiteSpace(notes) ? "Booking payment" : notes, magnitude, _chart.Cash, "asset", _chart.GuestLedger, "asset", bookingId: bookingId, cashRegisterId: registerId, sourceType: "booking-payment", sourceId: PhpExportJson.Text(row, "payment_id", "idsoldi"), createdBy: createdBy);
                }
            }
            count++;
        }
        return count;
    }

    private async Task<int> ImportCashJournalAsync(PhpTable table)
    {
        var year = table.Year ?? 0;
        if (year == 0)
            return 0;
        var count = 0;
        foreach (var row in table.Data.Rows)
        {
            var expenseId = PhpExportJson.Text(row, "expense_id", "idcosti");
            if (expenseId == "0")
            {
                var closing = PhpExportJson.Number(row, "amount", "val_costo");
                if (closing.HasValue && _maps.MainRegisterId is not null)
                {
                    var register = await _store.GetAsync<CashRegisterStorageModel>("cash_registers", _maps.MainRegisterId);
                    if (register is not null)
                    {
                        register.Description = $"Closing balance {closing.Value:0.00} EUR";
                        await _store.UpdateAsync("cash_registers", _maps.MainRegisterId, register);
                    }
                }
                continue;
            }

            var rawAmount = PhpExportJson.Number(row, "amount", "val_costo") ?? 0;
            if (rawAmount == 0)
                continue;
            var entryType = PhpExportJson.Text(row, "entry_type", "tipo_costo");
            var origin = PhpExportJson.Text(row, "origin", "provenienza_costo");
            var registerName = PhpExportJson.Text(row, "cash_register", "nome_cassa");
            var registerId = ResolveRegister(registerName);
            var outflow = string.Equals(entryType, "s", StringComparison.OrdinalIgnoreCase);
            var bookingCopy = string.Equals(origin, "p", StringComparison.OrdinalIgnoreCase);
            var when = PhpExportJson.Timestamp(row, "transaction_date", "data_transazione") ?? DateTime.UtcNow;
            var description = PhpExportJson.Text(row, "expense_name", "nome_costo");
            var person = PhpExportJson.Text(row, "person", "persona_costo");
            var method = PhpExportJson.Text(row, "payment_method", "metodo_pagamento");
            var currency = PhpExportJson.Text(row, "currency", "valuta") ?? "EUR";
            double? runningTotal = null;
            if (bookingCopy)
            {
                var match = _maps.RegisterCopies.FirstOrDefault(copy =>
                    copy.Year == year
                    && Math.Abs(copy.Amount - Math.Abs(rawAmount)) < 0.001
                    && string.Equals(copy.Method, method, StringComparison.OrdinalIgnoreCase)
                    && Math.Abs((copy.When - when).TotalMinutes) < 2);
                if (match is not null)
                {
                    runningTotal = match.RunningTotal;
                    _maps.RegisterCopies.Remove(match);
                }
            }
            var history = new MoneyHistoryStorageModel
            {
                Year = year,
                CashRegisterId = registerId,
                Amount = outflow ? -Math.Abs(rawAmount) : Math.Abs(rawAmount),
                Type = bookingCopy ? "register" : outflow ? "expense" : "income",
                Description = description,
                Date = DateOnly.FromDateTime(when),
                Person = person,
                PaymentMethod = method,
                Currency = currency,
                RunningCashTotal = runningTotal
            };
            var id = await SaveAsync("money_history", $"{year}_cash_{expenseId ?? count.ToString()}", history);
            if (!bookingCopy && _ledger is not null)
            {
                var magnitude = (decimal)Math.Abs(rawAmount);
                if (outflow)
                    await _ledger.PostBalancedAsync(when, "petty_cash_expense", description ?? "Petty cash expense", magnitude, _chart.OperatingExpense, "expense", _chart.Cash, "asset", cashRegisterId: registerId, payerName: person, sourceType: "money-history", sourceId: id, createdBy: PhpExportJson.Text(row, "created_by", "utente_inserimento"));
                else
                    await _ledger.PostBalancedAsync(when, "petty_cash_income", description ?? "Petty cash income", magnitude, _chart.Cash, "asset", _chart.OtherIncome, "revenue", cashRegisterId: registerId, payerName: person, sourceType: "money-history", sourceId: id, createdBy: PhpExportJson.Text(row, "created_by", "utente_inserimento"));
            }
            count++;
        }
        return count;
    }

    private string? ResolveRegister(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return _maps.MainRegisterId;
        if (_maps.RegistersByName.TryGetValue(name, out var id))
            return id;
        return _maps.MainRegisterId;
    }

    private async Task<string> SaveAsync<T>(string collection, string key, T document) where T : class
    {
        try
        {
            return await _store.CreateAsync(collection, key, document);
        }
        catch (InvalidOperationException)
        {
            var index = await _store.GetIndexAsync(collection);
            if (!index.TryGetValue(key, out var existing))
                throw;
            await _store.UpdateAsync(collection, existing, document);
            return existing;
        }
    }

    private static bool Has(CanonicalData data, string field)
    {
        return data.Rows.Count > 0 && data.Rows[0].ContainsKey(field);
    }

    private static int Rank(PhpTable table)
    {
        return table.Normalized switch
        {
            "cash_registers" => 0,
            "clients" => 1,
            "rooms" => 2,
            "user_privileges" => 3,
            "bookings" => 4,
            "cancelled_bookings" => 5,
            "booking_costs" => 6,
            "booking_guests" => 7,
            "booking_payments" => 8,
            "cash_journal" => 9,
            _ => 10
        };
    }

    private sealed class Maps
    {
        public Dictionary<string, string> Clients { get; } = new(StringComparer.Ordinal);
        public Dictionary<string, string> Rooms { get; } = new(StringComparer.Ordinal);
        public Dictionary<string, string> RegistersByName { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> RegistersByPhpId { get; } = new(StringComparer.Ordinal);
        public Dictionary<string, string> Bookings { get; } = new(StringComparer.Ordinal);
        public List<RegisterCopy> RegisterCopies { get; } = new();
        public string? MainRegisterId { get; set; }
    }
}

public sealed record RegisterCopy(int Year, double Amount, string? Method, DateTime When, double? RunningTotal);

public sealed record PhpTable(string SourceName, string Normalized, int? Year, CanonicalData Data);
