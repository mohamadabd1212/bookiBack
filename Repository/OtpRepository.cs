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
        try{
        Random random = new Random();
        string otpvalue = random.Next(100000, 1000000).ToString();
        var otp = new Otp{
            Id = Guid.NewGuid().ToString(),
            email=email,
            otp=otpvalue,
            date=DateTime.UtcNow
        };
        _context.otp.Add(otp);
        await _context.SaveChangesAsync();

        await _sendEmailRepository.SendEmail(email,"Otp",otp.otp);

        return "OTP sent successfully";
        }
        catch
    {
        return "Error Sending The Email";
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
