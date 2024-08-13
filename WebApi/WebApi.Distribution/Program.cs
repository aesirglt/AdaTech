using Microsoft.EntityFrameworkCore;
using WebApi.Application.Features.Cards;
using WebApi.Application.Interfaces;
using WebApi.Domain.Features.Cards;
using WebApi.Domain.Interfaces;
using WebApi.Infra.Data.Bases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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
app.MapControllers();
app.Run();
