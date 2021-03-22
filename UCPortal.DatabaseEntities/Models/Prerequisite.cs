using System;
using System.Collections.Generic;

#nullable disable

namespace UCPortal.DatabaseEntities.Models
{
    public partial class Prerequisite
    {
        public int PrerequisiteId { get; set; }
        public string InternalCode { get; set; }
        public string Prerequisites { get; set; }
    }
}
