using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oljeopardy.Models;

namespace Oljeopardy.DataAccess
{
    public interface IGameRepository
    {
        Game AddGame(string name, Guid chosenCategoryGuid, string userId);
        Game GetGameById(Guid gameId);
        List<Game> GetActiveGames();
        Participant AddParticipant(Guid gameId, Enums.TurnType turnType, string userId);
        GameCategory AddGameCategory(Guid chosenCategoryId, Guid participantId, Guid gameId, string userId);
        Game GetActiveGameForUser(string userId);
    }
}
