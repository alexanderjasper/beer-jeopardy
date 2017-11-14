using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Oljeopardy.DataAccess;
using Oljeopardy.Models;
using Oljeopardy.Models.JeopardyViewModels;

namespace Oljeopardy.Controllers
{
    public class GameController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGameRepository _gameRepository;

        public GameController(ICategoryRepository categoryRepository, UserManager<ApplicationUser> userManager, IGameRepository gameRepository)
        {
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _gameRepository = gameRepository;
        }

        public IActionResult Add()
        {
            ViewData["Title"] = "Start nyt spil";

            var userId = _userManager.GetUserId(HttpContext.User);
            var model = new AddGameViewModel()
            {
                CategoryList = _categoryRepository.GetCategoriesByUserId(userId)
            };

            return View(model);
        }

        public IActionResult CompleteAdd(AddGameViewModel model)
        {

            var userId = _userManager.GetUserId(HttpContext.User);
            if (model != null && model.ChosenCategoryGuid != Guid.Empty)
            {
                _gameRepository.AddGame(model.GameName, model.ChosenCategoryGuid, userId);
            }

            return RedirectToAction("Game", "Home");
        }

        public IActionResult Participate()
        {
            ViewData["Title"] = "Deltag i et spil";
            var userId = _userManager.GetUserId(HttpContext.User);

            var model = new ParticipateGameViewModel()
            {
                GameList = _gameRepository.GetActiveGames(),
                CategoryList = _categoryRepository.GetCategoriesByUserId(userId)
            };

            return View(model);
        }

        public IActionResult CompleteParticipation(ParticipateGameViewModel model)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            _gameRepository.AddParticipant(model.ChosenGameGuid, model.ChosenCategoryGuid, Enums.TurnType.Guess, userId);

            return RedirectToAction("Game", "Home");
        }

        public IActionResult SelectWinner(GameViewModel model)
        {
            try
            {
                if (model.ChosenWinnerId != null && model.Game != null)
                {
                    var userId = _userManager.GetUserId(HttpContext.User);
                    _gameRepository.SetAnswerQuestionWinner(model.ChosenWinnerId, model.Game.Id, userId, model.ChosenAnswerQuestionGuid);
                    return RedirectToAction("Game", "Home");
                }
                throw new Exception("No winner was chosen");
            }
            catch
            {
                throw new Exception("Could not submit winner");
            }
        }

        public IActionResult SelectAnswer(GameViewModel model)
        {
            try
            {
                if (model.ChosenWinnerId != null && model.Game != null)
                {
                    var userId = _userManager.GetUserId(HttpContext.User);
                    _gameRepository.SetSelectedAnswerQuestion(model.Game.Id, userId, model.ChosenAnswerQuestionGuid);
                    return RedirectToAction("Game", "Home");
                }
                throw new Exception("No answer selected");
            }
            catch
            {
                throw new Exception("Could not submit AnswerQuestion selection");
            }
        }
    }
}