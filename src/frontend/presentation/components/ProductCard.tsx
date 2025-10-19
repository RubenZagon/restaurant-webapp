import { useState } from 'react'
import { Product } from '@infrastructure/api/productsApi'
import { useCartStore } from '../../src/store/cartStore'
import { addProductToOrder } from '@infrastructure/api/ordersApi'

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

  return (
    <div style={{
      border: '1px solid #ddd',
      borderRadius: '8px',
      padding: '16px',
      marginBottom: '16px',
      backgroundColor: '#fff',
      boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
    }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
        <div style={{ flex: 1 }}>
          <h3 style={{ margin: '0 0 8px 0', fontSize: '1.2em' }}>{product.name}</h3>
          {product.description && (
            <p style={{ margin: '0 0 12px 0', color: '#666', fontSize: '0.9em' }}>
              {product.description}
            </p>
          )}
          {product.allergens && product.allergens.length > 0 && (
            <div style={{ fontSize: '0.85em', color: '#e74c3c', marginBottom: '12px' }}>
              <strong>Allergens:</strong> {product.allergens.join(', ')}
            </div>
          )}
        </div>
        <div style={{
          fontSize: '1.3em',
          fontWeight: 'bold',
          color: '#27ae60',
          marginLeft: '16px'
        }}>
          {product.price.toFixed(2)} {product.currency}
        </div>
      </div>
      <button
        onClick={handleAddToCart}
        disabled={adding}
        style={{
          width: '100%',
          padding: '12px',
          marginTop: '12px',
          backgroundColor: adding ? '#95a5a6' : '#27ae60',
          color: 'white',
          border: 'none',
          borderRadius: '6px',
          fontSize: '16px',
          fontWeight: 'bold',
          cursor: adding ? 'not-allowed' : 'pointer',
          transition: 'background-color 0.2s',
          opacity: adding ? 0.7 : 1
        }}
        onMouseEnter={(e) => {
          if (!adding) {
            e.currentTarget.style.backgroundColor = '#229954'
          }
        }}
        onMouseLeave={(e) => {
          if (!adding) {
            e.currentTarget.style.backgroundColor = '#27ae60'
          }
        }}
      >
        {adding ? 'Adding...' : 'Add to Cart'}
      </button>
    </div>
  )
}

export default ProductCard
