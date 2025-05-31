using Microsoft.AspNetCore.Identity;

namespace ruhanBack.Interfaces
{
    public interface IAuthRepository
    {
        Task<string>Login(string email, string password);  
        Task<IdentityResult> Register(string userName, string email, string password);  
    }
}
