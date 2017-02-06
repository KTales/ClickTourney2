using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
            if (player2 == "Bye")
                Completed = true;
        }

        public Match(Match game1, Match game2)
        {
            this.PriorMatch1 = game1;
            this.PriorMatch2 = game2;
        }

        public int MatchId { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public bool Completed { get; set; }
        public virtual Match PriorMatch1 { get; set; }
        public virtual Match PriorMatch2 { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}