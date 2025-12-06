# Restaurant Management System - Backend

## Configuration PostgreSQL

1. Installez PostgreSQL sur votre machine
2. Créez une base de données :
```sql
CREATE DATABASE RestaurantDB;
```

3. Modifiez `appsettings.json` avec vos identifiants PostgreSQL :
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=RestaurantDB;Username=postgres;Password=votre_password"
  }
}
```

## Installation

```bash
# Restaurer les packages
dotnet restore

# Créer la migration initiale
dotnet ef migrations add InitialCreate

# Appliquer la migration à la base de données
dotnet ef database update
```

## Démarrage

```bash
dotnet run
```

L'API sera disponible sur : `http://localhost:5000`
Swagger UI : `http://localhost:5000/swagger`

## Endpoints API

### Clients
- `GET /api/Clients` - Liste tous les clients
- `GET /api/Clients/{id}` - Détails d'un client
- `POST /api/Clients` - Créer un client
- `PUT /api/Clients/{id}` - Modifier un client
- `DELETE /api/Clients/{id}` - Supprimer un client
- `GET /api/Clients/{id}/Reservations` - Réservations d'un client
- `GET /api/Clients/{id}/Commandes` - Commandes d'un client

### Serveurs
- `GET /api/Serveurs` - Liste tous les serveurs
- `GET /api/Serveurs/{id}` - Détails d'un serveur
- `POST /api/Serveurs` - Créer un serveur
- `PUT /api/Serveurs/{id}` - Modifier un serveur
- `DELETE /api/Serveurs/{id}` - Supprimer un serveur
- `GET /api/Serveurs/{id}/Tables` - Tables d'un serveur
- `POST /api/Serveurs/{id}/EncaisserPourboire` - Encaisser un pourboire

### Commandes
- `GET /api/Commandes` - Liste toutes les commandes
- `GET /api/Commandes/{id}` - Détails d'une commande
- `POST /api/Commandes` - Créer une commande
- `PUT /api/Commandes/{id}` - Modifier une commande
- `DELETE /api/Commandes/{id}` - Supprimer une commande
- `POST /api/Commandes/{id}/CalculerTotal` - Calculer le total
- `POST /api/Commandes/{id}/Valider` - Valider une commande

### Réservations
- `GET /api/Reservations` - Liste toutes les réservations
- `GET /api/Reservations/{id}` - Détails d'une réservation
- `POST /api/Reservations` - Créer une réservation
- `PUT /api/Reservations/{id}` - Modifier une réservation
- `DELETE /api/Reservations/{id}` - Supprimer une réservation
- `POST /api/Reservations/{id}/Confirmer` - Confirmer une réservation

### Plats
- `GET /api/Plats` - Liste tous les plats
- `GET /api/Plats/{id}` - Détails d'un plat
- `POST /api/Plats` - Créer un plat
- `PUT /api/Plats/{id}` - Modifier un plat
- `DELETE /api/Plats/{id}` - Supprimer un plat
- `GET /api/Plats/Disponibles` - Plats disponibles

### Tables
- `GET /api/Tables` - Liste toutes les tables
- `GET /api/Tables/{id}` - Détails d'une table
- `POST /api/Tables` - Créer une table
- `PUT /api/Tables/{id}` - Modifier une table
- `DELETE /api/Tables/{id}` - Supprimer une table

## Structure du Projet

```
RestaurantManagement/
├── Models/              # Entités de la base de données
├── Data/                # DbContext et migrations
├── Controllers/         # Contrôleurs API
├── Program.cs           # Point d'entrée de l'application
└── appsettings.json     # Configuration
```

## Technologies
- .NET 8.0
- Entity Framework Core 8.0
- PostgreSQL (Npgsql)
- Swagger/OpenAPI
