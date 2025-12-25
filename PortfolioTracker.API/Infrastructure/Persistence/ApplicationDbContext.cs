using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<PortfolioCashBalance> PortfolioCashBalances => Set<PortfolioCashBalance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Customer)
            .WithOne(c => c.User)
            .HasForeignKey<Customer>(c => c.UserId);

        // Soft Delete
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.IsDeleted);

        // Temporarily disabled query filter for debugging
        // modelBuilder.Entity<Customer>()
        //     .HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Portfolios)
            .WithOne(p => p.Customer)
            .HasForeignKey(p => p.CustomerId);

        modelBuilder.Entity<Portfolio>()
            .HasMany(p => p.CashBalances)
            .WithOne(c => c.Portfolio)
            .HasForeignKey(c => c.PortfolioId);

        modelBuilder.Entity<Portfolio>()
            .HasQueryFilter(p => !p.IsDeleted);

        modelBuilder.Entity<PortfolioCashBalance>()
            .HasQueryFilter(c => !c.IsDeleted);

        // Unique constraint on CustomerId, Name, and Exchange
        modelBuilder.Entity<Portfolio>()
            .HasIndex(p => new { p.CustomerId, p.Name, p.Exchange })
            .IsUnique();

        // Unique constraint on PortfolioId and Currency
        modelBuilder.Entity<PortfolioCashBalance>()
            .HasIndex(cb => new { cb.PortfolioId, cb.Currency })
            .IsUnique();

    }
}
