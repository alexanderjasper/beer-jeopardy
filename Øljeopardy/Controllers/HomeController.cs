using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Oljeopardy.Data;
using Oljeopardy.DataAccess;
using Oljeopardy.Models;
using Oljeopardy.Models.JeopardyViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace Oljeopardy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMapper Mapper { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGameRepository _gameRepository;
        private IMemoryCache _cache;

        public HomeController(ApplicationDbContext context, 
            IMapper mapper, 
            UserManager<ApplicationUser> userManager, 
            ICategoryRepository categoryRepository, 
            IGameRepository gameRepository,
            IMemoryCache cache)
        {
            _context = context;
            Mapper = mapper;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _gameRepository = gameRepository;
            _cache = cache;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Øljeopardy";

            var userId = _userManager.GetUserId(HttpContext.User);
            var activeGame = _gameRepository.GetActiveGameForUser(userId);
            bool hasActiveGame = false;
            if (activeGame != null && activeGame.GameStatus == Enums.GameStatus.Active)
            {
                hasActiveGame = true;
            }

            var activeGames = _gameRepository.GetActiveGames();
            bool activeGameExists = false;
            if (activeGames != null && activeGames.Count() > 0)
            {
                activeGameExists = true;
            }

            var categories = _categoryRepository.GetCategoriesByUserId(userId);
            bool hasCategory = false;
            if (categories != null && categories.Count() > 0)
            {
                hasCategory = true;
            }

            var model = new HomeViewModel()
            {
                HasActiveGame = hasActiveGame,
                HasCategory = hasCategory,
                ActiveGameExists = activeGameExists
            };

            return View(model);
        }

        public IActionResult Categories(Enums.CategoriesPageAction pageAction)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var model = new CategoriesViewModel()
            {
                PageAction = pageAction,
                CategoryList = _categoryRepository.GetCategoriesByUserId(userId)
            };

            var message = "";
            switch (model.PageAction)
            {
                case Enums.CategoriesPageAction.AddedCategory:
                    message = "Kategori tilføjet";
                    break;
                case Enums.CategoriesPageAction.EditedCategory:
                    message = "Kategori redigeret";
                    break;
            }
            ViewData["Message"] = message;
            ViewData["Title"] = "Kategorier";

            return View(model);
        }

        public IActionResult Rules()
        {
            ViewData["Title"] = "Regler";

            return View();
        }

        public IActionResult Game()
        {
            ViewData["Title"] = "Spil";

            var userId = _userManager.GetUserId(HttpContext.User);
            var activeGame = _gameRepository.GetActiveGameForUser(userId) ?? new Game();

            var gameViewModel = new GameViewModel();
            if (activeGame.Id != Guid.Empty)
            {
                gameViewModel = GetGameViewModel(activeGame.Id, userId);
            }

            return View(gameViewModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private GameViewModel GetGameViewModel(Guid gameId, string userId)
        {
            var model = new GameViewModel();
            model.Game = _gameRepository.GetGameById(gameId);
            if (model.Game != null)
            {
                model.UsersOwnCategory = _categoryRepository.GetUsersCategoryForActiveGame(gameId, userId);
                var userParticipant = _gameRepository.GetUserParticipant(gameId, userId);
                if (userParticipant != null)
                {
                    model.UserTurnType = userParticipant.TurnType;
                }

                var gameCategories = _categoryRepository.GetGameCategoriesForGame(gameId);
                model.OtherGameCategories = new List<GameCategoryViewModel>();
                if (userParticipant.TurnType == Enums.TurnType.Choose)
                {
                    foreach (var gameCategory in gameCategories.Where(x => x.ParticipantId != userParticipant.Id))
                    {
                        var gameCategoryViewModel = new GameCategoryViewModel()
                        {
                            GameCategory = gameCategory,
                            Category = _categoryRepository.GetCategoryById(gameCategory.CategoryId)
                        };
                        model.OtherGameCategories.Add(gameCategoryViewModel);
                    }
                }
                else
                {
                    var gameCategory = gameCategories.FirstOrDefault(x => x.ParticipantId == userParticipant.Id);
                    var gameCategoryViewModel = new GameCategoryViewModel()
                    {
                        GameCategory = gameCategory,
                        Category = _categoryRepository.GetCategoryById(gameCategory.CategoryId)
                    };
                    model.OtherGameCategories.Add(gameCategoryViewModel);

                }

                if (model.Game.LatestCategoryChooserId != null)
                {
                    model.LatestCategoryChooserName =
                        _userManager.FindByIdAsync(model.Game.LatestCategoryChooserId).Result.UserName;
                }

                if (model.Game.SelectedAnswerQuestionId != null && model.Game.SelectedAnswerQuestionId.Value != Guid.Empty)
                {
                    var categoryOwnerParticipant =
                        _categoryRepository.GetParticipantFromAnswerQuestion(model.Game.SelectedAnswerQuestionId.Value, model.Game.Id);
                    model.CategoryOwnerName =
                        _userManager.FindByIdAsync(categoryOwnerParticipant.UserId).Result.UserName;
                }

                if (model.Game.SelectedAnswerQuestionId != null)
                {
                    var chosenCategory =
                        _categoryRepository.GetCategoryFromAnswerQuestion(model.Game.SelectedAnswerQuestionId.Value);
                    model.ChosenCategoryName = chosenCategory.Name;
                }

                var gameUsersTask = _gameRepository.GetUsersForGameAsync(gameId, userId);
                gameUsersTask.Wait();
                var gameUsers = gameUsersTask.Result;
                model.UserList = Mapper.Map<List<GameUserViewModel>>(gameUsers);

                model.Highscore = GetHighScoreViewModel(model.Game.Id);

                if (model.Game.SelectedAnswerQuestionId != null)
                {
                    var chosenAnswerQuestion = _categoryRepository.GetAnswerQuestionById(model.Game.SelectedAnswerQuestionId.Value);
                    model.ChosenAnswerQuestion = Mapper.Map<AnswerQuestionViewModel>(chosenAnswerQuestion);
                    model.ChosenAnswerQuestionPoints = _categoryRepository.GetAnswerQuestionPointsValue(chosenAnswerQuestion.Id);
                }

                _cache.Set("GameVersion:" + userId, model.Game.Version);
            }

            return model;
        }

        private HighscoreViewmodel GetHighScoreViewModel(Guid gameId)
        {
            try
            {
                var model = new HighscoreViewmodel()
                {
                    HighscoreEntries = new List<HighscoreEntryViewmodel>()
                };
                var participants = _gameRepository.GetParticipantsForGame(gameId);
                foreach (var participant in participants)
                {
                    model.HighscoreEntries.Add(new HighscoreEntryViewmodel()
                    {
                        UserName = _userManager.FindByIdAsync(participant.UserId).Result.UserName,
                        Score = _gameRepository.GetPointsSumForParticipant(participant.Id)
                    });
                }
                model.HighscoreEntries = model.HighscoreEntries.OrderByDescending(x => x.Score).ToList();
                return model;
            }
            catch
            {
                throw new Exception("Could not get high score view model");
            }
        }
    }
}
