using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using ruhanBack.Interfaces;
using System;

public class SendEmailRepository : ISendEmailRepository
{
    private readonly IConfiguration _configuration;
    private readonly SmtpClient _client;

    public SendEmailRepository(IConfiguration configuration)
    {
        _configuration = configuration;

        // Initialize the SMTP client once to avoid overhead per request
        _client = new SmtpClient
        {
            Host = _configuration["EmailSettings:Host"],
            Port = int.Parse(_configuration["EmailSettings:Port"]),
            EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]),
            Credentials = new NetworkCredential(
                _configuration["EmailSettings:Username"],
                _configuration["EmailSettings:Password"]
            )
        };
    }

    public async Task SendEmail(string email, string subject, string message)
    {
        try
        {
            using var mail = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:From"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mail.To.Add(email);

            await _client.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            // Consider logging this exception
            throw new InvalidOperationException("Failed to send email.", ex);
        }
    }
}
