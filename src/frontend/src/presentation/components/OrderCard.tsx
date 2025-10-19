import { Order } from '../../../infrastructure/api/ordersApi'

interface OrderCardProps {
  order: Order
  onStatusUpdate: (orderId: string, newStatus: string) => Promise<void>
  updating?: boolean
}

export function OrderCard({ order, onStatusUpdate, updating = false }: OrderCardProps) {
  const getStatusColor = (status: string): string => {
    switch (status) {
      case 'Confirmed':
        return '#3B82F6' // Blue
      case 'Preparing':
        return '#F59E0B' // Amber
      case 'Ready':
        return '#10B981' // Green
      case 'Delivered':
        return '#6B7280' // Gray
      default:
        return '#6B7280'
    }
  }

  const getNextStatus = (currentStatus: string): string | null => {
    switch (currentStatus) {
      case 'Confirmed':
        return 'Preparing'
      case 'Preparing':
        return 'Ready'
      case 'Ready':
        return 'Delivered'
      default:
        return null
    }
  }

  const getNextStatusLabel = (currentStatus: string): string | null => {
    switch (currentStatus) {
      case 'Confirmed':
        return 'Start Preparing'
      case 'Preparing':
        return 'Mark Ready'
      case 'Ready':
        return 'Mark Delivered'
      default:
        return null
    }
  }

  const nextStatus = getNextStatus(order.status)
  const nextStatusLabel = getNextStatusLabel(order.status)

  const formatTime = (dateString: string | null): string => {
    if (!dateString) return 'N/A'
    const date = new Date(dateString)
    return date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })
  }

  const getElapsedTime = (): string => {
    const created = new Date(order.createdAt)
    const now = new Date()
    const diffMs = now.getTime() - created.getTime()
    const diffMins = Math.floor(diffMs / 60000)
    return diffMins + ' min' + (diffMins !== 1 ? 's' : '')
  }

  return (
    <div
      style={{
        backgroundColor: 'white',
        borderRadius: '8px',
        padding: '16px',
        boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
        borderLeft: '4px solid ' + getStatusColor(order.status),
        marginBottom: '12px'
      }}
    >
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '12px' }}>
        <div>
          <h3 style={{ margin: '0 0 4px 0', fontSize: '18px', fontWeight: '600' }}>
            Table {order.tableNumber}
          </h3>
          <p style={{ margin: 0, fontSize: '12px', color: '#6B7280' }}>
            Order #{order.id.substring(0, 8)}
          </p>
        </div>
        <div style={{ textAlign: 'right' }}>
          <div
            style={{
              backgroundColor: getStatusColor(order.status),
              color: 'white',
              padding: '4px 12px',
              borderRadius: '12px',
              fontSize: '12px',
              fontWeight: '600',
              marginBottom: '4px'
            }}
          >
            {order.status}
          </div>
          <p style={{ margin: 0, fontSize: '11px', color: '#6B7280' }}>
            {getElapsedTime()} ago
          </p>
        </div>
      </div>

      {/* Order Lines */}
      <div style={{ borderTop: '1px solid #E5E7EB', paddingTop: '12px', marginBottom: '12px' }}>
        {order.lines.map((line) => (
          <div
            key={line.id}
            style={{
              display: 'flex',
              justifyContent: 'space-between',
              marginBottom: '6px',
              fontSize: '14px'
            }}
          >
            <span>
              <strong>{line.quantity}x</strong> {line.productName}
            </span>
            <span style={{ color: '#6B7280' }}>
              {line.subtotal.toFixed(2)} {order.currency}
            </span>
          </div>
        ))}
      </div>

      {/* Footer */}
      <div style={{ borderTop: '1px solid #E5E7EB', paddingTop: '12px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <p style={{ margin: '0 0 4px 0', fontSize: '12px', color: '#6B7280' }}>
              Created: {formatTime(order.createdAt)}
            </p>
            {order.confirmedAt && (
              <p style={{ margin: 0, fontSize: '12px', color: '#6B7280' }}>
                Confirmed: {formatTime(order.confirmedAt)}
              </p>
            )}
          </div>
          <div style={{ textAlign: 'right' }}>
            <p style={{ margin: '0 0 8px 0', fontSize: '18px', fontWeight: '700' }}>
              {order.total.toFixed(2)} {order.currency}
            </p>
            {nextStatus && nextStatusLabel && (
              <button
                onClick={() => onStatusUpdate(order.id, nextStatus)}
                disabled={updating}
                style={{
                  backgroundColor: getStatusColor(nextStatus),
                  color: 'white',
                  border: 'none',
                  borderRadius: '6px',
                  padding: '8px 16px',
                  fontSize: '14px',
                  fontWeight: '600',
                  cursor: updating ? 'not-allowed' : 'pointer',
                  opacity: updating ? 0.6 : 1,
                  transition: 'all 0.2s'
                }}
                onMouseOver={(e) => {
                  if (!updating) {
                    e.currentTarget.style.opacity = '0.9'
                  }
                }}
                onMouseOut={(e) => {
                  if (!updating) {
                    e.currentTarget.style.opacity = '1'
                  }
                }}
              >
                {updating ? 'Updating...' : nextStatusLabel}
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
