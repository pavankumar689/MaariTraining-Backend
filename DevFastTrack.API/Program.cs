using System.Text;
using DevFastTrack.API.Data;
using DevFastTrack.API.Middleware;
using DevFastTrack.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// ✅ PostgreSQL via Npgsql (works on Neon, Railway, Supabase)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpClient();

// ✅ CORS — allows localhost (dev) + Vercel (production)
// After deploying to Vercel, add your Vercel URL to this list
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>()
    ?? new[] { "http://localhost:4200", "https://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Auto-run migrations and seed on startup (safe for Railway)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        Console.WriteLine("🔄 Running database migrations...");
        await context.Database.MigrateAsync();
        Console.WriteLine("✅ Migrations applied successfully.");
        await DbSeeder.SeedAsync(context);
        Console.WriteLine("✅ Database seeded.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database startup error: {ex.Message}");
        Console.WriteLine("Check your ConnectionStrings__DefaultConnection environment variable.");
        throw; // re-throw so Railway marks deployment as failed
    }
}

// Register Exception Middleware
app.UseMiddleware<ExceptionMiddleware>();

// Show Swagger in both dev and production (helpful for testing Railway API)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

// ✅ Skip HTTPS redirect on Railway (Railway handles SSL termination itself)
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ✅ Dynamic port — Railway sets PORT env variable automatically
var port = Environment.GetEnvironmentVariable("PORT") ?? "5001";
app.Run($"http://0.0.0.0:{port}");
