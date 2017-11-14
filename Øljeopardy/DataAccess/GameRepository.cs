using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Oljeopardy.Data;
using Oljeopardy.Models;

namespace Oljeopardy.DataAccess
{
    public class GameRepository : IGameRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryRepository _categoryRepository;

        public GameRepository(ApplicationDbContext context, ICategoryRepository categoryRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;
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
                .Where(x => x.ActiveTime >= DateTime.Now.AddHours(-2))
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
            return _context.Games
                .FirstOrDefault(x => x.ActiveTime >= DateTime.Now.AddHours(-2) && allGamesForUser.Contains(x.Id));
        }

        public Participant GetUserParticipant(Guid gameId, string userId)
        {
            return _context.Participants.FirstOrDefault(x => x.GameId == gameId && x.UserId == userId);
        }

        public Participant GetParticipant(Guid participantId)
        {
            return _context.Participants.FirstOrDefault(x => x.Id == participantId);
        }

        public Dictionary<int,int> GetPointsForParticipant(Guid participantId)
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

        public void SetAnswerQuestionWinner(string winnerId, Guid gameId, string submitterUserId, Guid answerQuestionId)
        {
            try
            {
                var game = _context.Games.FirstOrDefault(x => x.Id == gameId);
                var winnerParticipant =
                    _context.Participants.FirstOrDefault(x => x.GameId == gameId && x.UserId == winnerId);
                var submitterParticipant =
                    _context.Participants.FirstOrDefault(x => x.GameId == gameId && x.UserId == submitterUserId);
                var answerQuestion = _context.AnswerQuestions.FirstOrDefault(x => x.Id == answerQuestionId);
                if (game != null &&
                    winnerParticipant != null &&
                    submitterParticipant != null &&
                    answerQuestion != null &&
                    game.GameStatus == Enums.GameStatus.Active &&
                    winnerParticipant.TurnType == Enums.TurnType.Guess &&
                    submitterParticipant.TurnType == Enums.TurnType.Read)
                {
                    winnerParticipant.TurnType = Enums.TurnType.Choose;
                    submitterParticipant.TurnType = Enums.TurnType.Guess;


                    var answerQuestionCategory = _categoryRepository.GetCategoryFromAnswerQuestion(answerQuestionId);
                    if (answerQuestionCategory != null)
                    {
                        var answerQuestionGameCategory =
                            _categoryRepository.GetGameCategoryFromAnswerQuestion(answerQuestionId, gameId);
                        if (answerQuestionGameCategory != null)
                        {
                            switch (_categoryRepository.GetAnswerQuestionPointsValue(answerQuestionId))
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
                    submitterParticipant.TurnType == Enums.TurnType.Choose)
                {
                    game.SelectedAnswerQuestionId = answerQuestionId;
                    submitterParticipant.TurnType = Enums.TurnType.Guess;
                }
            }
            catch
            {
                throw new DataException("Could not set selected AnswerQuestion");
            }
        }
    }
}
