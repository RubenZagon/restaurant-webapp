import { useState, useEffect, useRef } from 'react'
import * as signalR from '@microsoft/signalr'
import { Order, getAllActiveOrders, updateOrderStatus } from '@infrastructure/api/ordersApi.ts'
import { OrderCard } from '../../src/presentation/components/OrderCard'
import { colors } from '@/src/theme/colors.ts'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

interface OrderNotification {
  type: string
  orderId: string
  tableNumber: number
  occurredAt: string
}

interface OrderStatusNotification extends OrderNotification {
  oldStatus: string
  newStatus: string
}

export function KitchenDashboard() {
  const [orders, setOrders] = useState<Order[]>([])
  const [loading, setLoading] = useState(true)
  const [updatingOrderId, setUpdatingOrderId] = useState<string | null>(null)
  const [isConnected, setIsConnected] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [notification, setNotification] = useState<string | null>(null)
  const [searchTerm, setSearchTerm] = useState('')
  const [sortBy, setSortBy] = useState<'time' | 'table' | 'total'>('time')
  const connectionRef = useRef<signalR.HubConnection | null>(null)

  // Fetch all active orders
  const fetchOrders = async () => {
    try {
      const activeOrders = await getAllActiveOrders()
      setOrders(activeOrders)
      setError(null)
    } catch (err) {
      console.error('Error fetching orders:', err)
      setError('Failed to load orders')
    } finally {
      setLoading(false)
    }
  }

  // Handle order status update
  const handleStatusUpdate = async (orderId: string, newStatus: string) => {
    setUpdatingOrderId(orderId)
    try {
      await updateOrderStatus(orderId, newStatus)
      await fetchOrders() // Refresh orders after update
      showNotification('Order status updated successfully!')
    } catch (err) {
      console.error('Error updating order status:', err)
      setError('Failed to update order status')
    } finally {
      setUpdatingOrderId(null)
    }
  }

  // Show temporary notification
  const showNotification = (message: string) => {
    setNotification(message)
    setTimeout(() => setNotification(null), 3000)
  }

  // Setup SignalR connection for real-time updates
  useEffect(() => {
    fetchOrders()

    // Create SignalR connection
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(API_BASE_URL + '/hubs/order-notifications', {
        skipNegotiation: false,
        transport:
          signalR.HttpTransportType.WebSockets |
          signalR.HttpTransportType.ServerSentEvents |
          signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build()

    connectionRef.current = connection

    // Handle new order notifications
    connection.on('NewOrder', (notif: OrderNotification) => {
      console.log('New order received:', notif)
      showNotification('New order from Table ' + notif.tableNumber + '!')
      fetchOrders()
    })

    // Handle order status changes
    connection.on('OrderStatusChanged', (notif: OrderStatusNotification) => {
      console.log('Order status changed:', notif)
      fetchOrders()
    })

    // Handle order confirmation
    connection.on('OrderConfirmed', (notif: OrderNotification) => {
      console.log('Order confirmed:', notif)
      showNotification('New order from Table ' + notif.tableNumber + '!')
      fetchOrders()
    })

    // Start connection and subscribe to kitchen group
    const startConnection = async () => {
      try {
        await connection.start()
        console.log('SignalR connected')
        setIsConnected(true)
        await connection.invoke('SubscribeToKitchen')
        console.log('Subscribed to kitchen notifications')
      } catch (err) {
        console.error('SignalR connection error:', err)
        setIsConnected(false)
      }
    }

    startConnection()

    // Cleanup on unmount
    return () => {
      if (connectionRef.current) {
        connectionRef.current
          .invoke('UnsubscribeFromKitchen')
          .then(() => connectionRef.current?.stop())
          .catch((err) => console.error('Error disconnecting:', err))
      }
    }
  }, [])

  // Filter orders by search term
  const filteredOrders = orders.filter((order) => {
    if (!searchTerm) return true
    const searchLower = searchTerm.toLowerCase()
    return (
      order.tableNumber.toString().includes(searchLower) ||
      order.id.toLowerCase().includes(searchLower) ||
      order.lines.some((line) => line.productName.toLowerCase().includes(searchLower))
    )
  })

  // Sort orders
  const sortedOrders = [...filteredOrders].sort((a, b) => {
    switch (sortBy) {
      case 'time':
        return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
      case 'table':
        return a.tableNumber - b.tableNumber
      case 'total':
        return b.total - a.total // Descending
      default:
        return 0
    }
  })

  // Calculate statistics
  const stats = {
    totalOrders: orders.length,
    totalRevenue: orders.reduce((sum, order) => sum + order.total, 0),
    avgOrderValue: orders.length > 0 ? orders.reduce((sum, order) => sum + order.total, 0) / orders.length : 0,
    byStatus: {
      Confirmed: orders.filter((o) => o.status === 'Confirmed').length,
      Preparing: orders.filter((o) => o.status === 'Preparing').length,
      Ready: orders.filter((o) => o.status === 'Ready').length,
      Delivered: orders.filter((o) => o.status === 'Delivered').length
    }
  }

  // Group orders by status for Kanban layout
  const groupedOrders = {
    Confirmed: sortedOrders.filter((o) => o.status === 'Confirmed'),
    Preparing: sortedOrders.filter((o) => o.status === 'Preparing'),
    Ready: sortedOrders.filter((o) => o.status === 'Ready'),
    Delivered: sortedOrders.filter((o) => o.status === 'Delivered')
  }

  if (loading) {
    return (
      <div
        style={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          height: '100vh',
          fontSize: '20px'
        }}
      >
        Loading orders...
      </div>
    )
  }

  return (
    <div style={{
      backgroundColor: colors.background.main,
      minHeight: '100vh',
      padding: '20px'
    }}>
      {/* Header */}
      <div
        style={{
          backgroundColor: colors.background.paper,
          padding: '24px',
          borderRadius: '16px',
          marginBottom: '24px',
          boxShadow: '0 4px 12px rgba(0,0,0,0.08)',
          border: `2px solid ${colors.border.light}`
        }}
      >
        <div style={{ marginBottom: '24px' }}>
          <div style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            flexWrap: 'wrap',
            gap: '16px'
          }}>
            <h1 style={{
              margin: 0,
              fontSize: '32px',
              fontWeight: '700',
              color: colors.primary.main
            }}>
              Kitchen Dashboard
            </h1>
            <div style={{ display: 'flex', alignItems: 'center', gap: '16px', flexWrap: 'wrap' }}>
              {/* Connection Status */}
              <div style={{
                display: 'flex',
                alignItems: 'center',
                gap: '10px',
                padding: '8px 16px',
                backgroundColor: isConnected ? colors.orderStatus.ready.bg : '#ffebee',
                borderRadius: '10px',
                border: `2px solid ${isConnected ? colors.orderStatus.ready.border : colors.status.error}`
              }}>
                <div
                  style={{
                    width: '12px',
                    height: '12px',
                    borderRadius: '50%',
                    backgroundColor: isConnected ? colors.status.success : colors.status.error
                  }}
                />
                <span style={{
                  fontSize: '14px',
                  fontWeight: '600',
                  color: isConnected ? colors.orderStatus.ready.text : colors.status.error
                }}>
                  {isConnected ? 'Live' : 'Disconnected'}
                </span>
              </div>
              {/* Active Orders Count */}
              <div
                style={{
                  backgroundColor: colors.primary.main,
                  color: colors.text.inverse,
                  padding: '10px 20px',
                  borderRadius: '24px',
                  fontWeight: '700',
                  fontSize: '15px',
                  boxShadow: '0 2px 6px rgba(0,0,0,0.15)'
                }}
              >
                {orders.length} Active Orders
              </div>
            </div>
          </div>

          {/* Search and Sort Controls */}
          <div style={{
            display: 'flex',
            gap: '12px',
            marginTop: '20px',
            alignItems: 'center',
            flexWrap: 'wrap'
          }}>
            {/* Search Input */}
            <input
              type="text"
              placeholder="Search by table, order ID, or product..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              style={{
                flex: 1,
                minWidth: '250px',
                padding: '12px 18px',
                border: `2px solid ${colors.border.main}`,
                borderRadius: '10px',
                fontSize: '14px',
                outline: 'none',
                backgroundColor: colors.background.paper,
                color: colors.text.primary
              }}
            />

            {/* Sort Dropdown */}
            <select
              value={sortBy}
              onChange={(e) => setSortBy(e.target.value as 'time' | 'table' | 'total')}
              style={{
                padding: '12px 18px',
                border: `2px solid ${colors.border.main}`,
                borderRadius: '10px',
                fontSize: '14px',
                backgroundColor: colors.background.paper,
                color: colors.text.primary,
                cursor: 'pointer',
                outline: 'none',
                fontWeight: '500'
              }}
            >
              <option value="time">Sort by Time</option>
              <option value="table">Sort by Table</option>
              <option value="total">Sort by Total</option>
            </select>
          </div>
        </div>

        {/* Statistics Panel */}
        <div
          style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
            gap: '16px',
            marginTop: '20px'
          }}
        >
          <div style={{
            padding: '18px',
            backgroundColor: colors.background.secondary,
            borderRadius: '12px',
            border: `2px solid ${colors.border.light}`
          }}>
            <div style={{
              fontSize: '12px',
              color: colors.text.secondary,
              marginBottom: '6px',
              fontWeight: '600',
              textTransform: 'uppercase',
              letterSpacing: '0.5px'
            }}>
              Total Orders
            </div>
            <div style={{
              fontSize: '28px',
              fontWeight: '700',
              color: colors.primary.main
            }}>
              {stats.totalOrders}
            </div>
          </div>
          <div style={{
            padding: '18px',
            backgroundColor: colors.background.secondary,
            borderRadius: '12px',
            border: `2px solid ${colors.border.light}`
          }}>
            <div style={{
              fontSize: '12px',
              color: colors.text.secondary,
              marginBottom: '6px',
              fontWeight: '600',
              textTransform: 'uppercase',
              letterSpacing: '0.5px'
            }}>
              Total Revenue
            </div>
            <div style={{
              fontSize: '28px',
              fontWeight: '700',
              color: colors.primary.main
            }}>
              {stats.totalRevenue.toFixed(2)} EUR
            </div>
          </div>
          <div style={{
            padding: '18px',
            backgroundColor: colors.background.secondary,
            borderRadius: '12px',
            border: `2px solid ${colors.border.light}`
          }}>
            <div style={{
              fontSize: '12px',
              color: colors.text.secondary,
              marginBottom: '6px',
              fontWeight: '600',
              textTransform: 'uppercase',
              letterSpacing: '0.5px'
            }}>
              Avg Order Value
            </div>
            <div style={{
              fontSize: '28px',
              fontWeight: '700',
              color: colors.primary.main
            }}>
              {stats.avgOrderValue.toFixed(2)} EUR
            </div>
          </div>
          <div style={{
            padding: '18px',
            backgroundColor: colors.background.secondary,
            borderRadius: '12px',
            border: `2px solid ${colors.border.light}`
          }}>
            <div style={{
              fontSize: '12px',
              color: colors.text.secondary,
              marginBottom: '6px',
              fontWeight: '600',
              textTransform: 'uppercase',
              letterSpacing: '0.5px'
            }}>
              In Queue
            </div>
            <div style={{
              fontSize: '28px',
              fontWeight: '700',
              color: colors.status.warning
            }}>
              {stats.byStatus.Confirmed + stats.byStatus.Preparing}
            </div>
          </div>
        </div>
      </div>

      {/* Notification Toast */}
      {notification && (
        <div
          style={{
            position: 'fixed',
            top: '24px',
            right: '24px',
            backgroundColor: colors.status.success,
            color: colors.text.inverse,
            padding: '18px 28px',
            borderRadius: '12px',
            boxShadow: '0 6px 16px rgba(0,0,0,0.2)',
            zIndex: 1000,
            fontWeight: '600',
            fontSize: '15px',
            border: `2px solid ${colors.orderStatus.ready.border}`
          }}
        >
          {notification}
        </div>
      )}

      {/* Error Message */}
      {error && (
        <div
          style={{
            backgroundColor: '#ffebee',
            color: colors.status.error,
            padding: '18px 24px',
            borderRadius: '12px',
            marginBottom: '24px',
            border: `2px solid ${colors.status.error}`,
            fontWeight: '600'
          }}
        >
          {error}
        </div>
      )}

      {/* Kanban Board */}
      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(340px, 1fr))',
          gap: '24px'
        }}
      >
        {/* Confirmed Column */}
        <div style={{
          backgroundColor: colors.background.paper,
          borderRadius: '12px',
          overflow: 'hidden',
          border: `2px solid ${colors.secondary.main}`,
          boxShadow: '0 4px 8px rgba(0,0,0,0.08)'
        }}>
          <div
            style={{
              backgroundColor: colors.secondary.main,
              color: colors.text.inverse,
              padding: '14px 20px',
              fontWeight: '700',
              fontSize: '16px',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}
          >
            <span>Confirmed</span>
            <span
              style={{
                backgroundColor: 'rgba(255,255,255,0.25)',
                padding: '6px 14px',
                borderRadius: '14px',
                fontSize: '14px',
                fontWeight: '700'
              }}
            >
              {groupedOrders.Confirmed.length}
            </span>
          </div>
          <div style={{
            backgroundColor: colors.orderStatus.confirmed.bg,
            padding: '18px',
            minHeight: '500px'
          }}>
            {groupedOrders.Confirmed.length === 0 ? (
              <p style={{
                textAlign: 'center',
                color: colors.text.secondary,
                padding: '40px 20px',
                fontWeight: '500'
              }}>
                No confirmed orders
              </p>
            ) : (
              groupedOrders.Confirmed.map((order) => (
                <OrderCard
                  key={order.id}
                  order={order}
                  onStatusUpdate={handleStatusUpdate}
                  updating={updatingOrderId === order.id}
                />
              ))
            )}
          </div>
        </div>

        {/* Preparing Column */}
        <div style={{
          backgroundColor: colors.background.paper,
          borderRadius: '12px',
          overflow: 'hidden',
          border: `2px solid ${colors.status.warning}`,
          boxShadow: '0 4px 8px rgba(0,0,0,0.08)'
        }}>
          <div
            style={{
              backgroundColor: colors.status.warning,
              color: colors.text.inverse,
              padding: '14px 20px',
              fontWeight: '700',
              fontSize: '16px',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}
          >
            <span>Preparing</span>
            <span
              style={{
                backgroundColor: 'rgba(255,255,255,0.25)',
                padding: '6px 14px',
                borderRadius: '14px',
                fontSize: '14px',
                fontWeight: '700'
              }}
            >
              {groupedOrders.Preparing.length}
            </span>
          </div>
          <div style={{
            backgroundColor: colors.orderStatus.preparing.bg,
            padding: '18px',
            minHeight: '500px'
          }}>
            {groupedOrders.Preparing.length === 0 ? (
              <p style={{
                textAlign: 'center',
                color: colors.text.secondary,
                padding: '40px 20px',
                fontWeight: '500'
              }}>
                No orders in preparation
              </p>
            ) : (
              groupedOrders.Preparing.map((order) => (
                <OrderCard
                  key={order.id}
                  order={order}
                  onStatusUpdate={handleStatusUpdate}
                  updating={updatingOrderId === order.id}
                />
              ))
            )}
          </div>
        </div>

        {/* Ready Column */}
        <div style={{
          backgroundColor: colors.background.paper,
          borderRadius: '12px',
          overflow: 'hidden',
          border: `2px solid ${colors.status.success}`,
          boxShadow: '0 4px 8px rgba(0,0,0,0.08)'
        }}>
          <div
            style={{
              backgroundColor: colors.status.success,
              color: colors.text.inverse,
              padding: '14px 20px',
              fontWeight: '700',
              fontSize: '16px',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}
          >
            <span>Ready</span>
            <span
              style={{
                backgroundColor: 'rgba(255,255,255,0.25)',
                padding: '6px 14px',
                borderRadius: '14px',
                fontSize: '14px',
                fontWeight: '700'
              }}
            >
              {groupedOrders.Ready.length}
            </span>
          </div>
          <div style={{
            backgroundColor: colors.orderStatus.ready.bg,
            padding: '18px',
            minHeight: '500px'
          }}>
            {groupedOrders.Ready.length === 0 ? (
              <p style={{
                textAlign: 'center',
                color: colors.text.secondary,
                padding: '40px 20px',
                fontWeight: '500'
              }}>
                No ready orders
              </p>
            ) : (
              groupedOrders.Ready.map((order) => (
                <OrderCard
                  key={order.id}
                  order={order}
                  onStatusUpdate={handleStatusUpdate}
                  updating={updatingOrderId === order.id}
                />
              ))
            )}
          </div>
        </div>

        {/* Delivered Column */}
        <div style={{
          backgroundColor: colors.background.paper,
          borderRadius: '12px',
          overflow: 'hidden',
          border: `2px solid ${colors.neutral.gray}`,
          boxShadow: '0 4px 8px rgba(0,0,0,0.08)'
        }}>
          <div
            style={{
              backgroundColor: colors.neutral.gray,
              color: colors.text.inverse,
              padding: '14px 20px',
              fontWeight: '700',
              fontSize: '16px',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}
          >
            <span>Delivered</span>
            <span
              style={{
                backgroundColor: 'rgba(255,255,255,0.25)',
                padding: '6px 14px',
                borderRadius: '14px',
                fontSize: '14px',
                fontWeight: '700'
              }}
            >
              {groupedOrders.Delivered.length}
            </span>
          </div>
          <div style={{
            backgroundColor: colors.orderStatus.delivered.bg,
            padding: '18px',
            minHeight: '500px'
          }}>
            {groupedOrders.Delivered.length === 0 ? (
              <p style={{
                textAlign: 'center',
                color: colors.text.secondary,
                padding: '40px 20px',
                fontWeight: '500'
              }}>
                No delivered orders
              </p>
            ) : (
              groupedOrders.Delivered.map((order) => (
                <OrderCard
                  key={order.id}
                  order={order}
                  onStatusUpdate={handleStatusUpdate}
                  updating={updatingOrderId === order.id}
                />
              ))
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
