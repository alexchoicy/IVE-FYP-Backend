using api.Models;
using api.Services;
using api.utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApiVersioning();
builder.Services.AddSingleton<MqttClientservices>();
builder.Services.AddHostedService<MqttHostedService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NormalDataBaseContext>(options => {
    string? NormalDataBaseConnectionString = builder.Configuration.GetConnectionString("NormalDataBaseConnection");
    options.UseMySql(NormalDataBaseConnectionString, ServerVersion.AutoDetect(NormalDataBaseConnectionString));
});

builder.Services.AddScoped<ITest, Test>();

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
