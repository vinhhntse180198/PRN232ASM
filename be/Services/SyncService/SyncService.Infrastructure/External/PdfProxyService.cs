using SyncService.Application.Interfaces;

namespace SyncService.Infrastructure.External;

public class PdfProxyService
{
    private readonly HttpClient _http;

    public PdfProxyService(HttpClient http) => _http = http;

    public async Task<(Stream Stream, string ContentType)> FetchPdfAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new InvalidOperationException("Invalid PDF URL.");

        if (uri.Scheme is not "http" and not "https")
            throw new InvalidOperationException("Invalid PDF URL scheme.");

        using var response = await _http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Could not fetch PDF ({(int)response.StatusCode}).");

        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/pdf";
        var isPdf = contentType.Contains("pdf", StringComparison.OrdinalIgnoreCase)
            || uri.AbsolutePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

        if (!isPdf)
            throw new InvalidOperationException("URL does not point to a PDF file.");

        await using var source = await response.Content.ReadAsStreamAsync(cancellationToken);
        var memory = new MemoryStream();
        await source.CopyToAsync(memory, cancellationToken);
        memory.Position = 0;
        return (memory, contentType);
    }
}
