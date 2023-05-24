using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
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
    public class ReasonRepository : IReasonRepository
    {
        private new readonly StoreContext _context;

        public ReasonRepository(StoreContext context)
        {
            _context = context;
        }


        public async Task<IReadOnlyList<ReasonDto>> GetAllActiveReason()
        {
            var reason = _context.Reasons.Where(x => x.IsActive == true)
                                         .Select(x => new ReasonDto
                                         {
                                             Id = x.Id, 
                                             ReasonName = x.ReasonName,
                                             MainMenu = x.MainMenu.ModuleName,
                                             MainMenuId = x.MainMenuId,
                                             AddedBy = x.AddedBy,
                                             DateAdded = x.DateAdded.ToString("MM/dd/yyyy"), 
                                             IsActive = x.IsActive 
                                         });

            return await reason.ToListAsync();
        }

        public async Task<IReadOnlyList<ReasonDto>> GetAllInActiveReason()
        {
            var reason = _context.Reasons.Where(x => x.IsActive == false)
                                       .Select(x => new ReasonDto
                                       {
                                           Id = x.Id,
                                           ReasonName = x.ReasonName,
                                           MainMenu = x.MainMenu.ModuleName,
                                           MainMenuId = x.MainMenuId,
                                           AddedBy = x.AddedBy,
                                           DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                           IsActive = x.IsActive
                                       });

            return await reason.ToListAsync();
        }

        public async Task<bool> AddReason(Reason reason)
        {
            await _context.Reasons.AddAsync(reason);

            return true;
        }

        public async Task<bool> UpdateReason(Reason reason)
        {
            var reasons = await _context.Reasons.Where(x => x.Id == reason.Id)
                                                .FirstOrDefaultAsync();

            reasons.ReasonName = reason.ReasonName;
            reasons.MainMenuId = reason.MainMenuId;

            return true;
        }


        public async Task<bool> InActiveReason(Reason reason)
        {
            var reasons = await _context.Reasons.Where(x => x.Id == reason.Id)
                                               .FirstOrDefaultAsync();

            reasons.IsActive = false;

            return true;

        }

        public async Task<bool> ActivateReason(Reason reason)
        {
            var reasons = await _context.Reasons.Where(x => x.Id == reason.Id)
                                              .FirstOrDefaultAsync();
            reasons.IsActive = true;

            return true;
        }

     

        public async Task<PagedList<ReasonDto>> GetAllReasonWithPagination(bool status, UserParams userParams)
        {
            var reasons = _context.Reasons.Where(x => x.IsActive == status)
                                          .OrderByDescending(x => x.DateAdded)
                                         .Select(x => new ReasonDto
                                         {
                                             Id = x.Id,
                                             ReasonName = x.ReasonName,
                                             MainMenu = x.MainMenu.ModuleName,
                                             MainMenuId = x.MainMenuId,
                                             AddedBy = x.AddedBy,
                                             DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                             IsActive = x.IsActive
                                         });


            return await PagedList<ReasonDto>.CreateAsync(reasons, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<ReasonDto>> GetReasonWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var reasons = _context.Reasons.Where(x => x.IsActive == status)
                                          .OrderByDescending(x => x.DateAdded)
                                       .Select(x => new ReasonDto
                                       {
                                           Id = x.Id,
                                           ReasonName = x.ReasonName,
                                           MainMenu = x.MainMenu.ModuleName,
                                           MainMenuId = x.MainMenuId,
                                           AddedBy = x.AddedBy,
                                           DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                           IsActive = x.IsActive
                                       }).Where(x => x.ReasonName.ToLower().Contains(search.Trim().ToLower())
                                          || x.MainMenu.ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<ReasonDto>.CreateAsync(reasons, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> ValidateModuleId(int id)
        {
            var validateExisting = await _context.MainMenus.FindAsync(id);

            if (validateExisting == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateReasonEntry(Reason reason)
        {
            var validate = await _context.Reasons.Where(x => x.MainMenuId == reason.MainMenuId)
                                        .Where(x => x.ReasonName == reason.ReasonName)
                                        .ToListAsync();
            if (validate.Count != 0)
                return false;

            return true;
        }


        public async Task<bool> validateReasonSame(Reason reason)
        {
            var validate = await _context.Reasons.Where(x => x.Id == reason.Id)
                                               .Where(x => x.ReasonName == reason.ReasonName)
                                               .FirstOrDefaultAsync();

            if (validate == null)
            {
                return false;
            }

            return true;
        }

    }
}
