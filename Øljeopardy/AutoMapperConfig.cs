using AutoMapper;
using Øljeopardy.Models;
using Øljeopardy.Models.JeopardyViewModels;

namespace Øljeopardy
{
    public static class AutoMapperConfig
    {
        public class AutoMapperProfileConfiguration : Profile
        {
            public AutoMapperProfileConfiguration()
            {
                CreateMap<Category, CategoryViewModel>().ReverseMap();
                CreateMap<AnswerQuestion, AnswerQuestionViewModel>().ReverseMap();
            }
        }
    }
}