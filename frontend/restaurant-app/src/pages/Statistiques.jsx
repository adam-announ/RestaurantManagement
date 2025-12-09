import { useState, useEffect } from 'react';
import { 
  TrendingUp, 
  DollarSign, 
  ShoppingBag, 
  Users,
  Calendar,
  Award,
  Clock,
  PieChart
} from 'lucide-react';
import { statistiquesApi } from '../services/api';

function Statistiques() {
  const [dashboard, setDashboard] = useState(null);
  const [platsPopulaires, setPlatsPopulaires] = useState([]);
  const [revenusCategorie, setRevenusCategorie] = useState([]);
  const [ventesAnnee, setVentesAnnee] = useState(null);
  const [heuresPointe, setHeuresPointe] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [dashRes, platsRes, catRes, anneeRes, heuresRes] = await Promise.all([
          statistiquesApi.getDashboard(),
          statistiquesApi.getPlatsPopulaires(5),
          statistiquesApi.getRevenusParCategorie(),
          statistiquesApi.getVentesAnnee(new Date().getFullYear()),
          statistiquesApi.getHeuresPointe()
        ]);

        setDashboard(dashRes.data);
        setPlatsPopulaires(platsRes.data);
        setRevenusCategorie(catRes.data);
        setVentesAnnee(anneeRes.data);
        setHeuresPointe(heuresRes.data);
      } catch (error) {
        console.error('Erreur:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) return <div className="text-center py-8">Chargement...</div>;

  const statCards = [
    {
      title: "Ventes Aujourd'hui",
      value: `${dashboard?.ventesAujourdhui?.toFixed(2) || 0} €`,
      icon: DollarSign,
      color: 'bg-green-500'
    },
    {
      title: "Ventes du Mois",
      value: `${dashboard?.ventesMois?.toFixed(2) || 0} €`,
      icon: TrendingUp,
      color: 'bg-blue-500'
    },
    {
      title: "Commandes Aujourd'hui",
      value: dashboard?.commandesAujourdhui || 0,
      icon: ShoppingBag,
      color: 'bg-purple-500'
    },
    {
      title: "Réservations Aujourd'hui",
      value: dashboard?.reservationsAujourdhui || 0,
      icon: Calendar,
      color: 'bg-orange-500'
    }
  ];

  const moisNoms = ['Jan', 'Fév', 'Mar', 'Avr', 'Mai', 'Juin', 'Juil', 'Août', 'Sep', 'Oct', 'Nov', 'Déc'];

  return (
    <div>
      <h1 className="text-3xl font-bold mb-8">Statistiques & Rapports</h1>

      {/* Cards principales */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        {statCards.map((card, index) => {
          const Icon = card.icon;
          return (
            <div key={index} className="bg-white rounded-xl shadow-md p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-gray-500 text-sm">{card.title}</p>
                  <p className="text-2xl font-bold mt-2">{card.value}</p>
                </div>
                <div className={`${card.color} p-4 rounded-full`}>
                  <Icon className="text-white" size={24} />
                </div>
              </div>
            </div>
          );
        })}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Plats populaires */}
        <div className="bg-white rounded-xl shadow-md p-6">
          <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
            <Award className="text-orange-500" />
            Top 5 Plats Populaires
          </h2>
          <div className="space-y-4">
            {platsPopulaires.map((plat, index) => (
              <div key={plat.platId} className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <span className={`w-8 h-8 rounded-full flex items-center justify-center text-white font-bold ${
                    index === 0 ? 'bg-yellow-500' : index === 1 ? 'bg-gray-400' : index === 2 ? 'bg-orange-600' : 'bg-gray-300'
                  }`}>
                    {index + 1}
                  </span>
                  <div>
                    <p className="font-medium">{plat.nom}</p>
                    <p className="text-sm text-gray-500">{plat.categorie}</p>
                  </div>
                </div>
                <div className="text-right">
                  <p className="font-bold text-orange-500">{plat.quantiteVendue} vendus</p>
                  <p className="text-sm text-gray-500">{plat.chiffreAffaires?.toFixed(2)} €</p>
                </div>
              </div>
            ))}
            {platsPopulaires.length === 0 && (
              <p className="text-gray-500 text-center">Aucune donnée disponible</p>
            )}
          </div>
        </div>

        {/* Revenus par catégorie */}
        <div className="bg-white rounded-xl shadow-md p-6">
          <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
            <PieChart className="text-blue-500" />
            Revenus par Catégorie
          </h2>
          <div className="space-y-4">
            {revenusCategorie.map((cat, index) => {
              const colors = ['bg-blue-500', 'bg-green-500', 'bg-purple-500', 'bg-orange-500', 'bg-red-500'];
              const total = revenusCategorie.reduce((sum, c) => sum + c.chiffreAffaires, 0);
              const percentage = total > 0 ? ((cat.chiffreAffaires / total) * 100).toFixed(1) : 0;

              return (
                <div key={cat.categorie}>
                  <div className="flex justify-between mb-1">
                    <span className="font-medium">{cat.categorie}</span>
                    <span className="text-gray-600">{cat.chiffreAffaires?.toFixed(2)} € ({percentage}%)</span>
                  </div>
                  <div className="w-full bg-gray-200 rounded-full h-3">
                    <div
                      className={`${colors[index % colors.length]} h-3 rounded-full`}
                      style={{ width: `${percentage}%` }}
                    />
                  </div>
                </div>
              );
            })}
            {revenusCategorie.length === 0 && (
              <p className="text-gray-500 text-center">Aucune donnée disponible</p>
            )}
          </div>
        </div>
      </div>

      {/* Ventes par mois */}
      <div className="bg-white rounded-xl shadow-md p-6 mb-8">
        <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
          <TrendingUp className="text-green-500" />
          Ventes {ventesAnnee?.annee || new Date().getFullYear()}
          <span className="ml-auto text-lg font-bold text-green-500">
            Total: {ventesAnnee?.totalAnnee?.toFixed(2) || 0} €
          </span>
        </h2>
        <div className="flex items-end justify-between h-48 gap-2">
          {moisNoms.map((mois, index) => {
            const ventesMois = ventesAnnee?.ventesParMois?.find(v => v.mois === index + 1);
            const maxVente = Math.max(...(ventesAnnee?.ventesParMois?.map(v => v.total) || [1]));
            const height = ventesMois ? (ventesMois.total / maxVente) * 100 : 0;

            return (
              <div key={mois} className="flex-1 flex flex-col items-center">
                <div className="w-full bg-gray-100 rounded-t relative" style={{ height: '150px' }}>
                  <div
                    className="absolute bottom-0 w-full bg-gradient-to-t from-orange-500 to-orange-300 rounded-t transition-all"
                    style={{ height: `${height}%` }}
                  />
                </div>
                <span className="text-xs mt-2 text-gray-600">{mois}</span>
              </div>
            );
          })}
        </div>
      </div>

      {/* Heures de pointe */}
      <div className="bg-white rounded-xl shadow-md p-6">
        <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
          <Clock className="text-purple-500" />
          Heures de Pointe
        </h2>
        <div className="flex items-end justify-between h-32 gap-1">
          {Array.from({ length: 24 }, (_, i) => {
            const heure = heuresPointe.find(h => h.heure === i);
            const maxCommandes = Math.max(...heuresPointe.map(h => h.nombreCommandes), 1);
            const height = heure ? (heure.nombreCommandes / maxCommandes) * 100 : 0;

            return (
              <div key={i} className="flex-1 flex flex-col items-center">
                <div
                  className={`w-full rounded-t ${height > 70 ? 'bg-red-500' : height > 40 ? 'bg-orange-500' : 'bg-blue-300'}`}
                  style={{ height: `${Math.max(height, 5)}%` }}
                  title={`${i}h: ${heure?.nombreCommandes || 0} commandes`}
                />
                {i % 3 === 0 && <span className="text-xs mt-1 text-gray-500">{i}h</span>}
              </div>
            );
          })}
        </div>
      </div>
    </div>
  );
}

export default Statistiques;
