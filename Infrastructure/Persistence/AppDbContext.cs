using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionSummary> TransactionSummaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>()
            .ToTable(nameof(Transactions), schema: "CHALLENGE");

        modelBuilder.Entity<TransactionSummary>()
            .ToTable(nameof(TransactionSummaries), schema: "CHALLENGE")
            .HasKey(p => new { p.AccountId, p.Date });
    }
}