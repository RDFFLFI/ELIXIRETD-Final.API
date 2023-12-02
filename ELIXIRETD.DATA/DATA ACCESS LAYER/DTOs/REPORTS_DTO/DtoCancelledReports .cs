using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class DtoCancelledReports
    {
        public int OrderId { get; set; }

        public int OrderNo { get; set; }
        public string DateNeeded { get; set; }
        public string DateOrdered { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal QuantityOrdered { get; set; }

        public string CancelledDate { get; set; }   
        public string CancelledBy { get; set; }
        public string Reason { get; set; }




    }
}
