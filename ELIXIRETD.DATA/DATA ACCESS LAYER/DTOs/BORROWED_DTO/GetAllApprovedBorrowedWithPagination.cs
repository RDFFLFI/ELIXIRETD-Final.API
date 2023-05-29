using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class GetAllApprovedBorrowedWithPagination
    {

        public int BorrowedPKey { get; set; }
        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public decimal TotalQuantity { get; set; }

        public string ApproveDate { get; set; }

        public string Remarks { get; set; }

        public string ApproveBy { get; set; }

        public string TransactionDate { get; set; }


    }
}
