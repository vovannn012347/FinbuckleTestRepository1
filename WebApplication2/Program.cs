using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using WebApplication2;
using WebApplication2.Code;
using WebApplication2.Code.JwtTokenBicyce;
using WebApplication2.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//builder.Services.AddDbContext<ApplicationDbContext>
//    (options => options.UseSqlServer(connectionString));

//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var keyManager = new KeyManager();
builder.Services.AddSingleton(keyManager);
builder.Services.AddSingleton<IAuthorizationHandler, TenantHeaderRoleMatchRequirementHandler>();

builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = false;

        options.Password.RequiredLength = 4;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;

    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if(context.Request.Query.TryGetValue("t", out var token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };

        options.Configuration = new OpenIdConnectConfiguration
        {
            SigningKeys ={
                new RsaSecurityKey(keyManager.RsaKey)
            }
        };

        options.MapInboundClaims = false;

    });

//added by identity
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(...);

builder.Services.AddAuthorization(A =>
{
    //A.DefaultPolicy = new AuthorizationPolicyBuilder()
    //.RequireAuthenticatedUser()
    //.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme, JwtBearerDefaults.AuthenticationScheme)
    //.RequireClaim("role", "tester")
    //.Build();

    A.AddPolicy("any_policy", po => po
    .RequireAuthenticatedUser()
    .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme, JwtBearerDefaults.AuthenticationScheme)
    .RequireClaim("role", "tester"));

    A.AddPolicy("cookie_policy", po => po
    .RequireAuthenticatedUser()
    .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
    .RequireClaim("role", "tester"));

    A.AddPolicy("token_policy", po => {
        po
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireClaim("role", "tester");
        po.Requirements.Add(new TenantHeaderRoleMatchRequirement("tenant"));
    });
});
// Add HttpContextAccessor if not already added
builder.Services.AddHttpContextAccessor();

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

app.UseMiddleware<CustomTenantControlMiddleware>();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default2",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{__tenant__=}/{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
