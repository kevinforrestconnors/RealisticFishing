using System;
using System.Collections.Generic;

namespace RealisticFishing
{
    public class FishModel : StardewValley.Object, IComparable<FishModel>
    {
        public String FishName;
        public int MinLength;
        public int MaxLength;
        public double Length;

        public FishModel(String name, int minLength, int maxLength, double length)
        {
            this.FishName = name;
            this.MinLength = minLength;
            this.MaxLength = maxLength;
            this.Length = length;
        }

        public int CompareTo(FishModel o)
        {
            return this.Length.CompareTo(o.Length);
        }

        public FishModel MakeBaby() {
            return new FishModel(this.FishName, this.MinLength, this.MaxLength, EvolutionHelpers.GetMutatedFishLength(this.Length, this.MinLength, this.MaxLength));
        }
    }
}
