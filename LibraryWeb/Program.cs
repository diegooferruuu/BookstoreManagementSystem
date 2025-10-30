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
using ServiceProducts.Domain.Interfaces.Reports;
using ServiceProducts.Application.Services;
using ServiceProducts.Infrastructure.Reports;
using ServiceProducts.Infrastructure.Repositories;
using ServiceDistributors.Domain.Interfaces;
using ServiceDistributors.Application.Services;
using ServiceDistributors.Infrastructure.Repositories;
using ServiceSales.Domain.Interfaces;
using ServiceSales.Application.Services;
using ServiceSales.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// === Razor Pages ===
builder.Services.AddRazorPages(options =>
{
    // Páginas accesibles sin autenticación
    options.Conventions.AllowAnonymousToPage("/Auth/Login");
    options.Conventions.AllowAnonymousToPage("/Auth/Logout");
    options.Conventions.AllowAnonymousToPage("/Users/ChangePassword");

    // Todo lo demás requiere autenticación
    options.Conventions.AuthorizeFolder("/");

    // === Productos ===
    options.Conventions.AuthorizePage("/Products/Index", "AdminOrEmployee");
    options.Conventions.AuthorizePage("/Products/Report", "AdminOrEmployee");
    options.Conventions.AuthorizePage("/Products/Create", "AdminOnly");
    options.Conventions.AuthorizePage("/Products/Edit", "AdminOnly");
    options.Conventions.AuthorizePage("/Products/Delete", "AdminOnly");

    // === Distribuidores ===
    options.Conventions.AuthorizePage("/Distributors/Index", "AdminOrEmployee");
    options.Conventions.AuthorizePage("/Distributors/Create", "AdminOnly");
    options.Conventions.AuthorizePage("/Distributors/Edit", "AdminOnly");
    options.Conventions.AuthorizePage("/Distributors/Delete", "AdminOnly");

    // === Clientes ===
    options.Conventions.AuthorizeFolder("/Clients", "AdminOrEmployee");

    // === Ventas ===
    options.Conventions.AuthorizeFolder("/Sales", "AdminOrEmployee");

    // === Usuarios ===
    options.Conventions.AuthorizeFolder("/Users", "AdminOnly");
    options.Conventions.AuthorizePage("/Users/ChangePassword", "AdminOrEmployee");

    // === Páginas generales ===
    options.Conventions.AuthorizePage("/Index", "AdminOrEmployee");
    options.Conventions.AuthorizePage("/Error", "AdminOrEmployee");
})
.AddMvcOptions(options =>
{
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "Debe seleccionar una categoría.");
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => "El valor ingresado no es válido.");
    options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(x => $"Falta el valor para {x}.");
});

// === Configuración regional ===
var cultureInfo = new CultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(cultureInfo);
});

// === Base de datos ===
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var database = DataBaseConnection.GetInstance(connectionString);
builder.Services.AddSingleton<IDataBase>(database);

// === JWT ===
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtOptions = jwtSection.Get<ServiceUsers.Infrastructure.Auth.JwtOptions>()
    ?? new ServiceUsers.Infrastructure.Auth.JwtOptions();
builder.Services.AddSingleton(jwtOptions);

// === SendGrid ===
var sendGridSection = builder.Configuration.GetSection("SendGrid");
var sendGridOptions = sendGridSection.Get<SendGridOptions>()
    ?? new SendGridOptions();
builder.Services.AddSingleton(sendGridOptions);
builder.Services.AddSingleton<IEmailService, SendGridEmailService>();

// === Dependencias ===
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

builder.Services.AddScoped<IReportDirector, ProductReportDirector>();
builder.Services.AddScoped<IProductReportService, ProductReportService>();

builder.Services.AddSingleton<IDistributorRepository, DistributorRepository>();
builder.Services.AddSingleton<IDistributorService, DistributorService>();

builder.Services.AddSingleton<ISaleRepository, SaleRepository>();
builder.Services.AddSingleton<ISalesReportService, SalesReportService>();

// === Autenticación ===
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

// === Autorización ===
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("EmployeeOnly", p => p.RequireRole("Employee"));
    options.AddPolicy("AdminOrEmployee", p => p.RequireRole("Admin", "Employee"));

    // Compatibilidad con código existente
    options.AddPolicy("RequireEmployeeOrAdmin", p => p.RequireRole("Admin", "Employee"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// === API de autenticación ===
app.MapPost("/api/auth/login", async (IUserFacade facade, ServiceUsers.Application.DTOs.AuthRequestDto req, CancellationToken ct) =>
{
    var token = await facade.LoginAsync(req, ct);
    return token is not null
        ? Results.Ok(token)
        : Results.Unauthorized();
}).AllowAnonymous();

app.Run();
