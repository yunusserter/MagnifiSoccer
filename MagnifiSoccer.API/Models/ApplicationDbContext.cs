using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MagnifiSoccer.API.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        

        public DbSet<Game> Games { get; set; }
        public DbSet<GamePlayer> GamePlayers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public override DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .ToTable("User");

            modelBuilder.Entity<Rating>()
                .ToTable("Rating");

            modelBuilder.Entity<Game>()
                .ToTable("Game");

            modelBuilder.Entity<GamePlayer>()
                .ToTable("GamePlayer");

            modelBuilder.Entity<Group>()
                .ToTable("Group");

            modelBuilder.Entity<GroupMember>()
                .ToTable("GroupMember");
        }
    }
}

