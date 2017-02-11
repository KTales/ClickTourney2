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

        public Match(string player1, string player2, int matchNumber)
        {
            this.Player1 = player1;
            this.Player2 = player2;
            this.MatchNumber = matchNumber;
            if (player2 == "Bye")
                Completed = true;
        }

        public Match(int matchNumber, int prevMatch1, int prevMatch2)
        {
            this.MatchNumber = matchNumber;
            this.PreviousMatch1 = prevMatch1;
            this.PreviousMatch2 = prevMatch2;
        }

        public int MatchId { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public bool Completed { get; set; }
        public int MatchNumber { get; set; }
        public int PreviousMatch1 { get; set; }
        public int PreviousMatch2 { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}