using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DtDc_Billing.Models
{
    public class steplinechart
    {
       
       

        public steplinechart(double? netamount, int? month, int? year)
        {
            this.netamount = netamount;
            this.month = month;
            this.year = year;
        }

        public Nullable<double> year = null;
        public Nullable<double> netamount;
        public Nullable<int> month;
    }
}