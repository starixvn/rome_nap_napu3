using System;

namespace Fanex.BetList.Accounting.Models
{
    public class VirtualHRPopupModel
    {
        public DateTime EventTime { get; set; }

        public int Place { get; set; }

        public int HorseNumber { get; set; }

        public string HorseName { get; set; }

        public decimal WinOdds { get; set; }

        public decimal PlaceOdds { get; set; }

        public string Status { get; set; }
    }
}
