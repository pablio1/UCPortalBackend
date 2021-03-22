using System;
using System.Collections.Generic;

#nullable disable

namespace UCPortal.DatabaseEntities.Models
{
    public partial class Config
    {
        public int ConfigId { get; set; }
        public int Sequence { get; set; }
        public short IdYear { get; set; }
        public short CampusId { get; set; }
    }
}
