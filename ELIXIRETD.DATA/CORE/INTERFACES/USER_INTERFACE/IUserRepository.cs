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
    public interface IUserRepository
    {

        //----------USER
        Task<IReadOnlyList<UserDto>> GetAllActiveUsers();
        Task<IReadOnlyList<UserDto>> GetAllInActiveUsers();
        Task<bool> AddNewUser(User user);
        Task<bool> UpdateUserInfo(User user);
        Task<bool> InActiveUser(User user);
        Task<bool> ActivateUser(User user);

        Task<bool> ChangePassword(User user);


        Task<PagedList<UserDto>> GetAllUserWithPagination(bool status, UserParams userParams);
        Task<PagedList<UserDto>> GetAllUserWithPaginationOrig(UserParams userParams, bool status, string search);


        //-----------VALIDATION
        Task<bool> ValidateRoleId(int id);
        Task<bool> ValidateUserExist(string username);
        Task<bool> ValidateUserRolesModule(User user);
        Task<bool> ValidationPassword(User user);
        //Task<bool> ValidatePasswordAndUsername(User user);

        //Task UpdateUserRoleName( userRoleId);

        Task<bool> ValidateEmpIdAndFullName(string empId, string fullName);

        // Import

        Task<bool> AddNewUserImport (User user);


        Task<string> GenerateUsername(string fullName);













    }
}
