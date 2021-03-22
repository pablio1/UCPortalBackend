using System;
using System.Collections.Generic;

#nullable disable

namespace UCPortal.DatabaseEntities.Models
{
    public partial class _212grade
    {
        public int GradesId { get; set; }
        public string StudId { get; set; }
        public string EdpCode { get; set; }
        public DateTime Dte { get; set; }
        public string Midterm { get; set; }
        public string Final { get; set; }
    }
}
