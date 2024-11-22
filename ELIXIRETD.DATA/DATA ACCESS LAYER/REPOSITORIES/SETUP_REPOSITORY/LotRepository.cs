using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
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

        //---------LOT Section---------------//

        public async Task<IReadOnlyList<LotNameDto>> GetAllActiveLotName()
        {
            var lots = _context.LotSections.Where(x => x.IsActive == true)
                                        .Select(x => new LotNameDto
                                        {
                                            Id = x.Id,
                                            LotNamesId = x.LotNamesId,                                         
                                            LotCode = x.LotNames.LotCode,
                                            LotName = x.LotNames.LotName,
                                            SectionName = x.SectionName,
                                            AddedBy = x.AddedBy,
                                            IsActive = x.IsActive,
                                            DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

                                        });
            return await lots.ToListAsync();
        }

        public async Task<IReadOnlyList<LotNameDto>> GetAllInActiveLotName()
        {
            var lots = _context.LotSections.Where(x => x.IsActive == false)
                                       .Select(x => new LotNameDto
                                       {
                                           Id = x.Id,
                                           LotNamesId = x.LotNamesId,
                                           LotCode = x.LotNames.LotCode,
                                           LotName = x.LotNames.LotName,
                                           SectionName = x.SectionName,
                                           AddedBy = x.AddedBy,
                                           IsActive = x.IsActive,
                                           DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

                                       });
            return await lots.ToListAsync();
        }

        public async Task<bool> AddLotName(LotSection lotname)
        {
            await _context.LotSections.AddAsync(lotname);
            return true;
        }

        public async Task<bool> UpdateLotName(LotSection lotname)
        {
            var lots = await _context.LotSections.Where(x => x.Id == lotname.Id)
                                              .FirstOrDefaultAsync();

            lots.SectionName = lotname.SectionName;
            lots.LotNamesId = lotname.LotNamesId;

            return true;

        }

        public async Task<bool> ActivateLotName(LotSection lotname)
        {
            var lots = await _context.LotSections.Where(x => x.Id == lotname.Id)
                                              .FirstOrDefaultAsync();

            lots.IsActive = true;

            return true;


        }

        public async Task<bool> InActiveLotName(LotSection lotname)
        {
            var lots = await _context.LotSections.Where(x => x.Id == lotname.Id)
                                             .FirstOrDefaultAsync();

            lots.IsActive = false;

            return true;
        }


        public async Task<PagedList<LotNameDto>> GetAllLotNameWithPagination(bool status, UserParams userParams)
        {

            var lots = _context.LotSections.Where(x => x.IsActive == status)
                                        .Select(x => new LotNameDto
                                        {
                                            Id = x.Id,
                                            LotNamesId = x.LotNamesId,
                                            LotCode = x.LotNames.LotCode,
                                            LotName = x.LotNames.LotName,
                                            SectionName = x.SectionName,
                                            AddedBy = x.AddedBy,
                                            IsActive = x.IsActive,
                                            DateAdded = x.DateAdded.ToString("MM/dd/yyyy")
                                        });

            return await PagedList<LotNameDto>.CreateAsync(lots, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<LotNameDto>> GetLotNameWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var lots = _context.LotSections.Where(x => x.IsActive == status)
                                      .Select(x => new LotNameDto
                                      {
                                          Id = x.Id,
                                          LotNamesId = x.LotNamesId,
                                          LotCode = x.LotNames.LotCode,
                                          LotName = x.LotNames.LotName,
                                          SectionName = x.SectionName,
                                          AddedBy = x.AddedBy,
                                          IsActive = x.IsActive,
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

                                      }).Where(x => x.SectionName.ToLower().Contains(search.Trim().ToLower())
                                      || x.LotName.ToLower().Contains(search.Trim().ToLower())
                                       || x.LotCode.ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<LotNameDto>.CreateAsync(lots, userParams.PageNumber, userParams.PageSize);

        }


        //----------LOT Name----------------//


        public async Task<IReadOnlyList<LotCategoryDto>> GetAllActiveLotCategories()
        {
            var category = _context.Lotnames.Where(x => x.IsActive == true)
                                                 .Select(x => new LotCategoryDto
                                                 {
                                                     Id = x.Id,
                                                     LotCode = x.LotCode,
                                                     LotName = x.LotName,
                                                     AddedBy = x.AddedBy,
                                                     DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                     IsActive = x.IsActive
                                                 });

            return await category.ToListAsync();

        }

        public async Task<IReadOnlyList<LotCategoryDto>> GetAllInActiveLotCategories()
        {
            var category = _context.Lotnames.Where(x => x.IsActive == false)
                                               .Select(x => new LotCategoryDto
                                               {
                                                   Id = x.Id,
                                                   LotCode = x.LotCode,
                                                   LotName = x.LotName,
                                                   AddedBy = x.AddedBy,
                                                   DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                   IsActive = x.IsActive
                                               });

            return await category.ToListAsync();

        }

        public async Task<bool> AddLotCategory(LotNames lotname)
        {
            await _context.Lotnames.AddAsync(lotname);

            return true;

        }

        public async Task<bool> UpdateLotCategory(LotNames lotname)
        {
            var category = await _context.Lotnames.Where(x => x.Id == lotname.Id)
                                                       .FirstOrDefaultAsync();

            category.LotName = lotname.LotName;

            return true;


        }

        public async Task<bool> InActiveLotCategory(LotNames lotname)
        {
            var category = await _context.Lotnames.Where(x => x.Id == lotname.Id)
                                                      .FirstOrDefaultAsync();

            category.IsActive = false;

            return true;

        }

        public async Task<bool> ActivateLotCategory(LotNames lotname)
        {
            var category = await _context.Lotnames.Where(x => x.Id == lotname.Id)
                                                     .FirstOrDefaultAsync();

            category.IsActive = true;

            return true;
        }

        public async Task<PagedList<LotCategoryDto>> GetAllLotCategoryWithPagination(bool status, UserParams userParams)
        {
            var lots = _context.Lotnames.Where(x => x.IsActive == status)
                                             .OrderByDescending(x => x.DateAdded)
                                    .Select(x => new LotCategoryDto
                                    {
                                        Id = x.Id,
                                        LotCode = x.LotCode,
                                        LotName = x.LotName,
                                        AddedBy = x.AddedBy,
                                        IsActive = x.IsActive,
                                        DateAdded = x.DateAdded.ToString("MM/dd/yyyy")
                                    });


            return await PagedList<LotCategoryDto>.CreateAsync(lots, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<LotCategoryDto>> GetLotCategoryWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var lots = _context.Lotnames.Where(x => x.IsActive == status)
                                             .OrderByDescending(x => x.DateAdded)
                                    .Select(x => new LotCategoryDto
                                    {
                                        Id = x.Id,
                                        LotCode = x.LotCode,
                                        LotName = x.LotName,
                                        AddedBy = x.AddedBy,
                                        IsActive = x.IsActive,
                                        DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

                                    }).Where(x => x.LotName.ToLower().Contains(search.Trim().ToLower())
                                     || x.LotCode.ToLower().Contains(search.Trim().ToLower()));


            return await PagedList<LotCategoryDto>.CreateAsync(lots, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> ValidateLotCategoryId(int id)
        {
            var validateExisting = await _context.Lotnames.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> SectionNameExist(string section)
        {
            return await _context.LotSections.AnyAsync(x => x.SectionName == section);
        }

        public async Task<bool> ValidateLotNameAndSection(LotSection lot)
        {
            var validate = await _context.LotSections.Where(x => x.LotNamesId == lot.LotNamesId)
                                                .Where(x => x.SectionName == lot.SectionName)
                                                .Where(x => x.IsActive == true)
                                                .FirstOrDefaultAsync();

            if (validate == null)
                return false;

            return true;

        }

        public async Task<bool> LotCategoryNameExist(string name)
        {
            return await _context.Lotnames.AnyAsync(x => x.LotName == name);
        }

        public async Task<bool> ValidateLotCode(string lotcode)
        {
            return await _context.Lotnames.AnyAsync(x => x.LotCode == lotcode);
                                                 
     
        }

        public async Task<bool> ValidateLotInUse(int id)
        {
            return await _context.LotSections.AnyAsync(x => x.LotNamesId == id && x.IsActive == true);


        }

        public async Task<bool> ValidateLotNameSame(LotNames lotname)
        {
            var validatelot = await _context.Lotnames.Where(x => x.Id == lotname.Id && x.LotName == lotname.LotName)
                                                      .FirstOrDefaultAsync();

            if (validatelot == null) 
                return false;

            return true;
        }

        public async Task<bool> ValidateLotSectionSame(LotSection section)
        {
            var validate = await _context.LotSections.Where(x => x.Id == section.Id && x.SectionName == section.SectionName/* && x.LotNamesId == section.LotNamesId*/)
                                                     .FirstOrDefaultAsync();

            if (validate == null) 
                return false;

            return true;
        }
    }
}
