using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.WAREHOUSE_DTO
{
    public class ListofwarehouseReceivingIdDto
    {

         public int Id { get; set; }
        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string ReceivingDate { get; set; }
        public decimal ActualGood { get; set; }

    }
}
