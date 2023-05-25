using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Customer : BaseEntity
    {

        public int Customer_No { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public string ModifyBy { get; set; }
 
        public DateTime ? ModifyDate { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        //public string AccountCode { get; set; }
        //public string AccountTitles { get; set; }

        public DateTime DateAdded { get; set; } /*= DateTime.Now;*/
        public bool IsActive { get; set; } = true;
        public string AddedBy { get; set; }

        public DateTime SyncDate { get; set; }

        public string StatusSync { get; set; }


    }
}
