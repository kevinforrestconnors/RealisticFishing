using System;
using System.Collections.Generic;
using StardewValley;

namespace RealisticFishing
{
    public class RealisticFishingData
    {
        public int FishIDCounter;
        public int NumFishCaughtToday { get; set; }
        public List<Tuple<string, int>> AllFishCaughtToday { get; set; }
        public FishPopulation fp { get; set; }
        public Dictionary<String, List<FishModel>> population { get; set; }

        public RealisticFishingData()
        {
            this.FishIDCounter = 0;
            this.NumFishCaughtToday = 0;
            this.AllFishCaughtToday = new List<Tuple<string, int>>();
            this.fp = new FishPopulation();
            this.population = this.fp.population;
        }
    }
}
