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
        public const int saleModifier = 8;

        public int Id;
        public List<FishModel> FishStack = new List<FishModel>();

        public Boolean recoveredFromInventory = false;

        // Used to set a FishStack of a FishItem in a chest when the FishItem was removed from the inventory
        // and when a partial stack is added to a stack in a chest
        // this technique is used because there is no event handler for "AddedToChest"
        public static FishItem itemToAdd;

        // Set in FishItem.getOne().  Used to set the correct FishStack of an item that was partially removed
        // from the inventory and put into a chest AND to set the correct FishStack of the newly added 
        // FishItem in an inventory that had one stack worth of an item removed from a chest
        public static FishItem itemInChestToFix;

        // The FishItem in a chest that has to be updated when FishItem.addToStack() is called
        public static FishItem itemToChange;

        // Used for all other cases where the inventory interacts with a chest
        public static FishItem itemInChestToUpdate;

        public String Description;

        public FishItem(int id) 
            : base(id, 1, false, -1, 1) 
        {

            this.Name += " ";
            this.Id = id;
            this.Description = base.getDescription();
            this.FishStack.Add(new FishModel(-1, this.Name, -1, -1, 0, 1));
        }

        public FishItem(int id, FishModel fish) 
            : base(id, 1, false, -1, fish.quality)
        {

            this.Name += " ";
            this.Id = id;
            this.Description = base.getDescription();

            this.FishStack.Add(fish);
        }

        public override int maximumStackSize()
        {
            return 999;
        }

        public void AddToInventory()
        {

            Item item = this;
            FishItem fishItem = item as FishItem;

            for (int index = 0; index < (int)(Game1.player.maxItems); ++index)
            {
                // Adding to a non-empty inventory slot
                if (index < Game1.player.items.Count && Game1.player.items[index] != null && (Game1.player.items[index].maximumStackSize() != -1 && Game1.player.items[index].getStack() < Game1.player.items[index].maximumStackSize()) && Game1.player.items[index].Name.Equals(item.Name) && ((!(item is StardewValley.Object) || !(Game1.player.items[index] is StardewValley.Object) || (item as StardewValley.Object).quality.Value == (Game1.player.items[index] as StardewValley.Object).quality.Value && (item as StardewValley.Object).ParentSheetIndex == (Game1.player.items[index] as StardewValley.Object).ParentSheetIndex) && item.canStackWith(Game1.player.items[index])))
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

            if (count >= max) {
                return this.Description + "This stack contains " + this.Name + "of length: \n" + lengths + "\n...(truncated)";
            } else {
                return this.Description + "This stack contains " + this.Name + "of length: \n" + lengths;   
            }
        }

        public override int salePrice() {

            double p = 0;

            foreach (FishModel fish in this.FishStack)
            {
                p += base.Price * (Math.Round(fish.length) / FishItem.saleModifier) * (fish.quality + 1);
            }

            p /= this.FishStack.Count;

            return (int)Math.Round(p);
        }

        public override int sellToStorePrice()
        {
            double p = 0;

            foreach (FishModel fish in this.FishStack)
            {
                p += base.Price * (Math.Round(fish.length) / FishItem.saleModifier) * (fish.quality + 1);
            }

            p /= this.FishStack.Count;



            return (int)Math.Round(p);
        }

        public override int addToStack(int amount) {

            FishItem.itemToChange = this;

            return base.addToStack(amount);
        }

        public override bool canStackWith(Item other)
        {
            return base.canStackWith(other) && other is FishItem;
        }

        public void checkIfStackIsWrong() {
            
            if (this.FishStack.Count > this.Stack)
            {
                this.FishStack.RemoveRange(this.Stack, this.FishStack.Count - this.Stack);
            }
        }
    }
}
