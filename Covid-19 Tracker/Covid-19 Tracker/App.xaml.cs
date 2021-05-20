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

                //Update on startup
                var apiHandler = new ApiHandler();
                var processData = new ProcessData();
                var dataToDb = new DataToDb();
                var listWho = processData.CSVToListWHOCountries(apiHandler.DownloadFromUrl("https://covid19.who.int/who-data/vaccination-data.csv").Result).Result;
                await dataToDb.InitializeCountries(listWho);
                await dataToDb.SavetoDb(listWho);
                await dataToDb.SavetoDb(processData.JSONToDictMZCR(apiHandler.DownloadFromUrl("https://onemocneni-aktualne.mzcr.cz/api/v2/covid-19/zakladni-prehled.json").Result).Result);
                await dataToDb.SavetoDb(processData.CSVToDictWHOCR(apiHandler.DownloadFromUrl("https://covid19.who.int/WHO-COVID-19-global-data.csv").Result).Result);
            });
        }
    }
}
