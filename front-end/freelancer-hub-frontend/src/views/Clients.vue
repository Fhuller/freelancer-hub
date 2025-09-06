<script setup lang="ts">
import { ref, onMounted } from 'vue';
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue';
import ClientCard from '../components/ClientCard.vue';
import AddCard from '../components/AddCard.vue';
import { fetchClients, deleteClient, type ClientReadDto } from '../services/clients';

const clients = ref<ClientReadDto[]>([]);
const isLoadingClients = ref(false);
const error = ref('');

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

function novoItem() {
  alert('Cliquei no card de adicionar!');
}

onMounted(() => {
  loadClients();
});
</script>

<template>
  <AuthenticatedLayout>
    <AddCard label="Novo Cliente" :onClick="novoItem" />
  </AuthenticatedLayout>
</template>

<style scoped>


</style>