using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface ILocationRepository
    {
        Task<IReadOnlyList<LocationDto>> GetAllActiveLocation();
        Task<IReadOnlyList<LocationDto>> GetAllInActiveLocation();
        Task<bool> AddLocation(Location lcoation);
        Task<bool> UpdateLocation(Location lcoation);
        Task<bool> InActiveLocation(Location lcoation);
        Task<bool> ActivateLocation(Location lcoation);
        Task<PagedList<LocationDto>> GetLocationWithPagination(bool status, UserParams userParams);
        Task<PagedList<LocationDto>> GetLocationWithPaginationOrig(UserParams userParams, bool status, string search);

        Task<bool> LocationCodeExist(string location);





    }
}
