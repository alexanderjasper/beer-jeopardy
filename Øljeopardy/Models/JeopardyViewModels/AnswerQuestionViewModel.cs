using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Øljeopardy.Models.JeopardyViewModels
{
    public class AnswerQuestionViewModel
    {
        [Display(Name = "Svar")]
        public string Answer { get; set; }

        [Display(Name = "Spørgsmål")]
        public string Question { get; set; }
    }
}
