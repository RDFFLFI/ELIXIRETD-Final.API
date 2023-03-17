using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL
{
    public class BorrowedReceipt : BaseEntity
    {
        public string Customer { get; set; }

        public string CustomerCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalQuantity { get; set; }

        public DateTime PreparedDate { get; set; }

        public string PreparedBy { get; set; }

        public string Details { get; set; }

        public string Remarks { get; set; }

        public bool IsActive { get; set; }
        public bool ? IsReturned { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ? Consume  { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ? ReturnQuantity { get; set; }

        



    }
}
