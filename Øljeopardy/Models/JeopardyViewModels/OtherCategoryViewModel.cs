using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models.JeopardyViewModels
{
    public class OtherCategoryViewModel
    {
        public Category Category { get; set; }
        public string OwnerUserName { get; set; }
        public bool Saved { get; set; }
    }
}
