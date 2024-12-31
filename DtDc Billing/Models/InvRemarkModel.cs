using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class InvRemarkModel
    {

        
        public string InvoiceNo { get; set; }
    
        public string Remark { get; set; }

        public string NetAmount { get; set; }

        public string AfterUpdatedNetAmount { get; set; }

        
    }


}