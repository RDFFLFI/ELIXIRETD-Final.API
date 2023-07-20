namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
    public class TransactMoveOrder : BaseEntity
    {
      
        public int OrderNo { get; set; }
        public string Department { get; set; }
        public string Company { get; set; }
        public string Customercode { get; set; }
        public string CustomerName { get; set; }
        public bool IsActive { get; set; }
        public bool? IsApprove { get; set; }
        public string PreparedBy { get; set; }

        public DateTime? PreparedDate { get; set; }
        public int OrderNoPkey { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public bool IsTransact { get; set; }

        public string DeliveryStatus { get; set; }
        public string TransactionType { get; set; }


        public string Rush { get; set; }

        public string CustomerType { get; set; }


        public string ItemRemarks { get; set; }


    }
}
