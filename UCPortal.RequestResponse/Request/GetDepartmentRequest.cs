using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UCPortal.RequestResponse.Request
{
    public class GetDepartmentRequest
    {
        [Required]
        public string department { get; set; }
    }
}
