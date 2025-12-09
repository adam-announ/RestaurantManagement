import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { 
  Home, 
  UtensilsCrossed, 
  CalendarDays, 
  ClipboardList, 
  Users, 
  Package, 
  Receipt,
  Grid3X3,
  LogOut,
  User,
  BarChart3
} from 'lucide-react';

function Navbar() {
  const location = useLocation();
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const getNavItems = () => {
    const clientItems = [
      { path: '/', label: 'Dashboard', icon: Home },
      { path: '/plats', label: 'Menu', icon: UtensilsCrossed },
      { path: '/reservations', label: 'Mes Réservations', icon: CalendarDays },
      { path: '/commandes', label: 'Mes Commandes', icon: ClipboardList },
    ];

    const serveurItems = [
      { path: '/', label: 'Dashboard', icon: Home },
      { path: '/tables', label: 'Tables', icon: Grid3X3 },
      { path: '/plats', label: 'Menu', icon: UtensilsCrossed },
      { path: '/reservations', label: 'Réservations', icon: CalendarDays },
      { path: '/commandes', label: 'Commandes', icon: ClipboardList },
      { path: '/factures', label: 'Factures', icon: Receipt },
    ];

    const cuisinierItems = [
      { path: '/', label: 'Dashboard', icon: Home },
      { path: '/commandes', label: 'Commandes', icon: ClipboardList },
      { path: '/plats', label: 'Menu', icon: UtensilsCrossed },
      { path: '/stocks', label: 'Stocks', icon: Package },
    ];

    const managerItems = [
      { path: '/', label: 'Dashboard', icon: Home },
      { path: '/statistiques', label: 'Statistiques', icon: BarChart3 },
      { path: '/tables', label: 'Tables', icon: Grid3X3 },
      { path: '/plats', label: 'Menu', icon: UtensilsCrossed },
      { path: '/reservations', label: 'Réservations', icon: CalendarDays },
      { path: '/commandes', label: 'Commandes', icon: ClipboardList },
      { path: '/factures', label: 'Factures', icon: Receipt },
      { path: '/employes', label: 'Employés', icon: Users },
      { path: '/stocks', label: 'Stocks', icon: Package },
    ];

    switch (user?.role) {
      case 'Client': return clientItems;
      case 'Serveur': return serveurItems;
      case 'Cuisinier': return cuisinierItems;
      case 'Manager': return managerItems;
      default: return clientItems;
    }
  };

  const navItems = getNavItems();

  const getRoleBadgeColor = (role) => {
    const colors = {
      'Client': 'bg-blue-500',
      'Serveur': 'bg-green-500',
      'Cuisinier': 'bg-yellow-500',
      'Manager': 'bg-purple-500'
    };
    return colors[role] || 'bg-gray-500';
  };

  return (
    <nav className="bg-gray-900 text-white w-64 min-h-screen p-4 flex flex-col">
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-orange-500">🍽️ RestaurantApp</h1>
      </div>

      <div className="bg-gray-800 rounded-lg p-4 mb-6">
        <div className="flex items-center gap-3">
          <div className="bg-orange-500 p-2 rounded-full">
            <User size={20} />
          </div>
          <div>
            <p className="font-semibold">{user?.prenom} {user?.nom}</p>
            <span className={`text-xs px-2 py-1 rounded-full ${getRoleBadgeColor(user?.role)}`}>
              {user?.role}
            </span>
          </div>
        </div>
      </div>

      <ul className="space-y-2 flex-1">
        {navItems.map((item) => {
          const Icon = item.icon;
          const isActive = location.pathname === item.path;
          return (
            <li key={item.path}>
              <Link
                to={item.path}
                className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
                  isActive
                    ? 'bg-orange-500 text-white'
                    : 'hover:bg-gray-800 text-gray-300'
                }`}
              >
                <Icon size={20} />
                {item.label}
              </Link>
            </li>
          );
        })}
      </ul>

      <button
        onClick={handleLogout}
        className="flex items-center gap-3 px-4 py-3 rounded-lg text-gray-300 hover:bg-red-500 hover:text-white transition-colors mt-4"
      >
        <LogOut size={20} />
        Déconnexion
      </button>
    </nav>
  );
}

export default Navbar;
