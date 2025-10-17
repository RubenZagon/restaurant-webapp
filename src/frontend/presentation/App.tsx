import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import WelcomePage from './pages/WelcomePage'
import MenuPage from './pages/MenuPage'

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<WelcomePage />} />
        <Route path="/table/:tableNumber" element={<MenuPage />} />
      </Routes>
    </Router>
  )
}

export default App
