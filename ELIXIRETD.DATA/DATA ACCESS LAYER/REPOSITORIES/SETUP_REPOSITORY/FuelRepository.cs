using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO.FuelDto;
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
    public class FuelRepository : IFuelRepository
    {
        private readonly StoreContext _context;

        public FuelRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> ItemAlreadyExist(int item)
        {

            var validation = await _context.Fuels
                  .FirstOrDefaultAsync(f => f.MaterialId == item);
            
            if(validation is not null) 
                return false;

            return true;
        }

        public async Task<bool> ItemNotExist(int item)
        {
            var validation = await _context.Materials
                  .FirstOrDefaultAsync(f => f.Id == item);

            if (validation is null)
                return false;

            return true;
        }
        public async Task<bool> FuelNotExist(int id)
        {
            var fuelExist = await _context.Fuels
               .FirstOrDefaultAsync(f => f.Id == id);
            if (fuelExist is  null)
                return false;

            return true;
        }

        public async Task<bool> UpsertFuel(UpsertFuelDto fuel)
        {
            
            var fuelExist = await _context.Fuels
                .FirstOrDefaultAsync(f => f.Id == fuel.Id);

            if(fuelExist is not null)
            {
                fuelExist.MaterialId = fuel.MaterialId.Value;
                fuelExist.Updated_At = DateTime.Now;
                fuelExist.Modified_By = fuel.Modified_By;

            }
            else
            {
                var newFuel = new Fuel
                {
                    MaterialId = fuel.MaterialId.Value,
                    Added_By = fuel.Added_By,
                };

                await _context.Fuels.AddRangeAsync(newFuel);
            }

            return true;

        }

        public async Task<PagedList<GetFuelDto>> GetFuel(UserParams userParams, string Search, bool? Status)
        {

            var results = _context.Fuels
                .Include(f => f.Material)
                .Select(f => new GetFuelDto
                {
                    Id = f.Id,
                    MaterialId = f.MaterialId,
                    Item_Code = f.Material.ItemCode,
                    Item_Description = f.Material.ItemDescription,
                    Added_By = f.Added_By,
                    Created_At = f.Created_At,
                    Modified_By = f.Modified_By,
                    Updated_At = f.Updated_At,
                    Is_Active = f.Is_Active,
                });


            if(Status is not null )
                results = results.Where(r => r.Is_Active == Status);

            if(!string.IsNullOrEmpty(Search))
                results = results.Where(r => r.Item_Code.Contains(Search)
                || r.Item_Description.Contains(Search));


            return await PagedList<GetFuelDto>.CreateAsync(results,userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> UpdateFuelStatus(int id)
        {
            var fuelExist = await _context.Fuels
                   .FirstOrDefaultAsync(f => f.Id == id);

            fuelExist.Is_Active = !fuelExist.Is_Active;

            return true;

        }


    }
}
