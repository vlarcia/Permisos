

using AppRequisitos.Modelos;
using AppRequisitos.Models;
using AppRequisitos.Utilidades;
using Microsoft.EntityFrameworkCore;

namespace AppRequisitos.DataAccess
{
    public class SQLServerDbContext : DbContext
    {
        
        public DbSet<Fincas> Fincas { get; set; }
        public DbSet<Checklist> Checklist { get; set; }
        public DbSet<Actividades> Actividades { get; set; }
        public DbSet<Revisiones> Revisiones { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //string conexionDB2 = "Server=63.141.234.170,49382;Database=db_Productores;Trusted_Connection=True;user=usr_arcia;password=Arciapanta22";            
            string conexionDB2 = "Server=localhost;Database=db_cumplimiento;Trusted_Connection=True;user=sa;password=Laurean0";            
            optionsBuilder.UseSqlServer(conexionDB2);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Checklist>(entity =>
            //{
            //    entity.HasKey(c => c.IdRequisito);
            //    entity.Property(c => c.IdRequisito).IsRequired().ValueGeneratedOnAdd();
            //});

            modelBuilder.Entity<Fincas>(entity =>
            {
                entity.HasKey(c => c.IdFinca);
                entity.Property(c => c.IdFinca).IsRequired().ValueGeneratedOnAdd();
            });


            //modelBuilder.Entity<Revisiones>(entity =>
            //{
            //    entity.HasKey(p => p.Id_revision);
            //    entity.Property(p => p.Id_revision).IsRequired().ValueGeneratedOnAdd();
            //    //entity.HasOne(p => p.RefChecklist).WithMany(c => c.RefRevision).HasForeignKey(p => p.Id_revision);
            //    entity.HasOne(p => p.RefFincasNombre).WithMany(c => c.RefFincas).HasForeignKey(p => p.IdFinca);
            //});

            //modelBuilder.Entity<Actividades>(entity =>
            //{
            //    entity.HasKey(p => p.Id_actividad);
            //    entity.Property(p => p.Id_actividad).IsRequired();
            //    entity.HasOne(p => p.RefFincasNombre).WithMany(c => c.RefFincas2).HasForeignKey(p => p.IdFinca);
            //});

            modelBuilder.Entity<Usuarios>(entity =>
            {
                entity.HasKey(u => u.Iduser);
                entity.Property(c => c.Iduser).IsRequired().ValueGeneratedOnAdd();
            });
        }
    }
}
