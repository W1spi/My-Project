using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Data.NLimit.Common.DataContext.SqlServer;
using System.Net.Http.Headers; // MediaTypeWithQualityHeaderValue
using System.Security.Principal;
using Microsoft.Extensions.FileProviders;
using NLimit.Common.DataContext.SqlServer;
using NLimit.Web.Data.DataContext;
using NLimit.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using FluentValidation;
using Microsoft.AspNet.Identity;
using NLimit.Web.ClassServices.EntityValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // ���������� ������
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// поддержка сессий
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Время жизни сессии
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

builder.Services.AddNLimitContext();


builder.Services.AddHttpClient(
    name: "NLimit.WebApi",
    configureClient: options =>
    {
        options.BaseAddress = new Uri("https://localhost:7031/");
        options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0));
    });

// кастомная валидация
builder.Services.AddScoped<IValidator<PersonalAccountViewModel>, PersonalAccountValidator>();

var app = builder.Build();
IHostEnvironment? env = app.Services.GetService<IHostEnvironment>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHsts(); // добавляет заголовок Strict-Transport-Security
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// использование сессий
app.UseSession();

if (env is not null)
{
    // добавляем поддержку каталога node_modules
    app.UseFileServer(new FileServerOptions()
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules")),
        RequestPath = "/node_modules",
        EnableDirectoryBrowsing = false
    });
}

// MapControllerRoute() - переводит по указанному маршруту, если пользователь не укажет свои данные маршрута
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"); //"{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();



app.Run();
//Git-Test