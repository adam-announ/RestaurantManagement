-- Script d'initialisation de données pour RestaurantDB

-- Insertion de clients
INSERT INTO "Clients" ("Nom", "Prenom", "Telephone", "Email") VALUES
('Dupont', 'Jean', '0612345678', 'jean.dupont@email.com'),
('Martin', 'Marie', '0623456789', 'marie.martin@email.com'),
('Bernard', 'Pierre', '0634567890', 'pierre.bernard@email.com');

-- Insertion de serveurs
INSERT INTO "Serveurs" ("Secteur", "Pourboires") VALUES
('Terrasse', 0),
('Salle principale', 0),
('Salle VIP', 0);

-- Insertion de tables
INSERT INTO "Tables" ("NumeroTable", "Statut", "Emplacement", "IdServeur") VALUES
(1, 'LIBRE', 'Terrasse', 1),
(2, 'LIBRE', 'Terrasse', 1),
(3, 'LIBRE', 'Salle principale', 2),
(4, 'LIBRE', 'Salle principale', 2),
(5, 'LIBRE', 'Salle VIP', 3);

-- Insertion de plats
INSERT INTO "Plats" ("Nom", "Description", "Prix", "Categorie", "TempsPreparation", "Disponible") VALUES
('Pizza Margherita', 'Pizza classique avec tomate et mozzarella', 12.50, 'Plat principal', 15, true),
('Salade César', 'Salade romaine, poulet, parmesan, croûtons', 9.90, 'Entrée', 10, true),
('Tiramisu', 'Dessert italien au café', 6.50, 'Dessert', 5, true),
('Pâtes Carbonara', 'Pâtes à la crème, lardons, parmesan', 13.90, 'Plat principal', 20, true),
('Burger Maison', 'Burger avec frites maison', 15.50, 'Plat principal', 25, true);

-- Insertion de réservations
INSERT INTO "Reservations" ("DateReservation", "NombrePersonnes", "Statut", "IdClient", "IdTable") VALUES
(CURRENT_DATE + INTERVAL '1 day', 4, 'CONFIRMEE', 1, 3),
(CURRENT_DATE + INTERVAL '2 days', 2, 'EN_ATTENTE', 2, 1);

-- Insertion de commandes
INSERT INTO "Commandes" ("DateCommande", "HeureCommande", "MontantTotal", "Statut", "IdClient") VALUES
(CURRENT_DATE, '12:30:00', 35.80, 'EN_PREPARATION', 1),
(CURRENT_DATE, '13:15:00', 22.40, 'EN_COURS', 2);

-- Insertion de lignes de commande
INSERT INTO "LignesCommande" ("Quantite", "PrixUnitaire", "SousTotal", "IdCommande", "IdPlat") VALUES
(2, 12.50, 25.00, 1, 1),
(1, 6.50, 6.50, 1, 3),
(1, 9.90, 9.90, 2, 2),
(1, 12.50, 12.50, 2, 1);
