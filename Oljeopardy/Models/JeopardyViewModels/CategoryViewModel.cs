using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models.JeopardyViewModels
{
    public class CategoryViewModel
    {
        [Required]
        [Display(Name = "Navn")]
        public string Name { get; set; }
        public Guid Id { get; set; }

        public AnswerQuestionViewModel AnswerQuestion100 { get; set; }
        public AnswerQuestionViewModel AnswerQuestion200 { get; set; }
        public AnswerQuestionViewModel AnswerQuestion300 { get; set; }
        public AnswerQuestionViewModel AnswerQuestion400 { get; set; }
        public AnswerQuestionViewModel AnswerQuestion500 { get; set; }
    }
}
