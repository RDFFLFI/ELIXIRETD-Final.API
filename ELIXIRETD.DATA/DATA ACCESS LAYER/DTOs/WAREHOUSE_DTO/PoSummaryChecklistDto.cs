using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.WAREHOUSE_DTO
{
    public class PoSummaryChecklistDto
    {
        public int Id { get; set; }
        public string PO_Number { get; set; }
        public string PR_Number { get; set; }
        public DateTime PR_Date { get; set; }
        public DateTime PO_Date { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Supplier { get; set; }
        public string UOM { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal ActualGood { get; set; }
        public decimal ActualRemaining { get; set; }
        public bool IsActive { get; set; }
        public bool IsQcReceiveIsActive { get; set; }
        public bool IsWarehouseReceived { get; set; }

    }
}
