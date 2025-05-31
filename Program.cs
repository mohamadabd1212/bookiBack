using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ruhanBack.Data;
using ruhanBack.Interfaces;
using ruhanBack.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJsOrigin", policy =>
    {
        policy.WithOrigins(builder.Configuration["front-url"])  // Your Next.js frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // ✅ ALLOW COOKIES
    });
});

// Register ApplicationDbContext with SQL Server connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";  // Allow spaces
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Register your custom repository for authentication
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ISendEmailRepository, SendEmailRepository>();
builder.Services.AddScoped<IResetPasswordRepository, ResetPasswordRepository>();
builder.Services.AddScoped<ICheckEmailRepository, CheckEmailRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();
 
builder.Services.AddHttpContextAccessor();
var app = builder.Build();
app.UseCookiePolicy();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable CORS by specifying the policy name
app.UseCors("AllowNextJsOrigin");

app.UseRouting();

// Authentication and Authorization middleware
app.UseAuthentication();  
app.UseAuthorization();   

// Define the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    

app.Run();
