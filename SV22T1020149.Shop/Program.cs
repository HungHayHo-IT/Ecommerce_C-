using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using SV22T1020149.BusinessLayers;
using SV22T1020149.Shop.AppCodes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
    .AddMvcOptions(option =>
    {
        option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "LiteCommerce.Shop";
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();

// Session configuration (used for shopping cart)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var cultureInfo = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Configure ApplicationContext
ApplicationContext.Configure(
    httpContextAccessor: app.Services.GetRequiredService<IHttpContextAccessor>(),
    webHostEnvironment: app.Services.GetRequiredService<IWebHostEnvironment>(),
    configuration: app.Configuration
);

// Initialize business layer (safe for development when DB is unavailable)
var connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    try
    {
        Configuration.Initialize(connectionString);
        // Ensure Customers.Password column is large enough for hashed passwords
        try
        {
            var logger = app.Services.GetService(typeof(ILogger<Program>)) as ILogger;
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            var sql = @"IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Customers') AND name = N'Password')
                        BEGIN
                            ALTER TABLE dbo.Customers ADD [Password] NVARCHAR(500) NULL;
                        END
                        ELSE
                        BEGIN
                            DECLARE @len int = (SELECT max_length FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Customers') AND name = N'Password');
                            IF @len < 500
                                ALTER TABLE dbo.Customers ALTER COLUMN [Password] NVARCHAR(500) NULL;
                        END";
            using var cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetService(typeof(ILogger<Program>)) as ILogger;
            logger?.LogWarning(ex, "Failed to ensure Customers.Password column length.");
        }
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetService(typeof(Microsoft.Extensions.Logging.ILogger<Program>)) as Microsoft.Extensions.Logging.ILogger;
        logger?.LogWarning(ex, "Failed to initialize data layer. The application will continue without database connectivity.");
    }
}
else
{
    var logger = app.Services.GetService(typeof(Microsoft.Extensions.Logging.ILogger<Program>)) as Microsoft.Extensions.Logging.ILogger;
    logger?.LogWarning("Connection string 'LiteCommerceDB' is not configured. Skipping data layer initialization.");
}

app.Run();
