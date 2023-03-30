using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class DtoReturnedpagination
    {

        public int BorrowedPKey { get; set; }
        public int TotalbPKey { get; set; }
        public decimal ReturnedQuantity { get; set; }
        public string CustomerCode { get; set; }
        public int WarehouseId { get; set; }

        public string ItemCode { get; set; }
    }
}
