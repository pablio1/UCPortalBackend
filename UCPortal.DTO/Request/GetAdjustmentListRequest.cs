using System;
using System.Collections.Generic;
using System.Text;

namespace UCPortal.DTO.Request
{
    public class GetAdjustmentListRequest
    {
        public string course_department { get; set; }
        public int status { get; set; }
        public int limit { get; set; }
        public int page { get; set; }
    }
}
