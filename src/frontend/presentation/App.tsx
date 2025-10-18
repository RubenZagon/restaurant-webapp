import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import WelcomePage from './pages/WelcomePage'
import MenuPage from './pages/MenuPage'
import { KitchenDashboard } from './pages/KitchenDashboard'

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<WelcomePage />} />
        <Route path="/table/:tableNumber" element={<MenuPage />} />
        <Route path="/kitchen" element={<KitchenDashboard />} />
      </Routes>
    </Router>
  )
}

export default App
