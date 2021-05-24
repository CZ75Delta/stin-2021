using System;
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


namespace Covid_19_Tracker.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        #region Global Variables

        private readonly ApiHandler _apiHandler;
        private readonly ProcessData _processData;
        private readonly DataToDb _dataToDb;
        private readonly CheckInternetConnection _checkInternet;
        private Timer _updateTimer;
        private DispatcherTimer _retryTextTimer;
        private int _retrySeconds;
        private string _progressText;
        private bool _progressBar;
        private bool _updateEnabled;
        private ObservableCollection<Infected> _infected;
        private DateTime _lastUpdate;
        private DateTime _selectedDate;
        private DateTime _earliestDate;
        private DateTime _latestDate;
        private ScottPlot.WpfPlot _plotControl;

        #endregion

        #region Bindable Properties

        public string ProgressText { get => _progressText; private set { _progressText = value; OnPropertyChanged(); } }
        public bool ProgressBar { get => _progressBar; private set { _progressBar = value; OnPropertyChanged(); } }
        public bool UpdateEnabled { get => _updateEnabled; private set { _updateEnabled = value; OnPropertyChanged(); } }
        public ObservableCollection<Infected> Infected { get => _infected; private set { _infected = value; OnPropertyChanged(); } }
        public DateTime SelectedDate { get => _selectedDate; set { _selectedDate = value; OnPropertyChanged(); } }
        public DateTime EarliestDate { get => _earliestDate; set { _earliestDate = value; OnPropertyChanged(); } }
        public DateTime LatestDate { get => _latestDate; set { _latestDate = value; OnPropertyChanged(); } }
        public ScottPlot.WpfPlot PlotControl { get => _plotControl; set { _plotControl = value; OnPropertyChanged(); } }

        #endregion

        #region Command Declarations

        public Command RefreshCommand { get; private set; }
        public Command OnDateChangedCommand { get; private set; }

        #endregion

        #region Command Methods

        private async void UpdateData()
        {
            Log.Information("Starting update.");
            if (await _checkInternet.CheckForInternetConnection(1000))
            {
                _lastUpdate = DateTime.Now;
                await Task.Factory.StartNew(async () =>
                {
                    ProgressText = "Hledám aktualizace...";
                    UpdateEnabled = ProgressBar = true;

                    //GET WHO Vaccinations
                    var listWho = _processData.CSVToListWHOCountries(_apiHandler.DownloadFromUrl("https://covid19.who.int/who-data/vaccination-data.csv").Result).Result;
                    await _dataToDb.InitializeCountries(listWho);

                    await _dataToDb.SavetoDb(listWho);

                    //GET MZČR Summary
                    await _dataToDb.SavetoDb(_processData.JSONToDictMZCR(_apiHandler.DownloadFromUrl("https://onemocneni-aktualne.mzcr.cz/api/v2/covid-19/zakladni-prehled.json").Result).Result);

                    //GET WHO Infections
                    await _dataToDb.SavetoDb(_processData.CSVToDictWHOCR(_apiHandler.DownloadFromUrl("https://covid19.who.int/WHO-COVID-19-global-data.csv").Result).Result);

                    await UpdateInfectedToDate();
                    await SetPlot();

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
        }

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
            _dataToDb = new DataToDb();
            _apiHandler = new ApiHandler();
            _processData = new ProcessData();
            _checkInternet = new CheckInternetConnection();
            PlotControl = new WpfPlot();
            SelectedDate = DateTime.Today.AddDays(-1);
            Infected = new ObservableCollection<Infected>();
            RefreshCommand = new Command(UpdateData);
            OnDateChangedCommand = new Command(OnDateChanged);
            ProgressText = "Poslední aktualizace v " + DateTime.Now.ToString("HH:mm");
            SetUpdateTimer(600000);
            UpdateData();
        }

        #endregion

        #region Private Methods

        private async Task SetPlot()
        {
            await using var ctx = new TrackerDbContext();
            // create data sample data
            var dateTimes = await ctx.Infected.Select(x => x.Date).Distinct().ToListAsync();
            var casesMzcr = await ctx.Infected.Where(x => x.Source == "mzcr").Select(x => (double)x.TotalCases).Distinct().ToListAsync();
            var casesWho = await ctx.Infected.Where(x => x.Source == "who").Select(x => (double)x.TotalCases).Distinct().ToListAsync();
            var xs = dateTimes.Select(x => x.ToOADate()).ToArray();
            var ys = casesMzcr.ToArray();
            PlotControl.Plot.AddScatter(xs, ys,Color.Crimson,label:"MZČR");
            ys = casesWho.ToArray();
            PlotControl.Plot.AddScatter(xs, ys, Color.DarkTurquoise, label: "WHO");
            PlotControl.Plot.XAxis.DateTimeFormat(true);
            PlotControl.Plot.Legend();
            PlotControl.Plot.Title("Porovnání nakažených v ČR z dat MZČR a WHO");
            PlotControl.Plot.XLabel("Datum");
            PlotControl.Plot.YLabel("Celkový počet nakažených");
        }

        private async Task UpdateInfectedToDate()
        {
            await using var ctx = new TrackerDbContext();
            Infected = new ObservableCollection<Infected>(await ctx.Infected.Where(x => x.Date.Date == SelectedDate.Date).ToListAsync());
            LatestDate = await ctx.Infected.MaxAsync(r => r.Date);
            EarliestDate = await ctx.Infected.MinAsync(r => r.Date);
        }

        private void SetRetryTextTimer()
        {
            _retryTextTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            _retryTextTimer.Tick += SetRetryTextTimerTick;
            _retryTextTimer.Start();
            _retrySeconds = 30;
            Log.Warning("Starting retry Timer.");
            ProgressText = "Nelze se připojit k internetu, zkouším znovu za " + _retrySeconds + "s";
        }

        private void SetRetryTextTimerTick(object sender, EventArgs e)
        {
            _retrySeconds -= 1;
            ProgressText = "Nelze se připojit k internetu, zkouším znovu za " + _retrySeconds + "s";
            if (_retrySeconds != 0) return;
            _retryTextTimer.Stop();
            UpdateData();
            Log.Warning("Retrying update.");
        }

        private void SetUpdateTimer(double interval)
        {
            _updateTimer = new Timer(interval);
            _updateTimer.Elapsed += UpdateDataEvent;
            _updateTimer.AutoReset = true;
            _updateTimer.Enabled = true;
        }

        private void UpdateDataEvent(object sender, ElapsedEventArgs e)
        {
            UpdateData();
        }

        #endregion

        private new void OnPropertyChanged([CallerMemberName] string propertyName = "") { base.OnPropertyChanged(propertyName); }
    }
}
