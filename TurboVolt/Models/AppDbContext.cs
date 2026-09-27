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
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public virtual DbSet<Blivraison> Blivraison { get; set; }

    public virtual DbSet<BlivraisonXArticle> BlivraisonXArticle { get; set; }

    public virtual DbSet<Client> Client { get; set; }

    public virtual DbSet<User> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blivraison>(entity =>
        {
            entity.HasKey(e => e.IdBonLivraison);

            entity.ToTable("BLIVRAISON");

            entity.HasIndex(e => new { e.DateCreation, e.DateLivraison, e.DateModification, e.IdBonLivraison, e.IdClient, e.Idmagasin, e.RefBonLivraison, e.Supprime, e.IdExercice }, "IDX_BLIVRAISON");

            entity.Property(e => e.Adresse).HasMaxLength(500);
            entity.Property(e => e.ConditionReglement).HasMaxLength(1000);
            entity.Property(e => e.DateCreation)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.DateLivraison).HasColumnType("datetime");
            entity.Property(e => e.DateModification).HasColumnType("smalldatetime");
            entity.Property(e => e.DateSupprimer).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(256);
            entity.Property(e => e.DroitTimbre).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Escompte).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EtatBonLivraison).HasMaxLength(100);
            entity.Property(e => e.IdBl).HasColumnName("IdBL");
            entity.Property(e => e.Idmagasin).HasColumnName("IDMagasin");
            entity.Property(e => e.Iduser).HasColumnName("IDUser");
            entity.Property(e => e.IsFromFacture).HasMaxLength(50);
            entity.Property(e => e.IsFromStock).HasDefaultValue(0);
            entity.Property(e => e.IsReported).HasColumnType("datetime");
            entity.Property(e => e.IsTransfere).HasDefaultValue(0);
            entity.Property(e => e.MotifModification).HasMaxLength(50);
            entity.Property(e => e.NomClient).HasMaxLength(256);
            entity.Property(e => e.NumeroFacture).HasMaxLength(100);
            entity.Property(e => e.RefBonLivraison)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RefCommande).HasMaxLength(100);
            entity.Property(e => e.RemiseFamClt).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RemiseSur).HasMaxLength(50);
            entity.Property(e => e.ResteAPayer)
                .HasColumnType("decimal(24, 2)")
                .HasColumnName("Reste_A_Payer");
            entity.Property(e => e.Supprime).HasDefaultValue(false);
            entity.Property(e => e.Telephone).HasMaxLength(50);
            entity.Property(e => e.TotalBonLivraisonHt)
                .HasColumnType("decimal(24, 2)")
                .HasColumnName("TotalBonLivraisonHT");
            entity.Property(e => e.TotalBonLivraisonTtc)
                .HasColumnType("decimal(24, 2)")
                .HasColumnName("TotalBonLivraisonTTC");
            entity.Property(e => e.TotalRemise).HasColumnType("decimal(24, 2)");
            entity.Property(e => e.TypeRemise).HasMaxLength(50);
            entity.Property(e => e.Ville).HasMaxLength(100);

            entity.HasOne(d => d.IdUserModificationNavigation).WithMany(p => p.BlivraisonIdUserModificationNavigation)
                .HasForeignKey(d => d.IdUserModification)
                .HasConstraintName("FK_BLIVRAISON_USER1");

            entity.HasOne(d => d.IduserNavigation).WithMany(p => p.BlivraisonIduserNavigation)
                .HasForeignKey(d => d.Iduser)
                .HasConstraintName("FK_BLIVRAISON_USER");
        });

        modelBuilder.Entity<BlivraisonXArticle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_BLIVRAISON-X-ARTICLE");

            entity.ToTable("BLIVRAISON_X_ARTICLE", tb =>
                {
                    tb.HasTrigger("TRG_AJOUTER_BENEFICE_USER");
                    tb.HasTrigger("TRG_DELETE_ARTICLE_BL");
                    tb.HasTrigger("TRG_INSERT_ARTICLE_BL");
                    tb.HasTrigger("TRG_MODIFIER_BENEFICE_USER");
                    tb.HasTrigger("TRG_SUPPRIMER_BENEFICE_USER");
                    tb.HasTrigger("TRG_UPDATE_ARTICLE_BL");
                });

            entity.HasIndex(e => new { e.IdArticle, e.Iddepot, e.PrixBonLivraisonTtc, e.QuantiteLivree, e.IdBonLivraison }, "IDX_BLIVRAISON_X_ARTICLE_IdBonLivraison");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateCreation)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("smalldatetime");
            entity.Property(e => e.DateModification).HasColumnType("smalldatetime");
            entity.Property(e => e.DernierPrixAchatTtc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DernierPrixAchatTTC");
            entity.Property(e => e.Designation).HasMaxLength(4000);
            entity.Property(e => e.Iddepot).HasColumnName("IDDepot");
            entity.Property(e => e.IndexOrder).HasDefaultValue(0);
            entity.Property(e => e.MontantHt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("MontantHT");
            entity.Property(e => e.MontantRemise).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MontantTtc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("MontantTTC");
            entity.Property(e => e.PrixBonLivraison).HasColumnType("decimal(24, 2)");
            entity.Property(e => e.PrixBonLivraisonTtc)
                .HasColumnType("decimal(24, 2)")
                .HasColumnName("PrixBonLivraisonTTC");
            entity.Property(e => e.QteAvoirRetour).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QteColis).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QteColisAvoirRetour).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.QteParColis).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RemiseFamClt).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RemiseSur).HasMaxLength(50);
            entity.Property(e => e.TauxRemise).HasColumnType("decimal(24, 2)");
            entity.Property(e => e.TauxTva)
                .HasColumnType("decimal(24, 2)")
                .HasColumnName("TauxTVA");

            entity.HasOne(d => d.IdBonLivraisonNavigation).WithMany(p => p.BlivraisonXArticle)
                .HasForeignKey(d => d.IdBonLivraison)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BLIVRAIS_ASSOCIATI_BLIVRAIS");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient);

            entity.ToTable("CLIENT");

            entity.Property(e => e.AddresseFacturation)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Adresse).HasMaxLength(500);
            entity.Property(e => e.Agence)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Banque)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.BonusPoints).HasDefaultValue(0);
            entity.Property(e => e.ClientDivers).HasDefaultValue(false);
            entity.Property(e => e.Compte)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.ContactClient).HasMaxLength(100);
            entity.Property(e => e.Credit)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(24, 2)");
            entity.Property(e => e.DateCreation)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateModification).HasColumnType("datetime");
            entity.Property(e => e.Debit)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(24, 2)");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.EmailClient).HasMaxLength(100);
            entity.Property(e => e.Ice)
                .HasMaxLength(50)
                .HasColumnName("ICE");
            entity.Property(e => e.IsBloque).HasColumnName("isBloque");
            entity.Property(e => e.IsCoupon).HasColumnName("isCoupon");
            entity.Property(e => e.IsPoints)
                .HasDefaultValue(false)
                .HasColumnName("isPoints");
            entity.Property(e => e.IsTransport).HasColumnName("isTransport");
            entity.Property(e => e.NomClient).HasMaxLength(100);
            entity.Property(e => e.NumFax).HasMaxLength(100);
            entity.Property(e => e.NumTele).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(1000);
            entity.Property(e => e.ProgId).HasColumnName("ProgID");
            entity.Property(e => e.RefClt).HasMaxLength(100);
            entity.Property(e => e.SiteWebClient).HasMaxLength(100);
            entity.Property(e => e.SoldeMaximum).HasColumnType("decimal(24, 2)");
            entity.Property(e => e.Supprime).HasDefaultValue(false);
            entity.Property(e => e.Username).HasMaxLength(250);
            entity.Property(e => e.Ville).HasMaxLength(100);
            entity.Property(e => e.WebToken).HasMaxLength(1000);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("USER");

            entity.Property(e => e.DateCreation)
                .HasMaxLength(100)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DateModification).HasMaxLength(100);
            entity.Property(e => e.DeviceId)
                .HasMaxLength(100)
                .HasColumnName("deviceID");
            entity.Property(e => e.DeviceName)
                .HasMaxLength(50)
                .HasColumnName("deviceName");
            entity.Property(e => e.Fonction).HasMaxLength(100);
            entity.Property(e => e.Idrole).HasColumnName("IDRole");
            entity.Property(e => e.IsAdmin)
                .HasDefaultValue(false)
                .HasColumnName("isAdmin");
            entity.Property(e => e.IsCoWorker).HasColumnName("isCoWorker");
            entity.Property(e => e.IsCommerciale).HasColumnName("isCommerciale");
            entity.Property(e => e.IsCoupon).HasColumnName("isCoupon");
            entity.Property(e => e.IsDeviceLogin)
                .HasDefaultValue(false)
                .HasColumnName("isDeviceLogin");
            entity.Property(e => e.IsLivreur)
                .HasDefaultValue(false)
                .HasColumnName("isLivreur");
            entity.Property(e => e.IsTransport).HasColumnName("isTransport");
            entity.Property(e => e.Nom).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(256);
            entity.Property(e => e.PlayerId)
                .HasMaxLength(256)
                .HasColumnName("playerID");
            entity.Property(e => e.Prenom).HasMaxLength(100);
            entity.Property(e => e.Supprime).HasDefaultValue(false);
            entity.Property(e => e.Username).HasMaxLength(256);
            entity.Property(e => e.ValidationDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
