import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export interface OrderLine {
  id: string
  productId: string
  productName: string
  unitPrice: number
  quantity: number
  subtotal: number
}

export interface Order {
  id: string
  tableNumber: number
  sessionId: string
  lines: OrderLine[]
  status: string
  total: number
  currency: string
  createdAt: string
  confirmedAt: string | null
}

export interface AddProductToOrderRequest {
  productId: string
  quantity: number
}

export const getOrCreateOrderForTable = async (tableNumber: number): Promise<Order> => {
  const response = await axios.get<Order>(`${API_BASE_URL}/orders/table/${tableNumber}`)
  return response.data
}

export const addProductToOrder = async (
  orderId: string,
  request: AddProductToOrderRequest
): Promise<Order> => {
  const response = await axios.post<Order>(
    `${API_BASE_URL}/orders/${orderId}/products`,
    request
  )
  return response.data
}

export const confirmOrder = async (orderId: string): Promise<Order> => {
  const response = await axios.post<Order>(`${API_BASE_URL}/orders/${orderId}/confirm`)
  return response.data
}
