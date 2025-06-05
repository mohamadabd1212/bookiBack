using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ruhanBack.Interfaces;
using ruhanBack.Data;
using ruhanBack.models;
using Microsoft.EntityFrameworkCore;


public class OtpRepository : IOtpRepository
{
    private readonly ApplicationDbContext _context;

    private readonly ISendEmailRepository _sendEmailRepository;

    public OtpRepository(ApplicationDbContext context,ISendEmailRepository sendEmailRepository)
    {
        _context=context;
        _sendEmailRepository=sendEmailRepository;
    }

    public async Task<string> GenerateOtp(string email)
{
    try
    {
        Random random = new Random();
        string otpValue = random.Next(100000, 1000000).ToString();

        var otp = new Otp
        {
            Id = Guid.NewGuid().ToString(),
            email = email,
            otp = otpValue,
            date = DateTime.UtcNow
        };

        _context.otp.Add(otp);
        await _context.SaveChangesAsync();

        // Fire-and-forget email sending (does not delay response)
        _ = Task.Run(() => _sendEmailRepository.SendEmail(email, "Otp", otpValue));

        return "OTP sent successfully";
    }
    catch
    {
        return "Error sending the OTP";
    }
}


   public async Task<string> ValidateOtp(string email, DateTime date, string otp)
{
    var storedOtp = await _context.otp
        .FirstOrDefaultAsync(o => o.email == email && o.otp == otp);

    if (storedOtp == null)
    {
        return "Invalid Otp Try Again";
    }
    if ((DateTime.UtcNow - storedOtp.date).TotalMinutes > 2)
    {
        return "Expired Otp";
    }
    return "Valid";
}


}
