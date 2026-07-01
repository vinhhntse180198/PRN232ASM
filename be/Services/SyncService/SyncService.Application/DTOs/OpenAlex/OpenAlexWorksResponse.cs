using System.Text.Json.Serialization;

namespace SyncService.Application.DTOs.OpenAlex;

public class OpenAlexWorksResponse
{
    [JsonPropertyName("results")]
    public List<OpenAlexWork> Results { get; set; } = [];

    [JsonPropertyName("meta")]
    public OpenAlexMeta? Meta { get; set; }
}

public class OpenAlexMeta
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("page")]
    public int? Page { get; set; }

    [JsonPropertyName("per_page")]
    public int? PerPage { get; set; }
}

public class OpenAlexWork
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("doi")]
    public string? Doi { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("abstract")]
    public string? Abstract { get; set; }

    [JsonPropertyName("abstract_inverted_index")]
    public Dictionary<string, int[]>? AbstractInvertedIndex { get; set; }

    [JsonPropertyName("publication_year")]
    public int? PublicationYear { get; set; }

    [JsonPropertyName("publication_date")]
    public string? PublicationDate { get; set; }

    [JsonPropertyName("cited_by_count")]
    public int CitedByCount { get; set; }

    [JsonPropertyName("authorships")]
    public List<OpenAlexAuthorship> Authorships { get; set; } = [];

    [JsonPropertyName("primary_location")]
    public OpenAlexPrimaryLocation? PrimaryLocation { get; set; }

    [JsonPropertyName("open_access")]
    public OpenAlexOpenAccess? OpenAccess { get; set; }

    [JsonPropertyName("keywords")]
    public List<OpenAlexKeyword> Keywords { get; set; } = [];

    [JsonPropertyName("topics")]
    public List<OpenAlexTopic> Topics { get; set; } = [];
}

public class OpenAlexAuthorship
{
    [JsonPropertyName("author")]
    public OpenAlexAuthor? Author { get; set; }

    [JsonPropertyName("author_position")]
    public string? AuthorPosition { get; set; }
}

public class OpenAlexAuthor
{
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
}

public class OpenAlexPrimaryLocation
{
    [JsonPropertyName("source")]
    public OpenAlexSource? Source { get; set; }

    [JsonPropertyName("landing_page_url")]
    public string? LandingPageUrl { get; set; }

    [JsonPropertyName("pdf_url")]
    public string? PdfUrl { get; set; }

    [JsonPropertyName("is_oa")]
    public bool IsOa { get; set; }
}

public class OpenAlexOpenAccess
{
    [JsonPropertyName("is_oa")]
    public bool IsOa { get; set; }

    [JsonPropertyName("oa_url")]
    public string? OaUrl { get; set; }

    [JsonPropertyName("oa_status")]
    public string? OaStatus { get; set; }
}

public class OpenAlexSource
{
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
}

public class OpenAlexKeyword
{
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
}

public class OpenAlexTopic
{
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
}
