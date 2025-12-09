import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import Navbar from './components/Navbar';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Tables from './pages/Tables';
import Plats from './pages/Plats';
import Reservations from './pages/Reservations';
import Commandes from './pages/Commandes';
import Factures from './pages/Factures';
import Employes from './pages/Employes';
import Stocks from './pages/Stocks';
import Statistiques from './pages/Statistiques';

function AppContent() {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return (
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    );
  }

  return (
    <div className="flex min-h-screen bg-gray-100">
      <Navbar />
      <main className="flex-1 p-8 overflow-auto">
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/tables" element={
            <ProtectedRoute roles={['Serveur', 'Manager']}>
              <Tables />
            </ProtectedRoute>
          } />
          <Route path="/plats" element={<Plats />} />
          <Route path="/reservations" element={<Reservations />} />
          <Route path="/commandes" element={<Commandes />} />
          <Route path="/factures" element={
            <ProtectedRoute roles={['Serveur', 'Manager']}>
              <Factures />
            </ProtectedRoute>
          } />
          <Route path="/employes" element={
            <ProtectedRoute roles={['Manager']}>
              <Employes />
            </ProtectedRoute>
          } />
          <Route path="/stocks" element={
            <ProtectedRoute roles={['Cuisinier', 'Manager']}>
              <Stocks />
            </ProtectedRoute>
          } />
          <Route path="/statistiques" element={
            <ProtectedRoute roles={['Manager']}>
              <Statistiques />
            </ProtectedRoute>
          } />
          <Route path="/login" element={<Navigate to="/" replace />} />
          <Route path="/register" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
    </div>
  );
}

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppContent />
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
