﻿using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.CORE.INTERFACES.BORROWED_INTERFACE;
using ELIXIRETD.DATA.CORE.INTERFACES.FUEL_REGISTER_INTERFACE;
using ELIXIRETD.DATA.CORE.INTERFACES.IMPORT_INTERFACE;
using ELIXIRETD.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIRETD.DATA.CORE.INTERFACES.Orders;
using ELIXIRETD.DATA.CORE.INTERFACES.REPORTS_INTERFACE;
using ELIXIRETD.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIRETD.DATA.CORE.INTERFACES.USER_INTERFACE;
using ELIXIRETD.DATA.CORE.INTERFACES.WAREHOUSE_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.BORROWED_REPOSITORY;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.FUEL_REGISTER_REPOSITORY;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.IMPORT_REPOSITORY;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.INVENTORY_REPOSITORY;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.OrderingRepository;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORTS_REPOSITORY;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.WAREHOUSE_REPOSITORY;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

namespace ELIXIRETD.DATA.SERVICES
{
    public class UnitOfWork : IUnitOfWork, IDisposable

    {
        private readonly StoreContext _context;
        private readonly IMediator _mediator;

        private IDbContextTransaction _transaction;

        public IUserRepository Users { get; private set; }

        public IRoleRepository Roles { get; private set; }

        public IModuleRepository Modules { get; private set; }

        public IUomRepository Uoms { get; private set; }

        public IMaterialRepository Materials { get; set; }

        public ISupplierRepository Suppliers { get; set; }

        public ICustomerRepository Customers { get; set; }

        public ILotRepository Lots { get; set; }

        public IReasonRepository Reasons { get; set; }

        public ICompanyRepository Companies { get; set; }


        public ILocationRepository Locations { get; set; }

        public IPoSummaryRepository Imports { get; set; }

        public IWarehouseReceiveRepository Receives { get; set; }

        public IOrdering Orders { get; set; }

        public IMiscellaneous miscellaneous { get; set; }

        public IBorrowedItem Borrowed { get; set; }

        public IMRPInventory Inventory { get; set; }

        public ITransactType TransactType { get; set; }

        public IReports Reports { get; set; }


        public IFuelRegisterRepository FuelRegister { get; set; }

        public UnitOfWork(StoreContext context, IMediator mediator)

        {
            _context = context;
            _mediator = mediator;


            Users = new UserRepository(_context);
            Roles = new RoleRepository(_context);
            Modules = new ModuleRepository(_context);
            Uoms = new UomRepository(_context);
            Materials = new MaterialRepository(_context);
            Suppliers = new SupplierRepository(_context);
            Customers = new CustomerRepository(_context);
            Lots = new LotRepository(_context);
            Reasons = new ReasonRepository(_context);
            Companies = new CompanyRepository(_context);
            Locations = new LocationRepository(_context);
            Imports = new PoSummaryRepository(_context);
            Receives = new WarehouseRepository(_context);
            Orders = new OrderingRepository(_context);
            miscellaneous = new MiscellaneousRepository(_context);
            Borrowed = new BorrowedRepository(_context);
            Inventory = new MRPInvetoryRepository(_context);
            TransactType = new TransactTypeRepository(_context);
            Reports = new ReportsRepository(_context);
            FuelRegister = new FuelRegisterRepository(_context);

        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }


      

    }

}
