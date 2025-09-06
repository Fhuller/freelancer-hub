<script setup lang="ts">
import { ref } from 'vue'
import { signUp, signIn, getSession } from '../services/supabase'

// Campos separados para registro
const signUpEmail = ref('')
const signUpPassword = ref('')

// Campos separados para login
const signInEmail = ref('')
const signInPassword = ref('')

// Mensagens separadas para cada ação
const signUpMessage = ref('')
const signInMessage = ref('')
const userMessage = ref('')

// Estados de loading
const isSigningUp = ref(false)
const isSigningIn = ref(false)
const isLoadingUser = ref(false)

const handleSignUp = async () => {
  if (!signUpEmail.value || !signUpPassword.value) {
    signUpMessage.value = 'Por favor, preencha todos os campos.'
    return
  }
  
  isSigningUp.value = true
  signUpMessage.value = ''
  
  try {
    const { data, error } = await signUp(signUpEmail.value, signUpPassword.value)
    if (error) {
      signUpMessage.value = error.message
    } else {
      signUpMessage.value = 'Usuário registrado! Confira o e-mail para confirmação.'
      // Limpar campos após sucesso
      signUpEmail.value = ''
      signUpPassword.value = ''
    }
  } catch (err) {
    signUpMessage.value = 'Erro inesperado ao registrar usuário.'
  } finally {
    isSigningUp.value = false
  }
}

const handleSignIn = async () => {
  if (!signInEmail.value || !signInPassword.value) {
    signInMessage.value = 'Por favor, preencha todos os campos.'
    return
  }
  
  isSigningIn.value = true
  signInMessage.value = ''
  
  try {
    const { data, error } = await signIn(signInEmail.value, signInPassword.value)
    if (error) {
      signInMessage.value = error.message
    } else {
      signInMessage.value = 'Login realizado com sucesso!'
      // Limpar campos após sucesso
      signInEmail.value = ''
      signInPassword.value = ''
    }
  } catch (err) {
    signInMessage.value = 'Erro inesperado ao fazer login.'
  } finally {
    isSigningIn.value = false
  }
}

const handleGetUser = async () => {
  isLoadingUser.value = true
  userMessage.value = ''
  
  try {
    const { data } = await getSession()
    if (data.session) {
      userMessage.value = JSON.stringify(data.session, null, 2)
    } else {
      userMessage.value = 'Nenhum usuário logado.'
    }
  } catch (err) {
    userMessage.value = 'Erro ao verificar usuário.'
  } finally {
    isLoadingUser.value = false
  }
}

// Função para limpar mensagens
const clearMessages = () => {
  signUpMessage.value = ''
  signInMessage.value = ''
  userMessage.value = ''
}
</script>

<template>
  <div class="auth-container">
    <div class="auth-section">
      <h2>Registro</h2>
      <div class="form-group">
        <input 
          v-model="signUpEmail" 
          placeholder="Email para registro" 
          type="email"
          :disabled="isSigningUp"
        />
        <input 
          v-model="signUpPassword" 
          placeholder="Senha para registro" 
          type="password"
          :disabled="isSigningUp"
        />
        <button 
          @click="handleSignUp" 
          :disabled="isSigningUp || !signUpEmail || !signUpPassword"
        >
          {{ isSigningUp ? 'Registrando...' : 'Registrar' }}
        </button>
      </div>
      <div v-if="signUpMessage" class="message" :class="{ success: signUpMessage.includes('registrado') }">
        {{ signUpMessage }}
      </div>
    </div>

    <div class="auth-section">
      <h2>Login</h2>
      <div class="form-group">
        <input 
          v-model="signInEmail" 
          placeholder="Email para login" 
          type="email"
          :disabled="isSigningIn"
        />
        <input 
          v-model="signInPassword" 
          placeholder="Senha para login" 
          type="password"
          :disabled="isSigningIn"
        />
        <button 
          @click="handleSignIn" 
          :disabled="isSigningIn || !signInEmail || !signInPassword"
        >
          {{ isSigningIn ? 'Fazendo login...' : 'Login' }}
        </button>
      </div>
      <div v-if="signInMessage" class="message" :class="{ success: signInMessage.includes('sucesso') }">
        {{ signInMessage }}
      </div>
    </div>

    <div class="auth-section">
      <h2>Usuário Atual</h2>
      <button 
        @click="handleGetUser" 
        :disabled="isLoadingUser"
      >
        {{ isLoadingUser ? 'Verificando...' : 'Verificar usuário' }}
      </button>
      <button 
        @click="clearMessages" 
        class="clear-btn"
        v-if="signUpMessage || signInMessage || userMessage"
      >
        Limpar mensagens
      </button>
      <pre v-if="userMessage" class="user-info">{{ userMessage }}</pre>
    </div>
  </div>
</template>

<style scoped>
.auth-container {
  max-width: 1200px;       /* largura adequada para desktop */
  margin: 40px auto;       /* centraliza o conteúdo */
  padding: 20px;
  display: grid;           /* usa grid layout */
  grid-template-columns: repeat(3, 1fr); /* 3 colunas iguais */
  gap: 20px;
}

.auth-section {
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 8px;
  background-color: #f9f9f9;
  box-shadow: var(--shadow-default); /* leve sombra para estilo desktop */
  display: flex;
  flex-direction: column;
  justify-content: flex-start;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

input {
  padding: 12px;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 15px;
}

input:disabled {
  background-color: #f5f5f5;
  color: #999;
}

button {
  padding: 12px 18px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 15px;
  transition: background-color 0.2s;
}

button:hover:not(:disabled) {
  background-color: #0056b3;
}

button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.clear-btn {
  background-color: #6c757d;
  margin-top: 10px;
}

.clear-btn:hover {
  background-color: #545b62;
}

.message {
  margin-top: 12px;
  padding: 12px;
  border-radius: 6px;
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
  font-size: 14px;
}

.message.success {
  background-color: #d4edda;
  color: #155724;
  border-color: #c3e6cb;
}

.user-info {
  margin-top: 12px;
  padding: 12px;
  background-color: #f8f9fa;
  border: 1px solid #dee2e6;
  border-radius: 6px;
  font-size: 13px;
  max-height: 250px;
  overflow-y: auto;
  color: black;
  white-space: pre-wrap; /* mantém a formatação do JSON */
}

h2 {
  margin-top: 0;
  margin-bottom: 15px;
  color: #333;
  font-size: 20px;
}
</style>
