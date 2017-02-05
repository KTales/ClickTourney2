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

        public void createMatches()
        {
            switch (TournamentType)
            {
                case "Round Robin":
                    buildRoundRobin();
                    break;
                case "Double Round Robin":
                    buildRoundRobin(true);
                    break;
                case "Elimination":
                    buildElimination();
                    break;
                default:
                    break;
            }

        }

        private void buildElimination()
        {
            // Create player list
            List<string> players = new List<string>();
            for (int i = 1; i <= PlayerCount; i++)
            {
                players.Add("Player " + i);
            }

            // Calculate number of byes in first round
            int powerOfTwo = 2;
            while (powerOfTwo < PlayerCount)
                powerOfTwo *= 2;
            int numByes = powerOfTwo - PlayerCount;

            // Add byes to tourney
            for(int i = 0; i < numByes; ++i)
            {
                players.Add("Bye");
            }

            // Calculate number of required rounds
            int totalPlayers = players.Count;
            double roundCount = Math.Floor(Math.Log(totalPlayers) / Math.Log(2));

            // get two lists by reversing the second half of players
            List<string> aPlayers = new List<string>();
            List<string> bPlayers = new List<string>();
            aPlayers.AddRange(players.Take(totalPlayers / 2));
            bPlayers.AddRange(players.Skip(totalPlayers / 2).Take(totalPlayers/2).Reverse());

            //Create all of the matches
            List<Match> lastRound = new List<Match>();
            int playerIdx = 0;
            for(int round = 1;round <= roundCount; ++round)
            {
                totalPlayers /= 2;
                int playersThisRound = totalPlayers;
                List<Match> currentRound = new List<Match>();

                for (; playersThisRound > 0; --playersThisRound)
                {
                    // If this is the first round, set all of the names
                    if (round == 1)
                    {
                        currentRound.Add(new Match(aPlayers[playerIdx], bPlayers[playerIdx]));
                        playerIdx++;
                    }
                    else
                    {
                        // If not the first round, get the ids of the matches that feed into current from last round
                        // TODO: This does not work because the objects being stored in lasRound do not have ID's until they are saved to db
                        currentRound.Add(new Match(lastRound[currentRound.Count * 2].MatchId, lastRound[currentRound.Count * 2].MatchId + 1));
                    }
                }

                foreach(Match match in currentRound)
                {
                    Matches.Add(match);
                }
                lastRound = currentRound;
            }
        }

        public void buildRoundRobin(bool isDouble = false)
        {
            List<string> players = new List<string>();
            for (int i = 1; i <= PlayerCount; i++)
            {
                players.Add("Player " + i);
            }

            if (players.Count % 2 != 0)
                players.Add("Bye");

            int playersCount = players.Count;

            // Our new way
            List<string> aPlayers = new List<string>();
            List<string> bPlayers = new List<string>();
            aPlayers.AddRange(players.Take(playersCount / 2));
            bPlayers.AddRange(players.Skip(playersCount / 2).Take(playersCount / 2));
            int arrLength = aPlayers.Count;

            int matchesPer = playersCount - 1;

            if (isDouble)
                matchesPer *= 2;

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

        [Range(2, int.MaxValue, ErrorMessage = "Please enter a number between 3 and 20")]
        [Required]
        [DisplayName("# of Participants")]
        public int PlayerCount { get; set; }

        [DisplayName("Type")]
        public string TournamentType { get; set; }
    }
}