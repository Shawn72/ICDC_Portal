using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICDC_Portal.Models
{
    public class DetailsInsertClass
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }

        //public  string ActivationCode = Guid.NewGuid().ToString();

        public Guid ActivationCode = Guid.NewGuid();


    }
}