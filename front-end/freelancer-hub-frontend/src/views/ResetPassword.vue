<template>
  <div class="login-container">
    <div class="login-card">
      <div class="logo-section">
        <img alt="Freelancer Icon" src="@/assets/logo_icon.png"/>
        <h1>Redefinir Senha</h1>
        <p v-if="!linkExpired">Digite sua nova senha abaixo</p>
      </div>

      <div class="login-form">
        <!-- Erro de link expirado -->
        <div v-if="linkExpired" class="error-message">
          <h3>Link Expirado</h3>
          <p>O link para redefinição de senha expirou ou é inválido. Solicite um novo link.</p>
          <button @click="goToLogin" class="link-btn">
            Voltar para Login
          </button>
        </div>

        <!-- Formulário de redefinição -->
        <form v-else @submit.prevent="handleSubmit" class="form">
          <div class="form-group">
            <label for="new-password">Nova Senha</label>
            <input
              id="new-password"
              v-model="newPassword"
              type="password"
              placeholder="Digite sua nova senha"
              required
              :disabled="isLoading"
            />
          </div>
          
          <div class="form-group">
            <label for="confirm-password">Confirmar Nova Senha</label>
            <input
              id="confirm-password"
              v-model="confirmPassword"
              type="password"
              placeholder="Confirme sua nova senha"
              required
              :disabled="isLoading"
            />
          </div>

          <button
            type="submit"
            :disabled="isLoading"
            class="submit-btn"
          >
            {{ isLoading ? 'Redefinindo...' : 'Redefinir Senha' }}
          </button>

          <div class="toggle-mode">
            <button
              type="button"
              @click="goToLogin"
              class="link-btn"
            >
              Voltar para Login
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useToast } from 'vue-toast-notification'
import { createClient } from '@supabase/supabase-js'

const toast = useToast()
const router = useRouter()
const route = useRoute()

const newPassword = ref('')
const confirmPassword = ref('')
const isLoading = ref(false)
const linkExpired = ref(false)

const supabaseUrl = import.meta.env.VITE_SUPABASE_URL
const supabaseAnonKey = import.meta.env.VITE_SUPABASE_ANON_KEY
const supabase = createClient(supabaseUrl, supabaseAnonKey)

onMounted(async () => {
  console.log('URL atual:', window.location.href)
  console.log('Hash:', route.hash)
  console.log('Query:', route.query)

  // Verifica se há erro na URL (link expirado)
  if (route.hash && route.hash.includes('error=')) {
    linkExpired.value = true
    return
  }

  // Escuta o evento de PASSWORD_RECOVERY do Supabase
  const { data: { subscription } } = supabase.auth.onAuthStateChange(async (event, session) => {
    console.log('Auth event:', event, session)
    
    if (event === 'PASSWORD_RECOVERY') {
      console.log('Password recovery event detected')
      // Sessão já foi configurada automaticamente pelo Supabase
    } else if (event === 'SIGNED_IN' && session) {
      console.log('User signed in with session')
    }
  })

  // Parse dos tokens do hash fragment (fallback)
  const hashParams = new URLSearchParams(route.hash.substring(1))
  const accessToken = hashParams.get('access_token')
  const refreshToken = hashParams.get('refresh_token')
  
  console.log('Hash tokens:', { accessToken: !!accessToken, refreshToken: !!refreshToken })

  if (accessToken && refreshToken) {
    try {
      const { error } = await supabase.auth.setSession({
        access_token: accessToken,
        refresh_token: refreshToken
      })
      
      if (error) {
        console.error('Erro ao definir sessão:', error)
        toast.error('Erro ao inicializar sessão de redefinição de senha.')
        linkExpired.value = true
      } else {
        console.log('Sessão definida com sucesso via hash')
      }
    } catch (err) {
      console.error('Erro inesperado:', err)
      toast.error('Erro inesperado ao processar link.')
      linkExpired.value = true
    }
  }

  // Limpa a subscription quando o componente for desmontado
  onUnmounted(() => {
    subscription.unsubscribe()
  })
})

const handleSubmit = async () => {
  if (!newPassword.value || !confirmPassword.value) {
    toast.error('Preencha todos os campos.')
    return
  }

  if (newPassword.value !== confirmPassword.value) {
    toast.error('As senhas não coincidem.')
    return
  }

  if (newPassword.value.length < 6) {
    toast.error('A senha deve ter pelo menos 6 caracteres.')
    return
  }

  try {
    isLoading.value = true
    
    const { error } = await supabase.auth.updateUser({
      password: newPassword.value
    })

    if (error) {
      console.error('Erro ao atualizar senha:', error)
      toast.error(error.message)
      return
    }

    toast.success('Senha redefinida com sucesso! Faça login novamente.')
    
    // Faz logout para limpar a sessão temporária
    await supabase.auth.signOut()
    
    router.push('/login')
  } catch (err: any) {
    console.error('Erro inesperado:', err)
    toast.error(err?.message || 'Erro inesperado ao redefinir senha.')
  } finally {
    isLoading.value = false
  }
}

const goToLogin = () => {
  router.push('/login')
}
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 40px;
}

.login-card {
  background: white;
  padding: 50px;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
  width: 50%;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 40px;
}

.logo-section {
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border-right: 1px solid #eee;
  padding-right: 20px;
}

.logo-section img {
  margin-bottom: 20px;
  width: 150px;
  height: 140px;
}

.logo-section h1 {
  margin: 0;
  color: #333;
  font-weight: 600;
  font-size: 24px;
}

.logo-section p {
  margin: 10px 0 0 0;
  color: #666;
  font-size: 14px;
}

.login-form {
  display: flex;
  flex-direction: column;
  justify-content: center;
  padding-left: 20px;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-group label {
  font-weight: 500;
  color: #444;
  font-size: 15px;
}

.form-group input {
  padding: 12px;
  border: 2px solid #e1e5e9;
  border-radius: 6px;
  font-size: 15px;
  transition: border-color 0.2s;
}

.form-group input:focus {
  outline: none;
  border-color: #667eea;
}

.form-group input:disabled {
  background-color: #f8f9fa;
  color: #6c757d;
}

.error-message {
  background-color: #f8d7da;
  color: #721c24;
  padding: 20px;
  border-radius: 6px;
  border: 1px solid #f5c6cb;
  font-size: 14px;
}

.error-message h3 {
  margin-top: 0;
  margin-bottom: 10px;
  font-weight: 600;
}

.error-message p {
  margin-bottom: 15px;
}

.submit-btn {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 14px;
  border-radius: 6px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: opacity 0.2s;
}

.submit-btn:hover:not(:disabled) {
  opacity: 0.9;
}

.submit-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.toggle-mode {
  text-align: center;
  color: #555;
  font-size: 14px;
  margin-top: 10px;
}

.link-btn {
  background: none;
  border: none;
  color: #667eea;
  cursor: pointer;
  text-decoration: underline;
  margin-left: 5px;
  font-size: 14px;
}

.link-btn:hover {
  color: #764ba2;
}
</style>