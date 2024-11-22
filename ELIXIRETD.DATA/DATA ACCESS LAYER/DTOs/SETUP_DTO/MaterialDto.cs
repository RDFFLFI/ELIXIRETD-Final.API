using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO
{
    public class MaterialDto
    {

        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        //public int SubCategoryId { get; set; }
        //public string SubCategoryName { get; set; } 
        //public int AccountTitleId { get; set; }
        //public string AccountPName { get; set; }
        public string ItemCategoryName { get; set; }
        public int? ItemCategoryId { get; set; }
        public string Uom { get; set; }
        public int UomId { get; set; }
        public decimal BufferLevel { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }
        public string SyncStatus { get; set; }

        public string ModifyDate { get; set; }
        public string ModifyBy { get; set; }

        public string SyncDate { get; set; }

        public int ? LotSectionId { get; set; }
        public string SectionName { get; set; }


    }
}
