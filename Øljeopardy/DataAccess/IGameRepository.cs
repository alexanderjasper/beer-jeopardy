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
        Participant AddParticipant(Guid gameId, Guid participantId, Enums.TurnType turnType, string userId);
        GameCategory AddGameCategory(Guid chosenCategoryId, Guid participantId, Guid gameId, string userId);
        Game GetActiveGameForUser(string userId);
        Participant GetUserParticipant(Guid gameId, string userId);
        Participant GetParticipant(Guid participantId);
        Dictionary<int, int> GetPointsForParticipant(Guid participantId);
        void SetAnswerQuestionWinner(string winnerId, Guid gameId, string submitterUserId);
        void SetSelectedAnswerQuestion(Guid gameId, string submitterUserId, Guid answerQuestionId);
        Task<List<GameUser>> GetUsersForGameAsync(Guid gameId, string userId = null);
        Game IncrementGameVersion(Guid gameId);
    }
}
