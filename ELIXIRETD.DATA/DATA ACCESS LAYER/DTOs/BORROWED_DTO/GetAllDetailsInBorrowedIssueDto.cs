using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class GetAllDetailsInBorrowedIssueDto
    {

        public int WarehouseId { get; set; }
        public int BorrowedPKey { get; set; }
        public string Customer { get; set; }
        public string CustomerCode { get; set; }    
        public string PreparedDate { get; set; }    
        public string PreparedBy { get; set; }
        public string ItemCode {get; set; }
        public string ItemDescription { get; set; }

        public decimal Quantity { get; set; }
        public string Remarks { get; set; }
        public decimal Consumes {get; set; }
        public decimal ReturnQuantity { get; set; }


    }
}
