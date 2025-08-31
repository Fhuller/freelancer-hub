import { apiFetch } from './api'

export function fetchPayments() {
  return apiFetch('/Payment', { method: 'GET' })
}

export function fetchPaymentById(id: string) {
  return apiFetch(`/Payment/${id}`, { method: 'GET' })
}

export function createPayment(data: {
  userId: string
  invoiceId: string
  amount: number
  paymentDate: string
  paymentMethod: string
  notes?: string
}) {
  return apiFetch('/Payment', {
    method: 'POST',
    body: JSON.stringify(data)
  })
}

export function updatePayment(id: string, data: {
  amount: number
  paymentDate: string
  paymentMethod: string
  notes?: string
}) {
  return apiFetch(`/Payment/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data)
  })
}

export function deletePayment(id: string) {
  return apiFetch(`/Payment/${id}`, { method: 'DELETE' })
}
