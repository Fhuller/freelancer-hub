import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import AuthComponent from '../src/components/LoginRegister.vue' // Caminho do componente ajustado

// --- CORREÇÃO AQUI: MOCK USANDO A FÁBRICA vi.fn() ---
// Moca o módulo completo usando o caminho de alias que o componente usa.
vi.mock('@/services/supabase.ts', () => ({
  signUp: vi.fn(),
  signIn: vi.fn(),
  getSession: vi.fn(),
}))

// Importa as funções MOCKADAS para que possamos controlá-las nos testes (mockResolvedValueOnce, etc.)
// O caminho deve ser o mesmo do vi.mock
import { signUp, signIn, getSession } from '@/services/supabase.ts'


// Dados de teste
const testEmail = 'test@example.com'
const testPassword = 'password123'
const mockSession = { user: { id: '123', email: testEmail } }
const mockError = { message: 'Erro simulado' }

describe('AuthComponent', () => {
  // Limpa os mocks antes de cada teste para garantir isolamento
  beforeEach(() => {
    // Garante que o estado dos mocks seja limpo antes de cada 'it'
    vi.clearAllMocks()
    vi.resetAllMocks()
  })

  // --- Testes para handleSignUp ---
  
  describe('handleSignUp', () => {
    it('deve mostrar mensagem de erro se os campos de registro estiverem vazios', async () => {
      const wrapper = mount(AuthComponent)
      
      // Simula a chamada da função
      await wrapper.vm.handleSignUp()

      // Verifica se a mensagem de erro foi exibida
      expect(wrapper.vm.signUpMessage).toBe('Por favor, preencha todos os campos.')
      // Verifica se a função de serviço NÃO foi chamada (agora funciona porque signUp é um vi.fn())
      expect(signUp).not.toHaveBeenCalled()
    })

    it('deve registrar o usuário e mostrar mensagem de sucesso', async () => {
      // Configura o mock para retornar sucesso
      signUp.mockResolvedValueOnce({ data: { user: mockSession.user }, error: null }) // AGORA FUNCIONA
      
      const wrapper = mount(AuthComponent)
      
      // Define os campos de input (como se o usuário tivesse digitado)
      wrapper.vm.signUpEmail = testEmail
      wrapper.vm.signUpPassword = testPassword
      
      // Simula a chamada
      const signUpPromise = wrapper.vm.handleSignUp()
      
      // Verifica o estado de loading (isSigningUp)
      expect(wrapper.vm.isSigningUp).toBe(true)
      
      await signUpPromise
      
      // Verifica se a função de serviço foi chamada corretamente
      expect(signUp).toHaveBeenCalledWith(testEmail, testPassword)
      // Verifica a mensagem de sucesso
      expect(wrapper.vm.signUpMessage).toBe('Usuário registrado! Confira o e-mail para confirmação.')
      // Verifica se os campos foram limpos
      expect(wrapper.vm.signUpEmail).toBe('')
      expect(wrapper.vm.signUpPassword).toBe('')
      // Verifica se o loading terminou
      expect(wrapper.vm.isSigningUp).toBe(false)
    })
    
    it('deve mostrar mensagem de erro da API em caso de falha no registro', async () => {
      // Configura o mock para retornar erro
      signUp.mockResolvedValueOnce({ data: null, error: mockError }) // AGORA FUNCIONA

      const wrapper = mount(AuthComponent)
      wrapper.vm.signUpEmail = testEmail
      wrapper.vm.signUpPassword = testPassword
      
      await wrapper.vm.handleSignUp()
      
      // Verifica a mensagem de erro
      expect(signUp).toHaveBeenCalled()
      expect(wrapper.vm.signUpMessage).toBe(mockError.message)
      // Verifica se o loading terminou
      expect(wrapper.vm.isSigningUp).toBe(false)
    })
    
    it('deve mostrar mensagem de erro inesperado em caso de exceção', async () => {
      // Configura o mock para lançar uma exceção
      signUp.mockRejectedValue(new Error('Network error')) // AGORA FUNCIONA

      const wrapper = mount(AuthComponent)
      wrapper.vm.signUpEmail = testEmail
      wrapper.vm.signUpPassword = testPassword
      
      await wrapper.vm.handleSignUp()
      
      // Verifica a mensagem de erro inesperado
      expect(wrapper.vm.signUpMessage).toBe('Erro inesperado ao registrar usuário.')
      // Verifica se o loading terminou
      expect(wrapper.vm.isSigningUp).toBe(false)
    })
  })

  // --- Testes para handleSignIn ---

  describe('handleSignIn', () => {
    it('deve mostrar mensagem de erro se os campos de login estiverem vazios', async () => {
      const wrapper = mount(AuthComponent)
      
      await wrapper.vm.handleSignIn()

      expect(wrapper.vm.signInMessage).toBe('Por favor, preencha todos os campos.')
      expect(signIn).not.toHaveBeenCalled() // AGORA FUNCIONA
    })

    it('deve fazer login e mostrar mensagem de sucesso', async () => {
      // Configura o mock para retornar sucesso
      signIn.mockResolvedValueOnce({ data: { session: mockSession }, error: null }) // AGORA FUNCIONA

      const wrapper = mount(AuthComponent)
      
      wrapper.vm.signInEmail = testEmail
      wrapper.vm.signInPassword = testPassword
      
      const signInPromise = wrapper.vm.handleSignIn()
      
      expect(wrapper.vm.isSigningIn).toBe(true)
      
      await signInPromise
      
      expect(signIn).toHaveBeenCalledWith(testEmail, testPassword)
      expect(wrapper.vm.signInMessage).toBe('Login realizado com sucesso!')
      expect(wrapper.vm.signInEmail).toBe('')
      expect(wrapper.vm.signInPassword).toBe('')
      expect(wrapper.vm.isSigningIn).toBe(false)
    })
    
    it('deve mostrar mensagem de erro da API em caso de falha no login', async () => {
      // Configura o mock para retornar erro
      signIn.mockResolvedValueOnce({ data: null, error: mockError }) // AGORA FUNCIONA

      const wrapper = mount(AuthComponent)
      wrapper.vm.signInEmail = testEmail
      wrapper.vm.signInPassword = testPassword
      
      await wrapper.vm.handleSignIn()
      
      expect(signIn).toHaveBeenCalled()
      expect(wrapper.vm.signInMessage).toBe(mockError.message)
      expect(wrapper.vm.isSigningIn).toBe(false)
    })
  })

  // --- Testes para handleGetUser ---

  describe('handleGetUser', () => {
    it('deve mostrar os dados da sessão se o usuário estiver logado', async () => {
      // Configura o mock para retornar uma sessão
      getSession.mockResolvedValueOnce({ data: { session: mockSession } }) // AGORA FUNCIONA

      const wrapper = mount(AuthComponent)
      
      const getUserPromise = wrapper.vm.handleGetUser()
      
      expect(wrapper.vm.isLoadingUser).toBe(true)
      
      await getUserPromise
      
      expect(getSession).toHaveBeenCalled()
      // Espera que a mensagem contenha a sessão formatada (JSON.stringify)
      expect(wrapper.vm.userMessage).toContain(JSON.stringify(mockSession, null, 2))
      expect(wrapper.vm.isLoadingUser).toBe(false)
    })
    
    it('deve mostrar mensagem se não houver usuário logado (session: null)', async () => {
      // Configura o mock para retornar sem sessão
      getSession.mockResolvedValueOnce({ data: { session: null } }) // AGORA FUNCIONA

      const wrapper = mount(AuthComponent)
      
      await wrapper.vm.handleGetUser()
      
      expect(getSession).toHaveBeenCalled()
      expect(wrapper.vm.userMessage).toBe('Nenhum usuário logado.')
      expect(wrapper.vm.isLoadingUser).toBe(false)
    })
    
    it('deve mostrar mensagem de erro em caso de falha na verificação', async () => {
      // Configura o mock para lançar exceção
      getSession.mockRejectedValue(new Error('Erro de conexão')) // AGORA FUNCIONA

      const wrapper = mount(AuthComponent)
      
      await wrapper.vm.handleGetUser()
      
      expect(getSession).toHaveBeenCalled()
      expect(wrapper.vm.userMessage).toBe('Erro ao verificar usuário.')
      expect(wrapper.vm.isLoadingUser).toBe(false)
    })
  })
  
  // --- Testes para clearMessages ---
  
  describe('clearMessages', () => {
    it('deve limpar todas as mensagens', () => {
      const wrapper = mount(AuthComponent)
      
      // Define mensagens iniciais
      wrapper.vm.signUpMessage = 'Alguma coisa'
      wrapper.vm.signInMessage = 'Outra coisa'
      wrapper.vm.userMessage = 'E mais uma coisa'
      
      // Chama a função
      wrapper.vm.clearMessages()
      
      // Verifica se todas as mensagens estão vazias
      expect(wrapper.vm.signUpMessage).toBe('')
      expect(wrapper.vm.signInMessage).toBe('')
      expect(wrapper.vm.userMessage).toBe('')
    })
  })
})