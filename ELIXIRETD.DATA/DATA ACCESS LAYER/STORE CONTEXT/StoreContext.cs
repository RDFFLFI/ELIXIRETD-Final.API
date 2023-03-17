using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT
{
    public  class StoreContext : DbContext
    {

        public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> Roles { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<UserRoleModules> RoleModules { get; set; }
        public virtual DbSet<MainMenu> MainMenus { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Uom> Uoms { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<ItemCategory> ItemCategories { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerType> CustomerTypes { get; set; }
        public virtual DbSet<LotName> LotNames { get; set; }
        public virtual DbSet<LotCategory> LotCategories { get; set; }
        public virtual DbSet<Reason> Reasons { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<PoSummary> PoSummaries { get; set; }
        public virtual DbSet<Warehouse_Receiving> WarehouseReceived { get; set; }
        public virtual DbSet<Warehouse_Reject> WarehouseReject { get; set; }

        public virtual DbSet<Ordering> Orders { get; set; }
        public virtual DbSet<GenerateOrderNo> GenerateOrders { get; set; }
        public virtual DbSet<MoveOrder> MoveOrders { get; set; }
        public virtual DbSet<TransactMoveOrder> TransactOrder { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<MiscellaneousIssue> MiscellaneousIssues { get; set; }
        public virtual DbSet<MiscellaneousIssueDetails> MiscellaneousIssueDetail { get; set; }

        public virtual DbSet<MiscellaneousReceipt> MiscellaneousReceipts { get; set; }


        public virtual DbSet<BorrowedIssue> BorrowedIssues { get; set; }
        public virtual DbSet<BorrowedIssueDetails> BorrowedIssueDetails { get; set; }

        public virtual DbSet<BorrowedReceipt> BorrowedReceipts { get; set; }

       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DevConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }



    }
}
