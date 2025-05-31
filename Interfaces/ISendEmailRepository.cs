namespace ruhanBack.Interfaces
{
public interface ISendEmailRepository
{
    Task SendEmail(string email, string subject, string message);
    }
}