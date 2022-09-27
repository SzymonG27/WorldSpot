using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<EventModel> Events { get; set; }
        public DbSet<EventCommentModel> EventsComments { get; set; }
        public DbSet<EventTeamsRelationModel> EventTeamsRelations { get; set; }
        public DbSet<RouteModel> Routes { get; set; }
        public DbSet<TeamModel> Teams { get; set; }
        public DbSet<TeamUsersInviteModel> TeamUsersInvites { get; set; }
        public DbSet<TeamUsersRelationModel> TeamUsersRelations { get; set; }
        public DbSet<ChatModel> Chats { get; set; }
        public DbSet<MessageModel> Messages { get; set; }

    }
}
