using System.Text;
using BookstoreManagementSystem.Application.DTOs;
using BookstoreManagementSystem.Infrastructure.Auth;
using BookstoreManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
using System.Linq;
using BookstoreManagementSystem.Application.Services;
using Microsoft.AspNetCore.Http;
using BookstoreManagementSystem.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.RootDirectory = "/Web/Pages";
        // Razor Pages conventions for authorization
        // Products: list visible to Employee/Admin; CRUD only for Admin
        options.Conventions.AuthorizePage("/Products/Index", "RequireEmployeeOrAdmin");
        options.Conventions.AuthorizePage("/Products/Create", "RequireAdmin");
        options.Conventions.AuthorizePage("/Products/Edit", "RequireAdmin");
        options.Conventions.AuthorizePage("/Products/Delete", "RequireAdmin");
        options.Conventions.AuthorizePage("/Auth/Profile");
        // Clients: full CRUD for Employee/Admin
        options.Conventions.AuthorizeFolder("/Clients", "RequireEmployeeOrAdmin");
        // Distributors: list visible to Employee/Admin; CRUD only for Admin
        options.Conventions.AuthorizePage("/Distributors/Index", "RequireEmployeeOrAdmin");
        options.Conventions.AuthorizePage("/Distributors/Create", "RequireAdmin");
        options.Conventions.AuthorizePage("/Distributors/Edit", "RequireAdmin");
        options.Conventions.AuthorizePage("/Distributors/Delete", "RequireAdmin");
        options.Conventions.AuthorizeFolder("/Users", "RequireAdmin");
        options.Conventions.AllowAnonymousToPage("/Auth/Login");
        options.Conventions.AllowAnonymousToPage("/Auth/Logout");
    });

// Options
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
builder.Services.AddSingleton(jwtOptions);

var sendGridSection = builder.Configuration.GetSection("SendGrid");
var sendGridOptions = sendGridSection.Get<BookstoreManagementSystem.Infrastructure.Email.SendGridOptions>() 
    ?? new BookstoreManagementSystem.Infrastructure.Email.SendGridOptions();
builder.Services.AddSingleton(sendGridOptions);

// DI
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IDistributorRepository, DistributorRepository>();
builder.Services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IJwtAuthService, JwtAuthService>();
builder.Services.AddSingleton<IEmailService, BookstoreManagementSystem.Infrastructure.Email.SendGridEmailService>();
builder.Services.AddSingleton<IPasswordGenerator, BookstoreManagementSystem.Infrastructure.Security.SecurePasswordGenerator>();
builder.Services.AddSingleton<IUsernameGenerator, BookstoreManagementSystem.Infrastructure.Security.UsernameGenerator>();

// Authentication schemes
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
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
    options.AddPolicy("RequireEmployeeOrAdmin", p => p.RequireRole("Admin", "Employee"));
});

// Rate limiting for login endpoint
builder.Services.AddRateLimiter(_ => _.AddPolicy("login", httpContext =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        })));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Seed roles/admin on startup (await to ensure availability)
await BookstoreManagementSystem.Infrastructure.DataBase.Scripts.AuthSeed.EnsureAuthSeedAsync();

app.UseHttpsRedirection();
// Middleware global para convertir ValidationException en 400 JSON para llamadas API/Postman
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

        // No es una petición API/JSON: rethrow para que Razor Pages o el handler predeterminado puedan procesarla
        throw;
    }
});
app.UseRateLimiter();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// Minimal API: /api/auth/login
app.MapPost("/api/auth/login", async (IJwtAuthService auth, AuthRequestDto req, HttpContext http, CancellationToken ct) =>
{
    var result = await auth.SignInAsync(req, ct);
    return result.Success && result.Value is not null
        ? Results.Ok(result.Value)
        : Results.Unauthorized();
}).RequireRateLimiting("login").AllowAnonymous();

// Dev-only endpoint to run Auth seed on demand
if (app.Environment.IsDevelopment())
{
    app.MapPost("/api/dev/seed-auth", async (CancellationToken ct) =>
    {
        await BookstoreManagementSystem.Infrastructure.DataBase.Scripts.AuthSeed.EnsureAuthSeedAsync(ct);
        return Results.Ok(new { message = "Auth seed executed" });
    }).AllowAnonymous();
}

app.Run();
