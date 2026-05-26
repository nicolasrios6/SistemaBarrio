using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaBarrio.Models;

namespace SistemaBarrio.Data
{
    public class BarrioDbContext : IdentityDbContext<Usuario>
    {
        public BarrioDbContext(DbContextOptions<BarrioDbContext> options) : base(options) { }

        public DbSet<Visitante> Visitantes { get; set; }
        public DbSet<Visita> Visitas { get; set; }
        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Domicilio> Domicilios { get; set; }
        public DbSet<Autorizacion> Autorizaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ─── VISITANTE ───
            builder.Entity<Visitante>()
                .HasIndex(v => v.Dni)
                .IsUnique();

            builder.Entity<Visitante>()
                .Property(v => v.Dni)
                .IsRequired()
                .HasMaxLength(10);

            builder.Entity<Visitante>()
                .Property(v => v.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<Visitante>()
                .Property(v => v.Apellido)
                .IsRequired()
                .HasMaxLength(100);

            // ─── DOMICILIO ───
            builder.Entity<Domicilio>()
                .Property(d => d.Casa)
                .IsRequired();

            builder.Entity<Domicilio>()
                .Property(d => d.Manzana)
                .HasMaxLength(10);

            // ─── PROPIETARIO ───
            builder.Entity<Propietario>()
                .Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<Propietario>()
                .Property(p => p.Apellido)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<Propietario>()
                .Property(p => p.Telefono)
                .HasMaxLength(20);

            builder.Entity<Propietario>()
                .Property(p => p.Activo)
                .HasDefaultValue(true);

            builder.Entity<Propietario>()
                .HasOne(p => p.Domicilio)
                .WithMany(d => d.Propietarios)
                .HasForeignKey(p => p.DomicilioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ─── VISITA ────
            builder.Entity<Visita>()
                .Property(v => v.FechaHoraIngreso)
                .IsRequired();

            builder.Entity<Visita>()
                .Property(v => v.FechaHoraSalida)
                .IsRequired(false);

            builder.Entity<Visita>()
                .Property(v => v.EstadoVisita)
                .HasDefaultValue(EstadoVisita.EnCurso);

            builder.Entity<Visita>()
                .HasOne(v => v.Visitante)
                .WithMany(vi => vi.Visitas)
                .HasForeignKey(v => v.VisitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Visita>()
                .HasOne(v => v.Propietario)
                .WithMany()
                .HasForeignKey(v => v.PropietarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Visita>()
                .HasOne(v => v.Domicilio)
                .WithMany()
                .HasForeignKey(v => v.DomicilioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ─── AUTORIZACION ───
            builder.Entity<Autorizacion>()
                .Property(a => a.FechaAlta)
                .IsRequired();

            builder.Entity<Autorizacion>()
                .Property(a => a.FechaVencimiento)
                .IsRequired(false);

            builder.Entity<Autorizacion>()
                .Property(a => a.EstadoAutorizacion)
                .HasDefaultValue(EstadoAutorizacion.Vigente);

            builder.Entity<Autorizacion>()
                .HasOne(a => a.Visitante)
                .WithMany(v => v.Autorizaciones)
                .HasForeignKey(a => a.VisitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Autorizacion>()
                .HasOne(a => a.Propietario)
                .WithMany(p => p.Autorizaciones)
                .HasForeignKey(a => a.PropietarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
