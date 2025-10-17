import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

export interface Category {
  id: string
  name: string
  description: string | null
  isActive: boolean
}

export interface Product {
  id: string
  name: string
  description: string | null
  price: number
  currency: string
  categoryId: string
  allergens: string[]
  isAvailable: boolean
}

export const getAllCategories = async (): Promise<Category[]> => {
  try {
    const response = await apiClient.get<Category[]>('/api/categories')
    return response.data
  } catch (error) {
    if (axios.isAxiosError(error) && error.response) {
      throw new Error(error.response.data.error || 'Error fetching categories')
    }
    throw new Error('Connection error with server')
  }
}

export const getProductsByCategory = async (
  categoryId: string
): Promise<Product[]> => {
  try {
    const response = await apiClient.get<Product[]>(
      `/api/products/category/${categoryId}`
    )
    return response.data
  } catch (error) {
    if (axios.isAxiosError(error) && error.response) {
      throw new Error(error.response.data.error || 'Error fetching products')
    }
    throw new Error('Connection error with server')
  }
}
