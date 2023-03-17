using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class AccountRepository : IAccountRepository
    {
        private new readonly StoreContext _context;
        public AccountRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<AccountDto>> GetAllActiveAccount()
        {
            var accounts = _context.Accounts.Where(x => x.IsActive == true)
                                            .Select(x => new AccountDto
                                            {
                                                Id = x.Id, 
                                                AccountCode = x.AccountCode, 
                                                AccountName = x.AccountName, 
                                                AddedBy = x.AddedBy, 
                                                DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                IsActive = x.IsActive

                                            });

            return await accounts.ToListAsync();
        }

        public async Task<IReadOnlyList<AccountDto>> GetAllInActiveAccount()
        {
            var accounts = _context.Accounts.Where(x => x.IsActive == false)
                                           .Select(x => new AccountDto
                                           {
                                               Id = x.Id,
                                               AccountCode = x.AccountCode,
                                               AccountName = x.AccountName,
                                               AddedBy = x.AddedBy,
                                               DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                               IsActive = x.IsActive

                                           });

            return await accounts.ToListAsync();
        }

        public async Task<bool> AddAccount(Account account)
        {
            await _context.Accounts.AddAsync(account);
            return true;
        }


        public async Task<bool> UpdateAccount(Account account)
        {
            var accounts = await _context.Accounts.Where(x => x.Id == account.Id)
                                                  .FirstOrDefaultAsync();

            accounts.AccountName = account.AccountName;

            return true;
        }

    

        public async Task<bool> ActivateAccount(Account account)
        {
            var accounts = await _context.Accounts.Where(x => x.Id == account.Id)
                                                 .FirstOrDefaultAsync();

            accounts.IsActive = true;

            return true;
        }

        public async Task<bool> InActiveAccount(Account account)
        {
            var accounts = await _context.Accounts.Where(x => x.Id == account.Id)
                                                   .FirstOrDefaultAsync();

            accounts.IsActive = false;

            return true;
        }

      
        public async Task<PagedList<AccountDto>> GetAllAccountWithPagination(bool status, UserParams userParams)
        {
            var accounts = _context.Accounts.Where(x => x.IsActive == status)
                                            .OrderByDescending(x => x.DateAdded)       
                                           .Select(x => new AccountDto
                                           {
                                               Id = x.Id,
                                               AccountCode = x.AccountCode,
                                               AccountName = x.AccountName,
                                               AddedBy = x.AddedBy,
                                               DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                               IsActive = x.IsActive
                                           });

            return await PagedList<AccountDto>.CreateAsync(accounts, userParams.PageNumber, userParams.PageSize);
        }

      

        public async Task<PagedList<AccountDto>> GetAccountWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var accounts = _context.Accounts.Where(x => x.IsActive == status)
                                            .OrderByDescending(x => x.DateAdded)
                                         .Select(x => new AccountDto
                                         {
                                             Id = x.Id,
                                             AccountCode = x.AccountCode,
                                             AccountName = x.AccountName,
                                             AddedBy = x.AddedBy,
                                             DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                             IsActive = x.IsActive
                                         }).Where(x => x.AccountName.ToLower()
                                           .Contains(search.Trim().ToLower()));

            return await PagedList<AccountDto>.CreateAsync(accounts, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<bool> AccountCodeExist(string account)
        {
            return await _context.Accounts.AnyAsync(x => x.AccountCode == account);
        }

    }
}
