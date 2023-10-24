using ELIXIRETD.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO.BorrowedNotification;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.BORROWED_INTERFACE
{
    public interface IBorrowedItem
    {

        Task<IReadOnlyList<GetAvailableStocksForBorrowedIssue_Dto>> GetAvailableStocksForBorrowedIssue(string itemcode);

        Task<IReadOnlyList<GetAvailableStocksForBorrowedIssue_Dto>> GetAvailableStocksForBorrowedIssueNoParameters();

        Task<bool> AddBorrowedIssue(BorrowedIssue borrowed);
        Task<bool> AddBorrowedIssueDetails(BorrowedIssueDetails borrowed);
        Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllBorrowedReceiptWithPagination(UserParams userParams, bool status, /*string status,*/ int empid);

        Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllBorrowedIssuetWithPaginationOrig(UserParams userParams, string search, bool status, int empid);

        Task<bool> UpdateIssuePKey(BorrowedIssueDetails borowed);

        Task<bool> Cancelborrowedfortransact(BorrowedIssueDetails borrowed);

        Task<bool> CancelAllborrowedfortransact(BorrowedIssueDetails borrowed);


        Task<IReadOnlyList<GetAllDetailsInBorrowedIssueDto>> GetAllDetailsInBorrowedIssue(int id);
        Task<IReadOnlyList<GetAllAvailableBorrowIssueDto>> GetAllAvailableIssue(int empid);

        //Task<IReadOnlyList<DtoGetForBorrowedPrint>> GetForBorrowedPrint(int id);


        Task<IReadOnlyList<DTOGetItemForReturned>> GetItemForReturned(int id);


        Task<bool> EditReturnQuantity(BorrowedConsume consumes);

        Task<bool> AddBorrowConsume(BorrowedConsume consumes);

        Task<bool> CancelIssuePerItemCode(BorrowedConsume consumes);

        Task<bool> EditConsumeQuantity(BorrowedConsume consumes);

        Task<bool> ResetConsumePerItemCode (BorrowedConsume consumes);

        Task<bool> SaveReturnedQuantity(BorrowedIssueDetails borrowed);

        Task<IReadOnlyList<DtoGetConsumedItem>> GetConsumedItem(int id);




        Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItem(UserParams userParams, bool status, int empid);
        Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItemOrig(UserParams userParams, string search, bool status, int empid);

        Task<bool> CancelReturnItem(BorrowedIssueDetails borrowed);

        Task<bool> CancelAllConsumeItem(BorrowedConsume consume);

        Task<IReadOnlyList<DtoViewBorrewedReturnedDetails>> ViewBorrewedReturnedDetails(int id);

        Task<IReadOnlyList<DtoViewConsumeForReturn>> ViewConsumeForReturn(int id);





        // =========================================================== Updated Borrowed =======================================================================


        Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllForApprovalBorrowedWithPagination(UserParams userParams, bool status);

        Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllForApprovalBorrowedWithPaginationOrig(UserParams userParams, string search, bool status);

        Task<IReadOnlyList<GetAllDetailsInBorrowedIssueDto>> GetAllForApprovalDetailsInBorrowed(int id);

        Task<bool> ApprovedForBorrowed(BorrowedIssue borrowed);

        Task<bool> RejectForBorrowed(BorrowedIssue borrowed);


        Task<PagedList<GetRejectBorrowedPagination>> GetAllRejectBorrowedWithPagination(UserParams userParams);

        Task<PagedList<GetRejectBorrowedPagination>> GetAllRejectBorrowedWithPaginationOrig(UserParams userParams, string search);

        Task<PagedList<GetRejectBorrowedPagination>> GetAllRejectBorrowedWithPaginationCustomer(UserParams userParams, int empid);

        Task<PagedList<GetRejectBorrowedPagination>> GetAllRejectBorrowedWithPaginationCustomerOrig(UserParams userParams, string search, int empid);


        //Task<PagedList<GetRejectBorrowedPagination>> GetAllApprovedBorrowedWithPagination(UserParams userParams, bool status);

        //Task<PagedList<GetRejectBorrowedPagination>> GetAllApprovedBorrowedWithPaginationOrig(UserParams userParams, string search, bool status);

        Task<PagedList<DtoGetAllReturnedItem>> GetAllForApproveReturnedItem(UserParams userParams, bool status);
        Task<PagedList<DtoGetAllReturnedItem>> GetAllForApproveReturnedItemOrig(UserParams userParams, string search, bool status);

        Task<bool> ApproveForReturned(BorrowedIssue borrowed);

        Task<bool> CancelForReturned(BorrowedIssue borrowed);

        //Task<PagedList<GetAllDetailsBorrowedTransactionDto>> GetAllDetailsBorrowedTransaction(UserParams userParams);
        //Task<PagedList<GetAllDetailsBorrowedTransactionDto>> GetAllDetailsBorrowedTransactionOrig(UserParams userParams, string search);


        //============================================= Notification ========================================


        Task<IReadOnlyList<GetNotificationForBorrowedApprovalDto>> GetNotificationForBorrowedApproval();

        Task<IReadOnlyList<GetNotificationForReturnedApprovalDto>> GetNotificationForReturnedApproval();

        Task<IReadOnlyList<RejectBorrowedNotificationDto>> RejectBorrowedNotification();

        Task<IReadOnlyList<GetNotificationForBorrowedApprovalDto>> GetNotificationAllBorrowedNoParameters();




        Task<IReadOnlyList<GetNotificationForBorrowedApprovalDto>> GetNotificationBorrowedApprove(int empid);

        Task<IReadOnlyList<GetNotificationForReturnedApprovalDto>> GetNotificationReturnedApprove(int empid);

        Task<IReadOnlyList<RejectBorrowedNotificationDto>> RejectBorrowedNotificationWithParameter(int empid);

        Task<IReadOnlyList<GetNotificationForBorrowedApprovalDto>> GetNotificationAllBorrowed(int empid);





        // Update In Borrowed


        Task<bool> CancelAllBorrowed(BorrowedIssue borrowed);
        Task<bool> AddPendingBorrowedItem(BorrowedIssueDetails borrow);

        Task<bool> CloseSaveBorrowed(BorrowedIssueDetails borrow);

        Task<bool> EditBorrowedQuantity(BorrowedIssueDetails borrow);

        Task<bool> EditBorrowedIssue(BorrowedIssue borrow);


        Task<bool> CancelUpdateBorrowed(BorrowedIssueDetails borrowed);


        Task<IReadOnlyList<DtoViewBorrewedReturnedDetails>> ViewAllBorrowedDetails(int id);

     
        Task<IReadOnlyList<GetAllAvailableBorrowIssueDto>> GetTransactedBorrowedDetails(int empid);


    }
}
