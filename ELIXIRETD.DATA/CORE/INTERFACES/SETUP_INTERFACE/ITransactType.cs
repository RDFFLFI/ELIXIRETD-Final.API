using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP;
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
    public interface ITransactType
    {

        Task<bool> AddnewTransactType (TransactionType transactType);

        Task<IReadOnlyList<DtoTransactionType>> GetAllActiveTransactionType ();
        Task<IReadOnlyList<DtoTransactionType>> GetAllInActiveTransactionType();

        Task <bool> UpdateTransactionType (TransactionType transactType);
        Task<bool> UpdateActive(TransactionType transactType);
        Task<bool> UpdateInActive(TransactionType transactType);


        public Task<PagedList<DtoTransactionType>> GetTransactTypeWithPagination(bool status , UserParams userParams);
        public Task<PagedList<DtoTransactionType>> GetTransactTypeWithPaginationOrig(UserParams userParams, bool status, string search);


        // Validation 

        Task<bool> ValidationTransactName(string transactName);

        Task<bool> validateTransactTypeSame(TransactionType transact);

    }
}
