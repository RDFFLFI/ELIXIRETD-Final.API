using DocumentFormat.OpenXml.Office.CoverPageProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO
{
    public class ApproveFuelDto
    {
        public int Id {  get; set; }
        public string Approve_By { get; set; }
        public string Transact_By { get; set; }
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

    }
}
