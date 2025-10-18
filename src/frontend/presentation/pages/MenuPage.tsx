import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { startTableSession } from '@infrastructure/api/tableApi'
import { getAllCategories, getProductsByCategory, Category, Product } from '@infrastructure/api/productsApi'
import { getOrCreateOrderForTable, confirmOrder } from '@infrastructure/api/ordersApi'
import CategoryTabs from '../components/CategoryTabs'
import ProductCard from '../components/ProductCard'
import { CartIcon } from '../../src/presentation/components/CartIcon'
import { ShoppingCart } from '../../src/presentation/components/ShoppingCart'
import { Toast, ToastType } from '../../src/presentation/components/Toast'
import { useCartStore } from '../../src/store/cartStore'
import { useOrderNotifications } from '../../src/hooks/useOrderNotifications'

interface SessionData {
  sessionId: string
  tableNumber: number
  startedAt: string
}

function MenuPage() {
  const { tableNumber } = useParams<{ tableNumber: string }>()
  const [session, setSession] = useState<SessionData | null>(null)
  const [categories, setCategories] = useState<Category[]>([])
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(null)
  const [products, setProducts] = useState<Product[]>([])
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)
  const [loadingProducts, setLoadingProducts] = useState(false)
  const [isCartOpen, setIsCartOpen] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const [toast, setToast] = useState<{ message: string; type: ToastType } | null>(null)

  const setTableNumber = useCartStore(state => state.setTableNumber)
  const setOrderId = useCartStore(state => state.setOrderId)
  const orderId = useCartStore(state => state.orderId)

  // SignalR real-time notifications
  const { isConnected, lastNotification, error: signalRError } = useOrderNotifications(
    tableNumber ? parseInt(tableNumber) : 0
  )

  const showToast = (message: string, type: ToastType) => {
    setToast({ message, type })
  }

  // Handle incoming SignalR notifications
  useEffect(() => {
    if (lastNotification) {
      console.log('Received notification:', lastNotification)

      if (lastNotification.type === 'OrderStatusChanged') {
        const statusNotification = lastNotification as any
        showToast(
          `Your order status changed to: ${statusNotification.newStatus}`,
          'info'
        )
      }
    }
  }, [lastNotification])

  // Show SignalR connection errors
  useEffect(() => {
    if (signalRError) {
      console.error('SignalR error:', signalRError)
    }
  }, [signalRError])

  useEffect(() => {
    const init = async () => {
      if (!tableNumber) return

      try {
        setLoading(true)

        // Start session
        const sessionData = await startTableSession(parseInt(tableNumber))
        setSession(sessionData)

        // Set table number in cart store
        setTableNumber(parseInt(tableNumber))

        // Get or create order for this table
        const order = await getOrCreateOrderForTable(parseInt(tableNumber))
        setOrderId(order.id)

        // Load categories
        const categoriesData = await getAllCategories()
        setCategories(categoriesData)

        // Auto-select first category
        if (categoriesData.length > 0) {
          setSelectedCategoryId(categoriesData[0].id)
        }

        setError(null)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error loading data')
      } finally {
        setLoading(false)
      }
    }

    init()
  }, [tableNumber])

  useEffect(() => {
    const loadProducts = async () => {
      if (!selectedCategoryId) return

      try {
        setLoadingProducts(true)
        const productsData = await getProductsByCategory(selectedCategoryId)
        setProducts(productsData)
      } catch (err) {
        console.error('Error loading products:', err)
      } finally {
        setLoadingProducts(false)
      }
    }

    loadProducts()
  }, [selectedCategoryId])

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

  const handleCheckout = async () => {
    if (!orderId) {
      showToast('No order found. Please add items to your cart first.', 'warning')
      return
    }

    try {
      setSubmitting(true)
      await confirmOrder(orderId)

      showToast(`Order confirmed! Your order has been sent to the kitchen.`, 'success')

      // Clear cart after successful confirmation
      useCartStore.getState().clearCart()
      setIsCartOpen(false)

      // Create new order for future items
      setTimeout(async () => {
        const newOrder = await getOrCreateOrderForTable(parseInt(tableNumber!))
        setOrderId(newOrder.id)
      }, 1000)
    } catch (error) {
      console.error('Error confirming order:', error)
      showToast('Failed to confirm order. Please try again.', 'error')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <>
      {/* Toast Notification */}
      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}

      <div style={{ padding: '20px', maxWidth: '1200px', margin: '0 auto' }}>
        <header style={{ marginBottom: '24px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <div>
              <h1>Table {tableNumber}</h1>
              {session && (
                <p style={{ marginTop: '8px', opacity: 0.7, fontSize: '0.9em' }}>
                  Session: {new Date(session.startedAt).toLocaleTimeString()}
                </p>
              )}
            </div>
            <div style={{
              display: 'flex',
              alignItems: 'center',
              gap: '8px',
              padding: '8px 12px',
              borderRadius: '6px',
              backgroundColor: isConnected ? '#e8f5e9' : '#ffebee',
              fontSize: '0.85em'
            }}>
              <div style={{
                width: '8px',
                height: '8px',
                borderRadius: '50%',
                backgroundColor: isConnected ? '#4caf50' : '#f44336'
              }} />
              <span style={{ color: isConnected ? '#2e7d32' : '#c62828' }}>
                {isConnected ? 'Live' : 'Disconnected'}
              </span>
            </div>
          </div>
        </header>

        <main>
          <h2 style={{ marginBottom: '16px' }}>Menu</h2>

          <CategoryTabs
            categories={categories}
            selectedCategoryId={selectedCategoryId}
            onSelectCategory={setSelectedCategoryId}
          />

          {loadingProducts ? (
            <p>Loading products...</p>
          ) : products.length === 0 ? (
            <p style={{ textAlign: 'center', color: '#999', padding: '40px 0' }}>
              No products available in this category
            </p>
          ) : (
            <div>
              {products.map((product) => (
                <ProductCard key={product.id} product={product} />
              ))}
            </div>
          )}
        </main>
      </div>

      {/* Cart Icon */}
      <CartIcon onClick={() => setIsCartOpen(true)} />

      {/* Shopping Cart Sidebar */}
      <ShoppingCart
        isOpen={isCartOpen}
        onClose={() => setIsCartOpen(false)}
        onCheckout={handleCheckout}
        submitting={submitting}
      />
    </>
  )
}

export default MenuPage
