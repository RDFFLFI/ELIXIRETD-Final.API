namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto
{
    public class TotalListOfApprovedPreparedDateDto
    {

        public int Id { get; set; }


        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }
        public string Department { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }


        public string CustomerName { get; set; }

        public string CustomerCode { get; set; }

        public string Category { get; set; }

        public decimal TotalOrders { get; set; }

        public string PreparedDate { get; set; }

        public bool IsMove { get; set; }

        public bool IsReject { get; set; }  
        public string Remarks { get; set; }

        public bool IsRush { get; set; }

        public string Rush { get; set; }


    }
}
