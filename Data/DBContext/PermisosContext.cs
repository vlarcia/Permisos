using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Data.DBContext;

public partial class PermisosContext : DbContext
{
    public PermisosContext()
    {
    }

    public PermisosContext(DbContextOptions<PermisosContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<TbAlerta> TbAlertas { get; set; }

    public virtual DbSet<TbArea> TbAreas { get; set; }

    public virtual DbSet<TbConfiguracion> TbConfiguracions { get; set; }

    public virtual DbSet<TbDestinatario> TbDestinatarios { get; set; }

    public virtual DbSet<TbMenu> TbMenus { get; set; }

    public virtual DbSet<TbNegocio> TbNegocios { get; set; }

    public virtual DbSet<TbPermiso> TbPermisos { get; set; }

    public virtual DbSet<TbPermisoDestinatario> TbPermisoDestinatarios { get; set; }

    public virtual DbSet<TbRol> TbRols { get; set; }

    public virtual DbSet<TbRolMenu> TbRolMenus { get; set; }

    public virtual DbSet<TbUsuario> TbUsuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      
        modelBuilder.Entity<TbAlerta>(entity =>
        {
            entity.HasKey(e => e.IdAlerta).HasName("PK__Tb_Alert__D0995427AEE085F9");

            entity.ToTable("Tb_Alertas");

            entity.Property(e => e.IdAlerta).HasColumnName("idAlerta");
            entity.Property(e => e.FechaEnvio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaEnvio");
            entity.Property(e => e.IdDestinatario).HasColumnName("idDestinatario");
            entity.Property(e => e.IdPermiso).HasColumnName("idPermiso");
            entity.Property(e => e.MedioEnvio)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("medioEnvio");
            entity.Property(e => e.Mensaje)
                .HasMaxLength(500)
                .HasColumnName("mensaje");
            entity.Property(e => e.Resultado)
                .HasMaxLength(100)
                .HasColumnName("resultado");

            entity.HasOne(d => d.IdDestinatarioNavigation).WithMany(p => p.TbAlerta)
                .HasForeignKey(d => d.IdDestinatario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tb_Alerta__idDes__2DE6D218");

            entity.HasOne(d => d.IdPermisoNavigation).WithMany(p => p.TbAlerta)
                .HasForeignKey(d => d.IdPermiso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tb_Alerta__idPer__2EDAF651");
        });

        modelBuilder.Entity<TbArea>(entity =>
        {
            entity.HasKey(e => e.IdArea).HasName("PK__Tb_Area__750ECEA4F0089A70");

            entity.ToTable("Tb_Area");

            entity.Property(e => e.IdArea).HasColumnName("idArea");
            entity.Property(e => e.Activo)
                .HasDefaultValueSql("((1))")
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TbConfiguracion>(entity =>
        {
            entity.HasKey(e => e.IdReg);

            entity.ToTable("Tb_Configuracion");

            entity.Property(e => e.IdReg).HasColumnName("id_reg");
            entity.Property(e => e.Propiedad)
                .HasMaxLength(50)
                .HasColumnName("propiedad");
            entity.Property(e => e.Recurso)
                .HasMaxLength(50)
                .HasColumnName("recurso");
            entity.Property(e => e.Valor)
                .HasMaxLength(60)
                .HasColumnName("valor");
        });

        modelBuilder.Entity<TbDestinatario>(entity =>
        {
            entity.HasKey(e => e.IdDestinatario).HasName("PK__Tb_Desti__5D5A4C88A642D5CA");

            entity.ToTable("Tb_Destinatarios");

            entity.Property(e => e.IdDestinatario).HasColumnName("idDestinatario");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdArea).HasColumnName("idArea");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.RecibeAlta).HasColumnName("recibeAlta");
            entity.Property(e => e.RecibeBaja).HasColumnName("recibeBaja");
            entity.Property(e => e.RecibeMedia).HasColumnName("recibeMedia");
            entity.Property(e => e.TelefonoWhatsapp)
                .HasMaxLength(25)
                .HasColumnName("telefonoWhatsapp");


            entity.HasOne(d => d.IdAreaNavigation).WithMany(p => p.TbDestinatarios)
                .HasForeignKey(d => d.IdArea)
                .HasConstraintName("FK_Destinatario_Area");
        });

        modelBuilder.Entity<TbMenu>(entity =>
        {
            entity.HasKey(e => e.IdMenu).HasName("PK__Tb_Menu__C26AF483B9998698");

            entity.ToTable("Tb_Menu");

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
                .HasConstraintName("FK__Tb_Menu__idMenuP__403A8C7D");
        });

        modelBuilder.Entity<TbNegocio>(entity =>
        {
            entity.HasKey(e => e.IdNegocio).HasName("PK__Tb_Negoc__70E1E107D25C4EB3");

            entity.ToTable("Tb_Negocio");

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

        modelBuilder.Entity<TbPermiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("PK__Tb_Permi__06A5848611F542BE");

            entity.ToTable("Tb_Permisos");

            entity.Property(e => e.IdPermiso).HasColumnName("idPermiso");
            entity.Property(e => e.Criticidad)
                .HasMaxLength(10)
                .HasColumnName("criticidad");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.DiasGestion).HasColumnName("diasGestion");
            entity.Property(e => e.Encargado)
                .HasMaxLength(150)
                .HasColumnName("encargado");
            entity.Property(e => e.EstadoPermiso)
                .HasMaxLength(20)
                .HasColumnName("estadoPermiso");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaModificacion");
            entity.Property(e => e.FechaVencimiento)
                .HasColumnType("date")
                .HasColumnName("fechaVencimiento");
            entity.Property(e => e.IdArea).HasColumnName("idArea");
            entity.Property(e => e.Institucion)
                .HasMaxLength(500)
                .HasColumnName("institucion");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.Tipo)
                .HasMaxLength(10)
                .HasColumnName("tipo");
            entity.Property(e => e.NombreEvidencia).HasMaxLength(150).HasColumnName("nombreEvidencia");            
            entity.Property(e => e.UrlEvidencia).HasColumnName("urlEvidencia");
            entity.Property(e => e.NombreEvidencia2).HasMaxLength(150).HasColumnName("nombreEvidencia2");
            entity.Property(e => e.UrlEvidencia2).HasColumnName("urlEvidencia2");
            entity.Property(e => e.NombreEvidencia3).HasMaxLength(150).HasColumnName("nombreEvidencia3");
            entity.Property(e => e.UrlEvidencia3).HasColumnName("urlEvidencia3");

            entity.HasOne(d => d.IdAreaNavigation).WithMany(p => p.TbPermisos)
                .HasForeignKey(d => d.IdArea)
                .HasConstraintName("FK_Permiso_Area");
        });

        modelBuilder.Entity<TbPermisoDestinatario>(entity =>
        {
            entity.HasKey(e => e.IdPermisoDestinatario).HasName("PK__Tb_Permi__CB6FF7183F523D76");

            entity.ToTable("Tb_PermisoDestinatario");

            entity.Property(e => e.Estado).HasDefaultValueSql("((1))");
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdDestinatarioNavigation).WithMany(p => p.TbPermisoDestinatarios)
               .HasForeignKey(d => d.IdDestinatario)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_PermisoDestinatario_Destinatario");

            entity.HasOne(d => d.IdPermisoNavigation).WithMany(p => p.TbPermisoDestinatarios)
                .HasForeignKey(d => d.IdPermiso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PermisoDestinatario_Permiso");
        });

        modelBuilder.Entity<TbRol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Tb_Rol__3C872F768BBB0A8E");

            entity.ToTable("Tb_Rol");

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

        modelBuilder.Entity<TbRolMenu>(entity =>
        {
            entity.HasKey(e => e.IdRolMenu).HasName("PK__Tb_RolMe__CD2045D8C641B68F");

            entity.ToTable("Tb_RolMenu");

            entity.Property(e => e.IdRolMenu).HasColumnName("idRolMenu");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdMenu).HasColumnName("idMenu");
            entity.Property(e => e.IdRol).HasColumnName("idRol");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.TbRolMenus)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Tb_RolMen__idRol__412EB0B6");
        });

        modelBuilder.Entity<TbUsuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario");

            entity.ToTable("Tb_Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Clave)
                .HasMaxLength(100)
                .HasColumnName("clave");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .HasColumnName("correo");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreFoto)
                .HasMaxLength(100)
                .HasColumnName("nombreFoto");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .HasColumnName("telefono");
            entity.Property(e => e.UrlFoto)
                .HasMaxLength(500)
                .HasColumnName("urlFoto");

            entity.HasOne(d => d.IdAreaNavigation).WithMany(p => p.TbUsuarios)
                .HasForeignKey(d => d.IdArea)
                .HasConstraintName("FK_Usuario_Area");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.TbUsuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Usuario__idRol__5165187F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
