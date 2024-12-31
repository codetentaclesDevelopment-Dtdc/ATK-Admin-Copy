using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class IssueStationaryModel
    {
       
        public int Issue_id { get; set; }

        [Required(ErrorMessage = "Please Enter startno")]
        public string startno { get; set; }

        [Required(ErrorMessage = "Please Enter endno")]
        public string endno { get; set; }

        public Nullable<int> noofleafs { get; set; }
        public string Inssuedate { get; set; }

        [Required(ErrorMessage = "Please Enter Comapny_Id")]
        public string Comapny_Id { get; set; }

        [Required(ErrorMessage = "Please Enter Employee Name")]
        public string EmployeeName { get; set; }

        public string Pf_code { get; set; }


    }
}