import { useState } from 'react'

interface PaymentModalProps {
  isOpen: boolean
  onClose: () => void
  onConfirmPayment: (paymentMethod: string) => Promise<void>
  totalAmount: number
  currency: string
  isProcessing: boolean
}

export function PaymentModal({
  isOpen,
  onClose,
  onConfirmPayment,
  totalAmount,
  currency,
  isProcessing
}: PaymentModalProps) {
  const [selectedMethod, setSelectedMethod] = useState<string>('credit_card')

  if (!isOpen) return null

  const handleConfirm = async () => {
    await onConfirmPayment(selectedMethod)
  }

  const paymentMethods = [
    { id: 'credit_card', name: 'Credit Card', icon: '💳' },
    { id: 'debit_card', name: 'Debit Card', icon: '💳' },
    { id: 'cash', name: 'Cash', icon: '💵' }
  ]

  return (
    <div style={{
      position: 'fixed',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(0, 0, 0, 0.5)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      zIndex: 1000
    }}>
      <div style={{
        backgroundColor: 'white',
        borderRadius: '12px',
        padding: '24px',
        maxWidth: '500px',
        width: '90%',
        boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)'
      }}>
        {/* Header */}
        <div style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: '24px'
        }}>
          <h2 style={{ margin: 0, fontSize: '24px', fontWeight: 'bold' }}>
            Payment
          </h2>
          <button
            onClick={onClose}
            disabled={isProcessing}
            style={{
              background: 'none',
              border: 'none',
              fontSize: '24px',
              cursor: isProcessing ? 'not-allowed' : 'pointer',
              opacity: isProcessing ? 0.5 : 1
            }}
          >
            ×
          </button>
        </div>

        {/* Total Amount */}
        <div style={{
          backgroundColor: '#f5f5f5',
          padding: '16px',
          borderRadius: '8px',
          marginBottom: '24px',
          textAlign: 'center'
        }}>
          <div style={{ fontSize: '14px', color: '#666', marginBottom: '8px' }}>
            Total Amount
          </div>
          <div style={{ fontSize: '32px', fontWeight: 'bold', color: '#333' }}>
            {totalAmount.toFixed(2)} {currency}
          </div>
        </div>

        {/* Payment Methods */}
        <div style={{ marginBottom: '24px' }}>
          <div style={{
            fontSize: '16px',
            fontWeight: '600',
            marginBottom: '12px',
            color: '#333'
          }}>
            Select Payment Method
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
            {paymentMethods.map((method) => (
              <label
                key={method.id}
                style={{
                  display: 'flex',
                  alignItems: 'center',
                  padding: '16px',
                  border: selectedMethod === method.id ? '2px solid #007bff' : '2px solid #e0e0e0',
                  borderRadius: '8px',
                  cursor: isProcessing ? 'not-allowed' : 'pointer',
                  backgroundColor: selectedMethod === method.id ? '#f0f8ff' : 'white',
                  transition: 'all 0.2s',
                  opacity: isProcessing ? 0.6 : 1
                }}
              >
                <input
                  type="radio"
                  name="paymentMethod"
                  value={method.id}
                  checked={selectedMethod === method.id}
                  onChange={(e) => setSelectedMethod(e.target.value)}
                  disabled={isProcessing}
                  style={{ marginRight: '12px' }}
                />
                <span style={{ fontSize: '24px', marginRight: '12px' }}>
                  {method.icon}
                </span>
                <span style={{ fontSize: '16px', fontWeight: '500', color: '#333' }}>
                  {method.name}
                </span>
              </label>
            ))}
          </div>
        </div>

        {/* Action Buttons */}
        <div style={{
          display: 'flex',
          gap: '12px',
          justifyContent: 'flex-end'
        }}>
          <button
            onClick={onClose}
            disabled={isProcessing}
            style={{
              padding: '12px 24px',
              border: '1px solid #ddd',
              borderRadius: '6px',
              backgroundColor: 'white',
              cursor: isProcessing ? 'not-allowed' : 'pointer',
              fontSize: '16px',
              fontWeight: '500',
              opacity: isProcessing ? 0.5 : 1
            }}
          >
            Cancel
          </button>
          <button
            onClick={handleConfirm}
            disabled={isProcessing}
            style={{
              padding: '12px 24px',
              border: 'none',
              borderRadius: '6px',
              backgroundColor: isProcessing ? '#ccc' : '#28a745',
              color: 'white',
              cursor: isProcessing ? 'not-allowed' : 'pointer',
              fontSize: '16px',
              fontWeight: '500',
              transition: 'background-color 0.2s'
            }}
          >
            {isProcessing ? 'Processing...' : `Pay ${totalAmount.toFixed(2)} ${currency}`}
          </button>
        </div>

        {/* Processing Indicator */}
        {isProcessing && (
          <div style={{
            marginTop: '16px',
            padding: '12px',
            backgroundColor: '#fff3cd',
            borderRadius: '6px',
            textAlign: 'center',
            color: '#856404'
          }}>
            Processing your payment, please wait...
          </div>
        )}
      </div>
    </div>
  )
}
