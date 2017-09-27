using System;
using System.Collections.Generic;

namespace Fanex.BetList.Accounting.Models
{
    public class Event
    {
        public Event()
        {
            MatchList = new List<Match>();
        }

        public DateTime WinLossDate { get; set; }

        public List<Match> MatchList { get; set; }

        public decimal Odds { get; set; }

        public decimal Stake { get; set; }

        public decimal WinLoss { get; set; }

        public string Status { get; set; }
    }
}
