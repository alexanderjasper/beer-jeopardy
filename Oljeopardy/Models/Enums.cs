using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Oljeopardy.Models
{
    public class Enums
    {
        public enum TurnType
        {
            Guess,
            Read,
            Choose,
            ChooseOwn
        }

        public enum GameStatus
        {
            Active,
            Finished
        }

        public enum CategoriesPageAction
        {
            Nothing,
            AddedCategory,
            EditedCategory,
            DeletedCategory
        }
    }
}
