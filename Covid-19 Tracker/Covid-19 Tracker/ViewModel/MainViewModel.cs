using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Threading;
using Covid_19_Tracker.Base;
using Covid_19_Tracker.Model;
using Microsoft.EntityFrameworkCore;
using ScottPlot;
using Serilog;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Covid_19_Tracker.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        #region Global Variables

        private readonly WpfPlot _infectedWpfPlot;
        private readonly WpfPlot _vaccinatedWpfPlot;
        private Timer _updateTimer;
        private DispatcherTimer _retryTextTimer;
        private int _retrySeconds;
        private string _progressText;
        private bool _progressBar;
        private bool _updateEnabled;
        private bool _uiEnabled;
        private ObservableCollection<Infected> _infected;
        private ObservableCollection<CountryVaccination> _countries;
        private ObservableCollection<CountryVaccination> _countriesPicked;
        private DateTime _lastUpdate;
        private DateTime _selectedDate;
        private DateTime _earliestDate;
        private DateTime _latestDate;

        #endregion

        #region Bindable Properties

        public string ProgressText { get => _progressText; private set { _progressText = value; OnPropertyChanged(); } }
        public bool ProgressBar { get => _progressBar; private set { _progressBar = value; OnPropertyChanged(); } }
        public bool UpdateEnabled { get => _updateEnabled; private set { _updateEnabled = value; OnPropertyChanged(); } }
        public bool UiEnabled { get => _uiEnabled; private set { _uiEnabled = value; OnPropertyChanged(); } }
        public ObservableCollection<Infected> Infected { get => _infected; private set { _infected = value; OnPropertyChanged(); } }
        public ObservableCollection<CountryVaccination> Countries { get => _countries; private set { _countries = value; OnPropertyChanged(); } }
        public ObservableCollection<CountryVaccination> CountriesPicked { get => _countriesPicked; private set { _countriesPicked = value; OnPropertyChanged(); } }


        public DateTime SelectedDate { get => _selectedDate; set { _selectedDate = value; OnPropertyChanged(); } }
        public DateTime EarliestDate { get => _earliestDate; set { _earliestDate = value; OnPropertyChanged(); } }
        public DateTime LatestDate { get => _latestDate; set { _latestDate = value; OnPropertyChanged(); } }
        public WpfPlot InfectedWpfPlot { get => _infectedWpfPlot; private init { _infectedWpfPlot = value; OnPropertyChanged(); } }
        public WpfPlot VaccinatedWpfPlot { get => _vaccinatedWpfPlot; private init { _vaccinatedWpfPlot = value; OnPropertyChanged(); } }

        #endregion

        #region Command Declarations

        public Command RefreshCommand { get; }
        public Command OnDateChangedCommand { get; }

        #endregion

        #region Command Methods

        /// <summary>
        /// Occcurs once the automatic update timer elapses, or the update button is pressed. Tries to update the database with the most recent data if possible. If there's no internet connection starts a timer that will retry the update every 30s.
        /// </summary>
        private async void UpdateData()
        {
            Log.Information("Starting update.");
            if (await CheckInternetConnection.CheckForInternetConnection(1000))
            {
                _lastUpdate = DateTime.Now;
                await Task.Factory.StartNew(async () =>
                {
                    ProgressText = "Hledám aktualizace...";
                    UpdateEnabled = ProgressBar = true;

                    //Get and save WHO Vaccinations + Country data
                    var listWho = ProcessData.ProcessWhoVaccinated(ApiHandler.DownloadFromUrl("https://covid19.who.int/who-data/vaccination-data.csv").Result).Result;
                    await DataToDb.InitializeCountries(listWho);
                    await DataToDb.SaveToDb(listWho);

                    //Get and save MZČR Summary
                    await DataToDb.SavetoDb(ProcessData.ProcessMzcr(ApiHandler.DownloadFromUrl("https://onemocneni-aktualne.mzcr.cz/api/v2/covid-19/zakladni-prehled.json").Result).Result);

                    //Get and save WHO Infections
                    await DataToDb.SavetoDb(ProcessData.ProcessWhoInfected(ApiHandler.DownloadFromUrl("https://covid19.who.int/WHO-COVID-19-global-data.csv").Result).Result);

                    await UpdateInfectedToDate();
                    await UpdateCountries();

                    ProgressText = "Poslední aktualizace v " + _lastUpdate.ToString("HH:mm");
                    Log.Information("Update finished.");
                });
                
            }
            else
            {
                UpdateEnabled = false;
                SetRetryTextTimer();
            }

            ProgressBar = false;
            UiEnabled = true;
            await PlotInfectedData();
        }

        /// <summary>
        /// Occurs once the date is changed in the DatePicker. Updates the DataGrid with the data relevant to that date.
        /// </summary>
        private async void OnDateChanged()
        {
            await Task.Factory.StartNew(async () =>
            {
                await UpdateInfectedToDate();
            });
        }

        #endregion

        #region Constructor
        public MainViewModel()
        {
            //Initialize global variables
            InfectedWpfPlot = new WpfPlot();
            VaccinatedWpfPlot = new WpfPlot();
            SelectedDate = DateTime.Today.AddDays(-1);
            Infected = new ObservableCollection<Infected>();
            Countries = new ObservableCollection<CountryVaccination>();
            CountriesPicked = new ObservableCollection<CountryVaccination>();
            //Initialize View Commands
            RefreshCommand = new Command(_ => true, _ => UpdateData());
            OnDateChangedCommand = new Command(_ => true, _ => OnDateChanged());
            //Set Progress Text
            ProgressText = "Poslední aktualizace v " + DateTime.Now.ToString("HH:mm");
            //Set Update Timer
            SetUpdateTimer(600000);
            //Call data update
            UpdateData();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Plots data with data from the Infected table in the database
        /// </summary>
        private async Task PlotInfectedData()
        {
            await using var ctx = new TrackerDbContext();
            var dateTimesMzcr = await ctx.Infected.Where(x => x.Source == "mzcr").Select(x => x.Date).Distinct().ToListAsync();
            var dateTimesWho = await ctx.Infected.Where(x => x.Source == "who").Select(x => x.Date).Distinct().ToListAsync();
            var casesMzcr = await ctx.Infected.Where(x => x.Source == "mzcr").Select(x => (double)x.TotalCases).Distinct().ToListAsync();
            var casesWho = await ctx.Infected.Where(x => x.Source == "who").Select(x => (double)x.TotalCases).Distinct().ToListAsync();
            InfectedWpfPlot.Plot.Clear();
            InfectedWpfPlot.Plot.AddScatter(dateTimesMzcr.Select(x => x.ToOADate()).ToArray(), casesMzcr.ToArray(),Color.Crimson,label:"MZČR");
            InfectedWpfPlot.Plot.AddScatter(dateTimesWho.Select(x => x.ToOADate()).ToArray(), casesWho.ToArray(), Color.DarkTurquoise, label: "WHO");
            InfectedWpfPlot.Plot.XAxis.DateTimeFormat(true);
            InfectedWpfPlot.Plot.Legend();
            InfectedWpfPlot.Plot.Title("Porovnání nakažených v ČR z dat MZČR a WHO");
            InfectedWpfPlot.Plot.XLabel("Datum");
            InfectedWpfPlot.Plot.YLabel("Celkový počet nakažených");
        }

        /// <summary>
        /// Updates the Infected UI from the information in the database.
        /// </summary>
        private async Task UpdateInfectedToDate()
        {
            await using var ctx = new TrackerDbContext();
            Infected = new ObservableCollection<Infected>(await ctx.Infected.Where(x => x.Date.Date == SelectedDate.Date).ToListAsync());
            LatestDate = await ctx.Infected.MaxAsync(r => r.Date);
            EarliestDate = await ctx.Infected.MinAsync(r => r.Date);
        }

        private async Task UpdateCountries()
        {
            await using var ctx = new TrackerDbContext();
            List<CountryVaccination> countries = new List<CountryVaccination>();

            foreach (Country country in ctx.Countries)
            {
                var vaccinated = await ctx.Vaccinated.Where(x => x.Id == country.Id).Select(x => (double)x.TotalVaccinations).Distinct().ToListAsync();
                CountryVaccination cc = new CountryVaccination(country.Name, country.Population, vaccinated[0]);
                cc.PropertyChanged += CountryPropertyChanged;
                countries.Add(cc);

            }
            Countries = new ObservableCollection<CountryVaccination>(countries);
        }
        //Nastane, pokud je změna na některé z položek
        private void CountryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CountryVaccination country = (CountryVaccination)sender;
            if (CountriesPicked.Contains(country))
            {
                CountriesPicked.Remove(country);
            }
            else
            {
                CountriesPicked.Add(country);
            }
            //Trochu na prasáka donucení akutalizace kolekce
            int index = Countries.IndexOf(country);
            Countries.Remove(country);
            Countries.Insert(index, country);
        }



        /// <summary>
        /// Timer called every time there's no internet connection. Runs for 30 seconds by default, calls SetRetryTextTimerTick every second.
        /// </summary>
        /// <param name="interval">Time interval determining how often the timer elapses.</param>
        private void SetRetryTextTimer(int interval = 30)
        {
            _retryTextTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            _retryTextTimer.Tick += SetRetryTextTimerTick;
            _retryTextTimer.Start();
            _retrySeconds = interval;
            Log.Warning("Starting retry timer.");
            ProgressText = "Nelze se připojit k internetu, zkouším znovu za " + interval + "s";
        }

        /// <summary>
        /// Tick event called for each 1s tick of SetRetryTextTimer. Calls the update method after the set time elapses.
        /// </summary>
        private void SetRetryTextTimerTick(object sender, EventArgs e)
        {
            _retrySeconds -= 1;
            ProgressText = "Nelze se připojit k internetu, zkouším znovu za " + _retrySeconds + "s";
            if (_retrySeconds != 0) return;
            _retryTextTimer.Stop();
            UpdateData();
            Log.Warning("Retrying update.");
        }

        /// <summary>
        /// Sets a Timer to call the update method every time the timer elapses.
        /// </summary>
        /// <param name="interval">Time interval determining how often the timer elapses.</param>
        private void SetUpdateTimer(double interval)
        {
            _updateTimer = new Timer(interval);
            _updateTimer.Elapsed += UpdateDataEvent;
            _updateTimer.AutoReset = true;
            _updateTimer.Enabled = true;
        }

        /// <summary>
        /// Timed event for SetUpdateTimer, calls the data update method when triggered after the timer elapses.
        /// </summary>
        private void UpdateDataEvent(object sender, ElapsedEventArgs e)
        {
            UpdateData();
        }

        #endregion

        /// <summary>
        /// Notifies the UI that a property has been changed and that it should be updated in the UI.
        /// </summary>
        /// <param name="propertyName">Property to update in the UI.</param>
        private new void OnPropertyChanged([CallerMemberName] string propertyName = "") { base.OnPropertyChanged(propertyName); }
    }
}
