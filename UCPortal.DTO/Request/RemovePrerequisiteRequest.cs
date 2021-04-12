using System;
using System.Collections.Generic;
using System.Text;

namespace UCPortal.DTO.Request
{
    public class RemovePrerequisiteRequest
    {
        public string internal_code { get; set; }
        public string prerequisite { get; set; }
    }
}
