using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickTourney.Models
{
    public class Match
    {
        public Match() { }
        public Match(string player1, string player2)
        {
            this.Player1 = player1;
            this.Player2 = player2;
        }

        public Match(int game1, int game2)
        {
            this.PriorMatch1 = game1;
            this.PriorMatch2 = game2;
        }

        public int MatchId { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public int PriorMatch1 { get; set; }
        public int PriorMatch2 { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}