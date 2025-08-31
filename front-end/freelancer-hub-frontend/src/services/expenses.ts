import { apiFetch } from './api'

export function fetchExpenses() {
  return apiFetch('/Expense', { method: 'GET' })
}

export function fetchExpenseById(id: string) {
  return apiFetch(`/Expense/${id}`, { method: 'GET' })
}

export function createExpense(data: {
  userId: string
  title: string
  amount: number
  category: string
  paymentDate: string
  notes?: string
}) {
  return apiFetch('/Expense', {
    method: 'POST',
    body: JSON.stringify(data)
  })
}

export function updateExpense(id: string, data: {
  title: string
  amount: number
  category: string
  paymentDate: string
  notes?: string
}) {
  return apiFetch(`/Expense/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data)
  })
}

export function deleteExpense(id: string) {
  return apiFetch(`/Expense/${id}`, { method: 'DELETE' })
}
