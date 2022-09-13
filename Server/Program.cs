using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

var builder = WebApplication.CreateBuilder(args);

var dataConnectionString = builder.Configuration.GetConnectionString("WS_Data");

var assembly = typeof(Program).Assembly.GetName().Name;

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

builder.Services.AddLogging();

var app = builder.Build();

app.UseIdentityServer();

app.Run();
