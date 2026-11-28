using HotelDruid.Api.Models;
using HotelDruid.Shared;

namespace HotelDruid.Api.Services;

/// <summary>
/// A letter marked for check-in is queued when the guest arrives.
/// A Spanish hotel marks the existing Spanish registration letter, and leaves its wording alone.
/// </summary>
public static class CheckInDocuments
{
    public const string Type = "check-in";
    public const string SpainRegistration = "registration-es";

    public static async Task EnsureSpainTemplateAsync(IKeyValueStore store, ISystemConfigurationStore configurationStore)
    {
        var configuration = await configurationStore.GetAsync();
        if (!string.Equals(configuration?.RegistrationStandard, GuestRegistration.SpainId, StringComparison.OrdinalIgnoreCase))
            return;

        var key = DocumentTemplateSeeder.TemplateKey(SpainRegistration, 1);
        var index = await store.GetIndexAsync("contract_templates");
        if (!index.TryGetValue(key, out var id))
            return;

        var template = await store.GetAsync<ContractTemplateStorageModel>("contract_templates", id);
        if (template is null || template.DeliverOnCheckIn)
            return;

        template.DeliverOnCheckIn = true;
        await store.UpdateAsync("contract_templates", id, template);
    }

    public static async Task QueueAsync(
        IKeyValueStore store,
        IDocumentRenderService renderer,
        DispatchService dispatch,
        string bookingId,
        int year)
    {
        var index = await store.GetIndexAsync("contract_templates");
        foreach (var (_, id) in index)
        {
            var template = await store.GetAsync<ContractTemplateStorageModel>("contract_templates", id);
            if (template is null)
                continue;
            var marked = template.DeliverOnCheckIn
                || string.Equals(template.Type, Type, StringComparison.OrdinalIgnoreCase);
            if (!marked || string.IsNullOrWhiteSpace(template.Type))
                continue;

            var context = await renderer.BuildContextAsync(bookingId, year, template.Type);
            if (context is null)
                return;
            var html = renderer.RenderHtml(template.Content ?? string.Empty, context);
            var (eml, _, _) = renderer.ApplyFormat(html, context, DocumentFormat.Eml);
            await dispatch.EnqueueDocumentAsync(context, html, eml, template.Type);
        }
    }
}
