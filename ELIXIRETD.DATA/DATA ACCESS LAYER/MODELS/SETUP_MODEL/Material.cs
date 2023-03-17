namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Material : BaseEntity
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public SubCategory SubCategory { get; set; }
        public int SubCategoryId { get; set; }
        public Uom Uom { get; set; }
        public int UomId { get; set; }
        public int BufferLevel { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string AddedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public string ItemCategoryName { get; set; }



    }
}
