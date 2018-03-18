using Oljeopardy.Models;

namespace Oljeopardy.DataAccess
{
    public interface IUserRepository
    {
        ApplicationUser GetUserById(string userId);
    }
}
