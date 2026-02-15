using System.Text;
using System.Xml.Linq;

namespace HotelDroid.Api.Tests.Reporting;

/// <summary>
/// Test result model for dashboard reporting.
/// </summary>
public class TestResult
{
    public string TestName { get; set; } = string.Empty;
    public string TestClass { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;  // Passed, Failed, Skipped
    public long DurationMs { get; set; }
    public DateTime Timestamp { get; set; }
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
    public string Assembly { get; set; } = string.Empty;
    public Dictionary<string, string> Properties { get; set; } = new();
}

/// <summary>
/// Test result aggregator and dashboard HTML generator.
/// Consumes xUnit TRX files and produces dashboard HTML.
/// </summary>
public class TestDashboardGenerator
{
    private readonly List<TestResult> _results = new();

    /// <summary>
    /// Load test results from xUnit TRX file.
    /// </summary>
    public void LoadFromTrxFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"TRX file not found: {filePath}");

        var doc = XDocument.Load(filePath);
        var ns = XNamespace.Get("http://microsoft.com/schemas/VisualStudio/TeamTest/2010");

        var utrResults = doc.Descendants(ns + "UnitTestResult");
        foreach (var result in utrResults)
        {
            var testMethod = result.Attribute("testName")?.Value ?? "Unknown";
            var outcome = result.Attribute("outcome")?.Value ?? "Unknown";
            var duration = result.Attribute("duration")?.Value ?? "00:00:00";

            var testResult = new TestResult
            {
                TestName = testMethod,
                Status = outcome,
                DurationMs = ParseDuration(duration),
                Timestamp = DateTime.UtcNow,
                Assembly = testMethod.Split('.').FirstOrDefault() ?? "Unknown"
            };

            // Extract error message if failed
            var errorMsg = result.Element(ns + "Output")?.Element(ns + "ErrorInfo")?.Element(ns + "Message")?.Value;
            if (!string.IsNullOrEmpty(errorMsg))
            {
                testResult.ErrorMessage = errorMsg;
            }

            _results.Add(testResult);
        }
    }

    /// <summary>
    /// Generate HTML dashboard report.
    /// </summary>
    public string GenerateDashboardHtml()
    {
        var html = new StringBuilder();

        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang=\"en\">");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset=\"UTF-8\">");
        html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.AppendLine("    <title>HotelDroid Test Dashboard</title>");
        html.Append(GetCssStyles());
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine("    <div class=\"container\">");

        // Header
        html.AppendLine("        <header>");
        html.AppendLine("            <h1>🏨 HotelDroid Test Dashboard</h1>");
        html.AppendLine($"            <p>Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
        html.AppendLine("        </header>");

        // Summary Stats
        var totalTests = _results.Count;
        var passed = _results.Count(r => r.Status == "Passed");
        var failed = _results.Count(r => r.Status == "Failed");
        var skipped = _results.Count(r => r.Status == "Skipped");
        var passRate = totalTests > 0 ? (passed * 100.0 / totalTests) : 0;

        html.AppendLine("        <section class=\"summary\">");
        html.AppendLine("            <h2>Summary</h2>");
        html.AppendLine("            <div class=\"stats-grid\">");
        html.AppendLine($"                <div class=\"stat-card\">");
        html.AppendLine($"                    <span class=\"stat-value\">{totalTests}</span>");
        html.AppendLine($"                    <span class=\"stat-label\">Total Tests</span>");
        html.AppendLine($"                </div>");
        html.AppendLine($"                <div class=\"stat-card passed\">");
        html.AppendLine($"                    <span class=\"stat-value\">{passed}</span>");
        html.AppendLine($"                    <span class=\"stat-label\">Passed</span>");
        html.AppendLine($"                </div>");
        html.AppendLine($"                <div class=\"stat-card failed\">");
        html.AppendLine($"                    <span class=\"stat-value\">{failed}</span>");
        html.AppendLine($"                    <span class=\"stat-label\">Failed</span>");
        html.AppendLine($"                </div>");
        html.AppendLine($"                <div class=\"stat-card skipped\">");
        html.AppendLine($"                    <span class=\"stat-value\">{skipped}</span>");
        html.AppendLine($"                    <span class=\"stat-label\">Skipped</span>");
        html.AppendLine($"                </div>");
        html.AppendLine($"                <div class=\"stat-card\">");
        html.AppendLine($"                    <span class=\"stat-value\">{passRate:F1}%</span>");
        html.AppendLine($"                    <span class=\"stat-label\">Pass Rate</span>");
        html.AppendLine($"                </div>");
        html.AppendLine($"            </div>");
        html.AppendLine("        </section>");

        // Test Results Table
        html.AppendLine("        <section class=\"results\">");
        html.AppendLine("            <h2>Test Results</h2>");
        html.AppendLine("            <table class=\"results-table\">");
        html.AppendLine("                <thead>");
        html.AppendLine("                    <tr>");
        html.AppendLine("                        <th>Test Name</th>");
        html.AppendLine("                        <th>Status</th>");
        html.AppendLine("                        <th>Duration (ms)</th>");
        html.AppendLine("                        <th>Assembly</th>");
        html.AppendLine("                        <th>Details</th>");
        html.AppendLine("                    </tr>");
        html.AppendLine("                </thead>");
        html.AppendLine("                <tbody>");

        foreach (var result in _results.OrderByDescending(r => r.DurationMs))
        {
            var statusClass = result.Status.ToLower();
            var statusIcon = result.Status switch
            {
                "Passed" => "✓",
                "Failed" => "✗",
                "Skipped" => "⊘",
                _ => "?"
            };

            html.AppendLine($"                    <tr class=\"{statusClass}\">");
            html.AppendLine($"                        <td><code>{result.TestName}</code></td>");
            html.AppendLine($"                        <td><span class=\"badge {statusClass}\">{statusIcon} {result.Status}</span></td>");
            html.AppendLine($"                        <td>{result.DurationMs}</td>");
            html.AppendLine($"                        <td>{result.Assembly}</td>");
            html.AppendLine($"                        <td>");

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                html.AppendLine($"                            <details>");
                html.AppendLine($"                                <summary>Error</summary>");
                html.AppendLine($"                                <pre>{System.Web.HttpUtility.HtmlEncode(result.ErrorMessage)}</pre>");
                html.AppendLine($"                            </details>");
            }

            html.AppendLine($"                        </td>");
            html.AppendLine($"                    </tr>");
        }

        html.AppendLine("                </tbody>");
        html.AppendLine("            </table>");
        html.AppendLine("        </section>");

        // Performance Stats
        var avgDuration = _results.Any() ? _results.Average(r => r.DurationMs) : 0;
        var slowestTests = _results.OrderByDescending(r => r.DurationMs).Take(5);

        html.AppendLine("        <section class=\"performance\">");
        html.AppendLine("            <h2>Performance</h2>");
        html.AppendLine($"            <p>Average test duration: <strong>{avgDuration:F0}ms</strong></p>");
        html.AppendLine("            <h3>Slowest Tests</h3>");
        html.AppendLine("            <ol>");
        foreach (var test in slowestTests)
        {
            html.AppendLine($"                <li>{test.TestName} - {test.DurationMs}ms</li>");
        }
        html.AppendLine("            </ol>");
        html.AppendLine("        </section>");

        // Footer
        html.AppendLine("    </div>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }

    /// <summary>
    /// Generate JSON report for CI/CD integration.
    /// </summary>
    public string GenerateJsonReport()
    {
        var report = new
        {
            timestamp = DateTime.UtcNow.ToString("O"),
            summary = new
            {
                totalTests = _results.Count,
                passed = _results.Count(r => r.Status == "Passed"),
                failed = _results.Count(r => r.Status == "Failed"),
                skipped = _results.Count(r => r.Status == "Skipped"),
                passRate = _results.Any() ? (_results.Count(r => r.Status == "Passed") * 100.0 / _results.Count) : 0,
                averageDuration = _results.Any() ? _results.Average(r => r.DurationMs) : 0
            },
            results = _results
        };

        return System.Text.Json.JsonSerializer.Serialize(report, new System.Text.Json.JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
    }

    private static string GetCssStyles()
    {
        return """
            <style>
                * {
                    margin: 0;
                    padding: 0;
                    box-sizing: border-box;
                }

                body {
                    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: #333;
                    min-height: 100vh;
                    padding: 20px;
                }

                .container {
                    max-width: 1200px;
                    margin: 0 auto;
                    background: white;
                    border-radius: 10px;
                    box-shadow: 0 20px 60px rgba(0,0,0,0.3);
                    overflow: hidden;
                }

                header {
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    padding: 40px;
                    text-align: center;
                }

                header h1 {
                    font-size: 2.5em;
                    margin-bottom: 10px;
                }

                section {
                    padding: 40px;
                    border-bottom: 1px solid #eee;
                }

                section h2 {
                    font-size: 1.8em;
                    margin-bottom: 25px;
                    color: #667eea;
                }

                section h3 {
                    font-size: 1.2em;
                    margin-top: 20px;
                    margin-bottom: 10px;
                    color: #555;
                }

                .stats-grid {
                    display: grid;
                    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
                    gap: 20px;
                }

                .stat-card {
                    background: #f5f5f5;
                    padding: 25px;
                    border-radius: 8px;
                    text-align: center;
                    border-left: 4px solid #667eea;
                    transition: transform 0.3s ease;
                }

                .stat-card:hover {
                    transform: translateY(-5px);
                    box-shadow: 0 10px 20px rgba(0,0,0,0.1);
                }

                .stat-card.passed {
                    border-left-color: #10b981;
                }

                .stat-card.failed {
                    border-left-color: #ef4444;
                }

                .stat-card.skipped {
                    border-left-color: #f59e0b;
                }

                .stat-value {
                    display: block;
                    font-size: 2.5em;
                    font-weight: bold;
                    color: #667eea;
                    margin-bottom: 10px;
                }

                .stat-label {
                    display: block;
                    font-size: 0.9em;
                    color: #888;
                    text-transform: uppercase;
                    letter-spacing: 1px;
                }

                .results-table {
                    width: 100%;
                    border-collapse: collapse;
                    font-size: 0.95em;
                }

                .results-table thead {
                    background: #f5f5f5;
                    border-bottom: 2px solid #ddd;
                }

                .results-table th {
                    padding: 15px;
                    text-align: left;
                    font-weight: 600;
                    color: #555;
                }

                .results-table td {
                    padding: 12px 15px;
                    border-bottom: 1px solid #eee;
                }

                .results-table tr:hover {
                    background: #f9f9f9;
                }

                .results-table tr.passed {
                    border-left: 4px solid #10b981;
                }

                .results-table tr.failed {
                    border-left: 4px solid #ef4444;
                }

                .results-table tr.skipped {
                    border-left: 4px solid #f59e0b;
                }

                .badge {
                    display: inline-block;
                    padding: 5px 10px;
                    border-radius: 4px;
                    font-weight: 600;
                    font-size: 0.85em;
                }

                .badge.passed {
                    background: #d1fae5;
                    color: #065f46;
                }

                .badge.failed {
                    background: #fee2e2;
                    color: #991b1b;
                }

                .badge.skipped {
                    background: #fef3c7;
                    color: #92400e;
                }

                code {
                    background: #f5f5f5;
                    padding: 2px 6px;
                    border-radius: 3px;
                    font-family: 'Courier New', monospace;
                    font-size: 0.9em;
                }

                details {
                    cursor: pointer;
                }

                details summary {
                    color: #667eea;
                    text-decoration: underline;
                    user-select: none;
                }

                pre {
                    background: #f5f5f5;
                    padding: 10px;
                    border-radius: 4px;
                    overflow-x: auto;
                    font-size: 0.85em;
                    margin-top: 10px;
                }

                ol {
                    margin-left: 20px;
                }

                ol li {
                    margin-bottom: 8px;
                    padding: 8px;
                    border-left: 2px solid #ddd;
                    padding-left: 12px;
                }
            </style>
            """;
    }

    private static long ParseDuration(string duration)
    {
        if (TimeSpan.TryParse(duration, out var ts))
        {
            return (long)ts.TotalMilliseconds;
        }
        return 0;
    }
}
