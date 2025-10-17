import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

function WelcomePage() {
  const [tableNumber, setTableNumber] = useState('')
  const navigate = useNavigate()

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()

    if (tableNumber && parseInt(tableNumber) > 0) {
      navigate(`/table/${tableNumber}`)
    }
  }

  return (
    <div style={{
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      minHeight: '100vh',
      padding: '20px'
    }}>
      <h1>Welcome to Restaurant</h1>
      <p style={{ marginTop: '20px', marginBottom: '40px' }}>
        Scan your table QR code or enter the number manually
      </p>

      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
        <input
          type="number"
          placeholder="Table number"
          value={tableNumber}
          onChange={(e) => setTableNumber(e.target.value)}
          min="1"
          style={{
            padding: '12px',
            fontSize: '1.2em',
            borderRadius: '8px',
            border: '1px solid #ccc',
            textAlign: 'center'
          }}
        />
        <button type="submit" style={{ padding: '12px 24px', fontSize: '1.1em' }}>
          Access Menu
        </button>
      </form>
    </div>
  )
}

export default WelcomePage
