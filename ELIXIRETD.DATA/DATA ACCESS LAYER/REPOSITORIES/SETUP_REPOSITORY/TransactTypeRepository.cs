using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
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
            await _context.TransactionTypes.AddAsync(transactType);

            return true;
        }


        // validation 





    }
}
