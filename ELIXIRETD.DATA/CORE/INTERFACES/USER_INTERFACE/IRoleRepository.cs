using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.USER_INTERFACE
{
    public interface IRoleRepository
    {

        Task<IReadOnlyList<RoleDto>> GetAllActiveRoles();
        Task<IReadOnlyList<RoleDto>> GetAllInActiveRoles();
        Task<bool> AddNewRole(UserRole role);
        Task<bool> UpdateRoleInfo(UserRole role);
        Task<bool> InActiveRole(UserRole role);
        Task<bool> ActivateRole(UserRole role);
        Task<bool> ValidateRoleExist(string role);
        Task<bool> TagModules(UserRoleModules roleModule);
        Task<bool> UntagModuleinRole(UserRoleModules rolemodules);
        Task<bool> TagAndUntagUpdate(UserRoleModules rolemodule);
        Task<IReadOnlyList<RoleWithModuleDto>> GetRoleModuleById(int id, int menuid);
        Task<IReadOnlyList<UntagModuleDto>> GetUntagModuleByRoleId(int id, int menuid);
        Task<bool> CheckRoleandTagModules(UserRoleModules rolemodule);
        Task<IReadOnlyList<RoleWithModuleDto>> GetRoleModuleWithId(int id);
        Task<PagedList<RoleDto>> GetAllRoleWithPagination(bool status, UserParams userParams);
        Task<PagedList<RoleDto>> GetAllRoleWithPaginationOrig(UserParams userParams, bool status, string search);

        //Task<bool> validateActiveRoleModules(UserRoleModules rolemodule);

        Task<bool> ValidateRoleSame(UserRole roles);

        Task<bool> ValidateRoleInUse(int role);


        Task<UserRole> GetByCodeAsync(string userRoleCode);
    }
}
