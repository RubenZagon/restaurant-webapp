import { create } from 'zustand'

export interface CartItem {
  productId: string
  name: string
  description: string
  price: number
  currency: string
  quantity: number
  subtotal: number
}

interface CartStore {
  items: CartItem[]
  tableNumber: number | null
  orderId: string | null

  setTableNumber: (tableNumber: number) => void
  setOrderId: (orderId: string) => void

  addItem: (product: Omit<CartItem, 'quantity' | 'subtotal'>, quantity?: number) => void
  removeItem: (productId: string) => void
  updateQuantity: (productId: string, quantity: number) => void
  clearCart: () => void

  getTotalItems: () => number
  getTotalAmount: () => number
}

export const useCartStore = create<CartStore>((set, get) => ({
  items: [],
  tableNumber: null,
  orderId: null,

  setTableNumber: (tableNumber) => set({ tableNumber }),

  setOrderId: (orderId) => set({ orderId }),

  addItem: (product, quantity = 1) => {
    const existingItem = get().items.find(item => item.productId === product.productId)

    if (existingItem) {
      // If product already exists, increase quantity
      set(state => ({
        items: state.items.map(item =>
          item.productId === product.productId
            ? {
                ...item,
                quantity: item.quantity + quantity,
                subtotal: (item.quantity + quantity) * item.price
              }
            : item
        )
      }))
    } else {
      // Add new product to cart
      const newItem: CartItem = {
        ...product,
        quantity,
        subtotal: product.price * quantity
      }
      set(state => ({ items: [...state.items, newItem] }))
    }
  },

  removeItem: (productId) => {
    set(state => ({
      items: state.items.filter(item => item.productId !== productId)
    }))
  },

  updateQuantity: (productId, quantity) => {
    if (quantity <= 0) {
      get().removeItem(productId)
      return
    }

    if (quantity > 100) {
      console.warn('Maximum quantity is 100')
      return
    }

    set(state => ({
      items: state.items.map(item =>
        item.productId === productId
          ? {
              ...item,
              quantity,
              subtotal: quantity * item.price
            }
          : item
      )
    }))
  },

  clearCart: () => set({ items: [], orderId: null }),

  getTotalItems: () => {
    return get().items.reduce((total, item) => total + item.quantity, 0)
  },

  getTotalAmount: () => {
    return get().items.reduce((total, item) => total + item.subtotal, 0)
  }
}))
