using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO
{
    public class ReasonDto
    {

        public int Id { get; set; }
        public string MainMenu { get; set; }
        public int MainMenuId { get; set; }
        public string ReasonName { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }
        public string DateAdded { get; set; }


    }
}
