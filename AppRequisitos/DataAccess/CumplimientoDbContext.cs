
using AppRequisitos.Modelos;
using AppRequisitos.Models;
using AppRequisitos.Utilidades;
using Microsoft.EntityFrameworkCore;

namespace AppRequisitos.DataAccess
{
    public class CumplimientoDbContext : DbContext
    {
        
        public DbSet<Fincas> Fincas { get; set; }
        public DbSet<Checklist> Checklist { get; set; }
        public DbSet<Actividades> Actividades { get; set; }
        public DbSet<Revisiones> Revisiones { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> Detalleventa { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conexionDB = $"Filename={ConexionDb.DevolverRuta("cumplimiento.db")}";
            optionsBuilder.UseSqlite(conexionDB);
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Checklist>(entity =>
            {
                entity.HasKey(c => c.IdRequisito);
                entity.Property(c => c.IdRequisito).IsRequired().ValueGeneratedOnAdd();
                entity.HasIndex(c => c.IdRequisito); // Índice para mejorar el rendimiento
            });            

            modelBuilder.Entity<Fincas>(entity =>
            {
                entity.HasKey(c => c.IdFinca);
                entity.Property(c => c.IdFinca).IsRequired().ValueGeneratedOnAdd();
                entity.HasIndex(c => c.IdFinca); // Índice para mejorar el rendimiento
            });


            modelBuilder.Entity<Revisiones>(entity =>
            {
                entity.HasKey(p => p.Id_revision);
                entity.Property(p => p.Id_revision).IsRequired().ValueGeneratedOnAdd();
                //entity.HasOne(p => p.RefChecklist).WithMany(c => c.RefRevision).HasForeignKey(p => p.Id_revision);
                entity.HasOne(p => p.RefFincasNombre).WithMany(c => c.RefFincas).HasForeignKey(p => p.IdFinca);
            });

            modelBuilder.Entity<Actividades>(entity =>
            {
                entity.HasKey(p => p.Id_actividad);
                entity.Property(p => p.Id_actividad).IsRequired();            
                entity.HasOne(p => p.RefFincasNombre).WithMany(c => c.RefFincas2).HasForeignKey(p => p.IdFinca);
            });

            modelBuilder.Entity<Usuarios>(entity =>
            {
                entity.HasKey(u => u.Iduser);
                entity.Property(c => c.Iduser).IsRequired().ValueGeneratedOnAdd();
            });


        }
    }
}
