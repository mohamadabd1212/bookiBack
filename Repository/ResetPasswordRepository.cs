using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ruhanBack.Interfaces;


public class ResetPasswordRepository : IResetPasswordRepository
{
    private readonly UserManager<IdentityUser> _userManager;

    public ResetPasswordRepository(UserManager<IdentityUser> userManager){
        _userManager=userManager;
    }

    public async Task<string> ResetPassword(string email, string token, string password)
{
    var user = await _userManager.FindByEmailAsync(email);
    var result = await _userManager.ResetPasswordAsync(user, token, password);

    if (result.Succeeded)
    {
        return "Password reset successfully";
    }

    return "enter a valid password with 8 characters and 1 special character and 1 capital letter";
}


}
