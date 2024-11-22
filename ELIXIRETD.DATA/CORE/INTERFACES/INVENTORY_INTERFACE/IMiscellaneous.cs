using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.INVENTORY_INTERFACE
{
    public interface IMiscellaneous
    {

        Task<bool> AddMiscellaneousReceipt(MiscellaneousReceipt receipt);
        Task<bool> AddMiscellaneousReceiptInWarehouse(Warehouse_Receiving receive);
        Task<bool> InActiveMiscellaneousReceipt(MiscellaneousReceipt receipt);
        Task<bool> ActivateMiscellaenousReceipt (MiscellaneousReceipt receipt);
        Task<PagedList<GetAllMReceiptWithPaginationdDto>> GetAllMReceiptWithPaginationd(UserParams userParams, bool status);

        Task<IReadOnlyList<MiscReceiptItemListDto>> MiscReceiptItemList();

        Task<PagedList<GetAllMReceiptWithPaginationdDto>> GetAllMReceiptWithPaginationOrig(UserParams userParams, string search, bool status);

        Task<IReadOnlyList<GetWarehouseDetailsByMReceiptDto>> GetWarehouseDetailsByMReceipt(int id);

        Task<bool> AddMiscellaneousIssue(MiscellaneousIssue issue);
        Task<bool> AddMiscellaneousIssueDetails(MiscellaneousIssueDetails details);

        Task<bool> UpdateIssuePKey(MiscellaneousIssueDetails details);

        Task<IReadOnlyList<GetAvailableStocksForIssueDto>> GetAvailableStocksForIssue(string itemcode);

        Task<IReadOnlyList<GetAvailableStocksForIssueDto>> GetAvailableStocksForIssueNoParameters();


        Task<PagedList<GetAllMIssueWithPaginationDto>> GetAllMIssueWithPagination(UserParams userParams, bool status);
        Task<PagedList<GetAllMIssueWithPaginationDto>> GetAllMIssueWithPaginationOrig(UserParams userParams, string search, bool status);
        Task<bool> InActivateMiscellaenousIssue (MiscellaneousIssue issue);

        Task<bool> ActivateMiscellaenousIssue (MiscellaneousIssue issue);
        Task<IReadOnlyList<GetAllDetailsInMiscellaneousIssueDto>> GetAllDetailsInMiscellaneousIssue(int id);

        Task<IReadOnlyList<GetAllAvailableIssueDto>> GetAllAvailableIssue(int empid);

        Task<bool> CancelItemCodeInMiscellaneousIssue(MiscellaneousIssueDetails issue);








       //================================ Validation ====================================================

       Task<bool> ValidateMiscellaneousInIssue (MiscellaneousReceipt receipt);


    }
}
