using Microsoft.EntityFrameworkCore;

namespace Covid_19_Tracker.Model.DbContext
{
    public class TrackerDbContext : Microsoft.EntityFrameworkCore.DbContext
    {


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=TrackerDb.db");
    }
}
