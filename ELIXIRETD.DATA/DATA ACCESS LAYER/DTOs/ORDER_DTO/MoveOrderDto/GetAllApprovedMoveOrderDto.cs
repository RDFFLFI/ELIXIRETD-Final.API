using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto
{
    public class GetAllApprovedMoveOrderDto
    {
        public int OrderNo { get; set; }
        public int BarcodeNo { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string Uom { get; set; }
        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }

        public string Category { get; set; }

        public decimal Quantity { get; set; }

        public string OrderDate { get; set; }

        public string PreparedDate { get; set; }

        public bool IsApprove { get; set; }

        public string ApprovedDate {get; set; }

        public bool IsPrepared { get; set; }    

        public bool IsPrint { get; set; }
        public bool IsTransact { get; set; } 
    }
}
