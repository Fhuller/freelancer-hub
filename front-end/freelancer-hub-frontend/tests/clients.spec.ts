import { describe, it, expect, vi, beforeEach } from 'vitest'
import { fetchClients, fetchClientById, createClient, updateClient, deleteClient } from '@/services/clients'
import { apiFetch } from '@/services/api'

vi.mock('@/services/api', () => ({
  apiFetch: vi.fn()
}))

describe('clients service', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('fetchClients chama apiFetch com GET /Client', async () => {
    const fakeData = [{ id: '1', name: 'John', email: 'john@mail.com', notes: '', createdAt: 'now' }]
    ;(apiFetch as any).mockResolvedValue(fakeData)

    const result = await fetchClients()

    expect(apiFetch).toHaveBeenCalledWith('/Client', { method: 'GET' })
    expect(result).toBe(fakeData)
  })

  it('fetchClientById chama apiFetch com GET /Client/:id', async () => {
    const fakeData = { id: '1', name: 'John', email: 'john@mail.com', notes: '', createdAt: 'now' }
    ;(apiFetch as any).mockResolvedValue(fakeData)

    const result = await fetchClientById('1')

    expect(apiFetch).toHaveBeenCalledWith('/Client/1', { method: 'GET' })
    expect(result).toBe(fakeData)
  })

  it('createClient chama apiFetch com POST /Client e body JSON', async () => {
    const newClient = { name: 'Ana', email: 'ana@mail.com', notes: 'VIP' }
    const fakeResponse = { id: '2', ...newClient, createdAt: 'now' }
    ;(apiFetch as any).mockResolvedValue(fakeResponse)

    const result = await createClient(newClient)

    expect(apiFetch).toHaveBeenCalledWith('/Client', {
      method: 'POST',
      body: JSON.stringify(newClient)
    })
    expect(result).toBe(fakeResponse)
  })

  it('updateClient chama apiFetch com PUT /Client/:id e body JSON', async () => {
    ;(apiFetch as any).mockResolvedValue(undefined)

    await updateClient('1', { name: 'Updated' })

    expect(apiFetch).toHaveBeenCalledWith('/Client/1', {
      method: 'PUT',
      body: JSON.stringify({ name: 'Updated' })
    })
  })

  it('deleteClient chama apiFetch com DELETE /Client/:id', async () => {
    ;(apiFetch as any).mockResolvedValue(undefined)

    await deleteClient('1')

    expect(apiFetch).toHaveBeenCalledWith('/Client/1', { method: 'DELETE' })
  })
})
