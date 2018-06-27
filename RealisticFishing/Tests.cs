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

        private static Random rand = new Random();

        public static ModEntry ModEntryInstance;

        public static void GameEvents_OnUpdateTickTests(object sender, EventArgs e) {
            if (Tests.RunningTests) {

            }
        }
    }
}
