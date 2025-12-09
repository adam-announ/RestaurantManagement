using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data
{
    public class RestaurantContext : DbContext
    {
        public RestaurantContext(DbContextOptions<RestaurantContext> options)
            : base(options) { }

        // DbSets
        public DbSet<Personne> Personnes { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employe> Employes { get; set; }
        public DbSet<Serveur> Serveurs { get; set; }
        public DbSet<Cuisinier> Cuisiniers { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<LigneCommande> LigneCommandes { get; set; }
        public DbSet<Plat> Plats { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Facture> Factures { get; set; }
        public DbSet<Planning> Plannings { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration TPH (Table Per Hierarchy) pour l'héritage
            modelBuilder.Entity<Personne>()
                .HasDiscriminator<string>("TypePersonne")
                .HasValue<Personne>("Personne")
                .HasValue<Client>("Client")
                .HasValue<Employe>("Employe")
                .HasValue<Serveur>("Serveur")
                .HasValue<Cuisinier>("Cuisinier")
                .HasValue<Manager>("Manager");

            // Relation One-to-One : Stock - Ingredient
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Ingredient)
                .WithOne(i => i.Stock)
                .HasForeignKey<Stock>(s => s.IngredientId);

            // Relation One-to-One : Facture - Commande
            modelBuilder.Entity<Facture>()
                .HasOne(f => f.Commande)
                .WithOne(c => c.Facture)
                .HasForeignKey<Facture>(f => f.CommandeId);

            // Relation Many-to-Many : Plat - Ingredient
            modelBuilder.Entity<Plat>()
                .HasMany(p => p.Ingredients)
                .WithMany(i => i.Plats)
                .UsingEntity(j => j.ToTable("PlatIngredients"));

            // Index unique pour numéro de table
            modelBuilder.Entity<Table>()
                .HasIndex(t => t.Numero)
                .IsUnique();

            // Index unique pour email utilisateur
            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
