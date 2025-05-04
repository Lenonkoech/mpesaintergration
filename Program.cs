using Microsoft.EntityFrameworkCore;
using mpesaintergration.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// creating mpesa client
builder.Services.AddHttpClient("mpesa", c =>
{
    c.BaseAddress = new Uri("https://sandbox.safaricom.co.ke");
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
 ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.Run();