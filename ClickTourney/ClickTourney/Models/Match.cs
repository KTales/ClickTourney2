using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ClickTourney.Models
{
    public class Match
    {
        #region DB Fields
        public int MatchId { get; set; }
        public virtual Participant Player1 { get; set; }
        public virtual Participant Player2 { get; set; }
        public bool Completed { get; set; }
        public virtual Participant Winner { get; set; }
        public int MatchNumber { get; set; }
        public int PreviousMatch1 { get; set; }
        public int PreviousMatch2 { get; set; }
        public virtual Tournament Tournament { get; set; }
        #endregion

        public Match() { }
        public Match(Participant player1, Participant player2)
        {
            this.Player1 = player1;
            this.Player2 = player2;

            if (player2.Alias == "Bye")
            {
                Winner = Player1;
                Completed = true;
            }
        }
        public Match(Participant player1, Participant player2, int matchNumber)
        {
            this.Player1 = player1;
            this.Player2 = player2;
            this.MatchNumber = matchNumber;

            if (player2.Alias == "Bye")
            {
                Winner = Player1;
                Completed = true;
            }
            else if(player1.Alias == "Bye")
            {
                Winner = Player1;
                Completed = true;
            }
        }

        internal void updateElimTourney()
        {
            var nextMatch = Tournament.Matches.FirstOrDefault(m => m.PreviousMatch1 == MatchNumber || m.PreviousMatch2 == MatchNumber);
            if (nextMatch != null)
            {
                if (nextMatch.Player1 == null || nextMatch.Player1 == Player1 || nextMatch.Player1 == Player2)
                {
                    nextMatch.Player1 = Winner;
                }
                else
                {
                    nextMatch.Player2 = Winner;
                }
            }
        }

        public Match(int matchNumber, int prevMatch1, int prevMatch2)
        {
            this.MatchNumber = matchNumber;
            this.PreviousMatch1 = prevMatch1;
            this.PreviousMatch2 = prevMatch2;
        }
    }
}