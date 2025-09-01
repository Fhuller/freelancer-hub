<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue';
import ClientCard from '../components/ClientCard.vue';
import { fetchClients, deleteClient, type ClientReadDto } from '../services/clients';

const router = useRouter();
const clients = ref<ClientReadDto[]>([]);
const isLoadingClients = ref(false);
const error = ref('');
const clientToDelete = ref<ClientReadDto | null>(null);
const isDeleting = ref(false);

const loadClients = async () => {
  try {
    isLoadingClients.value = true;
    error.value = '';
    clients.value = await fetchClients();
  } catch (err) {
    error.value = 'Erro ao carregar clientes';
    console.error('Erro ao carregar clientes:', err);
  } finally {
    isLoadingClients.value = false;
  }
};

const viewClient = (clientId: string) => {
  router.push(`clients/${clientId}`);
};

const editClient = (clientId: string) => {
  router.push(`clients/${clientId}/edit`);
};

const createClient = () => {
  router.push('clients/new');
};

const confirmDeleteClient = (clientId: string) => {
  const client = clients.value.find(c => c.id === clientId);
  if (client) {
    clientToDelete.value = client;
  }
};

const cancelDelete = () => {
  clientToDelete.value = null;
};

const handleDeleteClient = async () => {
  if (!clientToDelete.value) return;

  try {
    isDeleting.value = true;
    await deleteClient(clientToDelete.value.id);

    clients.value = clients.value.filter(c => c.id !== clientToDelete.value!.id);

    clientToDelete.value = null;
  } catch (err) {
    console.error('Erro ao excluir cliente:', err);
    error.value = 'Erro ao excluir cliente';
  } finally {
    isDeleting.value = false;
  }
};

onMounted(() => {
  loadClients();
});
</script>

<template>
  <AuthenticatedLayout>
    <div class="content-section">
      <div class="section-header">
        <h2>Clientes</h2>
        <button @click="createClient" class="primary-btn">
          + Novo Cliente
        </button>
      </div>

      <p class="section-description">
        Gerencie seus clientes e projetos de forma organizada.
      </p>

      <div v-if="isLoadingClients" class="loading-state">
        <div class="loading-spinner"></div>
        <p>Carregando clientes...</p>
      </div>

      <div v-else-if="error" class="error-state">
        <p class="error-message">{{ error }}</p>
        <button @click="loadClients" class="retry-btn">Tentar novamente</button>
      </div>

      <div v-else-if="clients.length === 0" class="empty-state">
        <div class="empty-icon">👥</div>
        <h3>Nenhum cliente encontrado</h3>
        <p>Comece adicionando seu primeiro cliente.</p>
        <button @click="createClient" class="primary-btn">
          + Adicionar Cliente
        </button>
      </div>

      <div v-else class="clients-grid">
        <ClientCard v-for="client in clients" :key="client.id" :client="client" @view="viewClient" @edit="editClient"
          @delete="confirmDeleteClient" />
      </div>
    </div>

    <div v-if="clientToDelete" class="modal-overlay" @click="cancelDelete">
      <div class="modal-content" @click.stop>
        <h3>Confirmar Exclusão</h3>
        <p>Tem certeza que deseja excluir o cliente <strong>{{ clientToDelete.name }}</strong>?</p>
        <p class="warning-text">Esta ação não pode ser desfeita.</p>
        <div class="modal-actions">
          <button @click="cancelDelete" class="cancel-btn">Cancelar</button>
          <button @click="handleDeleteClient" class="danger-btn" :disabled="isDeleting">
            {{ isDeleting ? 'Excluindo...' : 'Excluir' }}
          </button>
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
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.section-header h2 {
  margin: 0;
  color: #333;
  font-size: 24px;
  font-weight: 600;
}

.section-description {
  color: #666;
  margin: 0 0 30px 0;
  font-size: 16px;
}

.primary-btn {
  background-color: #7c3aed;
  color: white;
  border: none;
  padding: 12px 20px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s ease;
  font-size: 14px;
}

.primary-btn:hover {
  background-color: #6d28d9;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
  color: #6b7280;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 3px solid #f3f4f6;
  border-top: 3px solid #7c3aed;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
}

.error-message {
  color: #dc2626;
  margin-bottom: 16px;
  font-size: 16px;
}

.retry-btn {
  background-color: #f3f4f6;
  color: #374151;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s ease;
}

.retry-btn:hover {
  background-color: #e5e7eb;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 20px;
  text-align: center;
}

.empty-icon {
  font-size: 64px;
  margin-bottom: 20px;
  opacity: 0.5;
}

.empty-state h3 {
  margin: 0 0 8px 0;
  color: #374151;
  font-size: 20px;
  font-weight: 600;
}

.empty-state p {
  margin: 0 0 24px 0;
  color: #6b7280;
}

.clients-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 20px;
}

.modal-content {
  background: white;
  border-radius: 12px;
  padding: 24px;
  max-width: 400px;
  width: 100%;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
}

.modal-content h3 {
  margin: 0 0 16px 0;
  color: #111827;
  font-size: 18px;
  font-weight: 600;
}

.modal-content p {
  margin: 0 0 8px 0;
  color: #374151;
  line-height: 1.5;
}

.warning-text {
  color: #dc2626 !important;
  font-size: 14px;
  margin-bottom: 24px !important;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
}

.cancel-btn {
  background-color: #f3f4f6;
  color: #374151;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s ease;
}

.cancel-btn:hover {
  background-color: #e5e7eb;
}

.danger-btn {
  background-color: #dc2626;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s ease;
}

.danger-btn:hover:not(:disabled) {
  background-color: #b91c1c;
}

.danger-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

@media (max-width: 768px) {
  .section-header {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }

  .primary-btn {
    width: 100%;
  }

  .clients-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }

  .modal-content {
    margin: 20px;
  }

  .modal-actions {
    flex-direction: column;
  }

  .cancel-btn,
  .danger-btn {
    width: 100%;
  }
}
</style>