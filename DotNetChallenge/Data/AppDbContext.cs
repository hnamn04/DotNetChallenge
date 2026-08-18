using DotNetChallenge.Models;
using DotNetChallenge.Models.Entities;
using DotNetChallenge.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DotNetChallenge.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

 
    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Inventory> Inventories => Set<Inventory>();

    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderItem> SalesOrderItems => Set<SalesOrderItem>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigureRoles(modelBuilder);
        ConfigureUserRoles(modelBuilder);

        ConfigureCustomers(modelBuilder);
        ConfigureSuppliers(modelBuilder);

        ConfigureCategories(modelBuilder);
        ConfigureUnits(modelBuilder);
        ConfigureProducts(modelBuilder);
        ConfigureInventory(modelBuilder);

        ConfigurePurchaseOrders(modelBuilder);
        ConfigurePurchaseOrderItems(modelBuilder);

        ConfigureSalesOrders(modelBuilder);
        ConfigureSalesOrderItems(modelBuilder);

        ConfigurePayments(modelBuilder);

        ConfigureStockTransactions(modelBuilder);
    }

    // USERS
    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Username)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.HasIndex(x => x.Username)
                .IsUnique();

            entity.HasIndex(x => x.Email)
                .IsUnique();
        });
    }

    // ROLES
    private static void ConfigureRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(x => x.Name)
                .IsUnique();
        });
    }

    // USER ROLES
    private static void ConfigureUserRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");

            entity.HasKey(x => new
            {
                x.UserId,
                x.RoleId
            });

            entity.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
 
    // CUSTOMERS
    private static void ConfigureCustomers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(255);

            entity.Property(x => x.Phone)
                .HasMaxLength(30);

            entity.Property(x => x.Address)
                .HasMaxLength(500);

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.HasIndex(x => x.Phone);
        });
    }

    // SUPPLIERS
    private static void ConfigureSuppliers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("suppliers");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(255);

            entity.Property(x => x.Phone)
                .HasMaxLength(30);

            entity.Property(x => x.Address)
                .HasMaxLength(500);

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.HasIndex(x => x.Phone);
        });
    }
 
    // CATEGORIES
    private static void ConfigureCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(x => x.Name)
                .IsUnique();
        });
    }

    // UNITS
    private static void ConfigureUnits(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Unit>(entity =>
        {
            entity.ToTable("units");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Symbol)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasIndex(x => x.Name)
                .IsUnique();

            entity.HasIndex(x => x.Symbol)
                .IsUnique();
        });
    }

    // PRODUCTS
    private static void ConfigureProducts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products", table =>
            {
                table.HasCheckConstraint(
                    "ck_products_cost_price",
                    "cost_price >= 0"
                );

                table.HasCheckConstraint(
                    "ck_products_selling_price",
                    "selling_price >= 0"
                );
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Code)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(2000);

            entity.Property(x => x.CostPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(x => x.SellingPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.HasIndex(x => x.Code)
                .IsUnique();

            entity.HasIndex(x => x.Name);

            entity.HasIndex(x => x.CategoryId);

            entity.HasIndex(x => x.UnitId);

            entity.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Unit)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
 
    // INVENTORY
    private static void ConfigureInventory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("inventory", table =>
            {
                table.HasCheckConstraint(
                    "ck_inventory_quantity",
                    "quantity >= 0"
                );

                table.HasCheckConstraint(
                    "ck_inventory_reserved_quantity",
                    "reserved_quantity >= 0"
                );

                table.HasCheckConstraint(
                    "ck_inventory_reserved_not_greater",
                    "reserved_quantity <= quantity"
                );
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Quantity)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(x => x.Version)
                .IsConcurrencyToken();

            entity.Property(x => x.ReservedQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            entity.HasIndex(x => x.ProductId)
                .IsUnique();

            entity.HasOne(x => x.Product)
                .WithOne(x => x.Inventory)
                .HasForeignKey<Inventory>(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    // PURCHASE ORDERS
    private static void ConfigurePurchaseOrders(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.ToTable("purchase_orders");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.OrderNumber)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(x => x.OrderNumber)
                .IsUnique();

            entity.HasIndex(x => x.SupplierId);

            entity.HasIndex(x => new
            {
                x.SupplierId,
                x.OrderDate
            });

            entity.HasOne(x => x.Supplier)
                .WithMany(x => x.PurchaseOrders)
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // PURCHASE ORDER ITEMS
    private static void ConfigurePurchaseOrderItems(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.ToTable("purchase_order_items", table =>
            {
                table.HasCheckConstraint(
                    "ck_purchase_order_items_quantity",
                    "quantity > 0"
                );

                table.HasCheckConstraint(
                    "ck_purchase_order_items_unit_price",
                    "unit_price >= 0"
                );

                table.HasCheckConstraint(
                    "ck_purchase_order_items_total_price",
                    "total_price >= 0"
                );
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Quantity)
                .IsRequired();

            entity.Property(x => x.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(x => x.TotalPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.HasIndex(x => x.PurchaseOrderId);

            entity.HasIndex(x => x.ProductId);

            entity.HasIndex(x => new
            {
                x.PurchaseOrderId,
                x.ProductId
            });

            entity.HasOne(x => x.PurchaseOrder)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Product)
                .WithMany(x => x.PurchaseOrderItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // SALES ORDERS
    private static void ConfigureSalesOrders(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SalesOrder>(entity =>
        {
            entity.ToTable("sales_orders");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.OrderNumber)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.PaymentStatus)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired()
                .HasDefaultValue(PaymentStatus.Unpaid);

            entity.HasIndex(x => x.OrderNumber)
                .IsUnique();

            entity.HasIndex(x => x.CustomerId);

            entity.HasIndex(x => new
            {
                x.CustomerId,
                x.OrderDate
            });

            entity.HasIndex(x => x.PaymentStatus);

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.SalesOrders)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // SALES ORDER ITEMS
    private static void ConfigureSalesOrderItems(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SalesOrderItem>(entity =>
        {
            entity.ToTable("sales_order_items", table =>
            {
                table.HasCheckConstraint(
                    "ck_sales_order_items_quantity",
                    "quantity > 0"
                );

                table.HasCheckConstraint(
                    "ck_sales_order_items_unit_price",
                    "unit_price >= 0"
                );
            });

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Quantity)
                .IsRequired();

            entity.Property(x => x.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(x => x.TotalPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.HasIndex(x => x.SalesOrderId);

            entity.HasIndex(x => x.ProductId);

            entity.HasIndex(x => new
            {
                x.SalesOrderId,
                x.ProductId
            });

            entity.HasOne(x => x.SalesOrder)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.SalesOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Product)
                .WithMany(x => x.SalesOrderItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
 
    // PAYMENTS
    private static void ConfigurePayments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(x => x.Method)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(x => x.SalesOrderId);

            entity.HasOne(x => x.SalesOrder)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // STOCK TRANSACTIONS
    private static void ConfigureStockTransactions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.ToTable("stock_transactions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ReferenceType)
                .HasMaxLength(50);

            entity.Property(x => x.Note)
                .HasMaxLength(1000);

            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(x => x.ProductId);

            entity.HasIndex(x => x.Type);

            entity.HasIndex(x => new
            {
                x.ReferenceType,
                x.ReferenceId
            });

            entity.HasIndex(x => new
            {
                x.ProductId,
                x.CreatedAt
            });

            entity.HasOne(x => x.Product)
                .WithMany(x => x.StockTransactions)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}