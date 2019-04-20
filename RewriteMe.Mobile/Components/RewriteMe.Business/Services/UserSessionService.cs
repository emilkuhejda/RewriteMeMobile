using System.Threading.Tasks;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Services
{
    public class UserSessionService: IUserSessionService
    {
        public UserSessionService()
        {
        }

        public async Task<bool> SignUpOrInAsync()
        {
            await Task.CompletedTask;

            return false;
        }
    }
}
