using System;
using System.Collections.Generic;
using System.Text;

namespace UCPortal.DTO.Request
{
    public class ViewStudentRegistrationRequest
    {
        public string id_number { get; set; }
        public int? payment { get; set; }
    }
}
