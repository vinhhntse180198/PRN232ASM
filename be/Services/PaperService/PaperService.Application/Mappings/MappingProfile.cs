using AutoMapper;
using PRN232ASM.PaperService.Application.DTOs.Responses;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ResearchPaper, PaperSummaryResponse>()
            .ForMember(d => d.JournalName, o => o.MapFrom(s => s.Journal.Name))
            .ForMember(d => d.Authors, o => o.MapFrom(s => s.PaperAuthors.OrderBy(pa => pa.AuthorOrder).Select(pa => pa.Author.Name).ToList()))
            .ForMember(d => d.Keywords, o => o.MapFrom(s => s.PaperKeywords.Select(pk => pk.Keyword.Name).ToList()));

        CreateMap<ResearchPaper, PaperDetailResponse>()
            .IncludeBase<ResearchPaper, PaperSummaryResponse>()
            .ForMember(d => d.Topics, o => o.MapFrom(s => s.PaperTopics.Select(pt => pt.Topic.Name).ToList()));

        CreateMap<Author, AuthorResponse>();
        CreateMap<Journal, JournalResponse>();
        CreateMap<Keyword, KeywordResponse>();
        CreateMap<ResearchTopic, TopicResponse>();
        CreateMap<Bookmark, BookmarkResponse>()
            .ForMember(d => d.PaperTitle, o => o.MapFrom(s => s.Paper.Title));
    }
}
