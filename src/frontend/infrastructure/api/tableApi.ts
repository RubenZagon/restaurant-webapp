import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

export interface TableSessionResponse {
  sessionId: string
  tableNumber: number
  startedAt: string
}

export const startTableSession = async (
  tableNumber: number
): Promise<TableSessionResponse> => {
  try {
    const response = await apiClient.post<TableSessionResponse>(
      `/api/tables/${tableNumber}/start-session`
    )
    return response.data
  } catch (error) {
    if (axios.isAxiosError(error) && error.response) {
      throw new Error(error.response.data.error || 'Error starting session')
    }
    throw new Error('Connection error with server')
  }
}
