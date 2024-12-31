using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class CustomerSupport
    {

        public long User_Id { get; set; }
     
        public string Name { get; set; }
        [Required(ErrorMessage = "Please Enter Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail")]
        public string Email { get; set; }
        public string Contact_no { get; set; }
        public string PF_Code { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please Enter Password")]
        public string Password_U { get; set; }
        public string Usertype { get; set; }
        public Nullable<System.DateTime> Datetime_User { get; set; }

    }
}