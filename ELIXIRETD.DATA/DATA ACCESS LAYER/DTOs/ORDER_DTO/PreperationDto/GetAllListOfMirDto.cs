using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.PreperationDto
{
    public class GetAllListOfMirDto
    {

        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public string CustomerType { get; set; }

        public int MIRId { get; set; }

        public decimal TotalQuantity { get; set; }

        public string DateNeeded { get; set; }

        public string OrderedDate { get; set; }

        public bool IsRush { get; set; }

        public string Rush { get; set; }

        public string ItemRemarks { get; set; }
        
    }
}
