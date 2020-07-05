using Microsoft.EntityFrameworkCore;
using Rockstars.Classes;

namespace Rockstars.Models
{
    public class SongContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }
        public SongContext(DbContextOptions<SongContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Song>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });
        }
    }
}
