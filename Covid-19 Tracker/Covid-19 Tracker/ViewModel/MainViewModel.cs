﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Covid_19_Tracker.Base;
using Covid_19_Tracker.Model;

namespace Covid_19_Tracker.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        #region Global Variables

        private readonly ApiHandler _apiHandler;
        private string _progressText;

        private readonly ProcessData _processData;
        private Dictionary<string, string> dictMzcr;

        #endregion

        #region Bindable Properties

        public string ProgressText { get => _progressText; private set { _progressText = value; OnPropertyChanged(); } }

        #endregion

        #region Command Declarations

        public Command RefreshCommand { get; private set; }

        #endregion

        #region Command Methods

        public void UpdateData()
        {
            ProgressText = _apiHandler.DownloadFromUrl("https://onemocneni-aktualne.mzcr.cz/api/v2/covid-19/zakladni-prehled.json");

            dictMzcr = new Dictionary<string, string>();

            dictMzcr =_processData.JSONToDictMZCR(ProgressText);

        }

        #endregion
        
        public MainViewModel()
        {
            _apiHandler = new ApiHandler();
            _processData = new ProcessData();
            RefreshCommand = new Command(UpdateData);
        }

        public new void OnPropertyChanged([CallerMemberName] string propertyName = "") { base.OnPropertyChanged(propertyName); }
    }
}
