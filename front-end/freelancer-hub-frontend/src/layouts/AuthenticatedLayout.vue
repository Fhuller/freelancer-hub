<script setup lang="ts">
import { ref } from 'vue'; 
import Header from '../components/Header.vue';
import Sidebar from '../components/Sidebar.vue'; 

const isSidebarOpen = ref(false); 

function toggleSidebar() {
  isSidebarOpen.value = !isSidebarOpen.value;
}
</script>

<template>
  <div class="authenticated-layout">
    <Header @toggle-sidebar="toggleSidebar" />
    
    <Sidebar :is-open="isSidebarOpen" @close="toggleSidebar" />

    <main class="content-wrapper">
      <slot />
    </main>
  </div>
</template>

<style scoped>
.authenticated-layout {
  min-height: 100vh;
  background-color: #f8f9fa;
  display: flex;
  flex-direction: column;
}

.content-wrapper {
  flex-grow: 1;
  max-width: 1200px;
  margin: 0 auto;
  padding: 30px 20px;
  width: 100%;
}
</style>