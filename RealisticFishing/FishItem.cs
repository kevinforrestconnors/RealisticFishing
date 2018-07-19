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

        // set in ModEntry.cs
        public static FishModel lastFishAddedToInventory;

        // this is set directly before addToStack is called in addToInventory
        public static FishItem itemToAdd;

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

            if (FishItem.lastFishAddedToInventory != null) {
                this.FishStack.Add(FishItem.lastFishAddedToInventory);
            }
        }

        public Item AddToInventory()
        {
            Item item = this;

            if (item == null)
                return (Item)null;
            if (item is SpecialItem)
                return item;
            for (int index = 0; index < (int)(Game1.player.maxItems); ++index)
            {
                if (index < Game1.player.items.Count && Game1.player.items[index] != null && (Game1.player.items[index].maximumStackSize() != -1 && Game1.player.items[index].getStack() < Game1.player.items[index].maximumStackSize()) && Game1.player.items[index].Name.Equals(item.Name) && ((!(item is StardewValley.Object) || !(Game1.player.items[index] is StardewValley.Object) || (item as StardewValley.Object).quality.Value == (Game1.player.items[index] as StardewValley.Object).quality.Value && (item as StardewValley.Object).parentSheetIndex.Value == (Game1.player.items[index] as StardewValley.Object).parentSheetIndex.Value) && item.canStackWith(Game1.player.items[index])))
                {
                    FishItem.itemToAdd = (FishItem)item;
                    int stack = (Game1.player.items[index] as FishItem).addToStack(1);
                    if (stack <= 0)
                        return (Item)null;
                    item.Stack = stack;
                }
            }
            for (int index = 0; index < (int)(Game1.player.maxItems); ++index)
            {
                
                if (Game1.player.items.Count > index && Game1.player.items[index] == null)
                {
                    Game1.player.items[index] = item;
                    return (Item)null;
                }
            }
            return item;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        public override Item getOne() {
            
            if (this.FishStack.Count > 0) {
                FishModel topFish = this.FishStack[this.FishStack.Count - 1];
                FishItem one = new FishItem(this.Id, topFish);
                one.Price = (int)Math.Round((base.Price * (topFish.length / 5) * topFish.quality));
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

        public override int addToStack(int quantity) {
            this.FishStack.AddRange(FishItem.itemToAdd.FishStack);
            return base.addToStack(quantity);
        }

        public override bool canStackWith(Item other)
        {
            return base.canStackWith(other) && other is FishItem;
        }
    }
}
