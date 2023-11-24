using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL
{
    public class MiscellaneousReceipt : BaseEntity
    {
        public string supplier { get; set; }
        public string SupplierCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalQuantity { get; set; }

        public DateTime PreparedDate { get; set; }

        public string PreparedBy { get; set; }

        public string Remarks { get; set; }

        public bool IsActive { get; set; }

        public string TransactionType { get; set; }


        public DateTime TransactionDate { get; set; }


        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        //public string AccountCode { get; set; }
        //public string AccountTitles { get; set; }

        //public string EmpId { get; set; }

        //public string FullName { get; set; }

        public string Details { get; set; }

    }
}
