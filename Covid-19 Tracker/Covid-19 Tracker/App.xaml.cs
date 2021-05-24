using System.Threading.Tasks;
using System.Windows;
using Covid_19_Tracker.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Covid_19_Tracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            var ic = new IdentifyComputer();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10,
                    fileSizeLimitBytes: 52428800, rollOnFileSizeLimit: true)
                .WriteTo.Telegram(
                    "1896853074:AAGadAOmXiE90sTPlsxEyniV-f0WnU1CKlA", "-500972830", applicationName: IdentifyComputer.GetIdentification().Result)
                .CreateLogger();
            Log.Information("Application started.");
            await Task.Factory.StartNew(async () =>
            {
                //Migrate Database
                await using var ctx = new TrackerDbContext();
                await ctx.Database.MigrateAsync();
                await ctx.SaveChangesAsync();
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
