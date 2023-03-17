using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.WAREHOUSE_DTO
{
    public class RejectWarehouseReceivingDto
    {
        public int Id { get; set; }
        public int QcReceivingId { get; set; }
        public int PO_Number { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Supplier { get; set; }
        public string Uom { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal ActualGood { get; set; }
        public decimal ActualReject { get; set; }
        public string ReceivingDate { get; set; }
        public string Remarks { get; set; }
        public bool IsWarehouseReceived { get; set; }
        public bool ConfirmRejectByWarehouse { get; set; }
        public bool ConfirmRejectByQc { get; set; }
    }
}
