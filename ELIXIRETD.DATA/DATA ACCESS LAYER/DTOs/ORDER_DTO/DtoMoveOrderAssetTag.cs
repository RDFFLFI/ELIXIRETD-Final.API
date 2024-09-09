using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO
{
    public class DtoMoveOrderAssetTag
    {
         public string ? PoNumber { get; set; }
         public string PrNumber { get; set; }
         public int MIRId { get; set; }
        public int WareHouseId { get; set; }
        public string AcquisitionDate { get; set; }
         public string CustomerCode { get; set; }
         public string CustomerName { get; set; }
         public string ItemCode { get; set; }
         public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public decimal ServedQuantity { get; set; }
        public string AssetTag { get; set; }
        public string ApproveDate { get; set; }
    

    }
}
