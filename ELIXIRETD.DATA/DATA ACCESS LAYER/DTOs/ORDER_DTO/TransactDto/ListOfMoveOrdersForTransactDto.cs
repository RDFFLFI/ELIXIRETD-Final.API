using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.TransactDto
{
    public class ListOfMoveOrdersForTransactDto
    {

        public int OrderNoPkey { get; set; }    

        public int OrderNoGenus {  get; set; }

        public int MIRId { get; set; }

        public int BarcodeNo { get; set; }

        public string OrderDate { get; set; }
        public string PreparedDate { get; set; }

        public string DateNeeded { get; set; }

        public string Department { get; set; }
        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }

        public string Category { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public decimal Quantity { get; set; }

        public bool IsPrepared { get; set; }

        public bool IsApprove { get; set; }

        public string ItemRemarks { get; set; }

        //public string Rush { get; set; }

        public decimal UnitCost { get; set; }

        public decimal TotalCost { get; set; }

        public string AssetTag { get; set; }




    }
}
