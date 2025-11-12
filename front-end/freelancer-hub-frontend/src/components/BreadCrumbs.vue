<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { fetchClientById } from '../services/clients';
import { fetchProjectById } from '../services/projects';
import type { ClientReadDto } from '../services/clients';

const route = useRoute();
const router = useRouter();

const clientName = ref<string | null>(null);
const projectName = ref<string | null>(null);

const navigateTo = (path: string) => {
  router.push(path);
};

const updateClientName = async (id: string) => {
  try {
    const client: ClientReadDto = await fetchClientById(id);
    clientName.value = client.name;
  } catch {
    clientName.value = `Cliente #${id}`;
  }
};

const updateProjectName = async (id: string) => {
  try {
    const project = await fetchProjectById(id);
    projectName.value = project.title;
  } catch {
    projectName.value = `Projeto #${id}`;
  }
};

// Observa mudança de rota para atualizar cliente/projeto
watch(
  () => route.params,
  (params) => {
    if (route.name === 'Client' && typeof params.id === 'string') {
      updateClientName(params.id);
    }
    if (route.name === 'ClientProject') {
      if (typeof params.id === 'string') {
        updateClientName(params.id);
      }
      if (typeof params.projectId === 'string') {
        updateProjectName(params.projectId);
      }
    }
  },
  { immediate: true }
);

// Computed breadcrumbs
const breadcrumbs = computed(() => {
  const currentRoute = route.name as string;
  const params = route.params;

  const items = [
    { name: 'Dashboard', path: '/app/dashboard', clickable: true }
  ];

  if (currentRoute === 'Clients') {
    items.push({ name: 'Clientes', path: '/app/clients', clickable: true });
  } else if (currentRoute === 'Client') {
    const clientId = params.id as string;
    items.push({ name: 'Clientes', path: '/app/clients', clickable: true });
    items.push({
      name: clientName.value || `Cliente #${clientId}`,
      path: `/app/clients/${clientId}`,
      clickable: false
    });
  } else if (currentRoute === 'ClientProject') {
    const clientId = params.id as string;
    const projectId = params.projectId as string;

    items.push({ name: 'Clientes', path: '/app/clients', clickable: true });
    items.push({
      name: clientName.value || `Cliente #${clientId}`,
      path: `/app/clients/${clientId}`,
      clickable: true
    });
    items.push({
      name: projectName.value || `Projeto #${projectId}`,
      path: `/app/clients/${clientId}/projects/${projectId}`,
      clickable: false
    });
  } else if (currentRoute === 'Finance') {
    items.push({ name: 'Financeiro', path: '/app/finance', clickable: true });
  }

  return items;
});
</script>

<template>
<nav class="breadcrumbs">
    <template v-for="(item, index) in breadcrumbs" :key="item.path">
        <div v-if="index > 0" class="breadcrumb-separator">/</div>
        <button
            @click="item.clickable && navigateTo(item.path)"
            class="breadcrumb-item"
            :class="{ 'current': index === breadcrumbs.length - 1 }"
        >
            {{ item.name }}
        </button>
    </template>
</nav>
</template>

<style scoped>
.breadcrumbs {
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 14px;
    color: #656d76;
}

.breadcrumb-separator {
    color: #8c959f;
    font-weight: 400;
}

.breadcrumb-item {
    background: none;
    border: none;
    color: #656d76;
    font-weight: 500;
    cursor: pointer;
    padding: 4px 8px;
    border-radius: 6px;
    font-size: 14px;
    transition: all 0.15s ease;
}

.breadcrumb-item:hover {
    background-color: #f3f4f6;
    color: #24292f;
}

.breadcrumb-item.current {
    color: var(--color-purple);
    font-weight: 600;
}
</style>