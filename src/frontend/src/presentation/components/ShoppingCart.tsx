import { useTranslation } from 'react-i18next'
import { useCartStore } from '../../store/cartStore'
import { colors } from '../../theme/colors'

interface ShoppingCartProps {
  isOpen: boolean
  onClose: () => void
  onCheckout: () => void
  onPayment?: () => void
  submitting?: boolean
  orderConfirmed?: boolean
}

export function ShoppingCart({
  isOpen,
  onClose,
  onCheckout,
  onPayment,
  submitting = false,
  orderConfirmed = false
}: ShoppingCartProps) {
  const { t } = useTranslation()
  const items = useCartStore(state => state.items)
  const updateQuantity = useCartStore(state => state.updateQuantity)
  const removeItem = useCartStore(state => state.removeItem)
  const getTotalAmount = useCartStore(state => state.getTotalAmount)
  const clearCart = useCartStore(state => state.clearCart)

  if (!isOpen) return null

  const total = getTotalAmount()
  const currency = items[0]?.currency || 'EUR'

  return (
    <>
      {/* Backdrop for mobile */}
      <div
        style={{
          position: 'fixed',
          top: 0,
          left: 0,
          width: '100vw',
          height: '100vh',
          backgroundColor: 'rgba(0, 0, 0, 0.5)',
          zIndex: 999,
          display: isOpen ? 'block' : 'none'
        }}
        onClick={onClose}
      />

      {/* Cart Sidebar */}
      <div style={{
        position: 'fixed',
        top: 0,
        right: 0,
        width: '100%',
        maxWidth: '450px',
        height: '100vh',
        backgroundColor: colors.background.paper,
        boxShadow: '-4px 0 20px rgba(0, 0, 0, 0.15)',
        zIndex: 1000,
        display: 'flex',
        flexDirection: 'column',
        transform: isOpen ? 'translateX(0)' : 'translateX(100%)',
        transition: 'transform 0.3s ease-in-out'
      }}>
      {/* Header */}
      <div style={{
        padding: '20px',
        borderBottom: `2px solid ${colors.border.light}`,
        backgroundColor: colors.primary.main,
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center'
      }}>
        <h2 style={{
          margin: 0,
          fontSize: '24px',
          fontWeight: '600',
          color: colors.text.inverse
        }}>
          {t('cart.title')}
        </h2>
        <button
          onClick={onClose}
          style={{
            background: 'none',
            border: 'none',
            fontSize: '32px',
            cursor: 'pointer',
            color: colors.text.inverse,
            lineHeight: '1',
            padding: '0 8px'
          }}
          aria-label={t('cart.closeCart')}
        >
          ×
        </button>
      </div>

      {/* Cart Items */}
      <div style={{
        flex: 1,
        overflowY: 'auto',
        padding: '20px',
        backgroundColor: colors.background.secondary
      }}>
        {items.length === 0 ? (
          <div style={{
            textAlign: 'center',
            color: colors.text.secondary,
            marginTop: '50px'
          }}>
            <p style={{ fontSize: '18px', fontWeight: '500' }}>{t('cart.empty')}</p>
            <p style={{ fontSize: '14px' }}>{t('cart.emptySubtitle')}</p>
          </div>
        ) : (
          items.map(item => (
            <div
              key={item.productId}
              style={{
                padding: '16px',
                borderBottom: `1px solid ${colors.border.light}`,
                marginBottom: '12px',
                backgroundColor: colors.background.paper,
                borderRadius: '8px',
                boxShadow: '0 2px 4px rgba(0,0,0,0.05)'
              }}
            >
              <div style={{
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'flex-start',
                marginBottom: '12px'
              }}>
                <div style={{ flex: 1 }}>
                  <h4 style={{
                    margin: '0 0 6px 0',
                    fontSize: '16px',
                    fontWeight: '600',
                    color: colors.text.primary
                  }}>
                    {item.name}
                  </h4>
                  <p style={{
                    margin: 0,
                    fontSize: '14px',
                    color: colors.text.secondary
                  }}>
                    {item.price.toFixed(2)} {item.currency}
                  </p>
                </div>
                <button
                  onClick={() => removeItem(item.productId)}
                  style={{
                    background: 'none',
                    border: 'none',
                    color: colors.status.error,
                    cursor: 'pointer',
                    fontSize: '24px',
                    padding: '0 8px',
                    lineHeight: '1'
                  }}
                  title={t('cart.removeItem')}
                  aria-label={t('cart.removeItem')}
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
                <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                  <button
                    onClick={() => updateQuantity(item.productId, item.quantity - 1)}
                    style={{
                      width: '36px',
                      height: '36px',
                      border: `1px solid ${colors.border.main}`,
                      background: colors.background.paper,
                      borderRadius: '6px',
                      cursor: 'pointer',
                      fontSize: '18px',
                      fontWeight: '600',
                      color: colors.text.primary,
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center'
                    }}
                    aria-label={t('cart.decreaseQuantity')}
                  >
                    -
                  </button>
                  <span style={{
                    fontSize: '16px',
                    minWidth: '30px',
                    textAlign: 'center',
                    fontWeight: '600',
                    color: colors.text.primary
                  }}>
                    {item.quantity}
                  </span>
                  <button
                    onClick={() => updateQuantity(item.productId, item.quantity + 1)}
                    disabled={item.quantity >= 100}
                    style={{
                      width: '36px',
                      height: '36px',
                      border: `1px solid ${colors.border.main}`,
                      background: item.quantity >= 100 ? colors.neutral.lightGray : colors.background.paper,
                      borderRadius: '6px',
                      cursor: item.quantity >= 100 ? 'not-allowed' : 'pointer',
                      fontSize: '18px',
                      fontWeight: '600',
                      color: item.quantity >= 100 ? colors.text.disabled : colors.text.primary,
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center'
                    }}
                    aria-label={t('cart.increaseQuantity')}
                  >
                    +
                  </button>
                </div>
                <div style={{
                  fontSize: '18px',
                  fontWeight: 'bold',
                  color: colors.primary.main
                }}>
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
          borderTop: `2px solid ${colors.border.main}`,
          backgroundColor: colors.neutral.white
        }}>
          <div style={{
            display: 'flex',
            justifyContent: 'space-between',
            marginBottom: '16px',
            fontSize: '22px',
            fontWeight: 'bold',
            color: colors.text.primary
          }}>
            <span>{t('common.total')}:</span>
            <span style={{ color: colors.primary.main }}>
              {total.toFixed(2)} {currency}
            </span>
          </div>

          {!orderConfirmed ? (
            <>
              <button
                onClick={onCheckout}
                disabled={submitting}
                style={{
                  width: '100%',
                  padding: '16px',
                  backgroundColor: submitting ? colors.neutral.gray : colors.status.success,
                  color: colors.text.inverse,
                  border: 'none',
                  borderRadius: '10px',
                  fontSize: '18px',
                  fontWeight: '600',
                  cursor: submitting ? 'not-allowed' : 'pointer',
                  marginBottom: '12px',
                  opacity: submitting ? 0.7 : 1,
                  transition: 'all 0.2s ease'
                }}
                onMouseEnter={(e) => {
                  if (!submitting) {
                    e.currentTarget.style.transform = 'translateY(-2px)'
                    e.currentTarget.style.boxShadow = '0 4px 8px rgba(0,0,0,0.15)'
                  }
                }}
                onMouseLeave={(e) => {
                  if (!submitting) {
                    e.currentTarget.style.transform = 'translateY(0)'
                    e.currentTarget.style.boxShadow = 'none'
                  }
                }}
              >
                {submitting ? t('cart.confirming') : t('cart.confirmOrder')}
              </button>

              <button
                onClick={clearCart}
                style={{
                  width: '100%',
                  padding: '12px',
                  backgroundColor: 'transparent',
                  color: colors.status.error,
                  border: `2px solid ${colors.status.error}`,
                  borderRadius: '8px',
                  fontSize: '14px',
                  fontWeight: '500',
                  cursor: 'pointer',
                  transition: 'all 0.2s ease'
                }}
                onMouseEnter={(e) => {
                  e.currentTarget.style.backgroundColor = colors.status.error
                  e.currentTarget.style.color = colors.text.inverse
                }}
                onMouseLeave={(e) => {
                  e.currentTarget.style.backgroundColor = 'transparent'
                  e.currentTarget.style.color = colors.status.error
                }}
              >
                {t('cart.clearCart')}
              </button>
            </>
          ) : (
            <>
              <div style={{
                padding: '14px',
                backgroundColor: colors.orderStatus.ready.bg,
                border: `2px solid ${colors.orderStatus.ready.border}`,
                borderRadius: '8px',
                marginBottom: '12px',
                color: colors.orderStatus.ready.text,
                textAlign: 'center',
                fontWeight: '600'
              }}>
                {t('cart.orderConfirmed')}
              </div>

              {onPayment && (
                <button
                  onClick={onPayment}
                  style={{
                    width: '100%',
                    padding: '16px',
                    backgroundColor: colors.secondary.main,
                    color: colors.text.inverse,
                    border: 'none',
                    borderRadius: '10px',
                    fontSize: '18px',
                    fontWeight: '600',
                    cursor: 'pointer',
                    marginBottom: '12px',
                    transition: 'all 0.2s ease'
                  }}
                  onMouseEnter={(e) => {
                    e.currentTarget.style.backgroundColor = colors.secondary.dark
                    e.currentTarget.style.transform = 'translateY(-2px)'
                  }}
                  onMouseLeave={(e) => {
                    e.currentTarget.style.backgroundColor = colors.secondary.main
                    e.currentTarget.style.transform = 'translateY(0)'
                  }}
                >
                  {t('cart.proceedToPayment')}
                </button>
              )}

              <button
                onClick={onClose}
                style={{
                  width: '100%',
                  padding: '12px',
                  backgroundColor: 'transparent',
                  color: colors.text.secondary,
                  border: `2px solid ${colors.border.main}`,
                  borderRadius: '8px',
                  fontSize: '14px',
                  fontWeight: '500',
                  cursor: 'pointer',
                  transition: 'all 0.2s ease'
                }}
                onMouseEnter={(e) => {
                  e.currentTarget.style.backgroundColor = colors.neutral.lightGray
                }}
                onMouseLeave={(e) => {
                  e.currentTarget.style.backgroundColor = 'transparent'
                }}
              >
                {t('common.close')}
              </button>
            </>
          )}
        </div>
      )}
    </div>
    </>
  )
}
