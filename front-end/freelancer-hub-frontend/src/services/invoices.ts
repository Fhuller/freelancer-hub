import { apiFetch } from './api'

export function fetchInvoices() {
  return apiFetch('/Invoice', { method: 'GET' })
}

export function fetchInvoiceById(id: string) {
  return apiFetch(`/Invoice/${id}`, { method: 'GET' })
}

export function createInvoice(data: {
  userId: string
  clientId: string
  projectId: string
  issueDate: string
  dueDate: string
  amount: number
  status: string
  pdfUrl?: string
}) {
  return apiFetch('/Invoice', {
    method: 'POST',
    body: JSON.stringify(data)
  })
}

export function updateInvoice(id: string, data: {
  issueDate: string
  dueDate: string
  amount: number
  status: string
  pdfUrl?: string
}) {
  return apiFetch(`/Invoice/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data)
  })
}

export function deleteInvoice(id: string) {
  return apiFetch(`/Invoice/${id}`, { method: 'DELETE' })
}
