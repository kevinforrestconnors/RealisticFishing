using System;
using System.Collections.Generic;
using StardewValley;

namespace RealisticFishing
{
    public class RealisticFishingData
    {

        public int NumFishCaughtToday { get; set; }
        public List<String> AllFishCaughtToday { get; set; }
        public FishPopulation fp { get; set; }
        public Dictionary<String, List<FishModel>> population { get; set; }

        public RealisticFishingData()
        {
            this.NumFishCaughtToday = 0;
            this.AllFishCaughtToday = new List<String>();
            this.fp = new FishPopulation();
            this.population = this.fp.population;
        }
    }
}
