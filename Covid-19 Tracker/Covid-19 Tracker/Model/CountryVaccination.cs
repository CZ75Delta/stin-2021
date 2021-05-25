using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Covid_19_Tracker.Model
{
    public class CountryVaccination
    {
        public string Name { get; private set; }
        public bool IsPicked { get; set; }
        public double VaccinatedCounter { get; private set; }
        public string VaccinatedPercent { get; private set; }
        public Visibility Viss {get; private set;}

        public CountryVaccination(String _name, long _population, double vaccinated)
        {
            Viss = Visibility.Collapsed;
            IsPicked = false;
            Name = _name;
            VaccinatedCounter = vaccinated;
            VaccinatedPercent = Math.Round((vaccinated) / _population,3)  * 100 + " %";
        }
    }
}
