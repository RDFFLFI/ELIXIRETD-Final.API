using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class DtoCancelledReports
    {
        public int MIRId { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }

        //[Column(TypeName = "Date")]
        public string DateNeeded { get; set; }
        //[Column(TypeName = "Date")]
        public string DateOrdered { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }

        public string DepartmentCode { get; set; }
        public string Department { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal QuantityOrdered { get; set; }

        public string Reason { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public DateTime ? CancelledDate { get; set; }
        public string CancelledBy { get; set; }

        public string ItemRemarks { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }







    }
}
