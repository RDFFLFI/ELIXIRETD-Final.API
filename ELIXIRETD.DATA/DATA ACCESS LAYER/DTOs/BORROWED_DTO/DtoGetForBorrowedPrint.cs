using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class DtoGetForBorrowedPrint
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public decimal BorrowedQuantity { get; set; }

        public decimal UnitCost { get; set; }

        public string TransactionDate { get; set; }

        public string PreparedDate { get; set; }

        public string Details { get; set; }





    }
}
