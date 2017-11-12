using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models.JeopardyViewModels
{
    public class ParticipateGameViewModel
    {
        public List<Game> GameList { get; set; }

        public List<Category> CategoryList { get; set; }
        
        [Required]
        public Guid ChosenGameGuid { get; set; }

        [Required]
        public Guid ChosenCategoryGuid { get; set; }
    }
}
