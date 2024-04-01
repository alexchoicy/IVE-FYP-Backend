using api.Controllers;
using api.Fliters;
using api.Middleware;
using api.Models;
using api.Services;
using api.Services.Chats;
using api.Services.Notifications;
using api.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
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
        "AdminOrigin",
        corsbuilder => corsbuilder
            .WithOrigins(builder.Configuration.GetSection("Cors:AdminOrigin").Get<string[]>())
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
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

builder.Services.AddDbContextFactory<NormalDataBaseContext>(options =>
{
    string? NormalDataBaseConnectionString = builder.Configuration.GetConnectionString("NormalDataBaseConnection");
    options.UseMySql(NormalDataBaseConnectionString, ServerVersion.AutoDetect(NormalDataBaseConnectionString));

});

builder.Services.AddDbContextFactory<StaffDataBaseContext>(options =>
{
    string? StaffDataBaseConnectionString = builder.Configuration.GetConnectionString("StaffDataBaseConnection");
    options.UseMySql(StaffDataBaseConnectionString, ServerVersion.AutoDetect(StaffDataBaseConnectionString));
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


builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<JWTServices>();
builder.Services.AddScoped<HashServices>();

// builder.Services.AddSingleton<ILprDataService, LprDataService>();
// builder.Services.AddScoped<ILprDataService, LprDataService>();
builder.Services.AddSingleton<IHourlyAvaiableSpaceServices, HourlyAvaiableSpaceServices>();

builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IParkingLotServices, ParkingLotServices>();
builder.Services.AddScoped<IVehicleServices, VehicleServices>();
builder.Services.AddScoped<IReservationServices, ReservationServices>();
builder.Services.AddScoped<IPaymentServices, PaymentServices>();
builder.Services.AddScoped<IParkingRecordServices, ParkingRecordServices>();
builder.Services.AddScoped<IAdminServices, AdminServices>();

builder.Services.AddSingleton<IChatServices, ChatServices>();
builder.Services.AddSingleton<IChatNotifications, ChatNotifications>();

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
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["token"];
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.HttpContext.Request.Headers["Authorization"] = "Bearer " + context.Token;
            }
            return Task.CompletedTask;
        },

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
        },
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("access-token", policy => policy.RequireClaim("type", "access-token"));
    options.AddPolicy("password-reset", policy => policy.RequireClaim("type", "password-reset"));
});
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<ExceptionHandlingMiddleware>();
}

app.UseWebSockets();

app.UseHttpsRedirection();

app.MapControllers();

app.UseCors("AdminUIOrigin");

app.Run();
