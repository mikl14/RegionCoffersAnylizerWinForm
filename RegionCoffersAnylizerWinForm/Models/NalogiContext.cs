using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        => optionsBuilder.UseNpgsql("Host=10.177.33.105;Port=5432;Database=coffers2024;Username=postgres;Password=schef2002");


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
   


        modelBuilder.Entity<Region>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("volgograd", "gs2024");

            entity.Property(e => e.Inn).HasColumnName("inn");
            entity.Property(e => e.Okpo).HasColumnName("okpo");
            entity.Property(e => e.Naimobj).HasColumnName("name");
            entity.Property(e => e.Adresf).HasColumnName("adresf");
            entity.Property(e => e.Okatof).HasColumnName("okatof");
            entity.Property(e => e.TypeObj).HasColumnName("type_obj");
            entity.Property(e => e.TypeMsp).HasColumnName("type_msp");
            entity.Property(e => e.Typeermsp).HasColumnName("type_ermsp");
            entity.Property(e => e.OkvedNeosn).HasColumnName("okved2_neosn");
            entity.Property(e => e.OkvedOsn).HasColumnName("okved2_osn");
            entity.Property(e => e.FactOkvedNeosn).HasColumnName("okved2_fact_neosn");
            entity.Property(e => e.FactOkvedOsn).HasColumnName("okved2_fact_osn");

            entity.Property(e => e.SchrFns).HasColumnName("schr_fns");
            entity.Property(e => e.SchrActualDate).HasColumnName("schr_fns_actual_date");
            entity.Property(e => e.Schr).HasColumnName("schr");
            entity.Property(e => e.Viruchka).HasColumnName("viruchka");
            entity.Property(e => e.Systemnalog).HasColumnName("system_nalog");
            entity.Property(e => e.License).HasColumnName("license");
            entity.Property(e => e.kpp).HasColumnName("kpp");
            entity.Property(e => e.actualType).HasColumnName("actual_type");
            entity.Property(e => e.actualDate).HasColumnName("actual_date");

            entity.Property(e => e.regAdress).HasColumnName("registry_adress");

            entity.Property(e => e.factoryType).HasColumnName("factory_type");

            entity.Property(e => e.countTosp).HasColumnName("count_tosp");

            entity.Property(e => e.usingUsn).HasColumnName("using_usn");

            entity.Property(e => e.oborot).HasColumnName("oborot");

            entity.Property(e => e.Oktmof).HasColumnName("oktmof");


            entity.Property(e => e.lifeEndDate).HasColumnName("life_end_date");

            entity.Property(e => e.SposobLikvid).HasColumnName("sposob_likvid");
          
        
          
        });


        modelBuilder.Entity<Coffers>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("april", "coffers2024");

            entity.Property(e => e.Inn).HasColumnName("inn");
            entity.Property(e => e.Slice).HasColumnName("slice");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
