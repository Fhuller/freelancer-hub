<script setup lang="ts">
import { ref, onMounted } from 'vue';
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue';
import ContentCard from '../components/ContentCard.vue';
import AddCard from '../components/AddCard.vue';
import BaseModal from '../components/BaseModal.vue';
import { fetchClients, createClient, deleteClient, type ClientReadDto, type ClientCreateDto } from '../services/clients';

const clients = ref<ClientReadDto[]>([]);
const isLoadingClients = ref(false);
const error = ref('');
const showModal = ref(false);

const clientTemplate: ClientCreateDto = {
  name: '',
  email: '',
  phone: undefined,
  company: undefined
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

function abrirModalNovoCliente() {
  showModal.value = true;
}

function abrirCard(cliente: ClientReadDto) {
  console.log('clicou no conteúdo', cliente);
}

function editar(cliente: ClientReadDto) {
  console.log('editar item', cliente);
}

function excluir(cliente: ClientReadDto) {
  console.log('excluir item', cliente);
}

async function saveNewClient(data: Record<string, any>) {
  const dto: ClientCreateDto = {
    name: data.name,
    email: data.email,
    phone: data.phone || undefined,
    company: data.company || undefined
  };
  await createClient(dto);
  await loadClients();
}
onMounted(() => {
  loadClients();
});
</script>

<template>
  <AuthenticatedLayout>
    <AddCard label="Novo Cliente" :onClick="abrirModalNovoCliente" />

    <div v-if="isLoadingClients">Carregando clientes...</div>
    <div v-else-if="error">{{ error }}</div>

    <ContentCard
      v-for="cliente in clients"
      :key="cliente.id"
      :label="cliente.name"
      :onMainClick="() => abrirCard(cliente)"
      :onEdit="() => editar(cliente)"
      :onDelete="() => excluir(cliente)"
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