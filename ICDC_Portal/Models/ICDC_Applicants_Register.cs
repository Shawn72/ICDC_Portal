//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ICDC_Portal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ICDC_Applicants_Register
    {
        public byte[] timestamp { get; set; }
        public string Entry_No { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string No_Series { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime LastLoginDate { get; set; }
        public string Activation_Code { get; set; }
        public byte Activated { get; set; }
    }
}