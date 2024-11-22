using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO
{
    public class DtoInventoryMovement
    {
        
        public string ItemCode { get; set; }    
        public string ItemDescription { get; set; }
        public string ItemCategory { get; set; }
        public decimal TotalReceiving {  get; set; }
        public decimal TotalMoveOrder { get; set; }
        public decimal TotalReceipt { get; set; }
        public decimal TotalIssue {  get; set; }
        public decimal TotalBorrowed { get; set; }
        public decimal TotalReturned { get; set; }
        public decimal TotalFuelRegister { get; set; }

        public decimal UnitCost { get; set; } 

        public decimal Amount { get; set; }

        //public decimal TotalIn { get; set; }
        //public decimal TotalOut { get; set; }
        public decimal Ending { get; set; }

        public decimal CurrentStock { get; set; }
        public decimal PurchaseOrder { get; set; }
        public decimal OtherPlus { get; set; }



    }
}
