using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ServiceCommon.Domain.Services;
using ServiceCommon.Infrastructure.DataBase;
using ServiceCommon.Domain.Interfaces;
using ServiceCommon.Infrastructure.Email;
using ServiceCommon.Application.Services;
using ServiceClients.Domain.Interfaces;
using ServiceClients.Application.Services;
using ServiceClients.Infrastructure.Repositories;
using ServiceUsers.Domain.Interfaces;
using ServiceUsers.Application.Services;
using ServiceUsers.Infrastructure.Auth;
using ServiceUsers.Infrastructure.Repositories;
using ServiceUsers.Infrastructure.Security;
using ServiceProducts.Domain.Interfaces;
using ServiceProducts.Application.Services;
using ServiceProducts.Infrastructure.Repositories;
using ServiceDistributors.Domain.Interfaces;
using ServiceDistributors.Application.Services;
using ServiceDistributors.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.RootDirectory = "/Pages";
        options.Conventions.AllowAnonymousToPage("/Auth/Login");
        options.Conventions.AllowAnonymousToPage("/Auth/Logout");
        options.Conventions.AllowAnonymousToPage("/Index");
    });

// Get connection string and create singleton database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var database = DataBaseConnection.GetInstance(connectionString);
builder.Services.AddSingleton<IDataBase>(database);

// Configure JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<ServiceUsers.Infrastructure.Auth.JwtOptions>() 
    ?? new ServiceUsers.Infrastructure.Auth.JwtOptions();
builder.Services.AddSingleton(jwtOptions);

// Configure SendGrid
var sendGridSection = builder.Configuration.GetSection("SendGrid");
var sendGridOptions = sendGridSection.Get<SendGridOptions>() 
    ?? new SendGridOptions();
builder.Services.AddSingleton(sendGridOptions);
builder.Services.AddSingleton<IEmailService, SendGridEmailService>();

// Register Services
builder.Services.AddSingleton<IClientRepository, ClientRepository>();
builder.Services.AddSingleton<IClientService, ClientService>();

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IUserService, ServiceUsers.Application.Services.UserService>();
builder.Services.AddSingleton<IPasswordGenerator, ServiceUsers.Infrastructure.Security.SecurePasswordGenerator>();
builder.Services.AddSingleton<IUsernameGenerator, ServiceUsers.Infrastructure.Security.UsernameGenerator>();
builder.Services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IJwtAuthService, JwtAuthService>();

builder.Services.AddSingleton<IProductRepository, ServiceProducts.Infrastructure.Repositories.ProductRepository>();
builder.Services.AddSingleton<ServiceProducts.Domain.Interfaces.ICategoryRepository, ServiceProducts.Infrastructure.Repositories.CategoryRepository>();
builder.Services.AddSingleton<IProductService, ServiceProducts.Application.Services.ProductService>();

builder.Services.AddSingleton<IDistributorRepository, ServiceDistributors.Infrastructure.Repositories.DistributorRepository>();
builder.Services.AddSingleton<IDistributorService, ServiceDistributors.Application.Services.DistributorService>();

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "AppScheme";
    options.DefaultChallengeScheme = "AppScheme";
})
.AddPolicyScheme("AppScheme", "App Auth Scheme", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        var path = context.Request.Path.Value ?? string.Empty;
        return path.StartsWith("/api", StringComparison.OrdinalIgnoreCase)
            ? JwtBearerDefaults.AuthenticationScheme
            : CookieAuthenticationDefaults.AuthenticationScheme;
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/Login";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
    options.AddPolicy("RequireEmployeeOrAdmin", p => p.RequireRole("Admin", "Employee"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Middleware for validation exceptions
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (ValidationException vex)
    {
        var wantsJson = context.Request.Path.StartsWithSegments("/api")
                        || (context.Request.Headers.TryGetValue("Accept", out var a) && a.ToString().Contains("application/json"));

        if (wantsJson)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            var errors = vex.Errors.Select(e => new { field = e.Field, message = e.Message });
            await context.Response.WriteAsJsonAsync(new { errors });
            return;
        }
        throw;
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// API Endpoints
app.MapPost("/api/auth/login", async (IJwtAuthService auth, ServiceUsers.Application.DTOs.AuthRequestDto req, CancellationToken ct) =>
{
    var result = await auth.SignInAsync(req, ct);
    return result.IsSuccess && result.Value is not null
        ? Results.Ok(result.Value)
        : Results.Unauthorized();
}).AllowAnonymous();

app.Run();
