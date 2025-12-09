import { useState, useEffect } from 'react';
import { FileText, Download, Eye } from 'lucide-react';
import { facturesApi, getFacturePdfUrl } from '../services/api';

function Factures() {
  const [factures, setFactures] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedFacture, setSelectedFacture] = useState(null);

  useEffect(() => {
    const fetchFactures = async () => {
      try {
        const response = await facturesApi.getAll();
        setFactures(response.data);
      } catch (error) {
        console.error('Erreur:', error);
      } finally {
        setLoading(false);
      }
    };
    fetchFactures();
  }, []);

  const downloadPdf = (id) => {
    window.open(getFacturePdfUrl(id), '_blank');
  };

  const viewDetails = async (id) => {
    try {
      const response = await facturesApi.getById(id);
      setSelectedFacture(response.data);
    } catch (error) {
      console.error('Erreur:', error);
    }
  };

  if (loading) return <div className="text-center py-8">Chargement...</div>;

  return (
    <div>
      <h1 className="text-3xl font-bold mb-6">Factures</h1>
      
      <div className="bg-white rounded-xl shadow-md overflow-hidden">
        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">N° Facture</th>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Date</th>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Client</th>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Montant</th>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Mode</th>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Statut</th>
              <th className="px-6 py-3 text-left text-sm font-medium text-gray-500">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y">
            {factures.map((facture) => (
              <tr key={facture.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 font-medium">#{facture.id.toString().padStart(6, '0')}</td>
                <td className="px-6 py-4">{new Date(facture.date).toLocaleDateString()}</td>
                <td className="px-6 py-4">
                  {facture.commande?.client 
                    ? `${facture.commande.client.prenom} ${facture.commande.client.nom}`
                    : 'Client anonyme'}
                </td>
                <td className="px-6 py-4 font-bold text-orange-500">{facture.montantTotal.toFixed(2)} €</td>
                <td className="px-6 py-4">{facture.modePaiement || '-'}</td>
                <td className="px-6 py-4">
                  <span className={`px-3 py-1 rounded-full text-sm ${
                    facture.statut === 'Payée' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                  }`}>
                    {facture.statut}
                  </span>
                </td>
                <td className="px-6 py-4">
                  <div className="flex gap-2">
                    <button
                      onClick={() => viewDetails(facture.id)}
                      className="p-2 text-blue-500 hover:bg-blue-50 rounded"
                      title="Voir détails"
                    >
                      <Eye size={18} />
                    </button>
                    <button
                      onClick={() => downloadPdf(facture.id)}
                      className="p-2 text-green-500 hover:bg-green-50 rounded"
                      title="Télécharger PDF"
                    >
                      <Download size={18} />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        
        {factures.length === 0 && (
          <div className="text-center py-8 text-gray-500">
            Aucune facture disponible
          </div>
        )}
      </div>

      {/* Modal Détails */}
      {selectedFacture && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-lg max-h-[80vh] overflow-y-auto">
            <div className="flex justify-between items-start mb-4">
              <h2 className="text-xl font-bold">Facture #{selectedFacture.id.toString().padStart(6, '0')}</h2>
              <span className={`px-3 py-1 rounded-full text-sm ${
                selectedFacture.statut === 'Payée' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
              }`}>
                {selectedFacture.statut}
              </span>
            </div>

            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <p className="text-gray-500">Date</p>
                  <p className="font-medium">{new Date(selectedFacture.date).toLocaleString()}</p>
                </div>
                <div>
                  <p className="text-gray-500">Mode de paiement</p>
                  <p className="font-medium">{selectedFacture.modePaiement || '-'}</p>
                </div>
              </div>

              {selectedFacture.commande?.ligneCommandes && (
                <div>
                  <h3 className="font-semibold mb-2">Articles</h3>
                  <div className="space-y-2">
                    {selectedFacture.commande.ligneCommandes.map(ligne => (
                      <div key={ligne.id} className="flex justify-between text-sm">
                        <span>{ligne.quantite}x {ligne.plat?.nom}</span>
                        <span>{(ligne.quantite * ligne.prixUnitaire).toFixed(2)} €</span>
                      </div>
                    ))}
                  </div>
                </div>
              )}

              <div className="border-t pt-4">
                <div className="flex justify-between font-bold text-lg">
                  <span>Total</span>
                  <span className="text-orange-500">{selectedFacture.montantTotal.toFixed(2)} €</span>
                </div>
              </div>
            </div>

            <div className="flex gap-2 mt-6">
              <button
                onClick={() => downloadPdf(selectedFacture.id)}
                className="flex-1 bg-green-500 text-white py-2 rounded-lg flex items-center justify-center gap-2 hover:bg-green-600"
              >
                <Download size={18} />
                Télécharger PDF
              </button>
              <button
                onClick={() => setSelectedFacture(null)}
                className="flex-1 bg-gray-200 py-2 rounded-lg hover:bg-gray-300"
              >
                Fermer
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default Factures;
