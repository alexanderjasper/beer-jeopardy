using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Øljeopardy.Models.Enums;

namespace Øljeopardy.Models
{
    public class Participant : JeopardyEntity
    {
        public string UserId { get; set; }
        public Guid GameId { get; set; }
        public TurnType TurnType { get; set; }

        public Game Game { get; set; }
        public int Points
        {
            get { throw new NotImplementedException(); }
        }
    }
}
