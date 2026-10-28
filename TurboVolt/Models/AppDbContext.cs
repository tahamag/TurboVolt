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

    public virtual DbSet<Avoirclient> Avoirclient { get; set; }

    public virtual DbSet<AvoirclientXArticle> AvoirclientXArticle { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Avoirclient>(entity =>
        {
            entity.HasKey(e => e.IdAvoirClient).HasName("PK_AVOIR");

            entity.ToTable("AVOIRCLIENT");

            entity.Property(e => e.CodeAvoir).HasMaxLength(250);
            entity.Property(e => e.DateCreation)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateCreationAuto)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateModification).HasColumnType("datetime");
            entity.Property(e => e.IdbonLivraison).HasColumnName("IDBonLivraison");
            entity.Property(e => e.Idmagasin).HasColumnName("IDMagasin");
            entity.Property(e => e.Idtype).HasColumnName("IDType");
            entity.Property(e => e.IsReported).HasColumnType("datetime");
            entity.Property(e => e.NomClient).HasMaxLength(256);
            entity.Property(e => e.Remarques).HasMaxLength(1000);
            entity.Property(e => e.ResteADonner)
                .HasColumnType("decimal(24, 2)")
                .HasColumnName("Reste_A_Donner");
            entity.Property(e => e.Supprime).HasDefaultValue(false);
            entity.Property(e => e.TotalTtc)
                .HasColumnType("decimal(24, 2)")
                .HasColumnName("TotalTTC");
            entity.Property(e => e.TotatHt)
                .HasColumnType("decimal(24, 2)")
                .HasColumnName("TotatHT");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<AvoirclientXArticle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_AVOIRCLIENT-X-ARTICLE");

            entity.ToTable("AVOIRCLIENT_X_ARTICLE", tb =>
                {
                    tb.HasTrigger("TRG_DELETE_ARTICLE_AVOIR_CLIENT");
                    tb.HasTrigger("TRG_DELETE_ARTICLE_AVOIR_CLIENT_BASE_BL");
                    tb.HasTrigger("TRG_INSERT_ARTICLE_AVOIR_CLIENT");
                    tb.HasTrigger("TRG_INSERT_ARTICLE_AVOIR_CLIENT_BASE_BL");
                    tb.HasTrigger("TRG_INSERT_UPDATE_AVOIR_CLIENT");
                    tb.HasTrigger("TRG_UPDATE_ARTICLE_AVOIR_CLIENT_BASE_BL");
                });

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Designation).HasMaxLength(4000);
            entity.Property(e => e.IdLineBl).HasColumnName("IdLineBL");
            entity.Property(e => e.Iddepot).HasColumnName("IDDepot");
            entity.Property(e => e.PrixAvoir).HasColumnType("money");
            entity.Property(e => e.PrixAvoirTtc)
                .HasColumnType("money")
                .HasColumnName("PrixAvoirTTC");
            entity.Property(e => e.QteColis).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QteParColis).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QuantiteReparer)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(24, 3)");
            entity.Property(e => e.Supprime).HasDefaultValue(false);
            entity.Property(e => e.Tva).HasColumnName("TVA");

            entity.HasOne(d => d.IdAvoirClientNavigation).WithMany(p => p.AvoirclientXArticle)
                .HasForeignKey(d => d.IdAvoirClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AVOIR_X_ASSOCIATI_AVOIR");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
