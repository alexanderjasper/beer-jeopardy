using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oljeopardy.Data;
using Oljeopardy.Models;
using System.Globalization;

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

        public bool DeleteCategory(Category category, string userId)
        {
            try
            {
                category.UserId = userId;
                category.Deleted = DateTime.Now;
                _context.Update(category);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                throw new Exception("Could not delete category");
            }
        }

        public bool SaveCategory(Guid categoryId, string userId)
        {
            try
            {
                var allUserCategories = GetUserCategoriesByUser(userId);
                if (allUserCategories.Any(x => x.CategoryId == categoryId))
                {
                    throw new Exception("Category is already saved");
                }
                else
                {
                    var userCategory = new UserCategory()
                    {
                        CategoryId = categoryId,
                        UserId = userId
                    };
                    _context.UserCategories.Add(userCategory);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not save category", e);
            }
        }

        public bool UnsaveCategory(Guid categoryId, string userId)
        {
            try
            {
                var userCategory = GetUserCategoriesByUser(userId).FirstOrDefault(x => x.CategoryId == categoryId && x.UserId == userId);
                if (userCategory == null)
                {
                    throw new Exception("Category is not saved");
                }
                else
                {
                    _context.UserCategories.Remove(userCategory);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not unsave category", e);
            }
        }

        private List<UserCategory> GetUserCategoriesByUser(string userId)
        {
            try
            {
                return _context.UserCategories.Where(x => x.UserId == userId).ToList();
            }
            catch (Exception e)
            {
                throw new Exception("Could not find UserCategories", e);
            }
        }

        public List<Category> GetCategoriesByUserId(string userId)
        {
            return _context.Categories
                .Where(c => c.UserId == userId && c.Deleted == null)
                .AsEnumerable()
                .OrderBy(c => c.Name, StringComparer.Create(new CultureInfo("da-DK"), true)).ToList();
        }

        public List<Category> GetOtherCategories(string userId)
        {
            var userCategories = GetUserCategoriesByUser(userId).Select(x => x.CategoryId);
            return _context.Categories
                .Where(c => c.UserId != userId && c.Deleted == null && c.Shared && !userCategories.Contains(c.Id))
                .ToList();
        }

        public List<Category> GetSavedCategories(string userId)
        {
            var userCategories = GetUserCategoriesByUser(userId).Select(x => x.CategoryId);
            var returnList = new List<Category>();
            foreach (var userCategory in userCategories)
            {
                returnList.Add(_context.Categories.FirstOrDefault(x => x.Id == userCategory));
            }
            return returnList;
        }

        public Category GetUsersCategoryForActiveGame(Guid gameId, string userId)
        {
            var participant = _context.Participants.FirstOrDefault(x => x.GameId == gameId && x.UserId == userId && x.Deleted == null);
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
            try
            {
                var returnList = new List<GameCategory>();
                var participants = _context.Participants.Where(x => x.GameId == gameId && x.Deleted == null);
                foreach (var participant in participants)
                {
                    var gameCategory = _context.GameCategories.FirstOrDefault(x => x.ParticipantId == participant.Id);
                    returnList.Add(gameCategory);
                }
                return returnList;
            }
            catch
            {
                throw new Exception("Could not get gamecategories for game");
            }
        }

        public int GetAnswerQuestionPointsValue(Guid answerQuestionId)
        {
            try
            {
                var category = GetCategoryFromAnswerQuestion(answerQuestionId);
                if (category == null)
                    throw new DataException("Could not find category from AnswerQuestion");
                if (category.AnswerQuestion100 != null && answerQuestionId == category.AnswerQuestion100.Id)
                    return 100;
                if (category.AnswerQuestion200 != null && answerQuestionId == category.AnswerQuestion200.Id)
                    return 200;
                if (category.AnswerQuestion300 != null && answerQuestionId == category.AnswerQuestion300.Id)
                    return 300;
                if (category.AnswerQuestion400 != null && answerQuestionId == category.AnswerQuestion400.Id)
                    return 400;
                if (category.AnswerQuestion500 != null && answerQuestionId == category.AnswerQuestion500.Id)
                    return 500;
                return 0;
            }
            catch
            {
                throw new Exception("Could not get Points for AnswerQuestion");
            }
        }

        public Participant GetParticipantFromAnswerQuestion(Guid answerQuestionId, Guid gameId)
        {
            try
            {
                var category = _context.Categories
                    .FirstOrDefault(x => x.AnswerQuestion100.Id == answerQuestionId ||
                    x.AnswerQuestion200.Id == answerQuestionId ||
                    x.AnswerQuestion300.Id == answerQuestionId ||
                    x.AnswerQuestion400.Id == answerQuestionId ||
                    x.AnswerQuestion500.Id == answerQuestionId);
                if (category == null)
                {
                    throw new Exception("Could not get category from AnswerQuestion");
                }
                var userId = category.UserId;
                return _context.Participants.FirstOrDefault(x => x.GameId == gameId && x.UserId == userId && x.Deleted == null);
            }
            catch
            {
                throw new DataException("Could not get Participant from AnswerQuestion");
            }
        }

        public List<GameCategory> GetOtherPlayersGameCategories(Guid gameId, Guid participantId)
        {
            try
            {
                return GetGameCategoriesForGame(gameId).Where(x => x.ParticipantId != participantId).ToList();
            }
            catch
            {
                throw new Exception("Could not get gamecategories for game");
            }
        }

        public bool ParticipantHasAnswerQuestionsToSelect(Guid gameId, Participant winnerParticipant)
        {
            try
            {
                if (winnerParticipant == null)
                {
                    throw new Exception("Winner participant is null");
                }
                var otherPlayersGamecategories = GetOtherPlayersGameCategories(gameId, winnerParticipant.Id);
                foreach (var gameCategory in otherPlayersGamecategories)
                {
                    var category = GetCategoryById(gameCategory.CategoryId);
                    if (category != null)
                    {
                        if ((gameCategory.Won100ParticipantId == Guid.Empty) ||
                            (gameCategory.Won200ParticipantId == Guid.Empty) ||
                            (gameCategory.Won300ParticipantId == Guid.Empty) ||
                            (gameCategory.Won400ParticipantId == Guid.Empty) ||
                            (gameCategory.Won500ParticipantId == Guid.Empty))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        throw new Exception("Could not get winner's Category");
                    }
                }
                return false;
            }
            catch
            {
                throw new Exception("Could not determine if winner has AnswerQuestions to select");
            }
        }

        public bool WinnerHasAnswerQuestionsToSelect(Guid gameId, Participant winnerParticipant, Guid chosenAnswerQuestionId)
        {
            try
            {
                if (winnerParticipant == null)
                {
                    throw new Exception("Winner participant is null");
                }
                var otherPlayersGamecategories = GetOtherPlayersGameCategories(gameId, winnerParticipant.Id);
                foreach (var gameCategory in otherPlayersGamecategories)
                {
                    var category = GetCategoryById(gameCategory.CategoryId);
                    if (category != null)
                    {
                        if ((gameCategory.Won100ParticipantId == Guid.Empty && category.AnswerQuestion100.Id != chosenAnswerQuestionId) ||
                            (gameCategory.Won200ParticipantId == Guid.Empty && category.AnswerQuestion200.Id != chosenAnswerQuestionId) ||
                            (gameCategory.Won300ParticipantId == Guid.Empty && category.AnswerQuestion300.Id != chosenAnswerQuestionId) ||
                            (gameCategory.Won400ParticipantId == Guid.Empty && category.AnswerQuestion400.Id != chosenAnswerQuestionId) ||
                            (gameCategory.Won500ParticipantId == Guid.Empty && category.AnswerQuestion500.Id != chosenAnswerQuestionId))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        throw new Exception("Could not get winner's Category");
                    }
                }
                return false;
            }
            catch
            {
                throw new Exception("Could not determine if winner has AnswerQuestions to select");
            }
        }

        public bool ParticipantsGamecategoryHasAnswerQuestionsToSelect(Guid gameId, Guid participantId)
        {
            var gameCategory = _context.GameCategories.FirstOrDefault(x => x.GameId == gameId && x.ParticipantId == participantId);
            if (gameCategory != null)
            {
                if (gameCategory.Won100ParticipantId == Guid.Empty ||
                    gameCategory.Won200ParticipantId == Guid.Empty ||
                    gameCategory.Won300ParticipantId == Guid.Empty ||
                    gameCategory.Won400ParticipantId == Guid.Empty ||
                    gameCategory.Won500ParticipantId == Guid.Empty)
                {
                    return true;
                }
            }
            else
            {
                throw new Exception("Could not determine if participant has AnswerQuestions to select");
            }
            return false;
        }

        public bool ShareCategory(Guid categoryId, string userId)
        {
            try
            {
                var category = _context.Categories.FirstOrDefault(x => x.Id == categoryId && x.UserId == userId);
                if (category == null)
                {
                    throw new Exception("Could not find category to share.");
                }
                else
                {
                    category.Shared = true;
                    _context.Categories.Update(category);
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Could not share category.");
            }
        }
    }
}
