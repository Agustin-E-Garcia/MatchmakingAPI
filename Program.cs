using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<GameMatchmaking.Data.PlayerAuthDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("AuthDB")));
builder.Services.AddScoped<GameMatchmaking.Services.AuthService>();

builder.Services.AddDbContext<GameMatchmaking.Data.StatsDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("StatsDB")));
builder.Services.AddScoped<GameMatchmaking.Services.StatsService>();

builder.Services.AddSingleton<GameMatchmaking.Services.TokenService>();
builder.Services.AddSingleton<GameMatchmaking.Services.MatchmakingService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
       options.MapInboundClaims = false;
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidIssuer = builder.Configuration["Jwt:Issuer"],
           ValidateAudience = true,
           ValidAudience = builder.Configuration["Jwt:Audience"],
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
       }; 
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();