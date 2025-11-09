import { describe, it, expect, vi, beforeEach } from 'vitest'
import {
  fetchInvoices,
  fetchInvoiceById,
  createInvoice,
  updateInvoice,
  deleteInvoice
} from '@/services/invoices'
import { apiFetch } from '@/services/api'

vi.mock('@/services/api', () => ({
  apiFetch: vi.fn()
}))

describe('invoice service', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('fetchInvoices chama apiFetch com GET /Invoice', async () => {
    const fakeData = [{ id: '1', amount: 500, status: 'paid' }]
    ;(apiFetch as any).mockResolvedValue(fakeData)

    const result = await fetchInvoices()

    expect(apiFetch).toHaveBeenCalledWith('/Invoice', { method: 'GET' })
    expect(result).toBe(fakeData)
  })

  it('fetchInvoiceById chama apiFetch com GET /Invoice/:id', async () => {
    const fakeInvoice = { id: '1', amount: 800, status: 'pending' }
    ;(apiFetch as any).mockResolvedValue(fakeInvoice)

    const result = await fetchInvoiceById('1')

    expect(apiFetch).toHaveBeenCalledWith('/Invoice/1', { method: 'GET' })
    expect(result).toBe(fakeInvoice)
  })

  it('createInvoice chama apiFetch com POST /Invoice e body JSON', async () => {
    const newInvoice = {
      userId: 'u1',
      clientId: 'c1',
      projectId: 'p1',
      issueDate: '2025-11-09',
      dueDate: '2025-12-09',
      amount: 1000,
      status: 'pending',
      pdfUrl: 'http://example.com/invoice.pdf'
    }
    const fakeResponse = { id: '2', ...newInvoice }
    ;(apiFetch as any).mockResolvedValue(fakeResponse)

    const result = await createInvoice(newInvoice)

    expect(apiFetch).toHaveBeenCalledWith('/Invoice', {
      method: 'POST',
      body: JSON.stringify(newInvoice)
    })
    expect(result).toBe(fakeResponse)
  })

  it('updateInvoice chama apiFetch com PUT /Invoice/:id e body JSON', async () => {
    ;(apiFetch as any).mockResolvedValue(undefined)
    const updateData = {
      issueDate: '2025-11-09',
      dueDate: '2025-12-09',
      amount: 1200,
      status: 'paid',
      pdfUrl: 'http://example.com/invoice-updated.pdf'
    }

    await updateInvoice('1', updateData)

    expect(apiFetch).toHaveBeenCalledWith('/Invoice/1', {
      method: 'PUT',
      body: JSON.stringify(updateData)
    })
  })

  it('deleteInvoice chama apiFetch com DELETE /Invoice/:id', async () => {
    ;(apiFetch as any).mockResolvedValue(undefined)

    await deleteInvoice('1')

    expect(apiFetch).toHaveBeenCalledWith('/Invoice/1', { method: 'DELETE' })
  })
})
