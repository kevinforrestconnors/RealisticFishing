using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace RealisticFishing
{

    public class FishPopulation
    {

        public Dictionary<String, List<FishModel>> population;

        public List<Tuple<String, int, int>> AllFish;

        public FishPopulation()
        {

            this.AllFish = new List<Tuple<String, int, int>>();

            foreach (KeyValuePair<int, string> item in Game1.content.Load<Dictionary<int, string>>("Data\\Fish")) {
                String[] fishFields = item.Value.Split('/');

                if (fishFields[1] != "trap") {
                    this.AllFish.Add(new Tuple<string, int, int>(fishFields[0], int.Parse(fishFields[3]), int.Parse(fishFields[4])));
                }
            }

            this.population = new Dictionary<String, List<FishModel>>();


            Random rand = new Random();

            for (int i = 0; i < this.AllFish.Count; i++) {

                List<FishModel> thisFishPopulation = new List<FishModel>();

                int populationSize = 50;

                for (int j = 0; j < populationSize; j++) {
                    String name = this.AllFish[i].Item1;
                    int minLength = this.AllFish[i].Item2;
                    int maxLength = this.AllFish[i].Item3;
                    double length = rand.NextDouble() * (maxLength - minLength) + minLength;

                    thisFishPopulation.Add(new FishModel(name, minLength, maxLength, length));
                }

                this.population.Add(this.AllFish[i].Item1, thisFishPopulation);
            } 
        }

        public String PrintChangedFish(List<String> filter) {
            String ret = "";

            for (int i = 0; i < this.AllFish.Count; i++)
            {

                String fishName = this.AllFish[i].Item1;

                List<FishModel> fishOfType;
                this.population.TryGetValue(fishName, out fishOfType);

                if (fishOfType.Count > 0 && (filter.Contains(fishName) || filter.Count == 0)) 
                {
                    ret += fishOfType[0].Name + " | Number of Fish: " + fishOfType.Count + "\n";
                }
            }

            return ret;
        }
    }
}
