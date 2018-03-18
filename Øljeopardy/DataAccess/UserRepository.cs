using System.Linq;
using Oljeopardy.Data;
using Oljeopardy.Models;

namespace Oljeopardy.DataAccess
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser GetUserById(string userId)
        {
            return _context.Users.FirstOrDefault(x => x.Id == userId);
        }
    }
}
