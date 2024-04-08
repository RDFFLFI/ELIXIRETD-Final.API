using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class GetAllBorrowedReceiptWithPaginationDto
    {

        public int BorrowedPKey { get; set; }
        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public string EmpId { get; set; }
        public string FullName { get; set; }

        public decimal TotalQuantity { get; set; }  

        public string BorrowedDate { get; set; }

        public string Remarks { get; set; }

        public string PreparedBy { get; set; }
        public bool IsActive { get; set; }  

        //public bool IsPrepared { get; set; }

        public string TransactionDate { get; set; }

        public bool IsApproved { get; set; }

        public string Reason { get; set; }

        public string ApproveDate { get; set; }

        public int  AgingDays { get; set; }


        public string StatusApprove { get; set; }

        //public decimal UnitCost { get; set; }

        public string Details { get; set; }


    }
}
