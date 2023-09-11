using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO
{
    public class AccountTitlesDto
    {
        public int Id { get; set; }
        public string AccountPName { get; set; }
        public string ItemCategoryName { get; set; }
        public string AddedBy { get; set; }
        public string DateAdded { get; set; }
        public bool IsActive { get; set; }

        public int ItemCategoryId { get; set; }

    }
}
