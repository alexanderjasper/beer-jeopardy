using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models
{
    public class GameCategory : JeopardyEntity
    {
        public Guid CategoryId { get; set; }
        public Guid GameId { get; set; }
        public Guid ParticipantId { get; set; }
        public Guid Won100UserId { get; set; }
        public Guid Won200UserId { get; set; }
        public Guid Won300UserId { get; set; }
        public Guid Won400UserId { get; set; }
        public Guid Won500UserId { get; set; }

        public Category Category { get; set; }
        public Game Game { get; set; }
        public Participant Participant { get; set; }
    }
}
