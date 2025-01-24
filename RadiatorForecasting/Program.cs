using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RadiatorForecasting.Data;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Регистрация HttpClient
builder.Services.AddHttpClient();

// Регистрация сервисов
builder.Services.AddControllersWithViews();

// Настройка подключения к базе данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Настройка Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Настройки паролей
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // Настройки блокировки
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Настройки пользователя
    options.User.RequireUniqueEmail = true;
});

// Настройка аутентификации через куки
builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

var app = builder.Build();

// Конвейер обработки запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Включение аутентификации и авторизации
app.UseAuthentication();
app.UseAuthorization();

// Промежуточное ПО для перенаправления неавторизованных пользователей
app.Use(async (context, next) =>
{
    // Проверяем, что пользователь не авторизован и запрашивает не страницу авторизации
    if (!context.User.Identity.IsAuthenticated &&
        !context.Request.Path.Value.Contains("/Account/Login"))
    {
        context.Response.Redirect("/Account/Login");
        return;
    }

    await next();
});


// Маршруты
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ProductionFacts}/{action=Index}/{id?}");

app.Run();

