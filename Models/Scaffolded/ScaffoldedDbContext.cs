using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Models.Scaffolded;

public partial class ScaffoldedDbContext : DbContext
{
    public ScaffoldedDbContext(DbContextOptions<ScaffoldedDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Ward> Wards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasIndex(e => e.CityName, "IX_Cities_CityName")
                .IsUnique()
                .HasFilter("([CityName] IS NOT NULL)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.Channel).HasDefaultValue("Online");

            entity.HasOne(d => d.AppUser).WithMany(p => p.OrderAppUsers).HasConstraintName("FK_Orders_AppUser");

            entity.HasOne(d => d.City).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Cities");

            entity.HasOne(d => d.User).WithMany(p => p.OrderUsers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Ward).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Wards");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<Ward>(entity =>
        {
            entity.HasIndex(e => new { e.CityId, e.WardName }, "IX_Wards_CityId_WardName")
                .IsUnique()
                .HasFilter("([CityId] IS NOT NULL AND [WardName] IS NOT NULL)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
