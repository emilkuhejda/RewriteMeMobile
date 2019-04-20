using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IUserSessionService
    {
        Task<bool> SignUpOrInAsync();
    }
}
