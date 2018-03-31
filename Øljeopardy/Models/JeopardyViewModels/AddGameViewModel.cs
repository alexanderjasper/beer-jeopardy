using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models.JeopardyViewModels
{
    public class AddGameViewModel
    {
        [Required]
        [Display(Name = "Spillets navn")]
        public string GameName { get; set; }

        public List<Category> OwnCategories { get; set; }
        public List<Category> SavedCategories { get; set; }

        public Guid ChosenOwnCategoryGuid { get; set; }
        public Guid ChosenSavedCategoryGuid { get; set; }
    }
}
