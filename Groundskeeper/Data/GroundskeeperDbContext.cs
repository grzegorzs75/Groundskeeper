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

    public DbSet<Photo> Photos => Set<Photo>();

    public DbSet<AppUser> AppUsers => Set<AppUser>(); protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }


}