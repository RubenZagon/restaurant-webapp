import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export interface PaymentDto {
  id: string
  orderId: string
  amount: number
  currency: string
  status: string
  transactionId?: string
  failureReason?: string
  createdAt: string
  processedAt?: string
}

export interface ProcessPaymentRequest {
  paymentMethod: string
}

export interface ProcessPaymentResponse {
  success: boolean
  data?: PaymentDto
  error?: string
}

/**
 * Process a payment for an order
 */
export async function processPayment(
  orderId: string,
  paymentMethod: string
): Promise<ProcessPaymentResponse> {
  try {
    const response = await axios.post<ProcessPaymentResponse>(
      `${API_BASE_URL}/api/payments/orders/${orderId}`,
      { paymentMethod }
    )

    return response.data
  } catch (error) {
    if (axios.isAxiosError(error) && error.response) {
      return {
        success: false,
        error: error.response.data.error || 'Payment processing failed'
      }
    }
    throw error
  }
}
