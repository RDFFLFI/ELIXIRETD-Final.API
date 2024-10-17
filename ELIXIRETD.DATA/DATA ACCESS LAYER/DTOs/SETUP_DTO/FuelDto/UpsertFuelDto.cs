using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO.FuelDto
{
    public class UpsertFuelDto
    {
        public int ? Id { get; set; }
        public int? MaterialId { get; set; }

        public string Added_By { get; set; }
        public string Modified_By { get; set; }
    }
}
