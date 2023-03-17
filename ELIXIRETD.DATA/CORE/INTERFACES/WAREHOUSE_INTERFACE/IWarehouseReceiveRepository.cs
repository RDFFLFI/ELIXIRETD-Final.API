using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.WAREHOUSE_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE
{
    public interface IWarehouseReceiveRepository
    {

        Task<bool> AddNewReceivingDetails(Warehouse_Receiving receive);
        Task<bool> CancelPo(PoSummary summary);
        Task<bool> ReturnPoInAvailableList(PoSummary summary);
        Task<PagedList<WarehouseReceivingDto>> GetAllPoSummaryWithPagination(UserParams userParams);
        Task<PagedList<WarehouseReceivingDto>> GetPoSummaryByStatusWithPaginationOrig(UserParams userParams, string search);
        Task<PagedList<CancelledPoDto>> GetAllCancelledPOWithPagination(UserParams userParams);
        Task<PagedList<CancelledPoDto>> GetAllCancelledPOWithPaginationOrig(UserParams userParams, string search);

        Task<PagedList<RejectWarehouseReceivingDto>> RejectRawMaterialsByWarehousePagination(UserParams userParams);
        Task<PagedList<RejectWarehouseReceivingDto>> RejectRawMaterialsByWarehousePaginationOrig(UserParams userParams, string search);

        Task<bool> ValidatePoId(int id);
        Task<bool> ValidateActualRemaining(Warehouse_Receiving receiving);


        Task<PagedList<WarehouseReceivingDto>> ListOfWarehouseReceivingIdWithPagination(UserParams userParams);
        Task<PagedList<WarehouseReceivingDto>> ListOfWarehouseReceivingIdWithPaginationOrig(UserParams userParams, string search);

        Task<IReadOnlyList<ListofwarehouseReceivingIdDto>> ListOfWarehouseReceivingId(string search);

        Task<IReadOnlyList<ListofwarehouseReceivingIdDto>> ListOfWarehouseReceivingId();

    }
}
