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

        private string _progressText;
        private int _progressBar;

        private Dictionary<string, string> dictMzcr;
        private Dictionary<string, string> dictWho;
        private DataToDb _dataToDb;

        private List<Dictionary<string, string>> listWho;

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
        public void UpdateData()
        {
            ProgressBar = 0;
            //MZČR
            var prehledMzcr = _apiHandler.DownloadFromUrl("https://onemocneni-aktualne.mzcr.cz/api/v2/covid-19/zakladni-prehled.json");
            ProgressBar = 8;
            
            dictMzcr = new Dictionary<string, string>();
            dictMzcr =_processData.JSONToDictMZCR(ProgressText);
            _dataToDb.DictToDb(dictMzcr);
            ProgressBar = 16;

            //WHO ČR
            string textWhoCr = _apiHandler.DownloadFromUrl("https://covid19.who.int/WHO-COVID-19-global-data.csv");
            ProgressBar = 24;
            dictWho = new Dictionary<string, string>();
            dictWho = _processData.CSVToDictWHOCR(textWhoCr);
            _dataToDb.DictToDb(dictWho);
            ProgressBar = 32;

            //WHO Countries
            string textWhoCountries = _apiHandler.DownloadFromUrl("https://covid19.who.int/who-data/vaccination-data.csv");
            ProgressBar = 40;

            listWho = new List<Dictionary<string, string>>();
            listWho = _processData.CSVToListWHOCountries(textWhoCountries);
            foreach (var dict in listWho)
            {
                _dataToDb.DictToDb(dict);
            }
            ProgressBar = 50;
            
            _dataToDb.UpdatePopulation();
        }

        #endregion

        public MainViewModel()
        {
            _apiHandler = new ApiHandler();
            _processData = new ProcessData();
            RefreshCommand = new Command(UpdateData);
        }

        private new void OnPropertyChanged([CallerMemberName] string propertyName = "") { base.OnPropertyChanged(propertyName); }
    }
}
