using Microsoft.AspNetCore.Mvc;
using WebApplication3.Model;
using WebApplication3.View;

namespace WebApplication3.Service
{
    public interface IUserService
    {
        Task<ActionResult<UserViewModel>> CreateAsync(UserViewModel user);
        Task <User> loginAsync(UserViewModel user);
        Task<AuthResultViewModel> GetTokenAsync(User authRequest,string ipAddress);
        Task<AuthResultViewModel> GetRefreshTokenAsync(string ipAddress, int UserId,string Email,RefreshToken userRefreshToken);
        Task<bool> IsTokenValid(string accessToken, string ipAddress);

        




    }
}
