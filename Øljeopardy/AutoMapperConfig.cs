using AutoMapper;
using Oljeopardy.Models;
using Oljeopardy.Models.JeopardyViewModels;

namespace Oljeopardy
{
    public static class AutoMapperConfig
    {
        public class AutoMapperProfileConfiguration : Profile
        {
            public AutoMapperProfileConfiguration()
            {
                CreateMap<Category, CategoryViewModel>().ReverseMap();
                CreateMap<AnswerQuestion, AnswerQuestionViewModel>().ReverseMap();
                CreateMap<GameUser, GameUserViewModel>().ReverseMap();
            }
        }
    }
}