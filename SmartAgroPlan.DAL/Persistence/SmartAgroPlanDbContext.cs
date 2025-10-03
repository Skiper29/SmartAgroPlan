﻿using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SmartAgroPlan.DAL.Entities.Crops;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.DAL.Persistence;

public class SmartAgroPlanDbContext : DbContext
{
    public SmartAgroPlanDbContext(DbContextOptions<SmartAgroPlanDbContext> options) : base(options)
    {
    }

    public DbSet<Field> Fields { get; set; }
    public DbSet<Soil> Soils { get; set; }
    public DbSet<FieldCropHistory> FieldCropHistories { get; set; }
    public DbSet<CropVariety> Crops { get; set; }
    public DbSet<FieldCondition> FieldConditions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("postgis");

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
