using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;

namespace RestaurantManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Serveur> Serveurs { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<LigneCommande> LignesCommande { get; set; }
        public DbSet<Plat> Plats { get; set; }
        public DbSet<Paiement> Paiements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Client
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Reservations)
                .WithOne(r => r.Client)
                .HasForeignKey(r => r.IdClient)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Commandes)
                .WithOne(cmd => cmd.Client)
                .HasForeignKey(cmd => cmd.IdClient)
                .OnDelete(DeleteBehavior.Cascade);

            // Serveur
            modelBuilder.Entity<Serveur>()
                .HasMany(s => s.Tables)
                .WithOne(t => t.Serveur)
                .HasForeignKey(t => t.IdServeur)
                .OnDelete(DeleteBehavior.SetNull);

            // Table
            modelBuilder.Entity<Table>()
                .HasMany(t => t.Reservations)
                .WithOne(r => r.Table)
                .HasForeignKey(r => r.IdTable)
                .OnDelete(DeleteBehavior.Restrict);

            // Commande
            modelBuilder.Entity<Commande>()
                .HasMany(c => c.LignesCommande)
                .WithOne(lc => lc.Commande)
                .HasForeignKey(lc => lc.IdCommande)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Commande>()
                .HasOne(c => c.Paiement)
                .WithOne(p => p.Commande)
                .HasForeignKey<Paiement>(p => p.IdCommande)
                .OnDelete(DeleteBehavior.Cascade);

            // Plat
            modelBuilder.Entity<Plat>()
                .HasMany(p => p.LignesCommande)
                .WithOne(lc => lc.Plat)
                .HasForeignKey(lc => lc.IdPlat)
                .OnDelete(DeleteBehavior.Restrict);

            // Enum conversions
            modelBuilder.Entity<Commande>()
                .Property(c => c.Statut)
                .HasConversion<string>();

            modelBuilder.Entity<Reservation>()
                .Property(r => r.Statut)
                .HasConversion<string>();

            modelBuilder.Entity<Paiement>()
                .Property(p => p.MethodePaiement)
                .HasConversion<string>();
        }
    }
}
