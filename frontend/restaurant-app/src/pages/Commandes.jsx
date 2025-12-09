import { useState, useEffect } from 'react';
import { Plus, Eye, ChefHat, Truck, CreditCard } from 'lucide-react';
import { commandesApi, tablesApi, platsApi, facturesApi } from '../services/api';

function Commandes() {
  const [commandes, setCommandes] = useState([]);
  const [tables, setTables] = useState([]);
  const [plats, setPlats] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [selectedCommande, setSelectedCommande] = useState(null);
  const [newCommande, setNewCommande] = useState({ tableId: '', platsSelection: [] });

  const fetchData = async () => {
    try {
      const [commandesRes, tablesRes, platsRes] = await Promise.all([
        commandesApi.getAll(),
        tablesApi.getDisponibles(),
        platsApi.getDisponibles()
      ]);
      setCommandes(commandesRes.data);
      setTables(tablesRes.data);
      setPlats(platsRes.data);
    } catch (error) {
      console.error('Erreur:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchData(); }, []);

  const handleCreateCommande = async (e) => {
    e.preventDefault();
    try {
      const response = await commandesApi.create({ tableId: parseInt(newCommande.tableId) });
      const commandeId = response.data.id;
      
      for (const item of newCommande.platsSelection) {
        if (item.quantite > 0) {
          await commandesApi.ajouterPlat(commandeId, { platId: item.platId, quantite: item.quantite });
        }
      }
      
      setShowModal(false);
      setNewCommande({ tableId: '', platsSelection: [] });
      fetchData();
    } catch (error) {
      console.error('Erreur:', error);
    }
  };

  const updateStatut = async (id, action) => {
    try {
      if (action === 'prete') await commandesApi.prete(id);
      else if (action === 'servir') await commandesApi.servir(id);
      else if (action === 'payer') {
        const facture = await facturesApi.generer(id);
        await facturesApi.payer(facture.data.id, 'CB');
      }
      fetchData();
    } catch (error) {
      console.error('Erreur:', error);
    }
  };

  const viewDetails = async (id) => {
    const response = await commandesApi.getById(id);
    setSelectedCommande(response.data);
    setShowDetailModal(true);
  };

  const getStatutColor = (statut) => {
    const colors = {
      'En cours': 'bg-yellow-100 text-yellow-800',
      'En préparation': 'bg-blue-100 text-blue-800',
      'Prête': 'bg-green-100 text-green-800',
      'Servie': 'bg-purple-100 text-purple-800',
      'Payée': 'bg-gray-100 text-gray-800'
    };
    return colors[statut] || 'bg-gray-100';
  };

  if (loading) return <div className="text-center py-8">Chargement...</div>;

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">Commandes</h1>
        <button
          onClick={() => setShowModal(true)}
          className="bg-orange-500 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-orange-600"
        >
          <Plus size={20} /> Nouvelle Commande
        </button>
      </div>

      <div className="grid gap-4">
        {commandes.map((commande) => (
          <div key={commande.id} className="bg-white rounded-xl shadow-md p-6">
            <div className="flex justify-between items-center">
              <div>
                <h3 className="text-lg font-bold">Commande #{commande.id}</h3>
                <p className="text-gray-500">
                  Table {commande.table?.numero || 'N/A'} • {new Date(commande.dateHeure).toLocaleString()}
                </p>
              </div>
              <div className="flex items-center gap-4">
                <span className="text-xl font-bold text-orange-500">{commande.total.toFixed(2)} €</span>
                <span className={`px-3 py-1 rounded-full text-sm ${getStatutColor(commande.statut)}`}>
                  {commande.statut}
                </span>
              </div>
            </div>
            <div className="flex gap-2 mt-4">
              <button onClick={() => viewDetails(commande.id)} className="p-2 text-gray-500 hover:bg-gray-100 rounded">
                <Eye size={18} />
              </button>
              {commande.statut === 'En cours' && (
                <button onClick={() => updateStatut(commande.id, 'prete')} className="p-2 text-green-500 hover:bg-green-50 rounded flex items-center gap-1">
                  <ChefHat size={18} /> Prête
                </button>
              )}
              {commande.statut === 'Prête' && (
                <button onClick={() => updateStatut(commande.id, 'servir')} className="p-2 text-blue-500 hover:bg-blue-50 rounded flex items-center gap-1">
                  <Truck size={18} /> Servir
                </button>
              )}
              {commande.statut === 'Servie' && (
                <button onClick={() => updateStatut(commande.id, 'payer')} className="p-2 text-purple-500 hover:bg-purple-50 rounded flex items-center gap-1">
                  <CreditCard size={18} /> Payer
                </button>
              )}
            </div>
          </div>
        ))}
      </div>

      {/* Modal Nouvelle Commande */}
      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-lg max-h-[80vh] overflow-y-auto">
            <h2 className="text-xl font-bold mb-4">Nouvelle Commande</h2>
            <form onSubmit={handleCreateCommande} className="space-y-4">
              <select
                value={newCommande.tableId}
                onChange={(e) => setNewCommande({ ...newCommande, tableId: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
              >
                <option value="">Sélectionner une table</option>
                {tables.map(t => <option key={t.id} value={t.id}>Table {t.numero}</option>)}
              </select>
              
              <div>
                <h3 className="font-medium mb-2">Plats</h3>
                {plats.map(plat => (
                  <div key={plat.id} className="flex items-center justify-between py-2 border-b">
                    <span>{plat.nom} - {plat.prix}€</span>
                    <input
                      type="number"
                      min="0"
                      placeholder="Qté"
                      className="w-20 border rounded px-2 py-1"
                      onChange={(e) => {
                        const qty = parseInt(e.target.value) || 0;
                        setNewCommande(prev => ({
                          ...prev,
                          platsSelection: [
                            ...prev.platsSelection.filter(p => p.platId !== plat.id),
                            { platId: plat.id, quantite: qty }
                          ]
                        }));
                      }}
                    />
                  </div>
                ))}
              </div>
              
              <div className="flex gap-2 justify-end">
                <button type="button" onClick={() => setShowModal(false)} className="px-4 py-2 border rounded-lg">Annuler</button>
                <button type="submit" className="px-4 py-2 bg-orange-500 text-white rounded-lg">Créer</button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Modal Détails */}
      {showDetailModal && selectedCommande && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">Commande #{selectedCommande.id}</h2>
            <div className="space-y-2">
              {selectedCommande.ligneCommandes?.map(ligne => (
                <div key={ligne.id} className="flex justify-between">
                  <span>{ligne.quantite}x {ligne.plat?.nom}</span>
                  <span>{(ligne.quantite * ligne.prixUnitaire).toFixed(2)}€</span>
                </div>
              ))}
              <div className="border-t pt-2 font-bold flex justify-between">
                <span>Total</span>
                <span>{selectedCommande.total.toFixed(2)}€</span>
              </div>
            </div>
            <button onClick={() => setShowDetailModal(false)} className="mt-4 w-full px-4 py-2 bg-gray-200 rounded-lg">
              Fermer
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

export default Commandes;
