using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class DtoWarehouseReceivingReports
    {

        public int WarehouseId { get; set; }
        public int PoNumber { get; set; }
        public string ReceiveDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescrption { get; set; }

        public string Uom { get; set; }

        public string Category { get; set; }

        public decimal Quantity { get; set; }

        public decimal TotalReject { get; set; }

        public string SupplierName { get; set; }

        public string ReceivedBy { get; set; }

        public string TransactionType { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalUnitPrice { get; set; }

    }
}
