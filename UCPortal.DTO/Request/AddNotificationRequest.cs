﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UCPortal.DTO.Request
{
    public class AddNotificationRequest
    {
        public string stud_id { get; set; }
        public string from_sender { get;set; }
        public string message { get; set; }
    }
}
