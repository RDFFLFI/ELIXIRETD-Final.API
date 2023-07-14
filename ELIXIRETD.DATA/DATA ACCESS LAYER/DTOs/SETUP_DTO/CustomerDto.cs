using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }

        public string SyncStatus { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        //public string AccountCode { get; set; }
        //public string AccountTitles { get; set; }


        //public string Address { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }

        public string ModifyDate { get; set; }
        public string ModifyBy { get; set; }

        public bool IsActive { get; set; }

        public string SyncDate { get; set; }

    }
}
