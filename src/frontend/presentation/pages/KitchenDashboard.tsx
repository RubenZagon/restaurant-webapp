import { useState, useEffect, useRef } from 'react'
import * as signalR from '@microsoft/signalr'
import { Order, getAllActiveOrders, updateOrderStatus } from '../../infrastructure/api/ordersApi'
import { OrderCard } from '../../src/presentation/components/OrderCard'

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
    <div style={{ backgroundColor: '#F3F4F6', minHeight: '100vh', padding: '20px' }}>
      {/* Header */}
      <div
        style={{
          backgroundColor: 'white',
          padding: '20px',
          borderRadius: '12px',
          marginBottom: '20px',
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
        }}
      >
        <div style={{ marginBottom: '20px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <h1 style={{ margin: 0, fontSize: '28px', fontWeight: '700' }}>Kitchen Dashboard</h1>
            <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
              {/* Connection Status */}
              <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                <div
                  style={{
                    width: '10px',
                    height: '10px',
                    borderRadius: '50%',
                    backgroundColor: isConnected ? '#10B981' : '#EF4444'
                  }}
                />
                <span style={{ fontSize: '14px', color: '#6B7280' }}>
                  {isConnected ? 'Live' : 'Disconnected'}
                </span>
              </div>
              {/* Active Orders Count */}
              <div
                style={{
                  backgroundColor: '#3B82F6',
                  color: 'white',
                  padding: '8px 16px',
                  borderRadius: '20px',
                  fontWeight: '600'
                }}
              >
                {orders.length} Active Orders
              </div>
            </div>
          </div>

          {/* Search and Sort Controls */}
          <div style={{ display: 'flex', gap: '12px', marginTop: '16px', alignItems: 'center' }}>
            {/* Search Input */}
            <input
              type="text"
              placeholder="Search by table, order ID, or product..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              style={{
                flex: 1,
                padding: '10px 16px',
                border: '1px solid #D1D5DB',
                borderRadius: '8px',
                fontSize: '14px',
                outline: 'none'
              }}
            />

            {/* Sort Dropdown */}
            <select
              value={sortBy}
              onChange={(e) => setSortBy(e.target.value as 'time' | 'table' | 'total')}
              style={{
                padding: '10px 16px',
                border: '1px solid #D1D5DB',
                borderRadius: '8px',
                fontSize: '14px',
                backgroundColor: 'white',
                cursor: 'pointer',
                outline: 'none'
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
            marginTop: '16px'
          }}
        >
          <div style={{ padding: '16px', backgroundColor: '#F9FAFB', borderRadius: '8px' }}>
            <div style={{ fontSize: '12px', color: '#6B7280', marginBottom: '4px' }}>Total Orders</div>
            <div style={{ fontSize: '24px', fontWeight: '700', color: '#111827' }}>{stats.totalOrders}</div>
          </div>
          <div style={{ padding: '16px', backgroundColor: '#F9FAFB', borderRadius: '8px' }}>
            <div style={{ fontSize: '12px', color: '#6B7280', marginBottom: '4px' }}>Total Revenue</div>
            <div style={{ fontSize: '24px', fontWeight: '700', color: '#111827' }}>
              {stats.totalRevenue.toFixed(2)} EUR
            </div>
          </div>
          <div style={{ padding: '16px', backgroundColor: '#F9FAFB', borderRadius: '8px' }}>
            <div style={{ fontSize: '12px', color: '#6B7280', marginBottom: '4px' }}>Avg Order Value</div>
            <div style={{ fontSize: '24px', fontWeight: '700', color: '#111827' }}>
              {stats.avgOrderValue.toFixed(2)} EUR
            </div>
          </div>
          <div style={{ padding: '16px', backgroundColor: '#F9FAFB', borderRadius: '8px' }}>
            <div style={{ fontSize: '12px', color: '#6B7280', marginBottom: '4px' }}>In Queue</div>
            <div style={{ fontSize: '24px', fontWeight: '700', color: '#111827' }}>
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
            top: '20px',
            right: '20px',
            backgroundColor: '#10B981',
            color: 'white',
            padding: '16px 24px',
            borderRadius: '8px',
            boxShadow: '0 4px 6px rgba(0,0,0,0.1)',
            zIndex: 1000,
            animation: 'slideIn 0.3s ease-out'
          }}
        >
          {notification}
        </div>
      )}

      {/* Error Message */}
      {error && (
        <div
          style={{
            backgroundColor: '#FEE2E2',
            color: '#991B1B',
            padding: '16px',
            borderRadius: '8px',
            marginBottom: '20px'
          }}
        >
          {error}
        </div>
      )}

      {/* Kanban Board */}
      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))',
          gap: '20px'
        }}
      >
        {/* Confirmed Column */}
        <div>
          <div
            style={{
              backgroundColor: '#3B82F6',
              color: 'white',
              padding: '12px 16px',
              borderRadius: '8px 8px 0 0',
              fontWeight: '600',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}
          >
            <span>Confirmed</span>
            <span
              style={{
                backgroundColor: 'rgba(255,255,255,0.2)',
                padding: '4px 12px',
                borderRadius: '12px',
                fontSize: '14px'
              }}
            >
              {groupedOrders.Confirmed.length}
            </span>
          </div>
          <div style={{ backgroundColor: '#EFF6FF', padding: '16px', borderRadius: '0 0 8px 8px', minHeight: '400px' }}>
            {groupedOrders.Confirmed.length === 0 ? (
              <p style={{ textAlign: 'center', color: '#6B7280', padding: '20px' }}>No confirmed orders</p>
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
        <div>
          <div
            style={{
              backgroundColor: '#F59E0B',
              color: 'white',
              padding: '12px 16px',
              borderRadius: '8px 8px 0 0',
              fontWeight: '600',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}
          >
            <span>Preparing</span>
            <span
              style={{
                backgroundColor: 'rgba(255,255,255,0.2)',
                padding: '4px 12px',
                borderRadius: '12px',
                fontSize: '14px'
              }}
            >
              {groupedOrders.Preparing.length}
            </span>
          </div>
          <div style={{ backgroundColor: '#FEF3C7', padding: '16px', borderRadius: '0 0 8px 8px', minHeight: '400px' }}>
            {groupedOrders.Preparing.length === 0 ? (
              <p style={{ textAlign: 'center', color: '#6B7280', padding: '20px' }}>No orders in preparation</p>
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
        <div>
          <div
            style={{
              backgroundColor: '#10B981',
              color: 'white',
              padding: '12px 16px',
              borderRadius: '8px 8px 0 0',
              fontWeight: '600',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}
          >
            <span>Ready</span>
            <span
              style={{
                backgroundColor: 'rgba(255,255,255,0.2)',
                padding: '4px 12px',
                borderRadius: '12px',
                fontSize: '14px'
              }}
            >
              {groupedOrders.Ready.length}
            </span>
          </div>
          <div style={{ backgroundColor: '#D1FAE5', padding: '16px', borderRadius: '0 0 8px 8px', minHeight: '400px' }}>
            {groupedOrders.Ready.length === 0 ? (
              <p style={{ textAlign: 'center', color: '#6B7280', padding: '20px' }}>No ready orders</p>
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
        <div>
          <div
            style={{
              backgroundColor: '#6B7280',
              color: 'white',
              padding: '12px 16px',
              borderRadius: '8px 8px 0 0',
              fontWeight: '600',
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center'
            }}
          >
            <span>Delivered</span>
            <span
              style={{
                backgroundColor: 'rgba(255,255,255,0.2)',
                padding: '4px 12px',
                borderRadius: '12px',
                fontSize: '14px'
              }}
            >
              {groupedOrders.Delivered.length}
            </span>
          </div>
          <div style={{ backgroundColor: '#F3F4F6', padding: '16px', borderRadius: '0 0 8px 8px', minHeight: '400px' }}>
            {groupedOrders.Delivered.length === 0 ? (
              <p style={{ textAlign: 'center', color: '#6B7280', padding: '20px' }}>No delivered orders</p>
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
