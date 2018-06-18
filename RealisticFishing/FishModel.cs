using System;
using System.Collections.Generic;

namespace RealisticFishing
{
    public class FishModel : IComparable<FishModel>
    {
        public String Name;
        public int MinLength;
        public int MaxLength;
        public double Length;

        public FishModel(String name, int minLength, int maxLength, double length)
        {
            this.Name = name;
            this.MinLength = minLength;
            this.MaxLength = maxLength;
            this.Length = length;
        }

        public int CompareTo(FishModel o)
        {
            return this.Length.CompareTo(o.Length);
        }

        public FishModel MakeBaby() {
            return new FishModel(this.Name, this.MinLength, this.MaxLength, EvolutionHelpers.GetMutatedFishLength(this.Length, this.MinLength, this.MaxLength));
        }
    }
}
