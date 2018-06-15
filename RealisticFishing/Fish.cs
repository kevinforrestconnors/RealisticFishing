using System;
using System.Collections.Generic;

namespace RealisticFishing
{
    public class Fish
    {
        public String name;
        public double Length;

        public static List<String> AllFish;

        public Fish(String name, double length)
        {
            this.name = name;
            this.Length = length;

            Fish.AllFish = new List<String>();
            Fish.AllFish.Add("Herring");
            Fish.AllFish.Add("Tuna");
            Fish.AllFish.Add("Mackerel");
        }
    }
}
