using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP
{
    public class DtoMRP
    {

        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }

        public string Uom { get; set; }
        public string ItemCategory { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal SOH { get; set; }
        public decimal PreparedQuantity { get; set; }
        public decimal Reserve { get; set; }

        public decimal BufferLevel { get; set; }

        public decimal ReceiveIn { get; set; }

        public decimal ReceiptIn { get; set; }
        public decimal MoveOrderOut { get; set; }

        public decimal IssueOut { get; set; }

        public decimal BorrowedOut { get; set; }

        public decimal ReturnedBorrowed { get; set; }
        public decimal BorrowConsume { get; set; }
        public decimal SuggestedPo { get; set; }
        public decimal ReserveUsage { get; set; }
        public decimal AverageIssuance { get; set; }
        public int DaysLevel { get; set; }





        //public decimal BorrowedDepartment { get; set; }



        //public decimal BorrowedDifference { get; set; }

    }
}
