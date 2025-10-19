import { useCartStore } from '../../store/cartStore'

interface CartIconProps {
  onClick: () => void
}

export function CartIcon({ onClick }: CartIconProps) {
  // Subscribe to items array directly so component re-renders when cart changes
  const items = useCartStore(state => state.items)
  const itemCount = items.reduce((total, item) => total + item.quantity, 0)

  return (
    <button
      onClick={onClick}
      style={{
        position: 'fixed',
        top: '20px',
        right: '20px',
        width: '60px',
        height: '60px',
        borderRadius: '50%',
        backgroundColor: '#27ae60',
        border: 'none',
        cursor: 'pointer',
        boxShadow: '0 4px 12px rgba(0, 0, 0, 0.15)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        zIndex: 999,
        transition: 'all 0.3s ease'
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.transform = 'scale(1.1)'
        e.currentTarget.style.backgroundColor = '#229954'
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.transform = 'scale(1)'
        e.currentTarget.style.backgroundColor = '#27ae60'
      }}
    >
      {/* Shopping Cart Icon (SVG) */}
      <svg
        width="28"
        height="28"
        viewBox="0 0 24 24"
        fill="none"
        stroke="white"
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
      >
        <circle cx="9" cy="21" r="1"></circle>
        <circle cx="20" cy="21" r="1"></circle>
        <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
      </svg>

      {/* Item Count Badge */}
      {itemCount > 0 && (
        <div
          style={{
            position: 'absolute',
            top: '-5px',
            right: '-5px',
            backgroundColor: '#e74c3c',
            color: 'white',
            borderRadius: '50%',
            width: '24px',
            height: '24px',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            fontSize: '12px',
            fontWeight: 'bold',
            border: '2px solid white'
          }}
        >
          {itemCount > 99 ? '99+' : itemCount}
        </div>
      )}
    </button>
  )
}
