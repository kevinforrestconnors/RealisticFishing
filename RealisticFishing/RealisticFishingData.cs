using System;
using System.Collections.Generic;
using RealiticFishing;
using StardewValley;

namespace RealisticFishing
{
    public class RealisticFishingData
    {
        public int CurrentFishIDCounter;
        public int NumFishCaughtToday { get; set; }
        public List<Tuple<string, int>> AllFishCaughtToday { get; set; }
        public FishPopulation fp { get; set; }
        public Dictionary<String, List<FishModel>> population { get; set; }

        public Dictionary<int, List<FishModel>> inventory { get; set; }

        public RealisticFishingData()
        {
            this.NumFishCaughtToday = 0;
            this.AllFishCaughtToday = new List<Tuple<string, int>>();
            this.fp = new FishPopulation();
            this.population = this.fp.population;
            this.CurrentFishIDCounter = this.fp.CurrentFishIDCounter;
            this.inventory = new Dictionary<int, List<FishModel>>();
        }
    }
}
