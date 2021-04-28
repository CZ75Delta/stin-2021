using System.Windows;
using Covid_19_Tracker.Model;
using Microsoft.EntityFrameworkCore;

namespace Covid_19_Tracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            using var ctx = new TrackerDbContext();
            ctx.Database.Migrate();
            ctx.SaveChanges();
        }
    }
}
