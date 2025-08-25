import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { signUp, signIn, signOut, getSession } from '../services/supabase'
import type { User, Session } from '@supabase/supabase-js'

export const useAuthStore = defineStore('auth', () => {
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
        return false
      }
      
      session.value = data.session
      return true
    } catch (err) {
      error.value = 'Erro inesperado ao fazer login'
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
        return false
      }
      
      return true
    } catch (err) {
      error.value = 'Erro inesperado ao registrar'
      return false
    } finally {
      isLoading.value = false
    }
  }

  const logout = async () => {
    try {
      await signOut()
      session.value = null
    } catch (err) {
      console.error('Erro ao fazer logout:', err)
    }
  }

  const checkAuth = async () => {
    try {
      const { data: { session: currentSession } } = await getSession()

      session.value = currentSession
    } catch (err) {
      console.error('Erro ao verificar autenticação:', err)
      session.value = null
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