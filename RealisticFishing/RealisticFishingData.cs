using System;
using System.Collections.Generic;

namespace RealisticFishing
{
    public class RealisticFishingData
    {

        public int NumFishCaughtToday { get; set; }
        public FishPopulation fp { get; set; }
        public Dictionary<String, List<FishModel>> population { get; set; }

        public RealisticFishingData()
        {
            this.NumFishCaughtToday = 0;
            this.fp = new FishPopulation();
            this.population = this.fp.population;
        }
    }
}
