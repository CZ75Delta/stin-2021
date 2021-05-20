using System.Threading.Tasks;
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
        protected override async void OnStartup(StartupEventArgs e)
        {
            await Task.Factory.StartNew(async () =>
            {
                //Migrate Database
                await using var ctx = new TrackerDbContext();
                await ctx.Database.MigrateAsync();
                await ctx.SaveChangesAsync();
            });
        }
    }
}
