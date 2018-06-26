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
        public int Id;
        public int MinLength;
        public int MaxLength;
        public double Length;
        public int DisplayLength;

        public String Description;

        public FishItem(int id, int minLength, int maxLength, double length, int quality) 
            : base(id, 1, false, -1, quality)
        {
            this.Id = id;
            this.MinLength = minLength;
            this.MaxLength = maxLength;
            this.Length = length;
            this.DisplayLength = (int)Math.Round(this.Length);
            this.Quality = quality;
            this.Description = base.getDescription();
            this.Category = -4;

            // Allows the item to be sold
            // TODO: THIS

            // Changes the price to scale off length
            this.Price = (int)(base.Price * (this.DisplayLength / 5));
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        public override Item getOne() {
            return (Item)new FishItem(this.Id, this.MinLength, this.MaxLength, this.Length, this.Quality);
        }

        public override string getDescription()
        {
            return this.Description + " " + this.DisplayLength.ToString() + " in. long.";
        }

        public override int salePrice()
        {
            return base.salePrice();
        }

        public override bool canStackWith(Item other)
        {
            return base.canStackWith(other);
        }
    }
}
