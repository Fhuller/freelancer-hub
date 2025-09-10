<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router'
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue';
import ContentCard from '../components/ContentCard.vue';
import AddCard from '../components/AddCard.vue';
import BaseModal from '../components/BaseModal.vue';
import { fetchClients, createClient, deleteClient, updateClient, type ClientReadDto, type ClientCreateDto, type ClientUpdateDto } from '../services/clients';

const clients = ref<ClientReadDto[]>([]);
const editingClient = ref<ClientReadDto | null>(null)
const isLoadingClients = ref(false);
const error = ref('');
const showModal = ref(false);
const router = useRouter()

const clientTemplate: ClientCreateDto = {
  name: '',
  email: '',
  phone: undefined,
  companyName: undefined
};

async function loadClients() {
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
}

function openNewClientModal() {
  clientTemplate.name = ''
  clientTemplate.email = ''
  clientTemplate.phone = undefined
  clientTemplate.companyName = undefined

  editingClient.value = null
  showModal.value = true
}

function openClientDetails(client: ClientReadDto) {
  router.push({ name: 'Client', params: { id: client.id } })
}

function editClient(client: ClientReadDto) {
  editingClient.value = client
  clientTemplate.name = client.name
  clientTemplate.email = client.email
  clientTemplate.phone = client.phone || undefined
  clientTemplate.companyName = client.companyName || undefined

  showModal.value = true
}

async function removeClient(client: ClientReadDto) {
  try {
    await deleteClient(client.id)
    await loadClients()
  } catch (err) {
    console.error('Erro ao excluir cliente:', err)
    error.value = 'Erro ao excluir cliente'
  }
}

async function saveNewClient(data: Record<string, any>) {
  if (editingClient.value) {
    const dto: ClientUpdateDto = {
      name: data.name,
      email: data.email,
      phone: data.phone || undefined,
      companyName: data.companyName || undefined
    }
    await updateClient(editingClient.value.id, dto)
    editingClient.value = null
  } else {
    const dto: ClientCreateDto = {
      name: data.name,
      email: data.email,
      phone: data.phone || undefined,
      companyName: data.companyName || undefined
    }
    await createClient(dto)
  }

  await loadClients()

  clientTemplate.name = ''
  clientTemplate.email = ''
  clientTemplate.phone = undefined
  clientTemplate.companyName = undefined
  showModal.value = false
}

onMounted(() => {
  loadClients();
});
</script>

<template>
  <AuthenticatedLayout :loading="isLoadingClients">
    <AddCard label="Novo Cliente" :onClick="openNewClientModal" />

    <div v-if="isLoadingClients">Carregando clientes...</div>
    <div v-else-if="error">{{ error }}</div>

    <ContentCard
      v-for="client in clients"
      :key="client.id"
      :label="client.name"
      :onMainClick="() => openClientDetails(client)"
      :onEdit="() => editClient(client)"
      :onDelete="() => removeClient(client)"
    />

    <BaseModal
      :visible="showModal"
      :model="clientTemplate"
      :onSave="saveNewClient"
      :model-name="'client'"
      @close="showModal = false"
    />
  </AuthenticatedLayout>
</template>

<style scoped>
</style>