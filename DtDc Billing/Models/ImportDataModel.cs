using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class ImportDataModel
    {
        public HttpPostedFileBase File { get; set; }

        public int type { get; set; }

    }
}