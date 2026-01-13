using Microsoft.EntityFrameworkCore;
using FintechWalletApi.Models;
using FintechWalletApi.Idempotency;
namespace FintechWalletApi.Data;
using FintechWalletApi.Audit;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();



}
