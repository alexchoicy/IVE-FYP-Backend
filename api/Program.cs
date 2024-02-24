using api.Controllers;
using api.Fliters;
using api.Middleware;
using api.Models;
using api.Services;
using api.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .MinimumLevel.Override("System", LogEventLevel.Error)
    .WriteTo.Console()
    .WriteTo.File($"Logs/{DateTime.Now:yyyyMMdd}/log-{DateTime.Now:HHmmss}.txt")
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// maybe needed lol
builder.Host.UseSerilog();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddSerilog();


builder.Services.AddControllers();
builder.Services.AddApiVersioning();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

builder.Services.AddScoped<ApiActionFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiActionFilter));
});


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


// Add MQTT services
builder.Services.AddSingleton<MqttClientservices>();
builder.Services.AddHostedService<MqttHostedService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<NormalDataBaseContext>(options =>
{
    string? NormalDataBaseConnectionString = builder.Configuration.GetConnectionString("NormalDataBaseConnection");
    options.UseMySql(NormalDataBaseConnectionString, ServerVersion.AutoDetect(NormalDataBaseConnectionString));

});

builder.Services.AddDbContext<StaffDataBaseContext>(options =>
{
    string? StaffDataBaseConnectionString = builder.Configuration.GetConnectionString("StaffDataBaseConnection");
    options.UseMySql(StaffDataBaseConnectionString, ServerVersion.AutoDetect(StaffDataBaseConnectionString));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<JWTServices>();
builder.Services.AddScoped<HashServices>();

builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IParkingLotServices, ParkingLotServices>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    string jwtkey = builder.Configuration["JWT:Key"] ?? "";
    if (jwtkey == "")
    {
        throw new Exception("JWT Key is not set");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
        System.Text.Encoding.UTF8.GetBytes(jwtkey)),
        ValidateLifetime = true,
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            string? ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            string endpoint = context.HttpContext.Request.Path;
            string? method = context.HttpContext.Request.Method;
            Log.Error($"Failed: Request from {ip} to {endpoint}, {method}, {context.ErrorDescription}");
            Console.WriteLine($"Failed: Request from {ip} to {endpoint}, {method}, {context.ErrorDescription}");
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new ApiResponse<string>
            {
                ErrorMessage = "You are not Authorized",
                Success = false
            });
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }
app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseHttpsRedirection();

app.MapControllers();

app.UseCors("AllowAll");

app.Run();
