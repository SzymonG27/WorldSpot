using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("WS_Data")));

builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IEventCommentService, EventCommentService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEventTeamRelationService, EventTeamRelationService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITeamUsersInviteService, TeamUsersInviteService>();
builder.Services.AddScoped<ITeamUsersRelationService, TeamUsersRelationService>();

var app = builder.Build();

app.MapControllers();

app.Run();
