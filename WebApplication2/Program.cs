using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication2;
using WebApplication2.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//builder.Services.AddDbContext<ApplicationDbContext>
//    (options => options.UseSqlServer(connectionString));

//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services
    .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

/*
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DatabaseTenant2;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
*/

builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithBasePathStrategy(options => options.RebaseAspNetCorePathBase = true)
    //.WithRouteStrategy()
    .WithConfigurationStore()
    .WithPerTenantAuthentication();

var app = builder.Build();

var store = app.Services.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();
foreach (var tenant in await store.GetAllAsync())
{
    using (var db = new ApplicationDbContext(tenant))
    {
        db.Database.Migrate();
    }
}

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMultiTenant();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CustomTenantControlMiddleware>();

app.MapControllerRoute(
    name: "default2",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{__tenant__=}/{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
