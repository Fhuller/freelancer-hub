<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useToast } from 'vue-toast-notification';

import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue';
import ContentCard from '../components/ContentCard.vue';
import AddCard from '../components/AddCard.vue';
import BaseModal from '../components/BaseModal.vue';
import { fetchClients, createClient, deleteClient, updateClient, type ClientReadDto, type ClientCreateDto, type ClientUpdateDto } from '../services/clients';

const { t } = useI18n();
const toast = useToast();

const clients = ref<ClientReadDto[]>([]);
const editingClient = ref<ClientReadDto | null>(null);
const isLoadingClients = ref(false);
const error = ref('');
const showModal = ref(false);
const router = useRouter();

const clientTemplate: ClientCreateDto = {
  name: '',
  email: '',
  phone: undefined,
  companyName: undefined,
  notes: ''
};

function getclientTemplateDisplay() {
  return {
    name: t('clientNamePlaceholder'),
    email: t('clientEmailPlaceholder'),
    phone: t('clientPhonePlaceholder'),
    companyName: t('clientCompanyPlaceholder'),
    notes: t('clientNotesPlaceholder')
  }
}

async function loadClients() {
  try {
    isLoadingClients.value = true;
    error.value = '';
    clients.value = await fetchClients();
  } catch (err) {
    console.error('Erro ao carregar clientes:', err);
    error.value = t('errorLoadingClients');
  } finally {
    isLoadingClients.value = false;
  }
}

function openNewClientModal() {
  clientTemplate.name = '';
  clientTemplate.email = '';
  clientTemplate.phone = undefined;
  clientTemplate.companyName = undefined;
  clientTemplate.notes = '';

  editingClient.value = null;
  showModal.value = true;
}

function openClientDetails(client: ClientReadDto) {
  router.push({ name: 'Client', params: { id: client.id } });
}

function editClient(client: ClientReadDto) {
  editingClient.value = client;
  clientTemplate.name = client.name;
  clientTemplate.email = client.email;
  clientTemplate.phone = client.phone || undefined;
  clientTemplate.companyName = client.companyName || undefined;
  clientTemplate.notes = client.notes;

  showModal.value = true;
}

async function removeClient(client: ClientReadDto) {
  await deleteClient(client.id);
  toast.success(t('clientDeleted'));
  await loadClients();
}

async function saveNewClient(data: Record<string, any>) {
  if (editingClient.value) {
    const dto: ClientUpdateDto = {
      name: data.name,
      email: data.email,
      phone: data.phone || undefined,
      companyName: data.companyName || undefined,
      notes: data.notes
    };
    await updateClient(editingClient.value.id, dto);
    toast.success(t('clientUpdated'));
    editingClient.value = null;
  } else {
    const dto: ClientCreateDto = {
      name: data.name,
      email: data.email,
      phone: data.phone || undefined,
      companyName: data.companyName || undefined,
      notes: data.notes
    };
    await createClient(dto);
    toast.success(t('clientCreated'));
  }

  await loadClients();

  clientTemplate.name = '';
  clientTemplate.email = '';
  clientTemplate.phone = undefined;
  clientTemplate.companyName = undefined;
  clientTemplate.notes = '';
  showModal.value = false;
}

onMounted(() => {
  loadClients();
});
</script>

<template>
  <AuthenticatedLayout :loading="isLoadingClients">
    <AddCard :label="t('ClientCreateDto')" :onClick="openNewClientModal" />

    <div v-if="isLoadingClients">{{ t('loadingClients') }}</div>
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
      :model-values="clientTemplate"
      :model-display="getclientTemplateDisplay()"
      :onSave="saveNewClient"
      :model-name="'client'"
      @close="showModal = false"
    />
  </AuthenticatedLayout>
</template>

<style scoped>
</style>