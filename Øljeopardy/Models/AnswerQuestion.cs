using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models
{
    public class AnswerQuestion : JeopardyEntity
    {
        public string Answer { get; set; }
        public string Question { get; set; }
    }
}
