using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO.ConsolidationDto
{
    public class FuelRegisterReportsDto
    {
        public int? Id { get; set; }

        public string Source { get; set; }

        public string RequestorId { get; set; }
        public string RequestorName { get; set; }

        public string Item_Code { get; set; }
        public string Item_Description { get; set; }

        public string Uom { get; set; }
        public string Item_Categories { get; set; }

        public int? Warehouse_ReceivingId { get; set; }

        public decimal Unit_Cost { get; set; }

        public decimal Liters { get; set; }
        public string Asset { get; set; }

        public decimal? Odometer { get; set; }
        public string Company_Code { get; set; }
        public string Company_Name { get; set; }

        public string Department_Code { get; set; }
        public string Department_Name { get; set; }

        public string Location_Code { get; set; }
        public string Location_Name { get; set; }
        public string Account_Title_Code { get; set; }
        public string Account_Title_Name { get; set; }
        public string EmpId { get; set; }

        public string Fullname { get; set; }

        public string Added_By { get; set; }
        public DateTime Created_At { get; set; }
        public string Modified_By { get; set; }

        public string Approve_By { get; set; }
        public DateTime? Approve_At { get; set; }
        public string Transact_By { get; set; }
        public DateTime? Transact_At { get; set; }

        public string Remarks { get; set; }

    }
}
