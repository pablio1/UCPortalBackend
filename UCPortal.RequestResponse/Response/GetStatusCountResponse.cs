using System;
using System.Collections.Generic;
using System.Text;

namespace UCPortal.RequestResponse.Response
{
    public class GetStatusCountResponse
    {
        public int registered { get; set; }
        public int approved_registration_registrar { get; set; }
        public int disapproved_registration_registrar { get; set; }
        public int approved_registration_dean { get; set; }
        public int disapproved_registration_dean { get; set; }
        public int selecting_subjects { get; set; }
        public int approved_by_dean { get; set; }
        public int disapproved_by_dean { get; set; }
        public int approved_by_accounting { get; set; }
        public int approved_by_cashier { get; set; }
        public int officially_enrolled { get; set; }
        public int withdrawn_enrollment_before_start_of_class { get; set; }
        public int withdrawn_enrollment_start_of_class { get; set; }
        public int cancelled { get; set; }
        public int request { get; set; }
        public int accounting_count { get; set; }
        public int pending_promissory { get; set; }
        public int approved_promissory { get; set; }
    }
}
