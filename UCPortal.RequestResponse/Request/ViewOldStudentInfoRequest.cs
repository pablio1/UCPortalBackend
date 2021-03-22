using System;
using System.Collections.Generic;
using System.Text;

namespace UCPortal.RequestResponse.Request
{
    public class ViewOldStudentInfoRequest
    {
        public string id_number { get; set; }
        public int? payment { get; set; }
    }
}
