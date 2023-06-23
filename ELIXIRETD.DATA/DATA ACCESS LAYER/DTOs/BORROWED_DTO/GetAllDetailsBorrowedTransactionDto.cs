namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class GetAllDetailsBorrowedTransactionDto
    {

        public int Id { get; set; }

        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }
        //public string ItemCode { get; set; }
        //public string ItemDescription { get; set; }

        public string TransactionDate { get; set; }

        public string BorrowedDate { get; set; }

        public bool IsApproved { get; set; }

        public string IsApprovedDate { get; set; }

        public decimal TotalBorrowed { get; set; }

        public bool? IsApproveReturned { get; set; }

        public string IsApproveReturnedDate { get; set; }

        public decimal Consumed { get; set; }
        public decimal ReturnedQuantity { get; set; }

        public bool? IsReject { get; set; }

        public string IsRejectDate { get; set; }

        public string Remarks { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }

        public string Reason { get; set; }


        public string PreparedBy { get; set; }

        public string StatusApprove { get; set; }

        public int AgingDays { get; set; }
    }
}
