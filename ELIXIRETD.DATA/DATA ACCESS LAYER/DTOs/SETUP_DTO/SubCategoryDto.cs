using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO
{
    public class SubCategoryDto
    {
        public int Id { get; set; }
        public int SubCategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public int ItemCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string AddedBy { get; set; }
        public string DateAdded { get; set; }
        public bool IsActive { get; set; }


    }
}
