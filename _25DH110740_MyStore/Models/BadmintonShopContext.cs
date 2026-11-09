using System.Data.Entity;

namespace _25DH110740_MyStore.Models
{
    public class _25DH110740_MyStoreContext : DbContext
    {
        public _25DH110740_MyStoreContext() : base("name=_25DH110740_MyStoreContext")
        {
            Database.SetInitializer(new DbInitializer());
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 0);
            modelBuilder.Entity<Order>().Property(p => p.TotalAmount).HasPrecision(18, 0);
            modelBuilder.Entity<OrderDetail>().Property(p => p.UnitPrice).HasPrecision(18, 0);
            base.OnModelCreating(modelBuilder);
        }
    }
}
