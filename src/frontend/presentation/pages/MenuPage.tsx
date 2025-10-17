import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { startTableSession } from '@infrastructure/api/tableApi'

interface SessionData {
  sessionId: string
  tableNumber: number
  startedAt: string
}

function MenuPage() {
  const { tableNumber } = useParams<{ tableNumber: string }>()
  const [session, setSession] = useState<SessionData | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const initSession = async () => {
      if (!tableNumber) return

      try {
        setLoading(true)
        const data = await startTableSession(parseInt(tableNumber))
        setSession(data)
        setError(null)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error starting session')
      } finally {
        setLoading(false)
      }
    }

    initSession()
  }, [tableNumber])

  if (loading) {
    return (
      <div style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh'
      }}>
        <p>Loading...</p>
      </div>
    )
  }

  if (error) {
    return (
      <div style={{
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh',
        padding: '20px'
      }}>
        <h2>Error</h2>
        <p style={{ color: 'red', marginTop: '20px' }}>{error}</p>
      </div>
    )
  }

  return (
    <div style={{ padding: '20px', maxWidth: '1200px', margin: '0 auto' }}>
      <header style={{ marginBottom: '40px' }}>
        <h1>Table {tableNumber}</h1>
        {session && (
          <p style={{ marginTop: '10px', opacity: 0.8 }}>
            Session started: {new Date(session.startedAt).toLocaleString()}
          </p>
        )}
      </header>

      <main>
        <h2>Menu</h2>
        <p style={{ marginTop: '20px' }}>
          The product catalog will be implemented in the next phase.
        </p>
      </main>
    </div>
  )
}

export default MenuPage
