using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class GetAvailableStocksForBorrowedIssue_Dto
    {

        public int WarehouseId { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public decimal RemainingStocks { get; set; }

        public string ReceivingDate { get; set; }

        public decimal UnitCost { get; set; }

        public decimal ActualRemaining { get; set; }



    }
}
