using System.Text;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Repositories;
using ProjectManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var corsPolicy = "_myAllowSpecificOrigins";
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddDbContext<ProjectDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(5001, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 5001;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true
    };
});

// Add MemoryCache
builder.Services.AddMemoryCache();

// Add Rate-Limiting Services
builder.Services.AddInMemoryRateLimiting();  // Use In-Memory Rate-Limiting
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Configure Rate-Limiting Options
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 100 // 100 requests per minute
        },
        new RateLimitRule
        {
            Endpoint = "/api/auth/login",
            Period = "1m",
            Limit = 5 // Only 5 requests per minute
        },
        new RateLimitRule
        {
            Endpoint = "/api/auth/register",
            Period = "10m",
            Limit = 3 // Only 3 requests per minute
        }
    };
});
builder.Services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();

// Register repositories
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Register services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHostedService<TokenCleanupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

// Add CSP-Header
app.Use(async (context, next) =>
{
    string nonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
    context.Response.Headers.Append("Content-Security-Policy",
        $"default-src 'self'; script-src 'self' 'nonce-{nonce}' https://apis.example.com; connect-src 'self' http://localhost:5001;");

    context.Items["CspNonce"] = nonce;
    await next.Invoke();
});

// Add X-Content-Type-Options Header
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    await next.Invoke();
});

// Add X-Frame-Options Header to prevent Clickjacking
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "DENY");  // Keine Frames erlauben
    await next.Invoke();
});

// Add Referrer-Policy Header
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    await next.Invoke();
});

// Rate-Limiting Middleware verwenden
app.UseIpRateLimiting();

app.UseCors(corsPolicy);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
