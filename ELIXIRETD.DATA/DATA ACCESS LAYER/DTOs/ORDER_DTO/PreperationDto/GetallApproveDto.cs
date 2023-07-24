namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO
{
    public class GetallApproveDto
    {
        public int MIRId { get; set; }
        public string Department { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string Category { get; set; }
        public decimal TotalOrders { get; set; }
        public string PreparedDate { get; set; }
        public bool IsRush { get; set; }

        public string Rush { get; set; }

        public bool IsMove { get; set; }

    }



}
