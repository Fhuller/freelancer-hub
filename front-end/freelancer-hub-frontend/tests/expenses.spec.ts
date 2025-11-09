import { describe, it, expect, vi, beforeEach } from 'vitest'
import {
  fetchExpenses,
  fetchExpenseById,
  createExpense,
  updateExpense,
  deleteExpense
} from '@/services/expenses'
import { apiFetch } from '@/services/api'

vi.mock('@/services/api', () => ({
  apiFetch: vi.fn()
}))

describe('expenses service', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('fetchExpenses chama apiFetch com GET /Expense', async () => {
    const fakeData = [{ id: '1', title: 'Conta de luz', amount: 120 }]
    ;(apiFetch as any).mockResolvedValue(fakeData)

    const result = await fetchExpenses()

    expect(apiFetch).toHaveBeenCalledWith('/Expense', { method: 'GET' })
    expect(result).toBe(fakeData)
  })

  it('fetchExpenseById chama apiFetch com GET /Expense/:id', async () => {
    const fakeExpense = { id: '1', title: 'Conta de luz', amount: 120 }
    ;(apiFetch as any).mockResolvedValue(fakeExpense)

    const result = await fetchExpenseById('1')

    expect(apiFetch).toHaveBeenCalledWith('/Expense/1', { method: 'GET' })
    expect(result).toBe(fakeExpense)
  })

  it('createExpense chama apiFetch com POST /Expense e body JSON', async () => {
    const newExpense = {
      userId: 'u1',
      title: 'Internet',
      amount: 150,
      category: 'Serviços',
      paymentDate: '2025-11-09'
    }
    const fakeResponse = { id: '2', ...newExpense }
    ;(apiFetch as any).mockResolvedValue(fakeResponse)

    const result = await createExpense(newExpense)

    expect(apiFetch).toHaveBeenCalledWith('/Expense', {
      method: 'POST',
      body: JSON.stringify(newExpense)
    })
    expect(result).toBe(fakeResponse)
  })

  it('updateExpense chama apiFetch com PUT /Expense/:id e body JSON', async () => {
    ;(apiFetch as any).mockResolvedValue(undefined)
    const updateData = {
      title: 'Internet',
      amount: 200,
      category: 'Serviços',
      paymentDate: '2025-11-09'
    }

    await updateExpense('1', updateData)

    expect(apiFetch).toHaveBeenCalledWith('/Expense/1', {
      method: 'PUT',
      body: JSON.stringify(updateData)
    })
  })

  it('deleteExpense chama apiFetch com DELETE /Expense/:id', async () => {
    ;(apiFetch as any).mockResolvedValue(undefined)

    await deleteExpense('1')

    expect(apiFetch).toHaveBeenCalledWith('/Expense/1', { method: 'DELETE' })
  })
})
