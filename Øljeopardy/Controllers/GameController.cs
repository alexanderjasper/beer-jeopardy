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
using Microsoft.Extensions.Caching.Memory;

namespace Oljeopardy.Controllers
{
    public class GameController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGameRepository _gameRepository;
        private IMemoryCache _cache;

        public GameController(ICategoryRepository categoryRepository, UserManager<ApplicationUser> userManager, IGameRepository gameRepository, IMemoryCache cache)
        {
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _gameRepository = gameRepository;
            _cache = cache;
        }

        public IActionResult Add()
        {
            ViewData["Title"] = "Start nyt spil";

            var userId = _userManager.GetUserId(HttpContext.User);
            var model = new AddGameViewModel()
            {
                CategoryList = _categoryRepository.GetCategoriesByUserId(userId)
            };

            return PartialView(model);
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

            return PartialView(model);
        }

        public IActionResult CompleteParticipation(ParticipateGameViewModel model)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            _gameRepository.AddParticipant(model.ChosenGameGuid, model.ChosenCategoryGuid, Enums.TurnType.Guess, userId);
            _gameRepository.IncrementGameVersion(model.ChosenGameGuid);

            return RedirectToAction("Game", "Home");
        }

        public IActionResult SelectWinner(GameViewModel model)
        {
            try
            {
                if (model.ChosenWinnerId != null && model.Game != null)
                {
                    var userId = _userManager.GetUserId(HttpContext.User);
                    _gameRepository.SetAnswerQuestionWinner(model.ChosenWinnerId, model.Game.Id, userId);
                    _gameRepository.IncrementGameVersion(model.Game.Id);
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
                if (model.ChosenAnswerQuestionGuid != null && model.GameId != null)
                {
                    var userId = _userManager.GetUserId(HttpContext.User);
                    _gameRepository.SetSelectedAnswerQuestion(model.GameId.Value, userId, model.ChosenAnswerQuestionGuid);
                    _gameRepository.IncrementGameVersion(model.GameId.Value);
                    return RedirectToAction("Game", "Home");
                }
                throw new Exception("No answer selected");
            }
            catch
            {
                throw new Exception("Could not submit AnswerQuestion selection");
            }
        }

        public IActionResult EatYourNote(GameViewModel model)
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                var game = _gameRepository.GetActiveGameForUser(userId);
                _gameRepository.EatYourNote(userId, game.Id);
                _gameRepository.IncrementGameVersion(game.Id);
                return RedirectToAction("Game", "Home");
            }
            catch (Exception e)
            {
                throw new Exception("Could not submit Eat Your Note", e);
            }
        }

        public IActionResult LeaveGame()
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                var game = _gameRepository.GetActiveGameForUser(userId);
                var participant = _gameRepository.GetUserParticipant(game.Id, userId);
                var participants = _gameRepository.GetParticipantsForGame(game.Id);
                if (participants.Count() <= 1)
                {
                    game.GameStatus = Enums.GameStatus.Finished;
                }
                else if (participant.TurnType == Enums.TurnType.Guess)
                {
                    var firstParticipant = participants.FirstOrDefault(x => x.Id != participant.Id);
                    if (firstParticipant.TurnType == Enums.TurnType.Guess)
                    {
                        if (_categoryRepository.ParticipantHasAnswerQuestionsToSelect(game.Id, firstParticipant))
                        {
                            firstParticipant.TurnType = Enums.TurnType.Choose;
                        }
                        else if (_categoryRepository.ParticipantsGamecategoryHasAnswerQuestionsToSelect(game.Id, firstParticipant.Id))
                        {
                            firstParticipant.TurnType = Enums.TurnType.ChooseOwn;
                        }
                        else
                        {
                            game.GameStatus = Enums.GameStatus.Finished;
                        }
                        _gameRepository.UpdateParticipant(firstParticipant);
                        game.LatestCategoryChooserId = firstParticipant.UserId;
                        game.SelectedAnswerQuestionId = null;
                        game.SelectedGameCategory = null;
                        game.UserId = firstParticipant.UserId;
                    }
                    _gameRepository.UpdateGame(game);
                    _gameRepository.IncrementGameVersion(game.Id);
                }
                else
                {
                    throw new Exception("Can only leave game when user's TurnType is Guess.");
                }
                _gameRepository.DeleteParticipant(participant.Id);
                _gameRepository.UpdateGame(game);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                throw new Exception("Could not leave Game", e);
            }
        }

        [HttpGet]
        [Route("checkIfGameChanged")]
        public IActionResult CheckIfGameChanged()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var game = _gameRepository.GetActiveGameForUser(userId);

            var cachedVersion = _cache.Get("GameVersion:" + userId);

            if (cachedVersion != null)
            {
                if (game != null && cachedVersion != null && (cachedVersion.ToString() != game.Version.ToString()))
                {
                    return Ok(true);
                }
            }
            return Ok(false);
        }

        [HttpGet]
        [Route("getGameId")]
        public IActionResult GetGameId()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var game = _gameRepository.GetActiveGameForUser(userId);

            if (game != null)
            {
                return Ok(game.Id.ToString());
            }
            else
            {
                throw new Exception("Tried to load Game, but user is not participating in a Game.");
            }
        }


    }
}