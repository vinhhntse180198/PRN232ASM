using System.Text.Json.Serialization;

namespace SyncService.Application.DTOs.OpenAlex;

public class OpenAlexWorksResponse
{
    [JsonPropertyName("results")]
    public List<OpenAlexWork> Results { get; set; } = [];
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

    [JsonPropertyName("publication_year")]
    public int? PublicationYear { get; set; }

    [JsonPropertyName("cited_by_count")]
    public int CitedByCount { get; set; }

    [JsonPropertyName("abstract_inverted_index")]
    public Dictionary<string, List<int>>? AbstractInvertedIndex { get; set; }

    [JsonPropertyName("authorships")]
    public List<OpenAlexAuthorship> Authorships { get; set; } = [];

    [JsonPropertyName("primary_location")]
    public OpenAlexPrimaryLocation? PrimaryLocation { get; set; }

    [JsonPropertyName("keywords")]
    public List<OpenAlexKeyword> Keywords { get; set; } = [];

    [JsonPropertyName("topics")]
    public List<OpenAlexTopic> Topics { get; set; } = [];
}

public class OpenAlexAuthorship
{
    [JsonPropertyName("author")]
    public OpenAlexAuthor? Author { get; set; }
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
