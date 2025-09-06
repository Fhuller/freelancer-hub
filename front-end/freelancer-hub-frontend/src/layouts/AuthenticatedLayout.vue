<script setup lang="ts">
import { ref } from 'vue'; 
import { useAuthStore } from '../stores/auth';
import Header from '../components/Header.vue';
import Sidebar from '../components/Sidebar.vue';
import ContentSection from '../components/ContentSection.vue';

const authStore = useAuthStore();
const isSidebarOpen = ref(false); 

function toggleSidebar() {
  isSidebarOpen.value = !isSidebarOpen.value;
}
</script>

<template>
  <div class="authenticated-layout">
    <Header @toggle-sidebar="toggleSidebar" />
    
    <Sidebar :is-open="isSidebarOpen" @close="toggleSidebar" />

    <ContentSection>
      <slot />
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
</style>