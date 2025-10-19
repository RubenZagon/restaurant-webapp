import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { startTableSession } from '@infrastructure/api/tableApi'
import { getAllCategories, getProductsByCategory, Category, Product } from '@infrastructure/api/productsApi'
import { getOrCreateOrderForTable, confirmOrder } from '@infrastructure/api/ordersApi'
import { processPayment } from '@infrastructure/api/paymentsApi'
import CategoryTabs from '../components/CategoryTabs'
import ProductCard from '../components/ProductCard'
import { CartIcon } from '../../src/presentation/components/CartIcon'
import { ShoppingCart } from '../../src/presentation/components/ShoppingCart'
import { PaymentModal } from '../../src/presentation/components/PaymentModal'
import { Toast, ToastType } from '../../src/presentation/components/Toast'
import { AllergenLegend } from '../../src/presentation/components/AllergenIcons'
import { useCartStore } from '@/src/store/cartStore.ts'
import { useOrderNotifications } from '@/src/hooks/useOrderNotifications.ts'
import { colors } from '@/src/theme/colors.ts'

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
  const [isPaymentModalOpen, setIsPaymentModalOpen] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const [processingPayment, setProcessingPayment] = useState(false)
  const [orderConfirmed, setOrderConfirmed] = useState(false)
  const [toast, setToast] = useState<{ message: string; type: ToastType } | null>(null)

  const setTableNumber = useCartStore(state => state.setTableNumber)
  const setOrderId = useCartStore(state => state.setOrderId)
  const orderId = useCartStore(state => state.orderId)
  const getTotalAmount = useCartStore(state => state.getTotalAmount)
  const items = useCartStore(state => state.items)

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
      setOrderConfirmed(true)
    } catch (error) {
      console.error('Error confirming order:', error)
      showToast('Failed to confirm order. Please try again.', 'error')
    } finally {
      setSubmitting(false)
    }
  }

  const handleOpenPayment = () => {
    setIsCartOpen(false)
    setIsPaymentModalOpen(true)
  }

  const handlePayment = async (paymentMethod: string) => {
    if (!orderId) {
      showToast('No order found.', 'error')
      return
    }

    try {
      setProcessingPayment(true)
      const result = await processPayment(orderId, paymentMethod)

      if (result.success) {
        showToast('Payment successful! Thank you for your order.', 'success')
        setIsPaymentModalOpen(false)

        // Clear cart and reset state
        useCartStore.getState().clearCart()
        setOrderConfirmed(false)

        // Create new order for future items
        setTimeout(async () => {
          const newOrder = await getOrCreateOrderForTable(parseInt(tableNumber!))
          setOrderId(newOrder.id)
        }, 1000)
      } else {
        showToast(`Payment failed: ${result.error}`, 'error')
      }
    } catch (error) {
      console.error('Error processing payment:', error)
      showToast('Payment processing failed. Please try again.', 'error')
    } finally {
      setProcessingPayment(false)
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

      <div style={{
        padding: '20px',
        maxWidth: '1200px',
        margin: '0 auto',
        backgroundColor: colors.background.main,
        minHeight: '100vh'
      }}>
        <header style={{
          marginBottom: '24px',
          backgroundColor: colors.background.paper,
          padding: '20px',
          borderRadius: '12px',
          border: `2px solid ${colors.border.light}`,
          boxShadow: '0 2px 8px rgba(0,0,0,0.08)'
        }}>
          <div style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            flexWrap: 'wrap',
            gap: '16px'
          }}>
            <div>
              <h1 style={{
                margin: '0 0 8px 0',
                color: colors.primary.main,
                fontSize: '32px',
                fontWeight: '700'
              }}>
                Table {tableNumber}
              </h1>
              {session && (
                <p style={{
                  margin: 0,
                  color: colors.text.secondary,
                  fontSize: '0.9em'
                }}>
                  Session: {new Date(session.startedAt).toLocaleTimeString()}
                </p>
              )}
            </div>
            <div style={{
              display: 'flex',
              alignItems: 'center',
              gap: '8px',
              padding: '10px 16px',
              borderRadius: '8px',
              backgroundColor: isConnected ? colors.orderStatus.ready.bg : '#ffebee',
              border: `2px solid ${isConnected ? colors.orderStatus.ready.border : '#ef5350'}`,
              fontSize: '0.9em'
            }}>
              <div style={{
                width: '10px',
                height: '10px',
                borderRadius: '50%',
                backgroundColor: isConnected ? colors.status.success : colors.status.error
              }} />
              <span style={{
                color: isConnected ? colors.orderStatus.ready.text : colors.status.error,
                fontWeight: '600'
              }}>
                {isConnected ? 'Live' : 'Disconnected'}
              </span>
            </div>
          </div>
        </header>

        <main>
          <h2 style={{
            marginBottom: '20px',
            color: colors.text.primary,
            fontSize: '28px',
            fontWeight: '600'
          }}>
            Menu
          </h2>

          <CategoryTabs
            categories={categories}
            selectedCategoryId={selectedCategoryId}
            onSelectCategory={setSelectedCategoryId}
          />

          {loadingProducts ? (
            <p style={{
              textAlign: 'center',
              color: colors.text.secondary,
              fontSize: '18px',
              padding: '40px 0'
            }}>
              Loading products...
            </p>
          ) : products.length === 0 ? (
            <p style={{
              textAlign: 'center',
              color: colors.text.secondary,
              padding: '40px 0',
              fontSize: '18px'
            }}>
              No products available in this category
            </p>
          ) : (
            <>
              <div style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))',
                gap: '20px',
                marginBottom: '32px'
              }}>
                {products.map((product) => (
                  <ProductCard key={product.id} product={product} />
                ))}
              </div>

              {/* Allergen Legend */}
              <AllergenLegend />
            </>
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
        onPayment={handleOpenPayment}
        submitting={submitting}
        orderConfirmed={orderConfirmed}
      />

      {/* Payment Modal */}
      <PaymentModal
        isOpen={isPaymentModalOpen}
        onClose={() => setIsPaymentModalOpen(false)}
        onConfirmPayment={handlePayment}
        totalAmount={getTotalAmount()}
        currency={items[0]?.currency || 'EUR'}
        isProcessing={processingPayment}
      />
    </>
  )
}

export default MenuPage
