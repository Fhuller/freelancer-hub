<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue';
import { fetchUsers } from '../services/users';

const router = useRouter();
const users = ref<any[]>([]);
const isLoadingUsers = ref(false);
const error = ref('');

const loadUsers = async () => {
  try {
    isLoadingUsers.value = true;
    error.value = '';
    users.value = await fetchUsers();
  } catch (err) {
    error.value = 'Erro ao carregar usuários';
    console.error('Erro ao carregar usuários:', err);
  } finally {
    isLoadingUsers.value = false;
  }
};

const goToClient = (clientId: number) => {
  router.push(`/clients/${clientId}`);
};

onMounted(() => {
  loadUsers();
});
</script>

<template>
  <AuthenticatedLayout>
    <div class="content-section">
      <div class="section-header">
        <h2>Clientes</h2>
        <button 
          @click="goToClient(1)" 
          class="test-btn"
        >
          Ver Cliente #1 (Teste)
        </button>
      </div>
      
      <p class="section-description">
        Gerencie seus clientes e projetos de forma organizada.
      </p>
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
  margin: 0;
  font-size: 16px;
}

.test-btn {
  background-color: #7c3aed;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s ease;
  font-size: 14px;
}

.test-btn:hover {
  background-color: #6d28d9;
}

@media (max-width: 768px) {
  .section-header {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }
  
  .test-btn {
    width: 100%;
  }
}
</style>