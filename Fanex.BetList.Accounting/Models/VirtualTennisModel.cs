using System;

namespace Fanex.BetList.Accounting.Models
{
    public class VirtualTennisModel
    {
        public DateTime KickOffTime { get; set; }

        public string HomeName { get; set; }

        public string AwayName { get; set; }

        public int HomePoints { get; set; }

        public int AwayPoints { get; set; }

        public string EventStatus { get; set; }

        public string LeageName { get; set; }
    }
}
