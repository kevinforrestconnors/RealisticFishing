using System;
using System.Collections.Generic;

namespace RealisticFishing
{
    public class FishModel
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
    }
}
