using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

namespace RealisticFishing
{
    public class FishItem : StardewValley.Object
    {
        public int MinLength;
        public int MaxLength;
        public double Length;
        public int DisplayLength;

        public int stackSize;
        public String description;

        public FishItem(int id, String name, int minLength, int maxLength, double length, int quality, int stackSize) 
            : base(id, 1, false, -1, quality)
        {
            this.Name = name;
            this.MinLength = minLength;
            this.MaxLength = maxLength;
            this.Length = length;
            this.DisplayLength = (int)Math.Round(this.Length);
            this.Quality = quality;
            this.description = base.getDescription();
            this.stackSize = stackSize;
            this.Category = -4;

            // Allows the item to be sold
            // TODO: THIS

            // Changes the price to scale off length
            this.Price = (int)(base.Price * (this.DisplayLength / 5));
        }

        public override string DisplayName { get => this.Name; set => this.Name = value; }
        public override int Stack { get => this.stackSize; set => this.stackSize = value; }


        public override int addToStack(int amount)
        {
            return 1;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        public override string getDescription()
        {
            return this.description + " " + this.DisplayLength.ToString() + " in. long.";
        }

        public override Item getOne()
        {
            return (Item)this;
        }

        public override int getStack()
        {
            return this.Stack;
        }

        public override bool isPlaceable()
        {
            return false;
        }

        public override int maximumStackSize()
        {
            return 999;
        }

        public override int salePrice()
        {
            return base.salePrice();
        }

        public override bool canBeTrashed()
        {
            return base.canBeTrashed();
        }

        public override string getCategoryName()
        {
            return base.getCategoryName();
        }

        public override bool canStackWith(Item other)
        {
            return true;
            //return base.canStackWith(other) && this.Name == other.Name;
        }
    }
}
