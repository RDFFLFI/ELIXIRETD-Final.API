using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO.FuelDto;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface IFuelRepository
    {
        Task<bool> UpsertFuel(UpsertFuelDto fuel);
        Task<PagedList<GetFuelDto>> GetFuel(UserParams userParams, string Search, bool? Status);
        Task<bool> UpdateFuelStatus(int id);



        Task<bool> FuelNotExist(int id);
        Task<bool> ItemAlreadyExist(int item);
        Task<bool> ItemNotExist(int item);
    }
}
