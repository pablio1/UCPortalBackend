using System;
using System.Collections.Generic;

#nullable disable

namespace UCPortal.DatabaseEntities.Models
{
    public partial class Curriculum
    {
        public int CurrId { get; set; }
        public int Year { get; set; }
        public int IsDeployed { get; set; }
    }
}
