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

        public GameRepository(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
