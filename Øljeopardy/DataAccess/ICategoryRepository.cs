using System;
using System.Collections.Generic;
using Oljeopardy.Models;

namespace Oljeopardy.DataAccess
{
    public interface ICategoryRepository
    {
        Category GetCategoryById(Guid categoryId);
        AnswerQuestion GetAnswerQuestionById(Guid answerQuestionId);
        bool UpdateCategory(Category category, string userId);
        bool AddCategory(Category category, string userId);
        List<Category> GetCategoriesByUserId(string userId);
        Category GetUsersCategoryForActiveGame(Guid gameId, string userId);
        List<GameCategory> GetGameCategoriesForGame(Guid gameId);
        Category GetCategoryFromAnswerQuestion(Guid answerQuestionId);
        GameCategory GetGameCategoryFromAnswerQuestion(Guid answerquestionId, Guid gameId);
    }
}
