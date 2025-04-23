using System.Numerics;
using DotNetEnv;

using Microsoft.EntityFrameworkCore;
using OrdrMate.Data;
using OrdrMate.Repositories;
using OrdrMate.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = Environment.GetEnvironmentVariable("DB_URL");

Console.WriteLine(connectionString);

builder.Services.AddDbContext<OrdrMateDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IManagerRepo, ManagerRepo>();
builder.Services.AddScoped<ManagerService, ManagerService>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

