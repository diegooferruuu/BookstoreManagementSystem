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
using BookstoreManagementSystem.Domain.Validations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.RootDirectory = "/Web/Pages";
        options.Conventions.AuthorizePage("/Products/Index", "RequireEmployeeOrAdmin");
        options.Conventions.AuthorizePage("/Products/Create", "RequireAdmin");
        options.Conventions.AuthorizePage("/Products/Edit", "RequireAdmin");
        options.Conventions.AuthorizePage("/Products/Delete", "RequireAdmin");
        options.Conventions.AuthorizePage("/Auth/Profile");
        options.Conventions.AuthorizeFolder("/Clients", "RequireEmployeeOrAdmin");
        options.Conventions.AuthorizePage("/Distributors/Index", "RequireEmployeeOrAdmin");
        options.Conventions.AuthorizePage("/Distributors/Create", "RequireAdmin");
        options.Conventions.AuthorizePage("/Distributors/Edit", "RequireAdmin");
        options.Conventions.AuthorizePage("/Distributors/Delete", "RequireAdmin");
        options.Conventions.AuthorizeFolder("/Users", "RequireAdmin");
        options.Conventions.AllowAnonymousToPage("/Auth/Login");
        options.Conventions.AllowAnonymousToPage("/Auth/Logout");
    })
    .AddMvcOptions(options =>
    {
        options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(fieldName => $"El campo {fieldName} debe ser un número.");
        options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "Este campo debe ser un número.");
        options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(name => $"Se requiere un valor para '{name}'.");
        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(value => $"El valor '{value}' no es válido.");
        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, fieldName) =>
        {
            if (!string.IsNullOrEmpty(fieldName) && (fieldName.EndsWith(".Price") || fieldName.Equals("Price", StringComparison.OrdinalIgnoreCase)))
            {
                return $"El precio no debe superar {ProductValidation.MaxPrice}.";
            }
            return $"El valor '{value}' no es válido para {fieldName}.";
        });
        options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "Este valor es obligatorio.");
        options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(value => "El valor especificado no es válido.");
        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(fieldName => $"El campo {fieldName} no puede ser nulo.");
        options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "El cuerpo de la solicitud es obligatorio.");
    });

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
builder.Services.AddSingleton(jwtOptions);

var sendGridSection = builder.Configuration.GetSection("SendGrid");
var sendGridOptions = sendGridSection.Get<BookstoreManagementSystem.Infrastructure.Email.SendGridOptions>() 
    ?? new BookstoreManagementSystem.Infrastructure.Email.SendGridOptions();
builder.Services.AddSingleton(sendGridOptions);

builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IDistributorRepository, DistributorRepository>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IJwtAuthService, JwtAuthService>();
builder.Services.AddSingleton<IEmailService, BookstoreManagementSystem.Infrastructure.Email.SendGridEmailService>();
builder.Services.AddSingleton<IPasswordGenerator, BookstoreManagementSystem.Infrastructure.Security.SecurePasswordGenerator>();
builder.Services.AddSingleton<IUsernameGenerator, BookstoreManagementSystem.Infrastructure.Security.UsernameGenerator>();

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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

await BookstoreManagementSystem.Infrastructure.DataBase.Scripts.AuthSeed.EnsureAuthSeedAsync();

app.UseHttpsRedirection();
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
app.UseRateLimiter();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapPost("/api/auth/login", async (IJwtAuthService auth, AuthRequestDto req, HttpContext http, CancellationToken ct) =>
{
    var result = await auth.SignInAsync(req, ct);
    return result.Success && result.Value is not null
        ? Results.Ok(result.Value)
        : Results.Unauthorized();
}).RequireRateLimiting("login").AllowAnonymous();

if (app.Environment.IsDevelopment())
{
    app.MapPost("/api/dev/seed-auth", async (CancellationToken ct) =>
    {
        await BookstoreManagementSystem.Infrastructure.DataBase.Scripts.AuthSeed.EnsureAuthSeedAsync(ct);
        return Results.Ok(new { message = "Auth seed executed" });
    }).AllowAnonymous();
}

app.Run();
