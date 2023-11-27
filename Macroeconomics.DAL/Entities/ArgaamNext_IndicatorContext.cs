using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Macroeconomics.DAL.Entities;

public partial class ArgaamNext_IndicatorContext : DbContext
{
    public ArgaamNext_IndicatorContext()
    {
    }

    public ArgaamNext_IndicatorContext(DbContextOptions<ArgaamNext_IndicatorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EconomicIndicator> EconomicIndicators { get; set; }

    public virtual DbSet<EconomicIndicatorField> EconomicIndicatorFields { get; set; }

    public virtual DbSet<EconomicIndicatorGroup> EconomicIndicatorGroups { get; set; }

    public virtual DbSet<EconomicIndicatorSource> EconomicIndicatorSources { get; set; }

    public virtual DbSet<EconomicIndicatorValue> EconomicIndicatorValues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EconomicIndicator>(entity =>
        {
            entity.HasKey(e => e.EconomicIndicatorID).HasName("PK2");

            entity.HasIndex(e => new { e.SubGroupID, e.ForYear, e.FiscalPeriodID, e.EconomicIndicatorID }, "_dta_index_EconomicIndicators_5_2024446336__K7_K5_K6_K1");

            entity.HasIndex(e => new { e.IsPublished, e.SubGroupID }, "missing_26_april_index_3");

            entity.HasIndex(e => new { e.IsPublished, e.ForYear, e.FiscalPeriodID }, "missing_index_1096");

            entity.HasIndex(e => new { e.IsPublished, e.ForYear, e.FiscalPeriodID }, "missing_index_1142");

            entity.HasIndex(e => new { e.FiscalPeriodID, e.ForYear }, "missing_index_1144");

            entity.HasIndex(e => new { e.ForYear, e.FiscalPeriodID }, "missing_index_1146");

            entity.HasIndex(e => e.ForYear, "missing_index_1148");

            entity.HasIndex(e => new { e.IsPublished, e.FiscalPeriodID }, "missing_index_128");

            entity.HasIndex(e => new { e.CountryID, e.IsPublished, e.FiscalPeriodID, e.SubGroupID }, "missing_index_206186");

            entity.HasIndex(e => new { e.CountryID, e.IsPublished, e.SubGroupID }, "missing_index_206188");

            entity.HasIndex(e => new { e.CountryID, e.FiscalPeriodID, e.SubGroupID }, "missing_index_206221");

            entity.HasIndex(e => new { e.CountryID, e.SubGroupID }, "missing_index_206223");

            entity.HasIndex(e => e.FiscalPeriodID, "missing_index_33583");

            entity.HasIndex(e => e.IsPublished, "missing_index_59958");

            entity.HasIndex(e => e.ForYear, "missing_index_60948");

            entity.HasIndex(e => new { e.IsPublished, e.ForYear }, "missing_index_61065");

            entity.HasIndex(e => new { e.CountryID, e.IsPublished, e.ForYear }, "missing_index_61134");

            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<EconomicIndicatorField>(entity =>
        {
            entity.HasKey(e => e.EconomicIndicatorFieldID).HasName("PK1");

            entity.Property(e => e.DisplayNameAr)
                .HasMaxLength(256)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.DisplayNameEn)
                .HasMaxLength(256)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.GroupID).HasDefaultValueSql("((5))");

            entity.HasOne(d => d.Group).WithMany(p => p.EconomicIndicatorFields)
                .HasForeignKey(d => d.GroupID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EconomicIndicatorFields_EconomicIndicatorGroups");
        });

        modelBuilder.Entity<EconomicIndicatorGroup>(entity =>
        {
            entity.HasKey(e => e.GroupID);

            entity.Property(e => e.NameAr)
                .HasMaxLength(256)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NameEn)
                .HasMaxLength(256)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.ParentGroup).WithMany(p => p.InverseParentGroup)
                .HasForeignKey(d => d.ParentGroupID)
                .HasConstraintName("FK_EconomicIndicatorGroups_EconomicIndicatorGroups");
        });

        modelBuilder.Entity<EconomicIndicatorSource>(entity =>
        {
            entity.HasKey(e => e.EconomicIndicatorSourceID).HasName("PK_EIsInfoSources");

            entity.Property(e => e.EISourceNameAr)
                .HasMaxLength(512)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.EISourceNameEn)
                .HasMaxLength(512)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<EconomicIndicatorValue>(entity =>
        {
            entity.HasIndex(e => new { e.EconomicIndicatorID, e.EconomicIndicatorFieldID }, "IX_EconomicIndicatorValues").IsUnique();

            entity.HasIndex(e => e.EconomicIndicatorFieldID, "missing_index_56230");

            entity.HasIndex(e => e.EconomicIndicatorFieldID, "missing_index_56232");

            entity.HasIndex(e => e.ValueEn, "missing_index_62836");

            entity.Property(e => e.NoteAr)
                .HasMaxLength(256)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.NoteEn)
                .HasMaxLength(256)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.ValueAr)
                .HasMaxLength(256)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.ValueEn)
                .HasMaxLength(256)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");

            entity.HasOne(d => d.EconomicIndicatorField).WithMany(p => p.EconomicIndicatorValues)
                .HasForeignKey(d => d.EconomicIndicatorFieldID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EconomicIndicatorValues_EconomicIndicatorFields");

            entity.HasOne(d => d.EconomicIndicator).WithMany(p => p.EconomicIndicatorValues)
                .HasForeignKey(d => d.EconomicIndicatorID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EconomicIndicatorValues_EconomicIndicators");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
