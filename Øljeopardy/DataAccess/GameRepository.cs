using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Oljeopardy.Data;
using Oljeopardy.Models;
using Microsoft.AspNetCore.Identity;

namespace Oljeopardy.DataAccess
{
    public class GameRepository : IGameRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameRepository(ApplicationDbContext context, ICategoryRepository categoryRepository, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        public Game AddGame(string name, Guid chosenCategoryGuid, string userId)
        {
            try
            {
                var gameToAdd = new Game()
                {
                    UserId = userId,
                    Name = name,
                    GameStatus = Enums.GameStatus.Active,
                    ActiveTime = DateTime.Now,
                    Version = 0
                };

                var addedGame = _context.Add(gameToAdd).Entity;
                var participant = new Participant()
                {
                    GameId = addedGame.Id,
                    TurnType = Enums.TurnType.Choose,
                    UserId = userId
                };
                var addedParticipant = _context.Add(participant).Entity;
                var gameCategory = new GameCategory()
                {
                    CategoryId = chosenCategoryGuid,
                    GameId = addedGame.Id,
                    ParticipantId = addedParticipant.Id
                };
                _context.Add(gameCategory);
                _context.SaveChanges();

                return gameToAdd;
            }
            catch
            {
                throw new DataException("Could not add new game");
            }
        }

        public Game GetGameById(Guid gameId)
        {
            return _context.Games.FirstOrDefault(x => x.Id == gameId);
        }

        public List<Game> GetActiveGames()
        {
            return _context.Games
                .Where(x => x.ActiveTime >= DateTime.Now.AddHours(-2) && x.GameStatus == Enums.GameStatus.Active)
                .OrderBy(x => x.Name)
                .ToList();
        }

        public Participant AddParticipant(Guid gameId, Guid categoryId, Enums.TurnType turnType, string userId)
        {
            try
            {
                var participant = new Participant()
                {
                    UserId = userId,
                    GameId = gameId,
                    TurnType = turnType
                };

                var addedparticipant = _context.Add(participant).Entity;

                var gameCategory = new GameCategory()
                {
                    CategoryId = categoryId,
                    GameId = gameId,
                    ParticipantId = addedparticipant.Id
                };
                _context.Add(gameCategory);

                _context.SaveChanges();
                return addedparticipant;
            }
            catch
            {
                throw new DataException("Could not add new participant");
            }

        }

        public GameCategory AddGameCategory(Guid chosenCategoryId, Guid participantId, Guid gameId, string userId)
        {
            try
            {
                var gameCategory = new GameCategory()
                {
                    CategoryId = chosenCategoryId,
                    GameId = gameId,
                    ParticipantId = participantId
                };

                _context.Add(gameCategory);
                _context.SaveChanges();
                return _context.GameCategories.FirstOrDefault();
            }
            catch
            {
                throw new DataException("Could not add new game category");
            }
        }

        public Game GetActiveGameForUser(string userId)
        {
            var allGamesForUser = _context.Participants
                .Where(x => x.UserId == userId)
                .Select(x => x.GameId);
            var activeGamesForUser = _context.Games.Where(x => x.ActiveTime >= DateTime.Now.AddHours(-2) && allGamesForUser.Contains(x.Id) && x.GameStatus == Enums.GameStatus.Active);
            if (activeGamesForUser.Count() > 0)
            {
                return activeGamesForUser.FirstOrDefault();
            }
            else
            {
                return _context.Games
                    .Where(x => x.ActiveTime >= DateTime.Now.AddHours(-2) && allGamesForUser.Contains(x.Id)).OrderByDescending(x => x.ActiveTime).FirstOrDefault();
            }
        }

        public Participant GetUserParticipant(Guid gameId, string userId)
        {
            return _context.Participants.FirstOrDefault(x => x.GameId == gameId && x.UserId == userId);
        }

        public Participant GetParticipant(Guid participantId)
        {
            return _context.Participants.FirstOrDefault(x => x.Id == participantId);
        }

        public Dictionary<int, int> GetPointsForParticipant(Guid participantId)
        {
            try
            {
                var pointsDictionary = new Dictionary<int, int>
                {
                    {100, _context.GameCategories.Count(x => x.Won100ParticipantId == participantId)},
                    {200, _context.GameCategories.Count(x => x.Won200ParticipantId == participantId)},
                    {300, _context.GameCategories.Count(x => x.Won300ParticipantId == participantId)},
                    {400, _context.GameCategories.Count(x => x.Won400ParticipantId == participantId)},
                    {500, _context.GameCategories.Count(x => x.Won500ParticipantId == participantId)}
                };
                return pointsDictionary;
            }
            catch
            {
                throw new DataException("Could not get points for Participant");
            }
        }

        public int GetPointsSumForParticipant(Guid participantId)
        {
            var dic = GetPointsForParticipant(participantId);
            return 100 * dic[100] + 200 * dic[200] + 300 * dic[300] + 400 * dic[400] + 500 * dic[500];
        }

        public void SetAnswerQuestionWinner(string winnerId, Guid gameId, string submitterUserId)
        {
            try
            {
                var game = _context.Games.FirstOrDefault(x => x.Id == gameId);
                var winnerParticipant =
                    _context.Participants.FirstOrDefault(x => x.GameId == gameId && x.UserId == winnerId);
                var submitterParticipant =
                    _context.Participants.FirstOrDefault(x => x.GameId == gameId && x.UserId == submitterUserId);
                var answerQuestion = _context.AnswerQuestions.FirstOrDefault(x => x.Id == game.SelectedAnswerQuestionId);
                if (game != null &&
                    winnerParticipant != null &&
                    submitterParticipant != null &&
                    answerQuestion != null &&
                    game.GameStatus == Enums.GameStatus.Active &&
                    winnerParticipant.TurnType == Enums.TurnType.Guess &&
                    submitterParticipant.TurnType == Enums.TurnType.Read)
                {
                    if (_categoryRepository.WinnerHasAnswerQuestionsToSelect(gameId, winnerParticipant, answerQuestion.Id))
                    {
                        winnerParticipant.TurnType = Enums.TurnType.Choose;
                        submitterParticipant.TurnType = Enums.TurnType.Guess;
                    }
                    else
                    {
                        if (_categoryRepository.ParticipantsGamecategoryHasAnswerQuestionsToSelect(gameId, winnerParticipant.Id))
                        {
                            winnerParticipant.TurnType = Enums.TurnType.ChooseOwn;
                            submitterParticipant.TurnType = Enums.TurnType.Guess;
                        }
                        else
                        {
                            game.GameStatus = Enums.GameStatus.Finished;
                        }
                    }


                    var answerQuestionCategory = _categoryRepository.GetCategoryFromAnswerQuestion(answerQuestion.Id);
                    if (answerQuestionCategory != null)
                    {
                        var answerQuestionGameCategory =
                            _categoryRepository.GetGameCategoryFromAnswerQuestion(answerQuestion.Id, gameId);
                        if (answerQuestionGameCategory != null)
                        {
                            switch (_categoryRepository.GetAnswerQuestionPointsValue(answerQuestion.Id))
                            {
                                case 100:
                                    answerQuestionGameCategory.Won100ParticipantId = winnerParticipant.Id;
                                    break;
                                case 200:
                                    answerQuestionGameCategory.Won200ParticipantId = winnerParticipant.Id;
                                    break;
                                case 300:
                                    answerQuestionGameCategory.Won300ParticipantId = winnerParticipant.Id;
                                    break;
                                case 400:
                                    answerQuestionGameCategory.Won400ParticipantId = winnerParticipant.Id;
                                    break;
                                case 500:
                                    answerQuestionGameCategory.Won500ParticipantId = winnerParticipant.Id;
                                    break;
                                default:
                                    throw new Exception("Could not parse PointsValue for AnswerQuestion");
                            }
                            game.SelectedAnswerQuestionId = null;
                            game.LatestCategoryChooserId = null;
                            game.UserId = winnerId;

                            _context.Update(winnerParticipant);
                            _context.Update(submitterParticipant);
                            _context.Update(answerQuestionGameCategory);
                            _context.Update(game);

                            _context.SaveChanges();
                            return;
                        }
                    }
                }
                throw new Exception("Could not set Category winner");
            }
            catch
            {
                throw new DataException("Could not set category winner");
            }
        }

        public void SetSelectedAnswerQuestion(Guid gameId, string submitterUserId, Guid answerQuestionId)
        {
            try
            {
                var game = _context.Games.FirstOrDefault(x => x.Id == gameId);
                var submitterParticipant =
                    _context.Participants.FirstOrDefault(x => x.GameId == gameId && x.UserId == submitterUserId);
                var answerQuestion = _context.AnswerQuestions.FirstOrDefault(x => x.Id == answerQuestionId);
                if (game != null &&
                    submitterParticipant != null &&
                    answerQuestion != null &&
                    game.GameStatus == Enums.GameStatus.Active &&
                    submitterParticipant.TurnType == Enums.TurnType.Choose || submitterParticipant.TurnType == Enums.TurnType.ChooseOwn)
                {
                    var ownerParticipant = _categoryRepository.GetParticipantFromAnswerQuestion(answerQuestion.Id, gameId);
                    if (ownerParticipant == null)
                    {
                        throw new DataException("Could not find owner of AnswerQuestion");
                    }

                    game.SelectedAnswerQuestionId = answerQuestionId;
                    game.LatestCategoryChooserId = submitterUserId;
                    game.UserId = null;
                    if (submitterParticipant.TurnType != Enums.TurnType.ChooseOwn)
                    {
                        ownerParticipant.TurnType = Enums.TurnType.Read;
                        submitterParticipant.TurnType = Enums.TurnType.Guess;
                    }
                    else
                    {
                        submitterParticipant.TurnType = Enums.TurnType.Read;
                    }

                    _context.Update(ownerParticipant);
                    _context.Update(game);
                    _context.Update(submitterParticipant);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw new DataException("Could not set selected AnswerQuestion");
            }
        }

        public async Task<List<GameUser>> GetUsersForGameAsync(Guid gameId, string ownUserId = null)
        {
            var userIds = new List<string>();
            if (ownUserId != null)
            {
                userIds = _context.Participants.Where(x => x.GameId == gameId && x.UserId != ownUserId).Select(x => x.UserId).ToList();
            }
            else
            {
                userIds = _context.Participants.Where(x => x.GameId == gameId).Select(x => x.UserId).ToList();
            }
            var gameUsers = new List<GameUser>();
            foreach (var userId in userIds)
            {
                var applicationUser = await _userManager.FindByIdAsync(userId);
                if (applicationUser != null)
                {
                    var gameUser = new GameUser()
                    {
                        UserId = userId,
                        Name = applicationUser.UserName
                    };
                    gameUsers.Add(gameUser);
                }
            }
            return gameUsers;
        }

        public Game IncrementGameVersion(Guid gameId)
        {
            try
            {
                var game = _context.Games.FirstOrDefault(x => x.Id == gameId);
                if (game != null)
                {
                    game.Version += 1;
                    _context.Update(game);
                    _context.SaveChanges();
                    return game;
                }
                throw new Exception("Could not find game to increment");
            }
            catch
            {
                throw new Exception("Could not increment game version");
            }
        }

        public List<Participant> GetParticipantsForGame(Guid gameId)
        {
            try
            {
                return _context.Participants.Where(x => x.GameId == gameId).ToList();
            }
            catch
            {
                throw new Exception("Could not get participants for game");
            }
        }
    }
}
