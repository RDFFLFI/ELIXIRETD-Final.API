using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO
{
    public class GetWarehouseDetailsByMReceiptDto
    {

        public int Id { get; set; }

        public int WarehouseId { get; set; }
        public string Itemcode { get; set; }
        public string ItemDescription { get; set; }

        public decimal TotalQuantity { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierName { get; set;}

        public string PreparedDate { get; set; }

        public string PreparedBy { get; set; }

        public string Remarks { get; set; }


        public string TransactionDate { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

        public string EmpId { get; set; }

        public string FullName { get; set; }


        public string Uom { get; set; }


        public decimal UnitCost { get; set; }

        public decimal TotalCost { get; set; }


        //public string TransactionType { get; set; } 

        public string Details { get; set; }
    }
}
