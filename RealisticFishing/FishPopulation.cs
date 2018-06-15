using System;
using System.Collections.Generic;

namespace RealisticFishing
{

    public class FishPopulation
    {

        public Dictionary<String, List<Fish>> fishPopulation;

        public FishPopulation()
        {
            this.fishPopulation = new Dictionary<String, List<Fish>>();
        }

        public override String ToString() {
            String ret = "";

            for (int i = 0; i < Fish.AllFish.Count; i++)
            {
                List<Fish> fishOfType;
                this.fishPopulation.TryGetValue(Fish.AllFish[i], out fishOfType);

                if (fishOfType.Count > 0)
                {
                    ret += fishOfType[0].name + " | Number of Fish: " + fishOfType.Count + "\n";
                }
            }

            return ret;
        }
    }
}
