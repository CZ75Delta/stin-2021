﻿using System.Collections.Generic;

namespace Covid_19_Tracker.Model
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Population { get; set; }
        public List<Infected> Infected { get; set; }
        public List<Vaccinated> Vaccinated { get; set; }
    }
}