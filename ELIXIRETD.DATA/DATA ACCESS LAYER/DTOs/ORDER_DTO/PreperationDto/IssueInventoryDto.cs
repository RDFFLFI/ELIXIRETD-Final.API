using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.PreperationDto
{
    public class IssueInventoryDto
    {

        public string ItemCode { get; set; }

        public decimal Quantity { get; set; } 

        public int warehouseId { get; set; }
    }
}
