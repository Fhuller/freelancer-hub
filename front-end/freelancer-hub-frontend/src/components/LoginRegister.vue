<script setup lang="ts">
import { ref } from 'vue'
import { signUp, signIn, getUser } from '../services/supabase'

const email = ref('')
const password = ref('')
const message = ref('')

const handleSignUp = async () => {
  const { data, error } = await signUp(email.value, password.value)
  if (error) message.value = error.message
  else message.value = 'Usuário registrado! Confira o e-mail para confirmação.'
}

const handleSignIn = async () => {
  const { data, error } = await signIn(email.value, password.value)
  if (error) message.value = error.message
  else message.value = 'Login realizado! Token gerado.'
}

const handleGetUser = async () => {
  const { data } = await getUser()
  message.value = JSON.stringify(data.user, null, 2)
}
</script>

<template>
  <div>
    <h2>Registro</h2>
    <input v-model="email" placeholder="Email" />
    <input v-model="password" placeholder="Senha" type="password" />
    <button @click="handleSignUp">Registrar</button>

    <h2>Login</h2>
    <input v-model="email" placeholder="Email" />
    <input v-model="password" placeholder="Senha" type="password" />
    <button @click="handleSignIn">Login</button>

    <h2>Usuário atual</h2>
    <button @click="handleGetUser">Verificar usuário</button>

    <pre>{{ message }}</pre>
  </div>
</template>
