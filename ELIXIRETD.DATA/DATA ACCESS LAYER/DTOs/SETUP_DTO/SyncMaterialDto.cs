using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO
{
    public class SyncMaterialDto
    {
        public int ? Material_No { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }

        public int BufferLevel { get; set; }
        public bool IsActive { get; set; } = true;

        public string UomCode { get; set; }
        public int UomId { get; set; }

        public string ItemCategoryName { get; set; }
        public int ItemCategoryId { get; set; }
        
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string AddedBy { get; set; }


        public string ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }

        public DateTime SyncDate { get; set; }

        public string StatusSync { get; set; }
    }
}
