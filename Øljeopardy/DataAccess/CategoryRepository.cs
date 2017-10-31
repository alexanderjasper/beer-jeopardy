using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Øljeopardy.Data;
using Øljeopardy.Models;

namespace Øljeopardy.DataAccess
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Category GetCategoryById(Guid categoryId)
        {
            var category = _context.Categories
                .Include(x => x.AnswerQuestion100)
                .Include(x => x.AnswerQuestion200)
                .Include(x => x.AnswerQuestion300)
                .Include(x => x.AnswerQuestion400)
                .Include(x => x.AnswerQuestion500)
                .FirstOrDefault(x => x.Id == categoryId);
            return category;
        }

        public bool UpdateCategory(Category category, string userId)
        {
            try
            {
                category.UserId = userId;
                _context.Update(category);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddCategory(Category category, string userId)
        {
            try
            {
                category.UserId = userId;
                _context.Add(category);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Category> GetCategoriesByUserId(string userId)
        {
            var categories = new List<Category>();
            using (_context)
            {
                var query = from c in _context.Categories
                    where c.UserId == userId
                    orderby c.Name
                    select c;
                foreach (var category in query)
                {
                    categories.Add(category);
                }
            }
            return categories;
        }
    }
}
