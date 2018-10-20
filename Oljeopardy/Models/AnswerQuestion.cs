using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Oljeopardy.DataAccess;

namespace Oljeopardy.Models
{
    public class AnswerQuestion : JeopardyEntity
    {
        public string Answer { get; set; }
        public string Question { get; set; }
    }
}