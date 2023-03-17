using ELIXIRETD.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
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
        Task<bool> AddBorrowedIssue(BorrowedIssue borrowed);
        Task<bool> AddBorrowedIssueDetails(BorrowedIssueDetails borrowed);
        Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllBorrowedReceiptWithPagination(UserParams userParams, bool status);

        Task<PagedList<GetAllBorrowedReceiptWithPaginationDto>> GetAllBorrowedIssuetWithPaginationOrig(UserParams userParams, string search, bool status);

        Task<bool> UpdateIssuePKey(BorrowedIssueDetails borowed);

        Task<bool> InActiveBorrowedIssues (BorrowedIssue borrowed);

        Task<bool> ActiveBorrowedIssues(BorrowedIssue borrowed);

        Task<IReadOnlyList<GetAllDetailsInBorrowedIssueDto>> GetAllDetailsInBorrowedIssue(int id);
        Task<IReadOnlyList<GetAllAvailableBorrowIssueDto>> GetAllAvailableIssue(int empid);

        Task<bool> CancelIssuePerItemCode(BorrowedIssueDetails borrowed);

        

        Task<bool> EditReturnQuantity(BorrowedIssueDetails borrowed);
        Task<bool> SaveReturnedQuantity (BorrowedIssue borrowed);
        Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItem(UserParams userParams);
        Task<PagedList<DtoGetAllReturnedItem>> GetAllReturnedItemOrig(UserParams userParams , string search);

        Task<IReadOnlyList<DtoViewBorrewedReturnedDetails>> ViewBorrewedReturnedDetails(int id);





    }
}
