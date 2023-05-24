using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.USER_INTERFACE
{
    public interface IModuleRepository
    {
        Task<IReadOnlyList<ModuleDto>> GetAllActiveModules();
        Task<IReadOnlyList<ModuleDto>> GetAllInActiveModules();
        Task<bool> AddNewModule(Module module);
        Task<bool> UpdateModule(Module module);
        Task<bool> CheckMainMenu(int id);
        Task<bool> SubMenuNameExist(string module , int mainmenu);
        Task<bool> ModuleNameExist(string module);
        Task<bool> InActiveModule(Module module);
        Task<bool> ActivateModule(Module module);
        Task<PagedList<ModuleDto>> GetAllModulessWithPagination(bool status, UserParams userParams);
        Task<PagedList<ModuleDto>> GetModulesByStatusWithPaginationOrig(UserParams userParams, bool status, string search);
        Task<bool> ValidateMenu(int module);
        Task<bool> validatemoduleMainMenu(int module);




        Task<IReadOnlyList<ModuleDto>> GetAllActiveMainMenu();
        Task<IReadOnlyList<ModuleDto>> GetAllInActiveMainMenu();
        Task<bool> AddNewMainMenu(MainMenu menu);
        Task<bool> UpdateMainMenu(MainMenu menu);
        Task<bool> InActiveMainMenu(MainMenu menu);
        Task<bool> ActivateMainMenu(MainMenu menu);
        Task<bool> MenuAlreadyExist(string menu);
        Task<PagedList<ModuleDto>> GetAllMainMenuWithPagination(bool status, UserParams userParams);
        Task<PagedList<ModuleDto>> GetMainMenuPaginationOrig(UserParams userParams, bool status, string search);
        Task<bool> UpdateMenuOrder(MainMenu menu);

        Task<bool> ValidateMenuSame(MainMenu menu);

        Task<bool> ValidateModuleSame(Module module);



        Task<bool> ValidateModuleAndMenuPath(int module , string path);


    }


}
