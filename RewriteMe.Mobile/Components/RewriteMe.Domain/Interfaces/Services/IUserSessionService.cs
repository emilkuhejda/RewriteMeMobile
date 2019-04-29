﻿using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IUserSessionService
    {
        Task<string> GetUserIdAsync();

        Task<string> GetUserNameAsync();

        Task<UserSession> GetUserSessionAsync();

        Task<string> GetAccessTokenSilentAsync();

        Task<bool> IsSignedInAsync();

        Task<bool> SignUpOrInAsync();

        Task<bool> EditProfileAsync();

        Task<bool> ResetPasswordAsync();

        Task SignOutAsync();
    }
}
