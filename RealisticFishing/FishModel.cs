using System;
using System.Collections.Generic;

namespace RealisticFishing
{
    public class FishModel
    {
        public String name;
        public double Length;

        public FishModel(String name, double length)
        {
            this.name = name;
            this.Length = length;
        }
    }
}
