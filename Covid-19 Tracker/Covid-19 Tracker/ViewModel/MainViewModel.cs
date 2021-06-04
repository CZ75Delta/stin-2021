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
using ScottPlot.Plottable;
using Serilog;
using System.ComponentModel;
using System.Threading;
using Timer = System.Timers.Timer;


namespace Covid_19_Tracker.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        #region Global Variables

        private readonly WpfPlot _infectedPlotControl;
        private readonly WpfPlot _vaccinatedPlotControl;
        private SignalPlot _mzcrPlot;
        private SignalPlot _whoPlot;
        private ScatterPlot _highlightedPointWho;
        private ScatterPlot _highlightedPointMzcr;
        private DateTime _lastUpdate;
        private DateTime _selectedDate;
        private DateTime _earliestDate;
        private DateTime _latestDate;
        private Timer _updateTimer;
        private DispatcherTimer _retryTextTimer;
        private ObservableCollection<Infected> _infected;
        private ObservableCollection<CountryVaccination> _countries;
        private ObservableCollection<CountryVaccination> _countriesPicked;
        private int _retrySeconds;
        private int _mzcrLastHighlightedIndex = -1;
        private int _whoLastHighlightedIndex = -1;
        private bool _progressBar;
        private bool _updateEnabled;
        private bool _uiEnabled;
        private bool _pickEnabled;
        private bool _updating;
        private string _progressText;
        private double[] _mzcrValues = new double[730];
        private double[] _whoValues = new double[730];

        #endregion

        #region Bindable Properties

        public string ProgressText { get => _progressText; private set { _progressText = value; OnPropertyChanged(); } }
        public bool ProgressBar { get => _progressBar; private set { _progressBar = value; OnPropertyChanged(); } }
        public bool UpdateEnabled { get => _updateEnabled; private set { _updateEnabled = value; OnPropertyChanged(); } }
        public bool UiEnabled { get => _uiEnabled; private set { _uiEnabled = value; OnPropertyChanged(); } }
        public bool PickEnabled { get => _pickEnabled; private set { _pickEnabled = value; OnPropertyChanged(); } }

        public ObservableCollection<Infected> Infected { get => _infected; private set { _infected = value; OnPropertyChanged(); } }
        public ObservableCollection<CountryVaccination> Countries { get => _countries; private set { _countries = value; OnPropertyChanged(); } }
        public ObservableCollection<CountryVaccination> CountriesPicked { get => _countriesPicked; private init { _countriesPicked = value; OnPropertyChanged(); } }
        public DateTime SelectedDate { get => _selectedDate; set { _selectedDate = value; OnPropertyChanged(); } }
        public DateTime EarliestDate { get => _earliestDate; set { _earliestDate = value; OnPropertyChanged(); } }
        public DateTime LatestDate { get => _latestDate; set { _latestDate = value; OnPropertyChanged(); } }
        public WpfPlot InfectedPlotControl { get => _infectedPlotControl; private init { _infectedPlotControl = value; OnPropertyChanged(); } }
        public WpfPlot VaccinatedPlotControl { get => _vaccinatedPlotControl; private init { _vaccinatedPlotControl = value; OnPropertyChanged(); } }

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
            if (_updating) { return; }
            UpdateEnabled = ProgressBar = _updating = true;
            ProgressText = "Hledám aktualizace...";
            Log.Information("Starting update.");
            if (await CheckInternetConnection.CheckForInternetConnection(1000))
            {
                PickEnabled = false;
                CountriesPicked.Clear();
                Countries.Clear();
                VaccinatedInit();
                _lastUpdate = DateTime.Now;
                _ = await Task.Factory.StartNew(async () =>
                  {
                      //Get and save WHO Vaccinations + Country data
                      var listWho = ProcessData.ProcessWhoVaccinated(ApiHandler.DownloadFromUrl("https://covid19.who.int/who-data/vaccination-data.csv").Result).Result;
                      await DataToDb.InitializeCountries(listWho); //Get and save MZČR Summary
                      await DataToDb.SavetoDb(ProcessData.ProcessMzcr(ApiHandler.DownloadFromUrl("https://onemocneni-aktualne.mzcr.cz/api/v2/covid-19/zakladni-prehled.json").Result).Result);
                      var mzcrMissing = GetMzcrMissingDates().Result;
                      if (mzcrMissing.Count > 0)
                      {
                          var mzcrHistory = await ApiHandler.DownloadFromUrl("https://onemocneni-aktualne.mzcr.cz/api/v2/covid-19/nakazeni-vyleceni-umrti-testy.json");
                          foreach (var missingDate in mzcrMissing)
                          {
                              await DataToDb.SavetoDb(ProcessData.ProcessMzcrDate(mzcrHistory, missingDate).Result);
                          }
                      }
                      //Get and save WHO Infections
                      var whoInfections = await ApiHandler.DownloadFromUrl("https://covid19.who.int/WHO-COVID-19-global-data.csv");
                      await DataToDb.SavetoDb(ProcessData.ProcessWhoInfected(whoInfections).Result);
                      var whoMissing = GetWhoMissingDates().Result;
                      if (whoMissing.Count > 0)
                      {
                          foreach (var missingDate in whoMissing)
                          {
                              await DataToDb.SavetoDb(ProcessData.ProcessWhoInfected(whoInfections, missingDate).Result);
                          }
                      }
                      await DataToDb.FixDailyInfected();
                      await UpdateInfectedToDate();
                      await UpdateCountries();
                      await PlotInfectedData();
                      if (CountriesPicked.Count > 0) UpdateVaccinatedData();
                      
                  });
            }
            else
            {
                UpdateEnabled = false;
                SetRetryTextTimer();
            }
            ProgressText = "Poslední aktualizace v " + _lastUpdate.ToString("HH:mm");
            Log.Information("Update finished.");
            ProgressBar = false;
            UiEnabled = true;
            PickEnabled = true;
            _updating = false;
        }

        /// <summary>
        /// Occurs once the date is changed in the DatePicker. Updates the Infected DataGrid with the data relevant to that date.
        /// </summary>
        private async void OnDateChanged()
        {
            await Task.Factory.StartNew(async () =>
            {
                await UpdateInfectedToDate();
            });
        }

        private void VaccinatedInit()
        {
            using var ctx = new TrackerDbContext();
            if (CountriesPicked.Count > 0)
            {
                var cz = CountriesPicked.FirstOrDefault(x => x.Name == "Czechia");
                var czIndex = CountriesPicked.IndexOf(cz);
                var czDb = ctx.Countries.FirstOrDefault(x => x.Name == "Czechia");
                if (czDb == null) return;
                var vaccinated = ctx.Vaccinated.Where(x => x.Id == czDb.Id).OrderByDescending(x => x.Date).Select(x => (double)x.TotalVaccinations).First();
                var czVac = new CountryVaccination(czDb.Name, czDb.Population, vaccinated);
                CountriesPicked.RemoveAt(czIndex);
                CountriesPicked.Insert(czIndex, czVac);
            }
            else
            {
                var cz = ctx.Countries.FirstOrDefault(x => x.Name == "Czechia");
                if (cz == null) return;
                var vaccinated = ctx.Vaccinated.Where(x => x.Id == cz.Id).OrderByDescending(x => x.Date).Select(x => (double)x.TotalVaccinations).First();
                var czVac = new CountryVaccination(cz.Name, cz.Population, vaccinated);
                CountriesPicked.Add(czVac);
                UpdateVaccinatedData();
            }
        }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            //Initialize global variables
            SelectedDate = DateTime.Today.AddDays(-1);
            Infected = new ObservableCollection<Infected>();
            //Initialize Plot Controls
            InfectedPlotControl = new WpfPlot { Configuration = { DoubleClickBenchmark = false } };
            //Initialize Infected Plot Controls
            VaccinatedPlotControl = new WpfPlot { Configuration = { DoubleClickBenchmark = false, LeftClickDragPan = false, LockHorizontalAxis = true, LockVerticalAxis = true, Zoom = false } };
            //Initialize collections
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

        private static async Task<List<DateTime>> GetMzcrMissingDates()
        {
            await using var ctx = new TrackerDbContext();
            var latestDate = await ctx.Infected.Where(x => x.Source == "mzcr").MaxAsync(r => r.Date);
            var earliestDate = await ctx.Infected.Where(x => x.Source == "mzcr").MinAsync(r => r.Date);
            var range = Enumerable.Range(0, (int)(latestDate - earliestDate).TotalDays + 1).Select(i => earliestDate.AddDays(i));
            return range.Except(await ctx.Infected.Where(x => x.Source == "mzcr").OrderBy(x => x.Date).Select(x => x.Date).ToListAsync()).ToList();
        }
        private static async Task<List<DateTime>> GetWhoMissingDates()
        {
            await using var ctx = new TrackerDbContext();
            var latestDate = await ctx.Infected.Where(x => x.Source == "who").MaxAsync(r => r.Date);
            var earliestDate = await ctx.Infected.Where(x => x.Source == "who").MinAsync(r => r.Date);
            var range = Enumerable.Range(0, (int)(latestDate - earliestDate).TotalDays + 1).Select(i => earliestDate.AddDays(i));
            return range.Except(await ctx.Infected.Where(x => x.Source == "who").OrderBy(x => x.Date).Select(x => x.Date).ToListAsync()).ToList();
        }

        private void PlotInfectedFactory()
        {
            using var ctx = new TrackerDbContext();
            Array.Copy(ctx.Infected.Where(x => x.Source == "mzcr").OrderBy(x => x.Date).Select(x => (double)x.TotalCases).ToArray(), _mzcrValues, 0);
            Array.Copy(ctx.Infected.Where(x => x.Source == "who").OrderBy(x => x.Date).Select(x => (double)x.TotalCases).ToArray(), _whoValues, 0);
            _mzcrPlot = InfectedPlotControl.Plot.AddSignal(_mzcrValues, 1, Color.Crimson, label: "MZČR");
            _whoPlot = InfectedPlotControl.Plot.AddSignal(_whoValues, 1, Color.DarkTurquoise, "WHO");
            _mzcrPlot.OffsetX = ctx.Infected.MinAsync(r => r.Date).Result.ToOADate();
            _whoPlot.OffsetX = ctx.Infected.MinAsync(r => r.Date).Result.ToOADate();
            InfectedPlotControl.MouseDoubleClick += PlotControl_DoubleClick;
            InfectedPlotControl.MouseMove += PlotControl_MouseMove;
            InfectedPlotControl.Plot.XAxis.DateTimeFormat(true);
            InfectedPlotControl.Plot.XLabel("Datum");
            InfectedPlotControl.Plot.YLabel("Celkový počet nakažených");
            InfectedPlotControl.Plot.Title("Porovnání nakažených v ČR z dat MZČR a WHO");
            InfectedPlotControl.Plot.AxisAuto();
            InfectedPlotControl.Plot.Legend();
            _highlightedPointMzcr = InfectedPlotControl.Plot.AddPoint(0, 0);
            _highlightedPointMzcr.Color = Color.Green;
            _highlightedPointMzcr.MarkerSize = 10;
            _highlightedPointMzcr.MarkerShape = MarkerShape.openCircle;
            _highlightedPointMzcr.IsVisible = false;
            _highlightedPointWho = InfectedPlotControl.Plot.AddPoint(0, 0);
            _highlightedPointWho.Color = Color.Green;
            _highlightedPointWho.MarkerSize = 10;
            _highlightedPointWho.MarkerShape = MarkerShape.openCircle;
            _highlightedPointWho.IsVisible = false;
        }

        private void PlotVaccinatedFactory()
        {
            VaccinatedPlotControl.Plot.SetAxisLimits(yMin: 0, yMax: 100);
            VaccinatedPlotControl.Plot.XLabel("Země");
            VaccinatedPlotControl.Plot.XAxis.TickLabelStyle(rotation: 15);
            VaccinatedPlotControl.Plot.YLabel("% proočkovanost");
            VaccinatedPlotControl.Plot.Title("Proočkovanost ve vybraných státech");
            VaccinatedPlotControl.RightClicked -= VaccinatedPlotControl.DefaultRightClickEvent;
        }

        private void UpdateVaccinatedData()
        {
            VaccinatedPlotControl.Plot.Clear();
            PlotVaccinatedFactory();
            var (values, positions, labels) = GetVaccinatedData();
            if (values.Length <= 0 || positions.Length <= 0 || labels.Length <= 0) return;
            VaccinatedPlotControl.Plot.AddBar(values, positions);
            VaccinatedPlotControl.Plot.XTicks(positions, labels);
            VaccinatedPlotControl.Plot.SetAxisLimits(xMax: positions.Length - 0.5);
            VaccinatedPlotControl.Render();
        }

        private (double[], double[], string[]) GetVaccinatedData()
        {
            using var ctx = new TrackerDbContext();
            var count = CountriesPicked.Count;
            var vaccinatedValues = new double[count];
            var vaccinatedPositions = new double[count];
            var vaccinatedLabels = new string[count];
            for (var i = 0; i < count; i++)
            {
                vaccinatedValues[i] = double.Parse(CountriesPicked[i].VaccinatedPercent.Split(" %")[0]);
                vaccinatedPositions[i] = i;
                vaccinatedLabels[i] = CountriesPicked[i].Name;
            }
            return (vaccinatedValues, vaccinatedPositions, vaccinatedLabels);
        }

        private async void PlotControl_DoubleClick(object sender, EventArgs e)
        {
            SelectedDate = DateTime.FromOADate(_highlightedPointWho.Xs[0]);
            await UpdateInfectedToDate();
        }

        private void PlotControl_MouseMove(object sender, EventArgs e)
        {
            var (mouseCoordinateX, _) = InfectedPlotControl.GetMouseCoordinates();
            var (mzcrPointX, mzcrPointY, mzcrPointIndex) = _mzcrPlot.GetPointNearestX(mouseCoordinateX);
            var (whoPointX, whoPointY, whoPointIndex) = _whoPlot.GetPointNearestX(mouseCoordinateX);
            _highlightedPointMzcr.Xs[0] = mzcrPointX;
            _highlightedPointMzcr.Ys[0] = mzcrPointY;
            _highlightedPointMzcr.IsVisible = true;
            _highlightedPointWho.Xs[0] = whoPointX;
            _highlightedPointWho.Ys[0] = whoPointY;
            _highlightedPointWho.IsVisible = true;
            if (_mzcrLastHighlightedIndex == mzcrPointIndex && _whoLastHighlightedIndex == whoPointIndex) return;
            _mzcrLastHighlightedIndex = mzcrPointIndex;
            _whoLastHighlightedIndex = whoPointIndex;
            InfectedPlotControl.Render();
        }

        /// <summary>
        /// Plots data with data from the Infected table in the database
        /// </summary>
        private async Task PlotInfectedData()
        {
            if (_mzcrPlot == null)
            {
                //PlotVaccinatedFactory();
                PlotInfectedFactory();
                await PlotInfectedData();
            }
            else
            {
                await using var ctx = new TrackerDbContext();
                var casesMzcr = await ctx.Infected.Where(x => x.Source == "mzcr").OrderBy(x => x.Date).Select(x => (double)x.TotalCases).ToListAsync();
                var casesWho = await ctx.Infected.Where(x => x.Source == "who").OrderBy(x => x.Date).Select(x => (double)x.TotalCases).ToListAsync();
                Array.Clear(_mzcrValues, 0, _mzcrValues.Length);
                Array.Clear(_whoValues, 0, _whoValues.Length);
                Array.Copy(casesMzcr.ToArray(), _mzcrValues, casesMzcr.Count);
                Array.Copy(casesWho.ToArray(), _whoValues, casesWho.Count);
                _mzcrPlot.MaxRenderIndex = Array.FindLastIndex(_mzcrValues, value => value != 0);
                _whoPlot.MaxRenderIndex = Array.FindLastIndex(_whoValues, value => value != 0);
                InfectedPlotControl.Plot.AxisAuto();
                InfectedPlotControl.Plot.Render();
            }
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
            var countries = new List<CountryVaccination>();

            foreach (var country in ctx.Countries)
            {
                if (country.Name == "Czechia") continue;
                var vaccinated = await ctx.Vaccinated.Where(x => x.Id == country.Id).OrderByDescending(x => x.Date).Select(x => (double)x.TotalVaccinations).FirstAsync();
                var cc = new CountryVaccination(country.Name, country.Population, vaccinated);
                cc.PropertyChanged += CountryPropertyChanged;
                countries.Add(cc);
            }
            Countries = new ObservableCollection<CountryVaccination>(countries);
        }

        //Nastane, pokud je změna na některé z položek
        private void CountryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var country = (CountryVaccination)sender;
            if (CountriesPicked.Contains(country))
            {
                CountriesPicked.Remove(country);
            }
            else
            {
                CountriesPicked.Add(country);
            }
            UpdateVaccinatedData();
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
            //UpdateData();
        }

        #endregion

        /// <summary>
        /// Notifies the UI that a property has been changed and that it should be updated in the UI.
        /// </summary>
        /// <param name="propertyName">Property to update in the UI.</param>
        private new void OnPropertyChanged([CallerMemberName] string propertyName = "") { base.OnPropertyChanged(propertyName); }
    }
}
