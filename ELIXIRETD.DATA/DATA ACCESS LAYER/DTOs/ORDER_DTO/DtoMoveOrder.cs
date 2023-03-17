using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO
{
    public class DtoMoveOrder
    {
        public int Id { get; set; }
        public int BarcodeNo { get; set; }

        public int OrderNo { get; set; }
        public string CustomerName { get; set; }
        public string Department { get; set; }
        public string Company { get; set; }
        public string Category { get; set; }
        public string Uom { get; set; }
        public string OrderDate { get; set; }
        public string DateNeeed { get; set; }
        public string PreparedDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsReject { get; set; }
        public bool IsApprove { get; set; }
        public bool IsPrepared { get; set; }
        public string ApprovedDate { get; set; }
        public string RejectedDate { get; set; }
        public string Remarks { get; set; }

        public int BarCodes { get; set; }
        public int OrderNoPKey { get; set; }
        public bool IsPrint { get; set; }
        public bool IsTransact { get; set; }
        public string BatchNo { get; set; }


    }
}
