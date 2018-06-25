using System;
using RealiticFishing.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using RealisticFishing;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace RealiticFishing
{
    /// <summary>The main entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        public static Random rand = new Random();
         
        public bool ShouldRunTests = false;
        public bool RunningTests = false;

        // Used to detect if the player is fishing.
        private SBobberBar Bobber;

        // Handles the event handler logic.
        private bool BeganFishingGame = false;
        private bool EndedFishingGame = false;
        private bool JustFished = false;

        // Which direction the player was facing when they were fishing.  Used in ThrowFish.
        private int FishingDirection;

        // The last fish caught.
        public Item FishCaught;

        // The list of all fish caught today.
        public List<String> AllFishCaughtToday;

        // How many fish have been caught today.
        public int NumFishCaughtToday;

        // How many fish the player can catch each day.
        public int FishQuota = 10;

        // The class instance of the saved FishPopulation data
        public FishPopulation fp;
        // The population data structure
        public Dictionary<String, List<FishModel>> population;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            MenuEvents.MenuChanged += this.MenuEvents_MenuChanged;
            GameEvents.UpdateTick += this.GameEvents_OnUpdateTick;
            ControlEvents.KeyPressed += this.ControlEvents_KeyPressed;
            PlayerEvents.InventoryChanged += this.PlayerEvents_InventoryChanged;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            SaveEvents.AfterCreate += this.SaveEvents_AfterCreate;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;

            Tests.ModEntryInstance = this;

            if (this.ShouldRunTests)
            {
                GameEvents.EighthUpdateTick += Tests.GameEvents_OnUpdateTickTests;
            }
        }

        /*********
        ** Private methods
        *********/

        /* GameEvents_OnUpdateTick
        * Triggers every time the menu changes.
        * Handles the setup of the SBobberBar, used to detect if the player is fishing.
        */
        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (e.NewMenu is BobberBar menu)
                this.Bobber = SBobberBar.ConstructFromBaseClass(menu);
        }

        /* TimeEvents_AfterDayStarted
         * Triggers at the beginning of each day.
         * Regenerates X fish, where X corresponds in number and type to the 
         * fish that the player caught yesterday.
         */
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e) {

            List<String> changedFish = new List<String>();

            foreach (String fishName in this.AllFishCaughtToday) {
                changedFish.Add(fishName);

                List<FishModel> fishOfType;
                this.population.TryGetValue(fishName, out fishOfType);

                int numFishOfType = fishOfType.Count;
                int selectedFish = ModEntry.rand.Next(0, numFishOfType);

                fishOfType.Add(fishOfType[selectedFish].MakeBaby());

                this.population[fishName] = fishOfType;
            }

            foreach (Tuple<String, int, int> fish in this.fp.AllFish) {
                if (this.fp.IsAverageFishBelowValue(fish.Item1)) {
                    this.OnFishAtCriticalLevel(fish.Item1);
                }
            }

            this.NumFishCaughtToday = 0;
            this.AllFishCaughtToday = new List<String>();

            this.Monitor.Log("TimeEvents_AfterDayStarted: " + this.fp.PrintChangedFish(changedFish));
        }

        /* SaveEvents_AfterCreate
         * Triggers after a save file is updated. 
         * Used to retrieve data and seed the population of fish.
         */
        private void SaveEvents_AfterCreate(object sender, EventArgs e) {
            RealisticFishingData instance = this.Helper.ReadJsonFile<RealisticFishingData>($"data/{Constants.SaveFolderName}.json") ?? new RealisticFishingData();

            this.NumFishCaughtToday = instance.NumFishCaughtToday;
            this.AllFishCaughtToday = instance.AllFishCaughtToday;
            this.fp = instance.fp;
            this.population = instance.fp.population;

            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", instance);

            this.Monitor.Log("SaveEvents_AfterCreate: " + instance.fp.PrintChangedFish(new List<String>()));
        }

        /* SaveEvents_AfterLoad
        * Triggers after a save file is loaded.
        * Used to retrieve data and sometimes seed fish population.
        */
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            RealisticFishingData instance = this.Helper.ReadJsonFile<RealisticFishingData>($"data/{Constants.SaveFolderName}.json") ?? new RealisticFishingData();

            this.NumFishCaughtToday = instance.NumFishCaughtToday;
            this.AllFishCaughtToday = instance.AllFishCaughtToday;
            this.fp = instance.fp;
            this.population = instance.fp.population;

            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", instance);

            this.Monitor.Log("SaveEvents_AfterLoad: " + instance.fp.PrintChangedFish(new List<String>()));

            this.RunningTests = true;
        }

        /* SaveEvents_BeforeSave
        * Triggers before a save file is saved.
        * Used to serialize the data into the save file.
        */
        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            RealisticFishingData instance = this.Helper.ReadJsonFile<RealisticFishingData>($"data/{Constants.SaveFolderName}.json");
            instance.NumFishCaughtToday = this.NumFishCaughtToday;
            instance.AllFishCaughtToday = this.AllFishCaughtToday;
            instance.fp = this.fp;
            instance.population = this.fp.population;
            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", instance);

            this.Monitor.Log("BeforeSave: " + instance.fp.PrintChangedFish(new List<String>()));
        }

        /* GameEvents_OnUpdateTick
         * Triggers 60 times per second.  
         * Use one of the methods here https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Events#Game for other time durations
         */
        private void GameEvents_OnUpdateTick(object sender, EventArgs e)
        {

            if (Game1.activeClickableMenu is BobberBar && this.Bobber != null) {

                SBobberBar bobber = this.Bobber;

                if (!this.BeganFishingGame) {
                    this.OnFishingBegin();
                    this.BeganFishingGame = true;
                }

            } else if (this.EndedFishingGame) {
                
                this.OnFishingEnd();
                this.EndedFishingGame = false;

            } else {
                
                if (this.BeganFishingGame) {
                    
                    this.EndedFishingGame = true;
                    this.BeganFishingGame = false;
                }
            }
        }

        /* ControlEvents_KeyPressed
         * Triggers every a key is pressed.
         * Used to play/pause tests.
         */
        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (e.KeyPressed.Equals(Keys.O) && this.ShouldRunTests) {
                this.RunningTests = !this.RunningTests;
            }

        }

        /* PlayerEvents_InventoryChanged
         * Triggers every time the inventory changes.
         * Calls PromptThrowBackFish if the player just gained a fish and also just finished fishing.
         * If the player caught treasure, waits until the player gains the fish and this executes again when the player gains the fish.
         */
        private void PlayerEvents_InventoryChanged(object sender, EventArgsInventoryChanged e) {
            if (this.JustFished) { // Player finished fishing, but may not have caught anything.

                this.JustFished = false;

                if ((Game1.player.CurrentTool as FishingRod).treasureCaught) {
                    this.JustFished = true;
                    return;
                } else if (e.Added.Count > 0) {
                    this.FishCaught = e.Added[0].Item;
                    this.OnFishCaught(this.FishCaught);
                } else if (e.QuantityChanged.Count > 0) {
                    this.FishCaught = e.QuantityChanged[0].Item;
                    this.OnFishCaught(this.FishCaught);
                } else {
                    return;
                }

                this.PromptThrowBackFish();
            }
        }

        /* OnFishingEnd
        * Triggers once when the fishing minigame starts.  
        * Put function calls here, not iterative style code.
        */
        private void OnFishingBegin() {
            this.Monitor.Log("Fishing has begun.");
            this.FishingDirection = Game1.player.FacingDirection;
        }

        /* OnFishingEnd
         * Triggers once after the player catches a fish (not on trash)
         * Put function calls here, not iterative style code.
         */
        private void OnFishingEnd() {
            this.Monitor.Log("Fishing has ended.");
            this.JustFished = true;
        }

        /* OnFishCaught
         * Triggers once after the player actually receives the fish in their inventory.
         * Put function calls here, not iterative style code.
         */
        private void OnFishCaught(Item fish) {
            this.AllFishCaughtToday.Add(fish.Name);
        }

        /* OnFishAtCriticalLevel
         * Triggers when a population of fish with name <fishName> has average length 
         *   1 standard deviation below the mean
         */
        private void OnFishAtCriticalLevel(String fishName) {
            Monitor.Log("The average size of " + fishName + " has fallen to critical levels.");
        }


        /* PromptThrowBackFish
         * Triggers every time the player catches a fish while they are still under the quota.
         * Calls ThrowBackFish as a callback to handle the choice made.
         */
        private void PromptThrowBackFish() {

            if (this.NumFishCaughtToday >= this.FishQuota) {

                this.ThrowBackFish(Game1.player, "Yes");

            } else {
                String dialogue = "You have caught " + this.NumFishCaughtToday + " fish today.  You are permitted to catch 10 fish per day.  Throw it back?";

                Response[] answerChoices = new[]
                {
                    new Response("Yes", "Yes"),
                    new Response("No", "No")
                };

                Game1.currentLocation.createQuestionDialogue(dialogue, answerChoices, new GameLocation.afterQuestionBehavior(this.ThrowBackFish));
            }
        }

        /* ThrowBackFish
         * Triggers every time the player catches a fish while they are still under the quota.
         * If whichAnswer == "Yes", removes the fish from the inventory and calls ThrowFish
         */
        private void ThrowBackFish(Farmer who, string whichAnswer) {

            Item fish = this.FishCaught.getOne();

            if (whichAnswer == "Yes") {

                this.FishCaught.Stack--;

                if (this.FishCaught.Stack <= 0)
                {
                    Game1.player.removeItemFromInventory(this.FishCaught);
                }
                this.ThrowFish(fish, who.getStandingPosition(), this.FishingDirection, (GameLocation)null, -1);

            } else if (whichAnswer == "No") {

                this.NumFishCaughtToday++;
                this.RemoveFishFromOcean(fish);

                if (this.NumFishCaughtToday == this.FishQuota) {
                    this.Monitor.Log("You have reached the quota");
                }

            }
        }

        /* RemoveFishFromOcean
         * Removes a fish with name <fishName> from the fish population - <this.fp>
         */
        private void RemoveFishFromOcean(String fishName)
        {

            // Prints the number of fish of this type before removing it
            List<String> changedFish = new List<String>();
            changedFish.Add(fishName);
            this.Monitor.Log("RemoveFishFromOcean: " + this.fp.PrintChangedFish(changedFish));

            List<FishModel> fishOfType;
            this.population.TryGetValue(fishName, out fishOfType);

            int numFishOfType = fishOfType.Count;
            int selectedFish = ModEntry.rand.Next(0, numFishOfType);

            fishOfType.RemoveAt(selectedFish);

            this.Monitor.Log("RemoveFishFromOcean: " + this.fp.PrintChangedFish(changedFish));
        }

        private void RemoveFishFromOcean(Item fish)
        {
            RemoveFishFromOcean(fish.Name);
        }

        /* ThrowFish
         * Drops the fish that was just caught 192 pixels in front of the player, almost always into the water.
         * TODO: Make it so that the item always lands in the water, or that the item is destroyed if it doesn't.
         */
        private void ThrowFish(Item fish, Vector2 origin, int direction, GameLocation location, int groundLevel = -1) {
            if (location == null)
                location = Game1.currentLocation;
            Vector2 targetLocation = new Vector2(origin.X, origin.Y);

            switch (direction)
            {
                case -1:
                    targetLocation = Game1.player.getStandingPosition();
                    break;
                case 0: // up
                    origin.Y -= 192f;
                    targetLocation.Y += 192f;
                    break;
                case 1: // right
                    origin.X += 192f;
                    targetLocation.X -= 192f;
                    break;
                case 2: // down
                    origin.Y += 192f;
                    targetLocation.Y -= 192f;
                    break;
                case 3: // left
                    origin.X -= 192f;
                    targetLocation.X += 192f;
                    break;
            }

            Debris debris = new Debris(-2, 1, origin, targetLocation, 0.1f); // (int debrisType, int numberOfChunks, Vector2 debrisOrigin, Vector2 playerPosition, float velocityMultiplyer)
            debris.item = fish;
            if (groundLevel != -1)
                debris.chunkFinalYLevel = groundLevel;
            location.debris.Add(debris);
        }
    }
}