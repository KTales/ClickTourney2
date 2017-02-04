using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ClickTourney.Models
{
    public class Tournament
    {
        public Tournament()
        {
            this.createMatches();
        }

        private void createMatches()
        {
            throw new NotImplementedException();
        }

        public int TournamentId { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Match> Matches { get; set; }

        [DisplayName("# of Participants")]
        public int PlayerCount { get; set; }

        [DisplayName("Type")]
        public string TournamentType { get; set; }
    }
}