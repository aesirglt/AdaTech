using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using WebApi.Application.Features.Cards;
using WebApi.Application.Interfaces;
using WebApi.Distribution.Filters;
using WebApi.Domain.Features.Cards;
using WebApi.Domain.Interfaces;
using WebApi.Infra.Data.Bases;

var builder = WebApplication.CreateBuilder(args);
// Carregar variáveis de ambiente
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Configuração de JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});
builder.Services.AddAuthorization();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<LogActionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<KanbanContext>(builder
        => builder.UseInMemoryDatabase("KanbanContext"));

builder.Services.AddScoped<IReadRepository<Card>, ReadRepository<Card>>();
builder.Services.AddScoped<IWriteRepository<Card>, WriteRepository<Card>>();
builder.Services.AddScoped<ICardService, CardService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
app.Run();