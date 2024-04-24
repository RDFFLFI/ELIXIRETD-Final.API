using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.ListofServedDto;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class BorrowedTransactionReportsDto
    {
        public int BorrowedId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string EmpId { get; set; }
        public string FullName { get; set; }
        public string TransactedBy { get; set; }
        public string BorrowedDate { get; set; }
        public string Status { get; set; }
        public string Details { get; set; }
        public string Remarks { get; set; }

        public int ? AgingDays { get; set; }
        
            public int BorrowedItemPkey { get; set; }
            public string ItemCode { get; set; }
            public string ItemDescription { get; set; }
            public string Uom { get; set; }
            public decimal BorrowedQuantity { get; set; }

        
    }
}
