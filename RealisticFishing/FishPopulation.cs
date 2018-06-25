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

        public List<Tuple<String, int, int, int>> AllFish;

        public FishPopulation()
        {

            this.AllFish = new List<Tuple<String, int, int, int>>();

            foreach (KeyValuePair<int, string> item in Game1.content.Load<Dictionary<int, string>>("Data\\Fish")) {
                String[] fishFields = item.Value.Split('/');

                if (fishFields[1] != "trap" && fishFields[0] != "Green Algae" && fishFields[0] != "White Algae" && fishFields[0] != "Seaweed") {
                    this.AllFish.Add(new Tuple<string, int, int, int>(fishFields[0], int.Parse(fishFields[3]), int.Parse(fishFields[4]), 1));
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
                    int quality = this.AllFish[i].Item4;
                    double length = EvolutionHelpers.GetMutatedFishLength((maxLength + minLength) / 2, minLength, maxLength);

                    thisFishPopulation.Add(new FishModel(name, minLength, maxLength, length, quality));
                }

                this.population.Add(this.AllFish[i].Item1, thisFishPopulation);
            } 
        }

        public double GetAverageLengthOfFish(String fishName) {
            
            List<FishModel> fishOfType;
            this.population.TryGetValue(fishName, out fishOfType);

            double avg = 0.0;

            foreach (FishModel fish in fishOfType) {
                avg += fish.length;
            }

            avg /= fishOfType.Count;

            return avg;
        }

        public bool IsAverageFishBelowValue(String fishName, double value = 0.66) {

            double avgLength = this.GetAverageLengthOfFish(fishName);

            List<FishModel> fishOfType;
            this.population.TryGetValue(fishName, out fishOfType);

            double originalPopulationAverage = (fishOfType[0].minLength + fishOfType[0].maxLength) / 2;
                
            return (avgLength / originalPopulationAverage) < value;
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
                    ret += fishOfType[0].name + " | Number of Fish: " + fishOfType.Count + " | Average Length: " + this.GetAverageLengthOfFish(fishName) + "\n";
                }
            }

            return ret;
        }
    }
}
