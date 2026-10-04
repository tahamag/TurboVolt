using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TurboVolt.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Famillearticle> Famillearticle { get; set; }

    public virtual DbSet<Sousfamillearticle> Sousfamillearticle { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Famillearticle>(entity =>
        {
            entity.HasKey(e => e.IdFamilleArticle).HasName("PK_FAMARTICLE");

            entity.ToTable("FAMILLEARTICLE");

            entity.Property(e => e.CodeFamille).HasMaxLength(50);
            entity.Property(e => e.DateCreation)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateModification).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.DescriptionAr)
                .HasMaxLength(100)
                .HasColumnName("DescriptionAR");
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.Image).HasColumnType("image");
            entity.Property(e => e.IndexHome).HasColumnName("indexHome");
            entity.Property(e => e.IsCarreaux).HasDefaultValue(0);
            entity.Property(e => e.LibelleFamArticle).HasMaxLength(100);
            entity.Property(e => e.LibelleFamArticleAr)
                .HasMaxLength(100)
                .HasColumnName("LibelleFamArticleAR");
            entity.Property(e => e.PrixParUnite).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Supprime).HasDefaultValue(false);
        });

        modelBuilder.Entity<Sousfamillearticle>(entity =>
        {
            entity.HasKey(e => e.IdSousFamille);

            entity.ToTable("SOUSFAMILLEARTICLE");

            entity.Property(e => e.DateCreation)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateModification).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.DescriptionAr)
                .HasMaxLength(100)
                .HasColumnName("DescriptionAR");
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.Image).HasColumnType("image");
            entity.Property(e => e.LibelleSousFamille).HasMaxLength(100);
            entity.Property(e => e.LibelleSousFamilleAr)
                .HasMaxLength(100)
                .HasColumnName("LibelleSousFamilleAR");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
