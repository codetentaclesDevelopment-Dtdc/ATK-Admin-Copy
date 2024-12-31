using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class CompanyExpiryModel
    {

       
        public string CompanyId { get; set; }     
        public string CompanyName { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }

        public string pfcode { get; set; }
    }
}