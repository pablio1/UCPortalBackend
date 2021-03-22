using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UCPortal.RequestResponse.Request
{
    public class GetStudyLoadRequest
    {
        [Required]
        public string id_number { get; set; }
    }
}
