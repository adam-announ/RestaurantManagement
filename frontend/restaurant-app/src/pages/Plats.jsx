import { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, Check, X } from 'lucide-react';
import { platsApi } from '../services/api';

function Plats() {
  const [plats, setPlats] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingPlat, setEditingPlat] = useState(null);
  const [formData, setFormData] = useState({
    nom: '', description: '', prix: '', categorie: '', disponible: true
  });

  const fetchPlats = async () => {
    try {
      const response = await platsApi.getAll();
      setPlats(response.data);
    } catch (error) {
      console.error('Erreur:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchPlats(); }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const data = { ...formData, prix: parseFloat(formData.prix) };
      if (editingPlat) {
        await platsApi.update(editingPlat.id, { ...data, id: editingPlat.id });
      } else {
        await platsApi.create(data);
      }
      setShowModal(false);
      setEditingPlat(null);
      setFormData({ nom: '', description: '', prix: '', categorie: '', disponible: true });
      fetchPlats();
    } catch (error) {
      console.error('Erreur:', error);
    }
  };

  const handleEdit = (plat) => {
    setEditingPlat(plat);
    setFormData({
      nom: plat.nom,
      description: plat.description || '',
      prix: plat.prix,
      categorie: plat.categorie || '',
      disponible: plat.disponible
    });
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (confirm('Supprimer ce plat ?')) {
      await platsApi.delete(id);
      fetchPlats();
    }
  };

  const toggleDisponibilite = async (plat) => {
    await platsApi.updateDisponibilite(plat.id, !plat.disponible);
    fetchPlats();
  };

  const categories = [...new Set(plats.map(p => p.categorie).filter(Boolean))];

  if (loading) return <div className="text-center py-8">Chargement...</div>;

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">Menu - Plats</h1>
        <button
          onClick={() => { setShowModal(true); setEditingPlat(null); }}
          className="bg-orange-500 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-orange-600"
        >
          <Plus size={20} /> Ajouter
        </button>
      </div>

      {categories.map(categorie => (
        <div key={categorie} className="mb-8">
          <h2 className="text-xl font-semibold mb-4 text-gray-700">{categorie}</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {plats.filter(p => p.categorie === categorie).map(plat => (
              <div key={plat.id} className={`bg-white rounded-xl shadow-md p-6 ${!plat.disponible && 'opacity-50'}`}>
                <div className="flex justify-between items-start mb-2">
                  <h3 className="text-lg font-bold">{plat.nom}</h3>
                  <span className="text-orange-500 font-bold">{plat.prix.toFixed(2)} €</span>
                </div>
                <p className="text-gray-500 text-sm mb-4">{plat.description}</p>
                <div className="flex justify-between items-center">
                  <button
                    onClick={() => toggleDisponibilite(plat)}
                    className={`flex items-center gap-1 px-2 py-1 rounded text-sm ${
                      plat.disponible ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
                    }`}
                  >
                    {plat.disponible ? <Check size={14} /> : <X size={14} />}
                    {plat.disponible ? 'Disponible' : 'Indisponible'}
                  </button>
                  <div className="flex gap-2">
                    <button onClick={() => handleEdit(plat)} className="p-2 text-blue-500 hover:bg-blue-50 rounded">
                      <Edit size={18} />
                    </button>
                    <button onClick={() => handleDelete(plat.id)} className="p-2 text-red-500 hover:bg-red-50 rounded">
                      <Trash2 size={18} />
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      ))}

      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">{editingPlat ? 'Modifier' : 'Ajouter'} un plat</h2>
            <form onSubmit={handleSubmit} className="space-y-4">
              <input
                type="text"
                placeholder="Nom du plat"
                value={formData.nom}
                onChange={(e) => setFormData({ ...formData, nom: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
              />
              <textarea
                placeholder="Description"
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
              />
              <input
                type="number"
                step="0.01"
                placeholder="Prix"
                value={formData.prix}
                onChange={(e) => setFormData({ ...formData, prix: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
              />
              <input
                type="text"
                placeholder="Catégorie (Entrées, Plats, Desserts...)"
                value={formData.categorie}
                onChange={(e) => setFormData({ ...formData, categorie: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
              />
              <label className="flex items-center gap-2">
                <input
                  type="checkbox"
                  checked={formData.disponible}
                  onChange={(e) => setFormData({ ...formData, disponible: e.target.checked })}
                />
                Disponible
              </label>
              <div className="flex gap-2 justify-end">
                <button type="button" onClick={() => setShowModal(false)} className="px-4 py-2 border rounded-lg">
                  Annuler
                </button>
                <button type="submit" className="px-4 py-2 bg-orange-500 text-white rounded-lg">
                  {editingPlat ? 'Modifier' : 'Ajouter'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default Plats;
