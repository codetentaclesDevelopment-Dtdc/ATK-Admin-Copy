using DtDc_Billing.Entity_FR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DtDc_Billing.Models
{
    public class Jobclass
    {
        private DB_A43B74_wingsgrowdbEntities db = new DB_A43B74_wingsgrowdbEntities();





        public void ExpiredStationary()
        {


            //var stationary = db.Stationaries.ToList().Where(m => DateTime.Now.Date >= m.Expiry_Date.Value.AddDays(60) && m.Status == 0).ToList();

            var stationary = db.Stationaries.ToList().Where(m => m.Status == 0).ToList();

            List<string> Mystring = new List<string>();


            foreach (var i in stationary)
            {
                char stch = i.startno[0];
                char Endch = i.endno[0];

                long startConsignment = Convert.ToInt64(i.startno.Substring(1));
                long EndConsignment = Convert.ToInt64(i.endno.Substring(1));


                int flag = 0;

                for (long b = startConsignment; b <= EndConsignment; b++)
                {





                    ExpiredStationary ex = new ExpiredStationary();

                    string consignmentno = stch + b.ToString();


                    Transaction transaction = db.Transactions.Where(m => m.Consignment_no == consignmentno).FirstOrDefault();

                    if (transaction == null)
                    {
                        ExpiredStationary ex1 = db.ExpiredStationaries.Where(mbox => mbox.Consignment_no == consignmentno).FirstOrDefault();



                        ex.Consignment_no = stch + b.ToString();

                        ex.Expiry_Date = i.Expiry_Date.Value.AddDays(90);

                        ex.Expiry_Exceded = "Near To Expire";

                        TimeSpan ? difference =  i.Expiry_Date.Value.AddDays(90) - DateTime.Now;

                        ex.days = Math.Max(0,difference.Value.Days);



                        if (DateTime.Now > i.Expiry_Date.Value.AddDays(90))
                        {
                            ex.Expiry_Exceded = "Expired";
                        }

                        if (ex1 == null)
                        {
                            db.ExpiredStationaries.Add(ex);
                        }
                        else
                        {
                            ex.Ex_Id = ex1.Ex_Id;
                            db.Entry(ex1).State = EntityState.Detached;
                            db.Entry(ex).State = EntityState.Modified;
                        }



                        db.SaveChanges();

                        flag = 1;


                    }








                }


                if(flag == 0)
                {
                    i.Status = 1;
                    db.Entry(i).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }








        }



        public void deletefromExpiry(string Con_no)
        {

            ExpiredStationary expiredStationary = db.ExpiredStationaries.Where(m => m.Consignment_no == Con_no).FirstOrDefault();

            if (expiredStationary != null)
            {
                db.ExpiredStationaries.Remove(expiredStationary);
                db.SaveChanges();
            }
        }


    }
}