using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RealiticFishing;
using StardewValley;
using StardewValley.Objects;


namespace RealisticFishing
{

    public class FishItem : StardewValley.Object
    {
        public int Id;
        public List<FishModel> FishStack = new List<FishModel>();

        public Boolean recoveredFromInventory = false;

        public static FishItem itemToAdd;
        public static FishItem itemInChestToFix;
        public static FishItem itemInChestToUpdate;
        public static FishItem itemToChange;

        public String Description;

        public FishItem(int id) 
            : base(id, 1, false, -1, 1) 
        {

            Tests.ModEntryInstance.Monitor.Log("\nFishItem(id) called.");

            this.Name += " ";
            this.Id = id;
            this.Description = base.getDescription();
            this.FishStack.Add(new FishModel(-1, this.Name, -1, -1, 0, 1));
        }

        public FishItem(int id, FishModel fish) 
            : base(id, 1, false, -1, fish.quality)
        {
            Tests.ModEntryInstance.Monitor.Log("\nFishItem(id, fish) called.");

            this.Name += " ";
            this.Id = id;
            this.Description = base.getDescription();

            this.FishStack.Add(fish);
        }

        public void AddToInventory()
        {
            Tests.ModEntryInstance.Monitor.Log("\nAddToInventory");

            Item item = this;
            FishItem fishItem = item as FishItem;

            for (int index = 0; index < (int)(Game1.player.maxItems); ++index)
            {
                // Adding to a non-empty inventory slot
                if (index < Game1.player.items.Count && Game1.player.items[index] != null && (Game1.player.items[index].maximumStackSize() != -1 && Game1.player.items[index].getStack() < Game1.player.items[index].maximumStackSize()) && Game1.player.items[index].Name.Equals(item.Name) && ((!(item is StardewValley.Object) || !(Game1.player.items[index] is StardewValley.Object) || (item as StardewValley.Object).quality.Value == (Game1.player.items[index] as StardewValley.Object).quality.Value && (item as StardewValley.Object).parentSheetIndex.Value == (Game1.player.items[index] as StardewValley.Object).parentSheetIndex.Value) && item.canStackWith(Game1.player.items[index])))
                {
                    (Game1.player.items[index] as FishItem).Stack += fishItem.Stack;
                    (Game1.player.items[index] as FishItem).FishStack.AddRange(fishItem.FishStack);
                    return;
                }
            }
            for (int index = 0; index < (int)(Game1.player.maxItems); ++index)
            {
                // Adding to an empty inventory slot
                if (Game1.player.items.Count > index && Game1.player.items[index] == null)
                {
                    Game1.player.items[index] = item;
                    return;
                }
            }
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        public override Item getOne() {

            Tests.ModEntryInstance.Monitor.Log("\nGetOne called");

            this.checkIfStackIsWrong();

            if (this.FishStack.Count > 0) {
                
                FishItem one = new FishItem(this.Id, this.FishStack[this.FishStack.Count - 1]);
                FishItem.itemInChestToFix = one;
                FishItem.itemInChestToUpdate = this;

                return (Item)one;
            } else {
                Tests.ModEntryInstance.Monitor.Log("Something went wrong!");
                throw new MissingMemberException();
            }
        }

        public override string getDescription()
        {

            if (this.FishStack.Count == 1) {
                return this.Description + " This one is " + ((int)Math.Round(this.FishStack[0].length)).ToString() + " in. long.";
            }

            string lengths = "";

            int count = 0;
            int max = 10;

            this.FishStack.Reverse();

            foreach (FishModel fish in this.FishStack) {

                if (count == 0) {
                    lengths += ((int)Math.Round(fish.length)).ToString();
                } else {
                    lengths += ", " + ((int)Math.Round(fish.length)).ToString();
                }

                count++;

                if (count == max) {
                    break;
                }
            }

            this.FishStack.Reverse();

            if (count == max) {
                return this.Description + "This stack contains " + this.Name + "of length: \n" + lengths + "\n...(truncated)";
            } else {
                return this.Description + "This stack contains " + this.Name + "of length: \n" + lengths;   
            }
        }

        public override int sellToStorePrice()
        {
            double p = 0;

            foreach (FishModel fish in this.FishStack)
            {
                p += base.Price * (fish.length / 5) * fish.quality;
            }

            p /= this.FishStack.Count;

            return (int)Math.Round(p);
        }

        public override int addToStack(int amount) {

            Tests.ModEntryInstance.Monitor.Log("\naddToStack called");

            FishItem.itemToChange = this;

            return base.addToStack(amount);
        }

        public override bool canStackWith(Item other)
        {
            return base.canStackWith(other) && other is FishItem;
        }

        public void checkIfStackIsWrong() {

            Tests.ModEntryInstance.Monitor.Log("checkIfStackIsWrong called");
            
            // Needs testing: possibly necessary to handle removing stacks from chests
            if (this.FishStack.Count > this.Stack)
            {
                Tests.ModEntryInstance.Monitor.Log("Removing some items from FishStack");
                Tests.ModEntryInstance.Monitor.Log("Removing items in range " + this.Stack.ToString() + ", " + (this.FishStack.Count - this.Stack).ToString());
                this.FishStack.RemoveRange(this.Stack, this.FishStack.Count - this.Stack);
            }

            //// Necessary to handle removing stacks from chests
            //if (this.Stack > this.FishStack.Count)
            //{
            //    Tests.ModEntryInstance.Monitor.Log("Altering stack size");
            //    this.FishStack = FishItem.itemToAdd.FishStack;
            //    this.Stack = this.FishStack.Count;
            //}
        }
    }
}
