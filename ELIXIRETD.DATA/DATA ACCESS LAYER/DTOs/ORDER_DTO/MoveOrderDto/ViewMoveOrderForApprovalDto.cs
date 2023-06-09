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

        public int MIRId { get; set; }

        public int BarcodeNo { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public string CustomerName { get; set; }

        public string Customercode { get; set; }

        public string ApprovedDate { get; set; }

        public decimal Quantity { get; set; }
        public string Address { get; set; }

        public string Rush { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

    }
}
