using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class DtoMiscIssue
    {

        public int IssueId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Details { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public decimal Quantity { get; set; }

        public string TransactBy { get; set; }
        public string TransactDate { get; set; }

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

        public string AddressOrder { get; set; }

        public string PreparedDate { get; set; }


        public string Remarks { get; set; }

        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }






    }
}
