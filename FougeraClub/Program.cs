using KhaledTeamRecycling.Areas.Admin.Mappings;
using KhaledTeamRecycling.Extensions;
using KhaledTeamRecycling.Hub;
using KhaledTeamRecycling.Middelware;
using KhaledTeamRecycling.Seeders;

var builder = WebApplication.CreateBuilder(args);


// 🔐 Authentication Setup
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
    });


// 🧠 AutoMapper Setup (fix: use typeof(Profile) not .Assembly)
builder.Services.AddAutoMapper(typeof(AdminProfile)); // ✅ Works if profile is in WebUI

// 📦 Register Infrastructure Services (e.g., DbContext, Repositories, Identity)
builder.Services.AddInfrastructure(builder.Configuration);

// 🌐 Register Web Layer Services (Controllers, Localizers, ViewModels, etc.)
builder.Services.ConfigureWebLayer(builder);

// 🖼 MVC Controllers and Views
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();


// 🧠 Session support
builder.Services.AddSession(options =>
{
    // ⏱️ Session duration is one hour
    options.IdleTimeout = TimeSpan.FromHours(1);

    // 🔒 It is preferable to activate HttpOnly for security
    options.Cookie.HttpOnly = true;

    // ✅ The session is deleted when you close the browser (optional)
    options.Cookie.IsEssential = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(1); 
    options.SlidingExpiration = true;
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Admin/Home/AccessDeniedError403";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()       // ✅ allows all origins
            .AllowAnyHeader()       // ✅ allows all headers
            .AllowAnyMethod();      // ✅ allows all HTTP methods
    });
});


builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});


// ==========================
//       Build App
// ==========================
var app = builder.Build();
/* ========== SECURITY HEADERS (SAFE CSP) ========== */
app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy-Report-Only"] =
    "default-src 'self'; frame-src *;";


    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] =
        "geolocation=(), microphone=(), camera=()";

    await next();
});
/* ================================================ */
// ✅ 1. Use session BEFORE localization
app.UseSession();
app.UseCors("AllowAll"); // ✅ must come before SignalR and controllers
app.UseMiddleware<MaintenanceMiddleware>(); // ✅ intercepts requests

app.MapHub<NotificationHub>("/hubs/notifications");


// ✅ 2. Now localization can read from session safely
app.UseAppLocalization();

// ✅ 3. Routing, Auth, Static Files, etc.
app.ConfigureMiddleware();

// ==========================
//     Optional DB Setup
// ==========================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    await DbInitilaizer.Initialize(services, env);
}

app.Run();
