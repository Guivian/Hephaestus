using System.Text;
using Hephaestus.Api.Data;
using Hephaestus.Api.Models;
using Hephaestus.Api.Middleware;
using Hephaestus.Api.Options;
using Hephaestus.Api.Repositories;
using Hephaestus.Api.Security;
using Hephaestus.Api.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Apenas o cliente Web local pode efetuar pedidos cross-origin no browser.
// Aplicações Android nativas não estão sujeitas à política CORS do browser.
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalClients", policy =>
        policy
            .WithOrigins("https://localhost:44383")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddDbContext<HephaestusDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("HephaestusDatabase")));
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordHasher<TwoFactorChallenge>, PasswordHasher<TwoFactorChallenge>>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<TwoFactorService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<GoogleAuthService>();
builder.Services.AddScoped<IDomainRepository, DomainRepository>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<HistoryService>();
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.SectionName));
builder.Services.Configure<GoogleOptions>(builder.Configuration.GetSection(GoogleOptions.SectionName));
builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection(FileStorageOptions.SectionName));

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("A configuração Jwt:Key não existe.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Jwt:Key deve ter pelo menos 32 bytes.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    })
    .AddCookie(GoogleAuthService.ExternalCookieScheme, options =>
    {
        options.Cookie.Name = "Hephaestus.Google.External";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddGoogle(GoogleAuthService.GoogleScheme, options =>
    {
        options.SignInScheme = GoogleAuthService.ExternalCookieScheme;
        options.ClientId = builder.Configuration["Authentication:Google:WebClientId"]
            ?? throw new InvalidOperationException("O Web Client ID Google não está configurado.");
        options.ClientSecret = builder.Configuration["Authentication:Google:WebClientSecret"]
            ?? throw new InvalidOperationException("O Web Client Secret Google não está configurado.");
        options.CallbackPath = "/signin-google";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminOnly, policy =>
        policy.RequireRole(RoleNames.Admin));

    options.AddPolicy(Policies.Management, policy =>
        policy.RequireRole(RoleNames.Admin, RoleNames.Manager));

    options.AddPolicy(Policies.TechnicalStaff, policy =>
        policy.RequireRole(RoleNames.Admin, RoleNames.Manager, RoleNames.Technician));

    options.AddPolicy(Policies.StandardOnly, policy =>
        policy.RequireRole(RoleNames.Standard));
});
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Em Development, o emulador Android usa http://10.0.2.2:5022. Em qualquer
// outro ambiente, todos os pedidos continuam a ser redirecionados para HTTPS.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("LocalClients");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<DomainExceptionMiddleware>();

app.MapControllers();

app.Run();
