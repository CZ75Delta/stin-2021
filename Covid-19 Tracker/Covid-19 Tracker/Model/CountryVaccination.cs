using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covid_19_Tracker.Model
{
    class CountryVaccination
    {
        public readonly string Name;
        public bool IsPicked { get; set; }
        public readonly int VaccinatedCounter;
        public readonly string VaccinatedPercent;

        private CountryVaccination(Country c)
        {
            Name = c.Name;

        }

    }
}
