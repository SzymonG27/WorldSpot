using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventComment> EventsComments { get; set; }
        public DbSet<EventTeamsRelation> EventTeamsRelations { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamUsersInvite> TeamUsersInvites { get; set; }
        public DbSet<TeamUsersRelation> TeamUsersRelations { get; set; }

    }
}
