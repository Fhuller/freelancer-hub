import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import { signUp, signIn, signOut, getSession } from '../services/supabase'
import type { Session } from '@supabase/supabase-js'
import router from '@/router'
import { useToast } from 'vue-toast-notification'
import { createUser, fetchCurrentUser, updateUserLanguage } from '../services/users'
import { createClient } from '@supabase/supabase-js'
import { useI18n } from 'vue-i18n'

export const useAuthStore = defineStore('auth', () => {
  const toast = useToast()
  const { locale } = useI18n()
  const session = ref<Session | null>(null)
  const user = ref<{ id: string; name: string; email: string; language: string } | null>(null)
  const isLoading = ref(false)
  const error = ref<string>('')

  const supabaseUrl = import.meta.env.VITE_SUPABASE_URL
  const supabaseAnonKey = import.meta.env.VITE_SUPABASE_ANON_KEY
  const supabase = createClient(supabaseUrl, supabaseAnonKey)

  const isAuthenticated = computed(() => !!session.value)
  const accessToken = computed(() => session.value?.access_token)

  const loadCurrentUser = async () => {
    try {
      const response = await fetchCurrentUser()
      if (response) {
        user.value = response
        if (user.value?.language) {
          locale.value = user.value.language
        }
      }
    } catch (err) {
      console.error('Erro ao carregar usuário:', err)
    }
  }

  const setLanguage = async (lang: string) => {
    locale.value = lang
    if (user.value?.id) {
      try {
        await updateUserLanguage(user.value.id, lang)
        user.value.language = lang
      } catch (err) {
        console.error('Erro ao atualizar idioma no backend:', err)
      }
    }
  }

  const login = async (email: string, password: string) => {
    try {
      isLoading.value = true
      error.value = ''
      const { data, error: authError } = await signIn(email, password)
      if (authError) {
        error.value = authError.message
        toast.error(authError.message)
        return false
      }
      session.value = data.session
      toast.success('Login realizado com sucesso!')

      try {
        await createUser({
          name: email.split('@')[0],
          email: email
        })
      } catch (backendErr: any) {
        console.error('Erro ao criar usuário no backend:', backendErr)
      }

      await loadCurrentUser()
      return true
    } catch (err: any) {
      error.value = 'Erro inesperado ao fazer login'
      toast.error(err?.message || 'Erro inesperado ao fazer login')
      return false
    } finally {
      isLoading.value = false
    }
  }

  const logout = async () => {
    try {
      if (session.value) {
        await supabase.auth.setSession({
          access_token: session.value.access_token,
          refresh_token: session.value.refresh_token
        })
      }

      await supabase.auth.signOut()

      session.value = null
      user.value = null

      locale.value = 'pt'

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
      if (session.value) await loadCurrentUser()
    } catch (err: any) {
      console.error('Erro ao verificar autenticação:', err)
      session.value = null
      user.value = null
    }
  }

  return {
    session,
    user,
    isLoading,
    error,
    isAuthenticated,
    accessToken,
    login,
    logout,
    checkAuth,
    setLanguage,
    loadCurrentUser
  }
})
