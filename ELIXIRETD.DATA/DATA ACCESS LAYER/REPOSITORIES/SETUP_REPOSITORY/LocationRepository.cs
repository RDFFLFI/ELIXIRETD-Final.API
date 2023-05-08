using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class LocationRepository : ILocationRepository
    {
        private new readonly StoreContext _context;
        public LocationRepository(StoreContext context)
        {
            _context = context;
        }


        public async Task<IReadOnlyList<LocationDto>> GetAllActiveLocation()
        {
            var location = _context.Locations.Where(x => x.IsActive == true)
                                             .Select(x => new LocationDto
                                             {
                                                 Id = x.Id,
                                                 LocationCode = x.LocationCode, 
                                                 LocationName = x.LocationName, 
                                                 AddedBy = x.AddedBy, 
                                                 IsActive = x.IsActive, 
                                                 DateAdded = x.DateAdded.ToString("MM/dd/yyyy")
                                             });

            return await location.ToListAsync();
        }

        public async Task<IReadOnlyList<LocationDto>> GetAllInActiveLocation()
        {
            var location = _context.Locations.Where(x => x.IsActive == false)
                                            .Select(x => new LocationDto
                                            {
                                                Id = x.Id,
                                                LocationCode = x.LocationCode,
                                                LocationName = x.LocationName,
                                                AddedBy = x.AddedBy,
                                                IsActive = x.IsActive,
                                                DateAdded = x.DateAdded.ToString("MM/dd/yyyy")
                                            });

            return await location.ToListAsync();
        }



        public async Task<bool> ActivateLocation(Location location)
        {
            var locations = await _context.Locations.Where(x => x.Id == location.Id)
                                                    .FirstOrDefaultAsync();

            locations.IsActive = true;

            return true;


        }

        public async Task<bool> InActiveLocation(Location location)
        {
            var locations = await _context.Locations.Where(x => x.Id == location.Id)
                                                 .FirstOrDefaultAsync();

     

            locations.IsActive = false;

            return true;
        }

        public async Task<bool> AddLocation(Location location)
        {
            await _context.Locations.AddAsync(location);
            return true;
        }



        public async Task<bool> UpdateLocation(Location location)
        {
            var locations = await _context.Locations.Where(x => x.Id == location.Id)
                                              .FirstOrDefaultAsync();

            locations.LocationName = location.LocationName;

            return true;

        }

   
        public async Task<PagedList<LocationDto>> GetLocationWithPagination(bool status, UserParams userParams)
        {
            var location = _context.Locations.Where(x => x.IsActive == status)
                                             .OrderByDescending(x => x.DateAdded)
                                           .Select(x => new LocationDto
                                           {
                                               Id = x.Id,
                                               LocationCode = x.LocationCode,
                                               LocationName = x.LocationName,
                                               AddedBy = x.AddedBy,
                                               DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                               IsActive = x.IsActive
                                           });

            return await PagedList<LocationDto>.CreateAsync(location, userParams.PageNumber, userParams.PageSize);
 
        }

        public async Task<PagedList<LocationDto>> GetLocationWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var location = _context.Locations.Where(x => x.IsActive == status)
                                             .OrderByDescending(x => x.DateAdded)
                                        .Select(x => new LocationDto
                                        {
                                            Id = x.Id,
                                            LocationCode = x.LocationCode,
                                            LocationName = x.LocationName,
                                            AddedBy = x.AddedBy,
                                            DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                            IsActive = x.IsActive
                                        }).Where(x => x.LocationName.ToLower()
                                          .Contains(search.Trim().ToLower()));

            return await PagedList<LocationDto>.CreateAsync(location, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> LocationCodeExist(string location)
        {
            return await _context.Locations.AnyAsync(x => x.LocationCode == location);
        }

       
    }
}
