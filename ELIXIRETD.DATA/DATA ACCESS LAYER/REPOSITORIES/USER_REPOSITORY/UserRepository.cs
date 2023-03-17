using ELIXIRETD.DATA.CORE.INTERFACES.USER_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.USER_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES
{
    public class UserRepository : IUserRepository
    {
        private readonly StoreContext _context;

        public UserRepository(StoreContext context) 
        {
            _context = context;
        }

        public async Task<IReadOnlyList<UserDto>> GetAllActiveUsers()
        {
            var user = _context.Users.Where(x => x.IsActive == true)
                                     .Select(x => new UserDto
                                     {
                                         Id = x.Id,
                                         FullName = x.FullName,
                                         UserName = x.UserName,
                                         Password = x.Password,
                                         UserRoleId = x.UserRoleId,
                                         UserRole = x.UserRole.RoleName,
                                         Department = x.Department.DepartmentName,
                                         DepartmentId = x.DepartmentId,
                                         DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                         IsActive = x.IsActive,
                                         AddedBy = x.AddedBy
                                     });

            return await user.ToListAsync();
            
        }

        public async Task<IReadOnlyList<UserDto>> GetAllInActiveUsers()
        {
            var user = _context.Users.Where(x => x.IsActive == false)
                                    .Select(x => new UserDto
                                    {
                                        Id = x.Id,
                                        FullName = x.FullName,
                                        UserName = x.UserName,
                                        Password = x.Password,
                                        UserRoleId = x.UserRoleId,
                                        UserRole = x.UserRole.RoleName,
                                        Department = x.Department.DepartmentName,
                                        DepartmentId = x.DepartmentId,
                                        DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                        IsActive = x.IsActive,
                                        AddedBy = x.AddedBy 
                                    });

            return await user.ToListAsync();

        }

        public async Task<bool> AddNewUser(User user)
        {
            await _context.Users.AddAsync(user);
            return true;
        }

        public async Task<bool> UpdateUserInfo(User user)
        {
            var existingUser = await _context.Users.Where(x => x.Id == user.Id)
                                              .FirstOrDefaultAsync();


            existingUser.FullName = user.FullName; 
            existingUser.UserName = user.UserName;
            existingUser.Password = user.Password;
            existingUser.UserRoleId = user.UserRoleId;
            existingUser.DepartmentId = user.DepartmentId;

            return true;

        }

        public async Task<bool> ActivateUser(User user)
        {
            var users = await _context.Users.Where(x => x.Id == user.Id)
                                            .FirstOrDefaultAsync();

            users.IsActive = true;

            return true;

        }

        public async Task<bool> InActiveUser(User user)
        {
            var users = await _context.Users.Where(x => x.Id == user.Id)
                                             .FirstOrDefaultAsync();

            users.IsActive = false;

            return true;
        }

        public async Task<PagedList<UserDto>> GetAllUserWithPagination(bool status, UserParams userParams)
        {

            var users = _context.Users.Where(x => x.IsActive == status)
                                      .OrderByDescending(x => x.DateAdded)
                                      .Select(x => new UserDto
                                        {
                                            Id = x.Id,
                                            FullName = x.FullName,
                                            UserName = x.UserName,
                                            Password = x.Password,
                                            UserRoleId = x.UserRoleId, 
                                            UserRole = x.UserRole.RoleName,
                                            Department = x.Department.DepartmentName,
                                            DepartmentId = x.DepartmentId,
                                            DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                            IsActive = x.IsActive,
                                            AddedBy = x.AddedBy
                                        });

            return await PagedList<UserDto>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<UserDto>> GetAllUserWithPaginationOrig(UserParams userParams, bool status, string search)
        {

            var users = _context.Users.OrderByDescending(x => x.DateAdded)
                                      .Select(x => new UserDto
                                      {
                                          Id = x.Id,
                                          FullName = x.FullName,
                                          UserName = x.UserName,
                                          Password = x.Password,
                                          UserRoleId = x.UserRoleId,
                                          UserRole = x.UserRole.RoleName,
                                          Department = x.Department.DepartmentName,
                                          DepartmentId = x.DepartmentId,
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                          IsActive = x.IsActive,
                                          AddedBy = x.AddedBy
                                      }).Where(x => x.IsActive == status)
                                        .Where(x => x.UserName.ToLower()
                                        .Contains(search.Trim().ToLower()));

            return await PagedList<UserDto>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }


        //--------------DEPARTMENT


        public async Task<IReadOnlyList<DepartmentDto>> GetAllActiveDepartment()
        {
            var department = _context.Departments.Where(x => x.IsActive == true)
                                                 .Select(x => new DepartmentDto
                                                 {
                                                     Id = x.Id,
                                                     DepartmentName = x.DepartmentName,
                                                     AddedBy = x.AddedBy,
                                                     DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

                                                 });

            return await department.ToListAsync();
            
        }

        public async Task<IReadOnlyList<DepartmentDto>> GetAllInActiveDepartment()
        {
            var department = _context.Departments.Where(x => x.IsActive == false)
                                                 .Select(x => new DepartmentDto
                                                 {
                                                     Id = x.Id,
                                                     DepartmentName = x.DepartmentName,
                                                     AddedBy = x.AddedBy,
                                                     DateAdded = x.DateAdded.ToString("MM/dd/yyyy")

                                                 });

            return await department.ToListAsync();

        }

        public async Task<bool> AddNewDepartment(Department department)
        {
            await _context.Departments.AddAsync(department);
            return true;
        }

        public async Task<bool> UpdateDepartment(Department department)
        {
                var dep = await _context.Departments.Where(x => x.Id == department.Id)
                                                    .FirstOrDefaultAsync();

            dep.DepartmentName = department.DepartmentName;

            return true;

        }

        public async Task<bool> InActiveDepartment(Department department)
        {
           var dep = await _context.Departments.Where(x => x.Id == department.Id)
                                               .FirstOrDefaultAsync();

            dep.IsActive = false;

            return true;
        }

        public async Task<bool> ActivateDepartment(Department department)
        {
            var dep = await _context.Departments.Where(x => x.Id == department.Id)
                                           .FirstOrDefaultAsync();

            dep.IsActive = true;

            return true;
        }

        public async Task<bool> ValidateRoleId(int id)
        {
            var roles = await _context.Roles.FindAsync(id);

            if (roles == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateDepartmentId(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateUserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username);
        }

        public async Task<bool> ValidateDepartmentCodeExist(string code)
        {
            return await _context.Departments.AnyAsync(x => x.DepartmentCode == code);
        }

        public async Task<PagedList<DepartmentDto>> GetAllDepartmentWithPagination(bool status, UserParams userParams)
        {
            var department = _context.Departments.Where(x => x.IsActive == status)
                                                 .Select(x => new DepartmentDto
                                                 {
                                                     Id = x.Id,
                                                     DepartmentCode = x.DepartmentCode,
                                                     DepartmentName = x.DepartmentName,
                                                     AddedBy = x.AddedBy,
                                                     DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                     IsActive = x.IsActive
                                                 });

               return await PagedList<DepartmentDto>.CreateAsync(department, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<DepartmentDto>> GetAllDepartmentWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var department = _context.Departments.Where(x => x.IsActive == status)
                                                 .Select(x => new DepartmentDto
                                                {
                                                    Id = x.Id, 
                                                    DepartmentCode = x.DepartmentCode,
                                                    DepartmentName = x.DepartmentName,
                                                    AddedBy = x.AddedBy,
                                                    DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                    IsActive = x.IsActive
                                                })
                                                  .Where(x => x.DepartmentName.ToLower()
                                                  .Contains(search.Trim().ToLower()));

            return await PagedList<DepartmentDto>.CreateAsync(department, userParams.PageNumber, userParams.PageSize);

        }


        public async Task<bool> ValidateUserRolesModule(User user)
        {

            var roleModule = await _context.RoleModules.Where(x => x.RoleId == user.UserRoleId)
                                                        .FirstOrDefaultAsync();

            if (roleModule == null)
            {
                roleModule = new UserRoleModules
                {
                    RoleId = user.UserRoleId,
                    ModuleId = 0
                };
                _context.RoleModules.Add(roleModule);
                return true;

            }
            else
            {
                if (roleModule.ModuleId == 0)
                    return true;
                else
                    return false;
            }
            
           
        }
    }
}
