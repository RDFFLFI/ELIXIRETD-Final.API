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

    }
}
