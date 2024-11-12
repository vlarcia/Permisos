using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Entity;

namespace Data.DBContext;

public partial class CumplimientoContext : DbContext
{
    public CumplimientoContext()
    {
    }

    public CumplimientoContext(DbContextOptions<CumplimientoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actividad> Actividades { get; set; }

    public virtual DbSet<AndroidId> AndroidIds { get; set; }

    public virtual DbSet<CheckList> CheckLists { get; set; }

    public virtual DbSet<Configuracion> Configuracions { get; set; }

    public virtual DbSet<DetalleVisita> DetalleVisitas { get; set; }

    public virtual DbSet<MaestroFinca> MaestroFincas { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Negocio> Negocios { get; set; }

    public virtual DbSet<NumeroCorrelativo> NumeroCorrelativos { get; set; }

    public virtual DbSet<PlanesTrabajo> PlanesTrabajos { get; set; }

    public virtual DbSet<Revisione> Revisiones { get; set; }

    public virtual DbSet<Revision> Revisions { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<RolMenu> RolMenus { get; set; }

    public virtual DbSet<TipoInforme> TipoInformes { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Usuario1> Usuarios1 { get; set; }

    public virtual DbSet<Visita> Visitas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actividad>(entity =>
        {
            entity.HasKey(e => e.IdActividad);

            entity.Property(e => e.IdActividad).HasColumnName("id_actividad");
            entity.Property(e => e.Avanceanterior)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("avanceanterior");
            entity.Property(e => e.Avances)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("avances");
            entity.Property(e => e.Comentarios)
                .HasMaxLength(300)
                .HasColumnName("comentarios");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasColumnName("estado");
            entity.Property(e => e.FechaFin)
                .HasColumnType("date")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.FechaIni)
                .HasColumnType("date")
                .HasColumnName("fecha_ini");
            entity.Property(e => e.FechaUltimarevision)
                .HasColumnType("date")
                .HasColumnName("fecha_ultimarevision");
            entity.Property(e => e.IdFinca).HasColumnName("id_finca");
            entity.Property(e => e.IdPlan).HasColumnName("id_plan");
            entity.Property(e => e.IdRequisito).HasColumnName("id_requisito");
            entity.Property(e => e.Recursos)
                .HasMaxLength(150)
                .HasColumnName("recursos");
            entity.Property(e => e.Responsable)
                .HasMaxLength(50)
                .HasColumnName("responsable");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdFincaNavigation).WithMany(p => p.Actividades)
                .HasForeignKey(d => d.IdFinca)
                .HasConstraintName("FK_Actividades_MaestroFinca");

            entity.HasOne(d => d.IdPlanNavigation).WithMany(p => p.Actividades)
                .HasForeignKey(d => d.IdPlan)
                .HasConstraintName("FK_Actividades_PlanesTrabajo");
        });

        modelBuilder.Entity<AndroidId>(entity =>
        {
            entity.HasKey(e => e.IdAndroid);

            entity.ToTable("AndroidId");

            entity.Property(e => e.IdAndroid).HasColumnName("id_android");
            entity.Property(e => e.Androidid1)
                .HasMaxLength(40)
                .HasColumnName("androidid");
            entity.Property(e => e.Dispositvo)
                .HasMaxLength(100)
                .HasColumnName("dispositvo");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasDefaultValueSql("(N'N/A')")
                .HasColumnName("email");
        });

        modelBuilder.Entity<CheckList>(entity =>
        {
            entity.HasKey(e => e.IdRequisito);

            entity.ToTable("CheckList");

            entity.Property(e => e.IdRequisito).HasColumnName("id_requisito");
            entity.Property(e => e.Ambito)
                .HasMaxLength(255)
                .HasColumnName("ambito");
            entity.Property(e => e.Bonsucro)
                .HasMaxLength(255)
                .HasColumnName("bonsucro");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Norma)
                .HasMaxLength(255)
                .HasColumnName("norma");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
        });

        modelBuilder.Entity<Configuracion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Configuracion");

            entity.Property(e => e.Propiedad)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("propiedad");
            entity.Property(e => e.Recurso)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("recurso");
            entity.Property(e => e.Valor)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("valor");
        });

        modelBuilder.Entity<DetalleVisita>(entity =>
        {
            entity.HasKey(e => e.IdReg);

            entity.Property(e => e.IdReg).HasColumnName("id_reg");
            entity.Property(e => e.Avanceanterior)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("avanceanterior");
            entity.Property(e => e.Avances)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("avances");
            entity.Property(e => e.Comentarios)
                .HasMaxLength(250)
                .HasColumnName("comentarios");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasColumnName("estado");
            entity.Property(e => e.Estadoanterior)
                .HasMaxLength(20)
                .HasColumnName("estadoanterior");
            entity.Property(e => e.Fecha)
                .HasColumnType("date")
                .HasColumnName("fecha");
            entity.Property(e => e.IdActividad).HasColumnName("id_actividad");
            entity.Property(e => e.IdFinca).HasColumnName("id_finca");
            entity.Property(e => e.IdVisita).HasColumnName("id_visita");
            entity.Property(e => e.Nombrefoto1)
                .HasMaxLength(100)
                .HasColumnName("nombrefoto1");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(250)
                .HasColumnName("observaciones");
            entity.Property(e => e.Urlfoto1)
                .HasMaxLength(500)
                .HasColumnName("urlfoto1");

            entity.HasOne(d => d.IdActividadNavigation).WithMany(p => p.DetalleVisita)
                .HasForeignKey(d => d.IdActividad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleVisitas_Actividades");

            entity.HasOne(d => d.IdFincaNavigation).WithMany(p => p.DetalleVisita)
                .HasForeignKey(d => d.IdFinca)
                .HasConstraintName("FK_DetalleVisitas_MaestroFinca");

            entity.HasOne(d => d.IdVisitaNavigation).WithMany(p => p.DetalleVisita)
                .HasForeignKey(d => d.IdVisita)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleVisitas_Visitas");
        });

        modelBuilder.Entity<MaestroFinca>(entity =>
        {
            entity.HasKey(e => e.IdFinca);

            entity.ToTable("MaestroFinca");

            entity.Property(e => e.IdFinca).HasColumnName("id_finca");
            entity.Property(e => e.Area)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("area");
            entity.Property(e => e.CodFinca).HasColumnName("cod_finca");
            entity.Property(e => e.DatetimeUpdate)
                .HasColumnType("datetime")
                .HasColumnName("datetime_update");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("email");
            entity.Property(e => e.Emailsuper)
                .HasMaxLength(50)
                .HasColumnName("emailsuper");
            entity.Property(e => e.Empresa)
                .HasMaxLength(30)
                .HasColumnName("empresa");
            entity.Property(e => e.Encargado)
                .HasMaxLength(100)
                .HasColumnName("encargado");
            entity.Property(e => e.Grupo).HasColumnName("grupo");
            entity.Property(e => e.Propiedad).HasColumnName("propiedad");
            entity.Property(e => e.Proveedor)
                .HasMaxLength(200)
                .HasColumnName("proveedor");
            entity.Property(e => e.Supervisor)
                .HasMaxLength(100)
                .HasColumnName("supervisor");
            entity.Property(e => e.Tecnico)
                .HasMaxLength(100)
                .HasColumnName("tecnico");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .HasColumnName("telefono");
            entity.Property(e => e.Variedad)
                .HasMaxLength(30)
                .HasColumnName("variedad");
            entity.Property(e => e.Zona)
                .HasMaxLength(30)
                .HasColumnName("zona");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdMenu).HasName("PK__Menu__C26AF483F3CF62CE");

            entity.ToTable("Menu");

            entity.Property(e => e.IdMenu).HasColumnName("idMenu");
            entity.Property(e => e.Controlador)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("controlador");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Icono)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("icono");
            entity.Property(e => e.IdMenuPadre).HasColumnName("idMenuPadre");
            entity.Property(e => e.PaginaAccion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("paginaAccion");

            entity.HasOne(d => d.IdMenuPadreNavigation).WithMany(p => p.InverseIdMenuPadreNavigation)
                .HasForeignKey(d => d.IdMenuPadre)
                .HasConstraintName("FK__Menu__idMenuPadr__4CA06362");
        });

        modelBuilder.Entity<Negocio>(entity =>
        {
            entity.HasKey(e => e.IdNegocio).HasName("PK__Negocio__70E1E1071F41FBCE");

            entity.ToTable("Negocio");

            entity.Property(e => e.IdNegocio)
                .ValueGeneratedNever()
                .HasColumnName("idNegocio");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreLogo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreLogo");
            entity.Property(e => e.NumeroDocumento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("numeroDocumento");
            entity.Property(e => e.PorcentajeImpuesto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("porcentajeImpuesto");
            entity.Property(e => e.SimboloMoneda)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("simboloMoneda");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.UrlLogo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlLogo");
        });

        modelBuilder.Entity<NumeroCorrelativo>(entity =>
        {
            entity.HasKey(e => e.IdNumeroCorrelativo).HasName("PK__NumeroCo__25FB547E51A5919B");

            entity.ToTable("NumeroCorrelativo");

            entity.Property(e => e.IdNumeroCorrelativo).HasColumnName("idNumeroCorrelativo");
            entity.Property(e => e.CantidadDigitos).HasColumnName("cantidadDigitos");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Gestion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("gestion");
            entity.Property(e => e.UltimoNumero).HasColumnName("ultimoNumero");
        });

        modelBuilder.Entity<PlanesTrabajo>(entity =>
        {
            entity.HasKey(e => e.IdPlan);

            entity.ToTable("PlanesTrabajo");

            entity.Property(e => e.IdPlan).HasColumnName("id_plan");
            entity.Property(e => e.Avance)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("avance");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasColumnName("estado");
            entity.Property(e => e.FechaFin)
                .HasColumnType("date")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.FechaIni)
                .HasColumnType("date")
                .HasColumnName("fecha_ini");
            entity.Property(e => e.IdFinca).HasColumnName("id_finca");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(200)
                .HasColumnName("observaciones");

            entity.HasOne(d => d.IdFincaNavigation).WithMany(p => p.PlanesTrabajos)
                .HasForeignKey(d => d.IdFinca)
                .HasConstraintName("FK_PlanesTrabajo_MaestroFinca");
        });

        modelBuilder.Entity<Revisione>(entity =>
        {
            entity.HasKey(e => e.IdRevision);

            entity.Property(e => e.IdRevision).HasColumnName("id_revision");
            entity.Property(e => e.Comentarios)
                .HasMaxLength(250)
                .HasColumnName("comentarios");
            entity.Property(e => e.Cumplimiento).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.Estado)
                .HasMaxLength(15)
                .HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasColumnType("date")
                .HasColumnName("fecha");
            entity.Property(e => e.Grupo).HasColumnName("grupo");
            entity.Property(e => e.IdFinca).HasColumnName("id_finca");
            entity.Property(e => e.IdRequisito).HasColumnName("id_requisito");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.Tipo)
                .HasMaxLength(15)
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdFincaNavigation).WithMany(p => p.Revisiones)
                .HasForeignKey(d => d.IdFinca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Revisiones_MaestroFinca");

            entity.HasOne(d => d.IdRequisitoNavigation).WithMany(p => p.Revisiones)
                .HasForeignKey(d => d.IdRequisito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Revisiones_CheckList");
        });

        modelBuilder.Entity<Revision>(entity =>
        {
            entity.HasKey(e => e.IdReg);

            entity.Property(e => e.IdReg).HasColumnName("id_reg");
           
            entity.Property(e => e.Cumplimiento).HasColumnType("decimal(18, 3)").HasColumnName("cumplimiento");
           
            entity.Property(e => e.Fecha).HasColumnType("date").HasColumnName("fecha");
           
            entity.Property(e => e.IdFinca).HasColumnName("id_finca");
           
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.Nombrefoto1).HasMaxLength(100).HasColumnName("nombrefoto1");
            entity.Property(e => e.Nombrefoto2).HasMaxLength(100).HasColumnName("nombrefoto2");            
            entity.Property(e => e.SentTo).HasColumnName("sent_to");
            entity.Property(e => e.Urlfoto1).HasMaxLength(500).HasColumnName("urlfoto1");
            entity.Property(e => e.Urlfoto2).HasMaxLength(500).HasColumnName("urlfoto2");

            entity.HasOne(d => d.IdFincaNavigation).WithMany(p=>p.Revision)
                .HasForeignKey(d => d.IdFinca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Revisions_MaestroFinca");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__3C872F766F396DCE");

            entity.ToTable("Rol");

            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
        });

        modelBuilder.Entity<RolMenu>(entity =>
        {
            entity.HasKey(e => e.IdRolMenu).HasName("PK__RolMenu__CD2045D863B9180F");

            entity.ToTable("RolMenu");

            entity.Property(e => e.IdRolMenu).HasColumnName("idRolMenu");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdMenu).HasColumnName("idMenu");
            entity.Property(e => e.IdRol).HasColumnName("idRol");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.RolMenus)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__RolMenu__idRol__5070F446");
        });

        modelBuilder.Entity<TipoInforme>(entity =>
        {
            entity.HasKey(e => e.IdTipoInforme).HasName("PK__TipoInfo__5F1A7A0DEDAD506D");

            entity.ToTable("TipoInforme");

            entity.Property(e => e.IdTipoInforme).HasColumnName("idTipoInforme");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__645723A617AD72F1");

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Clave)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("clave");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreFoto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreFoto");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.UrlFoto)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("urlFoto");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Usuario__idRol__5165187F");
        });

        modelBuilder.Entity<Usuario1>(entity =>
        {
            entity.HasKey(e => e.Iduser);

            entity.ToTable("Usuarios");

            entity.Property(e => e.Iduser).HasColumnName("iduser");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Admin).HasColumnName("admin");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellidos");
            entity.Property(e => e.Cartera).HasColumnName("cartera");
            entity.Property(e => e.Codcomedor).HasColumnName("codcomedor");
            entity.Property(e => e.Especial).HasColumnName("especial");
            entity.Property(e => e.Fechaing)
                .HasColumnType("date")
                .HasColumnName("fechaing");
            entity.Property(e => e.Foto).HasColumnName("foto");
            entity.Property(e => e.Inventarios).HasColumnName("inventarios");
            entity.Property(e => e.Login)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("login");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Passwd)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("passwd");
            entity.Property(e => e.Reporte).HasColumnName("reporte");
            entity.Property(e => e.Ventas).HasColumnName("ventas");
        });

        modelBuilder.Entity<Visita>(entity =>
        {
            entity.HasKey(e => e.IdVisita);

            entity.Property(e => e.IdVisita).HasColumnName("id_visita");
            entity.Property(e => e.AndroidId)
                .HasMaxLength(30)
                .HasColumnName("android_id");
            entity.Property(e => e.Fecha)
                .HasColumnType("date")
                .HasColumnName("fecha");
            entity.Property(e => e.IdFinca).HasColumnName("id_finca");
            entity.Property(e => e.IdPlan).HasColumnName("id_plan");
            entity.Property(e => e.Latitud)
                .HasMaxLength(20)
                .HasColumnName("latitud");
            entity.Property(e => e.Longitud)
                .HasMaxLength(20)
                .HasColumnName("longitud");
            entity.Property(e => e.Mandador)
                .HasMaxLength(100)
                .HasColumnName("mandador");
            entity.Property(e => e.Nombrefoto1)
                .HasMaxLength(100)
                .HasColumnName("nombrefoto1");
            entity.Property(e => e.Nombrefoto2)
                .HasMaxLength(100)
                .HasColumnName("nombrefoto2");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(250)
                .HasColumnName("observaciones");
            entity.Property(e => e.Responsable)
                .HasMaxLength(100)
                .HasColumnName("responsable");
            entity.Property(e => e.SentTo).HasColumnName("sent_to");
            entity.Property(e => e.Urlfoto1)
                .HasMaxLength(500)
                .HasColumnName("urlfoto1");
            entity.Property(e => e.Urlfoto2)
                .HasMaxLength(500)
                .HasColumnName("urlfoto2");
            entity.Property(e => e.Zafra).HasColumnName("zafra");

            entity.HasOne(d => d.IdFincaNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.IdFinca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Visitas_MaestroFinca");

            entity.HasOne(d => d.IdPlanNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.IdPlan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Visitas_PlanesTrabajo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
