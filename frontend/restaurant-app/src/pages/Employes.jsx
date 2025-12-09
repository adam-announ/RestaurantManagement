import { useState, useEffect } from 'react';
import { Plus, Edit, Trash2 } from 'lucide-react';
import { employesApi } from '../services/api';

function Employes() {
  const [employes, setEmployes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState({
    nom: '', prenom: '', telephone: '', salaire: '', poste: '', type: 'serveur', zone: '', specialite: ''
  });

  const fetchEmployes = async () => {
    try {
      const response = await employesApi.getAll();
      setEmployes(response.data);
    } catch (error) {
      console.error('Erreur:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchEmployes(); }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const data = { ...formData, salaire: parseFloat(formData.salaire) };
      if (formData.type === 'serveur') await employesApi.createServeur(data);
      else if (formData.type === 'cuisinier') await employesApi.createCuisinier(data);
      else await employesApi.createManager(data);
      
      setShowModal(false);
      setFormData({ nom: '', prenom: '', telephone: '', salaire: '', poste: '', type: 'serveur', zone: '', specialite: '' });
      fetchEmployes();
    } catch (error) {
      console.error('Erreur:', error);
    }
  };

  const handleDelete = async (id) => {
    if (confirm('Supprimer cet employé ?')) {
      await employesApi.delete(id);
      fetchEmployes();
    }
  };

  if (loading) return <div className="text-center py-8">Chargement...</div>;

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">Employés</h1>
        <button
          onClick={() => setShowModal(true)}
          className="bg-orange-500 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-orange-600"
        >
          <Plus size={20} /> Ajouter
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {employes.map((emp) => (
          <div key={emp.id} className="bg-white rounded-xl shadow-md p-6">
            <div className="flex justify-between items-start">
              <div>
                <h3 className="text-lg font-bold">{emp.nom} {emp.prenom}</h3>
                <p className="text-gray-500">{emp.poste}</p>
                <p className="text-sm text-gray-400">{emp.telephone}</p>
              </div>
              <button onClick={() => handleDelete(emp.id)} className="p-2 text-red-500 hover:bg-red-50 rounded">
                <Trash2 size={18} />
              </button>
            </div>
          </div>
        ))}
      </div>

      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">Ajouter un employé</h2>
            <form onSubmit={handleSubmit} className="space-y-4">
              <select
                value={formData.type}
                onChange={(e) => setFormData({ ...formData, type: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
              >
                <option value="serveur">Serveur</option>
                <option value="cuisinier">Cuisinier</option>
                <option value="manager">Manager</option>
              </select>
              <input type="text" placeholder="Nom" value={formData.nom} onChange={(e) => setFormData({ ...formData, nom: e.target.value })} className="w-full border rounded-lg px-3 py-2" required />
              <input type="text" placeholder="Prénom" value={formData.prenom} onChange={(e) => setFormData({ ...formData, prenom: e.target.value })} className="w-full border rounded-lg px-3 py-2" required />
              <input type="text" placeholder="Téléphone" value={formData.telephone} onChange={(e) => setFormData({ ...formData, telephone: e.target.value })} className="w-full border rounded-lg px-3 py-2" />
              <input type="number" placeholder="Salaire" value={formData.salaire} onChange={(e) => setFormData({ ...formData, salaire: e.target.value })} className="w-full border rounded-lg px-3 py-2" required />
              {formData.type === 'serveur' && (
                <input type="text" placeholder="Zone" value={formData.zone} onChange={(e) => setFormData({ ...formData, zone: e.target.value })} className="w-full border rounded-lg px-3 py-2" />
              )}
              {formData.type === 'cuisinier' && (
                <input type="text" placeholder="Spécialité" value={formData.specialite} onChange={(e) => setFormData({ ...formData, specialite: e.target.value })} className="w-full border rounded-lg px-3 py-2" />
              )}
              <div className="flex gap-2 justify-end">
                <button type="button" onClick={() => setShowModal(false)} className="px-4 py-2 border rounded-lg">Annuler</button>
                <button type="submit" className="px-4 py-2 bg-orange-500 text-white rounded-lg">Ajouter</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default Employes;
