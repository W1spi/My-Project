using Data.NLimit.Common.DataContext.SqlServer;
using NLimit.Common.DataContext.SqlServer;
using NLimit.WebApi.Repositoires.Users;
using NLimit.WebApi.Repositoires.Works;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NLimit.WebApi.Services.UserAuthentication;
using Microsoft.Extensions.Configuration;
using NLimit.WebApi.Services.Middleware;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();
builder.Services.AddNLimitContext();
builder.Services.AddControllers(options =>
{
    //options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "NLimit Service API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",

        Scheme = "Bearer",
        BearerFormat = "JWT",

        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
            new List<string>()
        }
    });
});

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // генерирует токен
        ValidateAudience = true, // провер€ет, авторизован ли пользователь дл€ получени€ токена
        ValidateIssuerSigningKey = true, // провер€ет, не истек ли токен и валиден ли ключ подписани€ эмитента

        ValidateLifetime = false,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) //Configuration["JwtToken:SecretKey"]
    };
});

builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWorkRepository, WorkRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NLimit Service Api Version");
        c.SupportedSubmitMethods(new[]
        {
            SubmitMethod.Get, SubmitMethod.Post,
            SubmitMethod.Put, SubmitMethod.Delete
        });
    });

    IdentityModelEventSource.ShowPII = true;
}

app.UseHttpsRedirection();

app.UseAuthorization();

// MapControllers() вызываетс€ дл€ сопоставлени€ перенаправленных контроллеров атрибутов 
// ƒолжен вызыватьс€ дл€ Api сервисов
app.MapControllers();

app.UseMiddleware<JWTMiddleware>();
//app.UseMiddleware<SecurityHeaders>();

app.UseCors(configurePolicy: options =>
{
    options.WithMethods("GET", "POST", "PUT", "DELETE");
    options.WithOrigins("https://localhost:7027/");
});

app.Run();
