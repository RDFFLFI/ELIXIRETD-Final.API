using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class UomRepository : IUomRepository
    {
        private new readonly StoreContext _context;

        public UomRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<UomDto>> GetAllActiveUoms()
        {
            var uoms = _context.Uoms.Where(x => x.IsActive == true)
                                    .Select(x => new UomDto
                                    {
                                        Id = x.Id, 
                                        UomCode = x.UomCode, 
                                        UomDescription = x.UomDescription,
                                        IsActive = x.IsActive, 
                                        DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                        AddedBy = x.AddedBy
                                    });

            return await uoms.ToListAsync();
        }

        public async Task<IReadOnlyList<UomDto>> GetAllInActiveUoms()
        {
            var uoms = _context.Uoms.Where(x => x.IsActive == false)
                                  .Select(x => new UomDto
                                  {
                                      Id = x.Id,
                                      UomCode = x.UomCode,
                                      UomDescription = x.UomDescription,
                                      IsActive = x.IsActive,
                                      DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                      AddedBy = x.AddedBy
                                  });

            return await uoms.ToListAsync();
        }

        public async Task<bool> ActivateUom(Uom uoms)
        {
            var existingUom = await _context.Uoms.Where(x => x.Id == uoms.Id)
                                                 .FirstOrDefaultAsync();

            existingUom.IsActive = true;
 
            return true;
        }


        public async Task<bool> InActiveUom(Uom uoms)
        {
            var existingUom = await _context.Uoms.Where(x => x.Id == uoms.Id)
                                                .FirstOrDefaultAsync();

            existingUom.IsActive = false;

            return true;
        }


        public async Task<bool> AddNewUom(Uom uoms)
        {
            await _context.AddAsync(uoms);

            return true;
        }

        public async Task<bool> UpdateUom(Uom uoms)
        {
            var existingUom = await _context.Uoms.Where(x => x.Id == uoms.Id)
                                                 .FirstOrDefaultAsync();

            existingUom.UomDescription = uoms.UomDescription;

            return true;
        }

        public async Task<PagedList<UomDto>> GetAllUomWithPagination(bool status, UserParams userParams)
        {
            var uom = _context.Uoms.Where(x => x.IsActive == status)
                                   .OrderByDescending(x => x.DateAdded)
                                 .Select(x => new UomDto
                                 {
                                     Id = x.Id,
                                     UomCode = x.UomCode,
                                     UomDescription = x.UomDescription,
                                     AddedBy = x.AddedBy,
                                     DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                     IsActive = x.IsActive

                                 });

            return await PagedList<UomDto>.CreateAsync(uom, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<UomDto>> GetUomWithPaginationOrig(UserParams userParams, bool status, string search)
        {

            var role = _context.Uoms.Where(x => x.IsActive == status)
                                    .OrderByDescending(x => x.DateAdded)
                                   .Select(x => new UomDto
                                   {
                                       Id = x.Id,
                                       UomCode = x.UomCode,
                                       UomDescription = x.UomDescription,
                                       AddedBy = x.AddedBy,
                                       DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                       IsActive = x.IsActive

                                   }).Where(x => x.UomDescription.ToLower()
                                     .Contains(search.Trim().ToLower()));

            return await PagedList<UomDto>.CreateAsync(role, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> UomCodeExist(string uom)
        {
            return await _context.Uoms.AnyAsync(x => x.UomCode == uom);
        }

        public async Task<bool> UomDescriptionExist(string uom)
        {
            return await _context.Uoms.AnyAsync(x => x.UomDescription == uom);
        }
    }
}
