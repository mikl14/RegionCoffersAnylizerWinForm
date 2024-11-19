using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RegionCoffersAnylizerWinForm.Models;

public partial class NalogiContext : DbContext
{
    public NalogiContext()
    {
    }

    public NalogiContext(DbContextOptions<NalogiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Coffers> Coffers { get; set; }

    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=nalogi;Username=postgres;Password=schef2002");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Region>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("adygea", "gs2024");

            entity.Property(e => e.Adresf).HasColumnName("adresf");
            entity.Property(e => e.FactOkvedNeosn).HasColumnName("fact_okved_neosn");
            entity.Property(e => e.FactOkvedOsn).HasColumnName("fact_okved_osn");
            entity.Property(e => e.Inn).HasColumnName("inn");
            entity.Property(e => e.License).HasColumnName("license");
            entity.Property(e => e.Naimobj).HasColumnName("naimobj");
            entity.Property(e => e.Okato).HasColumnName("okato");
            entity.Property(e => e.Okpo).HasColumnName("okpo");
            entity.Property(e => e.Oktmo).HasColumnName("oktmo");
            entity.Property(e => e.OkvedNeosn).HasColumnName("okved_neosn");
            entity.Property(e => e.OkvedOsn).HasColumnName("okved_osn");
            entity.Property(e => e.Schr).HasColumnName("schr");
            entity.Property(e => e.SposobLikvid).HasColumnName("sposob_likvid");
            entity.Property(e => e.Systemnalog).HasColumnName("systemnalog");
            entity.Property(e => e.TypeMsp).HasColumnName("type_msp");
            entity.Property(e => e.Typeermsp).HasColumnName("typeermsp");
            entity.Property(e => e.Typeofsn).HasColumnName("typeofsn");
            entity.Property(e => e.Typep).HasColumnName("typep");
            entity.Property(e => e.Viruchka).HasColumnName("viruchka");
        });


        modelBuilder.Entity<Coffers>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("june", "coffers2024");

            entity.Property(e => e.Inn).HasColumnName("inn");
            entity.Property(e => e.Slice).HasColumnName("slice");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
