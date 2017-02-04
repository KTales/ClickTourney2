using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickTourney.Models
{
    public class Match
    {
        public int MatchId { get; set; }

        public string Player1 { get; set; }
        public string Player2 { get; set; }

        public virtual Tournament Tournament { get; set; }
    }
}