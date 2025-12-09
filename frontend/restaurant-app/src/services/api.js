import axios from 'axios';

const API_URL = 'http://localhost:5167/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Intercepteur pour ajouter le token JWT
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Intercepteur pour gérer les erreurs 401
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Tables
export const tablesApi = {
  getAll: () => api.get('/tables'),
  getById: (id) => api.get(`/tables/${id}`),
  getDisponibles: () => api.get('/tables/disponibles'),
  create: (data) => api.post('/tables', data),
  update: (id, data) => api.put(`/tables/${id}`, data),
  updateStatut: (id, statut) => api.patch(`/tables/${id}/statut`, JSON.stringify(statut)),
  delete: (id) => api.delete(`/tables/${id}`),
};

// Clients
export const clientsApi = {
  getAll: () => api.get('/clients'),
  getById: (id) => api.get(`/clients/${id}`),
  create: (data) => api.post('/clients', data),
  update: (id, data) => api.put(`/clients/${id}`, data),
  delete: (id) => api.delete(`/clients/${id}`),
  getReservations: (id) => api.get(`/clients/${id}/reservations`),
  getCommandes: (id) => api.get(`/clients/${id}/commandes`),
};

// Plats
export const platsApi = {
  getAll: () => api.get('/plats'),
  getById: (id) => api.get(`/plats/${id}`),
  getDisponibles: () => api.get('/plats/disponibles'),
  getByCategorie: (cat) => api.get(`/plats/categorie/${cat}`),
  create: (data) => api.post('/plats', data),
  update: (id, data) => api.put(`/plats/${id}`, data),
  updateDisponibilite: (id, dispo) => api.patch(`/plats/${id}/disponibilite`, dispo),
  delete: (id) => api.delete(`/plats/${id}`),
};

// Reservations
export const reservationsApi = {
  getAll: () => api.get('/reservations'),
  getById: (id) => api.get(`/reservations/${id}`),
  getByDate: (date) => api.get(`/reservations/date/${date}`),
  create: (data) => api.post('/reservations', data),
  update: (id, data) => api.put(`/reservations/${id}`, data),
  confirmer: (id) => api.patch(`/reservations/${id}/confirmer`),
  annuler: (id) => api.patch(`/reservations/${id}/annuler`),
  delete: (id) => api.delete(`/reservations/${id}`),
};

// Commandes
export const commandesApi = {
  getAll: () => api.get('/commandes'),
  getById: (id) => api.get(`/commandes/${id}`),
  getEnCours: () => api.get('/commandes/encours'),
  getByTable: (tableId) => api.get(`/commandes/table/${tableId}`),
  create: (data) => api.post('/commandes', data),
  ajouterPlat: (id, data) => api.post(`/commandes/${id}/plats`, data),
  supprimerPlat: (id, ligneId) => api.delete(`/commandes/${id}/plats/${ligneId}`),
  updateStatut: (id, statut) => api.patch(`/commandes/${id}/statut`, JSON.stringify(statut)),
  preparer: (id, cuisinierId) => api.patch(`/commandes/${id}/preparer`, cuisinierId),
  prete: (id) => api.patch(`/commandes/${id}/prete`),
  servir: (id) => api.patch(`/commandes/${id}/servir`),
  delete: (id) => api.delete(`/commandes/${id}`),
};

// Factures
export const facturesApi = {
  getAll: () => api.get('/factures'),
  getById: (id) => api.get(`/factures/${id}`),
  generer: (commandeId) => api.post(`/factures/commande/${commandeId}`),
  payer: (id, modePaiement) => api.patch(`/factures/${id}/payer`, JSON.stringify(modePaiement)),
  delete: (id) => api.delete(`/factures/${id}`),
};

// Employes
export const employesApi = {
  getAll: () => api.get('/employes'),
  getById: (id) => api.get(`/employes/${id}`),
  getServeurs: () => api.get('/employes/serveurs'),
  getCuisiniers: () => api.get('/employes/cuisiniers'),
  getManagers: () => api.get('/employes/managers'),
  createServeur: (data) => api.post('/employes/serveur', data),
  createCuisinier: (data) => api.post('/employes/cuisinier', data),
  createManager: (data) => api.post('/employes/manager', data),
  update: (id, data) => api.put(`/employes/${id}`, data),
  delete: (id) => api.delete(`/employes/${id}`),
  getPlanning: (id) => api.get(`/employes/${id}/planning`),
};

// Stocks
export const stocksApi = {
  getAll: () => api.get('/stocks'),
  getById: (id) => api.get(`/stocks/${id}`),
  getFaibles: () => api.get('/stocks/faibles'),
  ajouter: (id, quantite) => api.patch(`/stocks/${id}/ajouter`, quantite),
  retirer: (id, quantite) => api.patch(`/stocks/${id}/retirer`, quantite),
  update: (id, quantite) => api.put(`/stocks/${id}`, quantite),
};

// Ingredients
export const ingredientsApi = {
  getAll: () => api.get('/ingredients'),
  getById: (id) => api.get(`/ingredients/${id}`),
  getAlertes: () => api.get('/ingredients/alertes'),
  create: (data) => api.post('/ingredients', data),
  update: (id, data) => api.put(`/ingredients/${id}`, data),
  delete: (id) => api.delete(`/ingredients/${id}`),
};

// Plannings
export const planningsApi = {
  getAll: () => api.get('/plannings'),
  getById: (id) => api.get(`/plannings/${id}`),
  getByDate: (date) => api.get(`/plannings/date/${date}`),
  getSemaine: (date) => api.get(`/plannings/semaine/${date}`),
  create: (data) => api.post('/plannings', data),
  update: (id, data) => api.put(`/plannings/${id}`, data),
  delete: (id) => api.delete(`/plannings/${id}`),
};

// Statistiques
export const statistiquesApi = {
  getDashboard: () => api.get('/statistiques/dashboard'),
  getVentesJour: (date) => api.get(`/statistiques/ventes/jour${date ? `?date=${date}` : ''}`),
  getVentesMois: (annee, mois) => api.get(`/statistiques/ventes/mois?annee=${annee}&mois=${mois}`),
  getVentesAnnee: (annee) => api.get(`/statistiques/ventes/annee?annee=${annee}`),
  getPlatsPopulaires: (limit = 10) => api.get(`/statistiques/plats-populaires?limit=${limit}`),
  getRevenusParCategorie: () => api.get('/statistiques/revenus-par-categorie'),
  getHeuresPointe: () => api.get('/statistiques/heures-pointe'),
  getPerformanceServeurs: () => api.get('/statistiques/performance-serveurs'),
};

// PDF
export const getFacturePdfUrl = (id) => `http://localhost:5167/api/factures/${id}/pdf`;

export default api;
