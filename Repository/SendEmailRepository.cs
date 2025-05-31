using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using ruhanBack.Interfaces;

public class SendEmailRepository : ISendEmailRepository
{
    private readonly IConfiguration _configuration;

    public SendEmailRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmail(string email, string subject, string message)
    {
        var host = _configuration["EmailSettings:Host"];
        var port = int.Parse(_configuration["EmailSettings:Port"]);
        var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];
        var from = _configuration["EmailSettings:From"];

        using (var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            Credentials = new NetworkCredential(username, password)
        })
        using (var mail = new MailMessage(from, email)
        {
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        })
        {
            await client.SendMailAsync(mail);
        }
    }
}
