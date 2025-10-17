import { Product } from '@infrastructure/api/productsApi'
import { useCartStore } from '../../src/store/cartStore'

interface ProductCardProps {
  product: Product
}

function ProductCard({ product }: ProductCardProps) {
  const addItem = useCartStore(state => state.addItem)

  const handleAddToCart = () => {
    addItem({
      productId: product.id,
      name: product.name,
      description: product.description || '',
      price: product.price,
      currency: product.currency
    }, 1)
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
        style={{
          width: '100%',
          padding: '12px',
          marginTop: '12px',
          backgroundColor: '#27ae60',
          color: 'white',
          border: 'none',
          borderRadius: '6px',
          fontSize: '16px',
          fontWeight: 'bold',
          cursor: 'pointer',
          transition: 'background-color 0.2s'
        }}
        onMouseEnter={(e) => {
          e.currentTarget.style.backgroundColor = '#229954'
        }}
        onMouseLeave={(e) => {
          e.currentTarget.style.backgroundColor = '#27ae60'
        }}
      >
        Add to Cart
      </button>
    </div>
  )
}

export default ProductCard
