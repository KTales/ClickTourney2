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
        public virtual Tournament Tournament { get; set; }
        #endregion

        //CTORS
        public Participant()
        {

        }
    }
}