namespace ruhanBack.Interfaces
{
public interface IOtpRepository
{
    Task<string> GenerateOtp(string email);
    Task<string> ValidateOtp(string email, DateTime date, string otp);
}
}