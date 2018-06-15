using System;
using System.Collections.Generic;

namespace RealisticFishing
{
    public static class EvolutionHelpers
    {
        
        public static float MutationRate = 0.05f;


        public static double GetMutatedFishLength(int fishLength, int minLength, int maxLength) {

            Random rand = new Random(); //reuse this if you are generating many
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = fishLength + EvolutionHelpers.MutationRate * randStdNormal; // mean + stdDev * randStdNormal   random normal(mean,stdDev^2)

            if (randNormal > maxLength)
            {
                return maxLength;
            }
            else if (randNormal < minLength)
            {
                return minLength;
            }
            else
            {
                return Math.Floor(randNormal);
            }

        }

        public static Dictionary<String, List<FishModel>> ComputeTomorrowPopulation(Dictionary<String, List<FishModel>> population) {
            return new FishPopulation().fishPopulation;
        }
    }
}
