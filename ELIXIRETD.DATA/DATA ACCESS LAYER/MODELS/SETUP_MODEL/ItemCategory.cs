﻿namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class ItemCategory : BaseEntity
    {
     
        public string ItemCategoryName { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;

        public SubCategory SubCategory { get; set; }

        public int SubCategId { get; set; }

        public string AddedBy { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
