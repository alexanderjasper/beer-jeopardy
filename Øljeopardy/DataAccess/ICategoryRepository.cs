using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Øljeopardy.Models;

namespace Øljeopardy.DataAccess
{
    public interface ICategoryRepository
    {
        Category GetCategoryById(Guid categoryId);
        bool UpdateCategory(Category category, string userId);
        bool AddCategory(Category category, string userId);
        List<Category> GetCategoriesByUserId(string userId);
    }
}
