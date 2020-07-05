using Microsoft.EntityFrameworkCore;
using Rockstars.Classes;

namespace Rockstars.Models
{
    public class BandContext : DbContext
    {
        public DbSet<Band> Bands { get; set; }
        public BandContext(DbContextOptions<BandContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Band>(entity => {
                entity.HasIndex(e => e.Name).IsUnique();
            });
        }     
    }
}
