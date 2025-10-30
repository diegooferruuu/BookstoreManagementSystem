using System.Text;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
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
using ServiceUsers.Application.Facade;
using ServiceUsers.Infrastructure.Auth;
using ServiceUsers.Infrastructure.Repositories;
using ServiceUsers.Infrastructure.Security;
using ServiceProducts.Domain.Interfaces;
using ServiceProducts.Application.Services;
using ServiceProducts.Infrastructure.Repositories;
using ServiceDistributors.Domain.Interfaces;
using ServiceDistributors.Application.Services;
using ServiceDistributors.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    // Páginas accesibles sin autenticación
    options.Conventions.AllowAnonymousToPage("/Auth/Login");
    options.Conventions.AllowAnonymousToPage("/Auth/Logout");
    options.Conventions.AllowAnonymousToPage("/Users/ChangePassword");

    // Todo lo demás requiere autenticación
    options.Conventions.AuthorizeFolder("/");
})
.AddMvcOptions(options =>
{
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "Debe seleccionar una categoría.");
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => "El valor ingresado no es válido.");
    options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(x => $"Falta el valor para {x}.");
});

var cultureInfo = new CultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(cultureInfo);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var database = DataBaseConnection.GetInstance(connectionString);
builder.Services.AddSingleton<IDataBase>(database);

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<ServiceUsers.Infrastructure.Auth.JwtOptions>()
    ?? new ServiceUsers.Infrastructure.Auth.JwtOptions();
builder.Services.AddSingleton(jwtOptions);

var sendGridSection = builder.Configuration.GetSection("SendGrid");
var sendGridOptions = sendGridSection.Get<SendGridOptions>()
    ?? new SendGridOptions();
builder.Services.AddSingleton(sendGridOptions);
builder.Services.AddSingleton<IEmailService, SendGridEmailService>();

builder.Services.AddSingleton<IClientRepository, ClientRepository>();
builder.Services.AddSingleton<IClientService, ClientService>();

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IUserService, ServiceUsers.Application.Services.UserService>();
builder.Services.AddSingleton<IPasswordGenerator, SecurePasswordGenerator>();
builder.Services.AddSingleton<IUsernameGenerator, UsernameGenerator>();
builder.Services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IJwtAuthService, JwtAuthService>();
builder.Services.AddScoped<IUserFacade, UserFacade>();

builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<ServiceProducts.Domain.Interfaces.ICategoryRepository, CategoryRepository>();
builder.Services.AddSingleton<IProductService, ProductService>();

builder.Services.AddSingleton<IDistributorRepository, DistributorRepository>();
builder.Services.AddSingleton<IDistributorService, DistributorService>();

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
    options.Cookie.SameSite = SameSiteMode.Strict;
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

// API de autenticación (usa fachada)
app.MapPost("/api/auth/login", async (IUserFacade facade, ServiceUsers.Application.DTOs.AuthRequestDto req, CancellationToken ct) =>
{
    var token = await facade.LoginAsync(req, ct);
    return token is not null
        ? Results.Ok(token)
        : Results.Unauthorized();
}).AllowAnonymous();

// Usuario admin inicial
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IDataBase>();
    using var conn = db.GetConnection();

    var checkAdminSql = "SELECT COUNT(*) FROM users WHERE LOWER(username)='admin'";
    using var checkCmd = new Npgsql.NpgsqlCommand(checkAdminSql, conn);
    var count = (long)checkCmd.ExecuteScalar()!;

    if (count == 0)
    {
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<ServiceUsers.Domain.Models.User>();
        var admin = new ServiceUsers.Domain.Models.User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@local",
            FirstName = "Administrador",
            LastName = "",
            PasswordHash = passwordHasher.HashPassword(null!, "admin123456"),
            IsActive = true,
            MustChangePassword = true
        };

        var insertUserSql = @"INSERT INTO users (id, username, email, first_name, last_name, middle_name, password_hash, is_active, must_change_password)
                              VALUES (@id, @username, @email, @first, @last, NULL, @hash, TRUE, TRUE)";
        using var insertUserCmd = new Npgsql.NpgsqlCommand(insertUserSql, conn);
        insertUserCmd.Parameters.AddWithValue("@id", admin.Id);
        insertUserCmd.Parameters.AddWithValue("@username", admin.Username);
        insertUserCmd.Parameters.AddWithValue("@email", admin.Email);
        insertUserCmd.Parameters.AddWithValue("@first", admin.FirstName);
        insertUserCmd.Parameters.AddWithValue("@last", admin.LastName);
        insertUserCmd.Parameters.AddWithValue("@hash", admin.PasswordHash);
        insertUserCmd.ExecuteNonQuery();

        var ensureRoleSql = @"INSERT INTO roles (id, name)
                              SELECT gen_random_uuid(), 'Admin'
                              WHERE NOT EXISTS (SELECT 1 FROM roles WHERE name='Admin')";
        using var ensureRoleCmd = new Npgsql.NpgsqlCommand(ensureRoleSql, conn);
        ensureRoleCmd.ExecuteNonQuery();

        var linkRoleSql = @"INSERT INTO user_roles (user_id, role_id)
                            SELECT @userId, r.id FROM roles r WHERE r.name='Admin'";
        using var linkCmd = new Npgsql.NpgsqlCommand(linkRoleSql, conn);
        linkCmd.Parameters.AddWithValue("@userId", admin.Id);
        linkCmd.ExecuteNonQuery();
    }
}

app.Run();
