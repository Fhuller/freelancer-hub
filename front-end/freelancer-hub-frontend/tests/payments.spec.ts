import { describe, it, expect, vi, beforeEach } from 'vitest'
import {
  fetchPayments,
  fetchPaymentById,
  createPayment,
  updatePayment,
  deletePayment
} from '@/services/payments'
import { apiFetch } from '@/services/api'

vi.mock('@/services/api', () => ({
  apiFetch: vi.fn()
}))

describe('payments service', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('fetchPayments chama apiFetch com GET /Payment', async () => {
    const fakeData = [{ id: '1', amount: 200, paymentMethod: 'Pix' }]
    ;(apiFetch as any).mockResolvedValue(fakeData)

    const result = await fetchPayments()

    expect(apiFetch).toHaveBeenCalledWith('/Payment', { method: 'GET' })
    expect(result).toBe(fakeData)
  })

  it('fetchPaymentById chama apiFetch com GET /Payment/:id', async () => {
    const fakePayment = { id: '1', amount: 300, paymentMethod: 'Cartão' }
    ;(apiFetch as any).mockResolvedValue(fakePayment)

    const result = await fetchPaymentById('1')

    expect(apiFetch).toHaveBeenCalledWith('/Payment/1', { method: 'GET' })
    expect(result).toBe(fakePayment)
  })

  it('createPayment chama apiFetch com POST /Payment e body JSON', async () => {
    const newPayment = {
      userId: 'u1',
      invoiceId: 'i1',
      amount: 400,
      paymentDate: '2025-11-09',
      paymentMethod: 'Boleto',
      notes: 'Pago adiantado'
    }
    const fakeResponse = { id: '2', ...newPayment }
    ;(apiFetch as any).mockResolvedValue(fakeResponse)

    const result = await createPayment(newPayment)

    expect(apiFetch).toHaveBeenCalledWith('/Payment', {
      method: 'POST',
      body: JSON.stringify(newPayment)
    })
    expect(result).toBe(fakeResponse)
  })

  it('updatePayment chama apiFetch com PUT /Payment/:id e body JSON', async () => {
    ;(apiFetch as any).mockResolvedValue(undefined)
    const updateData = {
      amount: 450,
      paymentDate: '2025-11-09',
      paymentMethod: 'Transferência',
      notes: 'Valor reajustado'
    }

    await updatePayment('1', updateData)

    expect(apiFetch).toHaveBeenCalledWith('/Payment/1', {
      method: 'PUT',
      body: JSON.stringify(updateData)
    })
  })

  it('deletePayment chama apiFetch com DELETE /Payment/:id', async () => {
    ;(apiFetch as any).mockResolvedValue(undefined)

    await deletePayment('1')

    expect(apiFetch).toHaveBeenCalledWith('/Payment/1', { method: 'DELETE' })
  })
})
