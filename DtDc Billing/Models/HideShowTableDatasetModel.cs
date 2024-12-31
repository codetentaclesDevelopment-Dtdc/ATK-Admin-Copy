using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace DtDc_Billing.Models
{
    public class HideShowTableDatasetModel
    {
        public bool isRatem { get; set; }
        public bool isNondox { get; set; }
        public bool isDTDCPLUS { get; set; }
        public bool isDTDCPTP { get; set; }
        public bool isPriority { get; set; }
        public bool isLaptops { get; set; }
        public bool isRevPickupLaptops { get; set; }
        public bool isEcommerce { get; set; }
        public bool isGECreate { get; set; }
}
}