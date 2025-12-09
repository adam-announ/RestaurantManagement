import { useState, useEffect } from 'react';
import { 
  UtensilsCrossed, 
  CalendarDays, 
  ClipboardList, 
  Users,
  AlertTriangle,
  TrendingUp
} from 'lucide-react';
import { tablesApi, commandesApi, reservationsApi, stocksApi } from '../services/api';

function Dashboard() {
  const [stats, setStats] = useState({
    tablesDisponibles: 0,
    commandesEnCours: 0,
    reservationsJour: 0,
    stocksAlerte: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const [tables, commandes, reservations, stocks] = await Promise.all([
          tablesApi.getDisponibles(),
          commandesApi.getEnCours(),
          reservationsApi.getByDate(new Date().toISOString().split('T')[0]),
          stocksApi.getFaibles(),
        ]);

        setStats({
          tablesDisponibles: tables.data.length,
          commandesEnCours: commandes.data.length,
          reservationsJour: reservations.data.length,
          stocksAlerte: stocks.data.length,
        });
      } catch (error) {
        console.error('Erreur chargement stats:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  const cards = [
    {
      title: 'Tables Disponibles',
      value: stats.tablesDisponibles,
      icon: UtensilsCrossed,
      color: 'bg-green-500',
    },
    {
      title: 'Commandes En Cours',
      value: stats.commandesEnCours,
      icon: ClipboardList,
      color: 'bg-blue-500',
    },
    {
      title: 'Réservations Aujourd\'hui',
      value: stats.reservationsJour,
      icon: CalendarDays,
      color: 'bg-purple-500',
    },
    {
      title: 'Alertes Stock',
      value: stats.stocksAlerte,
      icon: AlertTriangle,
      color: stats.stocksAlerte > 0 ? 'bg-red-500' : 'bg-gray-500',
    },
  ];

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-xl text-gray-500">Chargement...</div>
      </div>
    );
  }

  return (
    <div>
      <h1 className="text-3xl font-bold mb-8">Dashboard</h1>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        {cards.map((card, index) => {
          const Icon = card.icon;
          return (
            <div key={index} className="bg-white rounded-xl shadow-md p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-gray-500 text-sm">{card.title}</p>
                  <p className="text-3xl font-bold mt-2">{card.value}</p>
                </div>
                <div className={`${card.color} p-4 rounded-full`}>
                  <Icon className="text-white" size={24} />
                </div>
              </div>
            </div>
          );
        })}
      </div>

      <div className="bg-white rounded-xl shadow-md p-6">
        <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
          <TrendingUp size={24} />
          Activité Récente
        </h2>
        <p className="text-gray-500">Bienvenue dans votre système de gestion de restaurant !</p>
      </div>
    </div>
  );
}

export default Dashboard;
