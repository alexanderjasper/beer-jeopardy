using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models
{
    public class UserCategory : JeopardyEntity
    {
        public string UserId { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
