import { Order } from '@infrastructure/api/ordersApi.ts'
import { colors } from '../../theme/colors'

interface OrderCardProps {
  order: Order
  onStatusUpdate: (orderId: string, newStatus: string) => Promise<void>
  updating?: boolean
}

export function OrderCard({ order, onStatusUpdate, updating = false }: OrderCardProps) {
  const getStatusColor = (status: string): string => {
    switch (status) {
      case 'Confirmed':
        return colors.secondary.main // Blue
      case 'Preparing':
        return colors.status.warning // Orange
      case 'Ready':
        return colors.status.success // Green
      case 'Delivered':
        return colors.neutral.gray // Gray
      default:
        return colors.neutral.gray
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
        backgroundColor: colors.background.paper,
        borderRadius: '10px',
        padding: '18px',
        boxShadow: '0 3px 8px rgba(0,0,0,0.12)',
        borderLeft: `5px solid ${getStatusColor(order.status)}`,
        marginBottom: '14px',
        transition: 'all 0.2s ease'
      }}
    >
      {/* Header */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '14px' }}>
        <div>
          <h3 style={{
            margin: '0 0 6px 0',
            fontSize: '20px',
            fontWeight: '700',
            color: colors.text.primary
          }}>
            Table {order.tableNumber}
          </h3>
          <p style={{
            margin: 0,
            fontSize: '12px',
            color: colors.text.secondary,
            fontFamily: 'monospace'
          }}>
            Order #{order.id.substring(0, 8)}
          </p>
        </div>
        <div style={{ textAlign: 'right' }}>
          <div
            style={{
              backgroundColor: getStatusColor(order.status),
              color: colors.text.inverse,
              padding: '6px 14px',
              borderRadius: '14px',
              fontSize: '13px',
              fontWeight: '700',
              marginBottom: '6px',
              boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
            }}
          >
            {order.status}
          </div>
          <p style={{
            margin: 0,
            fontSize: '11px',
            color: colors.text.secondary,
            fontWeight: '500'
          }}>
            {getElapsedTime()} ago
          </p>
        </div>
      </div>

      {/* Order Lines */}
      <div style={{
        borderTop: `2px solid ${colors.border.light}`,
        paddingTop: '14px',
        marginBottom: '14px',
        backgroundColor: colors.background.secondary,
        margin: '0 -18px',
        padding: '14px 18px'
      }}>
        {order.lines.map((line) => (
          <div
            key={line.id}
            style={{
              display: 'flex',
              justifyContent: 'space-between',
              marginBottom: '8px',
              fontSize: '15px',
              padding: '6px 0'
            }}
          >
            <span style={{ color: colors.text.primary }}>
              <strong style={{
                color: colors.primary.main,
                fontSize: '16px',
                marginRight: '6px'
              }}>
                {line.quantity}x
              </strong>
              {line.productName}
            </span>
            <span style={{
              color: colors.text.secondary,
              fontWeight: '600'
            }}>
              {line.subtotal.toFixed(2)} {order.currency}
            </span>
          </div>
        ))}
      </div>

      {/* Footer */}
      <div style={{
        borderTop: `2px solid ${colors.border.light}`,
        paddingTop: '14px'
      }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <p style={{
              margin: '0 0 4px 0',
              fontSize: '12px',
              color: colors.text.secondary
            }}>
              Created: {formatTime(order.createdAt)}
            </p>
            {order.confirmedAt && (
              <p style={{
                margin: 0,
                fontSize: '12px',
                color: colors.text.secondary
              }}>
                Confirmed: {formatTime(order.confirmedAt)}
              </p>
            )}
          </div>
          <div style={{ textAlign: 'right' }}>
            <p style={{
              margin: '0 0 10px 0',
              fontSize: '20px',
              fontWeight: '700',
              color: colors.primary.main
            }}>
              {order.total.toFixed(2)} {order.currency}
            </p>
            {nextStatus && nextStatusLabel && (
              <button
                onClick={() => onStatusUpdate(order.id, nextStatus)}
                disabled={updating}
                style={{
                  backgroundColor: getStatusColor(nextStatus),
                  color: colors.text.inverse,
                  border: 'none',
                  borderRadius: '8px',
                  padding: '10px 18px',
                  fontSize: '14px',
                  fontWeight: '700',
                  cursor: updating ? 'not-allowed' : 'pointer',
                  opacity: updating ? 0.6 : 1,
                  transition: 'all 0.2s ease',
                  boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
                }}
                onMouseOver={(e) => {
                  if (!updating) {
                    e.currentTarget.style.opacity = '0.9'
                    e.currentTarget.style.transform = 'translateY(-1px)'
                  }
                }}
                onMouseOut={(e) => {
                  if (!updating) {
                    e.currentTarget.style.opacity = '1'
                    e.currentTarget.style.transform = 'translateY(0)'
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
