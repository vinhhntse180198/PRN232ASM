using SyncService.Application.Interfaces;
using SyncService.Infrastructure.External;
using Microsoft.AspNetCore.Mvc;

namespace SyncService.Api.Controllers;

[ApiController]
[Route("api/openalex")]
public class OpenAlexController : ControllerBase
{
    private readonly IOpenAlexSearchService _searchService;
    private readonly IDoiResolver _doiResolver;
    private readonly PdfProxyService _pdfProxy;

    public OpenAlexController(
        IOpenAlexSearchService searchService,
        IDoiResolver doiResolver,
        PdfProxyService pdfProxy)
    {
        _searchService = searchService;
        _doiResolver = doiResolver;
        _pdfProxy = pdfProxy;
    }

    /// <summary>
    /// Tìm kiếm trực tiếp trên OpenAlex (giống openalex.org). Miễn phí với API key.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string query,
        [FromQuery] short? year,
        [FromQuery] bool openAccessOnly = false,
        [FromQuery] bool paywalledOnly = false,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 25,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _searchService.SearchAsync(query, year, openAccessOnly, paywalledOnly, page, perPage, cancellationToken);
            return Ok(new { data = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

  /// <summary>
  /// Duyệt tất cả bài nghiên cứu trên OpenAlex, lọc theo năm.
  /// </summary>
    [HttpGet("browse")]
    public async Task<IActionResult> Browse(
        [FromQuery] short? year,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 25,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _searchService.BrowseAllAsync(year, page, perPage, cancellationToken);
            return Ok(new { data = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("browse/paid")]
    public async Task<IActionResult> BrowsePaid(
        [FromQuery] short? year,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 25,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _searchService.BrowsePaywalledAsync(year, page, perPage, cancellationToken);
            return Ok(new { data = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("works/{openAlexId}")]
    public async Task<IActionResult> GetWork(string openAlexId, CancellationToken cancellationToken)
    {
        try
        {
            var paper = await _searchService.GetWorkAsync(openAlexId, cancellationToken);
            return Ok(new { data = paper });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("resolve")]
    public async Task<IActionResult> ResolveDoi([FromQuery] string doi, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _doiResolver.ResolveAsync(doi, cancellationToken);
            return Ok(new { data = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> ProxyPdf(
        [FromQuery] string url,
        [FromQuery] bool download = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (stream, contentType) = await _pdfProxy.FetchPdfAsync(url, cancellationToken);
            var disposition = download ? "attachment" : "inline";
            Response.Headers.Append("Content-Disposition", $"{disposition}; filename=\"paper.pdf\"");
            return File(stream, contentType);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("import/{openAlexId}")]
    public async Task<IActionResult> Import(string openAlexId, CancellationToken cancellationToken)
    {
        try
        {
            var paper = await _searchService.ImportToLibraryAsync(openAlexId, cancellationToken);
            return Ok(new { data = paper, message = "Saved to library." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
