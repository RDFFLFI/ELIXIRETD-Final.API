using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class DtoMiscReports
    {

        public int ReceiptId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Details { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; } 
        public string Uom { get; set; }
        public string Category { get; set; }
        public decimal Quantity { get; set; } 
        public string TransactBy { get; set; }
        public string TransactDate { get; set; }



    }
}
