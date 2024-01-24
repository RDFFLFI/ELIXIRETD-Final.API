using ELIXIRETD.DATA.CORE.INTERFACES.USER_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.USER_REPOSITORY.Excemption;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES
{
    public class RoleRepository : IRoleRepository
    {
        private new readonly StoreContext _context;
        public RoleRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<RoleDto>> GetAllActiveRoles()
        {
            var roles = _context.Roles.Where(x => x.IsActive == true)
                                      .Select(x => new RoleDto
                                      {
                                          Id = x.Id,
                                          RoleName = x.RoleName,
                                          AddedBy = x.AddedBy,
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

                                      });
            return await roles.ToListAsync();

        }

        public async Task<IReadOnlyList<RoleDto>> GetAllInActiveRoles()
        {
            var roles = _context.Roles.Where(x => x.IsActive == false)
                                      .Select(x => new RoleDto
                                      {
                                          Id = x.Id,
                                          RoleName = x.RoleName,
                                          AddedBy = x.AddedBy,
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

                                      });
            return await roles.ToListAsync();
        }



        public async Task<bool> AddNewRole(UserRole role)
        {
            await _context.Roles.AddAsync(role);
            return true;
        }

        public async Task<bool> UpdateRoleInfo(UserRole role)
        {
            var existingRole = await _context.Roles.Where(x => x.Id == role.Id)
                                                   .FirstOrDefaultAsync();

            existingRole.RoleName = role.RoleName;


            return true;

        }


        public async Task<bool> ActivateRole(UserRole role)
        {
            var roles = await _context.Roles.Where(x => x.Id == role.Id)
                                            .FirstOrDefaultAsync();

            roles.IsActive = true;
            return true;

        }

        public async Task<bool> InActiveRole(UserRole role)
        {
            var roles = await _context.Roles.Where(x => x.Id == role.Id)
                                          .FirstOrDefaultAsync();

            roles.IsActive = false;

            return true;
        }

        public async Task<bool> ValidateRoleExist(string role)
        {
            return await _context.Roles.AnyAsync(x => x.RoleName == role);
        }

        public async Task<PagedList<RoleDto>> GetAllRoleWithPagination(bool status, UserParams userParams)
        {
            var role = _context.Roles.Where(x => x.IsActive == status)
                                     .OrderByDescending(x => x.DateAdded)
                                     .Select(x => new RoleDto
                                     {
                                         Id = x.Id,
                                         RoleName = x.RoleName,
                                         AddedBy = x.AddedBy,
                                         DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                         IsActive = x.IsActive

                                     });

            return await PagedList<RoleDto>.CreateAsync(role, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<RoleDto>> GetAllRoleWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var role = _context.Roles.Where(x => x.IsActive == status)
                                       .OrderByDescending(x => x.DateAdded)
                                       .Select(x => new RoleDto
                                       {
                                           Id = x.Id,
                                           RoleName = x.RoleName,
                                           AddedBy = x.AddedBy,
                                           DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                           IsActive = x.IsActive

                                       }).Where(x => x.RoleName.ToLower().Contains(search.Trim().ToLower())); 

            return await PagedList<RoleDto>.CreateAsync(role, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> UntagModuleinRole(UserRoleModules rolemodules)
        {

            var existingrolemodule = await _context.RoleModules.Where(x => x.ModuleId == rolemodules.ModuleId)
                                                               .Where(x => x.RoleId == rolemodules.RoleId)
                                                               .FirstOrDefaultAsync();

            var taggedModules = await _context.RoleModules.Where(x => x.RoleId == rolemodules.RoleId && x.IsActive == true)
            .Select(x => new
            {
                x.ModuleId
            }).ToListAsync();

            if(taggedModules.Count> 1)
            {
                existingrolemodule.IsActive = false;
                return true;
            }

            if (existingrolemodule == null)
                throw new NoRoleFoundException(); // If 
            throw new AtLeast1RoleException(); // Else

        }

        // Tag Module

        public async Task<bool> TagModules(UserRoleModules roleModule)
        {
            await _context.AddAsync(roleModule);
            return true;
        }

        public async Task<bool> TagAndUntagUpdate(UserRoleModules rolemodule)
        {
            var rolemoduleStatus = await _context.RoleModules.Where(x => x.ModuleId == rolemodule.ModuleId)
                                                           .Where(x => x.RoleId == rolemodule.RoleId)
                                                           .FirstOrDefaultAsync();
            if (rolemoduleStatus == null)
                return await TagModules(rolemodule);

            if (rolemoduleStatus != null && rolemoduleStatus.IsActive == false)

                rolemoduleStatus.IsActive = true;

            return true;
        }

        public async Task<IReadOnlyList<RoleWithModuleDto>> GetRoleModuleById(int id, int menuid)
        {
            var rolemodules = from rolemodule in _context.RoleModules
                              join role in _context.Roles on rolemodule.RoleId equals role.Id
                              join module in _context.Modules on rolemodule.ModuleId equals module.Id
                              select new RoleWithModuleDto
                              {
                                  RoleName = role.RoleName,
                                  MainMenu = module.MainMenu.ModuleName,
                                  MainMenuId = module.MainMenuId,
                                  MenuPath = module.MainMenu.MenuPath,
                                  SubMenu = module.SubMenuName,
                                  ModuleName = module.ModuleName,
                                  Id = module.Id,
                                  IsActive = rolemodule.IsActive,
                                  RoleId = rolemodule.RoleId,
                              };

            return await rolemodules.Where(x => x.RoleId == id)
                                    .Where(x => x.IsActive == true)
                                    .Where(x => x.MainMenuId == menuid)
                                    .ToListAsync();
        }

        public async Task<IReadOnlyList<UntagModuleDto>> GetUntagModuleByRoleId(int id, int menuid)
        {
            var availablemodule = _context.Modules
                                                .Where(x => x.MainMenuId == menuid)
                                                .Where(x => !_context.RoleModules
                                                .Where(x => x.RoleId == id)
                                                .Where(x => x.IsActive == true)
                                                .Select(x => x.ModuleId)
                                                .Contains(x.Id));

            return await availablemodule

              .Select(rolemodule => new UntagModuleDto
              {
                  Remarks = "Untag",
                  MainMenu = rolemodule.MainMenu.ModuleName,
                  SubMenu = rolemodule.SubMenuName,
                  RoleId = id,
                  ModuleId = rolemodule.Id,
                  IsActive = rolemodule.IsActive,

              })
                   .Where(x => x.IsActive == true)
                   .ToListAsync();
        }

        public async Task<bool> CheckRoleandTagModules(UserRoleModules rolemodule)
        {
            var existingrolemodule = await _context.RoleModules.Where(x => x.RoleId == rolemodule.RoleId)
                                                               .Where(x => x.ModuleId == rolemodule.ModuleId)
                                                               .Where(x => x.IsActive == true)
                                                               .FirstOrDefaultAsync();
            if (existingrolemodule == null)
                return true;


            return false;
        }


        public async Task<IReadOnlyList<RoleWithModuleDto>> GetRoleModuleWithId(int id)
        {
            var rolemodules = _context.RoleModules

                              .Join(_context.Roles, rolemoduless => rolemoduless.RoleId, role => role.Id, (rolemodoless, role) => new { rolemodoless, role })
                              .Join(_context.Modules, rolemoduless => rolemoduless.rolemodoless.ModuleId, module => module.Id, (rolemoduless, module) => new { rolemoduless, module })
                              .OrderBy(x => x.module.MainMenu.MenuOrder)
                              .ThenBy(x => x.module.Id)
                              .Select(x => new RoleWithModuleDto
                              {

                                  RoleName = x.rolemoduless.role.RoleName,
                                  MainMenu = x.module.MainMenu.ModuleName,
                                  MainMenuId = x.module.MainMenuId,
                                  MenuPath = x.module.MainMenu.MenuPath,
                                  SubMenu = x.module.SubMenuName,
                                  ModuleName = x.module.ModuleName,
                                  Id = x.module.Id,
                                  MenuOrder = x.module.MainMenu.MenuOrder,
                                  IsActive = x.rolemoduless.rolemodoless.IsActive,
                                  RoleId = x.rolemoduless.rolemodoless.RoleId

                              });  

                                    return await rolemodules
                                    .Where(x => x.RoleId == id)
                                    .Where(x => x.IsActive == true)
                                    .ToListAsync();

        }

        public async Task<bool> ValidateRoleSame(UserRole roles)
        {
            var role = await _context.Roles.Where(x => x.Id == roles.Id && x.RoleName == roles.RoleName)
                                            .FirstOrDefaultAsync();

            if(role == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateRoleInUse(int role)
        {

            return await _context.Users.AnyAsync(x => x.UserRoleId == role && x.IsActive == true);

        }

        public async Task<UserRole> GetByCodeAsync(string userRoleCode)
        {
            return await _context.Roles.FirstOrDefaultAsync(u => u.RoleName == userRoleCode);
        }
    }
}
