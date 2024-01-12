using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto
{
    public class ListOfOrdersForMoveOrderDto
    {
        public int Id { get; set; }

        public int OrderNo { get; set; }
        public int MIRId { get; set; }

        public string OrderDate { get; set; }
        public string DateNeeded { get; set; }
        //public string Department { get; set; }
        //public string CustomerName { get; set; }
        //public string CustomerCode { get; set; }
        public string Category { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public decimal QuantityOrder { get; set; }

        public bool IsApproved { get; set; }

        public decimal PreparedQuantity { get; set; }

        public string Remarks { get; set; }

        public string ItemRemarks { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

        public string EmpId { get; set; }

        public string FullName { get; set; }
        //public string Rush { get; set; }

        public string AssetTag {  get; set; }



    }
}
