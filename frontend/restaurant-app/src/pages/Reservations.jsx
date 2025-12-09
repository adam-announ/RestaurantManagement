import { useState, useEffect } from 'react';
import { Plus, Check, X, Trash2 } from 'lucide-react';
import { reservationsApi, tablesApi, clientsApi } from '../services/api';

function Reservations() {
  const [reservations, setReservations] = useState([]);
  const [tables, setTables] = useState([]);
  const [clients, setClients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState({
    clientId: '', tableId: '', date: '', heure: '', nbPersonnes: ''
  });

  const fetchData = async () => {
    try {
      const [resRes, tablesRes, clientsRes] = await Promise.all([
        reservationsApi.getAll(),
        tablesApi.getAll(),
        clientsApi.getAll()
      ]);
      setReservations(resRes.data);
      setTables(tablesRes.data);
      setClients(clientsRes.data);
    } catch (error) {
      console.error('Erreur:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchData(); }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await reservationsApi.create({
        clientId: parseInt(formData.clientId),
        tableId: parseInt(formData.tableId),
        date: formData.date,
        heure: formData.heure + ':00',
        nbPersonnes: parseInt(formData.nbPersonnes)
      });
      setShowModal(false);
      setFormData({ clientId: '', tableId: '', date: '', heure: '', nbPersonnes: '' });
      fetchData();
    } catch (error) {
      alert(error.response?.data || 'Erreur lors de la création');
    }
  };

  const confirmer = async (id) => {
    await reservationsApi.confirmer(id);
    fetchData();
  };

  const annuler = async (id) => {
    await reservationsApi.annuler(id);
    fetchData();
  };

  const supprimer = async (id) => {
    if (confirm('Supprimer cette réservation ?')) {
      await reservationsApi.delete(id);
      fetchData();
    }
  };

  const getStatutColor = (statut) => {
    const colors = {
      'En attente': 'bg-yellow-100 text-yellow-800',
      'Confirmée': 'bg-green-100 text-green-800',
      'Annulée': 'bg-red-100 text-red-800'
    };
    return colors[statut] || 'bg-gray-100';
  };

  if (loading) return <div className="text-center py-8">Chargement...</div>;

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">Réservations</h1>
        <button
          onClick={() => setShowModal(true)}
          className="bg-orange-500 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-orange-600"
        >
          <Plus size={20} /> Nouvelle Réservation
        </button>
      </div>

      <div className="bg-white rounded-xl shadow-md overflow-hidden">
        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Client</th>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Table</th>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Date</th>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Heure</th>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Personnes</th>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Statut</th>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y">
            {reservations.map((res) => (
              <tr key={res.id}>
                <td className="px-6 py-4">{res.client?.nom} {res.client?.prenom}</td>
                <td className="px-6 py-4">Table {res.table?.numero}</td>
                <td className="px-6 py-4">{new Date(res.date).toLocaleDateString()}</td>
                <td className="px-6 py-4">{res.heure?.substring(0, 5)}</td>
                <td className="px-6 py-4">{res.nbPersonnes}</td>
                <td className="px-6 py-4">
                  <span className={`px-3 py-1 rounded-full text-sm ${getStatutColor(res.statut)}`}>
                    {res.statut}
                  </span>
                </td>
                <td className="px-6 py-4">
                  <div className="flex gap-2">
                    {res.statut === 'En attente' && (
                      <>
                        <button onClick={() => confirmer(res.id)} className="p-1 text-green-500 hover:bg-green-50 rounded">
                          <Check size={18} />
                        </button>
                        <button onClick={() => annuler(res.id)} className="p-1 text-red-500 hover:bg-red-50 rounded">
                          <X size={18} />
                        </button>
                      </>
                    )}
                    <button onClick={() => supprimer(res.id)} className="p-1 text-gray-500 hover:bg-gray-100 rounded">
                      <Trash2 size={18} />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">Nouvelle Réservation</h2>
            <form onSubmit={handleSubmit} className="space-y-4">
              <select
                value={formData.clientId}
                onChange={(e) => setFormData({ ...formData, clientId: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
              >
                <option value="">Sélectionner un client</option>
                {clients.map(c => <option key={c.id} value={c.id}>{c.nom} {c.prenom}</option>)}
              </select>
              <select
                value={formData.tableId}
                onChange={(e) => setFormData({ ...formData, tableId: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
              >
                <option value="">Sélectionner une table</option>
                {tables.map(t => <option key={t.id} value={t.id}>Table {t.numero} ({t.capacite} places)</option>)}
              </select>
              <input
                type="date"
                value={formData.date}
                onChange={(e) => setFormData({ ...formData, date: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
              />
              <input
                type="time"
                value={formData.heure}
                onChange={(e) => setFormData({ ...formData, heure: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
              />
              <input
                type="number"
                placeholder="Nombre de personnes"
                value={formData.nbPersonnes}
                onChange={(e) => setFormData({ ...formData, nbPersonnes: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
              />
              <div className="flex gap-2 justify-end">
                <button type="button" onClick={() => setShowModal(false)} className="px-4 py-2 border rounded-lg">Annuler</button>
                <button type="submit" className="px-4 py-2 bg-orange-500 text-white rounded-lg">Créer</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default Reservations;
