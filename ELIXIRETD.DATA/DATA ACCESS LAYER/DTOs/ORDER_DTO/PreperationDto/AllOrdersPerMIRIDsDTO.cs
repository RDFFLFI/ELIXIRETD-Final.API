using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.PreperationDto
{
    public class AllOrdersPerMIRIDsDTO
    {

        public int Id { get; set; }
        public int MIRId {  get; set; }

        public int OrderNo {  get; set; }

        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }    
        
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public decimal QuantityOrder { get; set; }
        //public bool IsActive { get; set; }
        //public bool IsPrepared { get; set; }

        public decimal ActualReserve { get; set; }
        public decimal Reserve { get; set; }
        public decimal StockOnHand { get; set; }
        //public string Rush { get; set; }

        public string ItemRemarks { get; set; }

        public decimal StandardQuantity { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

        public string EmpId { get; set; }

        public string FullName { get; set; }

        public string AssetTag {  get; set; }
    }
}
