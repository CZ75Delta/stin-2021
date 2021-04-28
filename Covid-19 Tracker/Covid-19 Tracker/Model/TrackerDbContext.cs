using Microsoft.EntityFrameworkCore;

namespace Covid_19_Tracker.Model
{
    public class TrackerDbContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Vaccinated> Vaccinated { get; set; }
        public DbSet<Infected> Infected { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=TrackerDb.db");
    }
}
