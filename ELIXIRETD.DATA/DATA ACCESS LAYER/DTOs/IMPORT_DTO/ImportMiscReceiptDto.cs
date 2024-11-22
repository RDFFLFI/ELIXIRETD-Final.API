using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO
{
    public class ImportMiscReceiptDto
    {

        public string SupplierCode { get; set; }
        public string SupplierName{ get; set; }
        public string PreparedBy { get; set; }
        public string Remarks { get; set; }
        public decimal TotalQuantity { get; set; }
        //public DateTime TransactionDate { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string Details { get; set; }

        public List<WarehouseReceiptDto> WarehouseReceipt { get; set; }

        public class WarehouseReceiptDto
        {
            public string SupplierName { get; set; }
            public string ItemCode { get; set; }
            public string ItemDescription { get; set; }
            public int MiscellaneousReceiptId { get; set; }
            public string Uom { get; set; }
            public string Quantity { get; set; }
            public string UnitCost { get; set; }
            public string AccountCode { get; set; }
            public string AccountTitles { get; set; }

            public string EmpId { get; set; }
            public string FullName { get; set; }


        }

    }
}
