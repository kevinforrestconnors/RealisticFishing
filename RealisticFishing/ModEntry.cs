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
using StardewValley.Minigames;

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
        private bool PlayerReceivedFish = false;
        private int whichFish;

        // Which direction the player was facing when they were fishing.  Used in ThrowFish.
        private int FishingDirection;

        // The last fish caught.
        public Item FishCaught;

        // The FishingRod information
        public bool fishWasCaught = false;

        // The list of Tuple<name, uniqueID> of all the fish caught today. 
        public List<Tuple<string, int>> AllFishCaughtToday;

        // How many fish have been caught today.
        public int NumFishCaughtToday;

        // How many fish the player can catch each day.
        public int FishQuota = 10;

        // The class instance of the saved FishPopulation data
        public FishPopulation fp;
        // The population data structure
        public Dictionary<String, List<FishModel>> population;

        public static List<FishItem> FishItemsRecentlyAdded = new List<FishItem>();

        public bool inventoryWasReconstructed = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            MenuEvents.MenuChanged += this.MenuEvents_MenuChanged;
            MenuEvents.MenuClosed += this.MenuEvents_MenuClosed;
            GameEvents.UpdateTick += this.GameEvents_OnUpdateTick;
            ControlEvents.KeyPressed += this.ControlEvents_KeyPressed;
            PlayerEvents.InventoryChanged += this.PlayerEvents_InventoryChanged;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            SaveEvents.AfterCreate += this.SaveEvents_AfterCreate;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            SaveEvents.AfterSave += this.SaveEvents_AfterSave;

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

            this.Monitor.Log("Menu is now: " + e.NewMenu.ToString());

            if (e.NewMenu is BobberBar menu) {
                this.Bobber = SBobberBar.ConstructFromBaseClass(menu);
            } 
        }

        /* GameEvents_OnUpdateTick
        * Triggers every time the menu changes.
        * Detects the treasure menu, allowing custom fish to be generated even if treasure is caught too.
        */
        private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
        }

        /* TimeEvents_AfterDayStarted
         * Triggers at the beginning of each day.
         * Regenerates X fish, where X corresponds in number and type to the 
         * fish that the player caught yesterday.
         */
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e) {

            List<String> changedFish = new List<String>();

            foreach (Tuple<string, int> tuple in this.AllFishCaughtToday) {
                string fishName = tuple.Item1;

                changedFish.Add(fishName);

                List<FishModel> fishOfType;
                this.population.TryGetValue(fishName, out fishOfType);

                int numFishOfType = fishOfType.Count;
                int selectedFish = ModEntry.rand.Next(0, numFishOfType);

                fishOfType.Add(fishOfType[selectedFish].MakeBaby());

                this.population[fishName] = fishOfType;
            }

            foreach (Tuple<String, int, int, int> fish in this.fp.AllFish) {
                if (this.fp.IsAverageFishBelowValue(fish.Item1)) {
                    this.OnFishAtCriticalLevel(fish.Item1);
                }
            }

            this.NumFishCaughtToday = 0;
            this.AllFishCaughtToday = new List<Tuple<string, int>>();

            this.Monitor.Log("TimeEvents_AfterDayStarted: " + this.fp.PrintChangedFish(changedFish));
        }

        /* SaveEvents_AfterCreate
         * Triggers after a save file is updated. 
         * Used to retrieve data and seed the population of fish.
         */
        private void SaveEvents_AfterCreate(object sender, EventArgs e) 
        {
            this.Monitor.Log("SaveEvents_AfterLoad");

            RealisticFishingData instance = this.Helper.ReadJsonFile<RealisticFishingData>($"data/{Constants.SaveFolderName}.json") ?? new RealisticFishingData();
      
            this.NumFishCaughtToday = instance.NumFishCaughtToday;
            this.AllFishCaughtToday = instance.AllFishCaughtToday;
            this.fp = instance.fp;
            this.population = instance.fp.population;
            this.fp.CurrentFishIDCounter = instance.CurrentFishIDCounter;

            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", instance);

            this.Monitor.Log("SaveEvents_AfterCreate: " + instance.fp.PrintChangedFish(new List<String>()));
        }

        /* SaveEvents_AfterLoad
        * Triggers after a save file is loaded.
        * Used to retrieve data and sometimes seed fish population.
        */
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            this.Monitor.Log("SaveEvents_AfterLoad");

            RealisticFishingData instance = this.Helper.ReadJsonFile<RealisticFishingData>($"data/{Constants.SaveFolderName}.json") ?? new RealisticFishingData();

            this.NumFishCaughtToday = instance.NumFishCaughtToday;
            this.AllFishCaughtToday = instance.AllFishCaughtToday;
            this.fp = instance.fp;
            this.population = instance.fp.population;
            this.fp.CurrentFishIDCounter = instance.CurrentFishIDCounter;

            if (!this.inventoryWasReconstructed) {

                this.inventoryWasReconstructed = true;

                foreach (Tuple<int, List<FishModel>> f in instance.inventory)
                {
                    int id = f.Item1;
                    var fishStack = f.Item2;

                    var itemToBeAdded = new FishItem(id, fishStack[0])
                    {
                        Stack = fishStack.Count,
                        FishStack = fishStack
                    };

                    itemToBeAdded.AddToInventory();
                }
            }

            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", instance);

            this.Monitor.Log("SaveEvents_AfterLoad: " + instance.fp.PrintChangedFish(new List<String>()));

            if (Tests.ShouldRunTests) {
                Tests.RunningTests = true;
            }
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
            instance.CurrentFishIDCounter = this.fp.CurrentFishIDCounter;

            instance.inventory.Clear();

            for (int index = 0; index < Game1.player.maxItems; ++index)
            {
                if (index < Game1.player.items.Count && Game1.player.items[index] != null)
                {
                    Item item = Game1.player.items[index];
                    if (item is FishItem) {
                        instance.inventory.Add(new Tuple<int, List<FishModel>>(item.parentSheetIndex, (item as FishItem).FishStack));
                        Game1.player.removeItemFromInventory(item);
                    }
                }
            }

            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", instance);

            this.inventoryWasReconstructed = false;

            this.Monitor.Log("BeforeSave: " + instance.fp.PrintChangedFish(new List<String>()));
        }

        private void SaveEvents_AfterSave(object sender, EventArgs e)
        {
            this.Monitor.Log("SaveEvents_AfterSave");

            RealisticFishingData instance = this.Helper.ReadJsonFile<RealisticFishingData>($"data/{Constants.SaveFolderName}.json") ?? new RealisticFishingData();

            if (!this.inventoryWasReconstructed)
            {

                this.inventoryWasReconstructed = true;

                foreach (Tuple<int, List<FishModel>> f in instance.inventory)
                {
                    int id = f.Item1;
                    var fishStack = f.Item2;

                    var itemToBeAdded = new FishItem(id, fishStack[0])
                    {
                        Stack = fishStack.Count,
                        FishStack = fishStack
                    };

                    itemToBeAdded.AddToInventory();
                }
            }
        }

        /* GameEvents_OnUpdateTick
         * Triggers 60 times per second.  
         * Use one of the methods here https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Events#Game for other time durations
         */
        private void GameEvents_OnUpdateTick(object sender, EventArgs e)
        {

            if (Game1.player.CurrentTool is FishingRod)
            {

                var rod = Game1.player.CurrentTool as FishingRod;

                if (rod.fishCaught && !this.PlayerReceivedFish)
                {

                    // Prevents the mod from giving the player 1 fish per tick ;)
                    this.PlayerReceivedFish = true;

                    this.whichFish = this.Helper.Reflection.GetField<int>(rod, "whichFish").GetValue();

                    // construct a temporary fish item to figure out what the caught fish's name is
                    FishItem tempFish = new FishItem(this.whichFish);

                    if (tempFish.Category == -4) { // is a fish
                        
                        // get the list of fish in the Population with that name
                        List<FishModel> fishOfType;
                        this.population.TryGetValue(tempFish.Name, out fishOfType);

                        // get a random fish of that type from the population
                        int numFishOfType = fishOfType.Count;
                        int selectedFishIndex = ModEntry.rand.Next(0, numFishOfType);
                        FishModel selectedFish = fishOfType[selectedFishIndex];

                        this.Helper.Reflection.GetField<int>(rod, "fishSize").SetValue((int)Math.Round(selectedFish.length));

                        // store a new custom fish item
                        FishItem.lastFishAddedToInventory = selectedFish;
                        Item customFish = (Item)new FishItem(this.whichFish, selectedFish);
                        ((FishItem)customFish).AddToInventory();
                        this.FishCaught = customFish;

                        // make sure the fish in the ocean will be regenerated at the end of the day
                        this.AllFishCaughtToday.Add(new Tuple<string, int>(selectedFish.name, selectedFish.uniqueID));

                        // Prompt the player to throw back the fish
                        this.PromptThrowBackFish(selectedFish.name, selectedFish.uniqueID);  
                    }
                }
            }


            if (Game1.activeClickableMenu is BobberBar && this.Bobber != null) {

                SBobberBar bobber = this.Bobber;

                if (!this.BeganFishingGame) {
                    this.OnFishingBegin();
                    this.PlayerReceivedFish = false;
                    this.BeganFishingGame = true;
                }

            } else if (this.EndedFishingGame) {
                
                this.OnFishingEnd();
                this.EndedFishingGame = false;

            } else if (this.BeganFishingGame) {{}
                    
                this.EndedFishingGame = true;
                this.BeganFishingGame = false;
            }
        }

        /* ControlEvents_KeyPressed
         * Triggers every a key is pressed.
         * Used to play/pause tests.
         */
        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (e.KeyPressed.Equals(Keys.O) && Tests.ShouldRunTests) {
                Tests.RunningTests = !Tests.RunningTests;
            }

        }

        /* PlayerEvents_InventoryChanged
         * Triggers every time the inventory changes.
         * Calls PromptThrowBackFish if the player just gained a fish and also just finished fishing.
         * If the player caught treasure, waits until the player gains the fish and this executes again when the player gains the fish.
         */
        private void PlayerEvents_InventoryChanged(object sender, EventArgsInventoryChanged e) {

            if (e.Added.Count > 0) {

                // Item.Category == -4 tests if item is a fish.
                if (e.Added[0].Item.Category == -4) {

                    if (!(e.Added[0].Item is FishItem)) {
                        Game1.player.removeItemFromInventory(e.Added[0].Item);
                    } else {
                        ModEntry.FishItemsRecentlyAdded.Add(e.Added[0].Item as FishItem);
                    }
                } else {
                    return;
                }

            } else if (e.QuantityChanged.Count > 0) {
                // Item.Category == -4 tests if item is a fish.
                if (e.QuantityChanged[0].Item.Category == -4) {
                    if (!(e.QuantityChanged[0].Item is FishItem))
                    {
                        Game1.player.removeItemFromInventory(e.QuantityChanged[0].Item);
                    } else {

                        if (e.QuantityChanged[0].StackChange > 0) {
                            ModEntry.FishItemsRecentlyAdded.Add(e.QuantityChanged[0].Item as FishItem);
                        } else {

                            int numRemoved = e.QuantityChanged[0].StackChange;
                            var item = (e.QuantityChanged[0].Item as FishItem);

                            if (e.QuantityChanged[0].Item.Stack < 0) {
                                item.FishStack.RemoveRange(item.FishStack.Count - numRemoved - 1, numRemoved);
                            }
                        }
                    }
                } else {
                    return;
                }
            } 
        }

        /* OnFishingEnd
        * Triggers once when the fishing minigame starts.  
        */
        private void OnFishingBegin() {
            this.Monitor.Log("Fishing has begun.");
            this.FishingDirection = Game1.player.FacingDirection;
        }

        /* OnFishingEnd
         * Triggers once after the player catches a fish (not on trash)
         */
        private void OnFishingEnd() {
            this.Monitor.Log("Fishing has ended.");
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
        private void PromptThrowBackFish(string fishName, int fishID) {

            if (this.NumFishCaughtToday >= this.FishQuota) {

                // TODO add delegate
                this.ThrowBackFish(Game1.player, "Yes");

            } else {
                String dialogue = "You have caught " + this.NumFishCaughtToday + " fish today.  You are permitted to catch 10 fish per day.  Throw it back?";

                Response[] answerChoices = new[]
                {
                    new Response("Yes", "Yes"),
                    new Response("No", "No")
                };

                Game1.currentLocation.createQuestionDialogue(dialogue, answerChoices, new GameLocation.afterQuestionBehavior(delegate (Farmer who, string whichAnswer) {
                    if (this.ThrowBackFish(who, whichAnswer)) {
                        
                        List<FishModel> fishOfType;
                        this.population.TryGetValue(fishName, out fishOfType);

                        FishModel selectedFish = fishOfType.Find((FishModel obj) => obj.uniqueID == fishID);
                        fishOfType.Remove(selectedFish);
                    }
                }));
            }
        }

        /* ThrowBackFish
         * Triggers every time the player catches a fish while they are still under the quota.
         * If whichAnswer == "Yes", removes the fish from the inventory and calls ThrowFish
         */
        private bool ThrowBackFish(Farmer who, string whichAnswer) {

            if (whichAnswer == "Yes") {

                Item fish = this.FishCaught.getOne();

                if (this.AllFishCaughtToday.Count > 0) {
                    this.AllFishCaughtToday.RemoveAt(this.AllFishCaughtToday.Count - 1);
                }

                this.FishCaught.Stack--;

                if ((this.FishCaught as FishItem).FishStack.Count > 0) {
                    (this.FishCaught as FishItem).FishStack.RemoveAt((this.FishCaught as FishItem).FishStack.Count - 1);
                }

                if (this.FishCaught.Stack <= 0)
                {
                    Game1.player.removeItemFromInventory(this.FishCaught);
                }
                this.ThrowFish(fish, who.getStandingPosition(), this.FishingDirection, (GameLocation)null, -1);
                return true;

            } else if (whichAnswer == "No") {

                this.NumFishCaughtToday++;

                if (this.NumFishCaughtToday == this.FishQuota) {
                    Game1.addHUDMessage(new HUDMessage("You have reached the fishing limit for today."));
                }
                return false;
            } else {
                return false;
            }
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