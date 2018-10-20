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
        public Guid Won100ParticipantId { get; set; }
        public Guid Won200ParticipantId { get; set; }
        public Guid Won300ParticipantId { get; set; }
        public Guid Won400ParticipantId { get; set; }
        public Guid Won500ParticipantId { get; set; }
        public bool EatYourNote100 { get; set; }
        public bool EatYourNote200 { get; set; }
        public bool EatYourNote300 { get; set; }
        public bool EatYourNote400 { get; set; }
        public bool EatYourNote500 { get; set; }

        public Category Category { get; set; }
        public Game Game { get; set; }
        public Participant Participant { get; set; }
    }
}
