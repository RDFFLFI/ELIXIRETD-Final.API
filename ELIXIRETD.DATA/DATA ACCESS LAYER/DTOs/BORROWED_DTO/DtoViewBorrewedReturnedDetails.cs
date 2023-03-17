using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class DtoViewBorrewedReturnedDetails
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }

        public decimal Quantity { get; set; }
        public decimal Consume { get; set; }

        public decimal ReturnQuantity { get; set; }



    }
}
