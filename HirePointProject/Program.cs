using HirePoint.Data;
using HirePoint.Models;
using HirePoint.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// MVC
// ==========================

builder.Services.AddControllersWithViews();

// ==========================
// HTTP CLIENT
// ==========================

builder.Services.AddHttpClient();

// ==========================
// DATABASE
// ==========================

builder.Services.AddDbContext<HirePointDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }
    )
);

// ==========================
// EMAIL
// ==========================

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, EmailService>();

// ==========================
// AI EMAIL
// ==========================

builder.Services.Configure<AISettings>(
    builder.Configuration.GetSection("AISettings"));

builder.Services.AddScoped<IAIEmailService, AIEmailService>();

// ==========================
// SESSION
// ==========================

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ==========================
// BUILD APPLICATION
// ==========================

var app = builder.Build();

// ==========================
// ERROR HANDLING
// ==========================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ==========================
// MIDDLEWARE
// ==========================

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// ==========================
// DEFAULT ROUTE
// ==========================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// ==========================
// RUN
// ==========================

app.Run();