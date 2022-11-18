using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OberMindTestDemo.Models;

namespace OberMindTestDemo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private readonly IConfiguration _Configuration;
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer(_Configuration.GetConnectionString("DefaultConnection"), builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });

            }

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<OrderMaster> OrderMaster { get; set; }

        public DbSet<ItemMaster> ItemMaster { get; set; }

        public DbSet<OrderDetail> OrderDetail { get; set; }

    }
}