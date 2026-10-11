using System.Reflection;
using HotelDruid.Api.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HotelDruid.Api.Services;

/// <summary>
/// Runs at startup and seeds the key-value store with default HTML document templates
/// if they are not already present.  Templates are stored as contract_templates with
/// Type = template-type-slug and Number = 1.
///
/// A stored template is refreshed only when it still matches the current or previous
/// embedded default. Customized templates are left in place.
/// </summary>
public sealed class DocumentTemplateSeeder : IHostedService
{
    /// <summary>
    /// The set of document types that ship with the system.
    /// Each maps to an embedded resource under Resources/Templates/{slug}.html
    /// </summary>
    public static readonly IReadOnlyList<string> KnownTemplateTypes =
    [
        "booking-confirmation",
        "invoice",
        "receipt",
        "welcome-letter",
        "email-confirmation"
    ];

    private readonly IKeyValueStore _store;
    private readonly ILogger<DocumentTemplateSeeder> _logger;

    public DocumentTemplateSeeder(IKeyValueStore store, ILogger<DocumentTemplateSeeder> logger)
    {
        _store = store;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var idx = await _store.GetIndexAsync("contract_templates");

        foreach (var type in KnownTemplateTypes)
        {
            var key = TemplateKey(type, 1);
            var resourceName = $"HotelDruid.Api.Resources.Templates.{type}.html";
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                _logger.LogWarning("Embedded resource '{Resource}' not found — skipping seed for '{Type}'",
                    resourceName, type);
                continue;
            }

            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync(cancellationToken);
            var previous = await ReadPreviousEmbeddedTemplateAsync(assembly, type);

            if (idx.TryGetValue(key, out var existingId))
            {
                var existing = await _store.GetAsync<ContractTemplateStorageModel>("contract_templates", existingId);
                if (existing is null || !IsUnmodifiedDefault(existing.Content, previous, content))
                {
                    _logger.LogDebug("Document template '{Type}' was customized — leaving it in place", type);
                    continue;
                }

                existing.Content = content;
                await _store.UpdateAsync("contract_templates", existingId, existing);
                _logger.LogInformation("Refreshed unmodified default document template '{Type}'", type);
                continue;
            }

            var model = new ContractTemplateStorageModel
            {
                Type = type,
                Number = 1,
                Content = content
            };

            await _store.CreateAsync("contract_templates", key, model);
            _logger.LogInformation("Seeded default document template '{Type}'", type);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// Returns the index key used for a given template type and number.
    /// Matches the sanitization applied by the contract-templates API endpoint.
    /// </summary>
    public static string TemplateKey(string type, int number)
    {
        var sanitized = System.Text.RegularExpressions.Regex.Replace(type, @"[^a-zA-Z0-9_]", "_");
        return $"{sanitized}_{number}";
    }

    private static async Task<string?> ReadPreviousEmbeddedTemplateAsync(Assembly assembly, string type)
    {
        var resourceName = $"HotelDruid.Api.Resources.Templates.Previous.{type}.html";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null) return null;
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    private static bool IsUnmodifiedDefault(string? stored, string? previous, string current)
    {
        var normalized = NormalizeTemplate(stored);
        return normalized == NormalizeTemplate(current) || normalized == NormalizeTemplate(previous);
    }

    private static string NormalizeTemplate(string? content)
        => (content ?? string.Empty).Replace("\r\n", "\n").Trim();
}
