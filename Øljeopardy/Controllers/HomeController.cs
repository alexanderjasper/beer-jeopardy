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

namespace Oljeopardy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMapper Mapper { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGameRepository _gameRepository;

        public HomeController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager, ICategoryRepository categoryRepository, IGameRepository gameRepository)
        {
            _context = context;
            Mapper = mapper;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _gameRepository = gameRepository;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Øljeopardy";

            return View();
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
                foreach (var gameCategory in gameCategories.Where(x => x.ParticipantId != userParticipant.Id))
                {
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

                if (model.Game.SelectedGameCategory != null)
                {
                    var categoryOwnerParticipant =
                        _gameRepository.GetParticipant(model.Game.SelectedGameCategory.ParticipantId);
                    model.CategoryOwnerName =
                        _userManager.FindByIdAsync(categoryOwnerParticipant.UserId).Result.UserName;
                }

                if (model.Game.SelectedAnswerQuestionId != null)
                {
                    var chosenCategory =
                        _categoryRepository.GetCategoryFromAnswerQuestion(model.Game.SelectedAnswerQuestionId.Value);
                    model.ChosenCategoryName = chosenCategory.Name;
                }
            }

            return model;
        }
    }
}
