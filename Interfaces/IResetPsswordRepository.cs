
namespace ruhanBack.Interfaces
{
public interface IResetPasswordRepository
{
 Task<string> ResetPassword(string email, string token, string password);
}
}
