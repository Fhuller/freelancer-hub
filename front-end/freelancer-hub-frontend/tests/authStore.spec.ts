import { describe, it, expect, beforeEach, vi, afterEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '@/stores/auth'
import * as supabaseService from '@/services/supabase'
import * as usersService from '@/services/users'
import { createClient } from '@supabase/supabase-js'
import router from '@/router'

// Mock dos módulos
vi.mock('@/services/supabase')
vi.mock('@/services/users')
vi.mock('@supabase/supabase-js')
vi.mock('@/router')

vi.mock('vue-toast-notification', () => ({
  useToast: () => ({
    success: vi.fn(),
    error: vi.fn(),
    info: vi.fn()
  })
}))

vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    locale: { value: 'pt' }
  })
}))

describe('useAuthStore', () => {
  let store: ReturnType<typeof useAuthStore>
  let mockSupabase: any

  beforeEach(() => {
    // Mock do cliente Supabase ANTES de criar o store
    mockSupabase = {
      auth: {
        signInWithOAuth: vi.fn(),
        resetPasswordForEmail: vi.fn(),
        setSession: vi.fn(),
        signOut: vi.fn()
      }
    }
    
    vi.mocked(createClient).mockReturnValue(mockSupabase as any)
    
    setActivePinia(createPinia())
    
    vi.spyOn(supabaseService, 'signIn').mockImplementation(vi.fn())
    vi.spyOn(supabaseService, 'signUp').mockImplementation(vi.fn())
    vi.spyOn(supabaseService, 'signOut').mockImplementation(vi.fn())
    vi.spyOn(supabaseService, 'getSession').mockImplementation(vi.fn())
    
    vi.spyOn(usersService, 'createUser').mockImplementation(vi.fn())
    vi.spyOn(usersService, 'fetchCurrentUser').mockImplementation(vi.fn())
    vi.spyOn(usersService, 'updateUserLanguage').mockImplementation(vi.fn())
    
    vi.mocked(router.push).mockImplementation(vi.fn())

    // Criar store DEPOIS de configurar o mock do createClient
    store = useAuthStore()

    // Limpar todas as chamadas dos mocks
    vi.clearAllMocks()
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  describe('Estado inicial', () => {
    it('deve inicializar com valores padrão', () => {
      expect(store.session).toBeNull()
      expect(store.user).toBeNull()
      expect(store.isLoading).toBe(false)
      expect(store.error).toBe('')
      expect(store.isAuthenticated).toBe(false)
      expect(store.accessToken).toBeUndefined()
    })
  })

  describe('login', () => {
    it('deve fazer login com sucesso', async () => {
      const mockSession = {
        access_token: 'mock-token',
        refresh_token: 'mock-refresh',
        user: { email: 'test@example.com' }
      }

      vi.spyOn(supabaseService, 'signIn').mockResolvedValue({
        data: { session: mockSession },
        error: null
      } as any)

      vi.spyOn(usersService, 'createUser').mockResolvedValue({} as any)
      vi.spyOn(usersService, 'fetchCurrentUser').mockResolvedValue({
        id: '1',
        name: 'Test User',
        email: 'test@example.com',
        language: 'pt'
      })

      const result = await store.login('test@example.com', 'password123')

      expect(result).toBe(true)
      expect(store.session).toEqual(mockSession)
      expect(store.isLoading).toBe(false)
      expect(store.error).toBe('')
      expect(supabaseService.signIn).toHaveBeenCalledWith('test@example.com', 'password123')
    })

    it('deve tratar erro de autenticação', async () => {
      vi.spyOn(supabaseService, 'signIn').mockResolvedValue({
        data: { session: null },
        error: { message: 'Credenciais inválidas' }
      } as any)

      const result = await store.login('test@example.com', 'wrong-password')

      expect(result).toBe(false)
      expect(store.error).toBe('Credenciais inválidas')
      expect(store.session).toBeNull()
      expect(store.isLoading).toBe(false)
    })

    it('deve tratar erro inesperado', async () => {
      vi.spyOn(supabaseService, 'signIn').mockRejectedValue(
        new Error('Network error')
      )

      const result = await store.login('test@example.com', 'password123')

      expect(result).toBe(false)
      expect(store.error).toBe('Erro inesperado ao fazer login')
      expect(store.isLoading).toBe(false)
    })
  })

  describe('register', () => {
    it('deve registrar usuário com sucesso', async () => {
      vi.spyOn(supabaseService, 'signUp').mockResolvedValue({
        data: { user: { email: 'new@example.com' } },
        error: null
      } as any)

      const result = await store.register('new@example.com', 'password123')

      expect(result).toBe(true)
      expect(store.isLoading).toBe(false)
      expect(store.error).toBe('')
      expect(supabaseService.signUp).toHaveBeenCalledWith('new@example.com', 'password123')
    })

    it('deve tratar erro no registro', async () => {
      vi.spyOn(supabaseService, 'signUp').mockResolvedValue({
        data: { user: null },
        error: { message: 'Email já cadastrado' }
      } as any)

      const result = await store.register('existing@example.com', 'password123')

      expect(result).toBe(false)
      expect(store.error).toBe('Email já cadastrado')
      expect(store.isLoading).toBe(false)
    })
  })

  describe('logout', () => {
    it('deve fazer logout com sucesso', async () => {
      store.session = {
        access_token: 'mock-token',
        refresh_token: 'mock-refresh'
      } as any
      store.user = {
        id: '1',
        name: 'Test',
        email: 'test@example.com',
        language: 'pt'
      }

      mockSupabase.auth.signOut.mockResolvedValue({})

      await store.logout()

      expect(store.session).toBeNull()
      expect(store.user).toBeNull()
      expect(mockSupabase.auth.signOut).toHaveBeenCalled()
      expect(router.push).toHaveBeenCalledWith('/login')
    })

    it('deve tratar erro no logout', async () => {
      store.session = { access_token: 'token' } as any
      mockSupabase.auth.signOut.mockRejectedValue(new Error('Logout error'))

      await store.logout()

      // Deve continuar executando mesmo com erro
      expect(mockSupabase.auth.signOut).toHaveBeenCalled()
    })
  })

  describe('checkAuth', () => {
    it('deve verificar sessão existente', async () => {
      const mockSession = {
        access_token: 'mock-token',
        refresh_token: 'mock-refresh',
        user: { email: 'test@example.com' }
      }

      vi.spyOn(supabaseService, 'getSession').mockResolvedValue({
        data: { session: mockSession }
      } as any)

      vi.spyOn(usersService, 'createUser').mockResolvedValue({} as any)
      vi.spyOn(usersService, 'fetchCurrentUser').mockResolvedValue({
        id: '1',
        name: 'Test User',
        email: 'test@example.com',
        language: 'pt'
      })

      await store.checkAuth()

      expect(store.session).toEqual(mockSession)
      expect(usersService.fetchCurrentUser).toHaveBeenCalled()
    })

    it('deve limpar sessão se não houver usuário autenticado', async () => {
      vi.spyOn(supabaseService, 'getSession').mockResolvedValue({
        data: { session: null }
      } as any)

      await store.checkAuth()

      expect(store.session).toBeNull()
      expect(store.user).toBeNull()
    })

    it('deve tratar erro na verificação', async () => {
      vi.spyOn(supabaseService, 'getSession').mockRejectedValue(
        new Error('Session error')
      )

      await store.checkAuth()

      expect(store.session).toBeNull()
      expect(store.user).toBeNull()
    })
  })

  describe('loginWithGoogle', () => {
    it('deve iniciar login com Google', async () => {
      mockSupabase.auth.signInWithOAuth.mockResolvedValue({ error: null })

      const result = await store.loginWithGoogle()

      expect(mockSupabase.auth.signInWithOAuth).toHaveBeenCalledWith({
        provider: 'google',
        options: {
          redirectTo: expect.stringContaining('/app/dashboard')
        }
      })
      expect(store.isLoading).toBe(false)
      expect(result).toBeUndefined() // A função não retorna nada no caso de sucesso
    })

    it('deve tratar erro no login com Google', async () => {
      mockSupabase.auth.signInWithOAuth.mockResolvedValue({
        error: { message: 'OAuth error' }
      })

      const result = await store.loginWithGoogle()

      expect(result).toBe(false)
      expect(store.error).toBe('OAuth error')
      expect(store.isLoading).toBe(false)
    })
  })

  describe('resetPassword', () => {
    it('deve solicitar redefinição de senha com sucesso', async () => {
      mockSupabase.auth.resetPasswordForEmail.mockResolvedValue({ error: null })

      const result = await store.resetPassword('test@example.com')

      expect(result).toBe(true)
      expect(mockSupabase.auth.resetPasswordForEmail).toHaveBeenCalledWith(
        'test@example.com',
        { redirectTo: expect.stringContaining('/reset-password') }
      )
      expect(store.isLoading).toBe(false)
    })

    it('deve tratar erro na redefinição de senha', async () => {
      mockSupabase.auth.resetPasswordForEmail.mockResolvedValue({
        error: { message: 'Email não encontrado' }
      })

      const result = await store.resetPassword('invalid@example.com')

      expect(result).toBe(false)
      expect(store.error).toBe('Email não encontrado')
      expect(store.isLoading).toBe(false)
    })
  })

  describe('setLanguage', () => {
    it('deve atualizar idioma do usuário', async () => {
      store.user = {
        id: '1',
        name: 'Test',
        email: 'test@example.com',
        language: 'pt'
      }

      vi.spyOn(usersService, 'updateUserLanguage').mockResolvedValue({} as any)

      await store.setLanguage('en')

      expect(usersService.updateUserLanguage).toHaveBeenCalledWith('1', 'en')
      expect(store.user.language).toBe('en')
    })

    it('deve tratar erro ao atualizar idioma', async () => {
      store.user = {
        id: '1',
        name: 'Test',
        email: 'test@example.com',
        language: 'pt'
      }

      vi.spyOn(usersService, 'updateUserLanguage').mockRejectedValue(
        new Error('Update error')
      )

      await store.setLanguage('en')

      // Não deve lançar erro, apenas logar
      expect(usersService.updateUserLanguage).toHaveBeenCalled()
    })
  })

  describe('loadCurrentUser', () => {
    it('deve carregar usuário atual', async () => {
      const mockUser = {
        id: '1',
        name: 'Test User',
        email: 'test@example.com',
        language: 'en'
      }

      vi.spyOn(usersService, 'fetchCurrentUser').mockResolvedValue(mockUser)

      await store.loadCurrentUser()

      expect(store.user).toEqual(mockUser)
    })

    it('deve tratar erro ao carregar usuário', async () => {
      vi.spyOn(usersService, 'fetchCurrentUser').mockRejectedValue(
        new Error('Fetch error')
      )

      await store.loadCurrentUser()

      // Não deve lançar erro, apenas logar
      expect(usersService.fetchCurrentUser).toHaveBeenCalled()
    })
  })

  describe('clearError', () => {
    it('deve limpar mensagem de erro', () => {
      store.error = 'Algum erro'

      store.clearError()

      expect(store.error).toBe('')
    })
  })

  describe('Computed properties', () => {
    it('isAuthenticated deve retornar true quando há sessão', () => {
      store.session = { access_token: 'token' } as any

      expect(store.isAuthenticated).toBe(true)
    })

    it('isAuthenticated deve retornar false quando não há sessão', () => {
      store.session = null

      expect(store.isAuthenticated).toBe(false)
    })

    it('accessToken deve retornar o token da sessão', () => {
      store.session = { access_token: 'my-token' } as any

      expect(store.accessToken).toBe('my-token')
    })

    it('accessToken deve retornar undefined quando não há sessão', () => {
      store.session = null

      expect(store.accessToken).toBeUndefined()
    })
  })
})