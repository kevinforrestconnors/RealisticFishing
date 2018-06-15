using System;
using System.Collections.Generic;

namespace RealisticFishing
{

    public class FishPopulation
    {

        public Dictionary<String, List<FishModel>> fishPopulation;

        public static List<String> AllFish;

        public FishPopulation()
        {

            FishPopulation.AllFish = new List<String>();
            FishPopulation.AllFish.Add("Herring");
            FishPopulation.AllFish.Add("Tuna");
            FishPopulation.AllFish.Add("Mackerel");

            this.fishPopulation = new Dictionary<String, List<FishModel>>();

            for (int i = 0; i < FishPopulation.AllFish.Count; i++) {

                List<Fish> thisFishPopulation = new List<FishModel>();

                int populationSize = new Random().Next(30, 60);

                for (int i = 0; i < populationSize; i++) {
                    thisFishPopulation.Add(new FishModel(FishPopulation.AllFish[i], FishPopulation.GetLengthOfNewFish()));
                }

                //this.fishPopulation.Add(FishPopulation.AllFish[i], 
            } 
        }

        public override String ToString() {
            String ret = "";

            for (int i = 0; i < FishPopulation.AllFish.Count; i++)
            {
                List<FishModel> fishOfType;
                this.fishPopulation.TryGetValue(FishPopulation.AllFish[i], out fishOfType);

                if (fishOfType.Count > 0)
                {
                    ret += fishOfType[0].name + " | Number of Fish: " + fishOfType.Count + "\n";
                }
            }

            return ret;
        }

        public static double GetLengthOfNewFish() {
            return 5.5;
        }
    }
}
