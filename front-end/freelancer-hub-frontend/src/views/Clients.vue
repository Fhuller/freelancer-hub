<script setup lang="ts">
import { ref, onMounted } from 'vue';
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue';
import ContentCard from '../components/ContentCard.vue';
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

function abrirCard() {
  console.log('clicou no conteúdo');
}

function editar() {
  console.log('editar item');
}

function excluir() {
  console.log('excluir item');
}

onMounted(() => {
  loadClients();
});
</script>

<template>
  <AuthenticatedLayout>
    <AddCard label="Novo Cliente" :onClick="novoItem" />
    <ContentCard
      label="Projeto A"
      :onMainClick="abrirCard"
      :onEdit="editar"
      :onDelete="excluir"
    />
  </AuthenticatedLayout>
</template>

<style scoped></style>