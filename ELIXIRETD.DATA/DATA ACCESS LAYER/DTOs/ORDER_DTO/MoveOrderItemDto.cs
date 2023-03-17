using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO
{
    public class MoveOrderItemDto
    {
        public int OrderNo { get; set; }
        public int OrderPKey { get; set; }
        public decimal QuantityPrepared { get; set; }
        public bool IsActive { get; set; }
    }
}
