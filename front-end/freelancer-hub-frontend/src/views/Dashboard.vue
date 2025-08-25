<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '../stores/auth'
import { fetchUsers } from '../services/users'

const authStore = useAuthStore()
const users = ref<any[]>([])
const isLoadingUsers = ref(false)
const error = ref('')

const loadUsers = async () => {
  try {
    isLoadingUsers.value = true
    error.value = ''
    
    // Aqui você pode passar o token se necessário
    const token = authStore.accessToken
    users.value = await fetchUsers(token)
  } catch (err) {
    error.value = 'Erro ao carregar usuários'
    console.error('Erro ao carregar usuários:', err)
  } finally {
    isLoadingUsers.value = false
  }
}

onMounted(() => {
  loadUsers()
})

const handleLogout = async () => {
  await authStore.logout()
  // O router guard irá redirecionar para /login automaticamente
}
</script>

<template>
  <div class="dashboard-container">
    <header class="dashboard-header">
      <div class="header-content">
        <div class="logo-section">
          <img alt="Vue logo" src="@/assets/logo.svg" width="40" height="40" />
          <h1>Dashboard</h1>
        </div>
        
        <div class="user-section">
          <span class="welcome-text">
            Olá, {{ authStore.session?.user.email }}!
          </span>
          <button @click="handleLogout" class="logout-btn">
            Sair
          </button>
        </div>
      </div>
    </header>

    <main class="dashboard-main">
      <div class="content-section">
        <div class="section-header">
          <h2>Dados da API</h2>
          <button 
            @click="loadUsers" 
            :disabled="isLoadingUsers"
            class="refresh-btn"
          >
            {{ isLoadingUsers ? 'Carregando...' : 'Atualizar' }}
          </button>
        </div>

        <div v-if="error" class="error-message">
          {{ error }}
        </div>

        <div v-else-if="isLoadingUsers" class="loading-state">
          <div class="spinner"></div>
          <p>Carregando dados...</p>
        </div>

        <div v-else-if="users.length > 0" class="data-section">
          <div class="data-grid">
            <div 
              v-for="(user, index) in users" 
              :key="index"
              class="data-card"
            >
              <pre>{{ JSON.stringify(user, null, 2) }}</pre>
            </div>
          </div>
        </div>

        <div v-else class="empty-state">
          <p>Nenhum dado encontrado.</p>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
.dashboard-container {
  min-height: 100vh;
  background-color: #f8f9fa;
}

.dashboard-header {
  background: white;
  border-bottom: 1px solid #dee2e6;
  padding: 0 20px;
}

.header-content {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 0;
}

.logo-section {
  display: flex;
  align-items: center;
  gap: 15px;
}

.logo-section h1 {
  margin: 0;
  color: #333;
  font-weight: 600;
}

.user-section {
  display: flex;
  align-items: center;
  gap: 15px;
}

.welcome-text {
  color: #666;
  font-weight: 500;
}

.logout-btn {
  background-color: #dc3545;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s;
}

.logout-btn:hover {
  background-color: #c82333;
}

.dashboard-main {
  max-width: 1200px;
  margin: 0 auto;
  padding: 30px 20px;
}

.content-section {
  background: white;
  border-radius: 8px;
  padding: 30px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
}

.section-header h2 {
  margin: 0;
  color: #333;
}

.refresh-btn {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s;
}

.refresh-btn:hover:not(:disabled) {
  background-color: #0056b3;
}

.refresh-btn:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.error-message {
  background-color: #f8d7da;
  color: #721c24;
  padding: 15px;
  border-radius: 6px;
  border: 1px solid #f5c6cb;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 15px;
  padding: 40px;
  color: #666;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #007bff;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.data-grid {
  display: grid;
  gap: 20px;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
}

.data-card {
  background-color: #f8f9fa;
  border: 1px solid #dee2e6;
  border-radius: 6px;
  padding: 15px;
}

.data-card pre {
  margin: 0;
  font-size: 12px;
  color: #333;
  white-space: pre-wrap;
  word-break: break-word;
}

.empty-state {
  text-align: center;
  padding: 40px;
  color: #666;
}

@media (max-width: 768px) {
  .header-content {
    flex-direction: column;
    gap: 15px;
  }

  .section-header {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }

  .data-grid {
    grid-template-columns: 1fr;
  }
}
</style>