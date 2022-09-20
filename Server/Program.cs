using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Data;
using Server.Models;

var seedData = args.Contains("/seed");
if (seedData == true)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}

var builder = WebApplication.CreateBuilder(args);

var dataConnectionString = builder.Configuration.GetConnectionString("WS_Data");
var assembly = typeof(Program).Assembly.GetName().Name;

if (seedData == true)
{
    SeedData.EnsureSeedData(dataConnectionString);
}

builder.Services.AddMvc();

builder.Services.AddDbContext<AppIdentityDbContext>(opt =>
    opt.UseSqlServer(dataConnectionString, conf => conf.MigrationsAssembly(assembly)));
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>();

builder.Services.AddIdentityServer()
    .AddConfigurationStore(opt =>
    {
        opt.ConfigureDbContext = conf =>
            conf.UseSqlServer(dataConnectionString, option => option.MigrationsAssembly(assembly)); //korzystanie z migracji projektu API
    })
    .AddOperationalStore(opt =>
    {
        opt.ConfigureDbContext = conf =>
            conf.UseSqlServer(dataConnectionString, option => option.MigrationsAssembly(assembly)); //korzystanie z migracji projektu API
    })
    .AddDeveloperSigningCredential()
    .AddAspNetIdentity<AppUser>();

builder.Services.AddControllersWithViews();

//builder.Services.AddLogging();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
