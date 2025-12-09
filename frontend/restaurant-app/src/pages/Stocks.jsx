import { useState, useEffect } from 'react';
import { Plus, AlertTriangle, Package } from 'lucide-react';
import { stocksApi, ingredientsApi } from '../services/api';

function Stocks() {
  const [stocks, setStocks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState({ nom: '', unite: '', seuilAlerte: '' });

  const fetchStocks = async () => {
    try {
      const response = await stocksApi.getAll();
      setStocks(response.data);
    } catch (error) {
      console.error('Erreur:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchStocks(); }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await ingredientsApi.create({
        nom: formData.nom,
        unite: formData.unite,
        seuilAlerte: parseInt(formData.seuilAlerte)
      });
      setShowModal(false);
      setFormData({ nom: '', unite: '', seuilAlerte: '' });
      fetchStocks();
    } catch (error) {
      console.error('Erreur:', error);
    }
  };

  const ajouterStock = async (id, quantite) => {
    await stocksApi.ajouter(id, quantite);
    fetchStocks();
  };

  if (loading) return <div className="text-center py-8">Chargement...</div>;

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">Gestion des Stocks</h1>
        <button
          onClick={() => setShowModal(true)}
          className="bg-orange-500 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-orange-600"
        >
          <Plus size={20} /> Nouvel Ingrédient
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {stocks.map((stock) => {
          const isLow = stock.ingredient && stock.quantite <= stock.ingredient.seuilAlerte;
          return (
            <div key={stock.id} className={`bg-white rounded-xl shadow-md p-6 ${isLow ? 'border-2 border-red-500' : ''}`}>
              <div className="flex justify-between items-start mb-4">
                <div>
                  <h3 className="text-lg font-bold flex items-center gap-2">
                    {isLow && <AlertTriangle className="text-red-500" size={20} />}
                    {stock.ingredient?.nom}
                  </h3>
                  <p className="text-gray-500">{stock.ingredient?.unite}</p>
                </div>
                <span className={`text-2xl font-bold ${isLow ? 'text-red-500' : 'text-green-500'}`}>
                  {stock.quantite}
                </span>
              </div>
              <div className="flex gap-2">
                <button
                  onClick={() => ajouterStock(stock.id, 10)}
                  className="flex-1 bg-green-100 text-green-700 px-3 py-2 rounded-lg hover:bg-green-200"
                >
                  +10
                </button>
                <button
                  onClick={() => ajouterStock(stock.id, 50)}
                  className="flex-1 bg-green-100 text-green-700 px-3 py-2 rounded-lg hover:bg-green-200"
                >
                  +50
                </button>
              </div>
            </div>
          );
        })}
      </div>

      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">Nouvel Ingrédient</h2>
            <form onSubmit={handleSubmit} className="space-y-4">
              <input type="text" placeholder="Nom" value={formData.nom} onChange={(e) => setFormData({ ...formData, nom: e.target.value })} className="w-full border rounded-lg px-3 py-2" required />
              <input type="text" placeholder="Unité (kg, L, pièces...)" value={formData.unite} onChange={(e) => setFormData({ ...formData, unite: e.target.value })} className="w-full border rounded-lg px-3 py-2" />
              <input type="number" placeholder="Seuil d'alerte" value={formData.seuilAlerte} onChange={(e) => setFormData({ ...formData, seuilAlerte: e.target.value })} className="w-full border rounded-lg px-3 py-2" required />
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

export default Stocks;
