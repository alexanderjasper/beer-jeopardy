using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models.JeopardyViewModels
{
    public class GameViewModel
    {
        public Game Game { get; set; }
        public Guid? GameId { get; set; }
        public Category UsersOwnCategory { get; set; }
        public List<GameCategoryViewModel> OtherGameCategories { get; set; }
        public string LatestCategoryChooserName { get; set; }
        public string CategoryOwnerName { get; set; }
        public string ChosenCategoryName { get; set; }
        public Enums.TurnType UserTurnType { get; set; }
        public List<GameUserViewModel> UserList { get; set; }
        public HighscoreViewmodel Highscore { get; set; }
        public AnswerQuestionViewModel ChosenAnswerQuestion { get; set; }
        public int ChosenAnswerQuestionPoints { get; set; }
        public bool ActiveGameExists { get; set; }

        public string ChosenWinnerId { get; set; }
        public Guid ChosenAnswerQuestionGuid { get; set; }
    }
}
