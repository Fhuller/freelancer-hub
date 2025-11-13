import { describe, it, expect, vi, beforeEach } from 'vitest'

// 1. Usar vi.hoisted() para garantir que a variável exista antes do mock
// A função retorna um objeto, então desestruturamos para obter o mockAuth
const { mockAuth } = vi.hoisted(() => {
  return {
    mockAuth: {
      signUp: vi.fn(),
      signInWithPassword: vi.fn(),
      signOut: vi.fn(),
      getSession: vi.fn()
    }
  }
})

// 2. Mock do supabase-js (agora pode acessar mockAuth com segurança)
vi.mock('@supabase/supabase-js', () => ({
  createClient: vi.fn(() => ({
    auth: mockAuth // Esta linha agora funciona
  }))
}))

// 3. Agora podemos importar o serviço
// (Graças ao hoisting, esta importação ocorre *após* a execução do mock)
import { signUp, signIn, signOut, getSession } from '@/services/session'

describe('Supabase service', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('deve chamar supabase.auth.signUp corretamente', async () => {
    const mockResponse = { data: { user: { id: '1' } }, error: null }
    mockAuth.signUp.mockResolvedValueOnce(mockResponse)

    const result = await signUp('email@test.com', 'senha123')

    expect(mockAuth.signUp).toHaveBeenCalledWith({
      email: 'email@test.com',
      password: 'senha123'
    })
    expect(result).toEqual(mockResponse)
  })

  it('deve chamar supabase.auth.signInWithPassword corretamente', async () => {
    const mockResponse = { data: { session: { access_token: 'token' } }, error: null }
    mockAuth.signInWithPassword.mockResolvedValueOnce(mockResponse)

    const result = await signIn('email@test.com', 'senha123')

    expect(mockAuth.signInWithPassword).toHaveBeenCalledWith({
      email: 'email@test.com',
      password: 'senha123'
    })
    expect(result).toEqual(mockResponse)
  })

  it('deve chamar supabase.auth.signOut corretamente', async () => {
    const mockResponse = { error: null }
    mockAuth.signOut.mockResolvedValueOnce(mockResponse)

    const result = await signOut()

    expect(mockAuth.signOut).toHaveBeenCalled()
    expect(result).toEqual(mockResponse)
  })

  it('deve chamar supabase.auth.getSession corretamente', async () => {
    const mockResponse = { data: { session: { id: 'sess123' } }, error: null }
    mockAuth.getSession.mockResolvedValueOnce(mockResponse)

    const result = await getSession()

    expect(mockAuth.getSession).toHaveBeenCalled()
    expect(result).toEqual(mockResponse)
  })
})