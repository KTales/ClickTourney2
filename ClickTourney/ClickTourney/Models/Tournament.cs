using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClickTourney.Models
{
    public class Tournament
    {
        public Tournament()
        {
            Matches = new List<Match>();
        }

        public void buildRoundRobin()
        {
            List<string> players = new List<string>();
            for (int i = 1; i <= PlayerCount; i++)
            {
                players.Add("Player " + i);
            }

            if (players.Count % 2 != 0)
                players.Add("Bye");

            int playersCount = players.Count;
            int matchesPer = playersCount - 1;

            // Our new way
            List<string> aPlayers = new List<string>();
            List<string> bPlayers = new List<string>();
            aPlayers.AddRange(players.Take(playersCount / 2));
            bPlayers.AddRange(players.Skip(playersCount / 2).Take(playersCount / 2));
            int arrLength = aPlayers.Count;

            for (; matchesPer > 0; matchesPer--)
            {
                for (int game = 0; game < arrLength; game++)
                {
                    Matches.Add(new Match(aPlayers[game], bPlayers[game]));
                }
                bPlayers.Add(aPlayers[arrLength - 1]);
                aPlayers.RemoveAt(arrLength - 1);
                aPlayers.Insert(1, bPlayers[0]);
                bPlayers.RemoveAt(0);
            }
        }

        public int TournamentId { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Match> Matches { get; set; }

        [DisplayName("# of Participants")]
        [Range(2, int.MaxValue, ErrorMessage = "Please enter a number between 3 and 20")]
        [Required]
        public int PlayerCount { get; set; }

        [DisplayName("Type")]
        public string TournamentType { get; set; }
    }
}