using Data.NLimit.Common.DataContext.SqlServer;
using NLimit.Common.DataContext.SqlServer;
using NLimit.WebApi.Repositoires.Users;
using NLimit.WebApi.Repositoires.Works;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();
builder.Services.AddNLimitContext();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "NLimit Service API",
        Version = "v1"
    });
});

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
}

app.UseHttpsRedirection();

app.UseAuthorization();

// MapControllers() вызываетс€ дл€ сопоставлени€ перенаправленных контроллеров атрибутов 
// ƒолжен вызыватьс€ дл€ Api сервисов
app.MapControllers();

//app.UseMiddleware<SecurityHeaders>();

app.UseCors(configurePolicy: options =>
{
    options.WithMethods("GET", "POST", "PUT", "DELETE");
    options.WithOrigins("https://localhost:7027/");
});

app.Run();
