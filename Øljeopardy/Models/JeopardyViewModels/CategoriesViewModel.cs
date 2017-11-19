using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models.JeopardyViewModels
{
    public class CategoriesViewModel
    {
        public Enums.CategoriesPageAction PageAction { get; set; }

        public List<Category> CategoryList { get; set; }
        public Guid? ChosenCategoryGuid { get; set; }
    }
}
