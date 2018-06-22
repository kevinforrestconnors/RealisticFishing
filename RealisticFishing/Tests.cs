using System;
using RealiticFishing.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System.Collections.Generic;
using RealiticFishing;

namespace RealisticFishing
{
    public static class Tests
    {
        
        public static bool ShouldRunTests = false;
        public static bool RunningTests = false;

        private static bool PopulationChangesBasedOnOverFishingCatchBiggestFishOnly = true;

        private static Random rand = new Random();

        public static ModEntry ModEntryInstance;

        public static void GameEvents_OnUpdateTickTests(object sender, EventArgs e) {
            if (Tests.RunningTests) {
                Tests.PopulationChangesBasedOnOverFishing();  
            }
        }

        private static void PopulationChangesBasedOnOverFishing()
        {

            // Remove a random fish of a weighted random length (leaning towards big fish)
            int numFish = ModEntryInstance.fp.AllFish.Count;
            var randomFish = ModEntryInstance.fp.AllFish[rand.Next(0, numFish)];
            ModEntryInstance.AllFishCaughtToday.Add(randomFish.Item1);

            double previousAvgLength = ModEntryInstance.fp.GetAverageLengthOfFish(randomFish.Item1);
            ModEntryInstance.Monitor.Log("Average Length of : " + randomFish.Item1 + " : " + previousAvgLength);

            List<FishModel> fishOfType;
            ModEntryInstance.population.TryGetValue(randomFish.Item1, out fishOfType);

            // Sort by length, ascending
            fishOfType.Sort();

            int numFishOfType = fishOfType.Count;
            // Selects a weighted average, leaning towards longer fish
            int selectedFish = rand.Next(rand.Next(0, numFishOfType), numFishOfType);

            if (Tests.PopulationChangesBasedOnOverFishingCatchBiggestFishOnly) {
                selectedFish = fishOfType.Count - 1;
            }

            fishOfType.RemoveAt(selectedFish);

            double currentAvgLength = ModEntryInstance.fp.GetAverageLengthOfFish(randomFish.Item1);
            ModEntryInstance.Monitor.Log("Average Length of : " + randomFish.Item1 + " : " + currentAvgLength);

            // Add a fish back that mutates from a random individual
            List<String> changedFish = new List<String>();

            foreach (String fishName in ModEntryInstance.AllFishCaughtToday)
            {
                changedFish.Add(fishName);

                List<FishModel> fishOfType2;
                ModEntryInstance.population.TryGetValue(fishName, out fishOfType2);

                int numFishOfType2 = fishOfType2.Count;
                int selectedFish2 = rand.Next(0, numFishOfType2);

                fishOfType2.Add(fishOfType2[selectedFish2].MakeBaby());

                ModEntryInstance.population[fishName] = fishOfType2;
            }

            ModEntryInstance.AllFishCaughtToday = new List<string>();
        }
    }
}
