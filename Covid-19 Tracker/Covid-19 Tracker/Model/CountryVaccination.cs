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
        public string Name { get; }
        private bool _isPicked;
        public char Symbol {get; private set; }
        public double VaccinatedCounter { get; }
        public string VaccinatedPercent { get; }

        public bool IsPicked
        {
            get => _isPicked;
            set {
                _isPicked = value;
                Symbol = _isPicked ? '-' : '+';
                OnPropertyChanged();
            }
        }

        public CountryVaccination(string name, long population, double vaccinated)
        {
            IsPicked = false;
            Symbol = '+';
            Name = name;
            VaccinatedCounter = vaccinated;
            VaccinatedPercent = Math.Round((vaccinated) / population * 100, 1)   + " %";
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
