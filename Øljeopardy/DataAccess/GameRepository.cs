using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Oljeopardy.Data;
using Oljeopardy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Oljeopardy.Services;

namespace Oljeopardy.DataAccess
{
    public class GameRepository : IGameRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHubContext<GameUpdateHub> _hubContext;

        public GameRepository(ApplicationDbContext context, ICategoryRepository categoryRepository, UserManager<ApplicationUser> userManager, IHubContext<GameUpdateHub> hubContext)
        {
            _context = context;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public Game AddGame(string name, Guid chosenCategoryGuid, string userId)
        {
            try
            {
                var activeGame = GetActiveGameForUser(userId);
                if (activeGame != null && activeGame.ActiveTime >= DateTime.Now.AddHours(-2) && activeGame.GameStatus == Enums.GameStatus.Active)
                {
                    throw new DataException("User is already participating in a game.");
                }
                var gameToAdd = new Game()
                {
                    UserId = userId,
                    Name = name,
                    GameStatus = Enums.GameStatus.Active,
                    ActiveTime = DateTime.Now,
                    Version = 0,
                    LatestCategoryChooserId = userId
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
            try
            {
                return _context.Games.Where(x => x.ActiveTime >= DateTime.Now.AddHours(-2) && x.GameStatus == Enums.GameStatus.Active).OrderBy(x => x.Name).ToList();
            }
            catch
            {
                throw new Exception("Could not get active Games");
            }
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
                .Where(x => x.UserId == userId && x.Deleted == null)
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
            return _context.Participants.FirstOrDefault(x => x.GameId == gameId && x.UserId == userId && x.Deleted == null);
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
                    {100, _context.GameCategories.Count(x => x.Won100ParticipantId == participantId && !x.EatYourNote100)},
                    {200, _context.GameCategories.Count(x => x.Won200ParticipantId == participantId && !x.EatYourNote200)},
                    {300, _context.GameCategories.Count(x => x.Won300ParticipantId == participantId && !x.EatYourNote300)},
                    {400, _context.GameCategories.Count(x => x.Won400ParticipantId == participantId && !x.EatYourNote400)},
                    {500, _context.GameCategories.Count(x => x.Won500ParticipantId == participantId && !x.EatYourNote500)},
                    {-100, _context.GameCategories.Count(x => x.Won100ParticipantId == participantId && x.EatYourNote100)},
                    {-200, _context.GameCategories.Count(x => x.Won200ParticipantId == participantId && x.EatYourNote200)},
                    {-300, _context.GameCategories.Count(x => x.Won300ParticipantId == participantId && x.EatYourNote300)},
                    {-400, _context.GameCategories.Count(x => x.Won400ParticipantId == participantId && x.EatYourNote400)},
                    {-500, _context.GameCategories.Count(x => x.Won500ParticipantId == participantId && x.EatYourNote500)}
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
            var positivePoints = 100 * dic[100] + 200 * dic[200] + 300 * dic[300] + 400 * dic[400] + 500 * dic[500];
            var negativePoints = -100 * dic[-100] - 200 * dic[-200] - 300 * dic[-300] - 400 * dic[-400] - 500 * dic[-500];
            return positivePoints + negativePoints;
        }

        public void SetAnswerQuestionWinner(string winnerId, Guid gameId, string submitterUserId)
        {
            try
            {
                var game = _context.Games.FirstOrDefault(x => x.Id == gameId);
                var winnerParticipant = GetUserParticipant(gameId, winnerId);
                var submitterParticipant = GetUserParticipant(gameId, submitterUserId);
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
                        game.LatestCategoryChooserId = winnerParticipant.UserId;
                        game.UserId = winnerId;

                        _context.Update(winnerParticipant);
                        _context.Update(submitterParticipant);
                        _context.Update(answerQuestionGameCategory);
                        _context.Update(game);

                        _context.SaveChanges();
                        return;
                    }
                }
                throw new Exception("Could not set Category winner");
            }
            catch
            {
                throw new DataException("Could not set category winner");
            }
        }

        public void EatYourNote(string userId, Guid gameId)
        {
            try
            {
                var game = _context.Games.FirstOrDefault(x => x.Id == gameId);
                var participant = GetUserParticipant(gameId, userId);
                var answerQuestion = _context.AnswerQuestions.FirstOrDefault(x => x.Id == game.SelectedAnswerQuestionId);
                if (game != null &&
                    participant != null &&
                    answerQuestion != null &&
                    game.GameStatus == Enums.GameStatus.Active &&
                    participant.TurnType == Enums.TurnType.Read)
                {
                    var answerQuestionGameCategory =
                        _categoryRepository.GetGameCategoryFromAnswerQuestion(answerQuestion.Id, gameId);
                    if (answerQuestionGameCategory != null)
                    {
                        switch (_categoryRepository.GetAnswerQuestionPointsValue(answerQuestion.Id))
                        {
                            case 100:
                                answerQuestionGameCategory.Won100ParticipantId = participant.Id;
                                answerQuestionGameCategory.EatYourNote100 = true;
                                break;
                            case 200:
                                answerQuestionGameCategory.Won200ParticipantId = participant.Id;
                                answerQuestionGameCategory.EatYourNote200 = true;
                                break;
                            case 300:
                                answerQuestionGameCategory.Won300ParticipantId = participant.Id;
                                answerQuestionGameCategory.EatYourNote300 = true;
                                break;
                            case 400:
                                answerQuestionGameCategory.Won400ParticipantId = participant.Id;
                                answerQuestionGameCategory.EatYourNote400 = true;
                                break;
                            case 500:
                                answerQuestionGameCategory.Won500ParticipantId = participant.Id;
                                answerQuestionGameCategory.EatYourNote500 = true;
                                break;
                            default:
                                throw new Exception("Could not parse PointsValue for AnswerQuestion");
                        }
                        game.SelectedAnswerQuestionId = null;
                        game.LatestCategoryChooserId = participant.UserId;
                        game.UserId = userId;

                        _context.Update(participant);
                        _context.Update(answerQuestionGameCategory);

                        if (_categoryRepository.WinnerHasAnswerQuestionsToSelect(gameId, participant, answerQuestion.Id))
                        {
                            participant.TurnType = Enums.TurnType.Choose;
                        }
                        else
                        {
                            if (_categoryRepository.ParticipantsGamecategoryHasAnswerQuestionsToSelect(gameId, participant.Id))
                            {
                                participant.TurnType = Enums.TurnType.ChooseOwn;
                            }
                            else
                            {
                                game.GameStatus = Enums.GameStatus.Finished;
                            }
                        }

                        _context.Update(game);

                        _context.SaveChanges();
                        return;
                    }
                }
                throw new Exception("Could not set Category winner");
            }
            catch
            {
                throw new DataException("Could not set category winner");
            }
        }

        public void DeleteParticipant(Guid participantId)
        {
            try
            {
                var participant = GetParticipant(participantId);
                if (participant == null)
                {
                    throw new DataException("Could not find participant.");
                }
                participant.Deleted = DateTime.Now;

                _context.Update(participant);
                _context.SaveChanges();
            }
            catch
            {
                throw new DataException("Could not delete participant.");
            }
        }

        public void SetSelectedAnswerQuestion(Guid gameId, string submitterUserId, Guid answerQuestionId)
        {
            try
            {
                var game = _context.Games.FirstOrDefault(x => x.Id == gameId);
                var submitterParticipant = GetUserParticipant(gameId, submitterUserId);
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
                userIds = _context.Participants.Where(x => x.GameId == gameId && x.UserId != ownUserId && x.Deleted == null).Select(x => x.UserId).ToList();
            }
            else
            {
                userIds = _context.Participants.Where(x => x.GameId == gameId && x.Deleted == null).Select(x => x.UserId).ToList();
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
                    _hubContext.Clients.Group(gameId.ToString()).InvokeAsync("GameUpdated", gameId.ToString());
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
                return _context.Participants.Where(x => x.GameId == gameId && x.Deleted == null).ToList();
            }
            catch
            {
                throw new Exception("Could not get participants for game");
            }
        }

        public Participant UpdateParticipant(Participant participant)
        {
            try
            {
                var DbParticipant = _context.Update(participant);
                _context.SaveChanges();
                return DbParticipant.Entity;
            }
            catch
            {
                throw new DataException("Could not update participant.");
            }
        }

        public Game UpdateGame(Game game)
        {
            try
            {
                var DbGame = _context.Update(game);
                _context.SaveChanges();
                return DbGame.Entity;
            }
            catch
            {
                throw new DataException("Could not update participant.");
            }
        }
    }
}
