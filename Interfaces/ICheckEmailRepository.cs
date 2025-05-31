namespace ruhanBack.Interfaces
{
public interface ICheckEmailRepository
{
    Task<string> CheckEmail(string email);
    Task<string> CheckEmailVerified(string email);
    Task verifyEmailRequest(string email);

    Task<string> verifyEmailValidation(string email);

}
}