using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Øljeopardy.Models.Enums;

namespace Øljeopardy.Models
{
    public class Game : JeopardyEntity
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public GameStatus GameStatus { get; set; }
        public DateTime ActiveTime { get; set; }
        public int Version { get; set; }
        public ApplicationUser LatestChooser { get; set; }
        public Guid SelectedGameCategoryId { get; set; }

        public GameCategory SelectedGameCategory { get; set; }
    }
}
