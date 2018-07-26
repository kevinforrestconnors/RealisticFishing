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
using Microsoft.Xna.Framework.Input;

namespace RealisticFishing
{
    public static class Tests
    {
        
        public static bool ShouldRunTests = true;
        public static bool RunningTests = true;

        private static Random rand = new Random();

        public static ModEntry ModEntryInstance;

        public static void GiveFish() {

            string fishName = ModEntryInstance.fp.AllFish[0].Item2;

            // get the list of fish in the Population with that name
            List<FishModel> fishOfType;
            ModEntryInstance.population.TryGetValue(fishName, out fishOfType);

            // get a random fish of that type from the population
            int numFishOfType = fishOfType.Count;
            int selectedFishIndex = ModEntry.rand.Next(0, numFishOfType);
            FishModel selectedFish = fishOfType[selectedFishIndex];

            // store a new custom fish item
            Item customFish = (Item)new FishItem(ModEntryInstance.fp.AllFish[0].Item1, selectedFish);
            FishItem.itemToAdd = customFish as FishItem;
            ((FishItem)customFish).AddToInventory();
            ModEntryInstance.FishCaught = customFish;
        }

        public static void GameEvents_OnUpdateTick(object sender, EventArgs e) 
        {
            if (Tests.RunningTests) {

            }
        }

        public static void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (Tests.RunningTests) {
                if (e.KeyPressed.Equals(Keys.G)) {
                    Tests.GiveFish();
                }
            }
        }
    }
}
