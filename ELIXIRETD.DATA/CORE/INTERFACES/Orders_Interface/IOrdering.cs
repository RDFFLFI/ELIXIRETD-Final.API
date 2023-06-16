using ELIXIRETD.DATA.CORE.API_RESPONSE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.MoveOrderDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.ORDER_DTO.Notification_Dto;
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
      
        Task<IReadOnlyList<GetallOrderfroScheduleApproveDto>> GetAllOrdersForScheduleApproval(int Id);
        Task<IReadOnlyList<GetAllCalendarApproveDto>> GetAllApprovedOrdersForCalendar(bool status );
        Task<bool> CancelOrders(Ordering orders);
        Task<GetMoveOrderDetailsForMoveOrderDto> GetMoveOrderDetailsForMoveOrder(int orderId);
        Task<bool> PrepareItemForMoveOrder(MoveOrder orders);
        Task<IReadOnlyList<ListOfPreparedItemsForMoveOrderDto>> ListOfPreparedItemsForMoveOrder(int id);
        Task<IReadOnlyList<ListOfOrdersForMoveOrderDto>> ListOfOrdersForMoveOrder(int id);

      

      
        Task<ItemStocksDto> GetFirstNeeded(string itemCode);

        Task<ItemStocksDto> GetActualItemQuantityInWarehouse(int id, string itemcode);

        Task<IReadOnlyList<GetAllOutOfStockByItemCodeAndOrderDateDto>> GetAllOutOfStockByItemCodeAndOrderDate(string itemcode, string orderdate);

        Task<bool> ApprovalForMoveOrders(MoveOrder moveorder);
        Task<IReadOnlyList<ViewMoveOrderForApprovalDto>> ViewMoveOrderForApproval(int id);

      
     

       
        
       

        Task<GetAllApprovedMoveOrderDto> GetAllApprovedMoveOrder(int id);

        Task<bool> CancelMoveOrder(MoveOrder moveOrder);
        Task<bool> UpdatePrintStatus(MoveOrder moveorder);
        Task <bool> CancelControlInMoveOrder (Ordering orders);
        Task<bool> ReturnMoveOrderForApproval(MoveOrder moveorder);
        Task<bool> RejectApproveMoveOrder(MoveOrder moveOrder);
        Task<bool> RejectForMoveOrder(MoveOrder moveOrder);

      
      
        Task<IReadOnlyList<TotalListForTransactMoveOrderDto>> TotalListForTransactMoveOrder(bool status);

        Task<IReadOnlyList<ListOfMoveOrdersForTransactDto>> ListOfMoveOrdersForTransact(int orderid);

        Task<bool> TransanctListOfMoveOrders(TransactMoveOrder transact);

        Task<bool> ValidatePrepareDate(Ordering orders);

        Task<bool> SavePreparedMoveOrder(MoveOrder order);




        //Notification

        Task<IReadOnlyList<DtoOrderNotif>> GetOrdersForNotification();
        Task<IReadOnlyList<DtoForMoveOrderNotif>> GetMoveOrdersForNotification();

        Task<IReadOnlyList<DtoForApprovalMoveOrderNotif>> GetForApprovalMoveOrdersNotification();

        Task<IReadOnlyList<DtoForTransactNotif>> GetAllForTransactMoveOrderNotification();

        Task<IReadOnlyList<DtoRejectMoveOrderNotif>> GetRejectMoveOrderNotification();

        Task<IReadOnlyList<GetallApproveDto>> GetAllListForApprovalOfSchedule();







        //============================ Validation ====================================================================
        Task<bool> ValidateExistOrderandItemCode(int TransactId, string ItemCode, string customertype, string itemdescription, string customercode);
        Task<bool> ValidateDateNeeded(Ordering orders);

        Task<bool> ValidateCustomerCode(string Customer);
        Task<bool> ValidateCustomerName(string Customer , string CustomerName , string customerType);
        Task<bool> ValidateUom(string Uom);
        Task<bool> ValidateItemCode (string ItemCode , string itemdescription);
        Task<bool> ValidateItemDescription (string ItemDescription);

        Task<bool> ValidateWarehouseId(int id , string itemcode);

        Task<bool> ValidateQuantity(decimal quantity);

        //Task<bool> ValidateDepartment(string departmentname);
        //Task<bool> ValidateCompany(string companycode, string companyname);
        //Task<bool> ValidateLocation(string locationcode, string locationname);


        //====================================================== Update Orders ==================================================================

        Task<bool> AddNewOrders(Ordering Orders);
        Task<PagedList<GetAllListofOrdersPaginationDto>> GetAllListofOrdersPagination(UserParams userParams/*, bool status*/);

        Task<PagedList<GetAllListofOrdersPaginationDto>> GetAllListofOrdersPaginationOrig (UserParams userParams, string search/* , bool status*/);

        Task<IReadOnlyList<GetAllListOfMirDto>> GetAllListOfMir(string Customer, bool status);
        Task<IReadOnlyList<GetAllListOfMirDto>> GetAllListOfMirOrders(string Customer);


        //Task<IEnumerable<AllOrdersPerMIRIDsDTO>> GetAllListOfMirOrdersbyMirId(int[] listofMirIds, string customerName);
   
        Task<IEnumerable<AllOrdersPerMIRIDsDTO>> GetAllListOfMirOrdersbyMirId(int[] listofMirIds, string customerName);

        Task<bool> PreparationOfSchedule ( Ordering orderspreparation);

        Task<IReadOnlyList<GetallApproveDto>> GetAllListForApprovalOfSchedule(bool status);





        Task<PagedList<GetAllListForMoveOrderPaginationDto>> GetAllListForMoveOrderPagination(UserParams userParams);
        Task<PagedList<GetAllListForMoveOrderPaginationDto>> GetAllListForMoveOrderPaginatioOrig(UserParams userParams, string search);

        Task<IReadOnlyList<TotalListOfApprovedPreparedDateDto>> TotalListOfApprovedPreparedDate(string customername, bool status);


        Task<PagedList<ForApprovalMoveOrderPaginationDto>> ForApprovalMoveOrderPagination(UserParams userParams, bool status);
        Task<PagedList<ForApprovalMoveOrderPaginationDto>> ForApprovalMoveOrderPaginationOrig(UserParams userParams, string search, bool status);


        Task<PagedList<ApprovedMoveOrderPaginationDto>> ApprovedMoveOrderPagination(UserParams userParams, bool status);

        Task<PagedList<ApprovedMoveOrderPaginationDto>> ApprovedMoveOrderPaginationOrig(UserParams userParams, string search , bool status);

        Task<PagedList<RejectedMoveOrderPaginationDto>> RejectedMoveOrderPagination(UserParams userParams, bool status);
        Task<PagedList<RejectedMoveOrderPaginationDto>> RejectedMoveOrderPaginationOrig(UserParams userParams, string search, bool status);


    }
}
