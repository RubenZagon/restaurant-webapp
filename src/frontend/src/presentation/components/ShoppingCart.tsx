import { useCartStore } from '../../store/cartStore'

interface ShoppingCartProps {
  isOpen: boolean
  onClose: () => void
  onCheckout: () => void
  submitting?: boolean
}

export function ShoppingCart({ isOpen, onClose, onCheckout, submitting = false }: ShoppingCartProps) {
  const items = useCartStore(state => state.items)
  const updateQuantity = useCartStore(state => state.updateQuantity)
  const removeItem = useCartStore(state => state.removeItem)
  const getTotalAmount = useCartStore(state => state.getTotalAmount)
  const clearCart = useCartStore(state => state.clearCart)

  if (!isOpen) return null

  const total = getTotalAmount()
  const currency = items[0]?.currency || 'EUR'

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      right: 0,
      width: '400px',
      height: '100vh',
      backgroundColor: '#ffffff',
      boxShadow: '-2px 0 10px rgba(0, 0, 0, 0.1)',
      zIndex: 1000,
      display: 'flex',
      flexDirection: 'column'
    }}>
      {/* Header */}
      <div style={{
        padding: '20px',
        borderBottom: '1px solid #ecf0f1',
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center'
      }}>
        <h2 style={{ margin: 0, fontSize: '24px' }}>Your Order</h2>
        <button
          onClick={onClose}
          style={{
            background: 'none',
            border: 'none',
            fontSize: '28px',
            cursor: 'pointer',
            color: '#7f8c8d'
          }}
        >
          ×
        </button>
      </div>

      {/* Cart Items */}
      <div style={{
        flex: 1,
        overflowY: 'auto',
        padding: '20px'
      }}>
        {items.length === 0 ? (
          <div style={{
            textAlign: 'center',
            color: '#7f8c8d',
            marginTop: '50px'
          }}>
            <p style={{ fontSize: '18px' }}>Your cart is empty</p>
            <p style={{ fontSize: '14px' }}>Add items from the menu to get started</p>
          </div>
        ) : (
          items.map(item => (
            <div
              key={item.productId}
              style={{
                padding: '15px',
                borderBottom: '1px solid #ecf0f1',
                marginBottom: '10px'
              }}
            >
              <div style={{
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'flex-start',
                marginBottom: '10px'
              }}>
                <div style={{ flex: 1 }}>
                  <h4 style={{ margin: '0 0 5px 0', fontSize: '16px' }}>
                    {item.name}
                  </h4>
                  <p style={{
                    margin: 0,
                    fontSize: '14px',
                    color: '#7f8c8d'
                  }}>
                    {item.price.toFixed(2)} {item.currency}
                  </p>
                </div>
                <button
                  onClick={() => removeItem(item.productId)}
                  style={{
                    background: 'none',
                    border: 'none',
                    color: '#e74c3c',
                    cursor: 'pointer',
                    fontSize: '20px',
                    padding: '0 5px'
                  }}
                  title="Remove item"
                >
                  ×
                </button>
              </div>

              {/* Quantity Controls */}
              <div style={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between'
              }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                  <button
                    onClick={() => updateQuantity(item.productId, item.quantity - 1)}
                    style={{
                      width: '30px',
                      height: '30px',
                      border: '1px solid #bdc3c7',
                      background: '#ffffff',
                      borderRadius: '5px',
                      cursor: 'pointer',
                      fontSize: '18px'
                    }}
                  >
                    -
                  </button>
                  <span style={{ fontSize: '16px', minWidth: '30px', textAlign: 'center' }}>
                    {item.quantity}
                  </span>
                  <button
                    onClick={() => updateQuantity(item.productId, item.quantity + 1)}
                    disabled={item.quantity >= 100}
                    style={{
                      width: '30px',
                      height: '30px',
                      border: '1px solid #bdc3c7',
                      background: item.quantity >= 100 ? '#ecf0f1' : '#ffffff',
                      borderRadius: '5px',
                      cursor: item.quantity >= 100 ? 'not-allowed' : 'pointer',
                      fontSize: '18px'
                    }}
                  >
                    +
                  </button>
                </div>
                <div style={{ fontSize: '16px', fontWeight: 'bold' }}>
                  {item.subtotal.toFixed(2)} {item.currency}
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      {/* Footer */}
      {items.length > 0 && (
        <div style={{
          padding: '20px',
          borderTop: '1px solid #ecf0f1',
          backgroundColor: '#f8f9fa'
        }}>
          <div style={{
            display: 'flex',
            justifyContent: 'space-between',
            marginBottom: '15px',
            fontSize: '20px',
            fontWeight: 'bold'
          }}>
            <span>Total:</span>
            <span>{total.toFixed(2)} {currency}</span>
          </div>

          <button
            onClick={onCheckout}
            disabled={submitting}
            style={{
              width: '100%',
              padding: '15px',
              backgroundColor: submitting ? '#95a5a6' : '#27ae60',
              color: 'white',
              border: 'none',
              borderRadius: '8px',
              fontSize: '18px',
              fontWeight: 'bold',
              cursor: submitting ? 'not-allowed' : 'pointer',
              marginBottom: '10px',
              opacity: submitting ? 0.7 : 1
            }}
          >
            {submitting ? 'Confirming...' : 'Confirm Order'}
          </button>

          <button
            onClick={clearCart}
            style={{
              width: '100%',
              padding: '12px',
              backgroundColor: 'transparent',
              color: '#e74c3c',
              border: '1px solid #e74c3c',
              borderRadius: '8px',
              fontSize: '14px',
              cursor: 'pointer'
            }}
          >
            Clear Cart
          </button>
        </div>
      )}
    </div>
  )
}
