using System;

namespace Covid_19_Tracker.Model
{
    public class Infected
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public int TotalCases { get; set; }
        public int NewCases { get; set; }
        public DateTime Date { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
