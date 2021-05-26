using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Covid_19_Tracker.Model
{
    public class CountryVaccination : INotifyPropertyChanged
    {
        public string Name { get; private set; }
        private bool _isPicked;
        public char Symbol {get; private set;}

        public bool IsPicked
        {
            get {return this._isPicked;}
            set {
                _isPicked = value;
                Symbol = _isPicked ? '-' : '+';
                OnPropertyChanged();
            }
        }
        public double VaccinatedCounter { get; private set; }
        public string VaccinatedPercent { get; private set; }

        public CountryVaccination(String _name, long _population, double vaccinated)
        {
            IsPicked = false;
            Symbol = '+';
            Name = _name;
            VaccinatedCounter = vaccinated;
            VaccinatedPercent = Math.Round((vaccinated) / _population * 100, 1)   + " %";
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
