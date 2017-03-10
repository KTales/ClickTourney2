﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClickTourney.Models
{
    public class Participant
    {
        #region DB Fields
        public int ParticipantId { get; set; }
        public string Alias { get; set; }
        #endregion

        //CTORS
        public Participant()
        {

        }
        public Participant(string alias)
        {
            this.Alias = alias;
        }
    }
}