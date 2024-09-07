using Services;
using System.ComponentModel;
using UnitOfWork.Interfaces;
using UnitOfWork.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registro de servicios en el contenedor de inyección de dependencias
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWorkSqlServer>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
