using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.FUEL_REGISTER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.FUEL_REGISTER_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.FUEL_REGISTER_INTERFACE
{
    public interface IFuelRegisterRepository
    {
        Task<FuelRegister> CreateFuelRegister(CreateFuelRegisterDto fuel);

        Task<bool> CreateFuelRegisterDetails(CreateFuelRegisterDetailsDto fuel);

        Task<IReadOnlyList<GetMaterialByStocksDto>> GetMaterialByStocks();

        Task<IReadOnlyList<GetMaterialStockByWarehouseDto>> GetMaterialStockByWarehouse();

        Task<PagedList<GetFuelRegisterDto>> GetFuelRegister(UserParams userParams, string Search, string Status , string UserId);

        Task<bool> ApproveFuel(ApproveFuelDto fuel);

        Task<bool> RejectFuel(RejectFuelDto fuel);
        Task<bool> TransactFuel(TransactedFuelDto fuel);

        Task<bool> CancelFuel(int id);

        Task<bool> MaterialNotExist(int id);

        Task<bool> WarehouseNotExist(int id);

        Task<bool> FuelRegisterNotExist(int id);

        Task<IReadOnlyList<GetDriverUserDto>> GetDriverUser();

        Task<IReadOnlyList<GetForApprovalFuelDto>> GetForApprovalFuel();



    }
}
