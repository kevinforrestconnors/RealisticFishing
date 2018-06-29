using System;
using System.Collections.Generic;
using RealiticFishing;
using StardewValley;

namespace RealisticFishing
{
    public class RealisticFishingData
    {
        
        public List<Tuple<int, List<FishModel>>> inventory { get; set; }
        public int CurrentFishIDCounter;
        public int NumFishCaughtToday { get; set; }
        public List<Tuple<string, int>> AllFishCaughtToday { get; set; }
        public FishPopulation fp { get; set; }
        public Dictionary<String, List<FishModel>> population { get; set; }

        public RealisticFishingData()
        {
            this.inventory = new List<Tuple<int, List<FishModel>>>();
            this.NumFishCaughtToday = 0;
            this.AllFishCaughtToday = new List<Tuple<string, int>>();
            this.fp = new FishPopulation();
            this.population = this.fp.population;
            this.CurrentFishIDCounter = this.fp.CurrentFishIDCounter;
        }
    }
}
