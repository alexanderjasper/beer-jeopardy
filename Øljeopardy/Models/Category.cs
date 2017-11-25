using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models
{
    public class Category : JeopardyEntity
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public AnswerQuestion AnswerQuestion100 { get; set; }
        public AnswerQuestion AnswerQuestion200 { get; set; }
        public AnswerQuestion AnswerQuestion300 { get; set; }
        public AnswerQuestion AnswerQuestion400 { get; set; }
        public AnswerQuestion AnswerQuestion500 { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
