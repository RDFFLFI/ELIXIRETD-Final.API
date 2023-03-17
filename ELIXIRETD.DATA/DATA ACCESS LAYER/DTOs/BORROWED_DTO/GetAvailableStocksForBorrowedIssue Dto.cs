using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class GetAvailableStocksForBorrowedIssue_Dto
    {

        public int WarehouseId { get; set; }

        public string ItemCode { get; set; }

        public decimal RemainingStocks { get; set; }

        public string ReceivingDate { get; set; }

    }
}
