using System.Net;
using System.Text.RegularExpressions;
using SyncService.Application.DTOs;
using SyncService.Application.Interfaces;

namespace SyncService.Infrastructure.External;

public partial class DoiResolver : IDoiResolver
{
    private readonly HttpClient _http;

    public DoiResolver(HttpClient http) => _http = http;

    public async Task<ResolvedPaperUrls> ResolveAsync(string doi, CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeDoi(doi);
        if (string.IsNullOrWhiteSpace(normalized))
            throw new InvalidOperationException("DOI is required.");

        var landingUrl = await FollowDoiRedirectAsync(normalized, cancellationToken);
        var pdfUrl = await TryFindPdfOnPageAsync(landingUrl, cancellationToken);

        return new ResolvedPaperUrls
        {
            LandingUrl = landingUrl,
            PdfUrl = pdfUrl
        };
    }

    private async Task<string?> FollowDoiRedirectAsync(string doi, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"https://doi.org/{doi}");
        request.Headers.TryAddWithoutValidation("Accept", "text/html");

        using var response = await _http.SendAsync(request, cancellationToken);
        var finalUrl = response.RequestMessage?.RequestUri?.ToString();

        if (!string.IsNullOrWhiteSpace(finalUrl) && !finalUrl.Contains("doi.org", StringComparison.OrdinalIgnoreCase))
            return finalUrl;

        if (response.Headers.Location is not null)
        {
            var location = response.Headers.Location.ToString();
            if (!location.Contains("doi.org", StringComparison.OrdinalIgnoreCase))
                return location.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                    ? location
                    : $"https://doi.org{location}";
        }

        return finalUrl;
    }

    private async Task<string?> TryFindPdfOnPageAsync(string? pageUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(pageUrl)) return null;

        try
        {
            var html = await _http.GetStringAsync(pageUrl, cancellationToken);
            var match = PdfHrefRegex().Match(html);
            if (!match.Success) return null;

            var href = match.Groups[1].Value;
            return ToAbsoluteUrl(pageUrl, href);
        }
        catch
        {
            return null;
        }
    }

    private static string? ToAbsoluteUrl(string baseUrl, string href)
    {
        if (string.IsNullOrWhiteSpace(href)) return null;
        if (href.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return href;

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri)) return null;
        return Uri.TryCreate(baseUri, href, out var absolute) ? absolute.ToString() : null;
    }

    private static string? NormalizeDoi(string? doi)
    {
        if (string.IsNullOrWhiteSpace(doi)) return null;
        return doi
            .Replace("https://doi.org/", "", StringComparison.OrdinalIgnoreCase)
            .Replace("http://doi.org/", "", StringComparison.OrdinalIgnoreCase)
            .Trim();
    }

    [GeneratedRegex("""href=["']([^"']*\.pdf[^"']*)["']""", RegexOptions.IgnoreCase)]
    private static partial Regex PdfHrefRegex();
}
