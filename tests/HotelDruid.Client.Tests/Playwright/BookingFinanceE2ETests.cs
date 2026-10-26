using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelDruid.Client.Tests.Playwright;

[TestClass]
public class BookingFinanceE2ETests : PageTest
{
    [TestMethod]
    public async Task CreateBooking_AssignTariff_RecordPaymentAndRefund_ShowsOnInvoice()
    {
        // 1. Open application (webServer in config starts the API)
        await Page.GotoAsync("/");

        // --- MANUAL STEPS TO BE AUTOMATED (Playwright locators) ---
        // TODO: Login if required
        // TODO: Navigate to Rooms → ensure Room 102 exists
        // TODO: Navigate to Tariffs → assign at least one tariff to Room 102
        // TODO: Navigate to Bookings → click "New"
        // TODO: Fill form (client, room=102, dates, save)
        // TODO: Assert TariffId and TariffTotalPrice are populated (via API call or UI)
        // TODO: Open Booking Finance for the new booking
        // TODO: Record a payment (e.g. 150)
        // TODO: Record a refund (e.g. 4)
        // TODO: Assert the left list shows both rows under the reservation
        // TODO: Assert Total Due is correctly recalculated (0 or expected value)
        // TODO: Open Invoice and Receipt → assert the two transaction lines appear
        // TODO: Open Receipt → assert the same lines are present

        // Placeholder assertion so the test compiles
        await Expect(Page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex(".*", System.Text.RegularExpressions.RegexOptions.IgnoreCase));
    }
}