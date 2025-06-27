using Microsoft.EntityFrameworkCore;
using BlogApp.Data;
using BlogApp.Models;
var builder = WebApplication.CreateBuilder(args);

// Kestrel varsayılan portunu ayarla
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5051);
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BlogApp.Data.BlogContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options => {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Varsayılan admin kullanıcısı ekle
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BlogContext>();
    if (!db.Users.Any(u => u.Username == "admin"))
    {
        db.Users.Add(new User { Username = "admin", Password = "admin", Role = "admin" });
        db.SaveChanges();
    }
}

app.Run();
