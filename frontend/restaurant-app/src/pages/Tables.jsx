import { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, Users } from 'lucide-react';
import { tablesApi } from '../services/api';

function Tables() {
  const [tables, setTables] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingTable, setEditingTable] = useState(null);
  const [formData, setFormData] = useState({ numero: '', capacite: '', statut: 'Disponible' });

  const fetchTables = async () => {
    try {
      const response = await tablesApi.getAll();
      setTables(response.data);
    } catch (error) {
      console.error('Erreur:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTables();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editingTable) {
        await tablesApi.update(editingTable.id, { ...formData, id: editingTable.id });
      } else {
        await tablesApi.create(formData);
      }
      setShowModal(false);
      setEditingTable(null);
      setFormData({ numero: '', capacite: '', statut: 'Disponible' });
      fetchTables();
    } catch (error) {
      console.error('Erreur:', error);
    }
  };

  const handleEdit = (table) => {
    setEditingTable(table);
    setFormData({ numero: table.numero, capacite: table.capacite, statut: table.statut });
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (confirm('Supprimer cette table ?')) {
      try {
        await tablesApi.delete(id);
        fetchTables();
      } catch (error) {
        console.error('Erreur:', error);
      }
    }
  };

  const getStatutColor = (statut) => {
    switch (statut) {
      case 'Disponible': return 'bg-green-100 text-green-800';
      case 'Occupée': return 'bg-red-100 text-red-800';
      case 'Réservée': return 'bg-yellow-100 text-yellow-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  if (loading) return <div className="text-center py-8">Chargement...</div>;

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">Gestion des Tables</h1>
        <button
          onClick={() => { setShowModal(true); setEditingTable(null); setFormData({ numero: '', capacite: '', statut: 'Disponible' }); }}
          className="bg-orange-500 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-orange-600"
        >
          <Plus size={20} /> Ajouter
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
        {tables.map((table) => (
          <div key={table.id} className="bg-white rounded-xl shadow-md p-6">
            <div className="flex justify-between items-start mb-4">
              <div>
                <h3 className="text-xl font-bold">Table {table.numero}</h3>
                <p className="text-gray-500 flex items-center gap-1">
                  <Users size={16} /> {table.capacite} places
                </p>
              </div>
              <span className={`px-3 py-1 rounded-full text-sm ${getStatutColor(table.statut)}`}>
                {table.statut}
              </span>
            </div>
            <div className="flex gap-2">
              <button onClick={() => handleEdit(table)} className="p-2 text-blue-500 hover:bg-blue-50 rounded">
                <Edit size={18} />
              </button>
              <button onClick={() => handleDelete(table.id)} className="p-2 text-red-500 hover:bg-red-50 rounded">
                <Trash2 size={18} />
              </button>
            </div>
          </div>
        ))}
      </div>

      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">{editingTable ? 'Modifier' : 'Ajouter'} une table</h2>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium mb-1">Numéro</label>
                <input
                  type="number"
                  value={formData.numero}
                  onChange={(e) => setFormData({ ...formData, numero: parseInt(e.target.value) })}
                  className="w-full border rounded-lg px-3 py-2"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Capacité</label>
                <input
                  type="number"
                  value={formData.capacite}
                  onChange={(e) => setFormData({ ...formData, capacite: parseInt(e.target.value) })}
                  className="w-full border rounded-lg px-3 py-2"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Statut</label>
                <select
                  value={formData.statut}
                  onChange={(e) => setFormData({ ...formData, statut: e.target.value })}
                  className="w-full border rounded-lg px-3 py-2"
                >
                  <option value="Disponible">Disponible</option>
                  <option value="Occupée">Occupée</option>
                  <option value="Réservée">Réservée</option>
                </select>
              </div>
              <div className="flex gap-2 justify-end">
                <button type="button" onClick={() => setShowModal(false)} className="px-4 py-2 border rounded-lg">
                  Annuler
                </button>
                <button type="submit" className="px-4 py-2 bg-orange-500 text-white rounded-lg">
                  {editingTable ? 'Modifier' : 'Ajouter'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default Tables;
