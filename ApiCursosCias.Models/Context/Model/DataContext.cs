using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ApiCursosCias.Models.Context.Model;

public partial class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MaCanalInscripcion> MaCanalInscripcions { get; set; }

    public virtual DbSet<MaDepartamento> MaDepartamentos { get; set; }

    public virtual DbSet<MaDiaFestivo> MaDiaFestivos { get; set; }

    public virtual DbSet<MaEmpresa> MaEmpresas { get; set; }

    public virtual DbSet<MaEstadoAgendamiento> MaEstadoAgendamientos { get; set; }

    public virtual DbSet<MaEstadoFupa> MaEstadoFupas { get; set; }

    public virtual DbSet<MaFuncionalidad> MaFuncionalidads { get; set; }

    public virtual DbSet<MaFupa> MaFupas { get; set; }

    public virtual DbSet<MaModulo> MaModulos { get; set; }

    public virtual DbSet<MaMunicipio> MaMunicipios { get; set; }

    public virtual DbSet<MaPlantilla> MaPlantillas { get; set; }

    public virtual DbSet<MaRol> MaRols { get; set; }

    public virtual DbSet<MaRolFuncionalidad> MaRolFuncionalidads { get; set; }

    public virtual DbSet<MaSede> MaSedes { get; set; }

    public virtual DbSet<MaTiemposDescuento> MaTiemposDescuentos { get; set; }

    public virtual DbSet<MaTipoComparendo> MaTipoComparendos { get; set; }

    public virtual DbSet<MaTipoDocumento> MaTipoDocumentos { get; set; }

    public virtual DbSet<MaTipoFiltro> MaTipoFiltros { get; set; }

    public virtual DbSet<MaTipoPlantilla> MaTipoPlantillas { get; set; }

    public virtual DbSet<MaTipomodulo> MaTipomodulos { get; set; }

    public virtual DbSet<MaValorCurso> MaValorCursos { get; set; }

    public virtual DbSet<TrAgendaCurso> TrAgendaCursos { get; set; }

    public virtual DbSet<TrAgendaCursoHst> TrAgendaCursoHsts { get; set; }

    public virtual DbSet<TrConsultaComparendo> TrConsultaComparendos { get; set; }

    public virtual DbSet<TrCurso> TrCursos { get; set; }

    public virtual DbSet<TrDocente> TrDocentes { get; set; }

    public virtual DbSet<TrPagoAgendum> TrPagoAgenda { get; set; }

    public virtual DbSet<TrUsuario> TrUsuarios { get; set; }

    public virtual DbSet<TrUsuarioLogin> TrUsuarioLogins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaCanalInscripcion>(entity =>
        {
            entity.Property(e => e.Estado).HasDefaultValue("S");
        });

        modelBuilder.Entity<MaDiaFestivo>(entity =>
        {
            entity.HasKey(e => e.IdDiaFestivo).HasName("PK__Ma_DiaFe__9A431884233CE810");

            entity.Property(e => e.Estado).HasDefaultValue("S");
        });

        modelBuilder.Entity<MaEmpresa>(entity =>
        {
            entity.HasKey(e => e.IdEmpresa).HasName("PK__Ma_Empre__5EF4033EBC969425");

            entity.Property(e => e.IdEmpresa).ValueGeneratedNever();
            entity.Property(e => e.Estado).HasDefaultValue("S");

            entity.HasOne(d => d.IdMunicipioNavigation).WithMany(p => p.MaEmpresas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("IdMunicipio");
        });

        modelBuilder.Entity<MaEstadoAgendamiento>(entity =>
        {
            entity.HasKey(e => e.IdEstadoAgendamiento).HasName("PK__Ma_Estad__255E54E89DCFECD8");

            entity.Property(e => e.IdEstadoAgendamiento).ValueGeneratedNever();
        });

        modelBuilder.Entity<MaEstadoFupa>(entity =>
        {
            entity.HasKey(e => e.IdEstadoFupa).HasName("PK__Ma_Estad__F72A574CC4AD2DC6");

            entity.Property(e => e.IdEstadoFupa).ValueGeneratedNever();
        });

        modelBuilder.Entity<MaFuncionalidad>(entity =>
        {
            entity.HasKey(e => e.IdFuncionalidad).HasName("PK__Ma_Funci__1118229229333F04");
        });

        modelBuilder.Entity<MaFupa>(entity =>
        {
            entity.HasKey(e => e.IdFupa).HasName("PK__Ma_Fupa__06E6E261412408EF");

            entity.Property(e => e.IdestadoFupa).HasDefaultValue(0);
        });

        modelBuilder.Entity<MaModulo>(entity =>
        {
            entity.HasKey(e => e.IdModulo).HasName("PK__Ma_Modul__D9F1531548F28A31");

            entity.Property(e => e.Estado).HasDefaultValue("S");
        });

        modelBuilder.Entity<MaMunicipio>(entity =>
        {
            entity.HasKey(e => e.IdMunicipio).HasName("PK__Ma_Munic__61005978C0EFC245");

            entity.HasOne(d => d.CodigoDepartamentoNavigation).WithMany(p => p.MaMunicipios).HasConstraintName("CodigoDepartamento");
        });

        modelBuilder.Entity<MaPlantilla>(entity =>
        {
            entity.HasKey(e => e.IdPlantilla).HasName("PK__Ma_Plant__176EEE6CFBC00E5C");
        });

        modelBuilder.Entity<MaRol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Ma_Rol__2A49584C61DF3743");
        });

        modelBuilder.Entity<MaRolFuncionalidad>(entity =>
        {
            entity.Property(e => e.IdRolFuncionalidad).ValueGeneratedOnAdd();

            entity.HasOne(d => d.IdFuncionalidadNavigation).WithMany().HasConstraintName("IdFuncionalidad");

            entity.HasOne(d => d.IdRolNavigation).WithMany().HasConstraintName("IdRol");
        });

        modelBuilder.Entity<MaSede>(entity =>
        {
            entity.HasKey(e => e.IdSede).HasName("PK__Ma_Sede__A7780DFFE8EC71BB");

            entity.Property(e => e.IdSede).ValueGeneratedNever();
            entity.Property(e => e.Estado).HasDefaultValue("S");
        });

        modelBuilder.Entity<MaTiemposDescuento>(entity =>
        {
            entity.Property(e => e.IdTiemposDescuento).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<MaTipoComparendo>(entity =>
        {
            entity.HasKey(e => e.IdTipoComparendo).HasName("PK__Ma_TipoC__1EDDA9BE1EE1DFAE");

            entity.Property(e => e.IdTipoComparendo).ValueGeneratedNever();
        });

        modelBuilder.Entity<MaTipoDocumento>(entity =>
        {
            entity.HasKey(e => e.IdTipoDocumento).HasName("PK__Ma_TipoD__3AB3332F48E1497B");

            entity.Property(e => e.IdTipoDocumento).ValueGeneratedNever();
        });

        modelBuilder.Entity<MaTipoFiltro>(entity =>
        {
            entity.HasKey(e => e.IdTipoFiltro).HasName("PK__Ma_TipoF__C9B3EF94D9731245");

            entity.Property(e => e.Estado).HasDefaultValue("S");
        });

        modelBuilder.Entity<MaTipoPlantilla>(entity =>
        {
            entity.HasKey(e => e.IdTipoPlantilla).HasName("PK__Ma_tipoP__D49B0509BA4243BD");
        });

        modelBuilder.Entity<MaTipomodulo>(entity =>
        {
            entity.Property(e => e.IdTipomodulo).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<MaValorCurso>(entity =>
        {
            entity.HasKey(e => e.IdValorCurso).HasName("PK__Ma_Valor__69D62BB62E3C986A");

            entity.Property(e => e.Estado).HasDefaultValue("S");
        });

        modelBuilder.Entity<TrAgendaCurso>(entity =>
        {
            entity.HasKey(e => e.IdAgendaCurso).HasName("PK__Tr_Agend__C0B9B768573E8215");

            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TrAgendaCursoHst>(entity =>
        {
            entity.Property(e => e.Comentario).HasDefaultValue("VENCIDA");
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TrConsultaComparendo>(entity =>
        {
            entity.HasKey(e => e.IdConsultaComparendo).HasName("PK__Tr_Consu__57DE24197E4E05EE");
        });

        modelBuilder.Entity<TrCurso>(entity =>
        {
            entity.HasKey(e => e.IdCurso).HasName("PK__Tr_curso__085F27D63266074A");

            entity.Property(e => e.Estado).HasDefaultValue("S");
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Ocupado).HasDefaultValue(0);
        });

        modelBuilder.Entity<TrDocente>(entity =>
        {
            entity.Property(e => e.FechaIngreso).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdDocente).ValueGeneratedOnAdd();
            entity.Property(e => e.IdEstado).HasDefaultValue(1);
            entity.Property(e => e.IdTipoDocumento).HasDefaultValue(1);
            entity.Property(e => e.IdUsuarioRegistra).HasDefaultValue(1);
        });

        modelBuilder.Entity<TrPagoAgendum>(entity =>
        {
            entity.HasKey(e => e.IdPagoAgenda).HasName("PK__Tr_PagoA__841D0AE2198C58E0");

            entity.Property(e => e.FechaPago).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TrUsuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Tr_Usuar__5B65BF979829104F");

            entity.Property(e => e.ClaveActualizada).HasDefaultValue(false);
            entity.Property(e => e.FechaIngreso).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdEstado).HasDefaultValue(1);
            entity.Property(e => e.IdTipoDocumento).HasDefaultValue(1);
            entity.Property(e => e.IdUsuarioRegistra).HasDefaultValue(1);
        });

        modelBuilder.Entity<TrUsuarioLogin>(entity =>
        {
            entity.HasKey(e => e.IdLogin).HasName("PK__Tr_Usuar__C3C6C6F1705255A4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
