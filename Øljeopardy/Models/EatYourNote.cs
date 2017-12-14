using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models
{
    public class EatYourNote : JeopardyEntity
    {
        public string UserId { get; set; }
        public Guid GameId { get; set; }
        public Guid AnswerQuestionId { get; set; }
    }
}
