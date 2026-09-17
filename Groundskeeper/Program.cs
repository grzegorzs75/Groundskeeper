using Groundskeeper.Components;
using Groundskeeper.Data;
using Groundskeeper.Models;
using Groundskeeper.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<GroundskeeperDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Groundskeeper")));

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddScoped<WorkContext>();

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";

        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();


app.MapPost("/account/login", async (
    HttpContext httpContext,
    IDbContextFactory<GroundskeeperDbContext> dbFactory) =>
{
    var form = await httpContext.Request.ReadFormAsync();

    var email = form["email"].ToString();
    var password = form["password"].ToString();

    await using var db = await dbFactory.CreateDbContextAsync();

    var user = await db.AppUsers
        .FirstOrDefaultAsync(x =>
            x.Email == email &&
            x.Active);

    if (user == null)
    {
        return Results.Redirect("/login?error=1");
    }

    var hasher = new PasswordHasher<AppUser>();

    var result = hasher.VerifyHashedPassword(
        user,
        user.PasswordHash,
        password);

    if (result == PasswordVerificationResult.Failed)
    {
        return Results.Redirect("/login?error=1");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.Name),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Role, user.UserGroup.ToString())
    };

    if (user.CustomerId.HasValue)
    {
        claims.Add(
            new Claim(
                "CustomerId",
                user.CustomerId.Value.ToString()));
    }

    var identity = new ClaimsIdentity(
        claims,
        CookieAuthenticationDefaults.AuthenticationScheme);

    var principal = new ClaimsPrincipal(identity);

    await httpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal);

    return Results.Redirect("/");
});

app.MapPost("/account/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);

    return Results.Redirect("/login");
});

app.Run();
