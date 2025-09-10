<script setup lang="ts">
import { ref } from 'vue'; 
import { useAuthStore } from '../stores/auth';
import Header from '../components/Header.vue';
import Sidebar from '../components/Sidebar.vue';
import ContentSection from '../components/ContentSection.vue';

const authStore = useAuthStore();
const isSidebarOpen = ref(false); 
const props = defineProps<{ loading?: boolean }>();

function toggleSidebar() {
  isSidebarOpen.value = !isSidebarOpen.value;
}
</script>

<template>
  <div class="authenticated-layout">
    <Header @toggle-sidebar="toggleSidebar" />
    <Sidebar :is-open="isSidebarOpen" @close="toggleSidebar" />

    <ContentSection>
      <template v-if="!loading">
        <slot />
      </template>
      <template v-else>
        <div class="loading-container">
          <img src="@/assets/loading.gif" alt="Carregando..." class="loading-image" />
        </div>
      </template>
    </ContentSection>
  </div>
</template>

<style scoped>
.authenticated-layout {
  min-height: 100vh;
  background-color: #f8f9fa;
  display: flex;
  flex-direction: column;
}

.loading-placeholder {
  width: 100%;
  text-align: center;
  padding: 40px;
  color: #6c757d;
}

.loading-container{
  width: 100%;
  grid-column: 1/-1;
  display: flex;
  align-items: center;
  justify-content: center;
}

.loading-image {
  width: 80px; 
  height: 80px;
}
</style>