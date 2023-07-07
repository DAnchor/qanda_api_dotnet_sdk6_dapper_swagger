using AutoMapper;
using QandA.DTOs.Payload.Response;
using QandA.DTOs.Payload.Request;

namespace QandA.Api.Configuration.MapperOptions;

public class QuestionsProfile : Profile
{
    public QuestionsProfile()
    {
        // requests
        CreateMap<QuestionPostRequest, QuestionPostFullRequest>();
        CreateMap<AnswerPostRequest, AnswerPostFullRequest>();
        // responses
        CreateMap<QuestionGetWithoutAnswerResponse, QuestionGetSingleResponse>();
        CreateMap<QuestionGetSingleResponse, QuestionGetSingleResponse>();
    }
}