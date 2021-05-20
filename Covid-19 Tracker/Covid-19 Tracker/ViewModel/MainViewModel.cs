using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly CheckInternetConnection _checkInternet;

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

        private async void UpdateData()
        {
            if (await _checkInternet.CheckForInternetConnection(1000))
            {
                await Task.Factory.StartNew(async () =>
                {
                    ProgressBar = 0;

                    //GET WHO Vaccinations
                    var listWho = _processData.CSVToListWHOCountries(_apiHandler.DownloadFromUrl("https://covid19.who.int/who-data/vaccination-data.csv").Result).Result;
                    await _dataToDb.InitializeCountries(listWho);
                    ProgressBar = 20;

                    await _dataToDb.SavetoDb(listWho);
                    ProgressBar = 40;

                    //GET MZČR Summary
                    await _dataToDb.SavetoDb(_processData.JSONToDictMZCR(_apiHandler.DownloadFromUrl("https://onemocneni-aktualne.mzcr.cz/api/v2/covid-19/zakladni-prehled.json").Result).Result);
                    ProgressBar = 60;

                    //GET WHO Infections
                    await _dataToDb.SavetoDb(_processData.CSVToDictWHOCR(_apiHandler.DownloadFromUrl("https://covid19.who.int/WHO-COVID-19-global-data.csv").Result).Result);
                    ProgressBar = 80;

                    ProgressText = "Naposledy aktualizováno v " + DateTime.Now.ToString("HH:mm");
                });
            }
            else
            {
                ProgressText = "Nelze se připojit k internetu.";
            }
        }

        #endregion

        public MainViewModel()
        {
            _dataToDb = new DataToDb();
            _apiHandler = new ApiHandler();
            _processData = new ProcessData();
            _checkInternet = new CheckInternetConnection();
            RefreshCommand = new Command(UpdateData);
            ProgressText = "Naposledy aktualizováno v " + DateTime.Now.ToString("HH:mm");
        }

        private new void OnPropertyChanged([CallerMemberName] string propertyName = "") { base.OnPropertyChanged(propertyName); }
    }
}
