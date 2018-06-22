using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;

namespace RealisticFishing
{
    public class FishItem : StardewValley.Object
    {
        public int MinLength;
        public int MaxLength;
        public double Length;

        public int stackSize;
        public String description;

        public FishItem(int id, String name, int minLength, int maxLength, double length, int quality, int stackSize) 
            : base(id, 1, false, -1, quality)
        {
            this.Name = name;
            this.MinLength = minLength;
            this.MaxLength = maxLength;
            this.Length = length;
            this.Quality = quality;
            this.description = base.getDescription();
            this.stackSize = stackSize;
            this.Category = -4;
        }

        public override string DisplayName { get => this.Name; set => this.Name = value; }
        public override int Stack { get => this.stackSize; set => this.stackSize = value; }


        public override int addToStack(int amount)
        {
            return 1;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {

            if ((bool)(this.isRecipe))
            {
                transparency = 0.5f;
                scaleSize *= 0.75f;
            }
            if ((bool)(this.bigCraftable))
            {
                Rectangle rectForBigCraftable = getSourceRectForBigCraftable((int)(this.parentSheetIndex));
                spriteBatch.Draw(Game1.bigCraftableSpriteSheet, location + new Vector2(32f, 32f), new Microsoft.Xna.Framework.Rectangle?(rectForBigCraftable), color * transparency, 0.0f, new Vector2(8f, 16f), (float)(4.0 * ((double)scaleSize < 0.2 ? (double)scaleSize : (double)scaleSize / 2.0)), SpriteEffects.None, layerDepth);
            }
            else
            {
                if ((int)(this.parentSheetIndex) != 590 & drawShadow)
                    spriteBatch.Draw(Game1.shadowTexture, location + new Vector2(32f, 48f), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), color * 0.5f, 0.0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, layerDepth - 0.0001f);
                spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2((float)(int)(32.0 * (double)scaleSize), (float)(int)(32.0 * (double)scaleSize)), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int)(this.parentSheetIndex), 16, 16)), color * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);
                if (drawStackNumber && this.maximumStackSize() > 1 && ((double)scaleSize > 0.3 && this.Stack != int.MaxValue) && this.Stack > 1)
                    Utility.drawTinyDigits((int)(this.stack), spriteBatch, location + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString((int)(this.stack), 3f * scaleSize)) + 3f * scaleSize, (float)(64.0 - 18.0 * (double)scaleSize + 2.0)), 3f * scaleSize, 1f, color);
                if (drawStackNumber && (int)(this.quality) > 0)
                {
                    float num = (int)(this.quality) < 4 ? 0.0f : (float)((Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                    spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(12f, 52f + num), new Microsoft.Xna.Framework.Rectangle?((int)(this.quality) < 4 ? new Microsoft.Xna.Framework.Rectangle(338 + ((int)(this.quality) - 1) * 8, 400, 8, 8) : new Microsoft.Xna.Framework.Rectangle(346, 392, 8, 8)), color * transparency, 0.0f, new Vector2(4f, 4f), (float)(3.0 * (double)scaleSize * (1.0 + (double)num)), SpriteEffects.None, layerDepth);
                }
                if (this.Category == -22 && this.uses.Value > 0)
                {
                    float power = ((float)(FishingRod.maxTackleUses - this.uses.Value) + 0.0f) / (float)FishingRod.maxTackleUses;
                    spriteBatch.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((int)location.X, (int)((double)location.Y + 56.0 * (double)scaleSize), (int)(64.0 * (double)scaleSize * (double)power), (int)(8.0 * (double)scaleSize)), Utility.getRedToGreenLerpColor(power));
                }
            }
            if (!(bool)(this.isRecipe))
                return;
            spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(16f, 16f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 451, 16, 16)), color, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, layerDepth + 0.0001f);
        }

        public override string getDescription()
        {
            return this.description + " " + ((int)this.Length).ToString() + "in. long.";
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
            return 1;
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
            return false;
        }
    }
}
