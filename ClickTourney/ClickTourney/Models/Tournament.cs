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
        #region DB Fields
        public int TournamentId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<Match> Matches { get; set; }
        [Range(2, 20, ErrorMessage = "Please enter a number between 3 and 20")]
        [Required]
        [DisplayName("# of Participants")]
        public int PlayerCount { get; set; }
        [DisplayName("Type")]
        public string TournamentType { get; set; }
        public bool IsVisible;
        #endregion

        public Tournament()
        {
            Matches = new List<Match>();
        }

        public void createMatches(List<string> pNames = null)
        {
            switch (TournamentType)
            {
                case "Round Robin":
                    buildRoundRobin(pNames);
                    break;
                case "Double Round Robin":
                    buildRoundRobin(pNames, true);
                    break;
                case "Elimination":
                    buildElimination(pNames);
                    break;
                default:
                    break;
            }

        }

        private void buildElimination(List<string> pNames = null)
        {
            cleanPNames(ref pNames);

            // Create player list
            List<Participant> players = new List<Participant>();
            for (int i = 1; i <= PlayerCount; i++)
            {
                players.Add(new Participant(pNames[i-1]));
            }

            // Calculate number of byes in first round
            int powerOfTwo = 2;
            while (powerOfTwo < PlayerCount)
                powerOfTwo *= 2;
            int numByes = powerOfTwo - PlayerCount;

            // Add byes to tourney
            for (int i = 0; i < numByes; ++i)
            {
                players.Add(new Participant("Bye"));
            }

            // Calculate number of required rounds
            int totalPlayers = players.Count;
            double roundCount = Math.Floor(Math.Log(totalPlayers) / Math.Log(2));

            // get two lists by reversing the second half of players
            List<Participant> aPlayers = new List<Participant>();
            List<Participant> bPlayers = new List<Participant>();
            aPlayers.AddRange(players.Take(totalPlayers / 2));
            bPlayers.AddRange(players.Skip(totalPlayers / 2).Take(totalPlayers / 2).Reverse());

            //Create all of the matches
            List<int> lastRoundMatches = new List<int>();
            int matchCount = 1;
            int playerIdx = 0;
            for (int round = 1; round <= roundCount; ++round)
            {
                totalPlayers /= 2;
                int matchesThisRound = totalPlayers;
                List<Match> currentRound = new List<Match>();

                for (int i = 0; i < matchesThisRound; ++i)
                {
                    // If this is the first round, set all of the names
                    if (round == 1)
                    {
                        currentRound.Add(new Match(aPlayers[playerIdx], bPlayers[playerIdx], matchCount));
                        playerIdx++;
                    }
                    else
                    {
                        // If not the first round, get the ids of the matches that feed into current from last round
                        currentRound.Add(new Match(matchCount, lastRoundMatches[currentRound.Count * 2], lastRoundMatches[currentRound.Count * 2 + 1]));
                    }
                    matchCount++;
                }
                lastRoundMatches.Clear();
                foreach (Match match in currentRound)
                {
                    Matches.Add(match);
                    lastRoundMatches.Add(match.MatchNumber);
                }
            }
        }

        private void buildRoundRobin(List<string> pNames = null, bool isDouble = false)
        {
            cleanPNames(ref pNames);
            List<Participant> players = new List<Participant>();
            for (int i = 1; i <= PlayerCount; i++)
            {
                players.Add(new Participant(pNames[i-1]));
            }

            if (players.Count % 2 != 0)
                players.Add(new Participant("Bye"));

            int playersCount = players.Count;

            // Our new way
            List<Participant> aPlayers = new List<Participant>();
            List<Participant> bPlayers = new List<Participant>();
            aPlayers.AddRange(players.Take(playersCount / 2));
            bPlayers.AddRange(players.Skip(playersCount / 2).Take(playersCount / 2));
            int arrLength = aPlayers.Count;

            int matchesPer = playersCount - 1;

            if (isDouble)
                matchesPer *= 2;

            int matchCount = 1;

            for (; matchesPer > 0; matchesPer--)
            {
                for (int game = 0; game < arrLength; game++)
                {
                    Matches.Add(new Match(aPlayers[game], bPlayers[game], matchCount));
                    matchCount++;
                }
                bPlayers.Add(aPlayers[arrLength - 1]);
                aPlayers.RemoveAt(arrLength - 1);
                aPlayers.Insert(1, bPlayers[0]);
                bPlayers.RemoveAt(0);
            }
        }

        private void cleanPNames(ref List<string> pNames)
        {
            if (pNames != null && pNames.Count < PlayerCount)
            {
                for (int i = pNames.Count; i < PlayerCount; ++i)
                {
                    pNames.Add("Player " + (i + 1));
                }
            }
        }
    }
}