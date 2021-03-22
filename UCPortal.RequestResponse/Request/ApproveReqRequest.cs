using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UCPortal.RequestResponse.Request
{
    public class ApproveReqRequest
    {
        [Required]
        public string id_number { get; set; }
        public int approved_deblock { get; set; }
        public int approved_overload { get; set; }
        public int approved_promissory { get; set; }
        public int max_units { get; set; }
        public int promise_pay { get; set; }
        public string message { get; set; }
    }
}
