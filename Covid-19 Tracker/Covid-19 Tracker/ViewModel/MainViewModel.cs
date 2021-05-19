using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Covid_19_Tracker.Base;
using Covid_19_Tracker.Model;

namespace Covid_19_Tracker.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        #region Global Variables

        private readonly ApiHandler _apiHandler;
        private readonly ProcessData _processData;
        private readonly DataToDb _dataToDb;

        private string _progressText;
        private int _progressBar;

        #endregion

        #region Bindable Properties

        public string ProgressText { get => _progressText; private set { _progressText = value; OnPropertyChanged(); } }
        public int ProgressBar { get => _progressBar; private set { _progressBar = value; OnPropertyChanged(); } }


        #endregion

        #region Command Declarations

        public Command RefreshCommand { get; private set; }

        #endregion

        #region Command Methods

        // TODO - kontrola internetového připojení
        // TODO - podle kontroly připojení internetu omezit použití aktualizace nebo vypsat varovnou zprávu
        private void UpdateData()
        {
            //Reset ProgressBar
            ProgressBar = 0;

            //WHO Countries
            var listWho = _processData.CSVToListWHOCountries(_apiHandler.DownloadFromUrl("https://covid19.who.int/who-data/vaccination-data.csv"));
            foreach (var dict in listWho)
            {
                _dataToDb.DictToDb(dict);
            }
            ProgressBar = 20;

            //Get MZČR Summary
            _dataToDb.DictToDb(_processData.JSONToDictMZCR(_apiHandler.DownloadFromUrl("https://onemocneni-aktualne.mzcr.cz/api/v2/covid-19/zakladni-prehled.json")));
            ProgressBar = 40;

            //GET WHO Infections
            _dataToDb.DictToDb(_processData.CSVToDictWHOCR(_apiHandler.DownloadFromUrl("https://covid19.who.int/WHO-COVID-19-global-data.csv")));
            ProgressBar = 60;

            _dataToDb.UpdatePopulation();
            ProgressBar = 80;
        }

        #endregion

        public MainViewModel()
        {
            _dataToDb = new DataToDb();
            _apiHandler = new ApiHandler();
            _processData = new ProcessData();
            RefreshCommand = new Command(UpdateData);
        }

        private new void OnPropertyChanged([CallerMemberName] string propertyName = "") { base.OnPropertyChanged(propertyName); }
    }
}
