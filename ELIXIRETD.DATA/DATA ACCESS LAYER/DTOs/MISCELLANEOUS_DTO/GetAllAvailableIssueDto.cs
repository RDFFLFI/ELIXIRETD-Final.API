using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO
{
    public class GetAllAvailableIssueDto
    {

        public int Id { get; set; } 
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public decimal TotalQuantity { get; set; }

        public decimal UnitCost { get; set; }

        public int Barcode { get; set; }
        public string PreparedDate { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

        public string EmpId { get; set; }

        public string FullName { get; set; }
    }
}
