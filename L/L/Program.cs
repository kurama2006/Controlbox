using L.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache(); // Requerido para almacenar sesión en memoria

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad antes de que expire
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ControlboxdbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("conexion"),
        Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb")
    ));

var app = builder.Build();

// Middleware (👇 agrega esto en el orden correcto)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔽 Mueve UseSession antes de UseAuthorization
app.UseSession(); // 👈 ¡Agrega esto antes de Authorization!

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
