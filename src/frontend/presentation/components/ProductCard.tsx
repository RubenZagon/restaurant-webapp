import { Product } from '@infrastructure/api/productsApi'

interface ProductCardProps {
  product: Product
}

function ProductCard({ product }: ProductCardProps) {
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
            <div style={{ fontSize: '0.85em', color: '#e74c3c' }}>
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
    </div>
  )
}

export default ProductCard
