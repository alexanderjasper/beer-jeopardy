using System;
using System.Diagnostics;
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

        public IActionResult Game(AddGameViewModel model)
        {
            ViewData["Title"] = "Spil";

            var returnModel = new GameViewModel();

            var userId = _userManager.GetUserId(HttpContext.User);
            if (model != null && model.ChosenCategoryGuid != Guid.Empty)
            {
                var addedGame = _gameRepository.AddGame(model.GameName, model.ChosenCategoryGuid, userId);
                returnModel.Game = addedGame;
            }
            else
            {
                var activeGame = _gameRepository.GetActiveGameForUser(userId) ?? new Game();
                returnModel.Game = activeGame;
            }
            return View(returnModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
