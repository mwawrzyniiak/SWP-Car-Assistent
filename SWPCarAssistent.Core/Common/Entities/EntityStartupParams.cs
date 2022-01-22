﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWPCarAssistent.Core.Common.Entities
{

    public class EntityStartupParams
    {
        public int Id { get; set; }

        public bool Lights { get; set; }

        public bool Wipers { get; set; }

        public bool CarWindows { get; set; }

        public bool Radio { get; set; }

        public bool AirConditioning { get; set; }

        public bool Heating { get; set; }
    }
}