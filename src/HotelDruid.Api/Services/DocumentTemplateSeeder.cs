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
/// Existing templates are NEVER overwritten so a restored dataset is always preserved.
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
            if (idx.ContainsKey(key))
            {
                _logger.LogDebug("Document template '{Type}' already present — skipping seed", type);
                continue;
            }

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
}
