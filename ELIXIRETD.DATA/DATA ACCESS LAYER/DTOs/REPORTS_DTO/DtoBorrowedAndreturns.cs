using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class DtoBorrowedAndreturns
    {
        public int BorrowedPKey { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }

        public string Category { get; set; }
        public decimal Quantity { get; set; }

        public decimal ReturnedQuantity { get; set; }

        public decimal Consumes { get; set; }
        public string BorrowedBy { get; set; }
        public string BorrowedDate { get; set; }


    }
}
