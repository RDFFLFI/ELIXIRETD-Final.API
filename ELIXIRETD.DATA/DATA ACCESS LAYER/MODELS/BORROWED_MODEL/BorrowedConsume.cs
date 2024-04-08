using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL
{
    public class BorrowedConsume : BaseEntity
    {
       

        public string ItemCode { get; set; }

        public int BorrowedItemPkey { get; set; }

        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public decimal Consume { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

        public string EmpId { get; set; }

        public string FullName { get; set; }

        public bool ? IsApproveReturn { get; set; }

        public bool IsActive { get; set; }

        public int BorrowedPkey { get; set; }

        public int ? ReportNumber { get; set; }


       
        

    }
}
