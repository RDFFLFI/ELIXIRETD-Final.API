using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface IAccountRepository
    {

        Task<IReadOnlyList<AccountDto>> GetAllActiveAccount();
        Task<IReadOnlyList<AccountDto>> GetAllInActiveAccount();
        Task<bool> AddAccount(Account account);
        Task<bool> UpdateAccount(Account account);
        Task<bool> InActiveAccount(Account account);
        Task<bool> ActivateAccount(Account account);
        Task<PagedList<AccountDto>> GetAllAccountWithPagination(bool status, UserParams userParams);
        Task<PagedList<AccountDto>> GetAccountWithPaginationOrig(UserParams userParams, bool status, string search);


        Task<bool> AccountCodeExist(string account);
    }
}
