using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuisBot.Models
{
    public class ItemData
    {
        public string Data { get; set; }
        public int RatingCount { get; set; }
        public int TotalRatings { get; set; }
        public double Average {
            get {
                if (RatingCount > 0)
                    return ((double) TotalRatings) / RatingCount;
                return 0;
            } }
    }
}