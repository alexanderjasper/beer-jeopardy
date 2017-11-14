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
        private readonly ICategoryRepository _categoryRepository;

        public AnswerQuestion(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public string Answer { get; set; }
        public string Question { get; set; }

        public int PointsValue
        {
            get
            {
                var category = _categoryRepository.GetCategoryFromAnswerQuestion(Id);
                if (category == null)
                    throw new DataException("Could not find category from AnswerQuestion");
                if (category.AnswerQuestion100 != null && Id == category.AnswerQuestion100.Id)
                    return 100;
                if (category.AnswerQuestion200 != null && Id == category.AnswerQuestion200.Id)
                    return 200;
                if (category.AnswerQuestion300 != null && Id == category.AnswerQuestion300.Id)
                    return 300;
                if (category.AnswerQuestion400 != null && Id == category.AnswerQuestion400.Id)
                    return 400;
                if (category.AnswerQuestion500 != null && Id == category.AnswerQuestion500.Id)
                    return 500;
                return 0;
            }
        }
    }
}