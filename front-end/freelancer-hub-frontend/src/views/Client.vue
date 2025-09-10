<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { fetchClientById, type ClientReadDto } from '../services/clients'
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue';

const route = useRoute()
const client = ref<ClientReadDto | null>(null)
const isLoading = ref(false)
const error = ref('')

async function loadClient() {
  try {
    isLoading.value = true
    error.value = ''
    client.value = await fetchClientById(route.params.id as string)
  } catch (err) {
    console.error(err)
    error.value = 'Erro ao carregar cliente'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadClient()
})
</script>

<template>
  <AuthenticatedLayout :loading="isLoading">
    <div v-if="error">{{ error }}</div>
    <pre v-else>{{ client ? JSON.stringify(client, null, 2) : '{}' }}</pre>
  </AuthenticatedLayout>
</template>
