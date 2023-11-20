using ELIXIRETD.DATA.CORE.INTERFACES.USER_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES
{
    public class ModuleRepository : IModuleRepository
    {
        private new readonly StoreContext _context;

        public ModuleRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ModuleDto>> GetAllActiveModules()
        {
            var module = _context.Modules.Where(x => x.IsActive == true)
                                         .Select(x => new ModuleDto
                                         {
                                             Id = x.Id,
                                             MainMenu = x.MainMenu.ModuleName,
                                             MainMenuId = x.MainMenuId,
                                             SubMenuName = x.SubMenuName,
                                             ModuleName = x.ModuleName,
                                             DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                             AddedBy = x.AddedBy,
                                             IsActive = x.MainMenu.IsActive,
                                             Reason = x.Reason

                                         });

            return await module.ToListAsync();

        }

        public async Task<IReadOnlyList<ModuleDto>> GetAllInActiveModules()
        {
            var module = _context.Modules.Where(x => x.IsActive == false)
                                        .Select(x => new ModuleDto
                                        {
                                            Id = x.Id,
                                            MainMenu = x.MainMenu.ModuleName,
                                            MainMenuId = x.MainMenuId,
                                            SubMenuName = x.SubMenuName,
                                            ModuleName = x.ModuleName,
                                            DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                            AddedBy = x.AddedBy,
                                            IsActive = x.MainMenu.IsActive,
                                            Reason = x.Reason
                                        });

            return await module.ToListAsync();

        }


        public async Task<bool> AddNewModule(Module module)
        {
            await _context.Modules.AddAsync(module);

            return true;
        }


        public async Task<bool> UpdateModule(Module module)
        {
            var existingModule = await _context.Modules.Where(x => x.Id == module.Id)
                                                       .FirstOrDefaultAsync();

            existingModule.MainMenuId = module.MainMenuId;
            existingModule.SubMenuName = module.SubMenuName;
            existingModule.ModuleName = module.ModuleName;

            return true;

        }

        public async Task<bool> CheckMainMenu(int id)
        {
            var mainMenuResult = await _context.MainMenus.FindAsync(id);

            if (mainMenuResult == null)
                return false;
            return true;
        }

        public async Task<bool> SubMenuNameExist(string module , int mainmenu)
        {
            return await _context.Modules.AnyAsync(x => x.SubMenuName == module && x.MainMenuId == mainmenu);
        }

        public async Task<bool> ModuleNameExist(string module)
        {
            return await _context.Modules.AnyAsync(x => x.ModuleName == module);
        }

        public async Task<bool> InActiveModule(Module module)
        {
            var existingModule = await _context.Modules.Where(x => x.Id == module.Id)
                                                       .FirstOrDefaultAsync();

            existingModule.IsActive = false;

            return true;
        }

        public async Task<bool> ActivateModule(Module module)
        {
            var existingModule = await _context.Modules.Where(x => x.Id == module.Id)
                                                       .FirstOrDefaultAsync();

            existingModule.IsActive = true;

            return true;
        }

        public async Task<PagedList<ModuleDto>> GetAllModulessWithPagination(bool status, UserParams userParams)
        {
            var modules = _context.Modules.Where(x => x.IsActive == status)
                                          .OrderByDescending(x => x.DateAdded)
                                          .Select(x => new ModuleDto
                                          {
                                              Id = x.Id,
                                              MainMenu = x.MainMenu.ModuleName,
                                              MainMenuId = x.MainMenu.Id,
                                              SubMenuName = x.SubMenuName,
                                              ModuleName = x.ModuleName,
                                              DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                              AddedBy = x.AddedBy,
                                              IsActive = x.IsActive

                                          });

            return await PagedList<ModuleDto>.CreateAsync(modules, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<ModuleDto>> GetModulesByStatusWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var modules = _context.Modules.Where(x => x.IsActive == status)
                                          .OrderByDescending(x => x.DateAdded)
                                           .Select(x => new ModuleDto
                                           {
                                               Id = x.Id,
                                               MainMenu = x.MainMenu.ModuleName,
                                               MainMenuId = x.MainMenu.Id,
                                               SubMenuName = x.SubMenuName,
                                               ModuleName = x.ModuleName,
                                               DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                               AddedBy = x.AddedBy,
                                               IsActive = x.IsActive
                                           }).Where(x => x.SubMenuName.ToLower().Contains(search.Trim().ToLower())
                                                   || x.MainMenu.ToLower().Contains(search.Trim().ToLower())
                                                   || x.ModuleName.ToLower().Contains(search.Trim().ToLower()));





            return await PagedList<ModuleDto>.CreateAsync(modules, userParams.PageNumber, userParams.PageSize);


        }



        //-------------------MAIN MENU


        public async Task<IReadOnlyList<ModuleDto>> GetAllActiveMainMenu()
        {

            var module = _context.MainMenus.Where(x => x.IsActive == true)
                                         .Select(x => new ModuleDto
                                         {
                                             Id = x.Id,
                                             MainMenu = x.ModuleName,
                                             DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                             AddedBy = x.AddedBy,
                                             MenuPath = x.MenuPath,
                                             IsActive= x.IsActive
                                         });

            return await module.ToListAsync();

        }

        public async Task<IReadOnlyList<ModuleDto>> GetAllInActiveMainMenu()
        {
            var module = _context.MainMenus.Where(x => x.IsActive == false)
                                       .Select(x => new ModuleDto
                                       {
                                           Id = x.Id,
                                           MainMenu = x.ModuleName,
                                           DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                           AddedBy = x.AddedBy,
                                           MenuPath = x.MenuPath,
                                            IsActive = x.IsActive
                                       });

            return await module.ToListAsync();

        }

        public async Task<bool> AddNewMainMenu(MainMenu menu)
        {
            await _context.AddAsync(menu);

            return true;
        }

        public async Task<bool> UpdateMainMenu(MainMenu menu)
        {
            var exisitngMenu = await _context.MainMenus.Where(x => x.Id == menu.Id)
                                                       .FirstOrDefaultAsync();

            exisitngMenu.ModuleName = menu.ModuleName;
            exisitngMenu.MenuPath = menu.MenuPath;

            return true;
        }

        public async Task<bool> UpdateMenuOrder(MainMenu menu)
        {
            var existing = await _context.MainMenus.Where(x => x.Id == menu.Id)
                                                                       .FirstOrDefaultAsync();

            existing.MenuOrder = menu.MenuOrder;

            return true;
        }

        public async Task<bool> InActiveMainMenu(MainMenu menu)
        {
            var existingMenu = await _context.MainMenus.Where(x => x.Id == menu.Id)
                                                     .FirstOrDefaultAsync();

            existingMenu.IsActive = false;

            return true;
        }

        public async Task<bool> ActivateMainMenu(MainMenu menu)
        {
            var existingMenu = await _context.MainMenus.Where(x => x.Id == menu.Id)
                                                 .FirstOrDefaultAsync();

            existingMenu.IsActive = true;

            return true;
        }

        public async Task<bool> MenuAlreadyExist(string menu)
        {
            return await _context.MainMenus.AnyAsync(x => x.ModuleName == menu);
        }

        public async Task<PagedList<ModuleDto>> GetAllMainMenuWithPagination(bool status, UserParams userParams)
        {
            var module = _context.MainMenus.Where(x => x.IsActive == status)
                                        .OrderBy(x => x.MenuOrder)
                                       .Select(x => new ModuleDto
                                       {
                                           Id = x.Id,
                                           MainMenu = x.ModuleName,
                                           DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                           AddedBy = x.AddedBy,
                                           MenuPath = x.MenuPath,
                                            IsActive = x.IsActive,
                                            MenuOrder= x.MenuOrder,

                                       });

            return await PagedList<ModuleDto>.CreateAsync(module, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<ModuleDto>> GetMainMenuPaginationOrig(UserParams userParams, bool status, string search)
        {
            var module = _context.MainMenus.Where(x => x.IsActive == status)
                                       .OrderBy(x => x.MenuOrder)
                                       .Select(x => new ModuleDto
                                       {
                                           Id = x.Id,
                                           MainMenu = x.ModuleName,
                                           DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                           AddedBy = x.AddedBy,
                                           MenuPath = x.MenuPath,
                                            IsActive = x.IsActive,
                                           MenuOrder = x.MenuOrder

                                       }).Where(x => x.MainMenu.ToLower().Contains(search.Trim().ToLower())
                                       || x.MenuPath.ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<ModuleDto>.CreateAsync(module, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> ValidateMenu(int module)
        {
            var valid = await _context.RoleModules.Where(x => x.ModuleId == module)
                                                   .Where(x => x.IsActive == true)
                                                   .FirstOrDefaultAsync();
            if(valid == null)
                return false;
            return true;
            
        }

        public async Task<bool> validatemoduleMainMenu(int module)
        {
            var valid = await _context.Modules.Where(x => x.MainMenuId == module)
                                              .FirstOrDefaultAsync();

            if (valid == null)
                return false;

            return true;


        }

        public async Task<bool> ValidateMenuSame(MainMenu menu)
        {
           var mainmenus = await _context.MainMenus.Where(x => x.Id == menu.Id && x.ModuleName == menu.ModuleName && x.MenuPath == menu.MenuPath)
                                                   .FirstOrDefaultAsync();

            if(mainmenus == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> ValidateModuleSame(Module module)
        {
            var modules = await _context.Modules.Where(x => x.Id == module.Id && x.MainMenuId == module.MainMenuId
                                                 && x.SubMenuName == module.SubMenuName && x.ModuleName == module.ModuleName)
                                                 .FirstOrDefaultAsync();

            if(modules == null) 
                return false;

            return true;
        }

        public async Task<bool> ValidateModuleAndMenuPath(int module, string path)
        {
            return await _context.Modules.AnyAsync(x => x.MainMenuId == module && x.ModuleName == path);
        }
    }
}
