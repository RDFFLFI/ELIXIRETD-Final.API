﻿using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.FUEL_REGISTER_MODEL;
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
        public virtual DbSet<UserRoleModules> RoleModules { get; set; }
        public virtual DbSet<MainMenu> MainMenus { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Uom> Uoms { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<ItemCategory> ItemCategories { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<LotSection> LotSections { get; set; }
        public virtual DbSet<LotNames> Lotnames { get; set; }
        public virtual DbSet<Reason> Reasons { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<PoSummary> PoSummaries { get; set; }
        public virtual DbSet<Warehouse_Receiving> WarehouseReceived { get; set; }
        public virtual DbSet<Ordering> Orders { get; set; }
        public virtual DbSet<GenerateOrderNo> GenerateOrders { get; set; }
        public virtual DbSet<MoveOrder> MoveOrders { get; set; }
        public virtual DbSet<TransactMoveOrder> TransactOrder { get; set; }

        public virtual DbSet<MiscellaneousIssue> MiscellaneousIssues { get; set; }
        public virtual DbSet<MiscellaneousIssueDetails> MiscellaneousIssueDetail { get; set; }
        public virtual DbSet<MiscellaneousReceipt> MiscellaneousReceipts { get; set; }
        public virtual DbSet<BorrowedIssue> BorrowedIssues { get; set; }
        public virtual DbSet<BorrowedIssueDetails> BorrowedIssueDetails { get; set; }
        public virtual DbSet<BorrowedConsume> BorrowedConsumes { get; set; }

        public virtual DbSet<TransactionType> TransactionTypes { get; set; }

        public virtual DbSet<FuelRegister> FuelRegisters { get; set; }
        public virtual DbSet<FuelRegisterDetail> FuelRegisterDetails { get; set; }



       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DevConnection");
            optionsBuilder.UseSqlServer(connectionString).EnableSensitiveDataLogging();
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

        }



    }
}
