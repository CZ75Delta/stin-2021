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

        public void UpdateData()
        {
            ProgressBar = 0;
            //MZČR
            var prehledMzcr = _apiHandler.DownloadFromUrl("https://onemocneni-aktualne.mzcr.cz/api/v2/covid-19/zakladni-prehled.json");
            ProgressBar = 8;
            
            dictMzcr = new Dictionary<string, string>();
            dictMzcr =_processData.JSONToDictMZCR(prehledMzcr);
            ProgressBar = 16;

            //WHO ČR
            string textWhoCr = _apiHandler.DownloadFromUrl("https://covid19.who.int/WHO-COVID-19-global-data.csv");
            ProgressBar = 24;
            dictWho = new Dictionary<string, string>();
            dictWho = _processData.CSVToDictWHOCR(textWhoCr);
            ProgressBar = 32;

            //WHO Countries
            string textWhoCountries = _apiHandler.DownloadFromUrl("https://covid19.who.int/who-data/vaccination-data.csv");
            ProgressBar = 40;

            listWho = new List<Dictionary<string, string>>();
            listWho = _processData.CSVToListWHOCountries(textWhoCountries);
            ProgressBar = 50;
            
            // TODO - tady někde po updatu dat zavolat update populací všech zemí
            
            string test = _apiHandler.DownloadFromUrl("https://covid19.who.int/who-data/vaccination-data.csv");
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
