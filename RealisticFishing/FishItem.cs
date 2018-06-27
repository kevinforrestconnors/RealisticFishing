using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RealiticFishing;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

namespace RealisticFishing
{
    public class FishItem : StardewValley.Object
    {
        public int Id;
        public List<FishModel> FishStack = new List<FishModel>();

        public static FishModel lastFishAddedToInventory;

        public String Description;

        public FishItem(int id) : base(id, 0, false, -1, 0) {}

        public FishItem(int id, FishModel fish) 
            : base(id, 0, false, -1, fish.quality)
        {
            this.Id = id;
            this.Description = base.getDescription();
            this.FishStack.Add(FishItem.lastFishAddedToInventory);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        public override Item getOne() {
            FishItem one = new FishItem(this.Id, this.FishStack[this.FishStack.Count - 1]);
            one.Price = (int)Math.Round((base.Price * (this.FishStack[0].length / 5)));
            return (Item)one;
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
                lengths += ((int)Math.Round(fish.length)).ToString() + ", ";
                count++;

                if (count == max) {
                    break;
                }
            }

            this.FishStack.Reverse();

            if (count == max) {
                return this.Description + "This stack contains " + this.Name + " of length: \n" + lengths + "\n...(truncated)";
            } else {
                return this.Description + "This stack contains " + this.Name + " of length: \n" + lengths;   
            }
        }

        public override int salePrice()
        {
            double p = 0;

            foreach (FishModel fish in this.FishStack)
            {
                p += base.Price * (fish.length / 5);
            }

            p /= this.FishStack.Count;

            return (int)Math.Round(p);
        }

        public override int addToStack(int amount) {
            int stackSize = base.addToStack(amount);

            if (stackSize != this.FishStack.Count) {
                foreach (FishItem fishItem in ModEntry.FishItemsRecentlyAdded) {
                    if (this.Name == fishItem.Name) {
                        this.FishStack.AddRange(fishItem.FishStack);
                    }
                }

                ModEntry.FishItemsRecentlyAdded.Clear();
            }

            return stackSize;
        }

        public override bool canStackWith(Item other)
        {
            return base.canStackWith(other);
        }
    }
}
