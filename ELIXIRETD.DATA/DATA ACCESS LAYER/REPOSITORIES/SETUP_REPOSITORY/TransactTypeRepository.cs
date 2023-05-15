using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
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
    public class TransactTypeRepository : ITransactType
    {
        private readonly StoreContext _context;

        public TransactTypeRepository( StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddnewTransactType(TransactionType transactType)
        {
            transactType.IsActive = true;

            await _context.TransactionTypes.AddAsync(transactType);

            return true;
        }

        public async Task<IReadOnlyList<DtoTransactionType>> GetAllActiveTransactionType()
        {
            var transact = _context.TransactionTypes.Where(x => x.IsActive == true)
                                                    .Select(x => new DtoTransactionType
                                                    {

                                                        Id = x.Id,
                                                        TransactionName = x.TransactionName,
                                                        AddedBy = x.Addedby,
                                                        DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                        IsActive = x.IsActive,

                                                    });

            return await transact.ToListAsync();
        }



        public async Task<IReadOnlyList<DtoTransactionType>> GetAllInActiveTransactionType()
        {
            var transact = _context.TransactionTypes.Where(x => x.IsActive == false)
                                                  .Select(x => new DtoTransactionType
                                                  {

                                                      Id = x.Id,
                                                      TransactionName = x.TransactionName,
                                                      AddedBy = x.Addedby,
                                                      DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                                      IsActive = x.IsActive,

                                                  });

            return await transact.ToListAsync();
        }

        public async Task<bool> UpdateActive(TransactionType transactType)
        {
            var transact = await _context.TransactionTypes.Where(x => x.Id == transactType.Id)
                                                          .FirstOrDefaultAsync();


            transact.IsActive = true;

            return true;
        }

        public async Task<bool> UpdateInActive(TransactionType transactType)
        {
            var transact = await _context.TransactionTypes.Where(x => x.Id == transactType.Id)
                                                          .FirstOrDefaultAsync();


            transact.IsActive = false;

            return true;
        }
    

        public async Task<bool> UpdateTransactionType(TransactionType transactType)
        {
            var transact = await _context.TransactionTypes.Where(x => x.Id == transactType.Id)
                                                          .FirstOrDefaultAsync();


            transact.TransactionName = transactType.TransactionName;

            return true;
        }

        public async Task<PagedList<DtoTransactionType>>GetTransactTypeWithPagination(bool status ,UserParams userParams)
        {
            var transaction = _context.TransactionTypes.Where(x => x.IsActive == status)
                                             .OrderByDescending(x => x.DateAdded)
                                      .Select(x => new DtoTransactionType
                                      {
                                          Id = x.Id,
                                          TransactionName = x.TransactionName,
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                          AddedBy = x.Addedby,
                                          IsActive = x.IsActive,
                                      });

            return await PagedList<DtoTransactionType>.CreateAsync(transaction, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<DtoTransactionType>> GetTransactTypeWithPaginationOrig(UserParams userParams, bool status, string search)
        {
            var transaction = _context.TransactionTypes.Where(x => x.IsActive == status)
                                             .OrderByDescending(x => x.DateAdded)
                                      .Select(x => new DtoTransactionType
                                      {
                                          Id = x.Id,
                                         TransactionName = x.TransactionName,
                                          DateAdded = x.DateAdded.ToString("MM/dd/yyyy"),
                                          AddedBy = x.Addedby,
                                          IsActive = x.IsActive

                                      }).Where(x => x.TransactionName.ToLower().Contains(search.Trim().ToLower()));

            return await PagedList<DtoTransactionType>.CreateAsync(transaction, userParams.PageNumber, userParams.PageSize);
        }


        // validation 

        public async Task<bool> ValidationTransactName(string transactName)
        {

            return await _context.TransactionTypes.AnyAsync(x => x.TransactionName == transactName);
     
        }

        public async Task<bool> validateTransactTypeSame(TransactionType transact)
        {
            var validatetransact = await _context.TransactionTypes.Where(x => x.Id == transact.Id && x.TransactionName == transact.TransactionName)
                                                                  .FirstOrDefaultAsync();

            if(validatetransact == null) 
                return false;

            return true;
        }


    }
}
