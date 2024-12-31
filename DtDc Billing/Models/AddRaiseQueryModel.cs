using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class AddRaiseQueryModel
    {
        public string RaiseQueries_Description { get; set; }

        public string RaiseQueries_Name { get; set; }
   
        public string Domain_Name { get; set; }

        public string RaiseQueries_datetime { get; set; }

       public string  RaiseQueries_Status { get; set; }

        public string username { get; set; }

        public int RaiseQueries_ID { get; set; }

        public string pfcode { get; set; }
        
    }
}