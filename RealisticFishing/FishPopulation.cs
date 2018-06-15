using System;
using System.Collections.Generic;
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

        public static List<Tuple<String, int, int>> AllFish;

        public FishPopulation()
        {

            FishPopulation.AllFish = new List<Tuple<String, int, int>>();

            foreach (KeyValuePair<int, string> item in Game1.content.Load<Dictionary<int, string>>("Data\\Fish")) {
                String[] fishFields = item.Value.Split('/');

                if (fishFields[1] != "trap") {
                    FishPopulation.AllFish.Add(new Tuple<string, int, int>(fishFields[0], int.Parse(fishFields[3]), int.Parse(fishFields[4])));
                }
            }

            this.population = new Dictionary<String, List<FishModel>>();


            Random rand = new Random();

            for (int i = 0; i < FishPopulation.AllFish.Count; i++) {

                List<FishModel> thisFishPopulation = new List<FishModel>();

                int populationSize = rand.Next(30, 60);

                for (int j = 0; j < populationSize; j++) {
                    String name = FishPopulation.AllFish[i].Item1;
                    int minLength = FishPopulation.AllFish[i].Item2;
                    int maxLength = FishPopulation.AllFish[i].Item3;
                    double length = rand.NextDouble() * (maxLength - minLength) + minLength;

                    thisFishPopulation.Add(new FishModel(name, minLength, maxLength, length));
                }

                this.population.Add(FishPopulation.AllFish[i].Item1, thisFishPopulation);
            } 
        }

        public override String ToString() {
            String ret = "";

            for (int i = 0; i < FishPopulation.AllFish.Count; i++)
            {
                List<FishModel> fishOfType;
                this.population.TryGetValue(FishPopulation.AllFish[i].Item1, out fishOfType);

                if (fishOfType.Count > 0)
                {
                    ret += fishOfType[0].Name + " | Number of Fish: " + fishOfType.Count + "\n";
                }
            }

            return ret;
        }
    }
}
