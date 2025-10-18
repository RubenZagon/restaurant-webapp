import { useEffect, useRef, useState } from 'react'
import * as signalR from '@microsoft/signalr'

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

interface UseOrderNotificationsResult {
  isConnected: boolean
  lastNotification: OrderNotification | null
  error: string | null
}

export function useOrderNotifications(tableNumber: number): UseOrderNotificationsResult {
  const [isConnected, setIsConnected] = useState(false)
  const [lastNotification, setLastNotification] = useState<OrderNotification | null>(null)
  const [error, setError] = useState<string | null>(null)
  const connectionRef = useRef<signalR.HubConnection | null>(null)

  useEffect(() => {
    // Create SignalR connection
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_BASE_URL}/hubs/order-notifications`, {
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.ServerSentEvents | signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build()

    connectionRef.current = connection

    // Set up event handlers
    connection.on('OrderConfirmed', (notification: OrderNotification) => {
      console.log('Order confirmed notification received:', notification)
      setLastNotification(notification)
    })

    connection.on('OrderStatusChanged', (notification: OrderStatusNotification) => {
      console.log('Order status changed notification received:', notification)
      setLastNotification(notification)
    })

    connection.on('NewOrder', (notification: OrderNotification) => {
      console.log('New order notification received:', notification)
      setLastNotification(notification)
    })

    // Handle connection state changes
    connection.onclose((error) => {
      console.log('SignalR connection closed:', error)
      setIsConnected(false)
      if (error) {
        setError(error.message)
      }
    })

    connection.onreconnecting((error) => {
      console.log('SignalR reconnecting:', error)
      setIsConnected(false)
    })

    connection.onreconnected((connectionId) => {
      console.log('SignalR reconnected:', connectionId)
      setIsConnected(true)
      setError(null)
      // Re-subscribe to table after reconnection
      connection.invoke('SubscribeToTable', tableNumber)
        .catch(err => console.error('Error re-subscribing to table:', err))
    })

    // Start connection
    const startConnection = async () => {
      try {
        await connection.start()
        console.log('SignalR connected successfully')
        setIsConnected(true)
        setError(null)

        // Subscribe to table notifications
        await connection.invoke('SubscribeToTable', tableNumber)
        console.log(`Subscribed to table ${tableNumber}`)
      } catch (err) {
        console.error('SignalR connection error:', err)
        setError(err instanceof Error ? err.message : 'Connection failed')
        setIsConnected(false)
      }
    }

    startConnection()

    // Cleanup on unmount
    return () => {
      if (connectionRef.current) {
        connectionRef.current.invoke('UnsubscribeFromTable', tableNumber)
          .then(() => {
            console.log(`Unsubscribed from table ${tableNumber}`)
            return connectionRef.current?.stop()
          })
          .catch(err => console.error('Error during cleanup:', err))
      }
    }
  }, [tableNumber])

  return {
    isConnected,
    lastNotification,
    error
  }
}
