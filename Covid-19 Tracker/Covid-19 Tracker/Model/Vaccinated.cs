using System;

namespace Covid_19_Tracker.Model
{
    public class Vaccinated
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public int TotalVaccinations { get; set; }
        public DateTime Date { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
