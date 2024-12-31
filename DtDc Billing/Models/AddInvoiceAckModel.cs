using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace DtDc_Billing.Models
{
    public class AddInvoiceAckModel
    {
        public string Invoiceno { get; set; }
        public string companyid { get; set; }
        public string invfromdate { get; set; }
        public string invtodate { get; set; }
        public int size { get; set; }
        

        [Required(ErrorMessage = "Please Select File")]
       public HttpPostedFileBase file { get; set; }
    }
}