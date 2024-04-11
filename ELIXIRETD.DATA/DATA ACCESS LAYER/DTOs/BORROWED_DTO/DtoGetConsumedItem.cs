using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class DtoGetConsumedItem
    {
        public int Id {  get; set; }
        public int BorrowedItemPKey { get; set; }
        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string Uom { get; set; }

        public decimal BorrowedQuantity { get; set; }

        public decimal ItemConsumedQuantity { get; set; }

        public decimal ConsumedQuantity { get; set; }


        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

        public string FullName { get; set; }

        public string EmpId { get; set; }

        public bool IsActive { get; set; }
        public bool? IsReturned { get; set; }
        public bool IsApproved { get; set; }

        public decimal ReturnedQuantity { get; set; }

        public int ? ReportNumber { get; set; }


    }
}
