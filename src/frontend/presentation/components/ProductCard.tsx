import { useState } from 'react'
import { Product } from '@infrastructure/api/productsApi'
import { useCartStore } from '../../src/store/cartStore'
import { addProductToOrder } from '@infrastructure/api/ordersApi'
import { AllergenList } from '../../src/presentation/components/AllergenIcons'
import { colors } from '../../src/theme/colors'

interface ProductCardProps {
  product: Product
}

function ProductCard({ product }: ProductCardProps) {
  const addItem = useCartStore(state => state.addItem)
  const orderId = useCartStore(state => state.orderId)
  const [adding, setAdding] = useState(false)

  const handleAddToCart = async () => {
    if (!orderId) {
      console.error('No order ID available')
      return
    }

    try {
      setAdding(true)

      // Add product to backend order
      await addProductToOrder(orderId, {
        productId: product.id,
        quantity: 1
      })

      // Add to local cart state for UI
      addItem({
        productId: product.id,
        name: product.name,
        description: product.description || '',
        price: product.price,
        currency: product.currency
      }, 1)
    } catch (error) {
      console.error('Error adding product to order:', error)
      alert('Failed to add product to cart. Please try again.')
    } finally {
      setAdding(false)
    }
  }

  // Generate placeholder image URL based on product name
  const getProductImageUrl = (productName: string): string => {
    // Use a placeholder service with product name as seed
    const seed = encodeURIComponent(productName)
    return `https://ui-avatars.com/api/?name=${seed}&size=200&background=8B4513&color=FFFBF0&bold=true`
  }

  return (
    <div style={{
      border: `2px solid ${colors.border.light}`,
      borderRadius: '12px',
      padding: '0',
      marginBottom: '16px',
      backgroundColor: colors.background.paper,
      boxShadow: '0 4px 6px rgba(0,0,0,0.08)',
      overflow: 'hidden',
      transition: 'all 0.3s ease'
    }}>
      {/* Product Image */}
      <div style={{
        width: '100%',
        height: '180px',
        backgroundColor: colors.neutral.lightGray,
        overflow: 'hidden',
        position: 'relative'
      }}>
        <img
          src={getProductImageUrl(product.name)}
          alt={product.name}
          style={{
            width: '100%',
            height: '100%',
            objectFit: 'cover'
          }}
          onError={(e) => {
            // Fallback if image fails to load
            const target = e.target as HTMLImageElement
            target.style.display = 'none'
            if (target.parentElement) {
              target.parentElement.style.display = 'flex'
              target.parentElement.style.alignItems = 'center'
              target.parentElement.style.justifyContent = 'center'
              target.parentElement.innerHTML = `<span style="color: ${colors.text.secondary}; font-size: 48px;">🍽️</span>`
            }
          }}
        />
      </div>

      {/* Product Content */}
      <div style={{ padding: '16px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '12px' }}>
          <div style={{ flex: 1 }}>
            <h3 style={{
              margin: '0 0 8px 0',
              fontSize: '1.25em',
              fontWeight: '600',
              color: colors.text.primary
            }}>
              {product.name}
            </h3>
            {product.description && (
              <p style={{
                margin: '0 0 12px 0',
                color: colors.text.secondary,
                fontSize: '0.9em',
                lineHeight: '1.5'
              }}>
                {product.description}
              </p>
            )}
          </div>
          <div style={{
            fontSize: '1.4em',
            fontWeight: 'bold',
            color: colors.primary.main,
            marginLeft: '16px',
            whiteSpace: 'nowrap'
          }}>
            {product.price.toFixed(2)} {product.currency}
          </div>
        </div>

        {/* Allergen Icons */}
        {product.allergens && product.allergens.length > 0 && (
          <div style={{ marginBottom: '12px' }}>
            <AllergenList allergens={product.allergens} size={20} />
          </div>
        )}

        {/* Add to Cart Button */}
        <button
          onClick={handleAddToCart}
          disabled={adding}
          style={{
            width: '100%',
            padding: '14px',
            marginTop: '8px',
            backgroundColor: adding ? colors.neutral.gray : colors.primary.main,
            color: colors.text.inverse,
            border: 'none',
            borderRadius: '8px',
            fontSize: '16px',
            fontWeight: '600',
            cursor: adding ? 'not-allowed' : 'pointer',
            transition: 'all 0.2s ease',
            opacity: adding ? 0.7 : 1
          }}
          onMouseEnter={(e) => {
            if (!adding) {
              e.currentTarget.style.backgroundColor = colors.primary.light
              e.currentTarget.style.transform = 'translateY(-1px)'
            }
          }}
          onMouseLeave={(e) => {
            if (!adding) {
              e.currentTarget.style.backgroundColor = colors.primary.main
              e.currentTarget.style.transform = 'translateY(0)'
            }
          }}
        >
          {adding ? 'Adding...' : 'Add to Cart'}
        </button>
      </div>
    </div>
  )
}

export default ProductCard
