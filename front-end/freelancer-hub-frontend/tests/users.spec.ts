import { describe, it, expect, vi, beforeEach } from 'vitest'

const { mockApiFetch } = vi.hoisted(() => {
  return {
    mockApiFetch: vi.fn()
  }
})

vi.mock('@/services/api', () => ({
  apiFetch: mockApiFetch
}))

import {
  fetchUsers,
  fetchUserById,
  fetchCurrentUser,
  createUser,
  updateUser,
  deleteUser,
  updateUserLanguage
} from '@/services/users' 

describe('User Service', () => {

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('deve chamar apiFetch para fetchUsers corretamente', async () => {
    const mockResponse = { data: [{ id: 'u1' }, { id: 'u2' }] }
    mockApiFetch.mockResolvedValueOnce(mockResponse)

    const result = await fetchUsers()

    expect(mockApiFetch).toHaveBeenCalledWith(
      '/User',
      { method: 'GET' }
    )
    expect(result).toEqual(mockResponse)
  })

  it('deve chamar apiFetch para fetchUserById corretamente', async () => {
    const mockResponse = { data: { id: 'u123', name: 'Alice' } }
    mockApiFetch.mockResolvedValueOnce(mockResponse)

    const result = await fetchUserById('u123')

    expect(mockApiFetch).toHaveBeenCalledWith(
      '/User/u123',
      { method: 'GET' }
    )
    expect(result).toEqual(mockResponse)
  })

  it('deve chamar apiFetch para fetchCurrentUser corretamente', async () => {
    const mockResponse = { data: { id: 'current', name: 'Me' } }
    mockApiFetch.mockResolvedValueOnce(mockResponse)

    const result = await fetchCurrentUser()

    expect(mockApiFetch).toHaveBeenCalledWith(
      '/User/me',
      { method: 'GET' }
    )
    expect(result).toEqual(mockResponse)
  })

  it('deve chamar apiFetch para createUser corretamente', async () => {
    const userData = { name: 'Bob', email: 'bob@test.com' }
    const mockResponse = { data: { id: 'new-u', ...userData } }
    mockApiFetch.mockResolvedValueOnce(mockResponse)

    const result = await createUser(userData)

    expect(mockApiFetch).toHaveBeenCalledWith(
      '/User',
      {
        method: 'POST',
        body: JSON.stringify(userData)
      }
    )
    expect(result).toEqual(mockResponse)
  })

  it('deve chamar apiFetch para updateUser corretamente', async () => {
    const updateData = { name: 'Charlie', email: 'charlie@test.com' }
    const mockResponse = { data: { id: 'u456', ...updateData } }
    mockApiFetch.mockResolvedValueOnce(mockResponse)

    const result = await updateUser('u456', updateData)

    expect(mockApiFetch).toHaveBeenCalledWith(
      '/User/u456',
      {
        method: 'PUT',
        body: JSON.stringify(updateData)
      }
    )
    expect(result).toEqual(mockResponse)
  })

  it('deve chamar apiFetch para deleteUser corretamente', async () => {
    const mockResponse = { data: null }
    mockApiFetch.mockResolvedValueOnce(mockResponse)

    const result = await deleteUser('u789')

    expect(mockApiFetch).toHaveBeenCalledWith(
      '/User/u789',
      { method: 'DELETE' }
    )
    expect(result).toEqual(mockResponse)
  })

  it('deve chamar apiFetch para updateUserLanguage com PATCH e o corpo correto', async () => {
    const userId = 'u999'
    const language = 'pt-BR'
    const mockResponse = { data: { id: userId, language: language } }
    mockApiFetch.mockResolvedValueOnce(mockResponse)

    const result = await updateUserLanguage(userId, language)

    expect(mockApiFetch).toHaveBeenCalledWith(
      '/User/u999/language',
      {
        method: 'PATCH',
        body: JSON.stringify(language)
      }
    )
    expect(result).toEqual(mockResponse)
  })
})