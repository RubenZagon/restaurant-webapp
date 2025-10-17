import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { startTableSession } from '@infrastructure/api/tableApi'
import { getAllCategories, getProductsByCategory, Category, Product } from '@infrastructure/api/productsApi'
import CategoryTabs from '../components/CategoryTabs'
import ProductCard from '../components/ProductCard'
import { CartIcon } from '../../src/presentation/components/CartIcon'
import { ShoppingCart } from '../../src/presentation/components/ShoppingCart'
import { useCartStore } from '../../src/store/cartStore'

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

  const setTableNumber = useCartStore(state => state.setTableNumber)

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

  const handleCheckout = () => {
    // TODO: Implement checkout logic to confirm order
    alert('Checkout functionality will be implemented in next phase')
  }

  return (
    <>
      <div style={{ padding: '20px', maxWidth: '1200px', margin: '0 auto' }}>
        <header style={{ marginBottom: '24px' }}>
          <h1>Table {tableNumber}</h1>
          {session && (
            <p style={{ marginTop: '8px', opacity: 0.7, fontSize: '0.9em' }}>
              Session: {new Date(session.startedAt).toLocaleTimeString()}
            </p>
          )}
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
      />
    </>
  )
}

export default MenuPage
