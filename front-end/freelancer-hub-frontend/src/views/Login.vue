<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useToast} from 'vue-toast-notification'

const toast = useToast()
const router = useRouter()
const authStore = useAuthStore()

const isRegisterMode = ref(false)
const isForgotMode = ref(false)

const email = ref('')
const password = ref('')
const confirmPassword = ref('')

const handleSubmit = async () => {
  if (isForgotMode.value) {
    if (!email.value) {
      toast.error('Digite seu e-mail para redefinir a senha')
      return
    }
    const success = await authStore.resetPassword(email.value)
    if (success) {
      isForgotMode.value = false
      email.value = ''
    }
    return
  }

  if (isRegisterMode.value) {
    if (password.value !== confirmPassword.value) {
      authStore.error = 'As senhas não coincidem'
      return
    }
    
    const success = await authStore.register(email.value, password.value)
    if (success) {
      isRegisterMode.value = false
      email.value = ''
      password.value = ''
      confirmPassword.value = ''
    }
  } else {
    const success = await authStore.login(email.value, password.value)
    if (success) {
      router.push('/dashboard')
    }
  }
}

const toggleMode = () => {
  isRegisterMode.value = !isRegisterMode.value
  isForgotMode.value = false
  authStore.clearError()
  email.value = ''
  password.value = ''
  confirmPassword.value = ''
}

const toggleForgotMode = () => {
  isForgotMode.value = !isForgotMode.value
  isRegisterMode.value = false
  authStore.clearError()
  email.value = ''
  password.value = ''
  confirmPassword.value = ''
}
</script>

<template>
  <div class="login-container">
    <div class="login-card">
      <div class="logo-section">
        <img alt="Freelancer Icon" src="@/assets/logo_icon.png"/>
        <h1>
          {{ isRegisterMode ? 'Criar Conta' : (isForgotMode ? 'Redefinir Senha' : 'Entrar') }}
        </h1>
      </div>

      <form @submit.prevent="handleSubmit" class="login-form">
        <div class="form-group">
          <label for="email">E-mail</label>
          <input 
            id="email"
            v-model="email" 
            type="email" 
            required
            :disabled="authStore.isLoading"
            placeholder="Digite seu e-mail"
          />
        </div>

        <div v-if="!isForgotMode" class="form-group">
          <label for="password">Senha</label>
          <input 
            id="password"
            v-model="password" 
            type="password" 
            required
            :disabled="authStore.isLoading"
            placeholder="Digite sua senha"
          />
        </div>

        <div v-if="isRegisterMode" class="form-group">
          <label for="confirmPassword">Confirmar Senha</label>
          <input 
            id="confirmPassword"
            v-model="confirmPassword" 
            type="password" 
            required
            :disabled="authStore.isLoading"
            placeholder="Confirme sua senha"
          />
        </div>

        <div v-if="authStore.error" class="error-message">
          {{ authStore.error }}
        </div>

        <button 
          type="submit" 
          class="submit-btn"
          :disabled="authStore.isLoading"
        >
          {{ authStore.isLoading 
              ? 'Aguarde...' 
              : (isRegisterMode 
                  ? 'Criar Conta' 
                  : (isForgotMode ? 'Enviar Link' : 'Entrar')) 
          }}
        </button>

        <div v-if="!isForgotMode" class="forgot-password">
          <button type="button" class="link-btn" @click="toggleForgotMode">
            Esqueci a senha
          </button>
        </div>

        <div class="toggle-mode">
          <template v-if="isRegisterMode">
            Já tem uma conta?
            <button type="button" @click="toggleMode" class="link-btn">
              Fazer Login
            </button>
          </template>
          <template v-else-if="isForgotMode">
            Lembrou da senha?
            <button type="button" @click="toggleForgotMode" class="link-btn">
              Fazer Login
            </button>
          </template>
          <template v-else>
            Não tem uma conta?
            <button type="button" @click="toggleMode" class="link-btn">
              Criar Conta
            </button>
          </template>
        </div>
      </form>
    </div>
  </div>
</template>

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

.login-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
  padding-left: 20px;
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
  padding: 10px;
  border-radius: 6px;
  border: 1px solid #f5c6cb;
  font-size: 14px;
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

.forgot-password {
  text-align: right;
  margin-top: -10px;
}

.toggle-mode {
  text-align: center;
  color: #555;
  font-size: 14px;
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
