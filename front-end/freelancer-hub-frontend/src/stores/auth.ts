import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { signUp, signIn, signOut, getSession } from '../services/supabase'
import type { Session } from '@supabase/supabase-js'
import router from '@/router'
import { useToast } from 'vue-toast-notification'

export const useAuthStore = defineStore('auth', () => {
  const toast = useToast()
  const session = ref<Session | null>(null)
  const isLoading = ref(false)
  const error = ref<string>('')

  const isAuthenticated = computed(() => !!session.value)
  const accessToken = computed(() => session.value?.access_token)

  const login = async (email: string, password: string) => {
    try {
      isLoading.value = true
      error.value = ''

      const { data, error: authError } = await signIn(email, password)

      if (authError) {
        error.value = authError.message
        toast.error(authError.message) // toast de erro
        return false
      }

      session.value = data.session
      toast.success('Login realizado com sucesso!')
      return true
    } catch (err: any) {
      error.value = 'Erro inesperado ao fazer login'
      toast.error(err?.message || 'Erro inesperado ao fazer login')
      return false
    } finally {
      isLoading.value = false
    }
  }

  const register = async (email: string, password: string) => {
    try {
      isLoading.value = true
      error.value = ''

      const { data, error: authError } = await signUp(email, password)

      if (authError) {
        error.value = authError.message
        toast.error(authError.message)
        return false
      }

      toast.success('Cadastro realizado com sucesso!')
      return true
    } catch (err: any) {
      error.value = 'Erro inesperado ao registrar'
      toast.error(err?.message || 'Erro inesperado ao registrar')
      return false
    } finally {
      isLoading.value = false
    }
  }

  const logout = async () => {
    try {
      await signOut()
      session.value = null
      router.push('/login')
      toast.info('Você saiu da conta.')
    } catch (err: any) {
      console.error('Erro ao fazer logout:', err)
      toast.error(err?.message || 'Erro ao fazer logout')
    }
  }

  const checkAuth = async () => {
    try {
      const { data: { session: currentSession } } = await getSession()
      session.value = currentSession
    } catch (err: any) {
      console.error('Erro ao verificar autenticação:', err)
      session.value = null
      toast.error('Erro ao verificar autenticação.')
    }
  }

  const clearError = () => {
    error.value = ''
  }

  return {
    session,
    isLoading,
    error,
    isAuthenticated,
    accessToken,
    login,
    register,
    logout,
    checkAuth,
    clearError
  }
})