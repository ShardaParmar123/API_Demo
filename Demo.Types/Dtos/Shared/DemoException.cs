﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Types.Dtos.Shared
{
    public class DemoException
    {
        public DemoException(int statusCode, string message = null, string details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
