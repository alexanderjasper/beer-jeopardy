using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oljeopardy.Data;
using Oljeopardy.Models;

namespace Oljeopardy.DataAccess
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Category GetCategoryById(Guid categoryId)
        {
            var category = _context.Categories
                .Include(x => x.AnswerQuestion100)
                .Include(x => x.AnswerQuestion200)
                .Include(x => x.AnswerQuestion300)
                .Include(x => x.AnswerQuestion400)
                .Include(x => x.AnswerQuestion500)
                .FirstOrDefault(x => x.Id == categoryId);
            return category;
        }

        public AnswerQuestion GetAnswerQuestionById(Guid answerQuestionId)
        {
            return _context.AnswerQuestions.FirstOrDefault(x => x.Id == answerQuestionId);
        }

        public bool UpdateCategory(Category category, string userId)
        {
            try
            {
                category.UserId = userId;
                _context.Update(category);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddCategory(Category category, string userId)
        {
            try
            {
                category.UserId = userId;
                _context.Add(category);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Category> GetCategoriesByUserId(string userId)
        {
            using (_context)
            {
                return _context.Categories
                    .Where(c => c.UserId == userId)
                    .OrderBy(c => c.Name).ToList();
            }
        }

        public Category GetUsersCategoryForActiveGame(Guid gameId, string userId)
        {
            var participant = _context.Participants.FirstOrDefault(x => x.GameId == gameId && x.UserId == userId);
            var gameCategory =
                _context.GameCategories.FirstOrDefault(x => x.GameId == gameId && x.ParticipantId == participant.Id);
            return _context.Categories.FirstOrDefault(x => x.Id == gameCategory.CategoryId);
        }

        public Category GetCategoryFromAnswerQuestion(Guid answerQuestionId)
        {
            try
            {
                return _context.Categories.FirstOrDefault(x => x.AnswerQuestion100.Id == answerQuestionId || x.AnswerQuestion200.Id == answerQuestionId ||
                    x.AnswerQuestion300.Id == answerQuestionId || x.AnswerQuestion400.Id == answerQuestionId ||
                    x.AnswerQuestion500.Id == answerQuestionId);
            }
            catch
            {
                throw new DataException("Could not get Category from AnswerQuestion");
            }
        }

        public GameCategory GetGameCategoryFromAnswerQuestion(Guid answerquestionId, Guid gameId)
        {
            try
            {
                var category = GetCategoryFromAnswerQuestion(answerquestionId);
                if (category != null)
                {
                    return _context.GameCategories.FirstOrDefault(
                        x => x.CategoryId == category.Id && x.GameId == gameId);
                }
                throw new DataException("No Category founr for AnswerQuestion");
            }
            catch
            {
                throw new DataException("Could not get GameCategory from AnswerQuestion");
            }
        }

        public List<GameCategory> GetGameCategoriesForGame(Guid gameId)
        {
            return _context.GameCategories.Where(x => x.GameId == gameId).ToList();
        }
    }
}
