using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO
{
    public class CancelledPoDto
    {
        public int Id { get; set; }
        public string PO_Number { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Supplier { get; set; }
        public decimal QuantityOrdered { get; set; }
        public string DateCancelled { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public decimal ActualGood { get; set; }
        public decimal TotalReject { get; set; }    
        public decimal ActualRemaining { get; set; }
    }
}
