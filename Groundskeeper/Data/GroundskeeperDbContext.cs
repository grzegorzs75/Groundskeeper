using Groundskeeper.Models;
using Microsoft.EntityFrameworkCore;

namespace Groundskeeper.Data;

public class GroundskeeperDbContext : DbContext
{
    public GroundskeeperDbContext(
        DbContextOptions<GroundskeeperDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Site> Sites => Set<Site>();

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<ServiceRecord> ServiceRecords => Set<ServiceRecord>();

    public DbSet<Issue> Issues => Set<Issue>();
}