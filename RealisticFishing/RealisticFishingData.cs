using System;
namespace RealisticFishing
{
    public class RealisticFishingData
    {

        public int NumFishCaughtToday { get; set; }

        public RealisticFishingData() {
            
        }

        public RealisticFishingData(int numFishCaughtToday)
        {
            this.NumFishCaughtToday = numFishCaughtToday;
        }
    }
}
