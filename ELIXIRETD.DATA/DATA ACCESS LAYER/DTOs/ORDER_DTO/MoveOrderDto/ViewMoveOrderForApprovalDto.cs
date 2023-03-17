using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto
{
    public class ViewMoveOrderForApprovalDto
    {
        public int Id { get; set; }

        public int OrderNo { get; set; }

        public int BarcodeNo { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public string CustomerName { get; set; }

        public string ApprovedDate { get; set; }

        public decimal Quantity { get; set; }
        public string Address { get; set; }




    }
}
