using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class AddBox_SizesModel
    {

       
        public Nullable<System.DateTime> date { get; set; }

        public string temprecdate { get; set; }


        [Required(ErrorMessage = "Please Enter Quantity")]
        public Nullable<int> Quantity { get; set; }

        public double amount { get; set; }
    }
}