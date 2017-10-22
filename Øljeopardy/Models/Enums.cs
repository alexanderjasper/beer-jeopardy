using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Øljeopardy.Models
{
    public class Enums
    {
        public enum TurnType
        {
            Guess,
            Read,
            Won
        }

        public enum GameStatus
        {
            Active,
            Inactive
        }

        public enum CategoriesPageAction
        {
            Nothing,
            AddedCategory,
            EditedCategory
        }
    }
}
