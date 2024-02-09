using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class WebApplication1Context : DbContext
    {
        public WebApplication1Context (DbContextOptions<WebApplication1Context> options)
            : base(options)
        {
        }

        public DbSet<QueueType> QueueTypes { get; set; }
        public DbSet<SimpleQueue> SimpleQueues { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Active> Actives { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // modelBuilder.Entity<QueueType>()
             //   .HasNoKey();

            // Other configurations...

            base.OnModelCreating(modelBuilder);
        }

      
    }
}
