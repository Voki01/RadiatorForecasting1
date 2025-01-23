using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RadiatorForecasting.Data;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ����������� HttpClient
builder.Services.AddHttpClient();

// ����������� ��������
builder.Services.AddControllersWithViews();

// ��������� ����������� � ���� ������
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // ��������� �������
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // ��������� ����������
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // ��������� ������������
    options.User.RequireUniqueEmail = true;
});

// ��������� �������������� ����� ����
builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

var app = builder.Build();

// �������� ��������� ��������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ��������� �������������� � �����������
app.UseAuthentication();
app.UseAuthorization();

// ������������� �� ��� ��������������� ���������������� �������������
app.Use(async (context, next) =>
{
    // ���������, ��� ������������ �� ����������� � ����������� �� �������� �����������
    if (!context.User.Identity.IsAuthenticated &&
        !context.Request.Path.Value.Contains("/Account/Login"))
    {
        context.Response.Redirect("/Account/Login");
        return;
    }

    await next();
});

// ���������� ����� � �������������
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndUsersAsync(services);
}

// ��������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ProductionFacts}/{action=Index}/{id?}");

app.Run();

// ����� ��� �������� �������� ������������� � �����
static async Task SeedRolesAndUsersAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // �������� �����
    string[] roleNames = { "��������", "������������" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // �������� ��������� ������������-���������
    var operatorUser = new IdentityUser
    {
        UserName = "operator@mail.com",
        Email = "operator@mail.com",
        EmailConfirmed = true
    };
    if (await userManager.FindByEmailAsync(operatorUser.Email) == null)
    {
        var result = await userManager.CreateAsync(operatorUser, "Operator123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(operatorUser, "��������");
        }
    }

    // �������� ��������� ������������-������������
    var managerUser = new IdentityUser
    {
        UserName = "manager@mail.com",
        Email = "manager@mail.com",
        EmailConfirmed = true
    };
    if (await userManager.FindByEmailAsync(managerUser.Email) == null)
    {
        var result = await userManager.CreateAsync(managerUser, "Manager123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(managerUser, "������������");
        }
    }
}
