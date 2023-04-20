using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO
{
    public class LotNameDto
    {
        public int Id { get; set; }
        public string LotCode { get; set; }
        public string LotName { get; set; }
        public int LotNamesId { get; set; }

        public string SectionName { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }



    }
}
