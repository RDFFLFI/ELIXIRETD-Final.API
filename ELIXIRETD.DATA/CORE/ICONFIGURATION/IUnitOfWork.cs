using ELIXIRETD.DATA.CORE.INTERFACES.BORROWED_INTERFACE;
using ELIXIRETD.DATA.CORE.INTERFACES.IMPORT_INTERFACE;
using ELIXIRETD.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIRETD.DATA.CORE.INTERFACES.Orders;
using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.CORE.INTERFACES.USER_INTERFACE;
using ELIXIRETD.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.ICONFIGURATION
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }

        IRoleRepository Roles { get; }

        IModuleRepository Modules { get; }

        IUomRepository Uoms { get; }

        IMaterialRepository Materials { get; }

        ISupplierRepository Suppliers { get; }
        
        ICustomerRepository Customers { get; }

        ILotRepository Lots { get; }

        IReasonRepository Reasons { get; }

        ICompanyRepository Companies{ get; }

        IAccountRepository Accounts { get; }

        ILocationRepository Locations { get; }

        IPoSummaryRepository Imports { get; }

        IWarehouseReceiveRepository Receives { get; }

        IOrdering Orders { get; }

        IMiscellaneous miscellaneous { get; }

        IBorrowedItem Borrowed { get; }

        IMRPInventory Inventory { get; }


                                           
        Task CompleteAsync();
    }
}
