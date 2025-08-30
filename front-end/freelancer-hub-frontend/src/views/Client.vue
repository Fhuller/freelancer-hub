<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue';

// Props recebidas da rota
const props = defineProps<{
  id: string
}>();

const router = useRouter();
const clientData = ref<any>(null);
const isLoading = ref(false);

// Simular carregamento de dados do cliente
const loadClient = async () => {
  try {
    isLoading.value = true;
    // Aqui você faria a chamada real para sua API
    // const client = await fetchClient(props.id);
    
    // Dados mockados para teste
    clientData.value = {
      id: props.id,
      name: 'Cliente Teste',
      email: 'cliente@teste.com',
      phone: '(11) 99999-9999',
      company: 'Empresa Teste Ltda',
      status: 'Ativo'
    };
  } catch (error) {
    console.error('Erro ao carregar cliente:', error);
  } finally {
    isLoading.value = false;
  }
};

const goBackToClients = () => {
  router.push('/clients');
};

onMounted(() => {
  loadClient();
});
</script>

<template>
  <AuthenticatedLayout>
    <div class="content-section">
      <div class="section-header">
        <div class="header-left">
          <button @click="goBackToClients" class="back-btn">
            ← Voltar para Clientes
          </button>
          <h2 v-if="clientData">Cliente: {{ clientData.name }}</h2>
          <h2 v-else>Carregando cliente...</h2>
        </div>
      </div>

      <div v-if="isLoading" class="loading-state">
        <div class="spinner"></div>
        <p>Carregando dados do cliente...</p>
      </div>

      <div v-else-if="clientData" class="client-details">
        <div class="detail-card">
          <h3>Informações Básicas</h3>
          <div class="detail-row">
            <span class="label">ID:</span>
            <span class="value">{{ clientData.id }}</span>
          </div>
          <div class="detail-row">
            <span class="label">Nome:</span>
            <span class="value">{{ clientData.name }}</span>
          </div>
          <div class="detail-row">
            <span class="label">Email:</span>
            <span class="value">{{ clientData.email }}</span>
          </div>
          <div class="detail-row">
            <span class="label">Telefone:</span>
            <span class="value">{{ clientData.phone }}</span>
          </div>
          <div class="detail-row">
            <span class="label">Empresa:</span>
            <span class="value">{{ clientData.company }}</span>
          </div>
          <div class="detail-row">
            <span class="label">Status:</span>
            <span class="value status-active">{{ clientData.status }}</span>
          </div>
        </div>
      </div>
    </div>
  </AuthenticatedLayout>
</template>

<style scoped>
.content-section {
  background: white;
  border-radius: 8px;
  padding: 30px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.section-header {
  margin-bottom: 30px;
}

.header-left {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.back-btn {
  background: transparent;
  border: 1px solid #d1d5db;
  color: #6b7280;
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  align-self: flex-start;
  transition: all 0.2s ease;
}

.back-btn:hover {
  background-color: #f9fafb;
  color: #374151;
}

.section-header h2 {
  margin: 0;
  color: #333;
  font-size: 24px;
  font-weight: 600;
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
  border-top: 4px solid #7c3aed;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.client-details {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.detail-card {
  background-color: #f8f9fa;
  border: 1px solid #e9ecef;
  border-radius: 8px;
  padding: 20px;
}

.detail-card h3 {
  margin: 0 0 20px 0;
  color: #333;
  font-size: 18px;
  font-weight: 600;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 0;
  border-bottom: 1px solid #e9ecef;
}

.detail-row:last-child {
  border-bottom: none;
}

.label {
  font-weight: 500;
  color: #666;
  min-width: 100px;
}

.value {
  color: #333;
  text-align: right;
}

.status-active {
  background-color: #d4edda;
  color: #155724;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
}

@media (max-width: 768px) {
  .detail-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 5px;
  }
  
  .value {
    text-align: left;
  }
}
</style>