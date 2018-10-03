using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICDC_Portal.Models
{
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TargetUrl { get; set; }
    }
}