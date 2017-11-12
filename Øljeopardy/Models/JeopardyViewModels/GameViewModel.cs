using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models.JeopardyViewModels
{
    public class GameViewModel
    {
        public Game Game { get; set; }
        public Category UsersOwnCategory { get; set; }
        public List<GameCategoryViewModel> OtherGameCategories { get; set; }
        public string LatestCategoryChooserName { get; set; }
        public string CategoryOwnerName { get; set; }
        public string ChosenCategoryName { get; set; }
    }
}
