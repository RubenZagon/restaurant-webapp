import { useNavigate } from 'react-router-dom'
import { TableIcon } from '../components/TableIcon'

function WelcomePage() {
  const navigate = useNavigate()

  const handleTableSelect = (tableNumber: number) => {
    navigate(`/table/${tableNumber}`)
  }

  // Array with 7 tables
  const tables = [1, 2, 3, 4, 5, 6, 7]

  return (
    <div style={{
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      minHeight: '100vh',
      padding: '20px',
      backgroundColor: '#f5f5f5'
    }}>
      <h1 style={{ marginBottom: '10px', color: '#333' }}>Guachinche Canario</h1>
      <p style={{ marginBottom: '40px', color: '#666', fontSize: '1.1em' }}>
        Selecciona tu mesa
      </p>

      <div style={{
        display: 'grid',
        gridTemplateColumns: 'repeat(auto-fit, minmax(150px, 1fr))',
        gap: '30px',
        maxWidth: '800px',
        width: '100%',
        padding: '20px',
        backgroundColor: 'white',
        borderRadius: '12px',
        boxShadow: '0 2px 8px rgba(0,0,0,0.1)'
      }}>
        {tables.map((tableNumber) => (
          <TableIcon
            key={tableNumber}
            tableNumber={tableNumber}
            onClick={() => handleTableSelect(tableNumber)}
          />
        ))}
      </div>
    </div>
  )
}

export default WelcomePage
