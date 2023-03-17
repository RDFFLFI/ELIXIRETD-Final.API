using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.PreperationDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.TransactDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.Orders
{
    public interface IOrdering
    {
        Task<bool> AddNewOrders(Ordering Orders);
        Task<PagedList<GetAllListofOrdersPaginationDto>> GetAllListofOrdersPagination(UserParams userParams);
        Task<IReadOnlyList<GetAllListofOrdersDto>> GetAllListofOrders(string Customer);
        Task<IReadOnlyList<OrderSummaryDto>> OrderSummary(string DateFrom, string DateTo);
        Task<bool> SchedulePreparedDate(Ordering orders);
        Task<bool> GenerateNumber(GenerateOrderNo generate);
        Task<bool> EditQuantityOrder(Ordering orders);
        Task <bool> ApprovePreparedDate(Ordering orders);
        Task<bool> RejectPreparedDate(Ordering orders);
        Task<IReadOnlyList<GetAllListCancelOrdersDto>> GetAllListOfCancelOrders();
        Task<bool> ReturnCancelOrdersInList( Ordering orders);
        Task<IReadOnlyList<DetailedListofOrdersDto>> DetailedListOfOrders (string customer);
        Task<IReadOnlyList<GetallApproveDto>> GetAllListForApprovalOfSchedule();
        Task<IReadOnlyList<GetallOrderfroScheduleApproveDto>> GetAllOrdersForScheduleApproval(int Id);
        Task<IReadOnlyList<GetAllCalendarApproveDto>> GetAllApprovedOrdersForCalendar();
        Task<bool> CancelOrders(Ordering orders);
        Task<GetMoveOrderDetailsForMoveOrderDto> GetMoveOrderDetailsForMoveOrder(int orderId);
        Task<bool> PrepareItemForMoveOrder(MoveOrder orders);
        Task<IReadOnlyList<ListOfPreparedItemsForMoveOrderDto>> ListOfPreparedItemsForMoveOrder(int id);
        Task<IReadOnlyList<ListOfOrdersForMoveOrderDto>> ListOfOrdersForMoveOrder(int id);

        Task<PagedList<GetAllListForMoveOrderPaginationDto>> GetAllListForMoveOrderPagination(UserParams userParams);

        Task<IReadOnlyList<TotalListOfApprovedPreparedDateDto>> TotalListOfApprovedPreparedDate(string customername);

        Task<ItemStocksDto> GetFirstNeeded(string itemCode);

        Task<ItemStocksDto> GetActualItemQuantityInWarehouse(int id, string itemcode);

        Task<IReadOnlyList<GetAllOutOfStockByItemCodeAndOrderDateDto>> GetAllOutOfStockByItemCodeAndOrderDate(string itemcode, string orderdate);

        Task<bool> ApprovalForMoveOrders(MoveOrder moveorder);
        Task<IReadOnlyList<ViewMoveOrderForApprovalDto>> ViewMoveOrderForApproval(int id);

        Task<PagedList<ForApprovalMoveOrderPaginationDto>> ForApprovalMoveOrderPagination(UserParams userParams);
        Task<PagedList<ForApprovalMoveOrderPaginationDto>> ForApprovalMoveOrderPaginationOrig(UserParams userParams, string search);

        Task<PagedList<ApprovedMoveOrderPaginationDto>> ApprovedMoveOrderPagination (UserParams userParams);
        
        Task<PagedList<ApprovedMoveOrderPaginationDto>> ApprovedMoveOrderPaginationOrig (UserParams userParams, string search);

        Task<GetAllApprovedMoveOrderDto> GetAllApprovedMoveOrder(int id);

        Task<bool> CancelMoveOrder(MoveOrder moveOrder);
        Task<bool> UpdatePrintStatus(MoveOrder moveorder);
        Task <bool> CancelControlInMoveOrder (Ordering orders);
        Task<bool> ReturnMoveOrderForApproval(MoveOrder moveorder);
        Task<bool> RejectApproveMoveOrder(MoveOrder moveOrder);
        Task<bool> RejectForMoveOrder(MoveOrder moveOrder);

        Task<PagedList<RejectedMoveOrderPaginationDto>> RejectedMoveOrderPagination(UserParams userParams);
        Task<PagedList<RejectedMoveOrderPaginationDto>> RejectedMoveOrderPaginationOrig(UserParams userParams, string search);

        Task<IReadOnlyList<TotalListForTransactMoveOrderDto>> TotalListForTransactMoveOrder(bool status);

        Task<IReadOnlyList<ListOfMoveOrdersForTransactDto>> ListOfMoveOrdersForTransact(int orderid);

        Task<bool> TransanctListOfMoveOrders(TransactMoveOrder transact);

        Task<bool> ValidatePrepareDate(Ordering orders);

        Task<bool> SavePreparedMoveOrder(MoveOrder order);










        //============================ Validation ====================================================================
        Task<bool> ValidateExistOrderandItemCode(int TransactId, string ItemCode , string customerName);
        Task<bool> ValidateDateNeeded(Ordering orders);
        Task<bool> ValidateCustomerName(string Customer);
        Task<bool> ValidateUom(string Uom);
        Task<bool> ValidateItemCode (string ItemCode);
        //Task<bool> ValidateCustomerCode(string Customercode);

        Task<bool> ValidateWarehouseId(int id , string itemcode);

        Task<bool> ValidateQuantity(decimal quantity);


    
       



    }
}
