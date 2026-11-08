using HotelDruid.Api.Models;
using HotelDruid.Api.Services;
using HotelDruid.Api.Services.ExportImport;
using HotelDruid.Shared.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace HotelDruid.Api.Tests.Integration.ExportImport;

public class PhpPackageImportTests : IDisposable
{
    private readonly string _root;
    private readonly FileKeyValueStore _store;
    private readonly LedgerRepository _ledger;
    private readonly ApiDistributor _distributor;

    public PhpPackageImportTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "php-import-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
        _store = new FileKeyValueStore(_root, NullLogger<FileKeyValueStore>.Instance);
        _ledger = new LedgerRepository(_root, NullLogger<LedgerRepository>.Instance);
        var index = new BookingLedgerIndexRepository(_root, NullLogger<BookingLedgerIndexRepository>.Instance);
        var posting = new FinancialPostingService(_ledger, index);
        var mapper = new CanonicalMapper(NullLogger<CanonicalMapper>.Instance);
        _distributor = new ApiDistributor(_store, new Mock<ISystemConfigurationStore>().Object, mapper, NullLogger<ApiDistributor>.Instance, posting);
    }

    [Fact]
    public void YearSuffix_AndDayNumbers_FollowThePhpExport()
    {
        var mapper = new CanonicalMapper(NullLogger<CanonicalMapper>.Instance);
        Assert.Equal("bookings", mapper.NormalizeTableName("bookings2026"));
        Assert.Equal(2026, mapper.GetTableYear("bookings2026"));
        Assert.Equal("rooms", mapper.NormalizeTableName("properties"));
        Assert.Equal("clients", mapper.NormalizeTableName("guests"));
        Assert.Equal("booking_payments", mapper.NormalizeTableName("payments2025"));
        Assert.Equal(new DateOnly(2026, 1, 3), PhpExportJson.DayOfYear(2025, 368));

        var table = PhpExportJson.ReadTable("""
            {
              "table_name": "payments2026",
              "row_count": 1,
              "columns": [{ "name": "booking_balance", "type": "float8", "nullable": true }],
              "rows": [{ "booking_balance": 90.0, "reason": "1;1;4;7" }]
            }
            """);
        Assert.NotNull(table);
        Assert.Equal("payments2026", table!.TableName);
        Assert.Equal(90d, PhpExportJson.Number(table.Rows[0], "booking_balance"));
    }

    [Fact]
    public async Task PhpPackage_PostsBookingMoneyOnce_AndKeepsPettyCashFields()
    {
        var when = "2026-01-02 11:05:00";
        var data = new Dictionary<string, CanonicalData>
        {
            ["cash_registers"] = Table("cash_registers", Row(("cash_register_id", 1L), ("name", "Cassa principale"), ("code", "MAIN"))),
            ["guests"] = Table("guests", Row(("id", 1L), ("last_name", "Rossi"), ("first_name", "Giulia"))),
            ["properties"] = Table("properties", Row(("unit_id", "101"), ("floor", "1"), ("max_occupancy", 2L))),
            ["permissions"] = Table("permissions", Row(("idutente", 2L), ("anno", 2026L), ("cash_registers_allowed", "s,1"), ("booking_payment_register", "Cassa principale"))),
            ["bookings2026"] = Table("bookings2026", Row(
                ("booking_id", 7L), ("guest_id", 1L), ("unit_id", "101"), ("start_date_id", 1L), ("end_date_id", 4L),
                ("guest_count", 2L), ("total_rate", 300d), ("deposit", 90d), ("paid_amount", 90d),
                ("currency", "EUR"), ("payment_method", "Carta"), ("tariffa", "1"), ("conferma", "S"))),
            ["booking_costs2026"] = Table("booking_costs2026", Row(
                ("booking_cost_id", 1L), ("booking_id", 7L), ("cost_type", "s"), ("name", "Extra person"), ("amount", 15d))),
            ["payments2026"] = Table("payments2026", Row(
                ("payment_id", 1L), ("reason", "1;1;4;7"), ("booking_balance", 90d), ("amount_before", 0d),
                ("payment_method", "Carta"), ("notes", "Caparra"), ("running_cash_total", 590d),
                ("currency", "EUR"), ("transaction_date", when), ("created_by", 2L))),
            ["expenses2026"] = Table("expenses2026",
                Row(("expense_id", 0L), ("expense_name", "Saldo cassa"), ("amount", 565d), ("currency", "EUR"), ("cash_register", "Cassa principale")),
                Row(("expense_id", 1L), ("expense_name", "Prenotazione 7"), ("amount", 90d), ("entry_type", "e"), ("origin", "p"),
                    ("cash_register", "Cassa principale"), ("person", "Giulia Bianchi"), ("payment_method", "Carta"),
                    ("currency", "EUR"), ("transaction_date", when)),
                Row(("expense_id", 2L), ("expense_name", "Detergenti"), ("amount", 25d), ("entry_type", "s"), ("origin", "s"),
                    ("cash_register", "Cassa principale"), ("person", "Marco Ricci"), ("payment_method", "Contanti"),
                    ("currency", "EUR"), ("transaction_date", "2026-01-03 09:00:00")))
        };

        var result = await _distributor.DistributeAsync(data);

        Assert.Empty(result.Errors);
        Assert.Equal(1, result.ByTable["bookings2026"].ImportedCount);
        var booking = (await _store.ListAsync<BookingStorageModel>("bookings")).Single();
        Assert.Equal(new DateOnly(2026, 1, 1), booking.ArrivalDate);
        Assert.Equal(new DateOnly(2026, 1, 4), booking.DepartureDate);
        Assert.Equal(300d, booking.TariffTotalPrice);
        Assert.Equal("Confirmed", booking.Status);

        var lines = await _ledger.GetEntriesForBookingAsync(
            (await _store.GetIndexAsync("bookings")).Values.Single(),
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 5));
        Assert.Contains(lines, line => line.EntryType == "booking_stay_charge" && line.Credit == 300m);
        Assert.Contains(lines, line => line.EntryType == "booking_deposit" && line.Debit == 90m && line.AccountCode == "1000");
        Assert.Contains(lines, line => line.EntryType == "booking_deposit" && line.Credit == 90m && line.AccountCode == "2400");
        Assert.DoesNotContain(lines, line => line.EntryType == "booking_extra_charge");
        Assert.Single(lines, line => line.EntryType == "booking_deposit" && line.Debit > 0);

        var history = await _store.ListAsync<MoneyHistoryStorageModel>("money_history");
        var copy = Assert.Single(history, row => row.Type == "register");
        Assert.Equal("Giulia Bianchi", copy.Person);
        Assert.Equal("Carta", copy.PaymentMethod);
        Assert.Equal("EUR", copy.Currency);
        Assert.Equal(590d, copy.RunningCashTotal);
        var petty = Assert.Single(history, row => row.Type == "expense");
        Assert.Equal("Marco Ricci", petty.Person);
        Assert.Equal("Contanti", petty.PaymentMethod);
        Assert.Equal(-25d, petty.Amount);

        var pettyLines = await _ledger.GetEntriesForDateAsync(new DateTime(2026, 1, 3));
        Assert.Contains(pettyLines, line => line.EntryType == "petty_cash_expense" && string.IsNullOrEmpty(line.BookingId) && line.PayerName == "Marco Ricci");
        Assert.DoesNotContain(pettyLines, line => line.EntryType == "petty_cash_income");

        var privilege = (await _store.ListAsync<UserPrivilegeStorageModel>("user_privileges")).Single();
        Assert.Equal("s,1", privilege.AllowedCashRegisters);
        Assert.Equal("Cassa principale", privilege.PaymentCashRegister);
        var register = (await _store.ListAsync<CashRegisterStorageModel>("cash_registers")).Single();
        Assert.Equal("Closing balance 565.00 EUR", register.Description);
        Assert.Single(await _store.ListAsync<BookingCostStorageModel>("booking_costs"));
    }

    [Fact]
    public async Task PhpPackage_SettlesPayments_AndKeepsCancelledStaysOffTheLedger()
    {
        var data = new Dictionary<string, CanonicalData>
        {
            ["cash_registers"] = Table("cash_registers", Row(("cash_register_id", 1L), ("name", "Cassa principale"))),
            ["guests"] = Table("guests",
                Row(("id", 1L), ("last_name", "Rossi"), ("first_name", "Giulia")),
                Row(("id", 2L), ("last_name", "Bianchi"), ("first_name", "Marco"))),
            ["properties"] = Table("properties",
                Row(("unit_id", "101"), ("max_occupancy", 2L)),
                Row(("unit_id", "102"), ("max_occupancy", 2L))),
            ["bookings2026"] = Table("bookings2026",
                Row(("booking_id", 10L), ("guest_id", 1L), ("unit_id", "101"), ("start_date_id", 10L), ("end_date_id", 11L),
                    ("total_rate", 80d), ("commento", "No show"), ("conferma", "S")),
                Row(("booking_id", 11L), ("guest_id", 2L), ("unit_id", "102"), ("start_date_id", 20L), ("end_date_id", 22L),
                    ("total_rate", 160d), ("checkin", "2026-01-20 15:00:00"), ("checkout", "2026-01-22 10:00:00"))),
            ["cancelled_bookings2026"] = Table("cancelled_bookings2026", Row(
                ("booking_id", 12L), ("guest_id", 1L), ("unit_id", "101"), ("start_date_id", 30L), ("end_date_id", 32L),
                ("total_rate", 200d), ("commento", "Cancelled"))),
            ["booking_clients2026"] = Table("booking_clients2026", Row(("idprenota", 11L), ("idclienti", 2L), ("num_ordine", 1L))),
            ["payments2026"] = Table("payments2026",
                Row(("payment_id", 1L), ("reason", "2;20;22;11"), ("booking_balance", 160d), ("payment_method", "Carta"),
                    ("notes", "Pagamento intero"), ("transaction_date", "2026-01-20 16:00:00")),
                Row(("payment_id", 2L), ("reason", "2;20;22;11"), ("booking_balance", -20d), ("payment_method", "Carta"),
                    ("notes", "Rimborso"), ("transaction_date", "2026-01-21 09:00:00")),
                Row(("payment_id", 3L), ("reason", "1;30;32;12"), ("booking_balance", -50d), ("payment_method", "Bonifico"),
                    ("notes", "Rimborso cancellazione"), ("transaction_date", "2026-01-15 09:00:00"))),
            ["sessions"] = Table("sessions", Row(("session_id", 1L), ("user", "admin")))
        };

        var result = await _distributor.DistributeAsync(data);

        Assert.Empty(result.Errors);
        Assert.True(result.ByTable["sessions"].Success);
        Assert.Equal(0, result.ByTable["sessions"].ImportedCount);

        var bookings = await _store.GetIndexAsync("bookings");
        var noShowId = bookings["2026_10"];
        var stayedId = bookings["2026_11"];
        var cancelledId = bookings["2026_12"];
        var noShow = await _store.GetAsync<BookingStorageModel>("bookings", noShowId);
        var stayed = await _store.GetAsync<BookingStorageModel>("bookings", stayedId);
        var cancelled = await _store.GetAsync<BookingStorageModel>("bookings", cancelledId);
        Assert.Equal("NoShow", noShow!.Status);
        Assert.Null(noShow.DeletedAt);
        Assert.Equal("CheckedOut", stayed!.Status);
        Assert.Equal("Cancelled", cancelled!.Status);
        Assert.NotNull(cancelled.DeletedAt);

        var archive = (await _store.ListAsync<CancelledBookingStorageModel>("cancelled_bookings")).Single();
        Assert.Equal(cancelledId, archive.BookingId);
        var guestLink = (await _store.ListAsync<BookingGuestStorageModel>("booking_guests")).Single();
        Assert.Equal(stayedId, guestLink.BookingId);

        var noShowLines = await Lines(noShowId);
        Assert.Contains(noShowLines, line => line.EntryType == "booking_stay_charge" && line.Credit == 80m);
        var stayedLines = await Lines(stayedId);
        Assert.Contains(stayedLines, line => line.EntryType == "booking_stay_charge" && line.Credit == 160m && line.AccountCode == "4100");
        Assert.Contains(stayedLines, line => line.EntryType == "booking_payment" && line.Debit == 160m && line.AccountCode == "1000" && line.CashRegisterId != null);
        Assert.Contains(stayedLines, line => line.EntryType == "booking_payment" && line.Credit == 160m && line.AccountCode == "1200");
        Assert.Contains(stayedLines, line => line.EntryType == "booking_refund" && line.Debit == 20m && line.AccountCode == "1200");
        Assert.Contains(stayedLines, line => line.EntryType == "booking_refund" && line.Credit == 20m && line.AccountCode == "1000");
        var cancelledLines = await Lines(cancelledId);
        Assert.DoesNotContain(cancelledLines, line => line.EntryType == "booking_stay_charge");
        Assert.Contains(cancelledLines, line => line.EntryType == "booking_refund" && line.Debit == 50m && line.AccountCode == "2400" && line.CashRegisterId == null);
        Assert.Contains(cancelledLines, line => line.EntryType == "booking_refund" && line.Credit == 50m && line.AccountCode == "1000");
    }

    private async Task<List<LedgerEntry>> Lines(string bookingId)
    {
        return await _ledger.GetEntriesForBookingAsync(bookingId, new DateTime(2026, 1, 1), new DateTime(2026, 2, 15));
    }

    public void Dispose()
    {
        _store.Dispose();
        try { Directory.Delete(_root, true); } catch { }
    }

    private static CanonicalData Table(string name, params Dictionary<string, object?>[] rows)
    {
        return new CanonicalData(name, rows.Length, rows.ToList(), new List<ColumnDefinition>());
    }

    private static Dictionary<string, object?> Row(params (string Key, object? Value)[] cells)
    {
        return cells.ToDictionary(cell => cell.Key, cell => cell.Value, StringComparer.OrdinalIgnoreCase);
    }
}
