using System.Collections.Generic;
using System.Linq;

namespace Covid_19_Tracker.Model
{
    public class DataToDB
    {
        public void DictDataToDB(Dictionary<string, string> dict)
        {
            using var ctx = new TrackerDbContext();
            foreach (var pair in dict)
            {
                ctx.Countries.FirstOrDefault(x => x.Sloupec == pair.Key);
                ctx.Countries.Find(pair.Key);
            }
        }
    }
}