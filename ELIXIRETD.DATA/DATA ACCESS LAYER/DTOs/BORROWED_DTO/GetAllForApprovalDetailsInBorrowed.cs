using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class GetAllForApprovalDetailsInBorrowed
    {

        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int BorrowedPKey { get; set; }
        public string Customer { get; set; }
        public string CustomerCode { get; set; }
        public string PreparedDate { get; set; }
        public string PreparedBy { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }

        public string Remarks { get; set; }

        public decimal Quantity { get; set; }

        public string TransactionDate { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }


        public string Uom { get; set; }

    }
}
