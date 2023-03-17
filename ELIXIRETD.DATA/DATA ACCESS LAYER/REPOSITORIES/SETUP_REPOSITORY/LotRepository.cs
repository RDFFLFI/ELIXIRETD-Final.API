using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class LotRepository : ILotRepository
    {
        private new readonly StoreContext _context;

        public LotRepository(StoreContext context)
        {
            _context = context;
        }

        //---------LOT NAME---------------//

        public async Task<IReadOnlyList<LotNameDto>> GetAllActiveLotName()
        {
            var lots = _context.LotNames.Where(x => x.IsActive == true)
                                        .Select(x => new LotNameDto
                                        {
                                            Id = x.Id,
                                            LotCategoryId = x.LotCategoryId,
                                            LotCategory = x.LotCategory.LotName,
                                            LotNameCode = x.LotNameCode,
                                            SectionName = x.SectionName,
                                            AddedBy = x.AddedBy,
                                            IsActive = x.IsActive,
                                            DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

                                        });
            return await lots.ToListAsync();
        }

        public async Task<IReadOnlyList<LotNameDto>> GetAllInActiveLotName()
        {
            var lots = _context.LotNames.Where(x => x.IsActive == false)
                                       .Select(x => new LotNameDto
                                       {
                                           Id = x.Id,
                                           LotCategoryId = x.LotCategoryId,
                                           LotCategory = x.LotCategory.LotName,
                                           LotNameCode = x.LotNameCode,
                                           SectionName = x.SectionName,
                                           AddedBy = x.AddedBy,
                                           IsActive = x.IsActive,
                                           DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

                                       });
            return await lots.ToListAsync();
        }

        public async Task<bool> AddLotName(LotName lotname)
        {
            await _context.LotNames.AddAsync(lotname);
            return true;
        }

        public async Task<bool> UpdateLotName(LotName lotname)
        {
            var lots = await _context.LotNames.Where(x => x.Id == lotname.Id)
                                              .FirstOrDefaultAsync();

            lots.SectionName = lotname.SectionName;
            lots.LotCategoryId = lotname.LotCategoryId;

            return true;

        }

        public async Task<bool> ActivateLotName(LotName lotname)
        {
            var lots = await _context.LotNames.Where(x => x.Id == lotname.Id)
                                              .FirstOrDefaultAsync();

            lots.IsActive = true;

            return true;
        }

        public async Task<bool> InActiveLotName(LotName lotname)
        {
            var lots = await _context.LotNames.Where(x => x.Id == lotname.Id)
                                             .FirstOrDefaultAsync();

            lots.IsActive = false;

            return true;
        }


        public async Task<PagedList<LotNameDto>> GetAllLotNameWithPagination(bool status, UserParams userParams)
        {

            var lots = _context.LotNames.Where(x => x.IsActive == status)
                                        .Select(x => new LotNameDto
                                        {
                                            Id = x.Id,
                                            LotCategoryId = x.LotCategoryId,
                                            LotCategory = x.LotCategory.LotName,
                                            LotNameCode = x.LotNameCode,
                                            SectionName = x.SectionName,
                                            AddedBy = x.AddedBy,
                                            IsActive = x.IsActive,
                                            DateAdded = x.DateAdded.ToString("MM/dd/yyyy")
                                        });

            return await PagedList<LotNameDto>.CreateAsync(lots, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<LotNameDto>> GetLotNameWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var lots = _context.LotNames.Where(x => x.IsActive == status)
                                      .Select(x => new LotNameDto
                                      {
                                          Id = x.Id,
                                          LotCategoryId = x.LotCategoryId,
                                          LotCategory = x.LotCategory.LotName,
                                          LotNameCode = x.LotNameCode,
                                          SectionName = x.SectionName,
                                          AddedBy = x.AddedBy,
                                          IsActive = x.IsActive,
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy")
                                      }).Where(x => x.SectionName.ToLower()
                                        .Contains(search.Trim().ToLower()));

            return await PagedList<LotNameDto>.CreateAsync(lots, userParams.PageNumber, userParams.PageSize);

        }












        //----------LOT CATEGORY----------------//


        public async Task<IReadOnlyList<LotCategoryDto>> GetAllActiveLotCategories()
        {
            var category = _context.LotCategories.Where(x => x.IsActive == true)
                                                 .Select(x => new LotCategoryDto
                                                 {
                                                     Id = x.Id,
                                                     LotCategoryName = x.LotName,
                                                     AddedBy = x.AddedBy,
                                                     DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                     IsActive = x.IsActive
                                                 });

            return await category.ToListAsync();

        }

        public async Task<IReadOnlyList<LotCategoryDto>> GetAllInActiveLotCategories()
        {
            var category = _context.LotCategories.Where(x => x.IsActive == false)
                                               .Select(x => new LotCategoryDto
                                               {
                                                   Id = x.Id,                         
                                                   LotCategoryName = x.LotName,
                                                   AddedBy = x.AddedBy,
                                                   DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                   IsActive = x.IsActive
                                               });

            return await category.ToListAsync();

        }

        public async Task<bool> AddLotCategory(LotCategory lotname)
        {
            await _context.LotCategories.AddAsync(lotname);

            return true;

        }

        public async Task<bool> UpdateLotCategory(LotCategory lotname)
        {
            var category = await _context.LotCategories.Where(x => x.Id == lotname.Id)
                                                       .FirstOrDefaultAsync();

            category.LotName = lotname.LotName;

            return true;


        }

        public async Task<bool> InActiveLotCategory(LotCategory lotname)
        {
            var category = await _context.LotCategories.Where(x => x.Id == lotname.Id)
                                                      .FirstOrDefaultAsync();

            category.IsActive = false;

            return true;

        }

        public async Task<bool> ActivateLotCategory(LotCategory lotname)
        {
            var category = await _context.LotCategories.Where(x => x.Id == lotname.Id)
                                                     .FirstOrDefaultAsync();

            category.IsActive = true;

            return true;
        }

        public async Task<PagedList<LotCategoryDto>> GetAllLotCategoryWithPagination(bool status, UserParams userParams)
        {
            var lots = _context.LotCategories.Where(x => x.IsActive == status)
                                             .OrderByDescending(x => x.DateAdded)
                                    .Select(x => new LotCategoryDto
                                    {
                                        Id = x.Id,
                                        LotCategoryName = x.LotName,
                                        AddedBy = x.AddedBy,
                                        IsActive = x.IsActive,
                                        DateAdded = x.DateAdded.ToString("MM/dd/yyyy")
                                    });


            return await PagedList<LotCategoryDto>.CreateAsync(lots, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<LotCategoryDto>> GetLotCategoryWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var lots = _context.LotCategories.Where(x => x.IsActive == status)
                                             .OrderByDescending(x => x.DateAdded)
                                    .Select(x => new LotCategoryDto
                                    {
                                        Id = x.Id,
                                        LotCategoryName = x.LotName,
                                        AddedBy = x.AddedBy,
                                        IsActive = x.IsActive,
                                        DateAdded = x.DateAdded.ToString("MM/dd/yyyy")
                                    }).Where(x => x.LotCategoryName.ToLower()
                                      .Contains(search.Trim().ToLower()));


            return await PagedList<LotCategoryDto>.CreateAsync(lots, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> ValidateLotCategoryId(int id)
        {
            var validateExisting = await _context.LotCategories.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> SectionNameExist(string section)
        {
            return await _context.LotNames.AnyAsync(x => x.SectionName == section);
        }

        public async Task<bool> ValidateLotNameAndSection(LotName lot)
        {
            var validate = await _context.LotNames.Where(x => x.LotCategoryId == lot.LotCategoryId)
                                                .Where(x => x.SectionName == lot.SectionName)
                                                .Where(x => x.IsActive == true)
                                                .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;

        }

        public async Task<bool> LotCategoryNameExist(string name)
        {
            return await _context.LotCategories.AnyAsync(x => x.LotName == name);
        }
    }
}
