using System;
using RealiticFishing.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace FishingMod
{
    /// <summary>The main entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>

        private SBobberBar Bobber;

        private bool BeganFishingGame = false;
        private bool EndedFishingGame = false;
        private bool JustFished = false;

        private int FishingDirection;

        private Item FishCaught;

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
        }

        /*********
        ** Private methods
        *********/

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (e.NewMenu is BobberBar menu)
                this.Bobber = SBobberBar.ConstructFromBaseClass(menu);
        }

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

        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {

        }

        private void PlayerEvents_InventoryChanged(object sender, EventArgsInventoryChanged e) {
            if (this.JustFished) { // Player finished fishing, but may not have caught anything.
                this.FishCaught = e.Added[0].Item;
                this.PromptThrowBackFish();
            }
            this.JustFished = false;
        }

        private void OnFishingBegin() {
            this.Monitor.Log("Fishing has begun.");
            this.FishingDirection = Game1.player.FacingDirection;
        }

        private void OnFishingEnd() {
            this.Monitor.Log("Fishing has ended.");
            this.JustFished = true;
        }

        private void PromptThrowBackFish() {
            Response[] answerChoices = new[]
            {
                    new Response("Yes", "Yes"),
                    new Response("No", "No")
                };

            Game1.currentLocation.createQuestionDialogue("Throw it back?", answerChoices, new GameLocation.afterQuestionBehavior(this.ThrowBackFish));
        }

        private void ThrowBackFish(Farmer who, string whichAnswer) {
            if (whichAnswer == "Yes") {

                Item fish = this.FishCaught.getOne();

                this.FishCaught.Stack--;
                if (this.FishCaught.Stack <= 0)
                {
                    Game1.player.removeItemFromInventory(this.FishCaught);
                }
                this.ThrowFish(fish, who.getStandingPosition(), this.FishingDirection, (GameLocation)null, -1);
            }
        }

        private void ThrowFish(Item fish, Vector2 origin, int direction, GameLocation location, int groundLevel = -1) {
            if (location == null)
                location = Game1.currentLocation;
            Vector2 targetLocation = new Vector2(origin.X, origin.Y);



            switch (direction)
            {
                case -1:
                    targetLocation = Game1.player.getStandingPosition();
                    break;
                case 0:
                    origin.Y -= 192f;
                    targetLocation.Y += 192f;
                    break;
                case 1:
                    origin.X += 192f;
                    targetLocation.X -= 192f;
                    break;
                case 2:
                    origin.Y += 192f;
                    targetLocation.Y -= 192f;
                    break;
                case 3:
                    origin.X -= 192f;
                    targetLocation.X += 192f;
                    break;
            }

            Debris debris = new Debris(-2, 1, origin, targetLocation, 0.1f);
            debris.item = fish;
            if (groundLevel != -1)
                debris.chunkFinalYLevel = groundLevel;
            location.debris.Add(debris);
        }
    }
}